using UnityEngine;

/// <summary>
/// Mengontrol perilaku proyektil peluru, termasuk pergerakan, batas waktu hidup, dan pemberian damage saat mengenai musuh.
/// </summary>
public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 2f;
    [SerializeField] private int damage = 25;
    private Vector2 direction;

    /// <summary>
    /// Menyiapkan peluru dengan arah yang diberikan.
    /// </summary>
    public void Setup(Vector2 dir)
    {
        direction = dir.normalized;
        transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        // Hancurkan peluru setelah beberapa waktu agar tidak memenuhi scene
        Destroy(gameObject, lifeTime);
    }

    /// <summary>
    /// Menggerakkan peluru setiap frame.
    /// </summary>
    private void Update()
    {
        // Pindahkan peluru setiap frame
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    /// <summary>
    /// Menangani tabrakan peluru dengan objek lain.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Abaikan pemain, peluru, dan objek lainnya (senjata, dll.)
        if (collision.CompareTag("Player") || collision.transform.root.CompareTag("Player"))
            return;
        if (collision.CompareTag("Bullet") || collision.transform.root.CompareTag("Bullet"))
            return;
        if (collision.CompareTag("Border") || collision.transform.root.CompareTag("Border"))
            return;

        if (collision.CompareTag("Enemy"))
        {
            // Terapkan kerusakan daripada langsung menghancurkan
            Health enemyHealth = collision.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
            else
            {
                Destroy(collision.gameObject);
            }
        }

        // Hancurkan peluru saat terkena apapun.
        Destroy(gameObject);
    }
}
