using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PushState : CharacterStateBase
{
    private IInteractable _pushTarget;

    public PushState(CharacterBase character) : base(character) { }

    public void OnCancled()
    {
        if (_pushTarget != null) _owner.ResetState();
    }

    public void SetPushTarget(IInteractable target, float range)
    {
        if (target == null && _pushTarget == null) return;

        if (target == null)
        {
            if (_pushTarget != null)
            {
                float dist = Vector2.Distance(_owner.transform.position, _pushTarget.gameObject.transform.position);

                if (dist > range + 0.5f)
                {
                    _owner.ResetState();
                }
            }

            return;
        }

        if (_pushTarget != target) _pushTarget = target;
    }

    public override void OnStart()
    {
        _stateName = "¹Ð±â";
        base.OnStart();

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

        _pushTarget?.OnInteraction(_owner.gameObject, InteractionType.Push);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

    }

    public override void OnExit()
    {
        base.OnExit();

        //_owner._rigidBody.velocity = Vector2.zero;
        _pushTarget?.OnInteraction(_owner.gameObject, InteractionType.None);
        _pushTarget = null;
    }
}