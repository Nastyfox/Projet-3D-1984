using System;
using UnityEngine;

public class GlobalControlsState : StateControls
{
    protected override void OnExit()
    {
        controlsStateController.controlOptions.HideControlOptions();
    }

    protected override void OnMouseClick(RaycastHit raycastHit)
    {
        controlsStateController.controlOptions.SetListenersControlsButtons(controlsStateController.buttonsEvents);
        controlsStateController.cameraManager.ChangeCameraFocus(raycastHit);
        controlsStateController.controlOptions.DisplayControlOptions(raycastHit);
    }
}