using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class ChaseState : CharacterStateBase
{
    private EnemyStats _enemyStats;
    private EnemyCharacterBase _character;
    private Transform _target;
    public ChaseState(CharacterBase character) : base(character) 
    { 
        _attackable = true;
        _character = _owner as EnemyCharacterBase;
        _enemyStats = _stats as EnemyStats;
    }

    public void SetChaseTarget(Transform target) => _target = target;

    public override void OnStart()
    {
        base.OnStart();

        if (_stats.moveType == MoveType.OnlyFlying) _movement.IsIgnoreHoverHeight(false);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        Vector2 facingDir = _owner._movement._isFacingRight ? Vector2.right : Vector2.left;

        Collider2D[] candidates = _enemyStats.attackSensor.GetInviewColliders(_owner.transform, facingDir, _enemyStats.targetLayer);

        if (candidates.Length > 0)
        {
            _character._navigation.Stop();
            _character.AIController.RequestAttack();
        }
        else
        {
            _character._navigation.MoveTowards(_target.position);
        }

        //float dist = Vector2.Distance(_target.position, _owner.transform.position);

        //if (dist < _enemyStats.attackRange)
        //{
        //    _character._navigation.Stop(); //공격 로직에 따라 움직이면서 공격할 수도
        //    _character.AIController.RequestAttack();
        //}
        //else
        //{
        //    _character._navigation.MoveTowards(_target.position);
        //}
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}