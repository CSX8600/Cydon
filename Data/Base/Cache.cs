using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cydon.Data.CySys;

namespace Cydon.Data.Base
{
    public class Cache
    {
        private static Dictionary<string, BaseCache> cachesByName = new Dictionary<string, BaseCache>();
        private static bool _initialized = false;

        public static T GetCache<T>() where T : BaseCache
        {
            return (T)cachesByName.Values.FirstOrDefault(bc => bc is T);
        }

        public static Thread Initialize()
        {
            if (_initialized)
            {
                return null;
            }

            Context context = new Context();
            List<CacheVersion> cacheVersions = new List<CacheVersion>(context.CacheVersions);
            foreach(CacheVersion cacheVersion in cacheVersions)
            {
                context.CacheVersions.Remove(cacheVersion);
            }

            context.SaveChanges();

            Type baseCacheType = typeof(BaseCache);
            foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach(Type type in assembly.GetTypes().Where(t => t != baseCacheType && baseCacheType.IsAssignableFrom(t)))
                {
                    BaseCache baseCache = (BaseCache)Activator.CreateInstance(type);
                    baseCache.Update();
                    cachesByName.Add(baseCache.Name, baseCache);

                    CacheVersion cacheVersion = new CacheVersion()
                    {
                        CacheName = baseCache.Name,
                        NextRefreshTime = DateTime.Now.AddHours(1)
                    };

                    context.CacheVersions.Add(cacheVersion);
                }
            }

            context.SaveChanges();

            return new Thread(Poll);
        }

        private static void Poll()
        {
            while (true)
            {
                Context context = new Context();
                IEnumerable<CacheVersion> cacheVersions = context.CacheVersions.Where(cv => cv.NextRefreshTime <= DateTime.Now);

                foreach (CacheVersion cacheVersion in cacheVersions)
                {
                    if (!cachesByName.ContainsKey(cacheVersion.CacheName))
                    {
                        break;
                    }

                    BaseCache cache = cachesByName[cacheVersion.CacheName];
                    cache.Update();

                    cacheVersion.NextRefreshTime = DateTime.Now.AddHours(1);
                }

                context.SaveChanges();

                Thread.Sleep(10000);
            }
        }
    }
}
