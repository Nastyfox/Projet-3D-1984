using System;
using UnityEngine;

public class MoveOption : MonoBehaviour
{
    [SerializeField] private MeshRenderer moveCursorRenderer;
    private bool isCursorVisible = false;

    private void OnEnable()
    {
        InteractionManager.clickToMoveEvent += DeactivateMoveCursor;
    }

    private void OnDisable()
    {
        InteractionManager.clickToMoveEvent -= DeactivateMoveCursor;
    }

    void Update()
    {
        if(isCursorVisible)
        {
            DisplayMoveCursor();
        }
    }

    public void ActivateMoveCursor()
    {
        isCursorVisible = true;
        moveCursorRenderer.enabled = isCursorVisible; // Show the cursor
        PlayerInputManager.SwitchActionMap("MoveControl"); // Switch to the MoveCursor action map
    }

    public void DeactivateMoveCursor(Vector3 position)
    {
        isCursorVisible = false;
        moveCursorRenderer.enabled = isCursorVisible; // Hide the cursor
        PlayerInputManager.SwitchActionMap("GlobalControl"); // Switch to the MoveCursor action map
    }

    private void DisplayMoveCursor()
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
