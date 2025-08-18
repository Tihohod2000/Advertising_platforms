namespace Advertising_platforms;

public class AdvertisingPlatforms
{
    public static Dictionary<string, List<string>> Platforms { get; set; } = new Dictionary<string, List<string>>();

    public static void AddPlatform(string local, string name)
    {
        if (Platforms.ContainsKey(local))
        {
            
            Platforms[local].Add(name);
        }
        else
        {
            Platforms[local] = new List<string> { name };
        }
    }
}