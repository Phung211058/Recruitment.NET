namespace Job22.Models
{
    public class JobList
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Requirement { get; set; }
        public string Benefit { get; set; }
        public string Location { get; set; }
        public string WorkingTime { get; set; }
        public int EmployerId { get; set; }
        public virtual Employer? Employer { get; set; }
        public virtual ICollection<Application>? Application { get; set; }
    }
}
