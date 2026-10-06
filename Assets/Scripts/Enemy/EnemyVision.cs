using UnityEngine;

[DisallowMultipleComponent]
public class EnemyVision : MonoBehaviour
{
    private const float DefaultSightRange = 8f;
    private const float DefaultCheckInterval = 1f;
    private const int DefaultSightLayers = (1 << 0) | (1 << 8) | (1 << 9);

    [Header("Vision")]
    [SerializeField, Min(0f)] private float sightRange = DefaultSightRange;
    [SerializeField, Min(0.05f)] private float checkInterval = DefaultCheckInterval;
    [SerializeField] private LayerMask sightLayers = DefaultSightLayers;

    private GameObject _cachedPlayer;
    private bool _hasCachedResult;
    private bool _cachedCanSeePlayer;
    private bool _hasAppliedInitialStagger;
    private float _nextCheckTime;
    private float _initialStagger;

    private void OnEnable()
    {
        _cachedPlayer = null;
        _hasCachedResult = false;
        _hasAppliedInitialStagger = false;
        _initialStagger = Random.Range(0f, Mathf.Max(0f, checkInterval));
    }

    public bool CanSeePlayer(GameObject player)
    {
        if (player == null || !player.activeInHierarchy || !player.scene.IsValid())
        {
            _cachedPlayer = null;
            _hasCachedResult = false;
            return false;
        }

        Vector2 origin = transform.position;
        Vector2 toPlayer = (Vector2)player.transform.position - origin;
        float distanceSquared = toPlayer.sqrMagnitude;
        float range = Mathf.Max(0f, sightRange);

        // The distance check is cheap and runs every call; only nearby players require a raycast.
        if (distanceSquared > range * range)
        {
            _cachedPlayer = player;
            _hasCachedResult = false;
            _cachedCanSeePlayer = false;
            return false;
        }

        if (_cachedPlayer != player)
        {
            _cachedPlayer = player;
            _hasCachedResult = false;
            _hasAppliedInitialStagger = false;
        }

        if (_hasCachedResult && Time.time < _nextCheckTime)
            return _cachedCanSeePlayer;

        _cachedCanSeePlayer = HasClearLineOfSight(player, origin, toPlayer, distanceSquared);
        _hasCachedResult = true;
        _nextCheckTime = Time.time + Mathf.Max(0.05f, checkInterval);

        // Offset each enemy's first refresh so large groups do not raycast on the same frame.
        if (!_hasAppliedInitialStagger)
        {
            _nextCheckTime += _initialStagger;
            _hasAppliedInitialStagger = true;
        }

        return _cachedCanSeePlayer;
    }

    private bool HasClearLineOfSight(GameObject player, Vector2 origin, Vector2 toPlayer, float distanceSquared)
    {
        if (distanceSquared <= Mathf.Epsilon)
            return true;

        float distance = Mathf.Sqrt(distanceSquared);
        RaycastHit2D hit = Physics2D.Raycast(origin, toPlayer / distance, distance, sightLayers);
        return hit.collider != null && hit.transform.root == player.transform.root;
    }
}
