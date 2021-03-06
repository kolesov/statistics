﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolesov.Domain.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> Emails { get; set; }
        public List<string> InterestedSkills { get; set; }
        public List<string> ExcludeSkills { get; set; }
        public List<string> KeyWords { get; set; }
        public List<string> MinusKeyWords { get; set; }
    }
}
