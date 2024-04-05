using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Student
    {
        public Student()
        {
            Enrolls = new HashSet<Enroll>();
            Submissions = new HashSet<Submission>();
        }

        public string UId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateOnly DoB { get; set; }
        public string Major { get; set; } = null!;

        public virtual Department MajorNavigation { get; set; } = null!;
        public virtual ICollection<Enroll> Enrolls { get; set; }
        public virtual ICollection<Submission> Submissions { get; set; }
    }
}
