using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectK.Games.LCR.Models
{
    public static class GenericExtensions
    {
        public static bool IsNullOrEmpty<T>(this IList<T> items)
        {
            return items == null || items.Count == 0;
        }

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
                    sb.Append(',');
            }

            return sb.ToString();
        }
        public static List<double> GetAxisCenters(double n1, double n2, int n, int delta)
        {
            var step = (n2 - n1) / n;
            var points = new List<double>();
            for (var i = 0; i <= n; i += delta)
            {
                var center = n1 + step * i;
                points.Add(center);
            }
            return points;
        }

        public static List<(double x, double y)> GetPoints(this List<IPoint<int>> points, (double x1, double y1, double x2, double y2) rect, (int x, int y) count)
        {
            var xCenters = GetAxisCenters(rect.x1, rect.x2, count.x, 1);
            var yCenters = GetAxisCenters(rect.y1, rect.y2, count.y, 1);
            var drawPoints = new List<(double x, double y)>();
            foreach (var point in points)
            {
                var x = xCenters[point.X];
                var y = yCenters[point.Y];
                drawPoints.Add((x, y));
            }
            return drawPoints;
        }
        public static List<(double x, double y)> GetPoints(this List<GameModel> games, (double x1, double y1, double x2, double y2) rect, (int x, int y) count)
        {
            var drawPoints = games.Cast<IPoint<int>>().ToList().GetPoints(rect, count);
            return drawPoints;
        }
    }
}
