using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
/// <summary>
/// Mengelola efek transisi visual saat pemain memasuki atau keluar area interior (misalnya, bangunan).
/// Menyembunyikan atap/penutup ketika pemain berada di dalam, dan menampilkannya kembali saat keluar.
/// </summary>
public class InteriorTransition : MonoBehaviour
{
    [Header("Interior Cover (e.g., Roof)")]
    [Tooltip("The Renderer to hide when the player enters the interior (e.g. Barn Roof TilemapRenderer).")]
    [SerializeField] private Renderer interiorCoverRenderer;
    [SerializeField] private GameObject interiorCoverObject;

    /// <summary>Dipanggil ketika collider lain masuk ke dalam trigger.</summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>() != null)
        {
            SetCoverActive(false);
        }
    }

    /// <summary>Dipanggil ketika collider lain keluar dari trigger.</summary>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Player>() != null)
        {
            SetCoverActive(true);

        }
    }

    /// <summary>Mengatur status aktif dari penutup interior.</summary>
    private void SetCoverActive(bool active)
    {
        if (interiorCoverRenderer != null) interiorCoverRenderer.enabled = active;
        if (interiorCoverObject != null) interiorCoverObject.SetActive(active);
    }
}
