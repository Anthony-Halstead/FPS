using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{

    [SerializeField] int HP;
    [SerializeField] Renderer model;
   // [SerializeField] NavMeshAgent agent;
    Color colorOrig;

    // Start is called before the first frame update
    void Start()
    {
        colorOrig = model.material.color;
        GameManager.instance.WinGame(1);
    }

    // Update is called once per frame
    void Update()
    {
       //agent.SetDestination(GameManager.instance.player.transform.position);
    }

    public void takeDamage(int amount)
    {
        
        HP -= amount;
        GameManager.instance.enemyHealthBar.fillAmount = HP / 3;
        StartCoroutine(flashColor());

        if (HP <= 0)
        {
            GameManager.instance.WinGame(-1);
            GameManager.instance.playerScript.money += 5;
            GameManager.instance.moneyText.text = "$" + GameManager.instance.playerScript.money;
            Destroy(gameObject);
        }
    }

    IEnumerator flashColor()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }
}
