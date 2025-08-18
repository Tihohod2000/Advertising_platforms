using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace Advertising_platforms.Controllers;

[ApiController]
[Route("api/[controller]")]
public class adPlatformsController : ControllerBase
{
    
    [HttpGet("search")]
    public IActionResult Search()
    {
        string location = HttpContext.Request.Query["location"].ToString();

        var search = AdvertisingPlatforms.Platforms[location];
        
        
        return Ok(new
        {
            search
        });
        
    }


    [HttpPost("UploadAdPlatform")]
    public async Task<IActionResult> UploadAdPlatform()
    {
        // Проверяем, есть ли файл в запросе
        if (Request.Form.Files.Count == 0)
        {
            return BadRequest("No file uploaded.");
        }

        var file = Request.Form.Files[0];
        
        if (Path.GetExtension(file.FileName).ToLower() != ".txt")
        {
            return BadRequest("Only .txt files.");
        }
        
        using (var reader = new StreamReader(file.OpenReadStream()))
        {
            string fileContent = await reader.ReadToEndAsync();
            fileContent = fileContent.Replace("\r", "");

            string[] ads = fileContent.Split("\n");

            foreach (var curr in ads)
            {
                string name = curr.Split(":")[0];
                string[] locals = curr.Split(":")[1].Split(",");
                for(byte i = 0; i < locals.Length; i++)
                {

                    AdvertisingPlatforms.AddPlatform(locals[i], name);

                    while (true)
                    {

                        int lastSlashIndex = locals[i].LastIndexOf('/');

                        if (lastSlashIndex == 0)
                        {
                            break;
                        }

                        if (lastSlashIndex > 0) 
                        {
                            locals[i] = locals[i].Substring(0, lastSlashIndex);
                            AdvertisingPlatforms.AddPlatform(locals[i], name);
                        }
                        else
                        {
                            break;
                        }
                    }
                }

            }
            
            return Ok(new {
                Platforms = AdvertisingPlatforms.Platforms,
                FileName = file.FileName,
                Content = fileContent,
                Size = file.Length
            });
        }
    }
}