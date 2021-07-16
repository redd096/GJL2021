using System.Collections.Generic;
using UnityEngine;
using redd096;

public class LoadRandom : MonoBehaviour
{
    [Header("Levels")]
    [SerializeField] bool loadRandomLevel = true;
    [CanShow("loadRandomLevel")] [SerializeField] bool destroyOldLevel = true;
    [CanShow("loadRandomLevel")] [SerializeField] GameObject[] levels = default;
    [CanShow("loadRandomLevel")] [SerializeField] bool resetLevelsListWhenFinished = false;
    [CanShow("resetLevelsListWhenFinished", "loadRandomLevel", NotOnlyFirst = true)] [SerializeField] GameObject lastLevel = default;

    [Header("Sprite")]
    [SerializeField] bool loadRandomSprite = false;
    [CanShow("loadRandomSprite")] [SerializeField] SpriteRenderer spriteRenderer = default;
    [CanShow("loadRandomSprite")] [SerializeField] Sprite[] sprites = default;

    void Start()
    {
        //load random level
        if (loadRandomLevel)
            LoadRandomLevel();

        //load random sprite
        if (loadRandomSprite)
            LoadRandomSprite();
    }

    void LoadRandomLevel()
    {
        //destroy old level
        if (destroyOldLevel)
            DestroyOldLevel();

        //instantiate random level
        InstantiateRandomLevel();
    }

    void LoadRandomSprite()
    {
        //set random sprite to sprite renderer
        if (spriteRenderer)
            spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
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
