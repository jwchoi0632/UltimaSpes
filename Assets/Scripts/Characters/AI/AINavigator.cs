using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct NavigationContext
{
    public bool isWall, isLedge, isChasing, isOnThin;
    public float yDiff, xDiff;
}

public class AINavigator : MonoBehaviour
{
    private AICharacterBase _owner;
    private CharacterStateMachine _stateMachine;
    private MovementComponent2D _movement;
    private EnemyStats _stats;
    private RoomManager _currentRoom;

    private void Awake()
    {
        _owner = GetComponent<AICharacterBase>();
    }

    public void SetCurrentRoom(RoomManager roomManager) => _currentRoom = roomManager;

    void Start()
    {
        _stateMachine = _owner._stateMachine;
        _movement = _owner._movement;
        _stats = _owner.Stats as EnemyStats;
    }

    public void MoveTowards(Vector2 targetPos)
    {
        if (_stats == null || _stateMachine == null) return;

        Vector2 currentPos = transform.position;
        Vector2 diff = targetPos - currentPos;

        if (CheckArrival(diff))
        {
            Stop();
            return;
        }

        switch (_stats.moveType)
        {
            case MoveType.OnlyFlying:
                HandleFlyingNavigation(diff);
                break;

            case MoveType.OnlyGround:
                HandleGroundedNavigation(diff.x > 0 ? 1f : -1f, targetPos);
                break;

            case MoveType.Both:
                HandleHybridNavigation(diff, targetPos);
                break;
        }
    }

    public void Stop() => _stateMachine.OnEndMoveInput();

    public bool IsPathBlocked()
    {
        float xDir = _movement._isFacingRight ? 1 : -1;

        if (CheckWall(xDir) || (_stats.moveType != MoveType.OnlyFlying && CheckLedge(xDir)))
        {
            return true;
        }

        return false;
    }

    public Vector2 GetNearestValidPoint()
    {
        if (_stats.moveType == MoveType.OnlyFlying)
        {
            RaycastHit2D ceilingHit = Physics2D.Raycast(transform.position, Vector2.up, 10f, _stats.obstacleLayer);
            if (ceilingHit.collider != null)
            {
                return ceilingHit.point + Vector2.down * (_owner._mainCollider.bounds.size.y / 2);
            }
        }

        RaycastHit2D groundHit = Physics2D.Raycast(transform.position, Vector2.down, 10f, _stats.obstacleLayer);
        return groundHit.collider ? groundHit.point + Vector2.up * (_owner._mainCollider.bounds.size.y / 2) : (Vector2)transform.position;
    }

    #region Navigation Strategies

    private bool CheckArrival(Vector2 diff)
    {
        if (_stats.moveType == MoveType.OnlyFlying)
            return diff.magnitude < 0.2f;

        return Mathf.Abs(diff.x) < 0.2f;
    }

    private void HandleGroundedNavigation(float xDir, Vector2 targetPos)
    {
        if (!_stats.useEnvironmentAwareness)
        {
            ExecuteBasicMove(xDir);
            return;
        }

        var context = GetNavigationContext(xDir, targetPos);


        if (CanDropThrough(context, targetPos))
        {
            ExecuteDropMove(xDir);
            return;
        }

        if (ShouldAttemptJump(context, targetPos))
        {
            _stateMachine.OnJumpInput();
        }

        if (IsPathBlocked(context, targetPos))
        {
            Stop();
        }
        else
        {
            ExecuteBasicMove(xDir);
        }
    }

    private void HandleFlyingNavigation(Vector2 diff)
    {
        Vector2 finalDir = diff.normalized;

        if (_stats.useEnvironmentAwareness)
        {
            finalDir = MaintainFlyingDistance(finalDir, diff);

            if (finalDir != Vector2.zero)
            {
                finalDir = GetSmartAvoidanceDir(finalDir);
            }
        }

        _stateMachine.OnMoveInput(finalDir.normalized);
    }

    private void HandleHybridNavigation(Vector2 diff, Vector2 targetPos)
    {
        bool needToFly = Mathf.Abs(diff.y) > _movement.GetMaxJumpHeight() || !_movement.CheckGround();

        if (needToFly)
        {
            //_movement.StartFlying();
            HandleFlyingNavigation(diff);
        }
        else
        {
            HandleGroundedNavigation(diff.x > 0 ? 1f : -1f, targetPos);
        }
    }
    #endregion

