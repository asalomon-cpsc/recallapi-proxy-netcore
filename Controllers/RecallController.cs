using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Opendata.Recalls.Commands;
using Opendata.Recalls.Models;
using Opendata.Recalls.Repository;
//using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;

namespace Opendata.Recalls.Controllers
{
    [Route("api/[controller]")]
    public class RecallController : Controller
    {
        private readonly IRecallApiProxyRepository _recallRepository;
        //private readonly ILogger _logger; TODO: implement logger

        public RecallController(IRecallApiProxyRepository recallRepository)//,ILogger logger)
        {
            _recallRepository = recallRepository;
            // _logger = logger;
        }
        // GET api/values


        // POST api/recall
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]SearchCommand value)
        {
        
            if (value == null)
            {
                return BadRequest("The supplied request is invalid");
            }
           // var searchObject =  Newtonsoft.Json.JsonConvert.DeserializeObject(value);
            return Ok(await _recallRepository.RetrieveRecall(value));
        }
     
        // GET api/recall
        [HttpGet]
        [Route("latest")]
        public async Task<IActionResult> Latest()
        {

            return Ok(await _recallRepository.RetrieveLastest());
        }

        [HttpGet]
        [Route("children")]
        public async Task<IActionResult> Children()
        {

            return Ok(await _recallRepository.RetrieveChildrensRecalls());
        }


    }
}
