using System;
using UnityEngine;

public class GlobalControlsState : IStateControls
{
    public static event Action<RaycastHit> clickOnElementEvent;

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
        clickOnElementEvent?.Invoke(raycastHit);
    }
}