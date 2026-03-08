using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using static UnityEditor.PlayerSettings;

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

    [Header("Install Indicator Data")]
    [SerializeField] private SpriteRenderer _installIndicator;
    [SerializeField] private Color _canInstallColor = Color.green;
    [SerializeField] private Color _canNotInstallColor = Color.red;

    private Dictionary<AttackType, WeaponDataBase> _weaponDic = new();
    private float[] _cooldownTimes;
    private LineRenderer _lineRenderer;

    private void Awake()
    {
        foreach (var slot in slots)
        {
            _weaponDic[slot.type] = slot.data;
        }

        _cooldownTimes = new float[slots.Count];
    }

    private void Start()
    {
        _firePoint.TryGetComponent<LineRenderer>(out _lineRenderer);
        
        if (_lineRenderer != null) _lineRenderer.enabled = false;
        if (_installIndicator != null) _installIndicator.enabled = false;
    }

    public WeaponDataBase GetWeaponData(AttackType type)
    {
        return _weaponDic.TryGetValue(type, out var weapon) ? weapon : null;
    }

    public WeaponDataBase GetWeaponData(int index)
    {
        if (slots.Count == 0) return null;

        return slots[index].data;
    }

    public int GetSlotCount() => slots.Count;

    public void Equip(AttackType type, WeaponDataBase weapon)
    {
        _weaponDic[type] = weapon;
    }

    public void Swap(AttackType type)
    {
        // TODO : 타입별 무기 리스트 만들어서 다음 인덱스로 스왑
    }

    public bool IsReady(int index)
    {
        if (index < 0 || index >= _cooldownTimes.Length) return false;

        return _cooldownTimes[index] <= Time.time;
    }

    public void SetCooldown(WeaponDataBase targetWeapon)
    {
        int index = GetWeaponIndex(targetWeapon);

        if (index < 0 || index >= _cooldownTimes.Length) return;

        _cooldownTimes[index] = Time.time + slots[index].data.coolDown;
    }

    public int GetWeaponIndex(WeaponDataBase data)
    {
        for (int i = 0; i < slots.Count; ++i)
        {
            if (data == slots[i].data) return i;
        }

        return -1;
    }

    public void UpdateAimLiner(Vector2 aimDirection)
    {
        if (_lineRenderer == null) return;

        if (!IsEnabledAimLiner()) SetAimLinerEnable(true);

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

    public void ShowInstallIndicator(Vector2 installPos, bool canInstall)
    {
        if (_installIndicator == null) return;

        if (!IsEnabledInstallIndicator()) SetInstallIndicatorEnable(true);

        _installIndicator.transform.position = installPos;
        _installIndicator.color = canInstall ? _canInstallColor : _canNotInstallColor;
    }

    public void SetAimLinerEnable(bool enable)
    {
        if (_lineRenderer == null) return;

        _lineRenderer.enabled = enable;
    }

    public void SetInstallIndicatorEnable(bool enable)
    {
        if (_installIndicator == null) return;

        _installIndicator.enabled = enable;
    }

    public bool IsEnabledInstallIndicator() => (_installIndicator != null) ? _installIndicator.enabled : false;

    public bool IsEnabledAimLiner() => (_lineRenderer != null) ? _lineRenderer.enabled : false;

    public Vector2 GetFirepoint() => _firePoint.transform.position;
}