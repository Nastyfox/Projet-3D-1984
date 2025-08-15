using System;

[Serializable]
public class IdleState : StateNPC
{
    protected override void OnEntry()
    {
        agent.ResetPath();
    }
}