using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace System.Text.Lang
{
    public static class LangExtensions
    {
        private static readonly IDictionary<CultureInfo, IDictionary<CultureInfo, IDictionary<char, char>>>
            KeyboardLayouts = new Dictionary
                <CultureInfo, IDictionary<CultureInfo, IDictionary<char, char>>>
            {
                {
                    CultureInfo.GetCultureInfo("en-us"), new Dictionary<CultureInfo, IDictionary<char, char>>
                    {
                        {
                            CultureInfo.GetCultureInfo("ru-ru"),
                            new Dictionary<char, char>(CaseInsensitiveCharComparer.Instance)
                            {
                                {'a', 'ф'},
                                {'b', 'и'},
                                {'c', 'с'},
                                {'d', 'в'},
                                {'e', 'у'},
                                {'f', 'а'},
                                {'g', 'п'},
                                {'h', 'р'},
                                {'i', 'ш'},
                                {'j', 'о'},
                                {'k', 'л'},
                                {'l', 'д'},
                                {'m', 'ь'},
                                {'n', 'т'},
                                {'o', 'щ'},
                                {'p', 'з'},
                                {'q', 'й'},
                                {'r', 'к'},
                                {'s', 'ы'},
                                {'t', 'е'},
                                {'v', 'м'},
                                {'w', 'ц'},
                                {'x', 'ч'},
                                {'y', 'н'},
                                {'z', 'я'},
                                {'`', 'ё'},
                                {'[', 'х'},
                                {']', 'ъ'},
                                {';', 'ж'},
                                {'\'', 'э'},
                                {'u', 'г'},
                                {',', 'б'},
                                {'.', 'ю'},
                                {'~', 'Ё'},
                                {'{', 'Х'},
                                {'}', 'Ъ'},
                                {':', 'Ж'},
                                {'"', 'Э'},
                                {'<', 'Б'},
                                {'>', 'Ю'}
                            }
                        }
                    }
                }
            };

        private static readonly IDictionary<CultureInfo, Phonetics> TablesDictionary =
            new Dictionary<CultureInfo, Phonetics>();

        static LangExtensions()
        {
            Dictionary<string, string[]> table = LangResource.Stat_en_us.Split('\n').ToDictionary(s =>
                s[0].ToString(CultureInfo.InvariantCulture) + s[2].ToString(CultureInfo.InvariantCulture),
                s => s.Substring(3).Trim().Split(';'), StringComparer.Ordinal);

            TablesDictionary.Add(CultureInfo.GetCultureInfo("en-us"), new Phonetics(table));
        }

        /// <summary>
        /// Set the shift state of the source char equals to target char shift state    
        /// </summary>
        /// <param name="source">Source char</param>
        /// <param name="target">Sample char</param>
        /// <returns>Source char with shift state as shift state as sample char</returns>
        public static char ToSameShift(this char source, char target)
        {
            return ToSameShift(source, target, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Culture-dependent shift state set of the source char equals to target char shift state         
        /// </summary>
        /// <param name="source">Source char</param>
        /// <param name="target">Символ-образец</param>
        /// <param name="ci">CultureInfo object</param>
        /// <returns>Source char with shift state as shift state as sample char</returns>
        public static char ToSameShift(this char source, char target, CultureInfo ci)
        {
            return Char.IsLetter(target)
                ? (Char.IsUpper(target)
                    ? Char.ToUpper(source, ci)
                    : Char.ToLower(source, ci))
                : source;
        }

        /// <summary>
        ///     Phonetic test of the source string of belonging to specified culture
        /// </summary>
        /// <param name="source">Source string to test</param>
        /// <param name="culture">CultureInfo object</param>
        /// <returns> Returns true if specified string sounds phoneticaly belongs to specified culture and false otherwise. Returns null, if method can't determine the language
        /// Реузьтат проверки или default(bool?) в случае, если определить принадлежность не удалось</returns>
        /// <exception cref="ArgumentException">Throws if the specified language does not supporting</exception>
        public static bool? SoundsLike(this string source, CultureInfo culture)
        {
            Phonetics phonetics;
            if (!TablesDictionary.TryGetValue(culture, out phonetics))
                throw new ArgumentException("Specified culture is not suporting.");
            return phonetics.Probe(source);
        }

        private static IDictionary<char, char> GetLayoutMap(CultureInfo sourceCultureInfo, CultureInfo targeCultureInfo)
        {
            IDictionary<CultureInfo, IDictionary<char, char>> maps;
            if (!KeyboardLayouts.TryGetValue(sourceCultureInfo, out maps)) return null;
            IDictionary<char, char> cultureMap;
            return maps.TryGetValue(targeCultureInfo, out cultureMap) ? cultureMap : null;
        }

        /// <summary>
        ///    Projects each char of specified string of source culture to the corresponding char of destination culture 
        /// </summary>
        /// <param name="source">Source string</param>
        /// <param name="sourceCultureInfo">Source string culture</param>
        /// <param name="targetCultureInfoCultureInfo">Destination string culture</param>
        /// <param name="result">Out parameter for the destination string</param>
        /// <returns>True if projection was succeeded</returns>
        /// <exception cref="ArgumentException">Throws if specified culture does not supporting</exception>
        public static bool TryMapLayout(this string source, CultureInfo sourceCultureInfo, CultureInfo targetCultureInfoCultureInfo, out string result)
        {
            IDictionary<char, char> map;
            if ((map = GetLayoutMap(sourceCultureInfo, targetCultureInfoCultureInfo)) == null)
                throw new ArgumentException("Specified culture is not suporting.");
            
            var sb = new StringBuilder(source.Length);
            try
            {
                result = source.Aggregate(sb, (s, x) => s.Append(map[x].ToSameShift(x)), s => s.ToString());
            }
            catch (KeyNotFoundException)
            {
                result = string.Empty;
                return false;
            }
            return true;
        }

        /// <summary>
        ///     Try to prject each string symbol of source culture to a new symbol of target culture, based on standard keyboard layout. Method returns false if source string is allready sounds like specified source culture or if the projection of one or more symbols failed
        /// </summary>
        /// <param name="source">Source string</param>
        /// <param name="sourceCultureInfo">Source string culture</param>
        /// <param name="targetCultureInfo">Target string culture</param>
        /// <param name="result">The out parameter, that takes projected string or String.Empty, if the projection failed</param>
        /// <returns>True if the projection was successfull</returns>
        public static bool CorrectLayout(this string source, CultureInfo sourceCultureInfo,
            CultureInfo targetCultureInfo, out string result)
        {
            // if source string sounds like a specified source culture or this thing can't be determined than return false 
            var res = source.SoundsLike(sourceCultureInfo);

            if (!res.HasValue || res.Value)
            {
                result = string.Empty;
                return false;
            }

            return source.TryMapLayout(sourceCultureInfo, targetCultureInfo, out result);
        }
        /// <summary>
        /// Async version of CorrectLayout. Try to prject each string symbol of source culture to a new symbol of target culture, based on standard keyboard layout. 
        /// </summary>
        /// <param name="source">Source string</param>
        /// <param name="sourceCultureInfo">Source string culture</param>
        /// <param name="targetCultureInfo">Target string culture</param>
        /// <returns>Task object</returns>
        public static Task<TextResult> CorrectLayoutAsync(this string source, CultureInfo sourceCultureInfo, CultureInfo targetCultureInfo)
        {
            return Task.Run(() =>
            {
                string text;
                var result = CorrectLayout(source, sourceCultureInfo, targetCultureInfo, out text);
                return new TextResult(result, text);
            });                        
        }
    }
}