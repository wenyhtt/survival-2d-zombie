using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    private const float WalkSkipFirstFrame = 0.48f;

    [Header("Animation Clips")]
    [SerializeField] private AnimationClip walkSideClip;
    [SerializeField] private AnimationClip walkUpClip;
    [SerializeField] private AnimationClip walkDownClip;

    [SerializeField] private float moveSpeed = 0.8f;
    [SerializeField] private bool useAggroRange = false; // If false, chases infinitely
    [SerializeField] private float aggroRange = 10f;
    [SerializeField] private float attackRange = 1.2f;

    [Header("Combat Settings")]
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Score")]
    [SerializeField] private int scoreValue = 10;
    public int ScoreValue => scoreValue;

    private float lastAttackTime;

    private Transform player;
    private Rigidbody2D rigidBody;
    private Animator animator;
    private SpriteRenderer[] childRenderers;

    private int currentAnimation;
    private int walkSide;
    private int walkUp;
    private int walkDown;
    private bool wasMoving;
    private bool isFacingRight;
    private bool lastFacingRight;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        childRenderers = GetComponentsInChildren<SpriteRenderer>();

        if (animator == null)
        {
            Debug.LogWarning($"{name} has no Animator for EnemyAI animations.", this);
            return;
        }

        if (animator.runtimeAnimatorController == null)
        {
            Debug.LogWarning($"{name} has no Animator Controller for EnemyAI animations.", this);
            return;
        }

        walkSide = GetAnimationStateHash(walkSideClip, "side");
        walkUp = GetAnimationStateHash(walkUpClip, "up");
        walkDown = GetAnimationStateHash(walkDownClip, "down");
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
            // Chase Player
            Vector2 movement = directionToPlayer.normalized;
            PlayMovementAnimation(movement);
            rigidBody.MovePosition(rigidBody.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
        else if (distance <= attackRange)
        {
            // Stop and Attack Player
            PlayMovementAnimation(Vector2.zero);
            
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                AttackPlayer();
                lastAttackTime = Time.time;
            }
        }
        else
        {
            PlayMovementAnimation(Vector2.zero);
        }
    }

    private void AttackPlayer()
    {
        if (player != null)
        {
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
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

        int nextAnimation;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            nextAnimation = walkSide;
            isFacingRight = direction.x > 0f;
        }
        else
        {
            nextAnimation = direction.y > 0f ? walkUp : walkDown;
            isFacingRight = false;
        }

        if (animator != null && nextAnimation == 0)
        {
            animator.speed = 0f;
            currentAnimation = 0;
        }
        else if (animator != null)
        {
            animator.speed = 1f;

            if (currentAnimation != nextAnimation || startedWalking)
            {
                animator.Play(nextAnimation, 0, WalkSkipFirstFrame);
                currentAnimation = nextAnimation;
            }
        }

        wasMoving = true;
    }

    private int GetAnimationStateHash(AnimationClip clip, string direction)
    {
        if (clip == null)
        {
            Debug.LogWarning($"{name} has no {direction} .anim clip attached to EnemyAI.", this);
            return 0;
        }

        int stateHash = Animator.StringToHash(clip.name);
        if (!animator.HasState(0, stateHash))
        {
            Debug.LogWarning($"{name} has no Animator state named '{clip.name}'. Add that .anim clip to the attached controller.", this);
            return 0;
        }

        return stateHash;
    }

    private void ApplyFacingDirection()
    {
        if (isFacingRight != lastFacingRight)
        {
            lastFacingRight = isFacingRight;

            if (childRenderers != null)
            {
                foreach (SpriteRenderer sr in childRenderers)
                    sr.flipX = isFacingRight;
            }
        }
    }
}
