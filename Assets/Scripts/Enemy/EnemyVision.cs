using UnityEngine;

[DisallowMultipleComponent]
/// <summary>
/// Sistem penglihatan musuh yang menggunakan raycast untuk mendeteksi pemain.
/// Mengoptimalkan performa dengan meng-cache hasil pengecekan dan menerapkan interval waktu antar pemeriksaan.
/// </summary>
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

    /// <summary>
    /// Mereset variabel-variabel pada saat objek diaktifkan.
    /// </summary>
    private void OnEnable()
    {
        _cachedPlayer = null;
        _hasCachedResult = false;
        _hasAppliedInitialStagger = false;
        _initialStagger = Random.Range(0f, Mathf.Max(0f, checkInterval));
    }

    /// <summary>
    /// Mengecek apakah musuh dapat melihat pemain dengan mempertimbangkan jarak dan halangan visibilitas.
    /// </summary>
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

        // Pemeriksaan jarak tidak memakan banyak sumber daya dan berjalan setiap panggilan; hanya pemain yang dekat yang memerlukan raycast.
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

        // Memberikan offset pada penyegaran pertama setiap musuh sehingga grup besar tidak melakukan raycast pada frame yang sama.
        if (!_hasAppliedInitialStagger)
        {
            _nextCheckTime += _initialStagger;
            _hasAppliedInitialStagger = true;
        }

        return _cachedCanSeePlayer;
    }

    /// <summary>
    /// Mengecek apakah ada garis pandang yang jelas antara musuh dan pemain tanpa terhalang objek lain.
    /// </summary>
    private bool HasClearLineOfSight(GameObject player, Vector2 origin, Vector2 toPlayer, float distanceSquared)
    {
        if (distanceSquared <= Mathf.Epsilon)
            return true;

        float distance = Mathf.Sqrt(distanceSquared);
        RaycastHit2D hit = Physics2D.Raycast(origin, toPlayer / distance, distance, sightLayers);
        return hit.collider != null && hit.transform.root == player.transform.root;
    }
}
