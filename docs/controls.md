# 🎮 Controls

Default input bindings for **Survival 2D Zombie** using the Unity New Input System.

---

## ⌨️ Keyboard & Mouse

| Action | Input | Notes |
|---|---|---|
| **Move Up** | `W` or `↑` | 8-directional movement |
| **Move Down** | `S` or `↓` | |
| **Move Left** | `A` or `←` | Sprite flips left |
| **Move Right** | `D` or `→` | |
| **Shoot / Attack** | `Left Mouse Button` | Fires bullet toward cursor (ranged) or attacks in facing direction (melee) |
| **Interact** | `E` | Opens / closes GunMan shop when nearby |
| **Switch to Weapon 1** | `1` | Equip first owned weapon |
| **Switch to Weapon 2** | `2` | Equip second owned weapon |
| **Switch to Weapon 3–9** | `3` – `9` | Equip additional owned weapons |

---

## 🕹️ Notes

- **Movement** is handled by Unity's Input System `InputAction` (Vector2 composite).
- **Weapon switching** uses direct `Keyboard.current[Key.Digit1..9]` polling in `WeaponSwitcher.cs`.
- **Interact** is an `InputActionReference` wired in the Inspector on `GunManInteract.cs`.
- **Attack** input is referenced per-weapon via `InputActionReference` on `WeaponShoot.cs` and `WeaponMelee.cs`.
- Aiming for ranged weapons is **mouse-cursor based** — bullets travel from the fire point toward the cursor position in world space.

---

## 🛍️ Shop Interaction Flow

1. Walk into the **GunMan NPC** trigger zone.
2. Press `E` to open the shop panel.
3. Click a weapon button to purchase (costs score).
4. Press `E` again (or walk away) to close the shop.
