using Microsoft.ML.Data;

namespace AbsenceManagementSystem.Core.DTO
{
    public class AbsencePredictionDto
    {
        [ColumnName("PredictedLabel")]
        public bool WillBeAbsent { get; set; }
    }
}
