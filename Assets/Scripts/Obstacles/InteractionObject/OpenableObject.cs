using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class OpenableObject : MonoBehaviour, IInteractable
{
    [SerializeField] protected InteractionType _interactionType;
    [SerializeField] protected float _toDisableTime = 1.0f;
    [SerializeField] protected PickupItem _dropItemPref;
    [SerializeField] protected int _dropCount = 3;

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
                ApplyOpen(causer);
                StartCoroutine(PostOpen());
            }
        }
    }

    protected virtual void ApplyOpen(GameObject causer)
    {
        if (_dropItemPref == null) return;

        for (int i = 0; i < _dropCount; ++i)
        {
            PickupItem item = SceneManagerBase.Instance._poolManager.Get<PickupItem>(_dropItemPref);

            item.InitItem(i); // TODO : 아이템 정보 랜덤하게 가져오고 전달하는 과정 필요
            item.Drop(transform.position);
        }
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
