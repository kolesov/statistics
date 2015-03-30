using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolesov.Domain.Models
{
    public class Project
    {
        public Project()
        {
            Skills = new List<string>();
        }

        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Employer { get; set; }
        public DateTime ParsedDate { get; set; }
        public List<string> Skills { get; set; }
        public string Budget { get; set; }
        public string Country { get; set; }
    }
}
