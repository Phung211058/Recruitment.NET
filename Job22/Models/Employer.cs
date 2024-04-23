namespace Job22.Models
{
    public class Employer
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? Image { get; set; }
        public string? Name { get; set; }
        public string? Position { get; set; }
        public string? Company { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Password { get; set; }
        public virtual ICollection<JobList>? JobList { get; set; }
    }
}
