using System;



namespace WebPageParsing
{

    class Program
    {       
        static void Main(string[] args)
        {
            var app = new Application();
            app.PersonalAppSettings();
            while (app.oneMoreRequest)
            {
                app.Run();
                app.ContinueOrEnd();
            }    
        }       
    }
}

