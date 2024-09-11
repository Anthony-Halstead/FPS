using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
/// <summary>
/// represents an abstract factory. 
/// objects deriving from type Spawnable can be instantiated from this.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Factory<T> : IFactory<T> where T : Spawnable
{
    Spawnable[] data;

    public Factory(Spawnable[] data)
    {
        this.data = data;
    }
    /// <summary>
    /// represents the creation logic for spawning an instance of a spawnable. 
    /// Later on this should include a strategy set for which objects are selected to be instantiated. 
    /// </summary>
    /// <param name="spawnPoint"></param>
    /// <returns></returns>
    public T Create(Transform spawnPoint)
    {
        Spawnable entitydata = data[Random.Range(0, data.Length)];
        GameObject instance = GameObject.Instantiate(entitydata.gameObject,spawnPoint.position,spawnPoint.rotation);
        return instance.GetComponent<T>();
    }
}
