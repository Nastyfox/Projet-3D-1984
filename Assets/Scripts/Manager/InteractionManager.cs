using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InteractionManager : MonoBehaviour
{
    public static event Action<RaycastHit> clickOnElementEvent;
    public static event Action<Vector3> clickToMoveEvent;
    [SerializeField] private GraphicRaycaster graphicRaycaster;
    [SerializeField] private EventSystem eventSystem;

    [Header("Camera parameters")]
    [SerializeField] private Camera mainCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnGlobalMouseClick(InputAction.CallbackContext context)
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
            RaycastHit rayHit;

            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out rayHit))
            {
                if (rayHit.transform != null)
                {
                    clickOnElementEvent?.Invoke(rayHit);
                }
            }
        }
    }

    public void OnMoveMouseClick(InputAction.CallbackContext context)
    {
        if (!context.canceled)
            return;

        RaycastHit rayHit;

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out rayHit))
        {
            if (rayHit.transform != null)
            {
                clickToMoveEvent?.Invoke(rayHit.point);
            }
        }
    }
}
