using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace Advertising_platforms.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdPlatformsController : ControllerBase
{
    private readonly AdvertisingPlatforms _advertisingPlatforms;

    public AdPlatformsController(AdvertisingPlatforms advertisingPlatforms)
    {
        _advertisingPlatforms = advertisingPlatforms;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string location)
    {
        // string location = HttpContext.Request.Query["location"].ToString();

        try
        {
            AdvertisingPlatformByLocalDto result = _advertisingPlatforms.AdvertisingPlatformByLocal(location);

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


    [HttpPost("UploadAdPlatforms")]
    public async Task<IActionResult> UploadAdPlatforms(IFormFile fileUpload)
    {
        if (fileUpload == null || fileUpload.Length == 0)
        {
            return BadRequest(new { message = "No file uploaded." });
        }
        
        if (Path.GetExtension(fileUpload.FileName).ToLower() != ".txt")
        {
            return BadRequest(new
            {
                message = "Only .txt file."
            });
        }

        FileUploadRequestDto file = new FileUploadRequestDto(fileUpload);

        FileReadResultDto result = await _advertisingPlatforms.ReadInfoFromFile(file);
        
        return Ok(new
        {
            result
        });
    }
}