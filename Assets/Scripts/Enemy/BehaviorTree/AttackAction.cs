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
    private EnemyVision _vision;

    protected override Status OnStart()
    {
        if (Self.Value == null || Player.Value == null)
            return Status.Failure;

        _vision = Self.Value.GetComponent<EnemyVision>();
        if (_vision == null)
            return Status.Failure;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Self.Value == null || Player.Value == null || _vision == null)
            return Status.Failure;

        if (!_vision.CanSeePlayer(Player.Value))
            return Status.Failure;

        // Check if player moved out of range while we were preparing to attack
        float distance = Vector2.Distance(Self.Value.transform.position, Player.Value.transform.position);
        if (distance > AttackRange.Value)
        {
            // Complete the sequence so the repeating tree starts over at See and Chase.
            return Status.Success;
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
            // Keep the sequence active so the enemy continues attacking on cooldown.
            return Status.Running;
        }

        // Still cooling down, but player is in range. Wait here.
        return Status.Running;
    }

    protected override void OnEnd()
    {
        _vision = null;
    }
}
