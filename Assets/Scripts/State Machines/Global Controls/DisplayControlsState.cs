using System;
using UnityEngine;

public class DisplayControlsState : IStateControls
{
    public static event Action<RaycastHit, bool> clickToCloseControls;

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
        clickToCloseControls?.Invoke(raycastHit, false);
    }
}