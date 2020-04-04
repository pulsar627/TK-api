using System;

namespace JobsPortal.Business.Models
{
    public class JobDetailsDto
    {
        public int Id { get; set; }
        public string JobTitle { get; set; }
        public string JobUrl { get; set; }

        public string PostedBy { get; set; }
        public DateTime? PostedDate { get; set; }
        public string AppliedBy { get; set; }
        public DateTime? AppliedDate { get; set; }
    }
}
