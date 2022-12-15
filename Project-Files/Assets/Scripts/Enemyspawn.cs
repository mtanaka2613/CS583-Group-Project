using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemyspawn : MonoBehaviour
{
    oublic GameObject enemyPrefab;
    preivate float spawnRange = 9.0f;
    // Start is called before the first frame update
    void Start()
    {
        float spawnPosX = Random.Range(-9, 9);
        float spawnPosY = Random.Range(-9, 9)

        Vector3 randomPos = new Vector3(spawnPosX, spawnPosY, 0);

        Instantiate(enemyPrefab, randomPos, enemyPrefab.Transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
