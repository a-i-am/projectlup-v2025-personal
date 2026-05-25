# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Unity 6 (6000.0.62f1) multi-game project containing 5 playable game modules (RL, ST, ES, PCR, DSG) running within a shared framework. Target platform: Windows Standalone 64-bit. C# 9.0 / .NET Standard 2.1.

## Build & Development

This is a Unity project — there are no CLI build scripts. All building is done through the Unity Editor or Visual Studio.

- **Open project**: Launch Unity Hub and open this folder
- **Build**: Unity Editor → File > Build Settings → Build
- **Scripts**: Compiled automatically by Unity; Visual Studio solution (`projectlup-v2025.sln`) is auto-generated
- **No test framework** is configured in this project

## Code Architecture

### Asset Directory Structure

Assets are organized numerically:
- `0_System/` — Asset bundles, system-level assets
- `1_Arts/` — Art assets, organized by game module (DSG, ES, PCR, ST)
- `2_Scripts/` — All C# source (608 files, ~25K LOC)
- `3_Prefabs/` — Prefabs by module
- `7_3thParty/` — Third-party libs (DOTween, Joystick Pack, TextMeshPro)
- `8_Editor/` — Custom Unity Editor tools and windows
- `9_Scenes/` — Scenes organized by module

### Script Organization (`Assets/2_Scripts/`)

```
Framework/      # CSV/JSON helpers, quest system, inventory
Managers/       # Core singleton managers
_Base/          # Abstract base classes
Data/           # Static/runtime data loaders
Games/
  Common/       # Shared game utilities
  RL/           # Roguelike (133 files)
  ES/           # Extraction Shooter (137 files)
  PCR/          # Production/Construction/Resource (115 files)
  DSG/          # Deck Strategy Game (71 files)
  ST/           # Shooting (65 files)
```

### Core Patterns

**Singleton Managers** — All managers extend `LUP.Singleton<T>` which auto-creates instances and uses `DontDestroyOnLoad`. Key managers: `DataManager`, `StageManager`, `ItemManager`, `QuestManager`, `SoundManager`, `InventoryManager`, `ResourceManager`.

**Stage Lifecycle** — All game modules extend `BaseStage` (abstract). Each stage follows: `OnStageEnter()` → `OnStageStay()` → `OnStageExit()`. Transitions are handled by `StageManager` with fade animations.

**Data Pipeline**:
- **Static data**: CSV files → loaded via `CSVDataSourceAdapter`
- **Runtime data**: JSON files in `Assets/Resources/Data/SavedData/` (e.g., `production_runtime.json`)
- `DataManager` orchestrates loading/saving for both types

**Central Enums** (`Define` namespace):
- `StageKind`: Unknown, Debug, Main, Intro, RL, ST, ES, PCR, DSG, Tutorial
- `ItemType`: Weapon, Armor, Consumable, Material, Quest, Currency
- `RuntimeDataType`: maps stage types to their JSON save filenames
- `AssetBundleKind`: Video, Audio, Image, VFX, GUI, Model, Shader, Data, …

### Game Module Structure

Each game module (RL, ST, ES, PCR, DSG) has:
- Its own scene(s) in `9_Scenes/<MODULE>/`
- Scripts under `Games/<MODULE>/`
- Art in `1_Arts/<MODULE>/`
- Prefabs in `3_Prefabs/<MODULE>/`
- Its own static/runtime data loaders extending the framework

## Character Reference

Character classes referenced in save data and scripts:
- `CH_F001`: Magic
- `CH_F002`: One-handed sword
- `CH_F003`: Two-handed sword
- `CH_M001`: Guns (pistol, shotgun, etc.)
- `CH_M002`: Throwing
- `CH_M003`: Axe

## Communication

모든 응답은 한국어로 작성한다. 사용자가 명시적으로 다른 언어를 요청할 때만 변경한다.

## Branch Conventions

Active feature branches use `feature/<MODULE>/<FEATURE>` naming (e.g., `feature/PCR/PSE_Current`).
