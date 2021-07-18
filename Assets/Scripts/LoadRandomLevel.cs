using System.Collections.Generic;
using UnityEngine;
using redd096;

#region save class

[System.Serializable]
public class TutorialSaveClass
{
    public bool firstTime = true;

    public TutorialSaveClass(bool firstTime)
    {
        this.firstTime = firstTime;
    }
}

#endregion

public class LoadRandomLevel : MonoBehaviour
{
    public const string TUTORIALNAME = "First Time";

    [Header("Random Levels")]
    [SerializeField] bool destroyOldLevel = true;
    [SerializeField] GameObject[] levels = default;
    [SerializeField] bool resetLevelsListWhenFinished = false;

    [Header("Tutorial Level")]
    [SerializeField] bool saveToNotRepeatAgain = true;
    [SerializeField] GameObject tutorialLevel = default;
    [SerializeField] string shopSceneName = "Scena Negozio";

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
        //instantiate tutorial
        if(GameManager.instance.FirstRoom)
        {
            InstantiateTutorialLevel();
        }
        //after few rooms, instantiate last level
        else if (showAfterFewRooms && GameManager.instance.CurrentRoom >= numberOfRoomsBeforeLastLevel && lastLevel)
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

    void InstantiateTutorialLevel()
    {
        //be sure to have tutorial level
        bool instantiateTutorial = tutorialLevel != null;

        //but if save to not repeat again, be sure is not already saved
        if (instantiateTutorial && saveToNotRepeatAgain)
        {
            TutorialSaveClass save = SaveLoadJSON.Load<TutorialSaveClass>(TUTORIALNAME);
            if (save != null && save.firstTime == false)
            {
                instantiateTutorial = false;
            }
        }

        //instantiate tutorial and save
        if(instantiateTutorial)
        {
            Instantiate(tutorialLevel, transform);
            SaveLoadJSON.Save(TUTORIALNAME, new TutorialSaveClass(true));
        }
        //else set is not first room and load shop scene
        else
        {
            GameManager.instance.FirstRoom = false;
            SceneLoader.instance.LoadScene(shopSceneName);
        }
    }

    void InstantiateRandomLevel()
    {
        //select only levels not already seen
        List<GameObject> possibleLevels = GetPossibleLevels();

        //when finished levels
        if(possibleLevels.Count <= 0)
        {
            //reset list
            if (resetLevelsListWhenFinished)
            {
                possibleLevels = new List<GameObject>(levels);
            }
            //or instantiate last level if there is one
            else
            {
                if(lastLevel)
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
