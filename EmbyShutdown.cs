﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace EmbyShutdown
{
    class EmbyShutdown
    {

        private const string SessionFormat = "http://192.168.50.42:8096/emby/Sessions?api_key={0}";

        //private const string SessionFormat = "http://localhost:8096/emby/Sessions?api_key={0}";

        static void Main(string[] args)
        {
            EmbyShutdown p = new EmbyShutdown();
            p.RealMain(args);
        }

        public void RealMain(string[] args)
        {
            
            Console.WriteLine($"EmbyShutdown Verion: {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}");
            Console.WriteLine("");

            Uri uriResult;
            if (args.Length != 1)
            {
                Console.WriteLine("EmbyShutdown needs Emby API key paraeter.");
                Console.WriteLine("EmbyShutdown API_KEY}");
                Console.WriteLine("To get Emby api key go to dashboard>advanced>security and generate one");
                return; 
            }

            string uriName = string.Format(SessionFormat, args[0]);
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            bool result = Uri.TryCreate(uriName, UriKind.Absolute, out uriResult) && uriResult.Scheme == Uri.UriSchemeHttp;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
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
#pragma warning disable CS8604 // Possible null reference argument.
                    bool userFound = CheckForActiveSessions(uriResult);
#pragma warning restore CS8604 // Possible null reference argument.

                    if (userFound == false)
                    {
                        Console.WriteLine("No users currently logged in.  Check again in 5 minutes just to make sure ...");
                        Thread.Sleep(300000);

                        userFound = CheckForActiveSessions(uriResult);

                        if (userFound == false)
                        {
                            Console.WriteLine("No users currently logged in.  Starting shutdown sequence.");

                            // Shut down Emby
                            new ShutdownEmby().Shutdown(args[0]);

                            Thread.Sleep(30000);

                            // Then Windows
                            new ShutdownWindows().DoExitWin(ShutdownWindows.EWX_SHUTDOWN | ShutdownWindows.EWX_FORCE);
                        }
                    }

                    Console.WriteLine("Users are currently logged in.  Wait 10 minutes and check again ...");

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

        private bool CheckForActiveSessions(Uri uriResult)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("user-agent", "EmbyShutdown");
            string sessionJson = httpClient.GetStringAsync(uriResult).Result;
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            List<EmbySessionData> sessionList = JsonConvert.DeserializeObject<List<EmbySessionData>>(sessionJson);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            bool userFound = false;
            if (sessionList != null)
            {
                foreach (EmbySessionData ed in sessionList)
                {
                    if (ed.UserName != null)
                    {
                        userFound = true;
                        Console.WriteLine($"User {ed.UserName} is currently logged in.");
                    }
                }
            }

            return userFound;
        }
    }
}
