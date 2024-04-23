    namespace Job22.Models
    {
        public class Application
        {
            public int Id { get; set; }
            public string? Image { get; set; }
            public int? CandidateId { get; set; }
            public virtual Candidate? Candidate { get; set; }
            public int? JobListId { get; set; }
            public virtual JobList? JobList { get; set; }

        }
    }
