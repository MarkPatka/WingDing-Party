---
# Claude Code Configuration
# Claude Code–специфичная конфигурация для проекта WingDing Party.
# Документация по проекту (архитектура, команды, порты) — в корневом CLAUDE.md.
# Сервисная документация — в src/<ServiceName>/CLAUDE.md.
---

## Skills

### /graphify
When the user types `/graphify`, invoke the Skill tool with `skill: "graphify"` before doing anything else.

- Output directory: `.claude/graphify-out/`
- One service at a time — rebuilding for a different service replaces the previous graph
- Built-in Claude Code skill; project-level notes: `.claude/skills/graphify/`

## Knowledge Graph

Каждый сервис может иметь граф знаний. Граф строится командой `/graphify src/<ServiceName>` и сохраняется в `.claude/graphify-out/`. **Один граф за раз** — при смене сервиса нужно пересобрать.

Перед работой с любым сервисом проверь, актуален ли граф:

```powershell
Get-Content .claude/graphify-out/manifest.json | Select-String "source"
```

Если граф покрывает нужный сервис — используй его для навигации вместо чтения исходников. Обращайся к исходникам только для деталей реализации, которых нет в графе.

**Не читай `.claude/graphify-out/obsidian/`** — это vault для визуализации в Obsidian, не несёт полезной информации и только тратит токены. Используй `graph.json` и `GRAPH_REPORT.md`.

Инструкции по использованию графа для конкретного сервиса — в `src/<ServiceName>/CLAUDE.md`.
