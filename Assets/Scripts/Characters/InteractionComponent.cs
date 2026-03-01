using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Flags]
public enum InteractionType
{
    None = 0,
    Talk = 1 << 0,
    Pickup = 1 << 1,
    Carry = 1 << 2,
    Push = 1 << 3,
    Obtain = 1 << 4,
    Open = 1 << 5,
    Throw = 1 << 6,
    Activate = 1 << 7,
    Breakable = 1 << 8
}

public interface IInteractable
{
    public InteractionType SupportedType { get; }
    public GameObject gameObject { get; }

    public bool CanInteraction(GameObject causer, InteractionType type);
    public void OnInteraction(GameObject causer, InteractionType type, Vector2 force = default);
}

public class InteractionComponent : MonoBehaviour
{
    [Header("Interaction Sensors")]
    [SerializeReference, SubclassSelector] private PerceptionSensor _pickupSensor;
    [SerializeReference, SubclassSelector] private PerceptionSensor _talkSensor;
    [SerializeReference, SubclassSelector] private PerceptionSensor _openSensor;
    [SerializeReference, SubclassSelector] private PerceptionSensor _activateSensor;
    [SerializeReference, SubclassSelector] private PerceptionSensor _pushSensor;
    [SerializeReference, SubclassSelector] private PerceptionSensor _carrySensor;

    [Header("Interaction Settings")]
    [SerializeField] private LayerMask _interactionLayer;

    [Header("Obtain")]
    [SerializeField] private ObtainSensor _obtainSensor;

    private MovementComponent2D _movement;

    private bool _isEnable = true;
    private IInteractable _currentCarryObj = null;

    public float GetPushSensorRange() => _pushSensor.range;

    public void SetInteractionEnable(bool enable)
    {
        _isEnable = enable;
        _obtainSensor.gameObject.SetActive(enable);
    }

    public void OnInteraction()
    {
        if (!_isEnable) return;

        if (OnTalkInteraction()) return;
        if (OnActivateInteraction()) return;
    }

    public bool OnUpDirectionInteraction()
    {
        if (!_isEnable) return false;

        if (OnOpenInteraction()) return true;

        return false;
    }

    public bool OnDownDirectionInteraction()
    {
        if (!_isEnable) return false;

        if (OnPickupInteraction()) return true;

        return false;
    }

    public bool OnTalkInteraction()
    {
        if (!_isEnable) return false;

        IInteractable target = FindBestTarget(_talkSensor, InteractionType.Talk);

        if (target == null) return false;

        target.OnInteraction(gameObject, InteractionType.Talk);

        return true;
    }

    public bool OnActivateInteraction()
    {
        if (!_isEnable) return false;

        IInteractable target = FindBestTarget(_activateSensor, InteractionType.Activate);

        if (target == null) return false;

        target.OnInteraction(gameObject, InteractionType.Activate);

        return true;
    }

    public bool OnCarryInteraction()
    {
        if (!_isEnable) return false;
        if (_currentCarryObj != null) return false;

        IInteractable target = FindBestTarget(_carrySensor, InteractionType.Carry);

        if (target == null) return false;

        _currentCarryObj = target;
        //_currentCarryObj.OnInteraction(gameObject, InteractionType.Carry);

        return true;
    }

    public bool OnPickupInteraction()
    {
        if (!_isEnable) return false;

        IInteractable target = FindBestTarget(_pickupSensor, InteractionType.Pickup);

        if (target == null) return false;

        target.OnInteraction(gameObject, InteractionType.Pickup);

        return true;
    }

    public bool OnPushInteraction()
    {
        if (!_isEnable) return false;

        IInteractable target = FindBestTarget(_pushSensor, InteractionType.Push);

        if (target == null) return false;

        target.OnInteraction(gameObject, InteractionType.Push);

        return true;
    }

    public IInteractable GetPushTarget(float inputX)
    {
        if (!_isEnable) return null;

        IInteractable target = FindBestTarget(_pushSensor, InteractionType.Push);

        if (target != null)
        {
            float relativePosX = target.gameObject.transform.position.x - transform.position.x;

            if (relativePosX * inputX < 0) return null;
        }

        return target;
    }

    public bool OnOpenInteraction()
    {
        if (!_isEnable) return false;

        IInteractable target = FindBestTarget(_openSensor, InteractionType.Open);

        if (target == null) return false;

        target.OnInteraction(gameObject, InteractionType.Open);

        return true;
    }

    public void OnMissCarryObject()
    {
        if (_currentCarryObj == null) return;

        _currentCarryObj.OnInteraction(gameObject, InteractionType.Throw, Vector2.up);
        _currentCarryObj = null;
    }

    public IInteractable GetCarryableObject() => _currentCarryObj;
    public void ClearCarryableObject() => _currentCarryObj = null;

    private IInteractable FindBestTarget(PerceptionSensor sensor, InteractionType type)
    {
        if (sensor == null) return null;

        IInteractable result = null;
        float minDist = float.MaxValue;
        Vector2 dir = _movement._isFacingRight ? Vector2.right : Vector2.left;

        Collider2D[] colliders = sensor.GetInviewColliders(transform, dir, _interactionLayer);

        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.TryGetComponent<IInteractable>(out var interact))
            {
                if (interact.SupportedType.HasFlag(type))
                {
                    float dist = (transform.position - collider.transform.position).sqrMagnitude;

                    if (dist < minDist)
                    {
                        minDist = dist;
                        result = interact;
                    }
                }
            }
        }

        return result;
    }

    private void Awake()
    {
        _movement = GetComponent<MovementComponent2D>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 facingDir = Application.isPlaying && _movement != null
            ? (_movement._isFacingRight ? Vector2.right : Vector2.left)
            : Vector2.right;

        _pushSensor.DrawDebugGizmos(transform, facingDir);
    }
}