# 📋 Systems Reference

Detailed reference for every script in the project.

---

## 🧍 Player

### [`Player.cs`](../Assets/Scripts/Player/Player.cs)
Core player controller.

| Property / Method | Type | Description |
|---|---|---|
| `IsFacingLeft` | `bool` (get) | True when player sprite is flipped left |
| `IsFacingUp` | `bool` (get) | True when moving upward |
| `IsFacingDown` | `bool` (get) | True when moving downward |

**Inspector Fields**
| Field | Default | Description |
|---|---|---|
| `moveSpeed` | — | Player movement speed |
| `animator` | auto | Animator component reference |
| `spriteRenderer` | auto | SpriteRenderer for flip |
| `pickableItems` | — | Child transform repositioned when facing left |

**Behavior:** Reads Unity Input System movement input, plays directional walk animations (`walkSide`, `walkUp`, `walkDown`), and mirrors the sprite + weapon child position when facing left. Uses `LateUpdate` for sprite flip to avoid Animator override conflicts.

---

### [`PlayerScore.cs`](../Assets/Scripts/Player/PlayerScore.cs)
Tracks the player's score, which acts as both a progress metric and shop currency.

| Member | Description |
|---|---|
| `CurrentScore` | Read-only current score |
| `OnScoreChanged` | `event Action<int>` fired on any score change |
| `AddScore(int)` | Adds points (called by `Health.cs` on enemy death) |
| `SpendScore(int)` | Returns `true` and deducts if affordable; `false` otherwise |

---

## ❤️ Health

### [`Health.cs`](../Assets/Scripts/Health/Health.cs)
Shared HP component used by **both the player and enemies**.

| Member | Description |
|---|---|
| `CurrentHealth` | Read-only current HP |
| `MaxHealth` | Read-only max HP (can be increased by `AddBonusHealth`) |
| `IsDead` | Read-only dead flag |
| `OnHealthChanged` | `event Action<int>` — fires with new current HP |
| `OnPlayerDied` | `static event Action` — fires when player dies |
| `OnDeath` | `UnityEvent` — assignable in Inspector |
| `TakeDamage(int)` | Reduces HP, triggers flash, fires events, calls `Die()` if HP ≤ 0 |
| `AddBonusHealth(int)` | Increases both max and current HP (used for wave scaling on enemies) |

**Hit Flash:** When `flashOnHit` is enabled, briefly turns the sprite renderer red for `flashDuration` seconds.

**Death behavior:**
- **Player** → fires `OnPlayerDied`, destroys GameObject
- **Enemy** → awards `EnemyScoreValue.ScoreValue` to `PlayerScore`, destroys GameObject

---

## 🧟 Enemy

### [`EnemySpawner.cs`](../Assets/Scripts/Enemy/EnemySpawner.cs)
Manages wave flow and enemy instantiation. Singleton (`EnemySpawner.Instance`).

**State Machine**
```
CountingDown ──► Spawning ──► WaitingForDeath ──► CountingDown (next wave)
```

| Inspector Field | Description |
|---|---|
| `enemyPrefabs` | Array of enemy prefab variants |
| `baseEnemyCount` | Enemies in wave 1 |
| `waveIncrementMin/Max` | Random range added to enemy count each wave |
| `timeBetweenWaves` | Countdown seconds between waves |
| `spawnInterval` | Seconds between individual enemy spawns |
| `maxEnemiesAlive` | Cap on simultaneous alive enemies |
| `spawnOnEdge` | If true, spawns on map edges; otherwise spawns randomly inside area |
| `areaSize` | Width/height of spawn zone (Gizmo drawn in editor) |

**Key Properties**
| Property | Description |
|---|---|
| `CurrentWaveNumber` | 1-indexed current wave |
| `WaveCountdown` | Seconds remaining until next wave |
| `EnemiesRemaining` | Combined alive + yet-to-spawn enemies |

**Enemy HP Scaling:** Each wave beyond wave 1, enemies get `Random.Range(10, 21) * waveIndex` bonus HP via `Health.AddBonusHealth()`.

**Prefab Shuffling:** Uses Fisher-Yates shuffle to randomize the order of enemy prefab types across waves.

---

### [`EnemyFacing.cs`](../Assets/Scripts/Enemy/EnemyFacing.cs)
Reads `Rigidbody2D.linearVelocity` (set by Behavior Tree) to play directional animations and flip sprites.

| Inspector Field | Description |
|---|---|
| `walkSideClip` | AnimationClip for horizontal movement |
| `walkUpClip` | AnimationClip for upward movement |
| `walkDownClip` | AnimationClip for downward movement |

Freezes the Animator (`animator.speed = 0`) when the enemy is stationary.

---

### [`EnemyScoreValue.cs`](../Assets/Scripts/Enemy/EnemyScoreValue.cs)
Simple data component. Stores the score value awarded when this enemy is killed.

| Field | Default | Description |
|---|---|---|
| `scoreValue` | 10 | Points awarded to `PlayerScore` on death |

---

### [`AttackAction.cs`](../Assets/Scripts/Enemy/BehaviorTree/AttackAction.cs) (Behavior Tree Node)
Custom BT leaf node. Deals melee damage to the player when in range.

| Blackboard Variable | Description |
|---|---|
| `Self` | The enemy GameObject |
| `Player` | The player GameObject |
| `Damage` | Integer damage per attack |
| `AttackCooldown` | Seconds between attacks |

---

### [`ChaseAction.cs`](../Assets/Scripts/Enemy/BehaviorTree/ChaseAction.cs) (Behavior Tree Node)
Custom BT leaf node. Moves the enemy toward the player using `Rigidbody2D.linearVelocity`.

