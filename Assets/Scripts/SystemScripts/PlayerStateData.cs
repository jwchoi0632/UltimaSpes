using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateData : MonoBehaviour
{
    [SerializeField] private int _tierNum;
    [SerializeField] private int _maxOutLevel;
    [SerializeField] private List<float> _stageModifierList;

    private int _currentChapter = 1;
    private int _currentStage = 1;
    private int _playerOutLevel = 1;

    public void SetCurrentChapter(int chapter) => _currentChapter = chapter;
    public void SetCurrentStage(int stage) => _currentStage = stage;
    public void SetPlayerOutLevel(int level) => _playerOutLevel = level;

    public void IncreaseChapter()
    {
        ++_currentChapter;
        _currentStage = 1;
    }

    public void IncreaseStage() => ++_currentStage;

    public void ResetStageData()
    {
        _currentChapter = 1;
        _currentStage = 1;
    }

    public void IncreaseOutLevel()
    {
        ++_playerOutLevel;
    }

    public int GetPlayerLevelModifier()
    {
        int levelPerTier = _maxOutLevel / _tierNum;
        int levelModifier = (_playerOutLevel - 1) / levelPerTier;

        return Mathf.Clamp(levelModifier + 1, 1, _tierNum);
    }

    public float GetStageDepthModifier()
    {
        if (_stageModifierList.Count < _currentStage) return -1.0f;

        return _stageModifierList[_currentStage - 1];
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
