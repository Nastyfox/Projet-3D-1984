using System;
using UnityEngine;

public class FollowControlsState : IStateControls
{
    public static event Action<RaycastHit, bool> followEvent;

    public void OnEntry(ControlsStateController controller)
    {
        followEvent?.Invoke(new RaycastHit(), false);
    }

    public void OnUpdate(ControlsStateController controller)
    {

    }

    public void OnExit(ControlsStateController controller)
    {

    }

    public void OnMouseClick(ControlsStateController controller, RaycastHit raycastHit)
    {
    
    }
}