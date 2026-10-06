# 🎮 Kontrol

Binding input bawaan **Survival 2D Zombie** menggunakan Unity New Input System.

## ⌨️ Keyboard dan Mouse

| Aksi | Input | Keterangan |
|---|---|---|
| Bergerak ke atas | `W` atau `↑` | Gerakan 8 arah |
| Bergerak ke bawah | `S` atau `↓` | |
| Bergerak ke kiri | `A` atau `←` | Sprite dibalik ke kiri |
| Bergerak ke kanan | `D` atau `→` | |
| Menembak / menyerang | Tombol kiri mouse | Menembak dengan senjata jarak jauh atau menyerang ke arah hadap dengan senjata jarak dekat |
| Interaksi | `E` | Membuka/menutup toko GunMan saat berada di dekatnya |
| Pilih senjata 1–2 | `1` / `2` | Memilih senjata yang dimiliki pada slot tersebut |
| Pilih senjata 3–9 | `3`–`9` | Memilih senjata tambahan yang dimiliki |

## Catatan

- Gerakan menggunakan `InputAction` bertipe komposit `Vector2`.
- Pergantian senjata membaca tombol angka melalui `Keyboard.current` di `WeaponSwitcher.cs`.
- Aksi interaksi dihubungkan melalui `InputActionReference` pada `GunManInteract.cs`.
- Aksi serang dihubungkan per senjata melalui `InputActionReference`.
- Arah tembakan ditentukan oleh arah hadap pemain; senjata tidak membidik kursor.

## Interaksi Toko

1. Masuk ke area pemicu NPC **GunMan**.
2. Tekan `E` untuk membuka panel toko.
3. Klik tombol senjata untuk membelinya dengan skor.
4. Tekan `E` lagi atau menjauh untuk menutup toko.
