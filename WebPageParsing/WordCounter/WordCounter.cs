using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebPageParsing
{
    //Публичный для тестов
    /// <summary>
    /// Составляет словарь встречаемости слов в строке.
    /// </summary>
    public class WordCounter
    {        
        // string сепартор на случай, если мы решим добавить разделитель, состоящий из более, чем одного символа
        static char[] separators = new char[] { ' ', ',', '.', '!', '?', '\"', ';', ':', '[', ']', '(', ')', '\n', '\t', '\r' };
        string[] separatorsDynamic;
        Dictionary<string, int> wordsCounted = new Dictionary<string, int>();


        public WordCounter()
        {
            InitDynamicSeparators();
        }


        void InitDynamicSeparators()
        {
            separatorsDynamic = new string[separators.Length];
            for (int i = 0; i < separators.Length; i++)
                separatorsDynamic[i] = separators[i].ToString();
        }


        static string SeparatorStartEndFix(string word)
        {
            string w = word;
            foreach (var sep in separators)
            {
                w = w.StartsWith(sep.ToString()) ? w.Substring(1) : w;
                w = w.EndsWith(sep.ToString()) ? w.Substring(0, w.Length - 1) : w;
            }
            return w;
        }


        /// <summary>
        /// Метод принимает на вход строку и возвращает словарь встречаемости слов в этой строке.
        /// </summary>
        /// <param name="str">Строка, слова из который должны быть добавлены в словарь.</param>
        /// <returns>Словарь встречаемости слов в строке.</returns>
        public static Dictionary<string, int> CountWords(string str)
        {
            var wordsCounted = new Dictionary<string, int>();
            foreach (var word in str.ToLower().Split(separators, StringSplitOptions.RemoveEmptyEntries))
            {
                string w = SeparatorStartEndFix(word);
                if (wordsCounted.Keys.Contains(word))
                    wordsCounted[word] += 1;
                else
                    wordsCounted[word] = 1;
            }
            return wordsCounted;
        }

        
        /// <summary>
        /// Динамический метод, позволяющий постепенно наполнять словарь, передавая строки.
        /// </summary>
        /// <param name="str">Строка, слова из которой будут добавлены в словарь.</param>
        public void AppendWordsToCounter(string str)
        {

            foreach (var word in str.ToLower().Split(separatorsDynamic, StringSplitOptions.RemoveEmptyEntries))
            {
                string w = SeparatorStartEndFix(word);
                foreach (var sep in separators)
                {
                    w = w.StartsWith(sep.ToString()) ? w.Substring(1) : w;
                    w = w.EndsWith(sep.ToString()) ? w.Substring(0, w.Length - 1) : w;
                }
                if (wordsCounted.Keys.Contains(word))
                    wordsCounted[word] += 1;
                else
                    wordsCounted[word] = 1;
            }
        }


        /// <summary>
        /// Возвращает текущий словарь встречаемости слов в переданных до этого строках.
        /// </summary>
        /// <returns>Словарь встречаемости слов.</returns>
        public Dictionary<string, int> GetCounterDictionary() => wordsCounted;


        /// <summary>
        /// Метод позволяет добавить новый разделитель для слов
        /// </summary>
        /// <param name="sep">Разделитель слов</param>
        public void AppendSeparator(string sep)
        {
            string[] newDynamicSeparators = new string[separatorsDynamic.Length + 1] ;
            separatorsDynamic.CopyTo(newDynamicSeparators,0);
            newDynamicSeparators[newDynamicSeparators.Length - 1] = sep;
            separatorsDynamic = newDynamicSeparators;
        }
    }
}
