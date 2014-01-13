using System.Collections.Generic;

namespace System.Text.Lang
{
    /// <summary>
    ///     Позволяет определить созвучность строки некоторому языку, заданного таблицей частот
    /// </summary>
    internal class Phonetics
    {
        private readonly Dictionary<string, string[]> _freqTable;

        /// <summary>
        ///     Создаёт экземпляр класса Phonetics
        /// </summary>
        /// <param name="freqTable">Таблица частот для языка</param>
        public Phonetics(Dictionary<string, string[]> freqTable)
        {
            _freqTable = freqTable;
        }

        /// <summary>
        ///     Возвращает результат проверки строки на принадлежность языку, заданного таблицей частот
        /// </summary>
        /// <param name="str">Строка</param>
        /// <returns>Результат принадлежности. Если метод не в состоянии принять решение, то возвращается default(bool?)</returns>
        public bool? Probe(string str)
        {
            const int strSureLength = 3;
            const double tolerance = 0.000001;
            if (str.Length < strSureLength) return default(bool?);

            for (int i = 0; i < str.Length - 2; ++i)
            {
                double p = GetProbability(str[i], str[i + 1], str[i + 2]);
                if (Math.Abs(p) < tolerance) return false;
            }
            return true;
        }

        private double GetProbability(char p1, char p2, char p3)
        {
            var key = (p1.ToString() + p2.ToString()).ToLower();
            if (!_freqTable.ContainsKey(key)) return 0.0;
            int pos = Char.ToLower(p3) - 'a' + 1;
            string[] v = _freqTable[key];
            if (pos >= v.Length || pos < 0) return 0.0;
            return Convert.ToDouble(v[pos]);
        }
    }
}