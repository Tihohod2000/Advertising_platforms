namespace Advertising_platforms;

public static class AdvertisingPlatforms
{
    public static Dictionary<string, List<string>> AdvertisingPlatformsHash { get; private set; } = new Dictionary<string, List<string>>();

    public static void AddPlatform(string local, string name)
    {
        if (AdvertisingPlatformsHash.ContainsKey(local))
        {
            List<string> platforms = AdvertisingPlatformsHash[local];
            foreach (var platform in platforms)
            {
                if (platform == name)
                {
                    return;
                }
            }
            
            AdvertisingPlatformsHash[local].Add(name);
        }
        else
        {
            AdvertisingPlatformsHash[local] = new List<string> { name };
        }
    }

    public static async Task<string[]> ReadInfoFromFile(IFormFile file)
    {
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

                    AddPlatform(locals[i], name);

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
                            AddPlatform(locals[i], name);
                        }
                        else
                        {
                            break;
                        }
                    }
                }

            }
            
            return ads;
        }
    }

    public static async Task ClearDictionary()
    {
        AdvertisingPlatformsHash.Clear();
    }
}