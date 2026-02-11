using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIControllerBase : ControllerBase
{
    protected AICharacterBase _possessed;

    public abstract void Initialize();
    protected abstract void Deinitialize();

    protected override void InitComponent()
    {
        base.InitComponent();

        _possessed = _character as AICharacterBase;
    }

    public virtual void RequestAttack() { }

    private void OnDisable()
    {
        Deinitialize();
    }
}