using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using System.Net;
using Opendata.Recalls.Models;
using System.Collections.Generic;
using System.Text;
using System;
using System.Linq;
using System.Net.Http;
using Opendata.Recalls.Commands;

namespace Opendata.Recalls.Repository
{
  //TODO: this whole method needs to be refactored
    public class RecallApiProxyRepository : IRecallApiProxyRepository
    {
        private const string SERVICE_ROOT = "https://www.saferproducts.gov/RestWebServices/Recall?format=json";//Environment.GetEnvironmentVariable("CPSCAPIUrlRoot");
        public async Task<SearchQueryResult> RetrieveRecall(SearchCommand command)
        {

            StringBuilder uriBuilder = new StringBuilder(SERVICE_ROOT);

            List<Recalls.Models.Recall> recList = null;
            List<Recalls.Models.Recall> finalPO = null;

            if (!String.IsNullOrEmpty(command.Data.SearchFor))
            {
                StringBuilder uriBuilder1 = new StringBuilder(SERVICE_ROOT);
                uriBuilder1.AppendFormat("&RecallTitle={0}", command.Data.SearchFor);
                Task<List<Recall>> poList1 = RecallList(uriBuilder1.ToString());

                StringBuilder uriBuilder2 = new StringBuilder(SERVICE_ROOT);
                uriBuilder2.AppendFormat("&ProductName={0}", command.Data.SearchFor);
                var poList2 = RecallList(uriBuilder2.ToString());

                StringBuilder uriBuilder3 = new StringBuilder(SERVICE_ROOT);
                uriBuilder3.AppendFormat("&Hazard={0}", command.Data.SearchFor);
                var poList3 = RecallList(uriBuilder3.ToString());

                StringBuilder uriBuilder4 = new StringBuilder(SERVICE_ROOT);
                uriBuilder4.AppendFormat("&ManufacturerCountry={0}", command.Data.SearchFor);
                var poList4 = RecallList(uriBuilder4.ToString());

                StringBuilder uriBuilder5 = new StringBuilder(SERVICE_ROOT);
                uriBuilder5.AppendFormat("&Manufacturer={0}", command.Data.SearchFor);
                var poList5 = RecallList(uriBuilder5.ToString());

                StringBuilder uriBuilder6 = new StringBuilder(SERVICE_ROOT);
                uriBuilder6.AppendFormat("&RecallNumber={0}", command.Data.SearchFor);
                var poList6 = RecallList(uriBuilder6.ToString());
                finalPO = poList1.Result.Union(poList1.Result, new RecallComparer()).ToList();
                finalPO = finalPO.Union(poList2.Result, new RecallComparer()).ToList();
                finalPO = finalPO.Union(poList3.Result, new RecallComparer()).ToList();
                finalPO = finalPO.Union(poList4.Result, new RecallComparer()).ToList();
                finalPO = finalPO.Union(poList5.Result, new RecallComparer()).ToList();
                finalPO = finalPO.Union(poList6.Result, new RecallComparer()).ToList();

            }

            if (!String.IsNullOrEmpty(command.Data.ProductName) ||
                 !String.IsNullOrEmpty(command.Data.ManufacturerName) ||
                 !String.IsNullOrEmpty(command.Data.ProductType) ||
                 !String.IsNullOrEmpty(command.Data.ProductModel) ||
                 !String.IsNullOrEmpty(command.Data.RecallDateEnd) ||
                 !String.IsNullOrEmpty(command.Data.RecallDateStart))
            {
                if (!String.IsNullOrEmpty(command.Data.ProductName))
                {
                    uriBuilder.AppendFormat("&ProductName={0}", command.Data.ProductName);
                }

                if (!String.IsNullOrEmpty(command.Data.ManufacturerName))
                {
                    uriBuilder.AppendFormat("&manufacturer={0}", command.Data.ManufacturerName);
                }

                if (!String.IsNullOrEmpty(command.Data.ProductType))
                {
                    uriBuilder.AppendFormat("&ProductType={0}", command.Data.ProductType);
                }

                if (!String.IsNullOrEmpty(command.Data.ProductModel))
                {
                    uriBuilder.AppendFormat("&RecallDescription={0}", command.Data.ProductModel);
                }
                if (!String.IsNullOrEmpty(command.Data.RecallDateEnd))
                {
                    uriBuilder.AppendFormat("&RecallDateEnd={0:yyyy‐MM‐dd}", command.Data.RecallDateEnd);
                }

                if (!String.IsNullOrEmpty(command.Data.RecallDateStart))
                {
                    uriBuilder.AppendFormat("&RecallDateStart={0:yyyy‐MM‐dd}", command.Data.RecallDateStart);
                }

                recList = await RecallList(uriBuilder.ToString());
            }

            if (finalPO == null)
            {
                return new SearchQueryResult()
                {
                    ResultCount = recList.Count,
                    Recalls = recList.OrderByDescending(x => x.RecallDate).ToList()
                };
            }
            else if (recList == null)
            {

                return new SearchQueryResult()
                {
                    ResultCount = finalPO.Count,
                    Recalls = finalPO.OrderByDescending(x => x.RecallDate).ToList()

                };

            }
            else
            {
                finalPO = finalPO.Intersect(recList, new RecallComparer()).ToList();

                return new SearchQueryResult()
                {
                    ResultCount = finalPO.Count,
                    Recalls = finalPO.OrderByDescending(x => x.RecallDate).ToList()

                };

            }
        }

