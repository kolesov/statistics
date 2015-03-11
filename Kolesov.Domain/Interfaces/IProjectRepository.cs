using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolesov.Domain.Interfaces
{
    public interface IProjectRepository
    {
        void Add(string projectId);
        bool Exists(string projectId);
    }
}
