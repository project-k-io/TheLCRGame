using System.Collections.Generic;
using System.Text;

namespace ProjectK.Games.LCR.Models
{
    public static class GenericExtensions
    {
        public static int GetNextItemIndex2<T>(this IList<T> items, int index)
        {
            return index > 0 ? index - 1 : items.Count - 1;
        }
        public static int GetPrevItemIndex<T>(this IList<T> items, int index)
        {
            return index < items.Count - 1 ? index + 1 : 0;
        }

        public static T GetNextItem<T>(this IList<T> items, int index)
        {
            return items[items.GetNextItemIndex2(index)];
        }

        public static T GetPrevItem<T>(this IList<T> items, int index)
        {
            return items[items.GetPrevItemIndex(index)];
        }
        public static bool IsLast<T>(this IList<T> items, T item)
        {
            return items.IndexOf(item) == items.Count-1;
        }

        public static string ToText<T>(this IList<T> items)
        {
            var sb = new StringBuilder();
            foreach (var item in items)
            {
                sb.Append(item);
                if (!items.IsLast(item))
                    sb.Append(",");
            }

            return sb.ToString();
        }

    }
}
