# 🎮 Gambaran Game

**Survival 2D Zombie** adalah game survival zombie 2D tampak atas dengan gelombang musuh. Bertahanlah dari gelombang zombie yang makin sulit, dapatkan skor dengan mengalahkan musuh, lalu gunakan skor untuk membeli senjata dari NPC toko.

## Alur Utama

1. Hitung mundur dimulai di awal permainan dan setelah gelombang selesai.
2. Musuh muncul dari tepi peta atau secara acak di area kemunculan.
3. Kalahkan musuh untuk mendapatkan skor.
4. Setelah semua musuh dikalahkan, gelombang selesai dan hitung mundur berikutnya dimulai.
5. Gunakan skor untuk membeli senjata dari NPC **GunMan**.
6. Jika HP pemain mencapai 0, layar **Game Over** ditampilkan.

## Fitur

| Fitur | Deskripsi |
|---|---|
| Gelombang musuh | Gelombang terus berlanjut dengan jumlah musuh yang bertambah secara acak |
| Senjata jarak jauh | Menembakkan peluru dengan dukungan sebaran |
| Senjata jarak dekat | Serangan jarak dekat dengan animasi ayunan/lonjakan |
| Pergantian senjata | Pilih senjata yang dimiliki dengan tombol angka 1–9 |
| Toko | Beli senjata dari NPC GunMan menggunakan skor |
| Sistem kesehatan | Komponen `Health` bersama dengan efek terkena serangan dan peningkatan HP musuh |
| AI Behavior Tree | Musuh mengejar dan menyerang pemain menggunakan Unity Behavior |
| Animasi arah | Animasi pemain dan musuh mengikuti arah gerak |
| Skor | Musuh memberi skor saat dikalahkan; skor juga menjadi mata uang |
| Game Over | Permainan dijeda; pemain dapat memulai ulang atau kembali ke menu |

## Scene

| Scene | Kegunaan |
|---|---|
| `MainMenu` | Layar judul dan navigasi menu |
| `Main` | Scene permainan utama |

## Aset Seni

**Zombie Apocalypse Tileset** menyediakan sprite, tile, animasi, dan elemen UI, termasuk lingkungan kota dan pedesaan, elemen animasi, sprite zombie, serta ilustrasi poster.
