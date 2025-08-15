using System;
using UnityEngine;

public class ThrowControlsState : StateControls
{
    private Vector3 mousePosition;

    protected override void OnEntry()
    {
        controlsStateController.throwController.ActivateThrowTrajectory();
    }

    protected override void OnUpdate()
    {
        mousePosition = controlsStateController.throwController.GetMousePosition();
        controlsStateController.throwController.SimulateTrajectory();
        controlsStateController.characterController.FollowMoveCursor(mousePosition);
    }

    protected override void OnMouseClick(RaycastHit raycast)
    {
        controlsStateController.throwController.LaunchObject();
        controlsStateController.controlOptions.ChangeStateGrabbedObjectOptions();
        controlsStateController.ChangeState(controlsStateController.globalControlsState);
    }
}