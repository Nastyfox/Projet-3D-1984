using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    [Header("Camera parameters")]
    private List<CinemachineVirtualCamera> cinemachineCameras = new List<CinemachineVirtualCamera>();
    [SerializeField] private CinemachineVirtualCamera activeCinemachineCamera;
    [SerializeField] private Camera mainCamera;
    private CinemachineTransposer cinemachineFollow;
    private Dictionary<GameObject, CinemachineVirtualCamera> targetCameras = new Dictionary<GameObject, CinemachineVirtualCamera>();
    [Range(1f, 5f)]
    [SerializeField] private float transitionSpeed;

    [Header("World elements")]
    [SerializeField] private GameObject worldGameObject;

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
    [SerializeField] private GameObject targetLookAt;
    [SerializeField] private GameObject targetFollow;
    private GameObject target;
    [Range(20f, 200f)]
    [SerializeField] private float rotateSpeed;
    private float rotateXAxis;


    private void Awake()
    {
        foreach (CinemachineVirtualCamera virtualCamera in FindObjectsByType<CinemachineVirtualCamera>(FindObjectsSortMode.None))
        {
            targetCameras.Add(virtualCamera.transform.parent.gameObject, virtualCamera);
            cinemachineCameras.Add(virtualCamera);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cinemachineFollow = activeCinemachineCamera.GetCinemachineComponent<CinemachineTransposer>();
        followOffset = cinemachineFollow.m_FollowOffset;
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

        cinemachineFollow.m_FollowOffset = Vector3.Lerp(cinemachineFollow.m_FollowOffset, followOffset, zoomSpeed * Time.deltaTime);
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        rotateXAxis = context.ReadValue<Vector2>().x;

        this.transform.RotateAround(targetLookAt.transform.position, Vector3.up * Mathf.Sign(rotateXAxis), rotateSpeed * Time.deltaTime);
    }

    public void ChangeCameraFocus(RaycastHit rayHit)
    {
        if (rayHit.transform.tag != targetLookAt.transform.parent.transform.tag)
        {
            if (rayHit.transform.tag == "NPC")
            {
                target = rayHit.collider.gameObject;
                targetLookAt.transform.SetParent(target.transform);
                
            }
            else
            {
                target = worldGameObject;
                targetLookAt.transform.position = rayHit.point;
                targetLookAt.transform.SetParent(target.transform);
            }

            StartCoroutine(CameraTransitionCoroutine(targetLookAt.transform.localPosition));
        }
    }

    public void ChangeActiveCamera()
    {
        targetFollow.transform.forward = target.transform.forward;

        CinemachineVirtualCamera targetCamera = targetCameras.GetValueOrDefault(targetFollow);
        foreach (CinemachineVirtualCamera virtualCamera in cinemachineCameras)
        {
            virtualCamera.enabled = virtualCamera == targetCamera;
            activeCinemachineCamera = targetCamera;
            cinemachineFollow = activeCinemachineCamera.GetCinemachineComponent<CinemachineTransposer>();
            followOffset = cinemachineFollow.m_FollowOffset;
        }
    }

    IEnumerator CameraTransitionCoroutine(Vector3 startPosition)
    {
        float elapsedTime = 0f;

        while (elapsedTime < transitionSpeed)
        {
            this.transform.localPosition = Vector3.Lerp(startPosition, Vector3.zero, elapsedTime / transitionSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        this.transform.localPosition = Vector3.zero;
    }
}