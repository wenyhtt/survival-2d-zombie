using UnityEngine;

/// <summary>
/// Menggerakkan kamera agar selalu mengikuti posisi target (pemain) dengan efek pergerakan halus (smooth follow).
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float followSpeed = 5f;

    /// <summary>
    /// Mencari target pemain jika belum ditentukan.
    /// </summary>
    private void Awake()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
                target = player.transform;
        }
    }

    /// <summary>
    /// Menggerakkan kamera mengikuti target setiap frame fisika.
    /// </summary>
    private void FixedUpdate()
    {
        if (target == null)
            return;

        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.fixedDeltaTime);
    }
}