        public static async Task<List<Recalls.Models.Recall>> RecallList(String uriBuilder, int? limit = null)
        {
            List<Recalls.Models.Recall> PoList = null;
            using (HttpClient getClient = new HttpClient())
            {
                try
                {
                    // ensure desired encoding is used
                    getClient.DefaultRequestHeaders.Clear();
                    getClient.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("UTF8"));
                    string jsonResult = await getClient.GetStringAsync(uriBuilder);
                    PoList = JsonConvert.DeserializeObject<List<Recalls.Models.Recall>>(jsonResult);
                    PoList = limit.HasValue ? PoList.Take(limit.Value).ToList() : PoList;
                }
                catch (Exception ex)
                {
                    string errorMessage = ex.Message;
                }

            }
            return PoList;
        }

        public async Task<SearchQueryResult> RetrieveLastest(int limit = 15)
        {
            var yearFromNow = DateTime.Today.AddYears(-1).ToString("yyyy-MM-dd");
            var url = $"{SERVICE_ROOT}&RecallDateStart={yearFromNow}";
            var latestRecalls = await RecallList(url, limit);
            if (latestRecalls == null)
            {
                return new SearchQueryResult()
                {
                    ResultCount = 0
                };
            }
            return new SearchQueryResult()
            {
                ResultCount = latestRecalls.Count,
                Recalls = latestRecalls
            };
        }

        public async Task<SearchQueryResult> RetrieveChildrensRecalls(int limit = 15)
        {
            string[] childrenKeyWords = new string[]{
                 "infant",
                 "toy",
                 "toys",
                 "children",
                 "kids",
                 "infants",
                 "toddler",
                 "baby",
                 "babys"
             };
            IDictionary<string, Recall> uniqueRecalls = new Dictionary<string, Recall>();
            string lastYear = DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd");
            foreach (var keyWord in childrenKeyWords)
            {
                string url = $"{SERVICE_ROOT}&RecallDateStart={lastYear}&RecallDescription={keyWord}";
                var newRecalls = await RecallList(url, limit);
                newRecalls.ForEach(r =>
                {
                    if (!uniqueRecalls.ContainsKey(r.RecallNumber) && uniqueRecalls.Count < limit)
                    {
                        uniqueRecalls.Add(r.RecallNumber, r);
                    }
                });
            }

            return new SearchQueryResult()
            {
                ResultCount = uniqueRecalls.Count,
                Recalls = uniqueRecalls.Values.ToList()
            };

        }
    }
}