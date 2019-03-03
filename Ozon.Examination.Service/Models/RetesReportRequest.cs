using System.ComponentModel.DataAnnotations;

namespace Ozon.Examination.Service.Models
{
    public class RetesReportRequest
    {
        [Required]
        public int Year { get; set; }

        [Required]
        [Range(1,12)]
        public byte Month { get; set; }
    }
}
