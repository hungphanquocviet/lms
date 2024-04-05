using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class AssignCategory
    {
        public AssignCategory()
        {
            Assignments = new HashSet<Assignment>();
        }

        public int ClassId { get; set; }
        public string Category { get; set; } = null!;
        public uint GradeWeight { get; set; }
        public int CategoryId { get; set; }

        public virtual Class Class { get; set; } = null!;
        public virtual ICollection<Assignment> Assignments { get; set; }
    }
}
