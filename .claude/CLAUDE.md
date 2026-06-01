---
# Claude Code Configuration
# Claude Code–специфичная конфигурация для проекта WingDing Party.
# Документация по проекту (архитектура, команды, порты) — в корневом CLAUDE.md.
# Сервисная документация — в src/<ServiceName>/CLAUDE.md.
---

## Skills

### /graphify
When the user types `/graphify`, invoke the Skill tool with `skill: "graphify"` before doing anything else.

- Built-in Claude Code skill; project-level notes: `.claude/skills/graphify/`

## Knowledge Graph

### Расположение

Граф знаний хранится в `.claude/graphify-out/`. В корне репозитория есть **симлинк** `graphify-out/` → `.claude/graphify-out/`, поэтому graphify-скилл пишет туда же по умолчанию. Никакая ручная синхронизация между `graphify-out/` и `.claude/graphify-out/` **не нужна** — это один и тот же каталог.

```
.claude/graphify-out/
  graph.json          # сырые данные графа
  graph.html          # интерактивная визуализация
  GRAPH_REPORT.md     # отчёт с god nodes, community-разбиением, surprising connections
  cache/semantic/     # кэш семантической экстракции (key = hash содержимого файла)
  obsidian/           # vault для Obsidian (НЕ читать — см. ниже)
```

### Один граф на весь репозиторий, перезаписываемый «слот»

Per-service графов **нет** и per-service папок `graphify-out/` тоже быть не должно. Хранится один общий граф в `.claude/graphify-out/`, и он перезаписывается при каждом запуске под ту директорию, которую индексировали последней.

Это значит: если `GRAPH_REPORT.md` начинается с `# Graph Report - src/AuthService`, а сейчас нужен ClubService — граф **неактуален**, нужно перезапустить `/graphify src/ClubService`. Заголовок отчёта — основной маркер охвата.

Границы сервисов внутри одной сборки проходят естественным образом через community-разбиение. При запуске `/graphify src/<ServiceName> --update` инкрементально переэкстрагируются только изменённые файлы в указанной поддиректории — но только если предыдущий граф покрывал тот же scope.

### Игнорируемые файлы

`.graphifyignore` в корне репозитория исключает .NET build-артефакты (`obj/`, `bin/`, `*.g.cs`, `*.Designer.cs`, `*.AssemblyInfo.cs`, `*ModelSnapshot.cs`, `*.csproj.FileListAbsolute.txt`, `GlobalUsings.cs`). При добавлении новых типов автогена дополни этот файл — иначе `dotnet build` будет триггерить лишнюю переэкстракцию и тратить токены.

### Использование

Перед работой со знакомым сервисом проверь **scope** и **возраст** графа:

```powershell
# scope — какой сервис в текущем «слоте»
Select-String -Path .claude/graphify-out/GRAPH_REPORT.md -Pattern '^# Graph Report' | Select-Object -First 1
# возраст
Get-Date (Get-Item .claude/graphify-out/graph.json).LastWriteTime
```

Если scope совпадает с нужным сервисом и граф свежий — используй `graph.json` и `GRAPH_REPORT.md` для навигации вместо чтения исходников. Обращайся к исходникам только за деталями, которых нет в графе. Если scope не тот — либо перезапусти `/graphify` под нужный сервис, либо работай напрямую с кодом.

**Не читай `.claude/graphify-out/obsidian/`** — vault для Obsidian, токены тратит, полезной информации не несёт. Используй `graph.json` и `GRAPH_REPORT.md`.
