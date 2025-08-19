namespace Advertising_platforms;

public static class AdvertisingPlatforms
{
    public static Dictionary<string, List<string>> AdvertisingPlatformsHash { get; private set; } = new Dictionary<string, List<string>>();

    private static void AddPlatform(string local, string name)
    {
        //Проверяем записан путь в качестве ключа или нет
        if (AdvertisingPlatformsHash.ContainsKey(local))
        {
            //получаем список названий по ключу(локации) 
            List<string> platforms = AdvertisingPlatformsHash[local];
            
            //проверяем есть ли название площадки в списке
            foreach (var platform in platforms)
            {
                //локация уже записана, останавливаем
                if (platform == name)
                {
                    return;
                }
            }
            
            //есть ключ, но не записано название
            //получаем список по ключу(локации) и добавляем в Dictionary
            AdvertisingPlatformsHash[local].Add(name);
            
        }
        else
        {
            //Записываем новое значение
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
                
                //Очищаем Dictionary
                ClearDictionary();

                foreach (var line in ads)
                {
                    // Проверям пустали строка
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] parts = line.Split(":",  StringSplitOptions.RemoveEmptyEntries);

                    //Проверяем наличие названия площадки и наличие путей
                    if (parts.Length != 2) 
                        continue;
                    
                    //Если название или локация пусты, то пропускаем это строку
                    if (string.IsNullOrWhiteSpace(parts[0]) || string.IsNullOrWhiteSpace(parts[1])) 
                        continue;
                    
                    string name = parts[0];
                    string[] locals = parts[1].Trim().Split(",");
                    
                    //поочереди добовляем пути для площадки
                    for(byte i = 0; i < locals.Length; i++)
                    {
                        string local = locals[i].Trim();
                        //Если путь начинает не с /, то пропускаем
                        if(!local.StartsWith("/")) continue;

                        //Добавляем в Dictionary
                        AddPlatform(local, name);

                        //Сокращаем путь по одному уровн вложенности
                        while (true)
                        {

                            int lastSlashIndex = local.LastIndexOf('/');

                            if (lastSlashIndex == 0)
                            {
                                break;
                            }

                            //отсекаем последнюю вложенность и добавляем в Dictionary
                            if (lastSlashIndex > 0) 
                            {
                                
                                local = local.Substring(0, lastSlashIndex);
                                AddPlatform(local, name);
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
        catch (Exception)
        {
            return [];
        }
    }

    //Метод очистки Dictionary
    private static void ClearDictionary()
    {
        AdvertisingPlatformsHash.Clear();
    }
}