using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

/// <summary>
/// Aksi (Action) Behavior Tree yang mengatur pergerakan musuh untuk mengejar pemain.
/// Bergerak mendekati pemain hingga mencapai batas jarak tertentu (StopDistance).
/// </summary>
[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Chase", story: "[Self] chases [Player]", category: "Action", id: "c590c3e5c3dc08424533f7026046a1e8")]
public partial class ChaseAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<float> Speed;
    [SerializeReference] public BlackboardVariable<float> StopDistance;

    private Rigidbody2D _rb;
    private EnemyVision _vision;

    /// <summary>
    /// Dipanggil saat aksi mengejar pertama kali dimulai.
    /// Melakukan inisialisasi pada komponen fisika (Rigidbody2D) dan penglihatan (EnemyVision).
    /// </summary>
    protected override Status OnStart()
    {
        // Mencari objek pemain yang aktif secara dinamis jika variabel blackboard belum disetel, 
        // mengarah ke prefab, atau mengarah ke objek yang tidak aktif.
        if (Player.Value == null || !Player.Value.scene.IsValid() || !Player.Value.activeInHierarchy)
        {
            Player.Value = GameObject.FindGameObjectWithTag("Player");
        }

        // Jika musuh (Self) atau pemain (Player) masih kosong, batalkan aksi.
        if (Self.Value == null || Player.Value == null)
            return Status.Failure;

        // Mendapatkan referensi komponen fisika dan penglihatan
        _rb = Self.Value.GetComponent<Rigidbody2D>();
        _vision = Self.Value.GetComponent<EnemyVision>();
        if (_rb == null || _vision == null)
            return Status.Failure;

        return Status.Running;
    }

    /// <summary>
    /// Dipanggil setiap frame selama aksi berjalan.
    /// Memperbarui arah dan kecepatan musuh agar terus bergerak menuju pemain.
    /// </summary>
    protected override Status OnUpdate()
    {
        // Gagal jika salah satu referensi hilang di tengah pengejaran
        if (_rb == null || _vision == null || Player.Value == null)
            return Status.Failure;

        // Jika pandangan terhalang atau tidak bisa melihat pemain lagi, hentikan pengejaran
        if (!_vision.CanSeePlayer(Player.Value))
        {
            _rb.linearVelocity = Vector2.zero; // Hentikan gerakan musuh
            return Status.Failure;
        }

        // Menghitung vektor arah dari posisi musuh menuju pemain
        Vector2 toTarget = (Vector2)Player.Value.transform.position - _rb.position;

        // Mengecek apakah musuh sudah cukup dekat (mencapai batas StopDistance)
        if (toTarget.magnitude <= StopDistance.Value)
        {
            _rb.linearVelocity = Vector2.zero; // Hentikan gerakan musuh
            return Status.Success;             // Aksi mengejar dianggap sukses (biasanya lanjut menyerang)
        }

        // Menggerakkan musuh ke arah pemain sesuai dengan nilai Speed yang ditentukan
        _rb.linearVelocity = toTarget.normalized * Speed.Value;
        
        // Terus jalankan aksi ini
        return Status.Running;
    }

    /// <summary>
    /// Dipanggil saat aksi berakhir atau dibatalkan.
    /// Memastikan musuh berhenti bergerak dan mengosongkan referensi.
    /// </summary>
    protected override void OnEnd()
    {
        // Memastikan pergerakan musuh dihentikan
        if (_rb != null)
            _rb.linearVelocity = Vector2.zero;

        // Membersihkan referensi
        _rb = null;
        _vision = null;
    }
}
