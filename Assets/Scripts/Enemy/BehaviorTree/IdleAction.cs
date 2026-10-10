using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

/// <summary>
/// Aksi (Action) Behavior Tree yang membuat musuh berdiam diri di tempat.
/// Memaksa kecepatan pergerakan musuh menjadi nol secara terus-menerus hingga diinterupsi.
/// </summary>
[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Idle", story: "[Self] idle", category: "Action", id: "af8b245a3c49c5076071d6fd2949a79f")]
public partial class IdleAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    private Rigidbody2D _rigidbody;

    /// <summary>
    /// Dipanggil saat aksi berdiam diri dimulai.
    /// Mengambil komponen fisika dan memastikan kecepatan gerak disetel ulang menjadi nol.
    /// </summary>
    protected override Status OnStart()
    {
        if (Self.Value != null)
        {
            // Mengambil komponen Rigidbody2D (fisika 2D) dari musuh
            _rigidbody = Self.Value.GetComponent<Rigidbody2D>();

            // Hentikan pergerakan seketika
            if (_rigidbody != null)
                _rigidbody.linearVelocity = Vector2.zero;
        }

        // Aksi berjalan selamanya (sampai aksi lain menggantikan karena kondisi dari luar berubah)
        return Status.Running;
    }

    /// <summary>
    /// Dipanggil setiap frame selama musuh dalam keadaan diam.
    /// Berfungsi memastikan musuh tetap diam tanpa tergeser atau terdorong.
    /// </summary>
    protected override Status OnUpdate()
    {
        // Secara aktif memastikan nilai kecepatan adalah nol (0)
        if (_rigidbody != null)
            _rigidbody.linearVelocity = Vector2.zero;

        return Status.Running;
    }

    /// <summary>
    /// Dipanggil saat aksi berakhir (misal saat musuh beralih ke aksi mengejar).
    /// Berfungsi melepaskan referensi komponen fisika musuh.
    /// </summary>
    protected override void OnEnd()
    {
        // Mengosongkan memori referensi
        _rigidbody = null;
    }
}
