namespace Advertising_platforms;

public class GettingAdvertisingPlatforms
{
    public AdvertisingPlatformByLocalDto AdvertisingPlatformByLocal(string location)
    {
        var result = new AdvertisingPlatformByLocalDto();
        result.Locals = location;

        if (UploadAdvertisingPlatforms.AdvertisingPlatformsHash.TryGetValue(location, out List<string>? value))
        {
            result.Success = true;
            result.Message = "Данные найдены успешно";
            result.Name = value;
        }
        else
        {
            result.Success = false;
            result.Message = $"Данные по локации: {location} не найдены";
        }


        return result;
    }
}