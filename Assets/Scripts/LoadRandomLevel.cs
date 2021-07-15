using System.Collections.Generic;
using UnityEngine;
using redd096;

public class LoadRandomLevel : MonoBehaviour
{
    [Header("Levels")]
    [SerializeField] bool destroyOldLevel = true;
    [SerializeField] GameObject[] levels = default;

    void Start()
    {
        //destroy old levels
        DestroyOldLevel();

        //instantiate random level
        InstantiateRandomLevel();
    }

    void DestroyOldLevel()
    {
        //destroy old levels
        if (destroyOldLevel)
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);
        }
    }

    void InstantiateRandomLevel()
    {
        //select only levels not already seen
        List<GameObject> possibleLevels = new List<GameObject>();
        foreach(GameObject level in levels)
        {
            if (GameManager.instance.LevelsAlreadySeen.Contains(level) == false)
                possibleLevels.Add(level);
        }

        //instantiate random level
        int random = Random.Range(0, possibleLevels.Count);
        Instantiate(possibleLevels[random], transform);

        //save in already seen
        GameManager.instance.LevelsAlreadySeen.Add(possibleLevels[random]);
    }
}
