using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class ObtainSensor : MonoBehaviour
{
    [Header("Obtain Setting")]
    [SerializeField] private GameObject _owner;
    [SerializeField] private LayerMask _obtainLayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & _obtainLayer) != 0)
        {
            if (collision.gameObject.TryGetComponent<IInteractable>(out var target))
            {
                if (target.SupportedType.HasFlag(InteractionType.Obtain))
                {
                    target.OnInteraction(_owner, InteractionType.Obtain);
                }
            }
        }
    }
}