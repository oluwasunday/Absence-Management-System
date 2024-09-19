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
            var result = _predictor.PredictAbsenceMain(inputData);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<EmployeeLeavePredictResponse>>> PredictAbsences()
        {
            var result = await _predictor.PredictAbsences();
            return Ok(result);
        }
    }
}
