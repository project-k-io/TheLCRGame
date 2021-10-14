using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectK.Games.LCR.ViewModels
{
    public static class GenericExtensions
    {
        public static int GetNextItemIndex<T>(this IList<T> items, int index)
        {
            return index > 0 ? index - 1 : items.Count - 1;
        }
        public static int GetPrevItemIndex<T>(this IList<T> items, int index)
        {
            return index < items.Count - 1 ? index + 1 : 0;
        }

        public static T GetNextItem<T>(this IList<T> items, int index)
        {
            return items[items.GetNextItemIndex(index)];
        }

        public static T GetPrevItem<T>(this IList<T> items, int index)
        {
            return items[items.GetPrevItemIndex(index)];
        }

        public static (PlayerViewModel next, PlayerViewModel prev) GetNextAndPrevItems(this PlayerViewModel[] items, int index)
        {
            var item = items[index];
            var items2 = items.Where(item => !item.Active).ToList();
            var index2 = items2.IndexOf(item);
            var next =  items2.GetNextItem(index2);
            var prev = items2.GetPrevItem(index2);
            return (next, prev);
        }
    }
}
