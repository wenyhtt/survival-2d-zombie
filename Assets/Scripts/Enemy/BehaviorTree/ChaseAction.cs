using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Chase", story: "[Self] chases [Player]", category: "Action", id: "c590c3e5c3dc08424533f7026046a1e8")]
public partial class ChaseAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<float> Speed;
    [SerializeReference] public BlackboardVariable<float> StopDistance;

    private Rigidbody2D _rb;

    protected override Status OnStart()
    {
        // Dynamically find the active player if the blackboard variable is unset, 
        // points to a prefab, or points to an inactive object.
        if (Player.Value == null || !Player.Value.scene.IsValid() || !Player.Value.activeInHierarchy)
        {
            Player.Value = GameObject.FindGameObjectWithTag("Player");
        }

        if (Self.Value == null || Player.Value == null)
            return Status.Failure;

        _rb = Self.Value.GetComponent<Rigidbody2D>();
        if (_rb == null)
            return Status.Failure;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_rb == null || Player.Value == null)
            return Status.Failure;

        Vector2 toTarget = (Vector2)Player.Value.transform.position - _rb.position;

        if (toTarget.magnitude <= StopDistance.Value)
        {
            _rb.linearVelocity = Vector2.zero;
            return Status.Success;
        }

        _rb.linearVelocity = toTarget.normalized * Speed.Value;
        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (_rb != null)
            _rb.linearVelocity = Vector2.zero;
    }
}