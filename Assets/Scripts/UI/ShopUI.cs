using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Mengelola tampilan UI toko senjata (Shop) sebagai singleton.
/// Menampilkan daftar senjata yang dijual, menangani proses pembelian, dan memperbarui status tombol secara dinamis.
/// </summary>
public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance { get; private set; }

    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Button[] shopButtons;

    public bool IsOpen => shopPanel.activeSelf;
    private Transform currentPlayer;

    // Daftar item yang di-cache sehingga RefreshButtonStates() dapat mengevaluasi kembali setelah pembelian
    private List<GunManShopItem> currentShopItems;

    /// <summary>
    /// Diinisialisasi saat mulai, mengatur instance singleton dan menyembunyikan panel toko.
    /// </summary>
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        shopPanel.SetActive(false);
    }

    /// <summary>
    /// Membuka panel toko dan mengonfigurasi tombol untuk setiap item yang tersedia.
    /// </summary>
    public void OpenShop(List<GunManShopItem> items, Transform player)
    {
        currentShopItems = items;
        currentPlayer = player;
        shopPanel.SetActive(true);

        WeaponSwitcher switcher = currentPlayer != null
            ? currentPlayer.GetComponentInChildren<WeaponSwitcher>()
            : null;

        // Mengonfigurasi tombol panel Canvas yang ada
        for (int i = 0; i < shopButtons.Length; i++)
        {
            if (i < items.Count)
            {
                var item = items[i];
                shopButtons[i].gameObject.SetActive(true);
                shopButtons[i].onClick.RemoveAllListeners();

                TextMeshProUGUI label = shopButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                bool alreadyOwned = switcher != null && switcher.HasWeapon(item.weaponPrefab);

                // Menerapkan status visual yang dimiliki atau yang tersedia
                SetButtonOwned(shopButtons[i], label, item, alreadyOwned);

                if (!alreadyOwned)
                {
                    Button currentButton = shopButtons[i];
                    currentButton.onClick.AddListener(() => BuyWeapon(item, currentButton));
                }
            }
            else
            {
                // Menyembunyikan tombol yang tidak memiliki item toko yang cocok
                shopButtons[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Menutup panel toko.
    /// </summary>
    public void CloseShop()
    {
        shopPanel.SetActive(false);
    }

    /// <summary>
    /// Menangani proses pembelian senjata, mengurangi skor pemain, dan menambahkan senjata ke inventaris jika berhasil.
    /// </summary>
    private void BuyWeapon(GunManShopItem item, Button clickedButton)
    {
        if (currentPlayer != null)
        {
            PlayerScore score = currentPlayer.GetComponent<PlayerScore>();
            if (score != null)
            {
                if (score.SpendScore(item.price))
                {
                    WeaponSwitcher switcher = currentPlayer.GetComponentInChildren<WeaponSwitcher>();
                    if (switcher != null)
                    {
                        switcher.AddWeapon(item.weaponPrefab);
                        Debug.Log($"Bought and equipped: {item.itemName}. Remaining score: {score.CurrentScore}");

                        // Memperbarui semua status tombol sehingga item yang baru dibeli segera menjadi abu-abu
                        RefreshButtonStates();
                    }
                }
                else
                {
                    Debug.Log($"Not enough score to buy {item.itemName}!");
                    if (clickedButton != null)
                    {
                        StartCoroutine(FlashButtonRed(clickedButton));
                    }
                }
            }
        }
    }

    /// <summary>
    /// Menandai tombol toko secara visual sebagai dapat dibeli atau sudah dimiliki.
    /// Tombol yang dimiliki tidak dapat berinteraksi dan menampilkan label "Sudah Dimiliki" atau harga.
    /// </summary>
    private void SetButtonOwned(Button btn, TextMeshProUGUI label, GunManShopItem item, bool owned)
    {
        btn.interactable = !owned;

        if (label != null)
        {
            label.text = owned
                ? $"Already Owned\n{item.itemName}"
                : $"{item.itemName}\n${item.price}";
        }
    }

    /// <summary>
    /// Mengevaluasi ulang semua tombol toko yang terlihat terhadap kepemilikan senjata pemain saat ini.
    /// Dipanggil setelah pembelian yang berhasil sehingga UI diperbarui secara langsung tanpa membuka kembali toko.
    /// </summary>
    private void RefreshButtonStates()
    {
        if (currentPlayer == null || currentShopItems == null) return;

        WeaponSwitcher switcher = currentPlayer.GetComponentInChildren<WeaponSwitcher>();

        for (int i = 0; i < shopButtons.Length; i++)
        {
            if (!shopButtons[i].gameObject.activeSelf) continue;
            if (i >= currentShopItems.Count) continue;

            var item = currentShopItems[i];
            bool owned = switcher != null && switcher.HasWeapon(item.weaponPrefab);
            TextMeshProUGUI label = shopButtons[i].GetComponentInChildren<TextMeshProUGUI>();

            // Menghapus pendengar lama sebelum memutuskan apakah akan menambahkan yang baru
            shopButtons[i].onClick.RemoveAllListeners();
            SetButtonOwned(shopButtons[i], label, item, owned);

            if (!owned)
            {
                Button currentButton = shopButtons[i];
                currentButton.onClick.AddListener(() => BuyWeapon(item, currentButton));
            }
        }
    }

    private HashSet<Button> flashingButtons = new HashSet<Button>();

    /// <summary>
    /// Membuat tombol berkedip merah sementara ketika uang tidak cukup.
    /// </summary>
    private System.Collections.IEnumerator FlashButtonRed(Button button)
    {
        if (flashingButtons.Contains(button)) yield break;

        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            flashingButtons.Add(button);
            Color originalColor = buttonImage.color;
            buttonImage.color = Color.red;

            yield return new WaitForSeconds(0.2f);

            if (buttonImage != null)
            {
                buttonImage.color = originalColor;
            }
            flashingButtons.Remove(button);
        }
    }
}
