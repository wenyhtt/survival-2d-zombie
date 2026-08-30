using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(10)]
public class WeaponShoot : MonoBehaviour
{
    private Player player;
    [SerializeField] private GameObject bulletPrefab;
    
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

        // Fallback to weapon transform if the point isn't assigned
        Vector3 spawnPosition = activePoint != null ? activePoint.position : transform.position;

        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.Setup(shootDirection);
        }
    }
}
