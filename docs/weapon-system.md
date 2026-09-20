# 🔫 Weapon System

Documentation for ranged weapons, melee weapons, weapon switching, and the shop.

---

## 🗂️ Weapon Inventory

The `WeaponSwitcher` component manages all weapons owned by the player. Weapons are child GameObjects of the `Weapons` parent object on the player prefab.

```
Player
  └── Weapons (WeaponSwitcher)
        ├── Knife      (WeaponMelee)     ← default starter weapon
        ├── Pistol     (WeaponShoot)     ← may be pre-equipped
        └── Shotgun    (WeaponShoot)     ← purchased from shop
```

- Only **one weapon** is active (`SetActive(true)`) at a time.
- Switching activates the selected weapon and deactivates all others.

---

## 🔄 Switching Weapons

Press number keys **1–9** to equip a weapon by slot index.

```
Key 1 → Slot 0 (first weapon)
Key 2 → Slot 1 (second weapon)
...
Key 9 → Slot 8 (ninth weapon)
```

Newly purchased weapons are **auto-equipped** when added via `AddWeapon()`.

---

## 🔫 Ranged Weapons (`WeaponShoot`)

### How Shooting Works

1. Player presses the attack input (left mouse button).
2. Fire rate cooldown is checked (`Time.time >= lastFireTime + 1/fireRate`).
3. For each bullet in `bulletsPerShot`:
   - If `bulletsPerShot > 1`, each bullet gets a random angle offset within `±spreadAngle/2`.
   - A `Bullet` prefab is instantiated at `firePoint`.
   - `Bullet.Setup(direction)` is called with the aimed direction.

### Aiming
- Direction is calculated from `firePoint.position` toward the **mouse cursor position in world space** using `Camera.main.ScreenToWorldPoint`.

### Bullet Behavior (`Bullet.cs`)
- Moves in a straight line at `speed` units/second.
- Destroyed after `lifeTime` seconds.
- On collision with an `Enemy` tag: applies `damage` to `Health`, then destroys itself.
- Ignores: `Player`, `Bullet`, `Border` tagged objects.
- Destroys itself on any other hit (walls, NPC, etc.).

### Inspector Configuration

| Field | Description |
|---|---|
| `bulletPrefab` | The bullet to spawn |
| `firePoint` | Spawn transform for bullets |
| `fireRate` | Shots per second (e.g. `2` = fire every 0.5s) |
| `bulletsPerShot` | `1` = pistol, `3+` = shotgun spread |
| `spreadAngle` | Max total spread in degrees |
| `attackActionReference` | Linked Input Action |

---

## 🔪 Melee Weapons (`WeaponMelee`)

### How Attacking Works

1. Player presses the attack input.
2. Attack cooldown is checked.
3. `AttackDirection` is determined from `Player.IsFacingUp/Down/Left` (defaults to right).
4. A **visual thrust** animation plays (weapon moves forward then snaps back).
5. `Physics2D.OverlapCircleAll` detects enemies within range in the attack direction.
6. `Health.TakeDamage(damage)` is called on each hit enemy.

### Hit Detection Geometry

```
         Player
           │
   ←  attackRange  →
           ●  ← hitCenter = playerPos + direction * attackRange
         (radius = attackRange * 0.75)
```

### Inspector Configuration

| Field | Description |
|---|---|
| `damage` | HP removed per hit |
| `attackRange` | Distance + radius of hit zone |
| `attackCooldown` | Min seconds between attacks |
| `thrustDistance` | How far the weapon lunges (in units) |
| `thrustDuration` | Total time for forward + return animation |
| `attackActionReference` | Linked Input Action |

> Use the **Scene Gizmo** (select weapon in Editor) to visualize the attack hit zone as a red wire sphere.

---

## 🎨 Weapon Visuals (`WeaponFacing`)

Weapons swap their sprite and sorting order based on the player's facing direction, creating a pseudo-3D look:

| Direction | Sprite | Sort Order |
|---|---|---|
| Left / Right | `sideSprite` | 3 (in front of player) |
| Up | `upSprite` | 1 (behind player) |
| Down | `downSprite` | 3 (in front of player) |

The sprite also mirrors horizontally (`flipX`) when the player faces left.

---

## 🏪 Shop System

### GunMan NPC Flow

```
Player enters trigger zone
  └── isPlayerNearby = true

Player presses Interact (E)
  └── ShopUI.OpenShop(weaponsForSale, playerTransform)

Player clicks a weapon button
  └── BuyWeapon(item, button)
        ├── PlayerScore.SpendScore(item.price) → true?
        │       └── WeaponSwitcher.AddWeapon(item.weaponPrefab)
        │             └── Instantiate + auto-equip
        └── false → Flash button red briefly

Player walks out of trigger zone
  └── ShopUI.CloseShop()
```

### Duplicate Purchase Protection

`WeaponSwitcher` tracks owned prefabs in a `HashSet<GameObject>`. If a prefab is already owned:
- `HasWeapon()` returns `true` → button is disabled in UI ("Already Owned")
- `AddWeapon()` logs a warning and returns early

### Shop Item Definition

Configured per NPC in the Inspector on `GunManInteract`:

```csharp
[Serializable]
public struct GunManShopItem {
    public string itemName;    // Display name in shop UI
    public int price;          // Score cost
    public GameObject weaponPrefab; // Weapon to instantiate
}
```
