using Kolesov.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolesov.Repository
{
    public class SkillsRepository : ISkillsRepository
    {
        private string filename = "skills.txt";
        public List<string> GetAll()
        {
            if (File.Exists(filename))
                return File.ReadAllLines(filename).ToList();
            else
                return new List<string>();
        }

        public void Add(string skill)
        {
            if (!File.Exists(filename))
            {
                using (StreamWriter sw = File.CreateText(filename))
                {
                    sw.WriteLine(skill);
                }
            }
            else
            {
                if (!Exists(skill))
                {
                    using (StreamWriter sw = File.AppendText(filename))
                    {
                        sw.WriteLine(skill);
                    }
                }
            }
        }


        public bool Exists(string skill)
        {
            if (File.Exists(filename))
                return File.ReadAllLines(filename).Select(x => x.Equals(skill)).Any();
            else
                return false;
        }
    }
}
