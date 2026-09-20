using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSwitcher : MonoBehaviour
{
    [Tooltip("List of currently owned weapons. Can start with a default weapon like a handgun.")]
    [SerializeField] private List<Transform> ownedWeapons = new List<Transform>();

    // Tracks source prefabs to prevent duplicate purchases
    private readonly HashSet<GameObject> ownedPrefabs = new HashSet<GameObject>();
    
    private int currentWeaponIndex = 0;

    // We store the keys we want to check in an array for easy looping
    private readonly Key[] numberKeys = {
        Key.Digit1, Key.Digit2, Key.Digit3, Key.Digit4, Key.Digit5,
        Key.Digit6, Key.Digit7, Key.Digit8, Key.Digit9
    };

    private void Start()
    {
        // If the list is empty, initialize it with any weapons already attached (e.g., starter pistol)
        if (ownedWeapons.Count == 0)
        {
            foreach (Transform child in transform)
            {
                ownedWeapons.Add(child);
            }
        }

        // Starter weapons are pre-placed in the prefab and not tracked in ownedPrefabs
        // (they are not purchasable items, so no HashSet registration needed here)
        
        if (ownedWeapons.Count > 0)
        {
            SelectWeapon(currentWeaponIndex);
        }
    }

    private void Update()
    {
        if (Keyboard.current == null || ownedWeapons.Count == 0) return;

        int previousSelectedWeapon = currentWeaponIndex;

        // Cleanly loop through the number keys instead of using a long if-else chain
        for (int i = 0; i < numberKeys.Length; i++)
        {
            if (Keyboard.current[numberKeys[i]].wasPressedThisFrame && i < ownedWeapons.Count)
            {
                currentWeaponIndex = i;
                break; // Stop checking once we find a pressed key
            }
        }

        if (previousSelectedWeapon != currentWeaponIndex)
        {
            SelectWeapon(currentWeaponIndex);
        }
    }

    /// <summary>
    /// Returns true if the player already owns a weapon from the given source prefab.
    /// Used by ShopUI to disable already-purchased buttons.
    /// </summary>
    public bool HasWeapon(GameObject weaponPrefab)
    {
        return ownedPrefabs.Contains(weaponPrefab);
    }

    /// <summary>
    /// Call this method from your Shop/Sale System when a player buys a weapon.
    /// </summary>
    /// <param name="weaponPrefab">The prefab of the weapon being bought.</param>
    public void AddWeapon(GameObject weaponPrefab)
    {
        // Guard: prevent adding a weapon the player already owns
        if (ownedPrefabs.Contains(weaponPrefab))
        {
            Debug.LogWarning($"WeaponSwitcher: Player already owns '{weaponPrefab.name}'. Purchase blocked.");
            return;
        }

        // Instantiate the new weapon as a child of this Weapon parent
        GameObject newWeapon = Instantiate(weaponPrefab, transform);
        ownedPrefabs.Add(weaponPrefab);
        ownedWeapons.Add(newWeapon.transform);
        
        // Auto-equip the newly bought weapon
        currentWeaponIndex = ownedWeapons.Count - 1;
        SelectWeapon(currentWeaponIndex);
    }

    private void SelectWeapon(int index)
    {
        for (int i = 0; i < ownedWeapons.Count; i++)
        {
            if (ownedWeapons[i] != null)
            {
                // Enable the selected weapon and disable the others
                ownedWeapons[i].gameObject.SetActive(i == index);
            }
        }
    }
}
