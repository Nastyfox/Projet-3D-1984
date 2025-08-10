using System.Collections.Generic;
using UnityEngine;

public class ControlOptions : MonoBehaviour
{
    [SerializeField] private List<GameObject> controlOptions;
    [SerializeField] private Canvas canvasControlOptions;
    [SerializeField] private Camera mainCamera;

    private void OnEnable()
    {
        InteractionManager.clickOnElementEvent += DisplayControlOptions;
    }

    private void OnDisable()
    {
        InteractionManager.clickOnElementEvent -= DisplayControlOptions;
    }

    private void LateUpdate()
    {
        canvasControlOptions.transform.LookAt(canvasControlOptions.transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
    }

    public void DisplayControlOptions(RaycastHit rayHit)
    {
        if (rayHit.transform != this.transform)
            return;

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
