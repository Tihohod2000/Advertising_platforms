using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace Advertising_platforms.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdPlatformsController : ControllerBase
{
    private readonly UploadAdvertisingPlatforms _uploadAdvertisingPlatforms;
    private readonly GettingAdvertisingPlatforms _gettingAdvertisingPlatforms;

    public AdPlatformsController(UploadAdvertisingPlatforms uploadAdvertisingPlatforms, GettingAdvertisingPlatforms gettingAdvertisingPlatforms)
    {
        _uploadAdvertisingPlatforms = uploadAdvertisingPlatforms;
        _gettingAdvertisingPlatforms = gettingAdvertisingPlatforms;
    }

    [HttpGet("search")]
    public Task<IActionResult> Search([FromQuery] string location)
    {
            AdvertisingPlatformByLocalDto result = _gettingAdvertisingPlatforms.AdvertisingPlatformByLocal(location);

            if (result.Success == false)
            {
                throw new KeyNotFoundException("Данные по локации не найдены!");
            }
            
            return Task.FromResult<IActionResult>(Ok(new
            {
                result
            }));
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

        FileReadResultDto result = await _uploadAdvertisingPlatforms.ReadInfoFromFile(file);

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