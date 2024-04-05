﻿using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Professor
    {
        public Professor()
        {
            Classes = new HashSet<Class>();
        }

        public string UId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateOnly DoB { get; set; }
        public string Dept { get; set; } = null!;

        public virtual Department DeptNavigation { get; set; } = null!;
        public virtual ICollection<Class> Classes { get; set; }
    }
}
