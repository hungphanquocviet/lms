using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Submission
    {
        public string StudentId { get; set; } = null!;
        public int AssignId { get; set; }
        public DateTime Time { get; set; }
        public string Content { get; set; } = null!;
        public uint Score { get; set; }

        public virtual Assignment Assign { get; set; } = null!;
        public virtual Student Student { get; set; } = null!;
    }
}
