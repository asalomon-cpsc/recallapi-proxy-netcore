
using Opendata.Recalls.Models;
using System.Collections.Generic;


namespace Opendata.Recalls.Repository
{

    public class SearchQueryResult{
        public int ResultCount { get; set; }
        public List<Recall> Recalls { get; set; }
    }

}