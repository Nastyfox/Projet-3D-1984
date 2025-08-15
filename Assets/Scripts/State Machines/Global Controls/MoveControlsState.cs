using System;
using UnityEngine;

[Serializable]

public class MoveControlsState : StateControls
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
        CharacterController.destinationReached -= EndMovement;
    }

    protected override void OnMouseClick(RaycastHit raycastHit)
    {
        CharacterController.destinationReached += EndMovement;
        characterController.MoveToPosition(raycastHit);
        moveCursorRenderer.enabled = false;
    }

    private void MoveCursorOnMousePosition()
    {
        //Display move cursor on ground where mouse is pointing
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            // Assuming the ground is tagged as "Ground"
            if (hit.transform.CompareTag("Ground"))
            {
                // Set the position of the move cursor to the hit point
                moveCursor.transform.position = hit.point;
            }
        }
    }

    private void EndMovement()
    {
        controlsStateController.ChangeState(controlsStateController.globalControlsState);
    }
}