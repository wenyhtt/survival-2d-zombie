using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
/// <summary>
/// Mengontrol animasi dan arah hadap sprite musuh berdasarkan kecepatan geraknya.
/// Menentukan apakah musuh menghadap kiri, kanan, atas, atau bawah berdasarkan data dari Rigidbody2D.
/// </summary>
public class EnemyFacing : MonoBehaviour
{
    private const float WalkSkipFirstFrame = 0.48f;

    [Header("Animation Clips")]
    [SerializeField] private AnimationClip walkSideClip;
    [SerializeField] private AnimationClip walkUpClip;
    [SerializeField] private AnimationClip walkDownClip;

    private Animator animator;
    private SpriteRenderer[] childRenderers;
    private Rigidbody2D rigidBody;

    private int currentAnimation;
    private int walkSide;
    private int walkUp;
    private int walkDown;
    private bool wasMoving;
    private bool isFacingRight;
    private bool lastFacingRight;

    /// <summary>
    /// Menginisialisasi komponen dan hash status animasi pada saat awal.
    /// </summary>
    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        childRenderers = GetComponentsInChildren<SpriteRenderer>();

        if (animator == null)
        {
            Debug.LogWarning($"{name} has no Animator for EnemyFacing animations.", this);
            return;
        }

        if (animator.runtimeAnimatorController == null)
        {
            Debug.LogWarning($"{name} has no Animator Controller for EnemyFacing animations.", this);
            return;
        }

        walkSide = GetAnimationStateHash(walkSideClip, "side");
        walkUp = GetAnimationStateHash(walkUpClip, "up");
        walkDown = GetAnimationStateHash(walkDownClip, "down");
    }

    /// <summary>
    /// Memperbarui animasi pergerakan berdasarkan kecepatan Rigidbody.
    /// </summary>
    private void Update()
    {
        // Menggunakan kecepatan Rigidbody (diatur oleh Behavior Tree) untuk menentukan animasi dan arah hadap
        PlayMovementAnimation(rigidBody.linearVelocity);
    }

    /// <summary>
    /// Menerapkan arah hadap sprite setelah semua pembaruan selesai.
    /// </summary>
    private void LateUpdate()
    {
        ApplyFacingDirection();
    }

    /// <summary>
    /// Memainkan animasi pergerakan berdasarkan arah yang diberikan.
    /// </summary>
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

    /// <summary>
    /// Mendapatkan hash dari klip animasi berdasarkan arah yang diberikan.
    /// </summary>
    private int GetAnimationStateHash(AnimationClip clip, string direction)
    {
        if (clip == null)
        {
            Debug.LogWarning($"{name} has no {direction} .anim clip attached to EnemyFacing.", this);
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

    /// <summary>
    /// Membalikkan sprite renderer untuk menyesuaikan dengan arah pandang musuh.
    /// </summary>
    private void ApplyFacingDirection()
    {
        if (isFacingRight != lastFacingRight)
        {
            lastFacingRight = isFacingRight;

            if (childRenderers != null)
            {
                foreach (SpriteRenderer sr in childRenderers)
                {
                    sr.flipX = isFacingRight;
                }
            }
        }
    }
}
