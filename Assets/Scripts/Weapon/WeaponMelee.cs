using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class WeaponMelee : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private int damage = 25;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private float attackCooldown = 0.5f;
    
    [Header("Visual Settings")]
    [Tooltip("How far the weapon moves when attacking. 0.125 is typically 2 pixels at 16 Pixels Per Unit.")]
    [SerializeField] private float thrustDistance = 0.125f;
    [SerializeField] private float thrustDuration = 0.1f;
    
    [Header("Input")]
    [SerializeField] private InputActionReference attackActionReference;

    private Player player;
    private float lastAttackTime;
    private Vector3 originalLocalPosition;
    private Coroutine attackCoroutine;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
        originalLocalPosition = transform.localPosition;
    }

    private void OnEnable()
    {
        // Reset position just in case it was disabled mid-thrust
        transform.localPosition = originalLocalPosition;
    }

    private void Update()
    {
        bool attackPressed = attackActionReference != null && 
                             attackActionReference.action.enabled && 
                             attackActionReference.action.WasPressedThisFrame();

        if (attackPressed && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
        }
    }

    private void Attack()
    {
        if (player == null) return;

        lastAttackTime = Time.time;

        Vector2 attackDirection = Vector2.right;
        if (player.IsFacingUp) attackDirection = Vector2.up;
        else if (player.IsFacingDown) attackDirection = Vector2.down;
        else if (player.IsFacingLeft) attackDirection = Vector2.left;

        // Trigger visual thrust animation
        if (attackCoroutine != null) StopCoroutine(attackCoroutine);
        attackCoroutine = StartCoroutine(ThrustRoutine(attackDirection));

        // Damage detection using a circle in front of the player
        Vector2 hitCenter = (Vector2)player.transform.position + attackDirection * attackRange;
        Collider2D[] hits = Physics2D.OverlapCircleAll(hitCenter, attackRange * 0.75f);
        
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                Health enemyHealth = hit.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damage);
                }
            }
        }
    }

    private IEnumerator ThrustRoutine(Vector2 direction)
    {
        Vector3 targetPos = originalLocalPosition + (Vector3)(direction * thrustDistance);
        
        float halfDuration = thrustDuration / 2f;
        float elapsed = 0f;

        // Move forward
        while (elapsed < halfDuration)
        {
            transform.localPosition = Vector3.Lerp(originalLocalPosition, targetPos, elapsed / halfDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Move back
        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            transform.localPosition = Vector3.Lerp(targetPos, originalLocalPosition, elapsed / halfDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalLocalPosition;
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the attack range in the Unity Editor
        Player p = GetComponentInParent<Player>();
        Vector2 attackDirection = Vector2.right;
        Vector3 origin = transform.position;

        if (p != null)
        {
            origin = p.transform.position;
            if (p.IsFacingUp) attackDirection = Vector2.up;
            else if (p.IsFacingDown) attackDirection = Vector2.down;
            else if (p.IsFacingLeft) attackDirection = Vector2.left;
        }

        Gizmos.color = Color.red;
        Vector2 hitCenter = (Vector2)origin + attackDirection * attackRange;
        Gizmos.DrawWireSphere(hitCenter, attackRange * 0.75f);
    }
}
