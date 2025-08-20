using System;
using UnityEngine;

[Serializable]
public class ThrowControlsState : CharacterStateControls
{
    private Vector3 mousePosition;
    
    private bool controlPressed = false;

    [SerializeField] private ThrowController throwController;


    protected override void OnEntry()
    {
        throwController.ActivateThrowTrajectory();
    }

    protected override void OnUpdate()
    {
        mousePosition = throwController.GetMousePosition(mainCamera);
        characterController.FollowMoveCursor(mousePosition);
        throwController.SimulateTrajectory(mousePosition);
    }

    protected override void OnMouseClick(RaycastHit raycast)
    {
        throwController.LaunchObject(mousePosition);
        controlOptions.ChangeStateGrabbedObjectOptions();
        controlsStateController.ChangeState(controlsStateController.globalControlsState);
    }

    protected override void OnScroll(float scrollValue)
    {
        if (controlPressed)
        {
            throwController.ChangeThrowHeight(scrollValue);
        }
        else
        {
            cameraManager.OnZoom(scrollValue);
        }
    }

    protected override void OnControlPressed(bool isPressed)
    {
        controlPressed = isPressed;
    }
}