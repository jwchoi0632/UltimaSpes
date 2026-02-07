using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoticeState : CharacterStateBase
{
    private bool _isNoticeable = true;
    private float _currentTime;

    public NoticeState(CharacterBase character) : base(character) { }

    public void InitNotice() => _isNoticeable = true;
    public void SetNoticeTime(float time) => _currentTime = time;

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (_isNoticeable)
        {
            if (_currentTime > 0)
            {
                _currentTime -= Time.deltaTime;
                return;
            }
        }

        _stateMachine.OnChase();
    }

    public override void OnExit()
    {
        base.OnExit();

        _isNoticeable = false;
    }
}