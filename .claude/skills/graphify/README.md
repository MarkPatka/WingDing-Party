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

Выходные файлы сохраняются в `.claude/graphify-out/` (не в корневой `graphify-out/`). Graphify-скилл пишет в `graphify-out/` по умолчанию — после каждого запуска результаты переносятся в `.claude/graphify-out/` и корневой удаляется. Подробные инструкции — в `.claude/CLAUDE.md`.

**Один граф за раз.** При смене сервиса граф пересобирается полностью.

## Инкрементальное обновление

```
/graphify src/<ServiceName> --update
```

## Проверка актуальности

```powershell
Get-Content graphify-out/manifest.json | Select-String "source"
```
