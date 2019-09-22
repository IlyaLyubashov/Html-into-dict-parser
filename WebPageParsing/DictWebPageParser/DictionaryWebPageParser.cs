using System.IO;
using System.Collections.Generic;
using AngleSharp.Html.Parser;
using System.Text.RegularExpressions;

namespace WebPageParsing
{
    //не сделал статическим, тк вдруг захочется поменять путь для сохранения
    /// <summary>
    /// Определяет методы парсинга HTML страницы и сохраняет ее на диск.
    /// </summary>
    class DictionaryWebPageParser
    {       
        static string projDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;


        /// <summary>
        /// Директория, в которую будет происходить сохранения спаршенных страниц.
        /// </summary>
        public string DirToSaveParsedPages { get; set; } = projDirectory + "\\parsed pages";



        static int GetCurIndex(string dir)
        {
            var regIndex = new Regex(@"\d+");
            var files = Directory.GetFiles(dir);
            int biggestIndex = 0;
            foreach (var file in files)
            {
                if (Regex.IsMatch(file, @"index\d+"))
                {
                    var index = int.Parse(regIndex.Match(file).Value);
                    if (biggestIndex < index)
                        biggestIndex = index;
                }
            }
            return biggestIndex;
        }


        void SaveHtmlToDrive(string htmlString)
        {
            if (!Directory.Exists(DirToSaveParsedPages))
                Directory.CreateDirectory(DirToSaveParsedPages);
            string pathAndFile = DirToSaveParsedPages + $"\\index{GetCurIndex(DirToSaveParsedPages) + 1}.html";
            using (var streamWriter = new StreamWriter(pathAndFile))
            {
                streamWriter.Write(htmlString);
            }
            System.Console.WriteLine($"Запись на диск успешно завершена! Файл находится по адресу: {pathAndFile}");
        }


        /// <summary>
        /// Построчный парсинг HTML страницы.
        /// </summary>
        /// <param name="stream">Поток, полученый из http response.</param>
        /// <param name="wordDict">Ссылка на словарь, полученный в результате парсинга, будет записана в данную перменную.</param>
        /// <param name="saveToDrive">Параметр, определяющий нужно ли сохранять, HTML страницу на диск.</param>
        public void LineByLineParsing(StreamReader stream, out Dictionary<string, int> wordDict, bool saveToDrive)
        {
            var handler = new HtmlHandler();
            var wordCounter = new HtmlWordCounter();
            if (!Directory.Exists(DirToSaveParsedPages))
                Directory.CreateDirectory(DirToSaveParsedPages);
            string pathAndFile = DirToSaveParsedPages + $"\\index{GetCurIndex(DirToSaveParsedPages) + 1}.html";           
            var streamWriter = new StreamWriter(pathAndFile);
            while (!stream.EndOfStream)
            {
                var str = stream.ReadLine();
                if (saveToDrive)
                    streamWriter.WriteLine(str);
                handler.HandleFragment(str.Trim());
                foreach (var completeFrag in handler.GetCompeleteContent())
                {
                    wordCounter.ProcessHtml(new HtmlParser().ParseDocument(completeFrag).Body);
                }
            }
            streamWriter.Close();
            System.Console.WriteLine($"Запись на диск успешно завершена! Файл находится по адресу: {pathAndFile}");
            wordDict = wordCounter.GetCounterDictionary();
        }


        /// <summary>
        /// Парсинг строки, содержащей полностью HTML документ.
        /// </summary>
        /// <param name="stream">Поток, полученый из http response.</param>
        /// <param name="wordDict">Ссылка на словарь, полученный в результате парсинга, будет записана в данную перменную.</param>
        /// <param name="saveToDrive">Параметр, определяющий нужно ли сохранять, HTML страницу на диск.</param>
        public void FullPageParsing(StreamReader stream, out Dictionary<string, int> wordDict, bool saveToDrive)
        {
            var htmlString = stream.ReadToEnd();
            HtmlWordCounter WC = new HtmlWordCounter();
            WC.ProcessHtml(new HtmlParser().ParseDocument(htmlString));
            wordDict = WC.GetCounterDictionary();
            if (saveToDrive)
                SaveHtmlToDrive(htmlString);
        }
    }
}
