using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
[CreateAssetMenu(fileName = "TransBallPerformer", menuName = "Combat/AttackPerformer/TransBallPerformer")]
public class TransBallPerformer : AttackPerformerBase
{
    [SerializeField] private TransBall _transballPref;
    [SerializeField] private float _knockImpulse = 2.0f;

    private TransBall _enterTransball;
    private TransBall _exitTransball;

    public override void Excute(CharacterBase owner, AttackDataBase attackData, AttackContext attackContext)
    {
        base.Excute(owner, attackData, attackContext);

        if (attackContext.isFailed)
        {
            Debug.Log("Install Fail");

            if (_exitTransball != null)
            {
                _exitTransball.Deactivate();
                _exitTransball = null;
            }

            return;
        }

        if (_exitTransball == null)
        {
            Debug.Log("Install Exit Transball");
            _exitTransball = SceneManagerBase.Instance._poolManager.Get<TransBall>(_transballPref);
            _exitTransball.transform.position = attackContext.spawnPos;
            _exitTransball.Activate();
        }
        else
        {
            Debug.Log("Install Enter Transball");
            _enterTransball = SceneManagerBase.Instance._poolManager.Get<TransBall>(_transballPref);
            _enterTransball.transform.position = attackContext.spawnPos;

            _enterTransball.SetTransTargetLayer(_finalLayer);
            _enterTransball.SetExitPos(_exitTransball);
            _enterTransball.Activate();

            _exitTransball = null;
            _enterTransball = null;
        }

        float direction = owner._movement._isFacingRight ? 1 : -1;
        Vector2 impulse = Vector2.zero;
        impulse.x = -1 * direction * _knockImpulse;

        owner._movement.ApplyImpulse(impulse);
    }

    public override void Undo()
    {
        base.Undo();

        if (_exitTransball != null)
        {
            _exitTransball.Deactivate();
            _exitTransball = null;
        }
    }
}