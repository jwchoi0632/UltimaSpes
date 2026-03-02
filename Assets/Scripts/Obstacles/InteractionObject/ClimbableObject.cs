using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ClimbableObject : MonoBehaviour, IInteractable
{
    private InteractionType _interactionType = InteractionType.Ladder;

    public InteractionType SupportedType => _interactionType;

    private void Awake()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    public bool CanInteraction(GameObject causer, InteractionType type)
    {
        return true;
    }

    public void OnInteraction(GameObject causer, InteractionType type, Vector2 force = default)
    {
        if (type  == InteractionType.Ladder)
        {

        }
    }
}