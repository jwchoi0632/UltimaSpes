using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingStateSensor : TrapSensorBase
{
    protected override bool CheckCondition(GameObject target)
    {
        MovementComponent2D movement = target.GetComponent<MovementComponent2D>();

        if (movement == null) return false;

        return movement.CheckCurrentState(movement._fallingState);
    }
}
