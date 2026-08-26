using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponShoot : MonoBehaviour
{
    private Player player;
    [SerializeField] private GameObject bulletPrefab;
    
    [Tooltip("The gap in Unity units from the weapon center to spawn the bullet.")]
    [SerializeField] private float spawnOffset = 0.5f; 
    [SerializeField] private float spawnOffsetFlipX = 0.5f; 
    
    [Tooltip("Optional: Assign an Input Action for shooting.")]
    [SerializeField] private InputActionReference shootActionReference;

    private void Awake()
    {
        player ??= GetComponentInParent<Player>();
    }

    private void Update()
    {
        // Check for new Input System Action or fallback to Mouse/Keyboard
        bool shootPressed = shootActionReference != null && shootActionReference.action.enabled && shootActionReference.action.WasPressedThisFrame();

        if (shootPressed)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null || player == null) return;

        Vector2 shootDirection = Vector2.right; // Default to right
        float currentSpawnOffset = spawnOffset;
        
        if (player.IsFacingUp)
        {
            shootDirection = Vector2.up;
        }
        else if (player.IsFacingDown)
        {
            shootDirection = Vector2.down;
        }
        else if (player.IsFacingLeft)
        {
            shootDirection = Vector2.left;
            currentSpawnOffset = spawnOffsetFlipX; // Use the specific offset for left facing
        }

        // Calculate spawn position based on the weapon's position plus the gap offset
        Vector3 spawnPosition = transform.position + (Vector3)(shootDirection * currentSpawnOffset);

        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.Setup(shootDirection);
        }
    }
}
