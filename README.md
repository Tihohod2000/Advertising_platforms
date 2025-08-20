Запуск происходит в докер контейнере
Для запуска сервиса требуется выполнить команду

```
docker-compose up --build
```

Созданы 2 endpoint 
1. Загрузка файла в формате .txt по пути
   ```
   http://localhost:80/api/adPlatforms/UploadAdPlatform
   ```
   Пример файла:
   ```
    Яндекс.Директ:/ru
    Ревдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik
    Газета уральских москвичей:/ru/msk,/ru/permobl,/ru/chelobl
    Крутая реклама:/ru/svrd
   ```
