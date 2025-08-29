namespace Advertising_platforms;

public class AdvertisingPlatforms
{
    public static Dictionary<string, List<string>> AdvertisingPlatformsHash { get; private set; } =
        new Dictionary<string, List<string>>();

    private void AddPlatform(string local, string name)
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

    private void AddPlatform(string local, List<string> names)
    {
        //Проверяем записан путь в качестве ключа или нет
        if (AdvertisingPlatformsHash.ContainsKey(local))
        {
            foreach (var name in names)
            {
                //Проверяем есть ли в списке по ключу название
                if (!AdvertisingPlatformsHash[local].Contains(name))
                {
                    //Если нет названия, то добавляем
                    AdvertisingPlatformsHash[local].Add(name);
                }
            }
        }
        else
        {
            if (names.Count <= 0)
            {
                AdvertisingPlatformsHash[local] = new List<string>();
            }
            else
            {
                //Записываем новое значение
                AdvertisingPlatformsHash[local].AddRange(names);
            }
            
        }
    }

    public AdvertisingPlatformByLocalDto AdvertisingPlatformByLocal(string location)
    {
        var result = new AdvertisingPlatformByLocalDto();
        result.Locals = location;

        if (AdvertisingPlatformsHash.TryGetValue(location, out List<string>? value))
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

    public async Task<FileReadResultDto> ReadInfoFromFile(FileUploadRequestDto fileUpload)
    {
        var result = new FileReadResultDto();
        var file = fileUpload._file;

        
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

                    string[] parts = line.Split(":", StringSplitOptions.RemoveEmptyEntries);

                    //Проверяем наличие названия площадки и наличие путей
                    if (parts.Length != 2)
                        continue;

                    //Если название или локация пусты, то пропускаем это строку
                    if (string.IsNullOrWhiteSpace(parts[0]) || string.IsNullOrWhiteSpace(parts[1]))
                        continue;

                    string name = parts[0];
                    string[] locals = parts[1].Trim().Split(",");

                    //поочереди добовляем пути для площадки
                    for (byte i = 0; i < locals.Length; i++)
                    {
                        string local = locals[i].Trim();
                        //Если путь начинает не с /, то пропускаем
                        if (!local.StartsWith("/")) continue;

                        //добавляем платформу в Dictionary
                        AddPlatform(local, name);

                        int index = local.LastIndexOf("/", StringComparison.Ordinal);
                        
                        while (index != 0)
                        {
                            local = local.Substring(0, index);
                            AddPlatform(local, new List<string>());
                            index = local.LastIndexOf("/", StringComparison.Ordinal);
                        }
                        
                        

                        //Получаем список ключей
                        var keys = AdvertisingPlatformsHash.Keys;

                        //Добавляем площадки с широкими областями в списки площадок с узкими облостями
                        foreach (var firstKey in keys)
                        {
                            foreach (var secondKey in keys)
                            {
                                //Пропускаем полностью совподающие локации
                                if (firstKey == secondKey) continue;


                                if (firstKey.StartsWith(secondKey))
                                {
                                    AddPlatform(firstKey, AdvertisingPlatformsHash[secondKey]);
                                }
                            }
                        }
                    }
                }
            }

            result.Success = true;
            result.PlatformsByLocal = AdvertisingPlatformsHash;

        return result;
    }

    //Метод очистки Dictionary
    private static void ClearDictionary()
    {
        AdvertisingPlatformsHash.Clear();
    }
}