using System.Collections.Concurrent;
using Advertising_platforms.Models;

namespace Advertising_platforms.Services;

public class UploadAdvertisingPlatforms
{
    public static ConcurrentDictionary<string, List<string>> AdvertisingPlatformsHash { get; private set; } =
        new ConcurrentDictionary<string, List<string>>();

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


    public async Task<FileReadResultDto> ReadInfoFromFile(FileUploadRequestDto fileUpload)
    {
        var result = new FileReadResultDto();
        var file = fileUpload._file;


        if (file.Length == 0)
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
                for (int j = 0; j < locals.Length; j++)
                {
                    string local = locals[j].Trim();
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