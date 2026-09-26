using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class InteriorTransition : MonoBehaviour
{
    [Header("Interior Cover (e.g., Roof)")]
    [Tooltip("The Renderer to hide when the player enters the interior (e.g. Barn Roof TilemapRenderer).")]
    [SerializeField] private Renderer interiorCoverRenderer;
    [SerializeField] private GameObject interiorCoverObject;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>() != null)
        {
            SetCoverActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Player>() != null)
        {
            SetCoverActive(true);

        }
    }

    private void SetCoverActive(bool active)
    {
        if (interiorCoverRenderer != null) interiorCoverRenderer.enabled = active;
        if (interiorCoverObject != null) interiorCoverObject.SetActive(active);
    }
}
