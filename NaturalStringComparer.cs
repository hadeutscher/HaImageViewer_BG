using System.Text.RegularExpressions;

namespace HaImageViewer_BG
{
    public class NaturalStringComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == null) return y == null ? 0 : -1;
            if (y == null) return 1;

            var xParts = Regex.Split(x, "([0-9]+)");
            var yParts = Regex.Split(y, "([0-9]+)");

            int max = Math.Max(xParts.Length, yParts.Length);
            for (int i = 0; i < max; i++)
            {
                if (i >= xParts.Length) return -1;
                if (i >= yParts.Length) return 1;

                var xPart = xParts[i];
                var yPart = yParts[i];

                // Compare numbers numerically
                if (int.TryParse(xPart, out int xi) && int.TryParse(yPart, out int yi))
                {
                    int cmp = xi.CompareTo(yi);
                    if (cmp != 0) return cmp;
                }
                else
                {
                    int cmp = string.Compare(xPart, yPart, StringComparison.OrdinalIgnoreCase);
                    if (cmp != 0) return cmp;
                }
            }
            return 0;
        }
    }
}