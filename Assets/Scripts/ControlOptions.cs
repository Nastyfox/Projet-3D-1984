using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ControlOptions : MonoBehaviour
{
    [SerializeField] private List<GameObject> controlOptions;
    [SerializeField] private List<GameObject> grabbedObjectControlOptions;
    [SerializeField] private Canvas canvasControlOptions;
    [SerializeField] private Camera mainCamera;

    private void LateUpdate()
    {
        canvasControlOptions.transform.LookAt(canvasControlOptions.transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
    }

    public void DisplayControlOptions(RaycastHit raycastHit)
    {
        if (raycastHit.transform != this.transform)
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

    public void HideControlOptions()
    {
        foreach (GameObject controlOption in controlOptions)
        {
            if (controlOption != null)
            {
                controlOption.SetActive(false);
            }
        }
    }

    public void ChangeStateGrabbedObjectOptions()
    {
        foreach (GameObject grabbedObjectControlOption in grabbedObjectControlOptions)
        {
            if (grabbedObjectControlOption != null)
            {
                grabbedObjectControlOption.SetActive(!grabbedObjectControlOption.activeSelf); //Invert the active state of the control options for grabbed objects
            }
        }
    }

    private UnityEvent test;

    public void SetListenersControlsButtons(List<UnityEvent> buttonsEvents)
    {
        int indexListener = 0;

        foreach (GameObject controlOption in controlOptions)
        {
            foreach(Transform child in controlOption.transform)
            {
                if (child.GetComponent<Button>() != null)
                {
                    Button button = child.GetComponent<Button>();
                    button.onClick.RemoveAllListeners();
                    UnityAction action = ConvertEventToAction(buttonsEvents[indexListener])[0];
                    button.onClick.AddListener(action);
                    indexListener++;
                }
            }
        }
    }

    private List<UnityAction> ConvertEventToAction(UnityEvent unityEvent)
    {
        int actionCount = unityEvent.GetPersistentEventCount();
        List<UnityAction> result = new List<UnityAction>();

        for (int i = 0; i < actionCount; i++)
        {
            UnityEngine.Object target = unityEvent.GetPersistentTarget(i);
            string methodName = unityEvent.GetPersistentMethodName(i);
            Type targetType = target.GetType();
            UnityAction ac = (UnityAction)Delegate.CreateDelegate(typeof(UnityAction), target, methodName);
            result.Add(ac);
        }

        return result;
    }
}
