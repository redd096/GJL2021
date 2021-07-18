using System.Collections.Generic;
using UnityEngine;
using redd096;

public class LoadRandomLevel : MonoBehaviour
{
    public const string TUTORIALNAME = "First Time";

    [Header("Random Levels")]
    [SerializeField] bool destroyOldLevel = true;
    [SerializeField] GameObject[] levels = default;
    [SerializeField] bool resetLevelsListWhenFinished = false;

    [Header("Last Level")]
    [SerializeField] bool showAfterFewRooms = true;
    [SerializeField] int numberOfRoomsBeforeLastLevel = 4;
    [CanShow("resetLevelsListWhenFinished", "showAfterFewRooms", checkAND = false)] [SerializeField] GameObject lastLevel = default;

    void Start()
    {
        //destroy old level
        if (destroyOldLevel)
            DestroyOldLevel();

        LoadLevel();

        //update grid
        AStar.instance.UpdateGrid();
    }

    void LoadLevel()
    {
        //after few rooms, instantiate last level
        if (showAfterFewRooms && GameManager.instance.CurrentRoom >= numberOfRoomsBeforeLastLevel && lastLevel)
        {
            InstantiateLastlevel();
        }
        //else instantiate random level
        else
        {
            InstantiateRandomLevel();
        }
    }

    #region load random level

    void DestroyOldLevel()
    {
        //destroy old levels
        foreach (Transform child in transform)
            Destroy(child.gameObject);
    }

    void InstantiateRandomLevel()
    {
        //select only levels not already seen
        List<GameObject> possibleLevels = GetPossibleLevels();

        //when finished levels
        if(possibleLevels.Count <= 0)
        {
            //reset list (if not setted last level, reset too)
            if (resetLevelsListWhenFinished || lastLevel == null)
            {
                possibleLevels = new List<GameObject>(levels);
            }
            //or instantiate last level
            else
            {
                Instantiate(lastLevel, transform);

                return;
            }
        }

        //instantiate random level
        int random = Random.Range(0, possibleLevels.Count);
        Instantiate(possibleLevels[random], transform);

        //save in already seen
        GameManager.instance.LevelsAlreadySeen.Add(possibleLevels[random]);
    }

    void InstantiateLastlevel()
    {
        //instantiate last level
        Instantiate(lastLevel, transform);
    }

    List<GameObject> GetPossibleLevels()
    {
        //select only levels not already seen
        List<GameObject> possibleLevels = new List<GameObject>();
        foreach (GameObject level in levels)
        {
            if (GameManager.instance.LevelsAlreadySeen.Contains(level) == false)
                possibleLevels.Add(level);
        }

        return possibleLevels;
    }

    #endregion
}
