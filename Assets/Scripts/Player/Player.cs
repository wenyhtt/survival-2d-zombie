using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    private static readonly int PlayerWalkSide = Animator.StringToHash("PlayerWalkSide");
    private static readonly int PlayerWalkUp = Animator.StringToHash("PlayerWalkUp");
    private static readonly int PlayerWalkDown = Animator.StringToHash("PlayerWalkDown");
    private const float PlayerWalkSkipFirstFrame = 0.48f;

    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private InputActionReference moveActionReference;

    private Rigidbody2D rigidBody;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 movement;
    private int currentAnimation;
    private bool wasMoving;

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

            if (spriteRenderer != null)
                spriteRenderer.flipX = direction.x < 0f;
        }
        else
        {
            nextAnimation = direction.y > 0f ? PlayerWalkUp : PlayerWalkDown;
        }

        if (animator != null && (currentAnimation != nextAnimation || startedWalking))
        {
            animator.Play(nextAnimation, 0, PlayerWalkSkipFirstFrame);
            currentAnimation = nextAnimation;
        }

        wasMoving = true;
    }
}
