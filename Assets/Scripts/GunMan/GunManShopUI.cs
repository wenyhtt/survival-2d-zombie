using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GunManShopUI : MonoBehaviour
{
    public static GunManShopUI Instance { get; private set; }

    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Button[] shopButtons;

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

        // Configure existing Canvas panel buttons
        for (int i = 0; i < shopButtons.Length; i++)
        {
            if (i < items.Count)
            {
                var item = items[i];
                shopButtons[i].gameObject.SetActive(true);
                shopButtons[i].onClick.RemoveAllListeners();

                TextMeshProUGUI label = shopButtons[i].GetComponentInChildren<TextMeshProUGUI>();

                Button currentButton = shopButtons[i];
                currentButton.onClick.AddListener(() => BuyWeapon(item, currentButton));
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
