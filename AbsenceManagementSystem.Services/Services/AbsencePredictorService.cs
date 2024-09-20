using AbsenceManagementSystem.Core.Domain;
using AbsenceManagementSystem.Core.DTO;
using AbsenceManagementSystem.Core.Enums;
using AbsenceManagementSystem.Core.IRepositories;
using AbsenceManagementSystem.Core.IServices;
using AbsenceManagementSystem.Core.UnitOfWork;
using AbsenceManagementSystem.Infrastructure.DbContext;
using CsvHelper;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.Data;
using System.Diagnostics;
using System.Globalization;

namespace AbsenceManagementSystem.Services.Services
{
    public class AbsencePredictorService : IAbsencePredictorService
    {
        private string _modelPath = "";
        private readonly MLContext _mlContext;
        private readonly IUnitOfWork _unitOfWork;
        private ITransformer _model;
        private ITransformer _loadedModel;
        private readonly IWebHostEnvironment _webEnv;
        private readonly IEmployeeRepository _empRepo;

        public AbsencePredictorService(IUnitOfWork unitOfWork, IWebHostEnvironment webEnv, IEmployeeRepository employeeRepository)
        {
            _unitOfWork = unitOfWork;
            _mlContext = new MLContext();
            _webEnv = webEnv;
            TrainModelMain();
            _empRepo = employeeRepository;
        }

