using Opendata.Recalls.Commands;
using Opendata.Recalls.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Opendata.Recalls.Repository
{
    public interface IRecallApiProxyRepository
    {
        Task<SearchQueryResult> RetrieveRecall(SearchCommand command);
        Task<SearchQueryResult> RetrieveLastest(int limit=15);

        Task<SearchQueryResult> RetrieveChildrensRecalls(int limit=15);
    }
}