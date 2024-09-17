using AbsenceManagementSystem.Core.Domain;
using AbsenceManagementSystem.Core.DTO;
using AbsenceManagementSystem.Core.Enums;
using AbsenceManagementSystem.Core.IServices;
using AbsenceManagementSystem.Core.UnitOfWork;
using AbsenceManagementSystem.Infrastructure.DbContext;
using CsvHelper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.Data;
using System.Globalization;

namespace AbsenceManagementSystem.Services.Services
{
    public class AbsencePredictorService : IAbsencePredictorService
    {
        //private readonly string _modelPath = "C:\\Users\\SundayOladejo\\Documents\\absence mgt\\ml-model\\model.zip";
        //private readonly string _modelPath = "C:\\Users\\SundayOladejo\\Documents\\absence mgt\\ml-model";
        private string _modelPath = "C:\\Users\\SundayOladejo\\Documents\\Portfolio\\AbsenceManagementSystemApi - Copy\\AbsenceManagementSystemApi\\";
        private readonly MLContext _mlContext;
        private readonly IUnitOfWork _unitOfWork;
        private ITransformer _model;
        private ITransformer _loadedModel;
        private readonly IWebHostEnvironment _webEnv;

        public AbsencePredictorService(IUnitOfWork unitOfWork, IWebHostEnvironment webEnv)
        {
            _unitOfWork = unitOfWork;
            _mlContext = new MLContext();
            //_loadedModel = LoadModel2();
            _webEnv = webEnv;
            TrainModelMain();
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

            float season = 0;

            if(inputData.StartDate.Month >= 3 && inputData.StartDate.Month <= 5)
            {
                season = (int)Season.Spring;
            }
            else if(inputData.StartDate.Month >= 6 && inputData.StartDate.Month <= 8)
            {
                season = (int)Season.Summer;
            }
            else if(inputData.StartDate.Month >= 9 && inputData.StartDate.Month <= 11)
            {
                season = (int)Season.Autumn;
            }
            else if(inputData.StartDate.Month == 12 || inputData.StartDate.Month == 1 || inputData.StartDate.Month == 2)
            {
                season = (int)Season.Winter;
            }

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

        private void LoadModelMain()
        {
            using (var stream = new FileStream(_modelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                _model = _mlContext.Model.Load(stream, out var modelInputSchema);
            }
        }
















        public bool PredictAbsence(EmployeeLeaveRequest inputData)
        {
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<EmployeeLeaveRequest, AbsencePredictionDto>(_model);
            var prediction = predictionEngine.Predict(inputData);
            return prediction.WillBeAbsent;
        }

        public double CalculateBradfordFactor(int absenceInstances, int totalDays)
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
        }
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
