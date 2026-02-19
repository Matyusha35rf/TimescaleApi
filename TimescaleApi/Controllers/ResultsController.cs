using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Services.Interfaces;
using DataAccess.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/results")]
    public class ResultsController(IResultQueryService resultQueryService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<Result>>> GetResults(
            [FromQuery] ResultFilter filter,
            CancellationToken cancellationToken)
        {
            var results = await resultQueryService.GetFilteredResultsAsync(filter, cancellationToken);
            return Ok(results);
        }
    }
}