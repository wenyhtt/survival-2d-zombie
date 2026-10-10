using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Mengelola inventaris senjata pemain dan pergantian senjata aktif.
/// Mendukung penambahan senjata baru dari toko dan perpindahan antar senjata menggunakan input keyboard atau scroll mouse.
/// </summary>
public class WeaponSwitcher : MonoBehaviour
{
    [Tooltip("List of currently owned weapons. Can start with a default weapon like a handgun.")]
    [SerializeField] private List<Transform> ownedWeapons = new List<Transform>();

    // Melacak prefab sumber untuk mencegah pembelian ganda
    private readonly HashSet<GameObject> ownedPrefabs = new HashSet<GameObject>();

    private int currentWeaponIndex = 0;

    // Kami menyimpan kunci yang ingin kami periksa dalam array untuk perulangan yang mudah
    private readonly Key[] numberKeys = {
        Key.Digit1, Key.Digit2, Key.Digit3, Key.Digit4, Key.Digit5,
        Key.Digit6, Key.Digit7, Key.Digit8, Key.Digit9
    };

    /// <summary>
    /// Inisialisasi daftar senjata saat permainan dimulai.
    /// </summary>
    private void Start()
    {
        // Jika daftar kosong, inisialisasi dengan senjata apa pun yang sudah terpasang (misalnya pistol pemula)
        if (ownedWeapons.Count == 0)
        {
            foreach (Transform child in transform)
            {
                ownedWeapons.Add(child);
            }
        }

        // Senjata pemula ditempatkan sebelumnya di prefab dan tidak dilacak di ownedPrefabs
        // (mereka bukan item yang dapat dibeli, jadi tidak perlu registrasi HashSet di sini)

        if (ownedWeapons.Count > 0)
        {
            SelectWeapon(currentWeaponIndex);
        }
    }

    /// <summary>
    /// Memperbarui logika setiap frame untuk memeriksa input penggantian senjata.
    /// </summary>
    private void Update()
    {
        if (Keyboard.current == null || ownedWeapons.Count == 0) return;

        int previousSelectedWeapon = currentWeaponIndex;

        // Melakukan iterasi dengan bersih melalui tombol angka alih-alih menggunakan rantai if-else yang panjang
        for (int i = 0; i < numberKeys.Length; i++)
        {
            if (Keyboard.current[numberKeys[i]].wasPressedThisFrame && i < ownedWeapons.Count)
            {
                currentWeaponIndex = i;
                break; // Berhenti memeriksa setelah kami menemukan kunci yang ditekan
            }
        }

        if (previousSelectedWeapon != currentWeaponIndex)
        {
            SelectWeapon(currentWeaponIndex);
        }
    }

    /// <summary>
    /// Mengembalikan nilai true jika pemain sudah memiliki senjata dari prefab sumber yang diberikan.
    /// Digunakan oleh ShopUI untuk menonaktifkan tombol yang sudah dibeli.
    /// </summary>
    public bool HasWeapon(GameObject weaponPrefab)
    {
        return ownedPrefabs.Contains(weaponPrefab);
    }

    /// <summary>
    /// Panggil metode ini dari Sistem Toko/Penjualan Anda saat pemain membeli senjata.
    /// </summary>
    /// <param name="weaponPrefab">Prefab dari senjata yang dibeli.</param>
    public void AddWeapon(GameObject weaponPrefab)
    {
        // Mencegah penambahan senjata yang sudah dimiliki pemain
        if (ownedPrefabs.Contains(weaponPrefab))
        {
            Debug.LogWarning($"WeaponSwitcher: Pemain sudah memiliki '{weaponPrefab.name}'. Pembelian diblokir.");
            return;
        }

        // Buat instance senjata baru sebagai anak dari induk Senjata ini
        GameObject newWeapon = Instantiate(weaponPrefab, transform);
        ownedPrefabs.Add(weaponPrefab);
        ownedWeapons.Add(newWeapon.transform);

        // Melengkapi senjata yang baru dibeli secara otomatis
        currentWeaponIndex = ownedWeapons.Count - 1;
        SelectWeapon(currentWeaponIndex);
    }

    /// <summary>
    /// Memilih senjata berdasarkan indeks dan menonaktifkan senjata lainnya.
    /// </summary>
    private void SelectWeapon(int index)
    {
        for (int i = 0; i < ownedWeapons.Count; i++)
        {
            if (ownedWeapons[i] != null)
            {
                // Aktifkan senjata yang dipilih dan nonaktifkan yang lain
                ownedWeapons[i].gameObject.SetActive(i == index);
            }
        }
    }
}
