using System;
using UnityEngine;

public class FollowControlsState : StateControls
{
    protected override void OnEntry()
    {
        controlsStateController.cameraManager.ChangeActiveCamera();
    }

    protected override void OnMouseClick(RaycastHit raycastHit)
    {
        controlsStateController.ChangeState(controlsStateController.globalControlsState);

    }
}