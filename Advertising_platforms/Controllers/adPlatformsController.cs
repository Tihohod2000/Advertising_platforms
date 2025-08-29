using System.ComponentModel.DataAnnotations;
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
            AdvertisingPlatformByLocalDto result = _advertisingPlatforms.AdvertisingPlatformByLocal(location);

            if (result.Success == false)
            {
                throw new KeyNotFoundException("Данные по локации не найдены!");
            }
            
            return Ok(new
            {
                result
            });
    }


    [HttpPost("UploadAdPlatforms")]
    public async Task<IActionResult> UploadAdPlatforms(IFormFile fileUpload)
    {
        if (fileUpload == null)
        {
            throw new ArgumentException("No file uploaded.");
        }
        
        if (Path.GetExtension(fileUpload.FileName).ToLower() != ".txt")
        {
            throw new ArgumentException("Only .txt file.");
        }

        FileUploadRequestDto file = new FileUploadRequestDto(fileUpload);

        FileReadResultDto result = await _advertisingPlatforms.ReadInfoFromFile(file);

        if (result.Success == false)
        {
            throw new ValidationException("Не корректный файл!");
        }
        
        return Ok(new
        {
            result
        });
    }
}