using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace Advertising_platforms.Controllers;

[ApiController]
[Route("api/[controller]")]
public class adPlatformsController : ControllerBase
{
    [HttpGet("search")]
    public async Task<IActionResult> Search()
    {
        string location = HttpContext.Request.Query["location"].ToString();

        try
        {
            var search = AdvertisingPlatforms.AdvertisingPlatformsHash[location];
            return Ok(new
            {
                search
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
            return BadRequest("No file uploaded.");
        }


        IFormFile file = Request.Form.Files[0];


        if (Path.GetExtension(file.FileName).ToLower() != ".txt")
        {
            return BadRequest("Only .txt file.");
        }

        await AdvertisingPlatforms.ClearDictionary();
        string[] fileContent = await AdvertisingPlatforms.ReadInfoFromFile(file);


        return Ok(new
        {
            FileName = file.FileName,
            Size = file.Length,
            Content = fileContent,
            Platforms = AdvertisingPlatforms.AdvertisingPlatformsHash
        });
    }
}