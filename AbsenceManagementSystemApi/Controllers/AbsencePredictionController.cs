using AbsenceManagementSystem.Core.Domain;
using AbsenceManagementSystem.Core.IServices;
using AbsenceManagementSystem.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace AbsenceManagementSystemApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AbsencePredictionController : ControllerBase
    {
        private readonly IAbsencePredictorService _predictor;

        public AbsencePredictionController(IAbsencePredictorService predictor)
        {
            _predictor = predictor;
        }

        [HttpPost]
        public ActionResult<bool> Predict([FromBody] EmployeeLeaveRequest inputData)
        {
            //_predictor.TrainModel2();
            var result = _predictor.PredictAbsenceMain(inputData);
            return Ok(result);
        }
    }
}
