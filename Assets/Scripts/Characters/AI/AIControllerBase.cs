using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIControllerBase : MonoBehaviour
{
    protected AICharacterBase _possessed;
    protected CharacterStateMachine _stateMachine;

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

    public abstract void Initialize();
    protected abstract void Deinitialize();

    public virtual void RequestAttack() { }

    protected virtual void OnAwake() { }
    protected virtual void OnStart() { }
    protected virtual void OnUpdate() { }

    protected virtual void InitComponent()
    {
        _possessed = GetComponent<AICharacterBase>();
        _stateMachine = GetComponent<CharacterStateMachine>();
    }

    private void OnDisable()
    {
        Deinitialize();
    }
}