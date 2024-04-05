using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Class
    {
        public Class()
        {
            AssignCategories = new HashSet<AssignCategory>();
            Enrolls = new HashSet<Enroll>();
        }

        public int CourseId { get; set; }
        public int Year { get; set; }
        public string Season { get; set; } = null!;
        public string Location { get; set; } = null!;
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string ProfId { get; set; } = null!;
        public int ClassId { get; set; }

        public virtual Course Course { get; set; } = null!;
        public virtual Professor Prof { get; set; } = null!;
        public virtual ICollection<AssignCategory> AssignCategories { get; set; }
        public virtual ICollection<Enroll> Enrolls { get; set; }
    }
}
