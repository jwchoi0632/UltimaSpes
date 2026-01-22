using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuousTrap : TrapBase
{
    [Header("Continuous Option")]
    [SerializeField] protected float _continuousTime = 3.0f;
    [SerializeField] protected GameObject _sprite;
    [SerializeField] protected Vector2 _disablePos; // test
    [SerializeField] protected Vector2 _enablePos; // test

    protected bool _isActivated = false;

    private void Start()
    {
        _sprite.transform.localPosition = _disablePos;// test
    }

    protected override void OnActivate(GameObject Target)
    {
        if (!_isActivated)
        {
            _isActivated = true;
            StartCoroutine(ApplyContinuousTimer());
            //TODO : 애니메이션 등 활성화 동작
            _sprite.transform.localPosition = _enablePos;// test
        }

        Target.GetComponent<CharacterBase>().DecreaseHp(_damage);
    }

    protected IEnumerator ApplyContinuousTimer()
    {
        yield return new WaitForSeconds(_continuousTime);

        _isActivated = false;
        //TODO : 비활성화 동작
        _sprite.transform.localPosition = _disablePos;// test
    }
}
