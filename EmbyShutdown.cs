using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace EmbyShutdown
{
    class EmbyShutdown
    {

        //private const string SessionFormat = "http://192.168.50.42:8096/emby/Sessions?api_key={0}";
        //private const string ShutdownFormat = "http://192.168.50.42:8096/emby/System/Shutdown?api_key={0}";

        private const string SessionFormat = "http://localhost:8096/emby/Sessions?api_key={0}";
        private const string ShutdownFormat = "http://localhost:8096/emby/System/Shutdown?api_key={0}";

        static void Main(string[] args)
        {
            EmbyShutdown p = new EmbyShutdown();
            p.RealMain(args);
        }

        public void RealMain(string[] args)
        {
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
            string agent = "EmbyShutdown";
            Console.WriteLine(agent);

            Uri uriResult;
            if (args.Length != 1)
            {
                Console.WriteLine("EmbyShutdown needs Emby API key paraeter.");
                Console.WriteLine("EmbyShutdown API_KEY}");
                Console.WriteLine("To get Emby api key go to dashboard>advanced>security and generate one");
                return; 
            }

            string uriName = string.Format(SessionFormat, args[0]);
            bool result = Uri.TryCreate(uriName, UriKind.Absolute, out uriResult) && uriResult.Scheme == Uri.UriSchemeHttp;
            if (!result)
            {
                Console.WriteLine("Invalid URI parameters");
                Console.WriteLine("EmbyShutdown API_KEY}");
                Console.WriteLine("To get Emby api key go to dashboard>advanced>security and generate one");
                return;
            }

            try
            {
                // Loop until there are no users
                bool usersOn = true;
                while (usersOn == true)
                {
                    // Get active sessions
                    HttpClient httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Add("user-agent", agent);
                    string sessionJson = httpClient.GetStringAsync(uriResult).Result;
                    List<EmbySessionData> sessionList = JsonConvert.DeserializeObject<List<EmbySessionData>>(sessionJson);

                    bool userFound = false;
                    foreach(EmbySessionData ed in sessionList)
                    {
                        if (ed.UserName != null)
                        {
                            userFound = true;
                            Console.WriteLine($"User {ed.UserName} is currently logged in.");
                        }
                    }

                    if (userFound == false)
                    {
                        Console.WriteLine($"No users currently logged in.  Starting shutdown sequence.");

                        // Shut down Emby
                        uriName = string.Format(ShutdownFormat, args[0]);
                        HttpClient httpClient2 = new HttpClient();
                        httpClient2.DefaultRequestHeaders.Add("user-agent", agent);
                        var crap = httpClient2.PostAsync(uriName, null).Result;

                        Thread.Sleep(30000);

                        // Then Windows
                        new Shutdown().DoExitWin(Shutdown.EWX_SHUTDOWN | Shutdown.EWX_FORCE);
                    }

                    Thread.Sleep(600000);
                }

            }
            catch (WebException wex)
            {
                Console.WriteLine(wex.Message);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }
    }
}
