using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    public UnityEvent<RaycastHit> onClickCameraEvent;

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

    public void OnMouseClick(InputAction.CallbackContext context)
    {
        if (!context.canceled)
            return;

        RaycastHit rayHit;

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out rayHit))
        {
            if (rayHit.transform != null)
            {
                onClickCameraEvent?.Invoke(rayHit);
                if(rayHit.transform.gameObject.TryGetComponent(out ControlOptions controlOptions))
                {
                    controlOptions.DisplayControlOptions();
                }
            }
        }
    }
}
