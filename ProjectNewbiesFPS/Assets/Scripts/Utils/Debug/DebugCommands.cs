using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DebugMenu
{
    [RequireComponent(typeof(DebugMenu))]
    public class DebugCommands : MonoBehaviour
    {
        // Maybe have a scriptable object that contains all enemies, objects, pickups, etc
        // So we can easily just get everything from one place and not have to update the
        // debug menu directly. Whenever a new object is made and tested, it gets added to
        // the SO.

        [SerializeField] internal AssetList _assetList;
        
        [SerializeField] internal List<string> commands;

        [SerializeField] private DebugMenu menu;

        public void Awake()
        {
            menu = GetComponent<DebugMenu>();
        }

        public void SpawnEnemy(string enemyName, int enemyCount)
        {
            Vector3 spawnPosition = new Vector3(0,0,Camera.main.transform.position.z + 10);
            
            foreach (GameObject enemy in _assetList.enemies)
            {
                if (enemy.name == enemyName)
                {
                    menu.debugText.text += "     " + "Spawning " + enemyName + " prefab" + "\n";

                    for (int i = 0; i < enemyCount; i++)
                    {
                        Vector3 spawnOffset = new Vector3(UnityEngine.Random.Range(-2f, 2f), 0f, UnityEngine.Random.Range(-2f, 2f));
                        Vector3 spawnPoint = spawnPosition + spawnOffset;

                        GameObject newEnemy = Instantiate(enemy, spawnPoint, Quaternion.identity);
                        
                        spawnPosition += new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));
                    }

                    return;
                }
                
            }
            
            menu.debugText.text += "     " + "Enemy with name " + enemyName + " not found!" + "\n";
        }

        public void SpawnPickup(string pickupName, int pickupCount)
        {
            Vector3 spawnPosition = new Vector3(0, 2, Camera.main.transform.position.z + 10);

            foreach (GameObject pickup in _assetList.pickups)
            {
                if (pickup.name == pickupName)
                {

                    for (int i = 0; i < pickupCount; i++)
                    {
                        GameObject newPickup = Instantiate(pickup, spawnPosition, Quaternion.identity);
                    }

                    return;
                }
            }
        }

        public void Help()
        {
            menu.debugText.text = "Available Commands: \n";

            foreach (string command in commands)
            {
                menu.debugText.text = "command \n";
            }
        }

        public void Clear()
        {
            menu.debugText.text = "";
        }

        public void KillAll()
        {
            GameObject[] enemiesInScene = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemiesInScene)
            {
                enemy.GetComponent<AIController>().TakeDamage(1000, Vector3.zero);
            }
        }
        
    }
    
}
