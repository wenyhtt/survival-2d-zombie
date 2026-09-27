# 🧟 Sistem Musuh

Ringkasan pengelolaan gelombang, AI, dan peningkatan kesulitan.

## 🌊 Sistem Gelombang

Komponen `EnemySpawner` mengatur siklus tiga status:

`CountingDown` → `Spawning` → `WaitingForDeath` → `CountingDown` (gelombang berikutnya)

| Status | Perilaku |
|---|---|
| `CountingDown` | Mengurangi `waveCountdown`; `WaveUI` menampilkan hitung mundur. |
| `Spawning` | Membuat musuh hingga jumlah gelombang tercapai atau batas `maxEnemiesAlive` tercapai. |
| `WaitingForDeath` | Menunggu semua musuh yang muncul dikalahkan. |

## 📈 Peningkatan Kesulitan

Setelah gelombang selesai, `currentWaveIndex` bertambah dan jumlah musuh berikutnya ditambah angka acak dari `waveIncrementMin` sampai `waveIncrementMax`. Musuh pada gelombang setelah gelombang pertama mendapat HP tambahan: `Random.Range(10, 21) × indeks gelombang`.

Nilai bawaan di skrip adalah 25 musuh pada gelombang pertama dan penambahan 3–8 musuh tiap gelombang.

## 📍 Posisi Kemunculan

Dengan `spawnOnEdge = true`, musuh muncul pada salah satu dari empat tepi area. Jika `false`, posisi dipilih acak di dalam persegi panjang `areaSize` yang berpusat pada transform spawner. Area ditampilkan sebagai Gizmo di Unity Editor.

## 🎲 Pemilihan Tipe Musuh

Prefab pada `enemyPrefabs` diacak menggunakan Fisher-Yates. Setiap gelombang memilih satu tipe prefab; daftar diacak ulang setelah seluruh tipe digunakan.

## 🤖 AI: Behavior Tree

Musuh menggunakan Unity Behavior dan node aksi khusus:

- `SeeAction` memeriksa pemain menggunakan variabel blackboard dan tag `Player`.
- `ChaseAction` menggerakkan musuh menuju pemain menggunakan `Rigidbody2D.linearVelocity`.
- `AttackAction` memberikan damage ketika pemain berada dalam jangkauan dan mematuhi jeda serangan.
- `IdleAction` menyediakan aksi diam.

Susunan graph yang dipakai ditentukan oleh Behavior Graph pada prefab musuh. Pastikan referensi blackboard dan jarak serang cocok dengan komponennya.

## 🎨 Animasi Musuh

`EnemyFacing.cs` membaca `Rigidbody2D.linearVelocity` untuk memilih klip gerak samping, atas, atau bawah. Untuk gerak horizontal, sprite dapat dibalik. Animator dijeda saat musuh diam.
