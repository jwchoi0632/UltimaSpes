using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class EnemyAIController : AIControllerBase
{
    protected PerceptionComponent _perception;
    protected WeaponComponent _weapon;
    protected EnemyStats _stats;

    protected Transform _target;
    protected Coroutine _targetLost;

    protected IChaseable _chase;

    public override void Initialize()
    {
        if (_perception != null)
        {
            BindPerceptionEvents();
            _perception.StartPerception();
        }

        _chase = _possessed as IChaseable;
        _stateMachine.ChangeState(_possessed._idleState);
        StartCoroutine(PostSpawn());
    }

    private IEnumerator PostSpawn()
    {
        yield return new WaitForSeconds(0.2f);

        _stateMachine.OnPatrol();
    }

    protected override void Deinitialize()
    {
        if (_perception != null )
        {
            UnbindPerceptionEvents();
            _perception.StopPerception();
        }

    }

    protected override void InitComponent()
    {
        base.InitComponent();

        _possessed.TryGetComponent<PerceptionComponent>(out _perception);
        _possessed.TryGetComponent<WeaponComponent>(out _weapon);
    }

    public override void RequestAttack()
    {
        base.RequestAttack();

        if (_weapon == null || _weapon?.GetSlotCount() == 0)
        {
            _stateMachine.OnWait(2.0f, _chase._chaseState);
            return;
        }

        switch (_stats.attackSelectType)
        {
            default:
            case AttackSelectType.OnlyFirst:
                OnFirstAttack();
                break;

            case AttackSelectType.Random:
                OnRandomAttack();
                break;

            case AttackSelectType.Decision:
                OnDecisionAttack();
                break;
        }
    }

    public void RequestPostHitAction()
    {
        if (_chase != null)
        {
            _perception.SetChaseMode(true);
            _stateMachine.OnChase();
        }
    }

    protected override void OnStart()
    {
        base.OnStart();

        _stats = _possessed.Stats as EnemyStats;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
    }

    protected virtual void OnFirstAttack()
    {
        if (_weapon.IsReady(0))
        {
            _stateMachine.OnAttack(_weapon.GetWeaponData(0));
        }
        else
        {
            _stateMachine.OnWait(0.5f, _chase._chaseState);
        }
    }

    protected virtual void OnRandomAttack()
    {
        List<int> readyIndices = new List<int>();

        for (int i = 0; i < _weapon.GetSlotCount(); i++)
        {
            if (_weapon.IsReady(i)) readyIndices.Add(i);
        }

        if (readyIndices.Count > 0)
        {
            int randomIndex = readyIndices[UnityEngine.Random.Range(0, readyIndices.Count)];
            _stateMachine.OnAttack(_weapon.GetWeaponData(randomIndex));
        }
        else
        {
            _stateMachine.OnWait(0.5f, _chase._chaseState);
            // TODO : 공격 대기 상태의 로직이 있으면 좋을듯..?
        }
    }

    protected virtual void OnDecisionAttack()
    {

    }

    protected virtual void OnTargetFound(Transform target)
    {
        _target = target;
        Debug.Log("Found Target");
        if (_targetLost != null)
        {
            StopCoroutine(_targetLost);
            _targetLost = null;
        }

        if (_chase != null)
        {
            _chase._chaseState.SetChaseTarget(target);
            _perception.SetChaseMode(true);
        }
        
        _stateMachine.OnNotice(1.0f);
    }

    protected virtual void OnTargetLost()
    {
        if ( _targetLost != null )
        {
            StopCoroutine(_targetLost);
            _targetLost = null;
        }

        _targetLost = StartCoroutine(PostTargetLost());
    }

    protected IEnumerator PostTargetLost()
    {
        yield return new WaitForSeconds(_stats.targetLostTime);

        _target = null;
        _perception.SetChaseMode(false);

        if (_possessed is IReturnable returnable)
        {
            _stateMachine.OnWait(1.0f, returnable._returnState);
        }
        else _stateMachine.OnReturnToStart();
    }

    protected virtual void BindPerceptionEvents()
    {
        _perception.OnTargetFound += OnTargetFound;
        _perception.OnTargetLost += OnTargetLost;
    }

    protected virtual void UnbindPerceptionEvents()
    {
        _perception.OnTargetFound -= OnTargetFound;
        _perception.OnTargetLost -= OnTargetLost;
    }
}