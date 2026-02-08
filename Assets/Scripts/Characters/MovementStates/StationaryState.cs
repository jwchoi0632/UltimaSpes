using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryState : MovementStateBase
{
    public StationaryState(MovementComponent2D context) : base(context) { }

    public override void OnStart()
    {
        base.OnStart();

        _rb.velocity = Vector3.zero;
    }
    
}
