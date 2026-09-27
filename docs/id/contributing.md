# 🤝 Kontribusi

Panduan menambahkan skrip, sistem, atau konten ke **Survival 2D Zombie**.

## Lokasi File

| Jenis | Folder |
|---|---|
| Logika pemain | `Assets/Scripts/Player/` |
| Logika musuh dan AI | `Assets/Scripts/Enemy/` |
| Node Behavior Tree | `Assets/Scripts/Enemy/BehaviorTree/` |
| Skrip senjata | `Assets/Scripts/Weapon/` |
| Skrip UI | `Assets/Scripts/UI/` |
| Skrip NPC | `Assets/Scripts/GunMan/` atau folder NPC baru |
| Scene | `Assets/Scenes/` |
| Prefab | `Assets/Prefabs/` |
| Sprite | `Assets/Sprites/` atau folder tileset terkait |

## Menambahkan Tipe Musuh

1. Buat prefab musuh di `Assets/Prefabs/Enemy/`.
2. Tambahkan komponen yang diperlukan: `Rigidbody2D`, `Collider2D` dengan tag `Enemy`, `Health`, `EnemyFacing`, dan `EnemyScoreValue`.
3. Pasang Behavior Graph Unity Behavior dan konfigurasi blackboard untuk node AI yang digunakan.
4. Tambahkan prefab ke `enemyPrefabs` pada `EnemySpawner` di scene.

## Menambahkan Senjata

**Jarak jauh:** Buat prefab dengan `WeaponShoot` dan `WeaponFacing`; atur prefab peluru, titik proyektil untuk empat arah, jumlah peluru, sebaran, serta `InputActionReference`. Masukkan item ke `weaponsForSale` milik GunMan melalui Inspector.

**Jarak dekat:** Buat prefab dengan `WeaponMelee` dan `WeaponFacing`; atur damage, jangkauan, jeda serangan, jarak dorongan, dan `InputActionReference`. Senjata dapat ditambahkan ke toko atau dipasang pada prefab pemain.

## Menambahkan Layar UI

1. Buat Canvas/Panel pada scene dan skrip baru di `Assets/Scripts/UI/`.
2. Berlangganan ke event sistem yang diperlukan, misalnya `Health.OnPlayerDied` atau `PlayerScore.OnScoreChanged`.
3. Lepas langganan di `OnDisable` agar event tidak memanggil objek yang sudah tidak aktif.

```csharp
private void OnEnable()  { SomeSystem.OnEvent += Handler; }
private void OnDisable() { SomeSystem.OnEvent -= Handler; }
```

## Pedoman Kode

- Gunakan `PascalCase` untuk nama skrip dan properti publik; gunakan `camelCase` untuk field privat.
- Utamakan `[SerializeField] private` untuk field yang diatur lewat Inspector.
- Gunakan `[RequireComponent]` jika skrip selalu membutuhkan komponen lain.
- Gunakan event C# untuk komunikasi antarsistem dan panggil dengan `OnEvent?.Invoke(args)`.
- Batasi singleton pada pengelola yang benar-benar perlu diakses secara global.
- Node Behavior Tree mewarisi `Unity.Behavior.Action`, memakai `BlackboardVariable<T>`, dan membersihkan status di `OnEnd()` bila diperlukan.

## Debug

- Periksa log Console untuk perubahan gelombang, damage, perubahan skor, dan pembelian ganda.
- Pilih `WeaponMelee` atau `EnemySpawner` di Editor untuk melihat Gizmo jangkauan atau area kemunculan.
