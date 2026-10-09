using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(10)]
/// <summary>
/// Mengontrol mekanisme tembakan senjata jarak jauh.
/// Menangani input, pembuatan peluru (Instantiate), pengaturan arah tembak, dan pola penyebaran (spread/shotgun).
/// </summary>
public class WeaponShoot : MonoBehaviour
{
    private Player player;
    [SerializeField] private GameObject bulletPrefab;
    
    [Header("Shotgun Settings")]
    [Tooltip("Jumlah peluru yang ditembakkan sekaligus. Set ke 1 untuk senjata tembakan tunggal, > 1 untuk penyebaran gaya senapan gentel.")]
    [SerializeField] private int pellets = 1;
    [Tooltip("Total sudut penyebaran dalam derajat saat menembakkan banyak pelet.")]
    [SerializeField] private float spreadAngle = 30f;
    
    [Header("Projectile Points")]
    [Tooltip("Tetapkan 4 GameObject kosong yang diposisikan di laras untuk setiap arah.")]
    [SerializeField] private Transform pointRight;
    [SerializeField] private Transform pointLeft;
    [SerializeField] private Transform pointUp;
    [SerializeField] private Transform pointDown;
    
    [Tooltip("Opsional: Tetapkan Tindakan Input untuk menembak.")]
    [SerializeField] private InputActionReference shootActionReference;

    private bool pendingShoot;

    /// <summary>
    /// Menginisialisasi referensi pemain saat dimuat.
    /// </summary>
    private void Awake()
    {
        player ??= GetComponentInParent<Player>();
    }

    /// <summary>
    /// Memeriksa input penembakan setiap frame.
    /// </summary>
    private void Update()
    {
        // Periksa Tindakan Sistem Input baru atau kembali ke Mouse/Keyboard
        if (shootActionReference != null && shootActionReference.action.enabled && shootActionReference.action.WasPressedThisFrame())
        {
            pendingShoot = true;
        }
    }

    /// <summary>
    /// Menangani penembakan setelah semua pembaruan lainnya selesai.
    /// </summary>
    private void LateUpdate()
    {
        if (pendingShoot)
        {
            Shoot();
            pendingShoot = false;
        }
    }

    /// <summary>
    /// Melakukan logika penembakan, membuat peluru dengan arah dan penyebaran yang sesuai.
    /// </summary>
    private void Shoot()
    {
        if (bulletPrefab == null || player == null) return;

        Vector2 shootDirection = Vector2.right; // Standar ke kanan
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
            Debug.LogError($"Titik proyektil untuk arah menghadap saat ini tidak ditetapkan pada {gameObject.name}! Harap tetapkan keempat Titik Proyektil di Inspektur.");
            return;
        }

        Vector3 spawnPosition = activePoint.position;

        for (int i = 0; i < pellets; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
            
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                // Hitung offset sudut penyebaran untuk pelet ini
                float angleOffset = 0f;
                if (pellets > 1)
                {
                    // Distribusikan secara merata dari -spreadAngle/2 ke +spreadAngle/2
                    float step = spreadAngle / (pellets - 1);
                    angleOffset = -spreadAngle / 2f + (step * i);
                }

                // Putar dasar shootDirection dengan angleOffset
                Vector2 finalDirection = Quaternion.Euler(0, 0, angleOffset) * shootDirection;
                bulletScript.Setup(finalDirection);
            }
        }
    }
}
