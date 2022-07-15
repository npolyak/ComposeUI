using System.Collections;

namespace Sebastion.Core.SvcUtils
{
    public static class ExclusionListHelper
    {
        public static void ChangeItems(this IList list, bool excludeOrInclude)
        {
            foreach (AppRetrievalSpeedInfo speedInfo in list)
            {
                speedInfo.ExcludeFromRetrieval = excludeOrInclude;
            }
        }

        public static void ExcludeItems(this IList list) => list.ChangeItems(true);

        public static void UnExcludeItems(this IList list) => list.ChangeItems(false);
    }
}
