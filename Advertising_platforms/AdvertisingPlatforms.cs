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
        try
        {
            
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                string fileContent = await reader.ReadToEndAsync();
                fileContent = fileContent.Replace("\r", "");

                string[] ads = fileContent.Split("\n");
                
                await ClearDictionary();

                foreach (var line in ads)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] parts = line.Split(":",  StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length != 2) 
                        continue;
                    
                    if (string.IsNullOrWhiteSpace(parts[0]) || string.IsNullOrWhiteSpace(parts[1])) 
                        continue;

                    // if (!parts[1].StartsWith("/")) 
                    //     continue;
                    
                    
                    string name = parts[0];
                    string[] locals = parts[1].Split(",");
                   
                    
                    
                    for(byte i = 0; i < locals.Length; i++)
                    {
                        if(!locals[i].StartsWith("/")) continue;

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
        catch (Exception e)
        {
            return [];
        }
    }

    private static async Task ClearDictionary()
    {
        AdvertisingPlatformsHash.Clear();
    }
}