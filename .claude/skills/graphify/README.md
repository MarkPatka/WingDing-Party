# graphify

Встроенный скилл Claude Code. Превращает любой ввод (код, документы, изображения) в граф знаний → кластеризованные сообщества → HTML + JSON + audit report.

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
