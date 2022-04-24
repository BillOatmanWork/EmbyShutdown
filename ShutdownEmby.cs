namespace EmbyShutdown
{
    public class ShutdownEmby
    {
        //private const string ShutdownFormat = "http://192.168.50.42:8096/emby/System/Shutdown?api_key={0}";
        private const string ShutdownFormat = "http://localhost:8096/emby/System/Shutdown?api_key={0}";

        public void Shutdown(string akiKey)
        {
            string uriName = string.Format(ShutdownFormat, akiKey);
            HttpClient httpClient2 = new HttpClient();
            httpClient2.DefaultRequestHeaders.Add("user-agent", "EmbyShutdown");
            httpClient2.PostAsync(uriName, null).Wait();
        }
    }
}
