using System.Globalization;
using System.Text;

namespace Adelys_WebSite.BL
{
    public static class SlugHelper
    {
        /// <summary>
        /// Adresse lisible tirée d'un titre : minuscules sans accents, mots séparés par des tirets
        /// </summary>
        public static string Slugify(string title, string fallback)
        {
            StringBuilder slug = new StringBuilder();
            foreach (char c in title.Normalize(NormalizationForm.FormD).ToLowerInvariant())
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark)
                {
                    continue;
                }
                if (char.IsAsciiLetterOrDigit(c))
                {
                    slug.Append(c);
                }
                else if (slug.Length > 0 && slug[^1] != '-')
                {
                    slug.Append('-');
                }
            }
            string result = slug.ToString().Trim('-');
            return result.Length > 0 ? result : fallback;
        }

        /// <summary>
        /// Ajoute -2, -3… tant que l'adresse est déjà prise
        /// </summary>
        public static string MakeUnique(string baseSlug, Func<string, bool> isTaken)
        {
            string slug = baseSlug;
            for (int suffix = 2; isTaken(slug); suffix++)
            {
                slug = $"{baseSlug}-{suffix}";
            }
            return slug;
        }
    }
}
