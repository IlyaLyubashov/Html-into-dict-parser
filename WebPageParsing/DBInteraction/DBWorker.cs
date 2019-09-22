using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace WebPageParsing
{
    /// <summary>
    /// Позволяет производить необходимые манипуляции внутри базы данных: занесения записей о
    /// обработанных страницах и создания таблиц-словарей.
    /// </summary>
    class DBWorker
    {
        MySqlCommand cmd;
        string mainTableName = "pages parsed to dictionary";


        public DBWorker(MySqlCommand cmd)
        {
            this.cmd = cmd;
        }


        /// <summary>
        /// Проверка существования таблицы для ведения записей об обработанных страницах.
        /// </summary>
        /// <returns>Значение, описывающее существует ли таблица для введения записей о распашенных страницах.</returns>
        public bool IsTableExists()
        {
            cmd.CommandText = "SHOW TABLES";
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (reader.GetString(0) == mainTableName)
                            return true;
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// Создает таблицу для ведения записей обработанных страниц.
        /// </summary>
        public void CreateTable()
        {
            cmd.CommandText = $"create table `{mainTableName}`(uri varchar(1000), id int(10) unsigned PRIMARY KEY AUTO_INCREMENT);";
            cmd.ExecuteNonQuery();
        }


        /// <summary>
        /// Получение названия для очередной-таблицы словаря. ID обработанной страницы - числовое значение в названии таблицы.
        /// </summary>
        /// <returns>Название для таблицы словаря.</returns>
        string GetNewTableName()
        {
            /* id гарантировано получим, т.к. новую запись в maintable сделали,осталось только забрать id
             никаких проблем, если это первая запись не возникнет*/
            cmd.CommandText = $"SELECT * from `{mainTableName}` ORDER BY ID DESC LIMIT 1;";
            using (var reader = cmd.ExecuteReader())
            {
                reader.Read();
                var id = reader.GetInt64("id"); 
                string newTableName = "parsed" + id.ToString();
                return newTableName;
            } 
        }


        /// <summary>
        /// Создание записи в основной таблице о распаршенной странице, заполнение таблицы значениями из словаря встречаемости слов.
        /// </summary>
        /// <param name="uri">Идентификатор распашенной страницы.</param>
        /// <param name="parsedDict">Словарь для занесения в бд.</param>
        public void CreateNewDictionary(string uri, Dictionary<string, int> parsedDict)
        {
            cmd.CommandText = $"insert `{mainTableName}`(uri) values(\"{uri}\")";
            cmd.ExecuteNonQuery();
            string newTableName = GetNewTableName();    
            cmd.CommandText = $"create table `{newTableName}`(word varchar(40), count int);";
            cmd.ExecuteNonQuery();
            foreach (var item in parsedDict)
            {
                cmd.CommandText = $"insert into {newTableName} values(\"{item.Key}\",{item.Value})";
                cmd.ExecuteNonQuery();
            }
        }
    }
}
