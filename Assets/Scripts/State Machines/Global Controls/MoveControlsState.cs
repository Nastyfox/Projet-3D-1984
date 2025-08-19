using System;
using UnityEngine;

[Serializable]

public class MoveControlsState : CharacterStateControls
{
    private MeshRenderer moveCursorRenderer;

    [SerializeField] private GameObject moveCursor;

    protected override void OnEntry()
    {
        moveCursorRenderer = moveCursor.GetComponent<MeshRenderer>();
        moveCursorRenderer.enabled = true;
    }

    protected override void OnUpdate()
    {
        MoveCursorOnMousePosition();
    }

    protected override void OnExit()
    {
        InteractableCharacterController.destinationReached -= EndMovement;
    }

    protected override void OnMouseClick(RaycastHit raycastHit)
    {
        InteractableCharacterController.destinationReached += EndMovement;
        characterController.MoveToPosition(raycastHit);
        moveCursorRenderer.enabled = false;
    }

    private void MoveCursorOnMousePosition()
    {
        //Display move cursor on ground where mouse is pointing
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycastHit;
        if (Physics.Raycast(ray, out raycastHit))
        {
            // Assuming the ground is tagged as "Ground"
            if (raycastHit.transform.CompareTag("Ground"))
            {
                // Set the position of the move cursor to the hit point
                moveCursor.transform.position = raycastHit.point;
            }
        }
    }

    private void EndMovement()
    {
        controlsStateController.ChangeState(controlsStateController.globalControlsState);
    }
}