using UnityEngine;

/// <summary>
/// Menggerakkan kamera di menu utama secara otomatis menuju titik-titik acak dalam area yang ditentukan.
/// Memberikan efek visual dinamis pada layar menu tanpa interaksi pemain.
/// </summary>
public class MainMenuCamera : MonoBehaviour
{
    [Header("Movement Area")]
    [Tooltip("Titik pusat dari area dimana kamera dapat bergerak.")]
    [SerializeField] private Vector2 areaCenter;

    [Tooltip("Ukuran area (lebar dan tinggi).")]
    [SerializeField] private Vector2 areaSize = new Vector2(20f, 20f);

    [Header("Movement Settings")]
    [Tooltip("Seberapa cepat kamera bergerak ke titik acak berikutnya.")]
    [SerializeField] private float moveSpeed = 2f;

    [Tooltip("Berapa lama kamera menunggu sebelum bergerak ke titik baru setelah mencapai targetnya.")]
    [SerializeField] private float waitTime = 0f;

    private Vector3 targetPosition;
    private float waitTimer;

    /// <summary>
    /// Mengatur titik target acak pertama saat permainan dimulai.
    /// </summary>
    private void Start()
    {
        // Atur titik target acak pertama saat permainan dimulai
        SetNewRandomTarget();
    }

    /// <summary>
    /// Memperbarui posisi kamera dan menangani waktu tunggu setiap frame.
    /// </summary>
    private void Update()
    {
        // Jika kita sedang menunggu, kurangi pengatur waktu
        if (waitTimer > 0)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                SetNewRandomTarget();
            }
            return;
        }

        // Bergerak dengan mulus menuju posisi target
        // Kami membuat ulang targetPosition di sini untuk memastikan Z selalu terkunci ke Z saat ini untuk berjaga-jaga
        Vector3 currentTarget = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, currentTarget, moveSpeed * Time.deltaTime);

        // Periksa apakah kamera telah mencapai posisi target
        if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(targetPosition.x, targetPosition.y)) < 0.1f)
        {
            if (waitTime <= 0f)
            {
                // Tidak ada waktu tunggu, segera pilih target baru
                SetNewRandomTarget();
            }
            else
            {
                // Mulai menunggu sebelum memilih titik baru
                waitTimer = waitTime;
            }
        }
    }

    /// <summary>
    /// Memilih posisi target acak baru dalam area yang ditentukan.
    /// </summary>
    private void SetNewRandomTarget()
    {
        // Pilih titik acak di dalam area yang ditentukan
        float randomX = Random.Range(areaCenter.x - areaSize.x / 2f, areaCenter.x + areaSize.x / 2f);
        float randomY = Random.Range(areaCenter.y - areaSize.y / 2f, areaCenter.y + areaSize.y / 2f);

        // Pertahankan posisi Z asli kamera (penting untuk game 2D)
        targetPosition = new Vector3(randomX, randomY, transform.position.z);

        Debug.Log($"[MainMenuCamera] Memilih target baru: {targetPosition}. Kamera saat ini berada di: {transform.position}");
    }

    /// <summary>
    /// Selalu menggambar garis batas area pergerakan pada tampilan Scene.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        float z = transform.position.z;

        // Hitung posisi sudut
        Vector3 topLeft = new Vector3(areaCenter.x - areaSize.x / 2f, areaCenter.y + areaSize.y / 2f, z);
        Vector3 topRight = new Vector3(areaCenter.x + areaSize.x / 2f, areaCenter.y + areaSize.y / 2f, z);
        Vector3 bottomRight = new Vector3(areaCenter.x + areaSize.x / 2f, areaCenter.y - areaSize.y / 2f, z);
        Vector3 bottomLeft = new Vector3(areaCenter.x - areaSize.x / 2f, areaCenter.y - areaSize.y / 2f, z);

        // Gambarkan empat garis batas
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
}
