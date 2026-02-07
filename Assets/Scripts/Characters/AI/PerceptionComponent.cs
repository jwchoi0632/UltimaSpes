using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerceptionComponent : MonoBehaviour
{
    private EnemyStats _stats;
    private EnemyCharacterBase _owner;

    private Coroutine _detection;
    private Transform _currentTarget;

    public event Action<Transform> OnTargetFound;
    public event Action OnTargetLost;

    private void Awake()
    {
        _owner = GetComponent<EnemyCharacterBase>();
        _stats = _owner.Stats as EnemyStats;
    }

    public void StartPerception()
    {
        StopPerception();

        _detection = StartCoroutine(DetectionRoutine());
    }

    public void StopPerception()
    {
        if (_detection != null)
        {
            StopCoroutine(_detection);
            _detection = null;
        }
    }

    private IEnumerator DetectionRoutine()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0, _stats.detectionInterval));

        while (true)
        {
            PerformPerception();
            yield return new WaitForSeconds(_stats.detectionInterval);
        }
    }

    public void SetChaseMode(bool isChase) => _stats.detectionSensor.IsChaseModeSensor(isChase);

    public void PerformPerception()
    {
        if (_stats == null || _stats.detectionSensor == null) return;

        Vector2 facingDir = _owner._movement._isFacingRight ? Vector2.right : Vector2.left;

        Collider2D[] candidates = _stats.detectionSensor.GetInviewColliders(transform, facingDir, _stats.targetLayer);

        Transform bestTarget = null;
        float minDistance = float.MaxValue;

        foreach (var col in candidates)
        {
            Vector2 targetPos = col.transform.position;
            Vector2 origin = transform.position;
            Vector2 dirToTarget = (targetPos - origin).normalized;
            float distToTarget = Vector2.Distance(origin, targetPos);

            RaycastHit2D hit = Physics2D.Raycast(origin, dirToTarget, distToTarget, _stats.obstacleLayer);

            if (hit.collider == null)
            {
                if (distToTarget < minDistance)
                {
                    minDistance = distToTarget;
                    bestTarget = col.transform;
                }
            }
        }

        UpdatePerceptionState(bestTarget);
    }

    private void UpdatePerceptionState(Transform newTarget)
    {
        if (newTarget != _currentTarget)
        {
            _currentTarget = newTarget;

            if (_currentTarget != null) OnTargetFound?.Invoke(_currentTarget);
            else OnTargetLost?.Invoke();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_stats == null || _stats.detectionSensor == null) return;

        Vector2 facingDir = Application.isPlaying && _owner != null
            ? (_owner._movement._isFacingRight ? Vector2.right : Vector2.left)
            : Vector2.right;

        _stats.detectionSensor.DrawDebugGizmos(transform, facingDir);
        _stats.attackSensor.DrawDebugGizmos(transform, facingDir);
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position, _stats.attackRange);
    }
}
