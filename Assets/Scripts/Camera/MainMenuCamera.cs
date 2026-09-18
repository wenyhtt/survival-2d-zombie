using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    [Header("Movement Area")]
    [Tooltip("The center point of the area where the camera can move.")]
    [SerializeField] private Vector2 areaCenter;

    [Tooltip("The size of the area (width and height).")]
    [SerializeField] private Vector2 areaSize = new Vector2(20f, 20f);

    [Header("Movement Settings")]
    [Tooltip("How fast the camera moves to the next random point.")]
    [SerializeField] private float moveSpeed = 2f;

    [Tooltip("How long the camera waits before moving to a new point after reaching its target.")]
    [SerializeField] private float waitTime = 0f;

    private Vector3 targetPosition;
    private float waitTimer;

    private void Start()
    {
        // Set the first random target point when the game starts
        SetNewRandomTarget();
    }

    private void Update()
    {
        // If we are waiting, decrease the timer
        if (waitTimer > 0)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                SetNewRandomTarget();
            }
            return;
        }

        // Move smoothly towards the target position
        // We recreate targetPosition here to ensure Z is always locked to current Z just in case
        Vector3 currentTarget = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, currentTarget, moveSpeed * Time.deltaTime);

        // Check if the camera has reached the target position
        if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(targetPosition.x, targetPosition.y)) < 0.1f)
        {
            if (waitTime <= 0f)
            {
                // No wait time, pick a new target immediately
                SetNewRandomTarget();
            }
            else
            {
                // Start waiting before picking a new point
                waitTimer = waitTime;
            }
        }
    }

    private void SetNewRandomTarget()
    {
        // Pick a random point within the defined area
        float randomX = Random.Range(areaCenter.x - areaSize.x / 2f, areaCenter.x + areaSize.x / 2f);
        float randomY = Random.Range(areaCenter.y - areaSize.y / 2f, areaCenter.y + areaSize.y / 2f);

        // Keep the original Z position of the camera (important for 2D games)
        targetPosition = new Vector3(randomX, randomY, transform.position.z);

        Debug.Log($"[MainMenuCamera] Picked new target: {targetPosition}. Camera is currently at: {transform.position}");
    }

    // Always draw the movement area border lines in the Scene view
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        float z = transform.position.z;

        // Calculate corner positions
        Vector3 topLeft = new Vector3(areaCenter.x - areaSize.x / 2f, areaCenter.y + areaSize.y / 2f, z);
        Vector3 topRight = new Vector3(areaCenter.x + areaSize.x / 2f, areaCenter.y + areaSize.y / 2f, z);
        Vector3 bottomRight = new Vector3(areaCenter.x + areaSize.x / 2f, areaCenter.y - areaSize.y / 2f, z);
        Vector3 bottomLeft = new Vector3(areaCenter.x - areaSize.x / 2f, areaCenter.y - areaSize.y / 2f, z);

        // Draw the four border lines
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
}
