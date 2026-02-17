using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Services;
namespace WebApi.Controllers
{
    [ApiController]
    [Route("CsvFile")]
    public class CsvFileController(IFileProcessingService fileProcessingService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> UploadCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файл не выбран или пуст");
            var result = await fileProcessingService.ProcessCsvFileAsync(file);
            return Ok(result);
        }
    }
}
