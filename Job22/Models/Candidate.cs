using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;
using static System.Net.Mime.MediaTypeNames;

namespace Job22.Models
{
    public class Candidate
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Password { get; set; }
        public string? UserId { get; set; }
        public virtual ICollection<Application>? Application { get; set; }
    }
}
