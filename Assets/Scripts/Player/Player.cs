using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
/// <summary>
/// Mengontrol pergerakan, animasi, dan arah hadap karakter pemain.
/// Membaca input dari Unity Input System dan menggerakkan Rigidbody2D sesuai arah yang ditekan.
/// </summary>
public class Player : MonoBehaviour
{
    private static readonly int PlayerWalkSide = Animator.StringToHash("PlayerWalkSide");
    private static readonly int PlayerWalkUp = Animator.StringToHash("PlayerWalkUp");
    private static readonly int PlayerWalkDown = Animator.StringToHash("PlayerWalkDown");
    private const float PlayerWalkSkipFirstFrame = 0.48f;

    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private InputActionReference moveActionReference;
    [SerializeField] private Transform pickableItems;

    private Rigidbody2D rigidBody;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 movement;
    private int currentAnimation;
    private bool wasMoving;
    private bool isFacingLeft;
    private bool lastFacingLeft;

    public bool IsFacingUp => currentAnimation == PlayerWalkUp;
    public bool IsFacingDown => currentAnimation == PlayerWalkDown;
    public bool IsFacingLeft => isFacingLeft;

    /// <summary>Dipanggil saat skrip diinisialisasi (instansiasi).</summary>
    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (pickableItems == null)
        {
            Transform found = transform.Find("PickableItems");
            if (found != null)
                pickableItems = found;
        }
    }

    /// <summary>Dipanggil setiap frame.</summary>
    private void FixedUpdate()
    {
        if (moveActionReference != null && moveActionReference.action.enabled)
            movement = moveActionReference.action.ReadValue<Vector2>();

        Vector2 clampedMovement = Vector2.ClampMagnitude(movement, 1f);
        PlayMovementAnimation(clampedMovement);
        rigidBody.MovePosition(rigidBody.position + clampedMovement * moveSpeed * Time.fixedDeltaTime);
    }

    /// <summary>Dipanggil setiap frame setelah Update selesai.</summary>
    private void LateUpdate()
    {
        ApplyFacingDirection();
    }

    /// <summary>Memainkan animasi pergerakan berdasarkan arah.</summary>
    private void PlayMovementAnimation(Vector2 direction)
    {
        if (direction.sqrMagnitude <= 0.01f)
        {
            if (animator != null)
            {
                if (currentAnimation != 0)
                {
                    animator.Play(currentAnimation, 0, 0f);
                    animator.Update(0f);
                }

                animator.speed = 0f;
            }

            wasMoving = false;
            return;
        }

        bool startedWalking = !wasMoving;

        if (animator != null)
            animator.speed = 1f;

        int nextAnimation;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            nextAnimation = PlayerWalkSide;
            isFacingLeft = direction.x < 0f;
        }
        else
        {
            nextAnimation = direction.y > 0f ? PlayerWalkUp : PlayerWalkDown;
            isFacingLeft = false;
        }

        if (animator != null && (currentAnimation != nextAnimation || startedWalking))
        {
            animator.Play(nextAnimation, 0, PlayerWalkSkipFirstFrame);
            currentAnimation = nextAnimation;
        }

        wasMoving = true;
    }

    /// <summary>Menerapkan arah hadapan pemain.</summary>
    private void ApplyFacingDirection()
    {
        if (isFacingLeft != lastFacingLeft)
        {
            lastFacingLeft = isFacingLeft;

            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = isFacingLeft;
            }
        }

        // Animator menulis nilai X baru setiap frame, jadi kita harus membalik nilainya pada setiap LateUpdate ketika menghadap ke kiri.
        if (pickableItems != null && isFacingLeft)
        {
            Vector3 localPos = pickableItems.localPosition;
            localPos.x = -localPos.x;
            pickableItems.localPosition = localPos;
        }
    }
}
