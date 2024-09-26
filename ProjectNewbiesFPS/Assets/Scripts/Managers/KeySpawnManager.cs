using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class KeySpawnManager : MonoBehaviour
{
    [Header("Spawn Stats")]
    [SerializeField] GameObject[] keySpawnPoints;
    //[SerializeField] GameObject spawnPos;
    [SerializeField] GameObject key;
    
   
    [SerializeField] int randomSpawnPoints;
    [SerializeField] int MaxSpawnPoints;
    
    

    // Start is called before the first frame update
    void Awake()
    {
        
        randomSpawnPoints = Random.Range(0, MaxSpawnPoints);

        Instantiate(key, keySpawnPoints[randomSpawnPoints].transform.position + new Vector3(0, 1, 0), keySpawnPoints[randomSpawnPoints].transform.localRotation);

        

    }

   

}

    

