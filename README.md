# Большое ДЗ №2. Синхронное межсервисное взаимодействие


## Содержание
  - [Содержание](#содержание)
  - [Инструкция по запуску](#инструкция-по-запуску)
  - [Архитектура системы](#архитектура-системы)
  - [Спецификация API](#спецификация-api)
  - [Тестирование](#тестирование)

## Инструкция по запуску

1. Скачать архив или клонировать репозиторий (очевидно)
2. Запустить Docker Desktop
3. В корневой папке выполнить команду
   ```
   docker-compose up --build
   ```
4. Во втором терминале выполнить следующие команды:
   ```
   dotnet ef database update --project FileStorage/FileStorage.Infrastructure --startup-project FileStorage/FileStorage.Web --context FileDbContext
   ```
   ```
   dotnet ef database update --project FileAnalysis/FileAnalysis.Infrastructure --startup-project FileAnalysis/FileAnalysis.Web --context AnalysisDbContext
   ```

    Это позволит применить миграции в обоих сервисах. 


## Архитектура системы

Приложение состоит из следующих компонентов:

- **FileStorage** - сервис, отвечающий за хранение и выдачу файлов. Для хранения метаданных каждого файла используется БД PostgreSQL
- **FileAnalysis** - сервис, отвечающий за анализ содержимого файла (кол-во абзацев, слов и всех символов в файле). Также используется БД PostgreSQL для хранения результатов анализа
- **API Gateway** - шлюз для перенаправления запросов каждому из сервисов.

Реализация каждого из сервисов построена на принципах Clean Architecture. 


## Спецификация API

Документацию OpenAPI к каждому компоненту системы можно получить, перейдя по **URL/swagger/v1/swagger.json**, где URL - это адрес конкретного сервиса. 
Приведу краткое описание endpoint'ов API Gateway и методов HTTP:
1. api/gateway/files/upload - POST метод, принимающий на вход файл формата .txt. Содержимое файла передаётся в FileStorage сервис, где вычисляется его хэш, а в случае наличия в БД информации о файлах с таким же хэшем проверяется на 100%-ый плагиат. Если файл с таким содержимым уже есть, возвращается его ID. Если нет - содержимое сохраняется в локальной файловой системе и возвращается ID нового файла.
2. api/gateway/files/{fileId} - GET метод, принимающий на вход ID файла в формате UUID. ID пересылается сервису FileStorage сервис. Если файл с таким ID есть в БД, то возвращается его содержимое. Иначе - информация об ошибке скачивания файла.
3. api/gateway/files/{fileId}/analysis - GET метод, принимающий на вход ID файла в формате UUID. ID пересылается в FileAnalysis сервис, откуда делается запрос в FileStorage на получение содержимого файла с таким ID. В случае успеха сервис FileAnalysis проводит анализ файла: считает кол-во абзацев, слов и символов (все ASCII символы). Результат анализа в формате JSON - возвращаемое значение. Иначе - информация об ошибке, возникшей во время проведения анализа.


## Тестирование

Каждый компонент приложения покрыт тестами: они содержатся в проектах Gateway.Tests, FileStorage.Tests и FileAnalysis.Tests. Информация о покрытии тестами расположена в директориях Gateway.CoverageReport, FileStorage.CoverageReport и FileAnalysis.CoverageReport. Для ознакомления с ней необходимо открыть файл **index.html** в каждой директории.

Для тестирования API endpoint'ов каждого компонента необходимо перейти по URL/swagger, где URL - адрес каждого сервиса.