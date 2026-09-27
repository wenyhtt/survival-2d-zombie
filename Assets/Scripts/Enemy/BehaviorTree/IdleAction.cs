using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

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

    private Rigidbody2D _rigidbody;
    private Vector2 _homePosition;
    private Vector2 _destination;
    private float _pauseRemaining;
    private bool _isPausing;

    protected override Status OnStart()
    {
        if (Self.Value == null)
            return Status.Failure;

        _rigidbody = Self.Value.GetComponent<Rigidbody2D>();
        if (_rigidbody == null)
            return Status.Failure;

        _homePosition = _rigidbody.position;
        _destination = _homePosition + UnityEngine.Random.insideUnitCircle * RoamRadius;
        _isPausing = false;
        _pauseRemaining = 0f;
        return Status.Running;
    }

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

        Vector2 toDestination = _destination - _rigidbody.position;
        if (toDestination.sqrMagnitude <= DestinationTolerance * DestinationTolerance)
        {
            _rigidbody.linearVelocity = Vector2.zero;
            _pauseRemaining = UnityEngine.Random.Range(MinimumPauseDuration, MaximumPauseDuration);
            _isPausing = true;
            return Status.Running;
        }

        _rigidbody.linearVelocity = toDestination.normalized * Speed;
        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (_rigidbody != null)
            _rigidbody.linearVelocity = Vector2.zero;

        _rigidbody = null;
    }
}
