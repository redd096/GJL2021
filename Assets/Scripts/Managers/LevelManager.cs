using System.Collections.Generic;
using UnityEngine;
using redd096;

public class LevelManager : MonoBehaviour
{
    [Header("DEBUG")]
    public List<Player> Players = new List<Player>();

    /// <summary>
    /// Called when player die
    /// </summary>
    public void EndGame()
    {
        //show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //show end menu
        GameManager.instance.uiManager.EndMenu(true);
    }
}
