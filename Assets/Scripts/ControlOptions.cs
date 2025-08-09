using System.Collections.Generic;
using UnityEngine;

public class ControlOptions : MonoBehaviour
{
    [SerializeField] private List<GameObject> controlOptions;
    [SerializeField] private Canvas canvasControlOptions;
    [SerializeField] private Camera mainCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        canvasControlOptions.transform.LookAt(canvasControlOptions.transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
    }

    public void DisplayControlOptions()
    {
        foreach (GameObject controlOption in controlOptions)
        {
            if (controlOption != null)
            {
                canvasControlOptions.transform.SetParent(this.transform);
                canvasControlOptions.transform.localPosition = Vector3.zero;
                controlOption.SetActive(true);
            }
        }
    }
}
