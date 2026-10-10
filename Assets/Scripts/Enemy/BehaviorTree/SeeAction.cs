using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

/// <summary>
/// Aksi (Action) Behavior Tree yang bertindak sebagai kondisi/sensor penglihatan.
/// Aksi ini memverifikasi apakah musuh dapat melihat pemain dengan jelas tanpa halangan.
/// </summary>
[Serializable, GeneratePropertyBag]
[NodeDescription(name: "See", story: "[self] can see [player]", category: "Action", id: "a6a1ca59bcef1423f4f15f9d4b758825")]
public partial class SeeAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Player;

    private EnemyVision _vision;

    /// <summary>
    /// Dipanggil saat pengecekan penglihatan dimulai.
    /// Berfungsi mencari objek pemain jika belum ada dan mengambil referensi sistem penglihatan musuh.
    /// </summary>
    protected override Status OnStart()
    {
        // Mencari target (pemain) jika nilainya kosong, belum valid di layar, atau tidak aktif
        if (Player.Value == null || !Player.Value.scene.IsValid() || !Player.Value.activeInHierarchy)
            Player.Value = GameObject.FindGameObjectWithTag("Player");

        // Gagal jika target pemain atau diri sendiri tidak ditemukan
        if (Self.Value == null || Player.Value == null)
            return Status.Failure;

        // Mengambil skrip EnemyVision (sensor deteksi musuh) yang terpasang pada diri sendiri
        _vision = Self.Value.GetComponent<EnemyVision>();
        if (_vision == null)
            return Status.Failure;

        return Status.Running;
    }

    /// <summary>
    /// Dipanggil setiap frame selama aksi berjalan.
    /// Melakukan pengecekan secara aktif terhadap garis pandang (line of sight) musuh ke pemain.
    /// </summary>
    protected override Status OnUpdate()
    {
        // Jika ada komponen yang hilang, aksi ini dianggap gagal
        if (_vision == null || Player.Value == null)
            return Status.Failure;

        // Memanggil fungsi CanSeePlayer, jika berhasil melihat mengembalikan Success, jika tidak mengembalikan Failure
        return _vision.CanSeePlayer(Player.Value) ? Status.Success : Status.Failure;
    }

    /// <summary>
    /// Dipanggil saat aksi berakhir atau dibatalkan.
    /// Berfungsi membersihkan variabel referensi penglihatan.
    /// </summary>
    protected override void OnEnd()
    {
        // Mengosongkan referensi sistem penglihatan musuh
        _vision = null;
    }
}
