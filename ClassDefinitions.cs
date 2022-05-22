using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmbyShutdown
{
    public class EmbySessionData
    {
        [JsonProperty("UserName")]
        public string? UserName { get; set; }

        [JsonProperty("Client")]
        public string? Client { get; set; }

        [JsonProperty("LastActivityDate")]
        public string? LastActivityDate { get; set; }  // ($date-time)      
    }
}
