using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum RoomCategoryType
{
    None,
    Start,
    Common,
    Event,
    Boss,
    End
}

public enum RoomContentType
{
    None,
    Safe,
    Boss,
    Corrider,
    Hazard,
    Puzzle,
    Combat,
    Mixed,
    Event,
    Reward,
    Hidden
}

[Serializable]
public struct RoomWayInformation
{
    public bool leftWay;
    public bool rightWay;
    public bool topWay;
    public bool bottomWay;
}

public class RoomManager : MonoBehaviour
{
    [SerializeField] private RoomWayInformation _roomWayInformation;
    [SerializeField] private RoomCategoryType _roomType;
    [SerializeField] private RoomContentType _roomContentType;
    [SerializeField] private Text _infoText;

    public RoomWayInformation WayInformation => _roomWayInformation;
    public RoomCategoryType CategoryType => _roomType;
    public RoomContentType ContentType => _roomContentType;

    public void InitTestInfo(RoomCategoryType type)
    {
        _roomType = type;

        string info = _roomType.ToString() + " ";

        if (_roomWayInformation.topWay) info += "╩С ";
        if (_roomWayInformation.bottomWay) info += "го ";
        if (_roomWayInformation.leftWay) info += "аб ";
        if (_roomWayInformation.rightWay) info += "©Л ";

        _infoText.text = info;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}