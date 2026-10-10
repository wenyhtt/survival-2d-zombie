using UnityEngine;

/// <summary>
/// Menyesuaikan tampilan sprite senjata agar sesuai dengan arah hadap pemain saat ini.
/// Mengganti sprite senjata antara versi samping, atas, dan bawah.
/// </summary>
public class WeaponFacing : MonoBehaviour
{
    private Player player; // Referensi ke Player di skrip
    private SpriteRenderer spriteRenderer;
    
    [SerializeField] private Sprite sideSprite;
    [SerializeField] private Sprite upSprite;
    [SerializeField] private Sprite downSprite;

    /// <summary>
    /// Menginisialisasi komponen dan referensi saat dimuat.
    /// </summary>
    private void Awake()
    {
        // Menugaskan komponen secara otomatis
        spriteRenderer ??= GetComponent<SpriteRenderer>();
        player ??= GetComponentInParent<Player>();
    }

    /// <summary>
    /// Memperbarui arah dan urutan penyortiran sprite senjata setiap frame.
    /// </summary>
    private void Update()
    {
        if (player == null || spriteRenderer == null) return;

        spriteRenderer.flipX = player.IsFacingLeft;

        // Menentukan sprite target dan urutan penyortiran berdasarkan arah
        Sprite targetSprite = sideSprite;
        int targetOrder = 3;

        if (player.IsFacingUp)
        {
            targetSprite = upSprite;
            targetOrder = 1;
        }
        else if (player.IsFacingDown)
        {
            targetSprite = downSprite;
        }

        // Terapkan sprite jika berubah
        if (targetSprite != null && spriteRenderer.sprite != targetSprite)
        {
            spriteRenderer.sprite = targetSprite;
        }

        // Terapkan urutan penyortiran jika berubah
        if (spriteRenderer.sortingOrder != targetOrder)
        {
            spriteRenderer.sortingOrder = targetOrder;
        }
    }
}
