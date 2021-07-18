using UnityEngine;
using redd096;

public class ReplaceGameObjectMouseOrGamepad : MonoBehaviour
{
    [Header("change object if use mouse or gamepad")]
    [SerializeField] GameObject objectMouse = default;
    [SerializeField] GameObject objectGamepad = default;

    bool usingMouse;

    void Start()
    {
        //set gameObject
        ReplaceGameObjects();
    }

    void Update()
    {
        //if change device, replace gameObjects
        if (GameManager.instance.levelManager.Players.Count > 0)
        {
            if(InputRedd096.IsCurrentControlScheme(GameManager.instance.levelManager.Players[0].playerInput, "KeyboardAndMouse") != usingMouse)
            {
                ReplaceGameObjects();
            }
        }
    }

    void ReplaceGameObjects()
    {
        //set if using mouse or gamepad
        if(GameManager.instance.levelManager.Players.Count > 0)
        {
            usingMouse = InputRedd096.IsCurrentControlScheme(GameManager.instance.levelManager.Players[0].playerInput, "KeyboardAndMouse");
        }

        //active or deactive objects
        if(objectMouse)
            objectMouse.SetActive(usingMouse);
        if(objectGamepad)
            objectGamepad.SetActive(!usingMouse);
    }
}
