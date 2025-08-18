using System;
using UnityEngine;

[Serializable]
public class ThrowControlsState : CharacterStateControls
{
    private Vector3 mousePosition;

    [SerializeField] private ThrowController throwController;


    protected override void OnEntry()
    {
        throwController.ActivateThrowTrajectory();
    }

    protected override void OnUpdate()
    {
        mousePosition = throwController.GetMousePosition(mainCamera);
        throwController.SimulateTrajectory();
        characterController.FollowMoveCursor(mousePosition);
    }

    protected override void OnMouseClick(RaycastHit raycast)
    {
        throwController.LaunchObject();
        controlOptions.ChangeStateGrabbedObjectOptions();
        controlsStateController.ChangeState(controlsStateController.globalControlsState);
    }

    protected override void OnScroll(float scrollValue)
    {
        
    }

    protected override void OnScrollControlPressed(float scrollValue)
    {
        throwController.ChangeThrowHeight(scrollValue);
    }
}