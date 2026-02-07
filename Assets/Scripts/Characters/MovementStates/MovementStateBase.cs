using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveable { void Move(Vector2 input); }
public interface IJumpable { void Jump(); }
public interface IGravityEffect { void ApplyGravity(); }

public abstract class MovementStateBase
{
    protected MovementComponent2D _context;
    protected Rigidbody2D _rb;
    protected CharacterStatsBase _stats;

    public MovementStateBase(MovementComponent2D context)
    {
        _context = context;
        _rb = context._rb;
        _stats = context._stats;
    }

    public virtual void OnStart() { }
    public virtual void OnFixedUpdate() { }
    public virtual void OnUpdate() { }
    public virtual void OnExit() { }
}