    #region 서브 판단 로직 (Conditions)

    private NavigationContext GetNavigationContext(float xDir, Vector2 targetPos)
    {
        return new NavigationContext
        {
            isWall = CheckWall(xDir),
            isLedge = CheckLedge(xDir),
            isChasing = _stateMachine._currentState is ChaseState,
            yDiff = targetPos.y - transform.position.y,
            xDiff = Mathf.Abs(targetPos.x - transform.position.x),
            isOnThin = _movement.IsOnThinPlatform()
        };
    }

    private bool CanDropThrough(NavigationContext ctx, Vector2 targetPos)
    {
        return ctx.isChasing && ctx.isOnThin && ctx.yDiff < -_stats.minJumpHeight;
    }

    private bool ShouldAttemptJump(NavigationContext ctx, Vector2 targetPos)
    {
        if (!_movement.CheckGround()) return false;

        bool forWall = ctx.isWall && ShouldJump(targetPos);
        bool forLedge = ctx.isLedge && ctx.isChasing && ShouldJump(targetPos);
        bool forHeight = ctx.isChasing && ctx.yDiff >= _stats.minJumpHeight && ctx.xDiff < _stats.minJumpWidth;

        return forWall || forLedge || forHeight;
    }

    private bool IsPathBlocked(NavigationContext ctx, Vector2 targetPos)
    {
        if (ctx.isWall && !ShouldJump(targetPos)) return true;

        if (ctx.isLedge)
        {
            if (ctx.isChasing && ctx.yDiff < -0.5f) return false;

            return true;
        }

        return false;
    }

    private Vector2 GetSmartAvoidanceDir(Vector2 currentDir)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, currentDir, _stats.wallCheckDist, _stats.obstacleLayer);

        if (hit.collider == null) return currentDir;

        float checkDist = 2.0f;
        RaycastHit2D upHit = Physics2D.Raycast(transform.position, Vector2.up, checkDist, _stats.obstacleLayer);
        RaycastHit2D downHit = Physics2D.Raycast(transform.position, Vector2.down, checkDist, _stats.obstacleLayer);

        float upSpace = upHit.collider ? upHit.distance : checkDist;
        float downSpace = downHit.collider ? downHit.distance : checkDist;

        float avoidY = (upSpace >= downSpace) ? 1f : -1f;

        return new Vector2(currentDir.x, avoidY).normalized;
    }

    private Vector2 MaintainFlyingDistance(Vector2 currentDir, Vector2 diff)
    {
        float dist = diff.magnitude;
        float stopDist = 0.5f;
        float hoverDist = 2.0f;

        if (dist < stopDist)
        {
            return Vector2.zero;
        }

        if (dist < hoverDist)
        {
            return new Vector2(currentDir.x, 1f).normalized;
        }

        return currentDir;
    }

    #endregion

    #region 구체적 액션 (Actions)

    private void ExecuteBasicMove(float xDir) => _stateMachine.OnMoveInput(new Vector2(xDir, 0));

    private void ExecuteDropMove(float xDir)
    {
        _stateMachine.OnMoveInput(new Vector2(xDir, -1f));
        _stateMachine.OnJumpInput();
    }

    #endregion

    #region Sensors

    private bool CheckWall(float xDir)
    {
        Vector2 origin = (Vector2)transform.position + Vector2.up * 0.2f;
        return Physics2D.Raycast(origin, Vector2.right * xDir, _stats.wallCheckDist, _stats.obstacleLayer);
    }

    private bool CheckLedge(float xDir)
    {
        Vector2 origin = (Vector2)transform.position + (Vector2.right * xDir * _stats.ledgeCheckOffset);
        return !Physics2D.Raycast(origin, Vector2.down, _stats.ledgeCheckDist, _stats.obstacleLayer);
    }

    private bool ShouldJump(Vector2 targetPos)
    {
        if (!_stats.canJump) return false;
        // 떨어지는 상황이 아닐 때만 점프 고려
        if (targetPos.y < transform.position.y - 0.5f) return false;

        float verticalDist = targetPos.y - transform.position.y;
        float maxHeight = _movement.GetMaxJumpHeight();

        return verticalDist < maxHeight + 1.0f;
    }

    #endregion
}