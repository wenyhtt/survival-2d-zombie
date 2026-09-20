using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance { get; private set; }

    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Button[] shopButtons;

    public bool IsOpen => shopPanel.activeSelf;
    private Transform currentPlayer;

    // Cached item list so RefreshButtonStates() can re-evaluate after a purchase
    private List<GunManShopItem> currentShopItems;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        shopPanel.SetActive(false);
    }

    public void OpenShop(List<GunManShopItem> items, Transform player)
    {
        currentShopItems = items;
        currentPlayer = player;
        shopPanel.SetActive(true);

        WeaponSwitcher switcher = currentPlayer != null
            ? currentPlayer.GetComponentInChildren<WeaponSwitcher>()
            : null;

        // Configure existing Canvas panel buttons
        for (int i = 0; i < shopButtons.Length; i++)
        {
            if (i < items.Count)
            {
                var item = items[i];
                shopButtons[i].gameObject.SetActive(true);
                shopButtons[i].onClick.RemoveAllListeners();

                TextMeshProUGUI label = shopButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                bool alreadyOwned = switcher != null && switcher.HasWeapon(item.weaponPrefab);

                // Apply owned or available visual state
                SetButtonOwned(shopButtons[i], label, item, alreadyOwned);

                if (!alreadyOwned)
                {
                    Button currentButton = shopButtons[i];
                    currentButton.onClick.AddListener(() => BuyWeapon(item, currentButton));
                }
            }
            else
            {
                // Hide buttons that have no matching shop item
                shopButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
    }

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

                        // Refresh all button states so the newly bought item grays out immediately
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
    /// Visually marks a shop button as purchasable or already owned.
    /// Owned buttons are non-interactable and display an "Already Owned" label.
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
    /// Re-evaluates all visible shop buttons against the player's current weapon ownership.
    /// Called after a successful purchase so the UI updates in real-time without reopening the shop.
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

            // Remove old listener before deciding whether to add a new one
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
