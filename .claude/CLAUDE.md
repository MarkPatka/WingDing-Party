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

### ВАЖНО: расположение файлов graphify

**Все файлы graphify хранятся в `.claude/graphify-out/`, а НЕ в `graphify-out/` в корне репозитория.**

Graphify-скилл пишет в `graphify-out/` (относительно CWD) по умолчанию — это не то место. В конце каждого запуска нужно переместить результаты в `.claude/graphify-out/`.

#### Перед запуском любого graphify-шага:

1. Убедись, что существующий граф и кэш взяты из `.claude/graphify-out/`, а не из корневого `graphify-out/`
2. Если существует `.claude/graphify-out/manifest.json`, но нет `graphify-out/manifest.json` — скопируй манифест перед запуском `detect_incremental`, иначе он посчитает все файлы новыми:

```bash
cp -r .claude/graphify-out/. graphify-out/
```

3. Семантический кэш: `.claude/graphify-out/cache/semantic/`
4. AST кэш: `src/<ServiceName>/graphify-out/cache/ast/`

#### После завершения любого graphify-запуска:

```bash
cp graphify-out/graph.json .claude/graphify-out/graph.json
cp graphify-out/graph.html .claude/graphify-out/graph.html
cp graphify-out/GRAPH_REPORT.md .claude/graphify-out/GRAPH_REPORT.md
cp graphify-out/manifest.json .claude/graphify-out/manifest.json
cp graphify-out/cost.json .claude/graphify-out/cost.json
cp -n graphify-out/cache/semantic/* .claude/graphify-out/cache/semantic/ 2>/dev/null
rm -rf graphify-out/
```

### Использование

Каждый сервис может иметь граф знаний. Граф строится командой `/graphify src/<ServiceName>` и сохраняется в `.claude/graphify-out/`. **Один граф за раз** — при смене сервиса нужно пересобрать.

Перед работой с любым сервисом проверь, актуален ли граф:

```powershell
Get-Content .claude/graphify-out/manifest.json | Select-String "source"
```

Если граф покрывает нужный сервис — используй его для навигации вместо чтения исходников. Обращайся к исходникам только для деталей реализации, которых нет в графе.

**Не читай `.claude/graphify-out/obsidian/`** — это vault для визуализации в Obsidian, не несёт полезной информации и только тратит токены. Используй `graph.json` и `GRAPH_REPORT.md`.

Инструкции по использованию графа для конкретного сервиса — в `src/<ServiceName>/CLAUDE.md`.
