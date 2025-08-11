using System;
using UnityEngine;

public class GrabControlState : IStateControls
{
    public static event Action<RaycastHit, bool> clickToGrabEvent;

    public void OnEntry(ControlsStateController controller)
    {

    }

    public void OnUpdate(ControlsStateController controller)
    {

    }

    public void OnExit(ControlsStateController controller)
    {

    }

    public void OnMouseClick(ControlsStateController controller, RaycastHit raycastHit)
    {
        clickToGrabEvent?.Invoke(raycastHit, true);
    }
}