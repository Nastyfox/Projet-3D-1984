using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Camera parameters")]
    [SerializeField] private CinemachineVirtualCamera[] virtualCameras;
    private CinemachineVirtualCamera activeVirtualCamera;
    [SerializeField] private Camera mainCamera;
    private CinemachineTransposer virtualCameraTransposer;

    [Header("Zoom parameters")]
    private float zoomScale;
    private Vector3 followOffset = new Vector3();
    [Range(1f, 3f)]
    [SerializeField] private float minZoom;
    [Range(8f, 10f)]
    [SerializeField] private float maxZoom;
    [Range(1f, 5f)]
    [SerializeField] private float zoomSpeed;

    [Header("Rotate parameters")]
    [SerializeField] private GameObject target;
    [Range(20f, 200f)]
    [SerializeField] private float rotateSpeed;
    private float rotateXAxis;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        activeVirtualCamera = virtualCameras[0];
        virtualCameraTransposer = activeVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        followOffset = virtualCameraTransposer.m_FollowOffset;
    }

    // Update is called once per frame
    void Update()
    {
            
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        Vector3 zoomDirection = followOffset.normalized;

        zoomScale = context.ReadValue<float>();

        followOffset -= zoomDirection * Mathf.Sign(zoomScale);
        if (followOffset.magnitude < minZoom) {
            followOffset = minZoom * zoomDirection;
        }
        if (followOffset.magnitude > maxZoom)
        {
            followOffset = maxZoom * zoomDirection;
        }

        virtualCameraTransposer.m_FollowOffset = Vector3.Lerp(virtualCameraTransposer.m_FollowOffset, followOffset, zoomSpeed * Time.deltaTime);
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        rotateXAxis = context.ReadValue<Vector2>().x;

        this.transform.RotateAround(target.transform.position, Vector3.up * Mathf.Sign(rotateXAxis), rotateSpeed * Time.deltaTime);
    }

    public void OnMouseClick(InputAction.CallbackContext context)
    {
        if (!context.canceled)
            return;

        RaycastHit rayHit;

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out rayHit, 100f))
        {
            if (rayHit.transform != null)
            {
                Debug.Log(rayHit.collider.gameObject.name);

                target = rayHit.collider.gameObject;
                CinemachineVirtualCamera targetCamera = target.GetComponentInChildren<CinemachineVirtualCamera>();
                Debug.Log(targetCamera.gameObject.name);
                foreach (CinemachineVirtualCamera virtualCamera in virtualCameras)
                {
                    virtualCamera.enabled = virtualCamera == targetCamera;
                }
            }
        }
    }
}
