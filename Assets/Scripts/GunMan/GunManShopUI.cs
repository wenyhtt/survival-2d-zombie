using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GunManShopUI : MonoBehaviour
{
    public static GunManShopUI Instance { get; private set; }

    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonContainer;

    public bool IsOpen => shopPanel.activeSelf;
    private Transform currentPlayer;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        shopPanel.SetActive(false);
    }

    public void OpenShop(List<GunManShopItem> items, Transform player)
    {
        currentPlayer = player;
        shopPanel.SetActive(true);
        
        // Clear old buttons
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        // Create new buttons
        foreach (var item in items)
        {
            GameObject btnObj = Instantiate(buttonPrefab, buttonContainer);
            // Assuming the button prefab has a Text component for label
            Text btnText = btnObj.GetComponentInChildren<Text>();
            if (btnText != null)
            {
                btnText.text = $"{item.itemName} - ${item.price}";
            }
            
            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(() => BuyWeapon(item));
        }
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
    }

    private void BuyWeapon(GunManShopItem item)
    {
        if (currentPlayer != null)
        {
            PlayerCurrency currency = currentPlayer.GetComponent<PlayerCurrency>();
            if (currency != null)
            {
                if (currency.SpendCoins(item.price))
                {
                    WeaponSwitcher switcher = currentPlayer.GetComponentInChildren<WeaponSwitcher>();
                    if (switcher != null)
                    {
                        switcher.AddWeapon(item.weaponPrefab);
                        Debug.Log($"Bought and equipped: {item.itemName}. Remaining coins: {currency.CurrentCoins}");
                    }
                }
                else
                {
                    Debug.Log($"Not enough coins to buy {item.itemName}!");
                }
            }
        }
    }
}
