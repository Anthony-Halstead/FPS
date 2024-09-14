using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PowerUps : MonoBehaviour, IPowerUps
{
    [SerializeField] bool killEnemiesUpgrade;
    [SerializeField] bool shootRateUpgrade;
    [SerializeField] bool doubleDamageUpgrade;
    [SerializeField] bool healthUpgrade;
    [SerializeField] bool magazineUpgrade;
    [SerializeField] bool refillUpgrade;

 
    public int killEnemiesUpgradeAmount;
    public float shootRateUpgradeAmount;
    public int doubleDamageUpgradeAmount;
    public int healthUpgradeUpAmount;
    public int magezineUpgradeUpAmount;
    int refillUpgradeUpAmount;
    

    // Start is called before the first frame update
    void Start()
    {
        refillUpgradeUpAmount = GameManager.instance.playerScript.magazineSize - GameManager.instance.playerScript.bulletsLeft;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
         
         if (other.CompareTag("Player") && doubleDamageUpgrade)
        {

            GameManager.instance.playerScript.shootDamage *= doubleDamageUpgradeAmount;

            Destroy(gameObject);

        }
        else if (other.CompareTag("Player") && shootRateUpgrade)
        {
            GameManager.instance.playerScript.shootRate -= shootRateUpgradeAmount;
            
            Destroy(gameObject);


        }
        else if(other.CompareTag("Player") && healthUpgrade)
        {
            GameManager.instance.playerScript.HPMax += healthUpgradeUpAmount;
            GameManager.instance.playerScript.HP = GameManager.instance.playerScript.HPMax;
            GameManager.instance.healthBar.fillAmount = (float)GameManager.instance.playerScript.HP / GameManager.instance.playerScript.HPMax;
            Destroy(gameObject);
        }
        else if(other.CompareTag("Player") && magazineUpgrade)
        {
            
            GameManager.instance.playerScript.magazineSize += 30;
            GameManager.instance.playerScript.bulletsLeft += GameManager.instance.playerScript.magazineSize;
            
            Destroy(gameObject);
        }
        if (other.CompareTag("Player") && killEnemiesUpgrade)
        {
            foreach (AIController ai in GameManager.instance.enemyAIScript)
            {
                ai.TakeDamage(killEnemiesUpgradeAmount, Vector3.zero);
            }
            Destroy(gameObject);

        }
        else if (other.CompareTag("Player") && refillUpgrade)
        {
            Debug.Log("Refill Picked Up");
            if (GameManager.instance.playerScript.bulletsLeft == 0)
            {
                GameManager.instance.playerScript.bulletsLeft = GameManager.instance.playerScript.magazineSize;
            }
            else
            {
                GameManager.instance.playerScript.bulletsLeft += refillUpgradeUpAmount;
            }
            Destroy(gameObject);
        }
    }
}