| Blackboard Variable | Description |
|---|---|
| `Self` | The enemy GameObject |
| `Player` | The player (dynamically found via tag if null or invalid) |
| `Speed` | Movement speed |
| `StopDistance` | Stop chasing below this distance (returns `Success`) |

Returns `Running` while chasing, `Success` when close enough, `Failure` if refs are missing.

---

## 🔫 Weapons

### [`WeaponShoot.cs`](../Assets/Scripts/Weapon/WeaponShoot.cs)
Ranged weapon — fires bullets in the player's facing direction.

| Inspector Field | Description |
|---|---|
| `bulletPrefab` | Bullet GameObject to instantiate |
| `pointRight/Left/Up/Down` | Projectile origin for each facing direction |
| `pellets` | Bullets fired per shot |
| `spreadAngle` | Max spread in degrees for multi-bullet shots |
| `shootActionReference` | Input Action for firing |

---

### [`WeaponMelee.cs`](../Assets/Scripts/Weapon/WeaponMelee.cs)
Melee weapon — attacks in the player's facing direction.

| Inspector Field | Description |
|---|---|
| `damage` | Damage dealt per hit |
| `attackRange` | Radius of the hit circle |
| `attackCooldown` | Minimum seconds between attacks |
| `thrustDistance` | How far the weapon moves during the attack animation |
| `thrustDuration` | Total duration of the thrust-and-return animation |
| `attackActionReference` | Input Action for attacking |

Uses `Physics2D.OverlapCircleAll` at `playerPosition + facingDirection * attackRange` to detect enemies.

---

### [`WeaponSwitcher.cs`](../Assets/Scripts/Weapon/WeaponSwitcher.cs)
Manages the player's weapon inventory. Switches active weapon via number keys.

| Method | Description |
|---|---|
| `AddWeapon(GameObject prefab)` | Instantiates and equips a new weapon; blocks duplicates |
| `HasWeapon(GameObject prefab)` | Returns `true` if the player already owns this weapon |
| `SelectWeapon(int index)` | Activates weapon at index, deactivates all others |

Keys `1–9` map to weapon slots. New weapons are auto-equipped on purchase.

---

### [`WeaponFacing.cs`](../Assets/Scripts/Weapon/WeaponFacing.cs)
Swaps the weapon sprite and sorting order based on the player's facing direction.

| Sprite Slot | Used When |
|---|---|
| `sideSprite` | Facing left or right |
| `upSprite` | Facing up (sorting order = 1, draws behind player) |
| `downSprite` | Facing down (sorting order = 3, draws in front) |

---

### [`Bullet.cs`](../Assets/Scripts/Weapon/Bullet.cs)
Moves in a straight line and deals damage on contact with enemies.

| Field | Default | Description |
|---|---|---|
| `speed` | 10 | Units per second |
| `lifeTime` | 2 | Seconds before auto-destroy |
| `damage` | 25 | HP removed from enemy `Health` |

Ignores collisions with: `Player`, `Bullet`, `Border` tags. Destroys self on any other hit.

---

## 🏪 Shop & NPC

### [`GunManInteract.cs`](../Assets/Scripts/GunMan/GunManInteract.cs)
NPC interaction trigger. Opens/closes the shop when the player presses the interact key within the collider zone.

| Field | Description |
|---|---|
| `weaponsForSale` | List of `GunManShopItem` structs (name, price, prefab) |
| `interactAction` | Input Action Reference for the interact key |

`GunManShopItem` struct:
```csharp
public struct GunManShopItem {
    public string itemName;
    public int price;
    public GameObject weaponPrefab;
}
```

---

### [`ShopUI.cs`](../Assets/Scripts/UI/ShopUI.cs)
Controls the weapon shop UI panel. Singleton (`ShopUI.Instance`).

| Method | Description |
|---|---|
| `OpenShop(items, player)` | Configures and shows the shop panel |
| `CloseShop()` | Hides the shop panel |
| `BuyWeapon(item, button)` | Calls `SpendScore`, then `AddWeapon`; flashes red if insufficient score |
| `RefreshButtonStates()` | Re-evaluates owned weapons after a purchase to gray out bought items |

Already-owned weapons display **"Already Owned"** and are non-interactable.

---

## 🖥️ UI

### [`HealthUI.cs`](../Assets/Scripts/UI/HealthUI.cs)
Displays player HP as a number. Subscribes to `Health.OnHealthChanged`.

### [`ScoreUI.cs`](../Assets/Scripts/UI/ScoreUI.cs)
Displays current score. Subscribes to `PlayerScore.OnScoreChanged`.

### [`WaveUI.cs`](../Assets/Scripts/UI/WaveUI.cs)
Displays current wave number and countdown timer. Reads from `EnemySpawner.Instance`.

### [`GameOverUI.cs`](../Assets/Scripts/UI/GameOverUI.cs)
Listens for `Health.OnPlayerDied`. Shows game over panel, stops BGM, plays SFX, and pauses the game.

| Button | Action |
|---|---|
| Restart | Reloads active scene; resets `Time.timeScale = 1` |
| Quit | Returns to `MainMenu` scene (or `Application.Quit` if disabled) |

### [`MenuUI.cs`](../Assets/Scripts/UI/MenuUI.cs)
Handles main menu button interactions (e.g., Play, Quit).

---

## 📷 Camera

### [`CameraFollow.cs`](../Assets/Scripts/Camera/CameraFollow.cs)
Smoothly follows the player using `Vector3.Lerp` or `SmoothDamp`.

### [`MainMenuCamera.cs`](../Assets/Scripts/Camera/MainMenuCamera.cs)
Camera behavior specific to the main menu scene.
