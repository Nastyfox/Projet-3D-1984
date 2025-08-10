using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    private static PlayerInput playerInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("GlobalControl");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void SwitchActionMap(string newActionMap)
    {
        if (playerInput != null)
        {
            playerInput.SwitchCurrentActionMap(newActionMap);
        }
    }
}
