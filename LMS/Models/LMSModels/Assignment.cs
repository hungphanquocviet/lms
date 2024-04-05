﻿using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Assignment
    {
        public Assignment()
        {
            Submissions = new HashSet<Submission>();
        }

        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public uint MaxPts { get; set; }
        public string Content { get; set; } = null!;
        public DateTime DueDate { get; set; }
        public int AssignId { get; set; }

        public virtual AssignCategory Category { get; set; } = null!;
        public virtual ICollection<Submission> Submissions { get; set; }
    }
}
