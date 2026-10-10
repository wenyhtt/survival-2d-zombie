using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

/// <summary>
/// Aksi (Action) Behavior Tree yang mengurus logika serangan musuh.
/// Mengecek jarak serangan dan waktu tunggu (cooldown) sebelum memberikan kerusakan (damage) kepada pemain.
/// </summary>
[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Attack", story: "[Self] attack [Player]", category: "Action", id: "f45b94f79d6f0e949dd422487bb3327a")]
public partial class AttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<int> AttackDamage;
    [SerializeReference] public BlackboardVariable<float> AttackCooldown;
    [SerializeReference] public BlackboardVariable<float> AttackRange;

    private float _lastAttackTime;
    private EnemyVision _vision;

    /// <summary>
    /// Dipanggil saat aksi serangan pertama kali dimulai.
    /// Berfungsi untuk melakukan inisialisasi komponen penglihatan (EnemyVision) dari musuh.
    /// </summary>
    protected override Status OnStart()
    {
        // Memastikan referensi musuh (Self) dan pemain (Player) tersedia
        if (Self.Value == null || Player.Value == null)
            return Status.Failure;

        // Mengambil komponen EnemyVision untuk memastikan musuh masih bisa melihat pemain
        _vision = Self.Value.GetComponent<EnemyVision>();
        if (_vision == null)
            return Status.Failure;

        return Status.Running;
    }

    /// <summary>
    /// Dipanggil setiap frame selama aksi berjalan.
    /// Mengontrol logika penyerangan, memeriksa jangkauan, dan menerapkan jeda waktu antar serangan.
    /// </summary>
    protected override Status OnUpdate()
    {
        // Gagal jika salah satu referensi hilang di tengah jalan
        if (Self.Value == null || Player.Value == null || _vision == null)
            return Status.Failure;

        // Jika pemain sudah tidak terlihat lagi, gagalkan aksi penyerangan ini
        if (!_vision.CanSeePlayer(Player.Value))
            return Status.Failure;

        // Memeriksa apakah pemain melarikan diri keluar dari jangkauan saat bersiap menyerang
        float distance = Vector2.Distance(Self.Value.transform.position, Player.Value.transform.position);
        if (distance > AttackRange.Value)
        {
            // Mengembalikan 'Success' (Berhasil) menyelesaikan rangkaian agar pohon perilaku (Behavior Tree)
            // dapat dievaluasi ulang, misalnya kembali ke aksi 'See' atau 'Chase'.
            return Status.Success;
        }

        // Memeriksa apakah waktu jeda serangan (cooldown) telah berlalu
        if (Time.time - _lastAttackTime >= AttackCooldown.Value)
        {
            // Mencoba mengambil komponen Health dari pemain
            Health playerHealth = Player.Value.GetComponent<Health>();
            if (playerHealth != null)
            {
                // Memberikan kerusakan (damage) kepada pemain
                playerHealth.TakeDamage(AttackDamage.Value);
            }

            // Mencatat waktu serangan terakhir
            _lastAttackTime = Time.time;
            
            // Mengembalikan 'Running' (Berjalan) untuk menjaga rangkaian aksi tetap aktif
            // sehingga musuh dapat terus menerus menyerang setelah jeda selesai
            return Status.Running;
        }

        // Jika masih dalam masa jeda serangan tetapi pemain ada dalam jangkauan, 
        // tunggu saja di sini (tetap Running).
        return Status.Running;
    }

    /// <summary>
    /// Dipanggil saat aksi berakhir atau dibatalkan.
    /// Berfungsi untuk membersihkan referensi memori yang tidak terpakai.
    /// </summary>
    protected override void OnEnd()
    {
        // Menghapus referensi visi musuh
        _vision = null;
    }
}
