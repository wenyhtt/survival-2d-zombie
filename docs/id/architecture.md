# 🏗️ Arsitektur

Dokumen ini menjelaskan susunan skrip dan hubungan utama antarsistem.

## Struktur Folder Skrip

| Folder | Tanggung jawab |
|---|---|
| `Assets/Scripts/Camera/` | Kamera gameplay dan menu |
| `Assets/Scripts/Enemy/` | Musuh, AI Behavior Tree, animasi, dan spawner |
| `Assets/Scripts/Environment/` | Interaksi lingkungan, seperti transisi interior |
| `Assets/Scripts/GunMan/` | Interaksi NPC toko |
| `Assets/Scripts/Health/` | Komponen HP bersama untuk pemain dan musuh |
| `Assets/Scripts/Player/` | Gerakan dan skor pemain |
| `Assets/Scripts/UI/` | Menu, status, toko, dan layar Game Over |
| `Assets/Scripts/Weapon/` | Peluru, serangan, tampilan, dan pergantian senjata |

## Hubungan Sistem

- `Player` membaca input dan mengatur gerakan; `PlayerScore` menyimpan skor yang ditampilkan `ScoreUI` dan dipakai `ShopUI`.
- `Health` menangani damage. Kematian pemain memicu `GameOverUI`; kematian musuh memberi skor melalui `EnemyScoreValue`.
- `EnemySpawner` mengatur gelombang, prefab musuh, serta bonus HP. Node Behavior Tree mengendalikan penglihatan, pengejaran, dan serangan musuh.
- `WeaponSwitcher` mengelola senjata. `WeaponShoot` membuat `Bullet`, sementara `WeaponMelee` menangani serangan dekat.
- `GunManInteract` membuka `ShopUI`; toko memotong skor dan meminta `WeaponSwitcher` menambahkan senjata.

## Pola yang Digunakan

| Pola | Penggunaan |
|---|---|
| Singleton | Akses bersama melalui `EnemySpawner.Instance` dan `ShopUI.Instance` |
| Event | `Health` dan `PlayerScore` memberi tahu UI tentang perubahan status |
| Behavior Tree | AI musuh disusun dari node aksi Unity Behavior |
| Komponen | HP, skor, dan tampilan arah dipisah agar dapat digunakan ulang |
| Fisher-Yates | Mengacak urutan prefab musuh |

## Alur Kematian Pemain

`Health.TakeDamage()` mengurangi HP. Saat HP mencapai 0, `Die()` menandai pemain mati, memicu `Health.OnPlayerDied`, dan menghancurkan GameObject. `GameOverUI` menerima event tersebut untuk menampilkan panel Game Over.

## Skor sebagai Mata Uang

Pemain mendapat skor saat mengalahkan musuh melalui `PlayerScore.AddScore()`. `ScoreUI` memperbarui tampilan melalui event `OnScoreChanged`, sedangkan toko memakai `SpendScore()` saat pembelian.
