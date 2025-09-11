using System.Collections.Concurrent;

namespace Advertising_platforms.Models;

// DTO для представления данных о рекламных площадках
public class AdvertisingPlatformByLocalDto
{
    public string Locals { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Name { get; set; } = new List<string>();
}

// DTO для результата чтения файла
public class FileReadResultDto
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public ConcurrentDictionary<string, List<string>> PlatformsByLocal { get; set; } = new ConcurrentDictionary<string, List<string>>();
}

// DTO для передачи файла
public class FileUploadRequestDto(IFormFile file)
{
    public IFormFile _file { get; private set; } = file;
}