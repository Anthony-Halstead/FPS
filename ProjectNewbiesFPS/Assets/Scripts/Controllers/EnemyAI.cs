using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] private Renderer model;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform shootPos;
    [SerializeField] private Transform headPos;
    
    [SerializeField] private int HP;

    [SerializeField] private GameObject bullet;
    [SerializeField] private float shootRate;

    private Color colorOriginal;

    bool isShooting;
    
    // Start is called before the first frame update
    void Start()
    {
        model = GetComponentInChildren<Renderer>();
        agent = GetComponent<NavMeshAgent>();
        colorOriginal = model.material.color;
        GameManager.instance.WinGame(1);
    }

    // Update is called once per frame
    void Update()
    {
        //agent.SetDestination(GameManager.instance.player.transform.position);

        if (!isShooting)
        {
            StartCoroutine(shoot());
        }
    }

    IEnumerator shoot()
    {
        isShooting = true;

        Instantiate(bullet, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(shootRate);
        
        isShooting = false;
    }
    
    public void takeDamage(int amount)
    {
        StartCoroutine(flashColor());
        
        HP -= amount;
        if (HP <= 0)
        {
            GameManager.instance.WinGame(-1);
            Destroy(gameObject);
        }
    }

    IEnumerator flashColor()
    {
        model.material.color = Color.red;
        
        yield return new WaitForSeconds(0.1f);
        
        model.material.color = colorOriginal;
    }
}
