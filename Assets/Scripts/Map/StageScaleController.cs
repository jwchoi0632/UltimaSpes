using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageScaleController
{
    private int _currentCombatScale;
    private int _currentRewardScale;

    public StageScaleController(StageScaleData data)
    {
        _currentCombatScale = data.combatScale;
        _currentRewardScale = data.rewardScale;
    }

    public bool IsCombatSpawnable() => _currentCombatScale > 0;
    public bool IsCombatSpawnable(int count) => _currentCombatScale >= count;
    public bool IsRewardSpawnable() => _currentRewardScale > 0;
    public bool IsRewardSpawnable(int count) => _currentCombatScale >= count;
    public void DecreaseCombatScale() => --_currentCombatScale;
    public void DecreaseCombatScale(int count) => _currentCombatScale -= count;
    public void DecreaseRewardScale() => --_currentRewardScale;
    public void DecreaseRewardScale(int count) => _currentCombatScale -= count;
}