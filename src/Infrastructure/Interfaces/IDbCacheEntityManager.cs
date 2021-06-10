using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface IDbCacheEntityManager
    {
        string[] Tags { get; }

        void AddTag(string tag);
        void AddTags(IEnumerable<string> tags);
        void Clear();
    }
}
