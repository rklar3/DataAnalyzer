using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAnalyzer.Models
{
    public class Root
    {
        [JsonProperty("RECORDS")]
        public Record[] Records { get; set; }
    }

    public class Record
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("created")]
        public string Created { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("risk")]
        public string Risk { get; set; }

        [JsonProperty("risk_level")]
        public int RiskLevel { get; set; }

        [JsonProperty("meta")]
        public string Meta { get; set; }

        [JsonProperty("active")]
        public string Active { get; set; }
    }

    public class MetaData
    {
        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("user_agent")]
        public string UserAgent { get; set; }

        [JsonProperty("ip_external")]
        public string IpExternal { get; set; }

        [JsonProperty("ip_internal")]
        public string[] IpInternal { get; set; }

        [JsonProperty("browser_uuid")]
        public string BrowserUuid { get; set; }

        private string _website;

        public string Website {
            get => _website = new System.Uri(Content).Host ?? Content; 
        }

        // Date of url request
        public DateTime DateOfSearch { get; set; }
    }

    public class RiskDetails
    {
        public int RiskLevel { get; set; }
        public string Risk { get; set; }
        public int SumOfRisks { get; set; }

        public List<MetaData> ListOfMetaData { get; set; } = new List<MetaData>();

        public List<MostSearchWebsites> ListOfMostSearched { get; set; } = new List<MostSearchWebsites>();

    }

    public class MostSearchWebsites
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }

}
