using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    private static readonly int WalkSide = Animator.StringToHash("KidZombieEnemyWalkSide");
    private static readonly int WalkUp = Animator.StringToHash("KidZombieEnemyWalkUp");
    private static readonly int WalkDown = Animator.StringToHash("KidZombieEnemyWalkDown");
    private const float WalkSkipFirstFrame = 0.48f;

    [SerializeField] private float moveSpeed = 0.8f;
    [SerializeField] private bool useAggroRange = false; // If false, chases infinitely
    [SerializeField] private float aggroRange = 10f;
    [SerializeField] private float attackRange = 0.5f;

    private Transform player;
    private Rigidbody2D rigidBody;
    private Animator animator;
    private SpriteRenderer[] childRenderers;

    private int currentAnimation;
    private bool wasMoving;
    private bool isFacingLeft;
    private bool lastFacingLeft;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        childRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    private void FixedUpdate()
    {
        if (player == null)
        {
            PlayMovementAnimation(Vector2.zero);
            return;
        }

        Vector2 directionToPlayer = player.position - transform.position;
        float distance = directionToPlayer.magnitude;

        bool withinAggro = !useAggroRange || distance <= aggroRange;

        if (withinAggro && distance > attackRange)
        {
            Vector2 movement = directionToPlayer.normalized;
            PlayMovementAnimation(movement);
            rigidBody.MovePosition(rigidBody.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            PlayMovementAnimation(Vector2.zero);
        }
    }

    private void LateUpdate()
    {
        ApplyFacingDirection();
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
            nextAnimation = WalkSide;
            isFacingLeft = direction.x > 0f;
        }
        else
        {
            nextAnimation = direction.y > 0f ? WalkUp : WalkDown;
            isFacingLeft = false;
        }

        if (animator != null && (currentAnimation != nextAnimation || startedWalking))
        {
            animator.Play(nextAnimation, 0, WalkSkipFirstFrame);
            currentAnimation = nextAnimation;
        }

        wasMoving = true;
    }

    private void ApplyFacingDirection()
    {
        if (isFacingLeft != lastFacingLeft)
        {
            lastFacingLeft = isFacingLeft;

            if (childRenderers != null)
            {
                foreach (SpriteRenderer sr in childRenderers)
                    sr.flipX = isFacingLeft;
            }
        }
    }
}
