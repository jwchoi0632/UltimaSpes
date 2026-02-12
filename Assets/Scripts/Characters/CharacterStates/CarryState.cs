using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class CarryState : CharacterStateBase, IAimming
{
    private IInteractable _target;
    private Transform _holdSocket;
    private float _aimInput;
    private float _currentAimAngle;
    private Vector2 _throwDirection;

    public CarryState(CharacterBase character) : base(character) { }

    public void SetCarryableObject(IInteractable target) => _target = target;
    public void SetHoldSocket(Transform socket) => _holdSocket = socket;
    public void SetAimInput(float input_y) => _aimInput = input_y;

    public void OnThrow()
    {
        if (_target != null)
        {
            Vector2 movementDir = new Vector2(_movement._rb.velocity.x, 0);
            Vector2 throwDir = _throwDirection.normalized + movementDir.normalized;
            _target.OnInteraction(_owner.gameObject, InteractionType.Throw, throwDir * 5.0f);
        }
        
        _stateMachine.ChangeState(_owner._normalState);
    }

    public override void OnStart()
    {
        base.OnStart();

        if (_target == null) _stateMachine.ChangeState(_owner._normalState);

        _target.gameObject.transform.SetParent(_holdSocket, false);
        _target.gameObject.transform.localPosition = Vector3.zero;

        _target.OnInteraction(_owner.gameObject, InteractionType.Carry);
        _currentAimAngle = 0.0f;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        UpdateAim();
    }

    public override void OnExit()
    {
        base.OnExit();

        if (_target == null) return;

        _target.gameObject.transform.SetParent(null, true);
        _stateMachine._interaction?.OnMissCarryObject();
        _target = null;
    }

    private void UpdateAim()
    {
        if (_aimInput != 0)
        {
            float targetAngle = _aimInput * 90f;
            _currentAimAngle = Mathf.Lerp(_currentAimAngle, targetAngle, Time.deltaTime * 1.5f);
        }

        _throwDirection = GetAimDirection();
    }

    public Vector2 GetAimDirection()
    {
        float lookDir = _owner._movement._isFacingRight ? 1f : -1f;

        float angleRad = _currentAimAngle * Mathf.Deg2Rad;

        return new Vector2(Mathf.Cos(angleRad) * lookDir, Mathf.Sin(angleRad));
    }
}