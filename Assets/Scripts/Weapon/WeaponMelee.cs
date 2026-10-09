using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

/// <summary>
/// Mengontrol senjata jarak dekat (melee) yang digunakan pemain.
/// Menangani deteksi area serangan, cooldown, dan pemberian damage kepada musuh yang terkena.
/// </summary>
public class WeaponMelee : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private int damage = 25;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private float attackCooldown = 0.5f;

    [Header("Visual Settings")]
    [Tooltip("Berapa jauh senjata bergerak saat menyerang. 0.125 biasanya adalah 2 piksel pada 16 Piksel Per Unit.")]
    [SerializeField] private float thrustDistance = 0.125f;
    [SerializeField] private float thrustDuration = 0.1f;

    [Header("Input")]
    [SerializeField] private InputActionReference attackActionReference;

    private Player player;
    private float lastAttackTime;
    private Vector3 originalLocalPosition;
    private Coroutine attackCoroutine;

    /// <summary>
    /// Dipanggil saat skrip dimuat. Menginisialisasi komponen dan posisi awal.
    /// </summary>
    private void Awake()
    {
        player = GetComponentInParent<Player>();
        originalLocalPosition = transform.localPosition;
    }

    /// <summary>
    /// Dipanggil saat objek diaktifkan.
    /// </summary>
    private void OnEnable()
    {
        // Atur ulang posisi untuk berjaga-jaga jika dinonaktifkan di tengah tusukan
        transform.localPosition = originalLocalPosition;
    }

    /// <summary>
    /// Diperbarui setiap frame. Memeriksa input serangan.
    /// </summary>
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

    /// <summary>
    /// Melakukan serangan jarak dekat ke arah pemain menghadap.
    /// </summary>
    private void Attack()
    {
        if (player == null) return;

        lastAttackTime = Time.time;

        Vector2 attackDirection = Vector2.right;
        if (player.IsFacingUp) attackDirection = Vector2.up;
        else if (player.IsFacingDown) attackDirection = Vector2.down;
        else if (player.IsFacingLeft) attackDirection = Vector2.left;

        // Memicu animasi tusukan visual
        if (attackCoroutine != null) StopCoroutine(attackCoroutine);
        attackCoroutine = StartCoroutine(ThrustRoutine(attackDirection));

        // Deteksi kerusakan menggunakan lingkaran di depan pemain
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

    /// <summary>
    /// Menggerakkan senjata ke depan dan ke belakang.
    /// </summary>
    private IEnumerator ThrustRoutine(Vector2 direction)
    {
        Vector3 targetPos = originalLocalPosition + (Vector3)(direction * thrustDistance);

        float halfDuration = thrustDuration / 2f;
        float elapsed = 0f;

        // Bergerak maju
        while (elapsed < halfDuration)
        {
            transform.localPosition = Vector3.Lerp(originalLocalPosition, targetPos, elapsed / halfDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Bergerak mundur
        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            transform.localPosition = Vector3.Lerp(targetPos, originalLocalPosition, elapsed / halfDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalLocalPosition;
    }

    /// <summary>
    /// Menggambar visualisasi rentang serangan di Unity Editor.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // Memvisualisasikan jangkauan serangan di Unity Editor
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
