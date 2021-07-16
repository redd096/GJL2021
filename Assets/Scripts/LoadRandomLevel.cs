using System.Collections.Generic;
using UnityEngine;
using redd096;

public class LoadRandomLevel : MonoBehaviour
{
    [Header("Random Levels")]
    [SerializeField] bool destroyOldLevel = true;
    [SerializeField] GameObject[] levels = default;
    [SerializeField] bool resetLevelsListWhenFinished = false;
    [CanShow("resetLevelsListWhenFinished", NOT = true)] [SerializeField] GameObject lastLevel = default;

    void Start()
    {
        //destroy old level
        if (destroyOldLevel)
            DestroyOldLevel();

        //instantiate random level
        InstantiateRandomLevel();
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
