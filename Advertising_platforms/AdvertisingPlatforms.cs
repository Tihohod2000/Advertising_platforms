namespace Advertising_platforms;

public static class AdvertisingPlatforms
{
    public static Dictionary<string, List<string>> AdvertisingPlatformsHash { get; private set; } = new Dictionary<string, List<string>>();

    private static void AddPlatform(string local, string name)
    {
        //Проверяем записан путь в качестве ключа или нет
        if (AdvertisingPlatformsHash.ContainsKey(local))
        {
            //Проверяем есть ли в списке по ключу название
            if (!AdvertisingPlatformsHash[local].Contains(name))
            {
                //Если нет названия, то добавляем
                AdvertisingPlatformsHash[local].Add(name);
            }
        }
        else
        {
            //Записываем новое значение
            AdvertisingPlatformsHash[local] = new List<string> { name };
        }
    }

    public static AdvertisingPlatformByLocalDto AdvertisingPlatformByLocal(string location)
    {
        var result = new AdvertisingPlatformByLocalDto();
        result.Locals = location;

        if (AdvertisingPlatformsHash.TryGetValue(location, out var value))
        {
            result.Success = true;
            result.Message = "Данные найдены успешно";
            result.Name = value;
        }
        else
        {
            result.Success = false;
            result.Message = $"Данные по локации: {location} не найдены";
            result.Name = [];
        }

        
        
        return result;
    } 

    public static async Task<FileReadResultDto> ReadInfoFromFile(IFormFile file)
    {
        var result = new FileReadResultDto();
        
        try
        {
            if (file == null || file.Length == 0)
            {
                result.Success = false;
                result.ErrorMessage = "Файл не предоставлен или пуст";
                return result;
            }
            
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
                        
                        //добавляем платформу в Dictionary
                        AddPlatform(local, name);
                        
                        //Получаем список ключей
                        var keys = AdvertisingPlatformsHash.Keys;

                        //Добавляем площадки с широкими областями в списки площадок с узкими облостями
                        foreach (var existingKey in keys)
                        {
                            //Пропускаем полностью совподающие локации
                                if(existingKey == local) continue;

                                //Если текущаа локация начинается с других слокаций, но не равны
                                //Добавляем площадку в список
                                //Это нужно чтобы сделать поиск площадок максимально быстрым
                                if (existingKey.StartsWith(local))
                                {
                                    AddPlatform(existingKey, name);
                                }
                        }
                    }
                }
                // return ads;
            }
            result.Success = true;
            result.PlatformsByLocal = AdvertisingPlatformsHash;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = $"Ошибка обработки файла: {ex.Message}";
            // result.RawLines = Array.Empty<string>();
        }

        return result;
    }

    //Метод очистки Dictionary
    private static void ClearDictionary()
    {
        AdvertisingPlatformsHash.Clear();
    }
}