        public void TrainModelMain()
        {
            // Get the root path of the project
            string rootPath = _webEnv.ContentRootPath;

            var context = new MLContext();

            var data = context.Data.LoadFromTextFile<EmployeeLeaveData>($"{rootPath}\\Absenteeism-sample-data-raw-csv.csv", hasHeader: true, separatorChar: ',');

            var split = context.Data.TrainTestSplit(data, testFraction: 0.2);

            var features = split.TrainSet.Schema
                .Select(col => col.Name)
                .Where(colName => colName != "Label" && colName != "Status")
                .ToArray();

            var pipeline = context.Transforms.Text.FeaturizeText("Text", "Status")
                .Append(context.Transforms.Concatenate("Features", features))
                .Append(context.Transforms.Concatenate("Feature", "Features", "Text"))
                .Append(context.Regression.Trainers.LbfgsPoissonRegression());

            var model = pipeline.Fit(split.TrainSet);
            var predictions = model.Transform(split.TestSet);
            var metrics = context.Regression.Evaluate(predictions);

            // Define the path to save the model (e.g., in a "MLModels" folder within the root directory)
            _modelPath = Path.Combine(rootPath, "MLModels", "myModel.zip");

            // Ensure the directory exists
            if (!Directory.Exists(Path.Combine(rootPath, "MLModels")))
            {
                Directory.CreateDirectory(Path.Combine(rootPath, "MLModels"));
            }

            // Save the trained model to the specified path
            using (var fileStream = new FileStream(_modelPath, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                _mlContext.Model.Save(model, data.Schema, fileStream);
            }

            Console.WriteLine($"R^2 - {metrics.RSquared}");
            Console.WriteLine($"MSE - {metrics.MeanSquaredError}");
            Console.WriteLine($"Loss fn - {metrics.LossFunction}");
            Console.WriteLine($"MAE - {metrics.MeanAbsoluteError}");
        }

        public bool PredictAbsenceMain(EmployeeLeaveRequest inputData)
        {
            LoadModelMain();

            float season = GetSeason(inputData.StartDate, inputData.EndDate);

            var predictionInputData = new EmployeeLeaveRequestPredict
            {
                Age = 4,
                NumberOfDaysOff = (inputData.StartDate - inputData.EndDate).Days,
                EmploymentDuration = 2,
                TotalLeaveDaysRemaining = 4,
                LeaveType = (float)inputData.LeaveType,
                Season = season
            };

            var predictionEngine = _mlContext.Model.CreatePredictionEngine<EmployeeLeaveRequestPredict, AbsencePredictionDto>(_model);
            var prediction = predictionEngine.Predict(predictionInputData);
            return prediction.WillBeAbsent;
        }

        public async Task<List<EmployeeLeavePredictResponse>> PredictAbsences()
        {
            LoadModelMain();
            var now = DateTime.Now;

            var response = new List<EmployeeLeavePredictResponse>();
            var count = 0;

            var employeesRequests = _unitOfWork.EmployeeLeaveRequests.GetAllAsQueryable().ToList();
            var employees = (await _empRepo.GetAllEmployeesAsync()).Data;

            // Prepare a list of input data for batch prediction
            var predictionInputDataList = new List<EmployeeLeaveRequestPredict>();

            //float season = GetSeason(inputData.StartDate, inputData.EndDate);

            foreach (var req in employeesRequests)
            {
                float season = GetSeason(req.StartDate, req.EndDate);
                var empl = employees?.FirstOrDefault(x => x.EmployeeId == req.EmployeeId);
                if (empl == null)
                    continue;

                var empLeaveRecord = employeesRequests.Where(x => x.EmployeeId == empl?.EmployeeId)?.Sum(x => x.NumberOfDaysOff) ?? 0;

                var predictionInputData = new EmployeeLeaveRequestPredict
                {
                    Age = (float)Math.Round((float)(empl.DateOfBirth - now).TotalDays, 1),
                    NumberOfDaysOff = (float)Math.Round((float)(req.EndDate - req.StartDate).Days, 1),
                    EmploymentDuration = (float)Math.Round((float)(empl.DateCreated - now).TotalDays, 1),
                    TotalLeaveDaysRemaining = empl?.TotalHolidayEntitlement ?? 0 - empLeaveRecord,
                    LeaveType = (float)req.LeaveType,
                    Season = season
                };
                predictionInputDataList.Add(predictionInputData);

                var predictionEngine = _mlContext.Model.CreatePredictionEngine<EmployeeLeaveRequestPredict, AbsencePredictionDto>(_model);
                var prediction = predictionEngine.Predict(predictionInputData);
                if (prediction.WillBeAbsent)
                {
                    response.Add(new EmployeeLeavePredictResponse
                    {
                        EmployeeName = empl.FirstName + " " + empl.LastName,
                        LeaveType = 0,
                        Status = true
                    });
                    count++;
                }

                if (count == 5) break;
            }

            Console.WriteLine("Predictions complete.");
            Debug.WriteLine("Predictions complete.");

            return response;
        }

        private void LoadModelMain()
        {
            using (var stream = new FileStream(_modelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                _model = _mlContext.Model.Load(stream, out var modelInputSchema);
            }
        }

        public async Task PredictAbsencesBatch(EmployeeLeaveRequest inputData)
        {
            // Load the pre-trained model
            LoadModelMain();

            var now = DateTime.Now;

            // Get all employees and leave requests from the database
            var employeesRequests = _unitOfWork.EmployeeLeaveRequests.GetAllAsQueryable().ToList();
            var employees = (await _empRepo.GetAllEmployeesAsync()).Data;

            // Prepare a list of input data for batch prediction
            var predictionInputDataList = new List<EmployeeLeaveRequestPredict>();

            foreach (var req in employeesRequests)
            {
                float season = GetSeason(inputData.StartDate, inputData.EndDate);
                var empl = employees.FirstOrDefault(x => x.EmployeeId == req.EmployeeId);

                var empLeaveRecord = employeesRequests.Where(x => x.EmployeeId == empl.EmployeeId).Sum(x => x.NumberOfDaysOff);

                var predictionInputData = new EmployeeLeaveRequestPredict
                {
                    Age = (float)(now - empl.DateOfBirth).TotalDays,
                    NumberOfDaysOff = (inputData.StartDate - inputData.EndDate).Days,
                    EmploymentDuration = (float)(now - empl.DateCreated).TotalDays,
                    TotalLeaveDaysRemaining = empl.TotalHolidayEntitlement - empLeaveRecord,
                    LeaveType = (float)req.LeaveType,
                    Season = season
                };
                predictionInputDataList.Add(predictionInputData);
            }

            // Load the input data into an IDataView for batch processing
            IDataView batchData = _mlContext.Data.LoadFromEnumerable(predictionInputDataList);

            // Perform batch prediction using the loaded model
            IDataView predictions = _model.Transform(batchData);

            // Extract predictions from the IDataView
            var predictedResults = _mlContext.Data.CreateEnumerable<AbsencePredictionDto>(predictions, reuseRowObject: false).ToList();

            var response = new List<EmployeeLeavePredictResponse>();
            int count = 0;

            // Iterate through each prediction and add to the response
            foreach (var prediction in predictedResults)
            {
                /*if (prediction.WillBeAbsent)
                {
                    response.Add(new EmployeeLeavePredictResponse
                    {
                        EmployeeName = employees[count].EmployeeName,
                        LeaveType = employees[count].LeaveType,
                        Status = prediction.WillBeAbsent
                    });
                }*/

                count++;
                if (count == 5) break; // Limit the response to 5 predictions if necessary
            }

            // At this point, you have a batch prediction for all employees
            Console.WriteLine("Batch predictions complete.");
            Debug.WriteLine("Batch predictions complete.");
        }

        private float GetSeason(DateTime startDate, DateTime endDate)
        {
            float season = 0;
            if (startDate.Month >= 3 && startDate.Month <= 5)
            {
                season = (int)Season.Spring;
            }
            else if (startDate.Month >= 6 && startDate.Month <= 8)
            {
                season = (int)Season.Summer;
            }
            else if (startDate.Month >= 9 && startDate.Month <= 11)
            {
                season = (int)Season.Autumn;
            }
            else if (startDate.Month == 12 || startDate.Month == 1 || startDate.Month == 2)
            {
                season = (int)Season.Winter;
            }
            return season;
        }














        /*public bool PredictAbsence(EmployeeLeaveRequest inputData)
        {
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<EmployeeLeaveRequest, AbsencePredictionDto>(_model);
            var prediction = predictionEngine.Predict(inputData);
            return prediction.WillBeAbsent;
        }*/

        /*public double CalculateBradfordFactor(int absenceInstances, int totalDays)
        {
            // Bradford Factor formula: B = S^2 * D (S = absence instances, D = total days absent)
            return absenceInstances * absenceInstances * totalDays;
        }

        public int CalculateHolidayEntitlement(Employee employee, DateTime currentDate)
        {
            int fullEntitlement = employee.TotalHolidayEntitlement;
            if (employee.StartDate.Year == currentDate.Year)
            {
                var daysWorked = (currentDate - employee.StartDate).TotalDays;
                var totalYearDays = (new DateTime(currentDate.Year, 12, 31) - new DateTime(currentDate.Year, 1, 1)).TotalDays;
                return (int)((daysWorked / totalYearDays) * fullEntitlement);
            }
            return fullEntitlement;
        }*/
    }

    public class EmployeeLeaveData
    {
        [LoadColumn(0)]
        public float LeaveType { get; set; }
        [LoadColumn(1)]
        public float Age { get; set; }
        [LoadColumn(2)]
        public float NumberOfDaysOff { get; set; }
        [LoadColumn(3)]
        public float TotalLeaveDaysRemaining { get; set; }
        [LoadColumn(4)]
        public float Season { get; set; }
        [LoadColumn(5)]
        public float EmploymentDuration { get; set; }
        [LoadColumn(6), ColumnName("Label")]
        public float Remark { get; set; }
        [LoadColumn(7)]
        public string Status { get; set; }
    }

    
}
