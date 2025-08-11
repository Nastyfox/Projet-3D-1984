using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(ControlsStateController))]
public class InteractionManager : MonoBehaviour
{
    [SerializeField] private GraphicRaycaster graphicRaycaster;
    [SerializeField] private EventSystem eventSystem;

    [Header("Camera parameters")]
    [SerializeField] private Camera mainCamera;

    [SerializeField] private ControlsStateController controlsStateController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMouseClick(InputAction.CallbackContext context)
    {
        if (!context.canceled)
            return;

        // Check if there are any UI elements underneath the current mouse position
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerEventData, results);

        // "results" stores a list of all the objects hit by the graphic raycast. If the number of results is zero, you know you're in the clear and there are no UI elements underneath the mouse.
        if (results.Count == 0)
        {
            RaycastHit raycastHit;

            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out raycastHit))
            {
                if (raycastHit.transform != null)
                {
                    controlsStateController.OnMouseClick(raycastHit);
                }
            }
        }
    }
}
