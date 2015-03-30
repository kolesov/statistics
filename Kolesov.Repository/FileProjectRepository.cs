using Kolesov.Domain.Interfaces;
using Kolesov.Domain.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolesov.Repository
{
    public class FileProjectRepository : IProjectRepository
    {
        private string filename = "projects.txt";
        public void Add(Project project)
        {
            var stringToWrite = project.Link + "," + string.Join("|", project.Skills);
            if (!File.Exists(filename))
            {
                using (StreamWriter sw = File.CreateText(filename))
                {
                    sw.WriteLine(stringToWrite);
                }
            }
            else
            {
                if (!Exists(project))
                {
                    using (StreamWriter sw = File.AppendText(filename))
                    {
                        sw.WriteLine(stringToWrite);
                    }
                }
            }
        }

        public bool Exists(Project project)
        {
            if (File.Exists(filename))
                return File.ReadAllLines(filename).Where(x => x.Contains(project.Link)).Any();
            else
                return false;
        }
    }
}
