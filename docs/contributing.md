# 🤝 Contributing

Guidelines for adding new scripts, systems, or content to **Survival 2D Zombie**.

---

## 📁 Where to Put Things

| What | Folder |
|---|---|
| Player logic | `Assets/Scripts/Player/` |
| Enemy logic / AI | `Assets/Scripts/Enemy/` |
| Behavior Tree nodes | `Assets/Scripts/Enemy/BehaviorTree/` |
| Weapon scripts | `Assets/Scripts/Weapon/` |
| UI scripts | `Assets/Scripts/UI/` |
| NPC scripts | `Assets/Scripts/GunMan/` or a new NPC folder |
| Shared utilities | `Assets/Scripts/` (root) or a `Utility/` subfolder |
| New scenes | `Assets/Scenes/` |
| New prefabs | `Assets/Prefabs/` (create if needed) |
| New sprites | `Assets/Sprites/` or under the tileset folder |

---

## ➕ Adding a New Enemy Type

1. Create a new enemy prefab in `Assets/Prefabs/Enemies/`.
2. Add required components:
   - `Rigidbody2D` (2D, typically `Kinematic` or `Dynamic`)
   - `Collider2D` (for physics and bullet hit detection) — tag it `"Enemy"`
   - `Health` — set `maxHealth` in the Inspector
   - `EnemyFacing` — assign `walkSideClip`, `walkUpClip`, `walkDownClip`
   - `EnemyScoreValue` — set the score reward
   - A Unity Behavior **Behavior Graph** with `ChaseAction` and `AttackAction` nodes
3. Add the prefab to `EnemySpawner.enemyPrefabs` in the scene.

---

## ➕ Adding a New Weapon

### Ranged Weapon
1. Create a weapon prefab (sprite + `WeaponShoot` + `WeaponFacing`).
2. Configure `bulletPrefab`, `firePoint`, `fireRate`, `bulletsPerShot`, `spreadAngle`.
3. Assign the shoot `InputActionReference`.
4. Add to the GunMan's `weaponsForSale` list in the Inspector with a name and price.

### Melee Weapon
1. Create a weapon prefab (sprite + `WeaponMelee` + `WeaponFacing`).
2. Configure `damage`, `attackRange`, `attackCooldown`, `thrustDistance`.
3. Assign the attack `InputActionReference`.
4. Optionally add to the shop or pre-equip in the player prefab.

---

## ➕ Adding a New UI Screen

1. Create a Canvas/Panel in the scene.
2. Create a new script in `Assets/Scripts/UI/`.
3. Subscribe to relevant events (e.g., `Health.OnPlayerDied`, `PlayerScore.OnScoreChanged`).
4. Always **unsubscribe in `OnDestroy`** to prevent memory leaks:
   ```csharp
   private void OnEnable()  { SomeSystem.OnEvent += Handler; }
   private void OnDisable() { SomeSystem.OnEvent -= Handler; }
   ```

---

## ✅ Code Guidelines

### Naming
- Scripts: `PascalCase` (e.g., `EnemySpawner.cs`)
- Public properties: `PascalCase` (e.g., `CurrentHealth`)
- Private fields: `camelCase` with `_` prefix optional (e.g., `currentHealth`)
- Constants: `PascalCase` or `UPPER_SNAKE_CASE` for readonly statics

### Components
- Prefer `[SerializeField] private` over `public` for Inspector-exposed fields.
- Use `??=` for auto-assignment: `spriteRenderer ??= GetComponent<SpriteRenderer>();`
- Use `[RequireComponent(typeof(...))]` when a script always needs another component.

### Events
- Use C# `event Action` or `event Action<T>` for inter-system communication.
- Use `UnityEvent` only when Inspector hookup by non-programmers is needed.
- Always null-check: `OnEvent?.Invoke(args);`

### Singletons
- Keep singletons minimal — only use for true global managers (`EnemySpawner`, `ShopUI`).
- Pattern:
  ```csharp
  public static MyManager Instance { get; private set; }
  private void Awake() {
      if (Instance == null) Instance = this;
      else Destroy(gameObject);
  }
  private void OnDestroy() {
      if (Instance == this) Instance = null;
  }
  ```

### Behavior Tree Nodes
- Inherit from `Unity.Behavior.Action`.
- Use `BlackboardVariable<T>` for all data passed in/out.
- Return `Status.Running`, `Status.Success`, or `Status.Failure` appropriately.
- Clean up in `OnEnd()` (e.g., zero out velocity).

---

## 🐛 Debug Tips

- `EnemySpawner` logs wave transitions to the Console.
- `Health.TakeDamage()` logs damage and current HP.
- `PlayerScore.AddScore()` logs score changes.
- `WeaponSwitcher.AddWeapon()` logs duplicate purchase warnings.
- Use the **Scene Gizmo** on `WeaponMelee` (select it in the Editor) to visualize attack range.
- Use the **Scene Gizmo** on `EnemySpawner` to visualize the spawn zone rectangle.
