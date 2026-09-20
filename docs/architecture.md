# 🏗️ Architecture

This document describes the high-level architecture, folder structure, and key design patterns used in the project.

---

## 📁 Scripts Folder Structure

```
Assets/Scripts/
├── Camera/
│   ├── CameraFollow.cs       # Smooth camera that follows the player
│   └── MainMenuCamera.cs     # Camera behaviour on the main menu
├── Enemy/
│   ├── BehaviorTree/
│   │   ├── AttackAction.cs   # BT leaf node: melee attack player
│   │   └── ChaseAction.cs    # BT leaf node: move toward player
│   ├── EnemyFacing.cs        # Directional animation & sprite flip
│   ├── EnemyScoreValue.cs    # Score awarded on death
│   └── EnemySpawner.cs       # Wave management & enemy instantiation
├── GunMan/
│   └── GunManInteract.cs     # NPC shop trigger & interaction logic
├── Health/
│   └── Health.cs             # Shared HP component (player & enemies)
├── Player/
│   ├── Player.cs             # Movement, animation, facing direction
│   └── PlayerScore.cs        # Score tracking & currency spending
├── UI/
│   ├── GameOverUI.cs         # Game over panel, restart/quit buttons
│   ├── HealthUI.cs           # Player HP display (TextMeshPro)
│   ├── MenuUI.cs             # Main menu buttons
│   ├── ScoreUI.cs            # Live score display
│   ├── ShopUI.cs             # Weapon shop panel and purchase logic
│   └── WaveUI.cs             # Current wave number & countdown
└── Weapon/
    ├── Bullet.cs             # Bullet movement, collision, and damage
    ├── WeaponFacing.cs       # Weapon sprite swap based on direction
    ├── WeaponMelee.cs        # Melee attack, thrust animation
    ├── WeaponShoot.cs        # Ranged fire, spread, bullet spawning
    └── WeaponSwitcher.cs     # Owns & switches between weapons (1–9)
```

---

## 🔗 System Relationships

```
┌──────────────────────────────────────────────────────────┐
│                        Player                            │
│  Player.cs ──► PlayerScore.cs ──► ShopUI (spend score)  │
│      │                                                   │
│      └──► Health.cs ──► GameOverUI (on death)            │
│      └──► WeaponSwitcher.cs                              │
│               ├──► WeaponShoot.cs ──► Bullet.cs          │
│               └──► WeaponMelee.cs                        │
└──────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────┐
│                      Enemy                               │
│  EnemySpawner.cs ──► Instantiate enemy prefab            │
│      │                                                   │
│      └──► Health.cs (with bonus HP per wave)             │
│               └── on death ──► PlayerScore.AddScore()    │
│                                                          │
│  BehaviorTree                                            │
│      ├── ChaseAction.cs ──► Rigidbody2D.velocity         │
│      └── AttackAction.cs ──► Health.TakeDamage()         │
│                                                          │
│  EnemyFacing.cs ──► reads Rigidbody2D.velocity           │
│                  ──► Animator + SpriteRenderer.flipX     │
└──────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────┐
│                     NPC: GunMan                          │
│  GunManInteract.cs ──► triggers ShopUI.OpenShop()        │
│  ShopUI.cs ──► PlayerScore.SpendScore()                  │
│           ──► WeaponSwitcher.AddWeapon()                 │
└──────────────────────────────────────────────────────────┘
```

---

## 🧩 Design Patterns

| Pattern | Where Used |
|---|---|
| **Singleton** | `EnemySpawner.Instance`, `ShopUI.Instance` — global access points |
| **Event-Driven** | `Health.OnPlayerDied`, `Health.OnHealthChanged`, `PlayerScore.OnScoreChanged` — loose coupling between systems |
| **Unity Behavior Trees** | Enemy AI composed of reusable `ChaseAction` and `AttackAction` leaf nodes |
| **Object Pooling (manual)** | Enemies tracked in `List<GameObject>` by `EnemySpawner`, null-checked each frame |
| **Component-based** | Health, Score, and Facing are separate components reused across player and enemies |
| **Fisher-Yates Shuffle** | Used in `EnemySpawner.RefillShuffledPrefabs()` to randomly order enemy types per wave |

---

## 🌐 Event Flow on Player Death

```
Health.TakeDamage()
  └─► currentHealth <= 0
        └─► isDead = true
              └─► Die()
                    ├─► Health.OnPlayerDied (static event)  ──► GameOverUI.HandlePlayerDeath()
                    └─► Destroy(gameObject)
```

---

## 📡 Score as Currency

Score serves a dual role as both a leaderboard metric and an in-game currency:

- **Earned** via `PlayerScore.AddScore()` when enemies are killed (`EnemyScoreValue.ScoreValue`)
- **Spent** via `PlayerScore.SpendScore()` when buying weapons in `ShopUI`
- **Displayed** live in `ScoreUI`, which subscribes to `PlayerScore.OnScoreChanged`
