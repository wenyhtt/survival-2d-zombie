using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(10)]
public class WeaponShoot : MonoBehaviour
{
    private Player player;
    [SerializeField] private GameObject bulletPrefab;
    
    [Header("Shotgun Settings")]
    [Tooltip("Number of bullets fired at once. Set to 1 for single-shot weapons, > 1 for shotgun-style spread.")]
    [SerializeField] private int pellets = 1;
    [Tooltip("Total spread angle in degrees when firing multiple pellets.")]
    [SerializeField] private float spreadAngle = 30f;
    
    [Header("Projectile Points")]
    [Tooltip("Assign 4 empty GameObjects positioned at the barrel for each direction.")]
    [SerializeField] private Transform pointRight;
    [SerializeField] private Transform pointLeft;
    [SerializeField] private Transform pointUp;
    [SerializeField] private Transform pointDown;
    
    [Tooltip("Optional: Assign an Input Action for shooting.")]
    [SerializeField] private InputActionReference shootActionReference;

    private bool pendingShoot;

    private void Awake()
    {
        player ??= GetComponentInParent<Player>();
    }

    private void Update()
    {
        // Check for new Input System Action or fallback to Mouse/Keyboard
        if (shootActionReference != null && shootActionReference.action.enabled && shootActionReference.action.WasPressedThisFrame())
        {
            pendingShoot = true;
        }
    }

    private void LateUpdate()
    {
        if (pendingShoot)
        {
            Shoot();
            pendingShoot = false;
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null || player == null) return;

        Vector2 shootDirection = Vector2.right; // Default to right
        Transform activePoint = pointRight;

        if (player.IsFacingUp)
        {
            shootDirection = Vector2.up;
            activePoint = pointUp;
        }
        else if (player.IsFacingDown)
        {
            shootDirection = Vector2.down;
            activePoint = pointDown;
        }
        else if (player.IsFacingLeft)
        {
            shootDirection = Vector2.left;
            activePoint = pointLeft;
        }

        if (activePoint == null)
        {
            Debug.LogError($"Projectile point for current facing direction is not assigned on {gameObject.name}! Please assign all 4 Projectile Points in the Inspector.");
            return;
        }

        Vector3 spawnPosition = activePoint.position;

        for (int i = 0; i < pellets; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
            
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                // Calculate spread angle offset for this pellet
                float angleOffset = 0f;
                if (pellets > 1)
                {
                    // Evenly distribute from -spreadAngle/2 to +spreadAngle/2
                    float step = spreadAngle / (pellets - 1);
                    angleOffset = -spreadAngle / 2f + (step * i);
                }

                // Rotate the base shootDirection by angleOffset
                Vector2 finalDirection = Quaternion.Euler(0, 0, angleOffset) * shootDirection;
                bulletScript.Setup(finalDirection);
            }
        }
    }
}
