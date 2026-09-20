using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

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

    protected override Status OnStart()
    {
        if (Self.Value == null || Player.Value == null)
            return Status.Failure;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Self.Value == null || Player.Value == null)
            return Status.Failure;

        // Check if player moved out of range while we were preparing to attack
        float distance = Vector2.Distance(Self.Value.transform.position, Player.Value.transform.position);
        if (distance > AttackRange.Value)
        {
            // Fail the attack so the Behavior Tree sequence restarts and goes back to Chase
            return Status.Failure;
        }

        // Check if cooldown has elapsed
        if (Time.time - _lastAttackTime >= AttackCooldown.Value)
        {
            Health playerHealth = Player.Value.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(AttackDamage.Value);
            }

            _lastAttackTime = Time.time;
            return Status.Success;
        }

        // Still cooling down, but player is in range. Wait here.
        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

