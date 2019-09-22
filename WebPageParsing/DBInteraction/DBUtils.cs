using MySql.Data.MySqlClient;

namespace WebPageParsing
{
    /// <summary>
    /// Содержит методы для получения MySqlConnection бд и данные с дефолтными параметрами для подключения к бд.
    /// </summary>
    static class DBUtils
    {
        static string host = "localhost";
        static string port = "3306";
        static string DBName = "test";
        static string username = "user";
        static string pass = "1U2S3E4R";
        

        /// <summary>
        /// Получение MySqlConnection сервера бд.
        /// </summary>
        /// <param name="host">Хост.</param>
        /// <param name="port">Порт.</param>
        /// <param name="database">Имя бд.</param>
        /// <param name="username">Имя пользователя.</param>
        /// <param name="password">Пароль.</param>
        /// <returns></returns>
        public static MySqlConnection GetDBConnection(string host, string port, string database,
            string username, string password)
        {
            string DBOptions = "Server=" + host + ";Database=" + database + ";port=" + 
                port + ";User Id=" + username + ";password=" + password;
            return new MySqlConnection(DBOptions);
        }


        /// <summary>
        /// Получение MySqlConnection сервера бд.
        /// </summary>
        /// <param name="DBConStr">Строка с параметрами подключения к серверу бд.</param>
        /// <returns></returns>
        public static MySqlConnection GetDBConnection(string DBConStr) => new MySqlConnection(DBConStr);
        

        /// <summary>
        /// Получение строки, содержащей параметры подключения к серверу бд.
        /// </summary>
        /// <returns>Строка с параметрами по умолчанию для подключения к серверу бд.</returns>
        public static string GetDefaultConString()
        {
            return "Server=" + host + ";Database=" + DBName + ";port=" + port + ";User Id=" + username + ";password=" + pass;
        }
    }
}
        
