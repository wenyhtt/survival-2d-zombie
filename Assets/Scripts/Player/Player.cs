using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    private static readonly int PlayerIdle = Animator.StringToHash("PlayerIdle");
    private static readonly int PlayerWalkSide = Animator.StringToHash("PlayerWalkSide");
    private static readonly int PlayerWalkUp = Animator.StringToHash("PlayerWalkUp");
    private static readonly int PlayerWalkDown = Animator.StringToHash("PlayerWalkDown");
    private const float PlayerWalkSideSkipFirstFrame = 0.48f;

    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private InputActionReference moveActionReference;

    private Rigidbody2D rigidBody;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 movement;
    private int currentAnimation;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMove(InputValue value)
    {
        movement = value.Get<Vector2>();
    }

    private void FixedUpdate()
    {
        if (moveActionReference != null && moveActionReference.action.enabled)
            movement = moveActionReference.action.ReadValue<Vector2>();

        Vector2 clampedMovement = Vector2.ClampMagnitude(movement, 1f);
        PlayMovementAnimation(clampedMovement);
        rigidBody.MovePosition(rigidBody.position + clampedMovement * moveSpeed * Time.fixedDeltaTime);
    }

    private void PlayMovementAnimation(Vector2 direction)
    {
        int nextAnimation = PlayerIdle;

        if (direction.sqrMagnitude > 0.01f)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                nextAnimation = PlayerWalkSide;

                if (spriteRenderer != null)
                    spriteRenderer.flipX = direction.x < 0f;
            }
            else
            {
                nextAnimation = direction.y > 0f ? PlayerWalkUp : PlayerWalkDown;
            }
        }

        if (animator != null && currentAnimation != nextAnimation)
        {
            animator.Play(nextAnimation, 0, nextAnimation == PlayerWalkSide ? PlayerWalkSideSkipFirstFrame : 0f);
            currentAnimation = nextAnimation;
        }
    }
}
