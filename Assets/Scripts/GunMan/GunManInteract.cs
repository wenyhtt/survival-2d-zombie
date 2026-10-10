using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[System.Serializable]
public struct GunManShopItem
{
    public string itemName;
    public int price;
    public GameObject weaponPrefab;
}

[RequireComponent(typeof(Collider2D))]
/// <summary>
/// Mengelola interaksi antara pemain dan NPC penjual senjata (GunMan).
/// Mendeteksi kedekatan pemain menggunakan trigger dan membuka/menutup UI toko senjata.
/// </summary>
public class GunManInteract : MonoBehaviour
{
    [SerializeField] private List<GunManShopItem> weaponsForSale;
    [SerializeField] private InputActionReference interactAction;

    private bool isPlayerNearby = false;
    private Transform playerTransform;

    /// <summary>Dipanggil ketika objek diaktifkan.</summary>
    private void OnEnable()
    {
        if (interactAction != null)
            interactAction.action.Enable();
    }

    /// <summary>Dipanggil ketika objek dinonaktifkan.</summary>
    private void OnDisable()
    {
        if (interactAction != null)
            interactAction.action.Disable();
    }

    /// <summary>Dipanggil ketika collider lain masuk ke dalam trigger.</summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            playerTransform = other.transform;
            Debug.Log("Press Interact to Shop"); // Ganti dengan UI mengambang (floating UI) nanti jika diinginkan
        }
    }

    /// <summary>Dipanggil ketika collider lain keluar dari trigger.</summary>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            playerTransform = null;
            if (ShopUI.Instance != null && ShopUI.Instance.IsOpen)
            {
                ShopUI.Instance.CloseShop();
            }
        }
    }

    /// <summary>Dipanggil setiap frame.</summary>
    private void Update()
    {
        if (isPlayerNearby && interactAction != null && interactAction.action.WasPressedThisFrame())
        {
            if (!ShopUI.Instance.IsOpen)
                ShopUI.Instance.OpenShop(weaponsForSale, playerTransform);
            else
                ShopUI.Instance.CloseShop();
        }
    }
}
