using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class OpenableObject : MonoBehaviour, IInteractable
{
    [SerializeField] protected InteractionType _interactionType;
    [SerializeField] protected float _toDisableTime = 1.0f;
    [SerializeField] protected PickupItem _dropItemPref;
    [SerializeField] protected ObtainItem _dropObtainPref;
    [SerializeField] protected int _dropCount = 3;

    protected bool _isInteractable = true;

    public InteractionType SupportedType => _interactionType;

    public void InitInteractable() => _isInteractable = true;

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
        _isInteractable = false;

        if (_dropItemPref == null) return;

        for (int i = 0; i < _dropCount; ++i)
        {
            int random = Random.Range(0, 2);

            DropItem item = null;

            if (random == 0) item = SceneManagerBase.Instance._poolManager.Get<PickupItem>(_dropItemPref);
            else item = SceneManagerBase.Instance._poolManager.Get<ObtainItem>(_dropObtainPref);

            item.InitItem(i); // TODO : 아이템 정보 랜덤하게 가져오고 전달하는 과정 필요
            item.Drop(transform.position);
        }
    }

    protected virtual bool CheckOpenable(GameObject causer)
    {
        return _isInteractable;
    }

    protected IEnumerator PostOpen()
    {
        yield return new WaitForSeconds(_toDisableTime);

        gameObject.SetActive(false);
    }
}
