using System;
using UnityEngine;

[Serializable]

public class FollowControlsState : CharacterStateControls
{
    protected override void OnEntry()
    {
        cameraManager.ChangeActiveCamera();
    }

    protected override void OnMouseClick(RaycastHit raycastHit)
    {
        cameraManager.ChangeCameraFocus(raycastHit);
        cameraManager.ChangeActiveCamera();
        controlsStateController.ChangeState(controlsStateController.globalControlsState);
    }

    protected override void OnScroll(float scrollValue)
    {
        // No specific action for scroll in follow state
    }

    protected override void OnMouseMoveMiddleButtonPressed(float axisValue)
    {
        // No specific action for middle mouse button in follow state
    }
}