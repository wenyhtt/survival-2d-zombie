using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

/// <summary>
/// Aksi (Action) Behavior Tree yang membuat musuh berdiam diri di tempat.
/// Memaksa kecepatan pergerakan musuh menjadi nol secara terus-menerus hingga diinterupsi.
/// Mengembalikan Failure saat pemain terdeteksi agar Behavior Tree dapat beralih ke aksi mengejar.
/// </summary>
[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Idle", story: "[Self] idle", category: "Action", id: "af8b245a3c49c5076071d6fd2949a79f")]
public partial class IdleAction : Action
{
    [SerializeReference] public BlackboardVariable<float> Speed;
    private const float RoamRadius = 1f;
    private const float MinimumPauseDuration = 1f;
    private const float MaximumPauseDuration = 3f;
    private const float DestinationTolerance = 0.05f;

    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Player;

    private Rigidbody2D _rigidbody;
    private EnemyVision _vision;

    protected override Status OnStart()
    {
        if (Self.Value == null)
            return Status.Failure;

            // Mengambil komponen penglihatan musuh untuk mendeteksi pemain
            _vision = Self.Value.GetComponent<EnemyVision>();

            // Hentikan pergerakan seketika
            if (_rigidbody != null)
                _rigidbody.linearVelocity = Vector2.zero;
        }

        // Mencari objek pemain jika belum disetel atau tidak valid
        if (Player.Value == null || !Player.Value.scene.IsValid() || !Player.Value.activeInHierarchy)
            Player.Value = GameObject.FindGameObjectWithTag("Player");

        return Status.Running;
    }

    /// <summary>
    /// Dipanggil setiap frame selama musuh dalam keadaan diam.
    /// Mengembalikan Failure jika pemain terdeteksi agar Behavior Tree beralih ke aksi mengejar.
    /// </summary>
    protected override Status OnUpdate()
    {
        if (_rigidbody == null)
            return Status.Failure;

        if (_isPausing)
        {
            _rigidbody.linearVelocity = Vector2.zero;
            _pauseRemaining -= Time.deltaTime;
            return _pauseRemaining <= 0f ? Status.Failure : Status.Running;
        }

        // Jika penglihatan tersedia dan pemain terdeteksi, keluar dari Idle
        // agar Behavior Tree dapat mengevaluasi ulang dan beralih ke aksi mengejar
        if (_vision != null && Player.Value != null && _vision.CanSeePlayer(Player.Value))
            return Status.Failure;

        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (_rigidbody != null)
            _rigidbody.linearVelocity = Vector2.zero;

        _rigidbody = null;
        _vision = null;
    }
}
