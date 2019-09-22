using System.Collections.Generic;
using System;

namespace WebPageParsing
{
    /// <summary>
    /// Содержит методы, занимающиеся записью в базу данных.
    /// </summary>
    static class DBWriter
    {
        /// <summary>
        /// Подключение к необходимой базе данных, создание записи о распаршенной странице и создание таблиц-словаря встречаемости слов.
        /// </summary>
        /// <param name="DBConStr">Строка для подключения к серверу базы данных с заданными параметрами.</param>
        /// <param name="uri">Идентификатор распаршенной страницы.</param>
        /// <param name="dict">Словарь встречаемости слов, записываемый в бд.</param>
        public static void MakeRecordsToDB(string DBConStr,string uri, Dictionary<string, int> dict)
        {
            try
            {
                var con = DBUtils.GetDBConnection(DBConStr);
                con.Open();
                var cmd = con.CreateCommand();
                var DBWorker = new DBWorker(cmd);
                if (!DBWorker.IsTableExists())
                    DBWorker.CreateTable();
                DBWorker.CreateNewDictionary(uri, dict);
                Console.WriteLine("Данные в базу успешно записаны!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Данные в базу не были записаны.\nException message:\n{ex.Message}");
            }
        }
    }
}
