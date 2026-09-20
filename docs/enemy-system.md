# 🧟 Enemy System

Deep dive into wave spawning, AI behavior, and difficulty scaling.

---

## 🌊 Wave System

The `EnemySpawner` component manages the full wave lifecycle using a three-state machine:

```
┌─────────────────────────────────────────────────────────────────┐
│                     Wave State Machine                          │
│                                                                 │
│  ┌──────────────┐    timer ≤ 0    ┌──────────────┐             │
│  │ CountingDown │ ──────────────► │   Spawning   │             │
│  └──────────────┘                 └──────────────┘             │
│         ▲                               │                       │
│         │                     all enemies spawned               │
│         │                               ▼                       │
│         │                    ┌──────────────────────┐          │
│         └────────────────────│  WaitingForDeath      │          │
│          all enemies dead     └──────────────────────┘          │
└─────────────────────────────────────────────────────────────────┘
```

### State Descriptions

| State | Behavior |
|---|---|
| `CountingDown` | Decrements `waveCountdown`. Shows countdown in `WaveUI`. |
| `Spawning` | Spawns one enemy per `spawnInterval` until `maxEnemiesAlive` or `currentWaveEnemyCount` is reached. |
| `WaitingForDeath` | Monitors `spawnedEnemies` list. When empty, calls `CompleteWave()`. |

---

## 📈 Difficulty Scaling

Each completed wave:
1. Increments `currentWaveIndex`.
2. Adds a **random amount** (`waveIncrementMin` to `waveIncrementMax`) to `currentWaveEnemyCount`.
3. New enemies get **bonus HP**: `Random.Range(10, 21) × waveIndex` via `Health.AddBonusHealth()`.

**Example progression:**

| Wave | Base Enemies | HP Bonus (example) |
|---|---|---|
| 1 | 5 | +0 (no bonus) |
| 2 | 5 + 3 = 8 | +10–20 × 1 = +10–20 HP |
| 3 | 8 + 4 = 12 | +10–20 × 2 = +20–40 HP |
| 5 | ~20 | +50–100 HP |

---

## 📍 Spawn Positions

Controlled by `spawnOnEdge` toggle in the Inspector:

**Edge Spawning (`spawnOnEdge = true`)**
Randomly picks one of 4 edges (top, bottom, left, right) and spawns at a random point along it.
```
   ┌──────T────────┐
   L              R
   └──────B────────┘
```

**Area Spawning (`spawnOnEdge = false`)**
Random position within the rectangular `areaSize` zone centered on the Spawner's transform.

> The spawn zone is visualized as a Gizmo wire cube in the Unity Editor.

---

## 🎲 Enemy Type Shuffling

When multiple enemy prefabs are configured in `enemyPrefabs`, the spawner picks one **type per wave** using a Fisher-Yates shuffle. The shuffle deck is refilled and re-shuffled once all types have been used.

This ensures all types appear before any type repeats, adding variety without pure randomness.

---

## 🤖 AI: Behavior Tree

Enemies use **Unity Behavior** (Behavior Trees) with two custom leaf node actions:

### `ChaseAction`
- Reads the `Player` blackboard variable (auto-finds by tag if null/inactive)
- Moves toward the player using `Rigidbody2D.linearVelocity`
- Stops and returns `Success` when within `StopDistance`
- Returns `Running` while chasing

### `AttackAction`
- Executes when within attack range (typically after `ChaseAction` returns `Success`)
- Calls `Health.TakeDamage(damage)` on the player
- Respects `AttackCooldown` to prevent spam

### Typical BT Structure
```
Selector
  └── Sequence
        ├── ChaseAction    (move toward player)
        └── AttackAction   (hit player when close)
```

---

## 🎨 Enemy Animations

`EnemyFacing.cs` drives animations by reading `Rigidbody2D.linearVelocity`:

| Velocity Direction | Animation Played |
|---|---|
| `|x| > |y|` (horizontal) | `walkSideClip` + sprite flip |
| `y > 0` (upward) | `walkUpClip` |
| `y < 0` (downward) | `walkDownClip` |
| Stationary | Freeze Animator (`speed = 0`) |

All child `SpriteRenderer` components are flipped simultaneously via `flipX` when facing right.
