using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour, IInteractiveObject {
    public Collider2D Hatch;

    public void HighLightOff()
    {
        throw new NotImplementedException();
    }

    public void HighLightOn()
    {
        throw new NotImplementedException();
    }

    public void Use(Character Caller)
    {
        Caller.UseLadder(this);
    }
}
