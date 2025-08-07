using System.Collections.Generic;
using UnityEngine;

public class ControlOptions : MonoBehaviour
{
    [SerializeField] private List<GameObject> controlOptions;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayControlOptions()
    {
        foreach (GameObject controlOption in controlOptions)
        {
            if (controlOption != null)
            {
                controlOption.SetActive(true);
            }
        }
    }
}
