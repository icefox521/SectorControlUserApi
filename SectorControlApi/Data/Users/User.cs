using System.ComponentModel.DataAnnotations.Schema;

namespace SectorControlApi.Data.Users
{
    /// <summary>
    /// Represents user table in DB.
    /// </summary>
    public class User
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("username")]
        public string? UserName { get; set; }
        [Column("email")]
        public string? Email { get; set; }
        [Column("salt")]
        public string? HashSalt { get; set; }
        [Column("password_hash")]
        public string? PasswordHash { get; set; }
        [Column("registration_date")]
        public DateTime? RegistrationDate { get; set; }
        [Column("last_logged_in")]
        public DateTime? LastLoggedIn { get; set; }
    }
}
