# 📋 Referensi Sistem

Ringkasan skrip dan komponen utama. Nama field, metode, dan properti tetap mengikuti kode sumber.

## 🧍 Pemain

### [`Player.cs`](../../Assets/Scripts/Player/Player.cs)
Mengatur input gerakan, animasi arah, dan arah hadap.

| Properti | Deskripsi |
|---|---|
| `IsFacingLeft` | Apakah pemain menghadap kiri |
| `IsFacingUp` | Apakah pemain menghadap atas |
| `IsFacingDown` | Apakah pemain menghadap bawah |

Skrip membaca gerakan dari Input System dan memperbarui animasi samping, atas, atau bawah. Saat menghadap kiri, sprite dan posisi objek yang dapat dipungut dapat dibalik/disesuaikan.

### [`PlayerScore.cs`](../../Assets/Scripts/Player/PlayerScore.cs)
Menyimpan skor pemain sebagai nilai progres sekaligus mata uang toko.

| Anggota | Deskripsi |
|---|---|
| `CurrentScore` | Skor saat ini, hanya-baca |
| `OnScoreChanged` | Event saat skor berubah |
| `AddScore(int)` | Menambahkan skor |
| `SpendScore(int)` | Memotong skor jika saldo mencukupi; mengembalikan `bool` |

## ❤️ Kesehatan

### [`Health.cs`](../../Assets/Scripts/Health/Health.cs)
Komponen HP bersama untuk pemain dan musuh. Menyediakan `CurrentHealth`, `MaxHealth`, `IsDead`, event `OnHealthChanged` dan `OnPlayerDied`, serta metode `TakeDamage(int)` dan `AddBonusHealth(int)`. Jika aktif, kerusakan membuat sprite berkedip merah. Pemain memicu event kematian; musuh memberi skor melalui `EnemyScoreValue` saat mati.

## 🧟 Musuh

### [`EnemySpawner.cs`](../../Assets/Scripts/Enemy/EnemySpawner.cs)
Mengatur siklus hitung mundur, pemunculan, dan menunggu musuh dikalahkan. Nilai Inspector meliputi `baseEnemyCount`, `waveIncrementMin/Max`, `timeBetweenWaves`, `enemyPrefabs`, `maxEnemiesAlive`, `spawnOnEdge`, dan `areaSize`.

| Properti | Deskripsi |
|---|---|
| `CurrentWaveNumber` | Nomor gelombang, dimulai dari 1 |
| `WaveCountdown` | Sisa waktu hitung mundur |
| `EnemiesRemaining` | Musuh yang masih perlu muncul atau masih hidup |
| `CurrentState` | Status spawner saat ini |

Prefab musuh diacak dengan Fisher-Yates. Musuh pada gelombang setelah pertama menerima tambahan HP acak yang dikalikan indeks gelombang.

### Skrip musuh lainnya

- [`EnemyFacing.cs`](../../Assets/Scripts/Enemy/EnemyFacing.cs): memilih animasi arah dari kecepatan `Rigidbody2D` dan menjeda animator saat diam.
- [`EnemyScoreValue.cs`](../../Assets/Scripts/Enemy/EnemyScoreValue.cs): menentukan skor yang diberikan saat musuh mati.
- [`EnemyVision.cs`](../../Assets/Scripts/Enemy/EnemyVision.cs): mengatur pemeriksaan penglihatan musuh.
- `SeeAction`, `ChaseAction`, `AttackAction`, dan `IdleAction` di `Assets/Scripts/Enemy/BehaviorTree/`: node aksi Behavior Tree untuk mendeteksi, mengejar, menyerang, dan diam.

## 🔫 Senjata

- [`WeaponShoot.cs`](../../Assets/Scripts/Weapon/WeaponShoot.cs): menembakkan `pellets` peluru melalui titik proyektil untuk arah hadap saat ini; beberapa peluru memakai `spreadAngle`.
- [`WeaponMelee.cs`](../../Assets/Scripts/Weapon/WeaponMelee.cs): mengatur damage, jangkauan, cooldown, dan animasi dorongan serangan jarak dekat.
- [`WeaponSwitcher.cs`](../../Assets/Scripts/Weapon/WeaponSwitcher.cs): `AddWeapon()` menambahkan senjata, `HasWeapon()` memeriksa kepemilikan, dan `SelectWeapon(int)` memilih slot.
- [`WeaponFacing.cs`](../../Assets/Scripts/Weapon/WeaponFacing.cs): mengubah sprite dan sorting order mengikuti arah pemain.
- [`Bullet.cs`](../../Assets/Scripts/Weapon/Bullet.cs): menggerakkan peluru lurus, memberi damage saat mengenai musuh, dan menghancurkan peluru setelah benturan atau batas waktu.

## 🏪 Toko dan NPC

- [`GunManInteract.cs`](../../Assets/Scripts/GunMan/GunManInteract.cs): membuka atau menutup toko saat pemain berada di area pemicu dan menekan tombol interaksi.
- [`ShopUI.cs`](../../Assets/Scripts/UI/ShopUI.cs): menampilkan item toko, mencegah pembelian duplikat, memotong skor, dan meminta `WeaponSwitcher` memasang senjata.

## 🖥️ UI

| Skrip | Fungsi |
|---|---|
| [`HealthUI.cs`](../../Assets/Scripts/UI/HealthUI.cs) | Menampilkan HP pemain |
| [`ScoreUI.cs`](../../Assets/Scripts/UI/ScoreUI.cs) | Menampilkan skor saat ini |
| [`WaveUI.cs`](../../Assets/Scripts/UI/WaveUI.cs) | Menampilkan gelombang, hitung mundur, dan jumlah musuh |
| [`GameOverUI.cs`](../../Assets/Scripts/UI/GameOverUI.cs) | Menampilkan panel Game Over dan menangani tombolnya |
| [`MenuUI.cs`](../../Assets/Scripts/UI/MenuUI.cs) | Menangani tombol menu utama |

## 📷 Kamera dan Lingkungan

- [`CameraFollow.cs`](../../Assets/Scripts/Camera/CameraFollow.cs): mengikuti pemain.
- [`MainMenuCamera.cs`](../../Assets/Scripts/Camera/MainMenuCamera.cs): menggerakkan kamera pada menu utama.
- [`InteriorTransition.cs`](../../Assets/Scripts/Environment/InteriorTransition.cs): mengatur tampilan penutup interior saat pemain masuk/keluar.
