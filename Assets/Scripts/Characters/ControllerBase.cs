using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ControllerBase : MonoBehaviour
{
    protected CharacterBase _character;
    protected CharacterStateMachine _stateMachine;

    protected virtual void OnAwake() { }
    protected virtual void OnStart() { }
    protected virtual void OnUpdate() { }

    protected virtual void InitComponent()
    {
        _character = GetComponent<CharacterBase>();
        _stateMachine = GetComponent<CharacterStateMachine>();
    }

    private void Awake()
    {
        InitComponent();
        OnAwake();
    }

    void Start()
    {
        OnStart();
    }

    
    void Update()
    {
        OnUpdate();
    }
}
