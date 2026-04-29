using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpawnPointBase : MonoBehaviour
{
    public abstract bool InitSpawnPoint(in RoomManager roomManager);
    public abstract void SpawnObject();
}
