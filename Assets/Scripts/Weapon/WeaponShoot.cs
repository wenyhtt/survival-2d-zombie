using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponShoot : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private GameObject bulletPrefab;
    
    [Tooltip("The gap in Unity units from the weapon center to spawn the bullet.")]
    [SerializeField] private float spawnOffset = 0.5f; 
    
    [Tooltip("Optional: Assign an Input Action for shooting.")]
    [SerializeField] private InputActionReference shootActionReference;

    private void Awake()
    {
        if (player == null)
            player = GetComponentInParent<Player>();
    }

    private void Update()
    {
        bool shootPressed = false;

        // Check for new Input System Action
        if (shootActionReference != null && shootActionReference.action.enabled)
        {
            shootPressed = shootActionReference.action.WasPressedThisFrame();
        }
        // Fallback to Left Mouse Click or Spacebar using Input System
        else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            shootPressed = true;
        }
        else if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            shootPressed = true;
        }

        if (shootPressed)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null || player == null) return;

        Vector2 shootDirection = Vector2.right; // Default to right
        
        if (player.IsFacingUp)
            shootDirection = Vector2.up;
        else if (player.IsFacingDown)
            shootDirection = Vector2.down;
        else if (player.IsFacingLeft)
            shootDirection = Vector2.left * 0.1f;

        // Calculate spawn position based on the weapon's position plus the gap offset
        Vector3 spawnPosition = transform.position + (Vector3)(shootDirection * spawnOffset);

        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.Setup(shootDirection);
        }
    }
}
