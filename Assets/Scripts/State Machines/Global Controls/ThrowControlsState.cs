using System;
using UnityEngine;

public class ThrowControlsState : IStateControls
{
    public static event Action<RaycastHit, bool> clickToThrowObjectEvent;

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
        clickToThrowObjectEvent?.Invoke(raycastHit, false);
    }
}