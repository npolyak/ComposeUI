using MorganStanley.Castle.Core;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Sebastion.Core
{
    public static class CollectionExt
    {
        public static ISet<T> ToSet<T>(this IEnumerable<T> coll)
        {
            HashSet<T> hashSet = new HashSet<T>();

            coll.ForEach(item => hashSet.Add(item));

            return hashSet;
        }

        public static void AddIfNotThere<T>(this ConcurrentBag<T> coll, T item)
        {
            if (!coll.Contains(item))
                coll.Add((T)(object)item);
        }

        public static void MixCollectionsRoundRobin<T>(this IEnumerable<IEnumerable<T>> collectionToMix, int limit, ICollection<T> result)
        {
            List<T[]> arrayCollectionToMix =
                collectionToMix.Select(coll => coll.ToArray()).ToList();

            int appIdx = 0;
            int resultLen = 0;
            List<T[]> newCollectionToMix = arrayCollectionToMix;
            while (newCollectionToMix.Any())
            {
                foreach (T[] envResult in arrayCollectionToMix)
                {
                    if (envResult.Length <= appIdx)
                    {
                        newCollectionToMix.Remove(envResult);
                        continue;
                    }

                    result.Add(envResult[appIdx]);
                    resultLen++;

                    if (resultLen > limit)
                        return;
                }
                arrayCollectionToMix = newCollectionToMix.ToList();

                appIdx++;
            }
        }
    }
}
