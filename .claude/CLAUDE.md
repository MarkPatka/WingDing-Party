---
# Claude Code Configuration
# Claude Code–специфичная конфигурация для проекта WingDing Party.
# Документация по проекту (архитектура, команды, порты) — в корневом CLAUDE.md.
# Сервисная документация — в src/<ServiceName>/CLAUDE.md.
---

## Skills

### /graphify
When the user types `/graphify`, invoke the Skill tool with `skill: "graphify"` before doing anything else.

- Output directory: `graphify-out/` at repo root
- One service at a time — rebuilding for a different service replaces the previous graph
- Built-in Claude Code skill; project-level notes: `.claude/skills/graphify/`

## Knowledge Graph

Каждый сервис может иметь граф знаний. Граф строится командой `/graphify src/<ServiceName>` и сохраняется в `graphify-out/` в корне репозитория. **Один граф за раз** — при смене сервиса нужно пересобрать.

Перед работой с любым сервисом проверь, актуален ли граф:

```powershell
Get-Content graphify-out/manifest.json | Select-String "source"
```

Если граф покрывает нужный сервис — используй его для навигации вместо чтения исходников. Обращайся к исходникам только для деталей реализации, которых нет в графе.

Инструкции по использованию графа для конкретного сервиса — в `src/<ServiceName>/CLAUDE.md`.
