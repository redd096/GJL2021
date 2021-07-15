using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadRandom : MonoBehaviour
{
    [Header("Levels")]
    [SerializeField] bool destroyOldLevel = true;
    [SerializeField] GameObject[] levels = default;

    void Start()
    {
        //destroy old levels
        if (destroyOldLevel)
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);
        }

        //instantiate random level
        Instantiate(levels[Random.Range(0, levels.Length)], transform);
    }
}
