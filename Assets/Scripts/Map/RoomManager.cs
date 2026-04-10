using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomCategoryType
{
    None,
    Start,
    EventA,
    EventB,
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

    public RoomWayInformation WayInformation => _roomWayInformation;
    public RoomCategoryType CategoryType => _roomType;
    public RoomContentType ContentType => _roomContentType;

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
