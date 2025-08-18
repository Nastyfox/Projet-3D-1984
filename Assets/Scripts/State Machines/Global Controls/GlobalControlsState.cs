using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class GlobalControlsState : CharacterStateControls
{
    [SerializeField] private List<UnityEvent> buttonsEvents;

    protected override void OnExit()
    {
        controlOptions.HideControlOptions();
    }

    protected override void OnMouseClick(RaycastHit raycastHit)
    {
        if(raycastHit.transform != controlsStateController.transform)
        {
            controlOptions.HideControlOptions();
            cameraManager.ChangeCameraFocus(raycastHit);
            return;
        }

        controlOptions.SetListenersControlsButtons(buttonsEvents);
        cameraManager.ChangeCameraFocus(raycastHit);
        controlOptions.DisplayControlOptions(raycastHit);
    }
}