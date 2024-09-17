using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class KeySpawnManager : MonoBehaviour
{
    [SerializeField] GameObject[] keySpawnPoints;
    //[SerializeField] GameObject spawnPos;
    [SerializeField] GameObject key;
   
    [SerializeField] int randomSpawnPoints;
    [SerializeField] int MaxSpawnPoints;
    

    // Start is called before the first frame update
    void Start()
    {

        randomSpawnPoints = Random.Range(0, MaxSpawnPoints);

        Instantiate(key, keySpawnPoints[randomSpawnPoints].transform.position + new Vector3(0, 1, 4), keySpawnPoints[randomSpawnPoints].transform.localRotation);

        //Instantiate(key, spawnPos.transform.position + new Vector3(0, 1, 4), spawnPos.transform.localRotation);
    }
   

}

    

