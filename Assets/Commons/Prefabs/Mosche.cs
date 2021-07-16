using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mosche : MonoBehaviour
{
    public GameObject mosca;
    public float spawnRangeVert;
    public float spawnRangeHoriz;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            Invoke("SpawnM", 0.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnM()
    {
        Instantiate(mosca, GenerateSpwnPosition(), mosca.transform.rotation);
    }

    private Vector3 GenerateSpwnPosition()
    {
        float spawnRangeY = Random.Range(-spawnRangeVert, spawnRangeVert);
        float spawnRangeX = Random.Range(-spawnRangeHoriz, spawnRangeHoriz);

        Vector3 spawnPos = new Vector3(spawnRangeX, spawnRangeY, 0);

        return spawnPos;
    }
}
