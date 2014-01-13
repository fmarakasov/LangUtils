using System.Collections.Generic;
using System.Globalization;

namespace System.Text.Lang
{
    /// <summary>
    ///     Реализует независимую от регистра проверку на равенство символов
    /// </summary>
    internal class CaseInsensitiveCharComparer : IEqualityComparer<char>
    {
        public static readonly CaseInsensitiveCharComparer Instance = new CaseInsensitiveCharComparer();

        private readonly CultureInfo _ci;

        public CaseInsensitiveCharComparer(CultureInfo ci)
        {
            _ci = ci;
        }

        public CaseInsensitiveCharComparer()
            : this(CultureInfo.CurrentCulture)
        {
        }

        public bool Equals(char x, char y)
        {
            return char.ToUpper(x, _ci) == char.ToUpper(y, _ci);
        }

        public int GetHashCode(char obj)
        {
            return char.ToUpper(obj).GetHashCode();
        }
    }
}