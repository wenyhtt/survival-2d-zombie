using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "See", story: "[self] can see [player]", category: "Action", id: "a6a1ca59bcef1423f4f15f9d4b758825")]
public partial class SeeAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Player;

    private EnemyVision _vision;

    protected override Status OnStart()
    {
        if (Player.Value == null || !Player.Value.scene.IsValid() || !Player.Value.activeInHierarchy)
            Player.Value = GameObject.FindGameObjectWithTag("Player");

        if (Self.Value == null || Player.Value == null)
            return Status.Failure;

        _vision = Self.Value.GetComponent<EnemyVision>();
        if (_vision == null)
            return Status.Failure;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_vision == null || Player.Value == null)
            return Status.Failure;

        return _vision.CanSeePlayer(Player.Value) ? Status.Success : Status.Failure;
    }

    protected override void OnEnd()
    {
        _vision = null;
    }
}
