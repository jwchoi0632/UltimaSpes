using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.Windows;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PushState : CharacterStateBase
{
    private IInteractable _pushTarget;
    private float _pushRange;
    private float _startDistanceY;
    private Vector2 _originalSize;

    public PushState(CharacterBase character) : base(character) { }

    public void OnCancled()
    {
        //Debug.Log("Push On Cancled");
        if (_pushTarget != null) _owner.ResetState();
    }

    public void SetPushTarget(IInteractable target, float range)
    {
        _pushTarget = target;
        _pushRange = range;

        _startDistanceY = Mathf.Abs(_owner.transform.position.y - _pushTarget.gameObject.transform.position.y);
    }

    public override void OnStart()
    {
        _stateName = "¹Ð±â";
        base.OnStart();

        //Debug.Log("On Push Start");

        //float dir = _owner._movement._isFacingRight ? 1 : -1;
        //var playerCol = _owner._mainCollider;
        //var targetCol = _pushTarget.gameObject.GetComponent<BoxCollider2D>();

        //if (playerCol != null && targetCol != null)
        //{
        //    float targetSurfaceX = dir > 0 ? targetCol.bounds.min.x : targetCol.bounds.max.x;
        //    float playerHalfWidth = playerCol.size.x * 0.5f;

        //    Vector3 snappedPos = _owner.transform.position;
        //    snappedPos.x = targetSurfaceX - (dir * (playerHalfWidth + 0.05f));
        //    _owner.transform.position = snappedPos;
        //}

        //_owner._rigidBody.velocity = Vector2.zero;

        var col = _owner._mainCollider;
        _originalSize = col.size;
        col.size = new Vector2(_originalSize.x - 0.1f, _originalSize.y);

        _pushTarget?.OnInteraction(_owner.gameObject, InteractionType.Push);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

    }

    public override void OnFiexedUpdate()
    {
        base.OnFiexedUpdate();

        CheckPushTarget();
    }

    public override void OnExit()
    {
        base.OnExit();

        //if (_pushTarget != null)
        //{
        //    Vector2 pos = _owner.transform.position;
        //    float distX = _pushTarget.gameObject.transform.position.x - pos.x;
        //    float exitDir = distX > 0 ? -1 : 1;

        //    _owner._rigidBody.AddForce(new Vector2(exitDir * 1f, 0), ForceMode2D.Impulse);
        //}

        _owner._mainCollider.size = _originalSize;

        _pushTarget?.OnInteraction(_owner.gameObject, InteractionType.None);
        _pushTarget = null;
    }

    private void CheckPushTarget()
    {
        float inputx = _movement._moveInput.x;

        if (inputx < 0.1f && inputx > -0.1f)
        {
            OnCancled();
            return;
        }

        float dir = _movement._isFacingRight ? 1 : -1;

        if (dir * inputx < 0)
        {
            OnCancled();
            return;
        }

        IInteractable target = _stateMachine._interaction.GetPushTarget(inputx);

        if (target == null && _pushTarget == null) return;

        if (target == null)
        {
            //if (_pushTarget != target)
            //{
            //    OnCancled();
            //}

            float distX = Mathf.Abs(_owner.transform.position.x - _pushTarget.gameObject.transform.position.x);
            float distY = Mathf.Abs(_owner.transform.position.y - _pushTarget.gameObject.transform.position.y);
            //float dist = Vector2.Distance(_owner.transform.position, _pushTarget.gameObject.transform.position);

            if (distX > _pushRange + 0.5f || distY > _startDistanceY + 0.2f)
            {
                OnCancled();
            }
        }

        //if (_pushTarget != target) _pushTarget = target;
    }
}