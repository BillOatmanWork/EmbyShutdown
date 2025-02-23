using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace EmbyShutdown
{
    class EmbyShutdown
    {

       // private const string SessionFormat = "http://192.168.50.42:{1}/emby/Sessions?api_key={0}";

        private const string SessionFormat = "http://localhost:{1}/emby/Sessions?api_key={0}";

        static void Main(string[] args)
        {
            EmbyShutdown p = new EmbyShutdown();
            p.RealMain(args);
        }

        public void RealMain(string[] args)
        {
            File.Delete("EmbyShutdown.log");

            ConsoleWithLog($"EmbyShutdown Version: {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}");
            ConsoleWithLog("");

            Uri uriResult;
            if (args.Length != 2)
            {
                ConsoleWithLog("EmbyShutdown needs Emby API key parameter as well as the servers port.");
                ConsoleWithLog("EmbyShutdown API_KEY port");
                ConsoleWithLog("To get Emby api key go to dashboard>advanced>security and generate one");
                return; 
            }

            string uriName = string.Format(SessionFormat, args[0], args[1]);
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            bool result = Uri.TryCreate(uriName, UriKind.Absolute, out uriResult) && uriResult.Scheme == Uri.UriSchemeHttp;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            if (!result)
            {
                ConsoleWithLog("Invalid URI parameters");
                ConsoleWithLog("EmbyShutdown API_KEY port");
                ConsoleWithLog("To get Emby api key go to dashboard>advanced>security and generate one");
                return;
            }

            try
            {
                // Loop until there are no users
                bool usersOn = true;
                while (usersOn == true)
                {
#pragma warning disable CS8604 // Possible null reference argument.
                    bool userFound = CheckForActiveSessions(uriResult);
#pragma warning restore CS8604 // Possible null reference argument.

                    if (userFound == false)
                    {
                        ConsoleWithLog($"No users currently logged in.  Check again in 5 minutes just to make sure ({DateTime.Now.AddMinutes(5)}) ...");
                        Thread.Sleep(300000);

                        userFound = CheckForActiveSessions(uriResult);

                        if (userFound == false)
                        {
                            ConsoleWithLog("No users currently logged in.  Starting shutdown sequence.");

                            // Shut down Emby
                            new ShutdownEmby().Shutdown(args[0]);

                            Thread.Sleep(30000);

                            // Then Windows
                            new ShutdownWindows().DoExitWin(ShutdownWindows.EWX_SHUTDOWN | ShutdownWindows.EWX_FORCE);

                            // Wait for Windows to shut down
                            Thread.Sleep(600000);
                        }
                    }

                    ConsoleWithLog($"Users are currently logged in.  Wait 10 minutes and check again ... ({DateTime.Now.AddMinutes(10)})");

                    Thread.Sleep(600000);
                }

            }
            catch (WebException wex)
            {
                ConsoleWithLog(wex.Message);
            }
            catch (Exception err)
            {
                ConsoleWithLog(err.Message);
            }
        }

        private bool CheckForActiveSessions(Uri uriResult)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("user-agent", "EmbyShutdown");
            string sessionJson = httpClient.GetStringAsync(uriResult).Result;
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.

            //using (StreamWriter file = File.CreateText(@"ActiveSessions.json"))
            //{
            //    file.Write(JsonPrettify(sessionJson));
            //}

            List<EmbySessionData> sessionList = JsonConvert.DeserializeObject<List<EmbySessionData>>(sessionJson);

#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            bool userFound = false;
            if (sessionList != null)
            {
                foreach (EmbySessionData ed in sessionList)
                {
                    if (ed.UserName != null)
                    {
                        DateTime.TryParse(ed.LastActivityDate, out DateTime lastActivity);
                        TimeSpan inactivity = DateTime.Now - lastActivity;

                        if (inactivity.TotalMinutes < 1)
                        {
                            userFound = true;
                            ConsoleWithLog($"User {ed.UserName} is currently logged in and inactive for {inactivity.TotalMinutes} minutes.");
                        }
                        else
                            ConsoleWithLog($"User {ed.UserName} is currently logged in but inactive for {inactivity.TotalMinutes} minutes so ignoring.");
                    }
                }
            }

            return userFound;
        }

        public static void ConsoleWithLog(string text)
        {
            Console.WriteLine(text);

            using (StreamWriter file = File.AppendText("EmbyShutdown.log"))
            {
                file.Write(text + Environment.NewLine);
            }
        }

        /// <summary>
        /// Indents and adds line breaks etc to make it pretty for printing/viewing
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static string JsonPrettify(string json)
        {
            using (var stringReader = new StringReader(json))
            using (var stringWriter = new StringWriter())
            {
                var jsonReader = new JsonTextReader(stringReader);
                var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Newtonsoft.Json.Formatting.Indented };
                jsonWriter.WriteToken(jsonReader);
                return stringWriter.ToString();
            }
        }
    }
}
