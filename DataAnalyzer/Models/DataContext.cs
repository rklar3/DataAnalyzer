using Microsoft.AspNetCore.Http.Features;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DataAnalyzer.Models
{
    public class DataContext
    {

        private StreamReader _steamreader = null;

        // Root Record object for audit.json file
        Root root = new Root();
        public int TotalRecords;

        public DataContext()
        {
            // We only need the stream reader once, so dispose right after deserialization
            try
            {
                using (_steamreader = new StreamReader(@"C:\Users\devbox1952\Downloads\audit.json"))
                {
                    root = JsonConvert.DeserializeObject<Root>(_steamreader.ReadToEnd());
                }

            }finally
            {
                if(_steamreader != null)
                    _steamreader.Dispose();
            }
        }


        // Returns a list of records grouped by risk and risk level
        public List<RiskDetails> getListOfRiskDetails()
        {
            TotalRecords = root.Records.Count();

            return root.Records.Where(x => x.RiskLevel > 0)
                                           .OrderBy(y => y.RiskLevel)
                                           .GroupBy(u => u.RiskLevel)
                                           .Select(x => new RiskDetails
                                           {
                                               RiskLevel = x.FirstOrDefault().RiskLevel,
                                               Risk = x.FirstOrDefault().Risk,
                                               SumOfRisks = x.Count()
                                           })
                                           .ToList();

        }

        // Returns count of total risks
        public int totalRiskRecords()
        {
           return root.Records.Where(x => x.RiskLevel > 0).Count();
        }


        // Returns new object of RiskDetails grouped by risklevel
        private RiskDetails queryRiskDetails(int riskLevel)
        {
            return root.Records.Where(x => x.RiskLevel == riskLevel)
                               .OrderByDescending(y => y.Created)
                               .GroupBy(u => u.RiskLevel)
                               .Select(x => new RiskDetails
                               {
                                   RiskLevel = x.FirstOrDefault().RiskLevel,
                                   Risk = x.FirstOrDefault().Risk,
                                   SumOfRisks = x.Count()
                               })
                               .FirstOrDefault();
        }


        private DateTime stringToDateTime(string requestData)
        {
            DateTime resultOut;

            string[] formats = { "dd/MM/yyyy", "dd/M/yyyy", "d/M/yyyy", "d/MM/yyyy", "dd/MM/yy", "dd/M/yy", "d/M/yy", "d/MM/yy"};

            if (!DateTime.TryParseExact(requestData.Trim(), formats, null, DateTimeStyles.None, out resultOut))
                return new DateTime();
            
            return resultOut;
        }

        private string contentToWebsiteDomainName(string content)
        {
            Uri tempURI;

            if (Uri.TryCreate(content, UriKind.RelativeOrAbsolute, out tempURI))
                try
                {
                    return tempURI.Host.Substring(tempURI.Host.LastIndexOf('.', tempURI.Host.LastIndexOf('.') - 1) + 1); //test.com   
                }
                catch (Exception e)
                {
                    return tempURI.Host;
                }
                
            else
                return content;
        }

        // Returns contents of a risk with riskLevel
        public RiskDetails getRiskDetails(int riskLevel)
        {

            var searchedWebsites = new List<string>();

            var riskValues = queryRiskDetails(riskLevel);

            foreach (var record in root.Records.Where(x => x.RiskLevel == riskLevel))
            {
                MetaData RiskDetails = JsonConvert.DeserializeObject<MetaData>(record.Meta);

                // parseDate takes first 10 chars of record.Created that contains "dd/MM/yyyy" 
                RiskDetails.DateOfSearch = stringToDateTime(record.Created.Substring(0, 10).Trim());

                riskValues.ListOfMetaData.Add(RiskDetails);

                searchedWebsites.Add(contentToWebsiteDomainName(RiskDetails.Content));
            }

            riskValues.ListOfMostSearched = getTop5Websites(searchedWebsites);

            return riskValues;
        }


        // Returns the 5 most common websites in a risk level
        private List<MostSearchWebsites> getTop5Websites(List<string> SearchedWebsites)
        {
            var Top5Websites = SearchedWebsites
                      .GroupBy(i => i)
                      .OrderByDescending(g => g.Count())
                      .Take(5)
                      .Select(g => new MostSearchWebsites
                      {
                          Name = g.Key,
                          Count = g.Count()
                      }).ToList();

            return Top5Websites;
        }

        public List<MostSearchWebsites> getOverallTop5Websites()
        {
            List<string> SearchedWebsites = new List<string>();

            foreach (Record record in root.Records)
            {
                MetaData RiskDetails = JsonConvert.DeserializeObject<MetaData>(record.Meta);

                SearchedWebsites.Add(contentToWebsiteDomainName(RiskDetails.Content));
            }

            return getTop5Websites(SearchedWebsites);
        }

    }
}