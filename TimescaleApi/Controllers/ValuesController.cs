using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Services.Interfaces;
using DataAccess.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/values")]
    public class ValuesController(IValueQueryService valueQueryService) : ControllerBase
    {
        [HttpGet("{fileName}/last-ten")]
        public async Task<ActionResult<List<ValueRecord>>> GetLastTenValues(
            string fileName,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return BadRequest("Имя файла не может быть пустым");

            var values = await valueQueryService.GetLastTenValuesAsync(fileName, cancellationToken);
            return Ok(values);
        }
    }
}