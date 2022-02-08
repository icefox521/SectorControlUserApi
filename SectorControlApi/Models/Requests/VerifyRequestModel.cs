using System.ComponentModel.DataAnnotations;

namespace SectorControlApi.Models.Requests
{
    public class VerifyRequestModel
    {
        [StringLength(50)]
        public string? Password { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
    }
}
