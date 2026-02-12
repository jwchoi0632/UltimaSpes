using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class OpenableObject : MonoBehaviour, IInteractable
{
    [SerializeField] protected InteractionType _interactionType;
    [SerializeField] protected float _toDisableTime = 1.0f;

    public InteractionType SupportedType => _interactionType;

    public bool CanInteraction(GameObject causer, InteractionType type)
    {
        if (type == InteractionType.Open) return CheckOpenable(causer);

        return false;
    }

    public void OnInteraction(GameObject causer, InteractionType type, Vector2 force = default)
    {
        if (type == InteractionType.Open)
        {
            if (CheckOpenable(causer))
            {
                Debug.Log("Open");
                ApplyOpen(causer);
                StartCoroutine(PostOpen());
            }
        }
    }

    protected virtual void ApplyOpen(GameObject causer)
    {
        Debug.Log("Drop Item");
    }

    protected virtual bool CheckOpenable(GameObject causer)
    {
        return true;
    }

    protected IEnumerator PostOpen()
    {
        yield return new WaitForSeconds(_toDisableTime);

        gameObject.SetActive(false);
    }
}
