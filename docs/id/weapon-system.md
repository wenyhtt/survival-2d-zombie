# 🔫 Sistem Senjata

Panduan senjata jarak jauh dan jarak dekat, pergantian senjata, serta toko.

## 🗂️ Inventaris Senjata

`WeaponSwitcher` mengelola senjata milik pemain. Senjata menjadi GameObject anak pada objek `Weapons` di prefab pemain. Hanya satu senjata aktif pada satu waktu.

## 🔄 Mengganti Senjata

Tombol angka `1–9` memilih slot senjata. Tombol `1` memilih indeks 0, tombol `2` indeks 1, dan seterusnya. Senjata yang baru dibeli otomatis dipasang melalui `AddWeapon()`.

## 🔫 Senjata Jarak Jauh (`WeaponShoot`)

Saat aksi tembak ditekan, `WeaponShoot` memilih salah satu dari empat titik proyektil berdasarkan arah hadap pemain, lalu membuat sejumlah peluru sesuai `pellets`. Jika ada beberapa peluru, sudutnya dibagi dalam rentang `spreadAngle`. Karena itu, senjata mengikuti arah hadap pemain, bukan posisi kursor.

`Bullet` bergerak lurus, dihancurkan setelah `lifeTime`, dan memberikan damage pada musuh yang terkena. Peluru mengabaikan tag `Player`, `Bullet`, dan `Border`; benturan lain menghancurkan peluru.

| Pengaturan | Kegunaan |
|---|---|
| `bulletPrefab` | Prefab peluru |
| `pellets` | Jumlah peluru tiap tembakan |
| `spreadAngle` | Rentang sebaran peluru dalam derajat |
| `pointRight/Left/Up/Down` | Titik asal proyektil untuk tiap arah |
| `shootActionReference` | Input Action untuk menembak |

## 🔪 Senjata Jarak Dekat (`WeaponMelee`)

Serangan menentukan arah dari arah hadap pemain, memainkan animasi dorongan senjata, lalu memakai `Physics2D.OverlapCircleAll` untuk mendeteksi musuh dalam jangkauan. `Health.TakeDamage(damage)` dipanggil pada musuh yang terkena.

| Pengaturan | Kegunaan |
|---|---|
| `damage` | Damage per serangan |
| `attackRange` | Jangkauan serangan |
| `attackCooldown` | Jeda minimum ant serangan |
| `thrustDistance` | Jarak gerak senjata saat menyerang |
| `thrustDuration` | Durasi animasi dorongan dan kembali |
| `attackActionReference` | Input Action untuk menyerang |

## 🎨 Tampilan Senjata (`WeaponFacing`)

Sprite dan urutan gambar berubah sesuai arah hadap pemain. Sprite samping dibalik saat menghadap kiri; sprite atas digambar di belakang pemain dan sprite bawah di depannya.

## 🏪 Sistem Toko

Saat pemain berada di area pemicu GunMan dan menekan tombol interaksi, `GunManInteract` membuka toko. Pembelian menggunakan `PlayerScore.SpendScore()`, lalu `WeaponSwitcher.AddWeapon()` menambahkan dan memasang senjata. Senjata yang sudah dimiliki tidak dapat dibeli lagi; skor yang tidak cukup membuat tombol berkedip merah.

Daftar barang diatur pada Inspector melalui `GunManShopItem` yang memiliki `itemName`, `price`, dan `weaponPrefab`.
