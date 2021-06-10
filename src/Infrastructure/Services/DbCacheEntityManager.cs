using Infrastructure.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Services
{
    public class DbCacheEntityManager : IDbCacheEntityManager
    {
        private ConcurrentBag<string> cachedEntityTags = new ConcurrentBag<string>();

        public string[] Tags => cachedEntityTags.ToArray();

        public void AddTag(string tag)
        {
            if (!cachedEntityTags.Contains(tag))
            {
                cachedEntityTags.Add(tag);
            }
        }

        public void AddTags(IEnumerable<string> tags)
        {
            foreach (var tag in tags)
            {
                AddTag(tag);
            }
        }

        public void Clear()
        {
            cachedEntityTags.Clear();
        }
    }
}
