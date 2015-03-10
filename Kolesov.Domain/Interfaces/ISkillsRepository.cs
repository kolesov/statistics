using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolesov.Domain.Interfaces
{
    public interface ISkillsRepository
    {
        List<string> GetAll();
        void Add(string skill);
        bool Exists(string skill);
    }
}
