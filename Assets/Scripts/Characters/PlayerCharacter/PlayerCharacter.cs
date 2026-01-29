using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WeaponComponent))]
public class PlayerCharacter : HitableCharacter, IAttackable, IStunable, IGroggyable
{
    public AttackState _attackState { get; private set; }
    public StunState _stunState { get; private set; }
    public GroggyState _groggyState { get; private set; }

    public WeaponComponent _weaponComponent { get; private set; }

    protected override void OnAwake()
    {
        base.OnAwake();
        
        _weaponComponent = GetComponent<WeaponComponent>();
    }

    protected override void OnStart()
    {
        base.OnStart();

        BindInputAction();
    }

    void Update()
    {

    }

    protected override void Die()
    {
        Debug.Log("player die");
    }

    protected override void InitState()
    {
        base.InitState();

        _attackState = new AttackState(this);
        _stunState = new StunState(this);
        _groggyState = new GroggyState(this);
    }

    public void ApplyDamage(IHitable target)
    {
        target.TakeDamage(ApplyAttackInfo());
    }
    
    public HitInfo ApplyAttackInfo()
    {
        HitInfo attackInfo = new HitInfo();

        attackInfo.damage = CalculateDamage();
        attackInfo.causer = gameObject;

        return attackInfo;
    }

    public float CalculateDamage()
    {
        float result = _stats.strength;

        return result;
    }

    protected void BindInputAction()
    {
        var actions = InputReader.Instance.inputActions.PlayerActionMap;

        InputReader.Instance.BindAction(actions.Move,
            performed: () => _stateMachine.OnMoveInput(actions.Move.ReadValue<Vector2>()),
            canceled: () => _stateMachine.OnEndMoveInput());

        InputReader.Instance.BindAction(actions.Jump,
            started: () => _stateMachine.OnJumpInput(),
            canceled: () => _stateMachine.OnEndJumpInput());

        InputReader.Instance.BindAction(actions.Melee,
            started: () => _stateMachine.OnAttackInput(AttackType.Melee),
            canceled: () => _stateMachine.OnEndAttackInput());

        InputReader.Instance.BindAction(actions.Ranged,
            started: () => _stateMachine.OnAttackInput(AttackType.Range),
            canceled: () => _stateMachine.OnEndAttackInput());

        InputReader.Instance.BindAction(actions.Skill,
            started: () => _stateMachine.OnAttackInput(AttackType.Skill),
            canceled: () => _stateMachine.OnEndAttackInput());
    }
}
