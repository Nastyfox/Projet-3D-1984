using System;
using UnityEngine;

public class DropControlsState : IStateControls
{
    public static event Action<RaycastHit, bool> clickToDropObjectEvent;

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
        clickToDropObjectEvent?.Invoke(raycastHit, false);
    }
}