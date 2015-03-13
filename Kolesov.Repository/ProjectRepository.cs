using Kolesov.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolesov.Repository
{
    public class ProjectRepository : IProjectRepository
    {
        private string filename = "projects.txt";
        public void Add(string projectId, string[] skills)
        {
            var stringToWrite = projectId + "," + string.Join("|", skills);
            if (!File.Exists(filename))
            {
                using (StreamWriter sw = File.CreateText(filename))
                {
                    sw.WriteLine(stringToWrite);
                }
            }
            else
            {
                if (!Exists(projectId))
                {
                    using (StreamWriter sw = File.AppendText(filename))
                    {
                        sw.WriteLine(stringToWrite);
                    }
                }
            }
        }

        public bool Exists(string projectId)
        {
            if (File.Exists(filename))
                return File.ReadAllLines(filename).Where(x => x.Contains(projectId)).Any();
            else
                return false;
        }
    }
}
