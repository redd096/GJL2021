﻿using System.Collections.Generic;
using UnityEngine;
using redd096;

public class BotolaInteractable : MonoBehaviour, IInteractable
{
    [Header("Rules to Open")]
    [SerializeField] bool checkNoEnemiesInScene = true;
    [SerializeField] bool checkPlayerHasWeapon = true;
    [SerializeField] List<RuntimeAnimatorController> enemiesToIgnore = new List<RuntimeAnimatorController>();

    [Header("On Interact")]
    [SerializeField] bool updateVisitedRooms = true;
    [SerializeField] bool destroyWeapon = true;
    [SerializeField] float timeBeforeLoadNewScene = 1;
    [SerializeField] string sceneToLoad = "Scena Game";
    [SerializeField] bool resetProgress = false;

    [Header("TUTORIAL")]
    [SerializeField] bool finishTutorial = false;

    [Header("DEBUG")]
    [ReadOnly] [SerializeField] bool isOpen;

    public System.Action<bool> onCloseOpen { get; set; }
    public System.Action onInteract { get; set; }

    void FixedUpdate()
    {
        //if change state (can open or not)
        if (CheckCanOpen() != isOpen)
            OpenCloseBotola();
    }

    #region open or close

    bool CheckCanOpen()
    {
        bool canOpen = true;

        //check there are not enemies in scene
        if (canOpen && checkNoEnemiesInScene)
        {
            //foreach enemy in scene
            Enemy[] enemiesInScene = FindObjectsOfType<Enemy>();
            foreach(Enemy enemyInScene in enemiesInScene)
            {
                //if there is an enemy, and is not an enemy to ignore, then can't open
                if(enemiesToIgnore.Contains(enemyInScene.GetComponent<Animator>().runtimeAnimatorController) == false)
                {
                    canOpen = false;
                    break;
                }
            }
        }

        //check player has weapon equipped
        if (canOpen && checkPlayerHasWeapon)
            canOpen = GameManager.instance.levelManager.Players[0].CurrentWeapon != null;

        return canOpen;
    }

    void OpenCloseBotola()
    {
        //open or close
        isOpen = !isOpen;

        //call event
        onCloseOpen?.Invoke(isOpen);
    }

    #endregion

    #region interact

    /// <summary>
    /// Called from player when interact
    /// </summary>
    /// <param name="player"></param>
    public void Interact(Player player)
    {
        if (isOpen == false)
            return;

        //stop this script
        enabled = false;    //stop check open/close
        isOpen = false;     //can't interact anymore

        //destroy weapon
        if (destroyWeapon)
            player.DropWeapon();

        //deactive player
        player.GetComponent<Animator>().enabled = false;

        //update current room
        if (updateVisitedRooms)
            GameManager.instance.CurrentRoom++;

        //if finish tutorial, save it
        if(finishTutorial)
            SaveLoadJSON.Save(LoadRandomLevel.TUTORIALNAME, new TutorialSaveClass(false));

        //reset progress
        if (resetProgress)
            GameManager.instance.ResetAll();

        //call event
        onInteract?.Invoke();

        //load scene after few seconds
        Invoke("LoadScene", timeBeforeLoadNewScene);
    }

    void LoadScene()
    {
        //change scene
        SceneLoader.instance.LoadScene(sceneToLoad);
    }

    /// <summary>
    /// Called to re-enable interactable (for example when close vendor)
    /// </summary>
    public void ReactiveInteractable()
    {
        enabled = true;
    }

    #endregion
}
