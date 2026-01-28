using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HitActionBase : ScriptableObject
{
    public abstract void OnStart(CharacterStateMachine stateMachine);
    public abstract void OnUpdate(CharacterStateMachine stateMachine, HitInfo info);
    public abstract void OnExit(CharacterStateMachine stateMachine);
}