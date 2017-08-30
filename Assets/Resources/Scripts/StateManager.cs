using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    public Action<State> ChangeState;

    public State(Action<State> ChangeState)
    {
        this.ChangeState = ChangeState;
    }

    public virtual void Start() { }

    public virtual void Update() { }

    public virtual void End() { }
}

public class StateManager : MonoBehaviour
{
    protected State CurrentState;
    protected State NextState;

    protected void AcceptState()
    {
        if (NextState == null) return;
        string Log = "";
        if (CurrentState != null)
        {
            Log += CurrentState.ToString();
            CurrentState.End();
        }
        NextState.Start();
        Log += " -> " + NextState.ToString();
        CurrentState = NextState;
        NextState = null;
        Debug.Log(Log);
    }

    protected void ChangeState(State nextState)
    {
        NextState = nextState;
        /*
        string Log = "";
        if (CurrentState != null)
        {
            Log += CurrentState.ToString();
            CurrentState.End();
        }
        CurrentState = nextState;
        CurrentState.Start();
        Log += " -> " + CurrentState.ToString();
        Debug.Log(Log);*/
    }
}
