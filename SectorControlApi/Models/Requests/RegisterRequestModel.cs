using System.ComponentModel.DataAnnotations;

namespace SectorControlApi.Models.Requests
{
    public class RegisterRequestModel
    {
        [StringLength(50)]
        public string? Name { get; set; }
        [StringLength(50)]
        public string? Password { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
    }
}
