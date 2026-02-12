using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BreakableObject : MonoBehaviour, IHitable, IInteractable
{
    [SerializeField] protected InteractionType _interactionType;

    public HitState _hitState { get; private set; }
    public HitDatabase HitData { get; private set;}

    public InteractionType SupportedType => _interactionType;

    public bool CanInteraction(GameObject causer, InteractionType type)
    {
        if (type == InteractionType.Breakable) return true;

        return false;
    }

    public void DecreaseHp(float decreaseValue) { }

    public void IncreaseHp(float increaseValue) { }

    public void OnInteraction(GameObject causer, InteractionType type, Vector2 force = default)
    {
        gameObject.SetActive(false);
    }

    public void PostHit() { }

    public void TakeDamage(HitInfo hitInfo)
    {
        OnInteraction(hitInfo.causer, _interactionType);
    }
}