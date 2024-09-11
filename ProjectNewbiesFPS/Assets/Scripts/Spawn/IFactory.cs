using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Abstract factory class representing the factory interface for all created factories that derive from spawnable
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IFactory<T> where T : Spawnable
{
    T Create(Transform spawnPoint);
}
