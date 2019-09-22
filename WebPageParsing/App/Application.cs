using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using Microsoft.VisualBasic.Devices;

namespace WebPageParsing
{
    /// <summary>
    /// Содержит логику управления приложением для парсинга HTML страницы.
    /// </summary>
    class Application
    {
        string DBConString = DBUtils.GetDefaultConString();


        /// <summary>
        /// Нужно ли сохранять HTML на диск?
        /// </summary>
        public bool isSavedToDrive { get; set; } = true;


        /// <summary>
        /// Нужно ли спарсить еще одну страницу в текущем процессе?
        /// </summary>
        public bool oneMoreRequest { get; set; } = true;


        /// <summary>
        /// Нужно ли записывать данные в бд?
        /// </summary>
        public bool isWrittenToDB { get; set; } = false;
        

        bool YesNoQuery(string mes, char yKey, char noKey)
        {
            Console.WriteLine(mes);
            var keyChar = Console.ReadKey().KeyChar;
            while (true)
            {
                if (keyChar == yKey)
                {
                    Console.WriteLine();
                    return true;
                }
                else if (keyChar == noKey)
                {
                    Console.WriteLine();
                    return false;
                }
                else
                {
                    Console.WriteLine("\nНажмите y(yes)/n(not)");
                    keyChar = Console.ReadKey().KeyChar;
                }
            }
        }


        /// <summary>
        /// Определяет нужно ли повторить процесс парсинга страницы.
        /// </summary>
        /// <returns>Нужно ли спарсить еще одну страницу в текущем процессе?</returns>
        public bool ContinueOrEnd() => oneMoreRequest = YesNoQuery("Повторить попытку?(y/n)", 'y', 'n');


        bool ifUseDB() => YesNoQuery("Делать записи в базу данных?(y/n)", 'y', 'n');


        void MakingPersonalDBConnectStr()
        {
            Console.WriteLine("Enter host:");
            string host = Console.ReadLine();
            Console.WriteLine("Enter port:");
            string port = Console.ReadLine();
            Console.WriteLine("Enter database name:");
            string DBName = Console.ReadLine();
            Console.WriteLine("Enter username:");
            string username = Console.ReadLine();
            Console.WriteLine("Enter password:");
            string pass = Console.ReadLine();
            DBConString = "Server=" + host + ";Database=" + DBName + ";port=" + port + ";User Id=" + username + ";password=" + pass;
        }


        /// <summary>
        /// Установка персональных параметров для подключения к бд.
        /// </summary>
        public void PersonalAppSettings()
        {
            if (ifUseDB())
            {
                isWrittenToDB = true;
                Console.WriteLine("Подключения к серверу базы данных будет происходить " +
                    $"со следующими параметрами:\n{DBConString}");
                if (YesNoQuery("Изменить параметры?(y/n)", 'y', 'n'))
                    MakingPersonalDBConnectStr();
            }
            else
                isWrittenToDB = false;
        }

       
        bool IsLineByLineParsing(long contLength)
        {
            var availableRam = new ComputerInfo().AvailablePhysicalMemory;
            if (contLength == -1)
                return YesNoQuery("Не удалось определить размер HTML-страницы. " +
                    "Есть ли необходимость парсить страницу построчно?(y/n)", 'y', 'n');

            if ((ulong)contLength >availableRam )
            {
                if (MeasuredRamAllocation(contLength) > availableRam)
                    throw new Exception("Свободной оперативной памяти скорее всего не хватит на обработку страницы!");
                return true;
            }
            return false;
        }


        ulong MeasuredRamAllocation(long contSize) => (ulong)contSize * 12;


        void ConsoleWriteDict<T1,T2>(Dictionary<T1,T2> dict)
        {
            foreach (var item in dict)
                Console.WriteLine($"Key: {item.Key} Value: {item.Value}");
        }


        /// <summary>
        /// Запуск приложения для парсинга страницы.
        /// </summary>
        public void Run()
        {
            try
            {
                Console.WriteLine("Введите URI веб - страницы для парсинга:");
                var uri = Console.ReadLine();
                WebRequest request = WebRequest.Create(uri);
                WebResponse response = request.GetResponse();
                using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                {
                    var dictWebPageParser = new DictionaryWebPageParser();
                    Dictionary<string, int> wordDictionary;
                    if(IsLineByLineParsing(response.ContentLength))
                        dictWebPageParser.LineByLineParsing(stream, out wordDictionary, isSavedToDrive);
                    else
                        dictWebPageParser.FullPageParsing(stream, out wordDictionary, isSavedToDrive);

                    ConsoleWriteDict(wordDictionary);

                    if(isWrittenToDB)
                        DBWriter.MakeRecordsToDB(DBConString,uri, wordDictionary);
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception message:\n{ex.Message}");
            }
        }
    }
}
