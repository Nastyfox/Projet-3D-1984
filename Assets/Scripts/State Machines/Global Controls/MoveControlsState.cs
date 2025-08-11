using System;
using UnityEngine;

public class MoveControlsState : IStateControls
{
    public static event Action<RaycastHit, bool> clickToMoveEvent;

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
        clickToMoveEvent?.Invoke(raycastHit, false);
    }
}