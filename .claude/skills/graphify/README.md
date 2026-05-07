# graphify

Сторонний скилл Claude Code от [safishamsi](https://github.com/safishamsi/graphify).
Превращает любой ввод (код, документы, изображения) в граф знаний → кластеризованные сообщества → HTML + JSON + audit report.

## Установка (требуется Python 3.10+)

```
pip install graphifyy
```

После установки скилл автоматически становится доступен в Claude Code как `/graphify`.

## Использование в проекте

```
/graphify src/<ServiceName>
```

Выходные файлы сохраняются в `graphify-out/` в корне репозитория (папка gitignored — `obsidian/`, `cache/`; отслеживается только `graph.json` и `manifest.json`).

**Один граф за раз.** При смене сервиса граф пересобирается полностью.

## Инкрементальное обновление

```
/graphify src/<ServiceName> --update
```

## Проверка актуальности

```powershell
Get-Content graphify-out/manifest.json | Select-String "source"
```
