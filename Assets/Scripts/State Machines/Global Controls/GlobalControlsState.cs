using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class GlobalControlsState : StateControls
{
    [SerializeField] private List<UnityEvent> buttonsEvents;

    protected override void OnExit()
    {
        controlOptions.HideControlOptions();
    }

    protected override void OnMouseClick(RaycastHit raycastHit)
    {
        controlOptions.SetListenersControlsButtons(buttonsEvents);
        cameraManager.ChangeCameraFocus(raycastHit);
        controlOptions.DisplayControlOptions(raycastHit);
    }
}