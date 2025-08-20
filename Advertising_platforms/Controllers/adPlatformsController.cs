using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace Advertising_platforms.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdPlatformsController : ControllerBase
{
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string location)
    {
        // string location = HttpContext.Request.Query["location"].ToString();

        try
        {
            var result = AdvertisingPlatforms.AdvertisingPlatformByLocal(location);

            if (result.Success == false)
            {
                return NotFound(new
                {
                    result
                });
            }
            
            return Ok(new
            {
                result
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return NotFound($"Данные по локации {location} не найдены!");
        }
    }


    [HttpPost("UploadAdPlatform")]
    public async Task<IActionResult> UploadAdPlatform()
    {
        // Проверяем, есть ли файл в запросе
        if (Request.Form.Files.Count == 0)
        {
            return BadRequest(new
            {
                message = "No file uploaded."
            });
        }
        
        // IFormFile file = Request.Form.Files[0];
        FileUploadRequestDto file = new FileUploadRequestDto();
        file.File = Request.Form.Files[0];
        
        if (Path.GetExtension(file.File.FileName).ToLower() != ".txt")
        {
            return BadRequest(new
            {
                message = "Only .txt file."
            });
        }

        FileReadResultDto result = await AdvertisingPlatforms.ReadInfoFromFile(file);
        
        return Ok(new
        {
            result
        });
    }
}