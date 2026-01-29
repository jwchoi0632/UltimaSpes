using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponComponent : MonoBehaviour
{
    [System.Serializable]
    public struct WeaponSlot
    {
        public AttackType type;
        public WeaponDataBase data;
    }

    [Header("Default Weapon Data")]
    [SerializeField] private List<WeaponSlot> slots;

    [Header("Aim Liner Data")]
    [SerializeField] private LayerMask _obstacleLayer;
    [SerializeField] private GameObject _firePoint;
    [SerializeField] private float _aimMaxDist = 10.0f;

    private Dictionary<AttackType, WeaponDataBase> _weaponDic = new();
    private LineRenderer _lineRenderer;

    private void Awake()
    {
        foreach (var slot in slots)
        {
            _weaponDic[slot.type] = slot.data;
        }
    }

    private void Start()
    {
        _lineRenderer = _firePoint.GetComponent<LineRenderer>();
        _lineRenderer.enabled = false;
    }

    public WeaponDataBase GetWeaponData(AttackType type)
    {
        return _weaponDic.TryGetValue(type, out var weapon) ? weapon : null;
    }

    public void Equip(AttackType type, WeaponDataBase weapon)
    {
        _weaponDic[type] = weapon;
    }

    public void UpdateAimLiner(Vector2 aimDirection)
    {
        Vector2 startPos = _firePoint.transform.position;

        RaycastHit2D hit = Physics2D.Raycast(startPos, aimDirection, _aimMaxDist, _obstacleLayer);

        Vector3 endPos;

        if (hit.collider != null)
        {
            endPos = hit.point;
        }
        else
        {
            endPos = (Vector3)startPos + (Vector3)aimDirection * _aimMaxDist;
        }

        _lineRenderer.SetPosition(0, startPos);
        _lineRenderer.SetPosition(1, endPos);
    }

    public void SetAimLinerEnable(bool enable)
    {
        _lineRenderer.enabled = enable;
    }
}
