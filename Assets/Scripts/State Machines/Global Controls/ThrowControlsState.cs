using System;
using UnityEngine;

[Serializable]
public class ThrowControlsState : StateControls
{
    private Vector3 mousePosition;

    [SerializeField] private ThrowController throwController;


    protected override void OnEntry()
    {
        throwController.ActivateThrowTrajectory();
    }

    protected override void OnUpdate()
    {
        mousePosition = throwController.GetMousePosition();
        throwController.SimulateTrajectory();
        characterController.FollowMoveCursor(mousePosition);
    }

    protected override void OnMouseClick(RaycastHit raycast)
    {
        throwController.LaunchObject();
        controlOptions.ChangeStateGrabbedObjectOptions();
        controlsStateController.ChangeState(controlsStateController.globalControlsState);
    }
}