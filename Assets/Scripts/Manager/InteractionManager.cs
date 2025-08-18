using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InteractionManager : MonoBehaviour
{
    [SerializeField] private GraphicRaycaster graphicRaycaster;
    [SerializeField] private EventSystem eventSystem;

    [Header("Camera parameters")]
    [SerializeField] private Camera mainCamera;

    private ControllerStateControls currentControlsStateController;
    [SerializeField] CameraManager cameraManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetControlsStateController()
    {
        currentControlsStateController = null;
    }

    public void OnMouseClick(InputAction.CallbackContext context)
    {
        if (!context.canceled)
            return;

        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerEventData, results);

        if (results.Count == 0)
        {
            RaycastHit raycastHit;

            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out raycastHit))
            {
                if (raycastHit.transform != null)
                {
                    if (currentControlsStateController != null)
                    {
                        currentControlsStateController.OnMouseClick(raycastHit);
                        return;
                    }

                    else if (raycastHit.collider.gameObject.GetComponent<ControllerStateControls>())
                    {
                        currentControlsStateController = raycastHit.collider.gameObject.GetComponent<ControllerStateControls>();
                        currentControlsStateController.OnMouseClick(raycastHit);
                    }
                }
            }
        }
    }

    public void OnScroll(InputAction.CallbackContext context)
    {
        if (currentControlsStateController != null)
        {
            currentControlsStateController.OnScroll(context.ReadValue<float>());
        }

        else
        {
            cameraManager.OnZoom(context.ReadValue<float>());
        }
    }

    public void OnMouseMoveMiddleButtonPressed(InputAction.CallbackContext context)
    {
        if (currentControlsStateController != null)
        {
            currentControlsStateController.OnMouseMoveMiddleButtonPressed(context.ReadValue<Vector2>().x);
        }

        else
        {
            cameraManager.OnRotate(context.ReadValue<Vector2>().x);
        }
    }

    public void OnScrollControlPressed(InputAction.CallbackContext context)
    {
        if (currentControlsStateController != null)
        {
            currentControlsStateController.OnScrollControlPressed(context.ReadValue<float>());
        }
    }
}
