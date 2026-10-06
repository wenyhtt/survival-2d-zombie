# 🎮 Game Overview

**Survival 2D Zombie** is a top-down 2D wave-based zombie survival game. The player must survive against escalating waves of zombies, collect score by killing enemies, and spend it on weapons from an in-game shop NPC.

---

## 🎯 Core Game Loop

```
START → Countdown → Wave Begins → Kill Enemies → Earn Score
  ↑                                                     ↓
  └──────────── Next Wave (harder) ← Wave Complete ←───┘
                                                        ↓
                                              Visit GunMan Shop
                                              (buy weapons with score)
```

1. **Wave countdown** starts at the beginning or after each wave ends.
2. Enemies spawn in from the edges or randomly within the map.
3. The player kills enemies to earn score points.
4. After all enemies die, the wave completes and a countdown begins again.
5. Between waves, the player can visit the **GunMan NPC** to buy new weapons.
6. When the player's HP drops to 0, the **Game Over** screen is shown.

---

## ✨ Key Features

| Feature | Description |
|---|---|
| Wave System | Infinite escalating waves with random enemy count increase per wave |
| Ranged Weapons | Shoot in the player's facing direction with spread support |
| Melee Weapon | Close-range attack with visual thrust animation |
| Weapon Switching | Switch between owned weapons using number keys (1–9) |
| In-Game Shop | Buy weapons from a GunMan NPC using earned score |
| Health System | Shared `Health` component with hit flash, events, and scaling enemy HP |
| Behavior Tree AI | Enemies use Unity Behavior trees to chase and attack the player |
| Directional Animations | Player and enemies animate correctly based on movement direction |
| Score System | Enemies award score on death; score is used as currency |
| Game Over Screen | Pause game, play SFX, restart or return to main menu |

---

## 🗺️ Scenes

| Scene | Purpose |
|---|---|
| `MainMenu` | Title screen with menu navigation |
| `Main` | Primary gameplay scene |

---

## 🎨 Art Assets

- **Zombie Apocalypse Tileset** — provides all sprites, tiles, animations, and UI elements
  - Urban, rural, and environmental tiles
  - Bird, water, windmill, and other animated elements
  - Zombie sprites and poster art
