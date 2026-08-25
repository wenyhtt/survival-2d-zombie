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
public class GunManInteract : MonoBehaviour
{
    [SerializeField] private List<GunManShopItem> weaponsForSale;
    [SerializeField] private InputActionReference interactAction;
    
    private bool isPlayerNearby = false;
    private Transform playerTransform;

    private void OnEnable()
    {
        if (interactAction != null)
            interactAction.action.Enable();
    }

    private void OnDisable()
    {
        if (interactAction != null)
            interactAction.action.Disable();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            playerTransform = other.transform;
            Debug.Log("Press Interact to Shop"); // Replace with floating UI later if desired
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            playerTransform = null;
            if (GunManShopUI.Instance != null && GunManShopUI.Instance.IsOpen)
            {
                GunManShopUI.Instance.CloseShop();
            }
        }
    }

    private void Update()
    {
        if (isPlayerNearby && interactAction != null && interactAction.action.WasPressedThisFrame())
        {
            if (!GunManShopUI.Instance.IsOpen)
                GunManShopUI.Instance.OpenShop(weaponsForSale, playerTransform);
            else
                GunManShopUI.Instance.CloseShop();
        }
    }
}
