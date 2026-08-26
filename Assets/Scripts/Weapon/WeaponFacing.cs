using UnityEngine;

public class WeaponFacing : MonoBehaviour
{
    private Player player; // Reference to Player on the script
    private SpriteRenderer spriteRenderer;
    
    [SerializeField] private Sprite sideSprite;
    [SerializeField] private Sprite upSprite;
    [SerializeField] private Sprite downSprite;

    private void Awake()
    {
        // Auto-assign components
        spriteRenderer ??= GetComponent<SpriteRenderer>();
        player ??= GetComponentInParent<Player>();
    }

    private void Update()
    {
        if (player == null || spriteRenderer == null) return;

        spriteRenderer.flipX = player.IsFacingLeft;

        // Determine target sprite and sorting order based on direction
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

        // Apply sprite if it changed
        if (targetSprite != null && spriteRenderer.sprite != targetSprite)
        {
            spriteRenderer.sprite = targetSprite;
        }

        // Apply sorting order if it changed
        if (spriteRenderer.sortingOrder != targetOrder)
        {
            spriteRenderer.sortingOrder = targetOrder;
        }
    }
}
