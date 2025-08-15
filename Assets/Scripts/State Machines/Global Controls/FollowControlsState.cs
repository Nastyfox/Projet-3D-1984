using System;
using UnityEngine;

[Serializable]

public class FollowControlsState : StateControls
{
    protected override void OnEntry()
    {
        cameraManager.ChangeActiveCamera();
    }

    protected override void OnMouseClick(RaycastHit raycastHit)
    {
        controlsStateController.ChangeState(controlsStateController.globalControlsState);

    }
}