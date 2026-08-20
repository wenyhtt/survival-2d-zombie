using UnityEngine;

public class WeaponFacing : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    [SerializeField] private Sprite sideSprite;
    [SerializeField] private Sprite upSprite;
    [SerializeField] private Sprite downSprite;

    private void Awake()
    {
        // Auto-assign components if not set in the inspector
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
            
        if (player == null)
            player = GetComponentInParent<Player>();
    }

    private void Update()
    {
        if (player == null || spriteRenderer == null) return;

        // Swap the sprite based on the player's facing direction
        if (player.IsFacingUp)
        {
            if (upSprite != null && spriteRenderer.sprite != upSprite)
                spriteRenderer.sprite = upSprite;
                spriteRenderer.sortingOrder = 1;
        }
        else if (player.IsFacingDown)
        {
            if (downSprite != null && spriteRenderer.sprite != downSprite)
                spriteRenderer.sprite = downSprite;
                spriteRenderer.sortingOrder = 3;
        }
        else
        {
            if (sideSprite != null && spriteRenderer.sprite != sideSprite)
                spriteRenderer.sprite = sideSprite;
                spriteRenderer.sortingOrder = 3;
        }
    }
}
