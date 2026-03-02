using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoticeState : CharacterStateBase
{
    private bool _isNoticeable = true;
    private float _currentTime;
    private GameObject _noticeObj;
    private EnemyCharacterBase _character;

    public NoticeState(CharacterBase character) : base(character) 
    {
        _character = _owner as EnemyCharacterBase;
    }

    public void InitNotice(GameObject noticeObj)
    {
        _isNoticeable = true;
        _noticeObj = noticeObj;
        _noticeObj.SetActive(false);
    }

    public void SetNoticeTime(float time) => _currentTime = time;

    public override void OnStart()
    {
        _stateName = "Ã¹ ÀÎ½Ä";
        base.OnStart();

        if (_isNoticeable )
        {
            _noticeObj.SetActive(_isNoticeable);
            _character._navigation.Stop();
        }
    }

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
        _noticeObj.SetActive(_isNoticeable);
    }
}