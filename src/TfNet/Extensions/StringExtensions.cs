using System.Diagnostics.CodeAnalysis;

namespace TfNet.Extensions;

internal static class StringExtensions
{
    extension(string s)
    {
        [return: NotNullIfNotNull(nameof(s))]
        public string? ToFirstLetterLower()
        {
            if (s == null)
            {
                return null;
            }
            else if (s.Length == 0)
            {
                return s;
            }
            else if (s.Length == 1)
            {
                return char.IsLower(s[0]) ? s : s.ToLower();
            }
            else
            {
                return char.ToLower(s[0]) + s[1..];
            }
        }
    }
}
