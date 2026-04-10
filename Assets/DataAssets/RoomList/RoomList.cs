using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomList", menuName = "Map/RoomList")]
public class RoomList : ScriptableObject
{
    public List<RoomManager> StartRooms;
    public List<RoomManager> EventARooms;
    public List<RoomManager> EventBRooms;
    public List<RoomManager> BossRooms;
    public List<RoomManager> EndRooms;
}