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
  manifest.json       # mtime+hash каждого индексированного файла (для --update)
  cache/semantic/     # кэш семантической экстракции (key = hash содержимого файла)
  cost.json           # накопительный учёт токенов
```

### Один граф на весь репозиторий

Per-service графов **нет**. Хранится один общий граф; границы сервисов проходят естественным образом через community-разбиение. При запуске `/graphify src/<ServiceName> --update` инкрементально переэкстрагируются только изменённые файлы в указанной поддиректории.

### Игнорируемые файлы

`.graphifyignore` в корне репозитория исключает .NET build-артефакты (`obj/`, `bin/`, `*.g.cs`, `*.Designer.cs`, `*.AssemblyInfo.cs`, `*ModelSnapshot.cs`, `*.csproj.FileListAbsolute.txt`, `GlobalUsings.cs`). При добавлении новых типов автогена дополни этот файл — иначе `dotnet build` будет триггерить лишнюю переэкстракцию и тратить токены.

### Использование

Перед работой со знакомым сервисом проверь актуальность графа:

```powershell
Get-Date (Get-Item .claude/graphify-out/graph.json).LastWriteTime
```

Если граф покрывает нужный сервис — используй `graph.json` и `GRAPH_REPORT.md` для навигации вместо чтения исходников. Обращайся к исходникам только за деталями, которых нет в графе.

**Не читай `.claude/graphify-out/obsidian/`** — vault для Obsidian, токены тратит, полезной информации не несёт. Используй `graph.json` и `GRAPH_REPORT.md`.
