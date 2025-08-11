using UnityEngine;

public class MoveOption : MonoBehaviour
{
    [SerializeField] private MeshRenderer moveCursorRenderer;
    private bool isCursorVisible = false;

    [SerializeField] private ControlsStateController controlsStateController;

    private void OnEnable()
    {
        MoveControlsState.clickToMoveEvent += DeactivateMoveCursor;
    }

    private void OnDisable()
    {
        MoveControlsState.clickToMoveEvent -= DeactivateMoveCursor;
    }

    void Update()
    {
        if(isCursorVisible)
        {
            MoveCursorOnMousePosition();
        }
    }

    public void ActivateMoveCursor()
    {
        MoveCursorOnMousePosition();
        isCursorVisible = true;
        moveCursorRenderer.enabled = isCursorVisible; // Show the cursor
        controlsStateController.ChangeState(controlsStateController.moveControlsState); // Switch to MoveControlsState
    }

    public void DeactivateMoveCursor(RaycastHit raycastHit, bool grab)
    {
        isCursorVisible = false;
        moveCursorRenderer.enabled = isCursorVisible; // Hide the cursor
        controlsStateController.ChangeState(controlsStateController.globalControlsState); // Switch to MoveControlsState
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
                transform.position = hit.point;
            }
        }
    }
}
