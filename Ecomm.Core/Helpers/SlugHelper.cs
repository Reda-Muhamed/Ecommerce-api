using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Ecomm.Core.Helpers
{
    public static class SlugHelper
    {
        public static string Generate(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            text = text.ToLowerInvariant().Trim();

            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            var clean = sb.ToString().Normalize(NormalizationForm.FormC);

            // Remove invalid chars
            clean = Regex.Replace(clean, @"[^a-z0-9\s-]", "");

            // Replace spaces with hyphens
            clean = Regex.Replace(clean, @"\s+", "-");

            // Collapse multiple hyphens
            clean = Regex.Replace(clean, @"-+", "-");

            return clean.Trim('-');
        }
    }
}
