using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PowerUps : MonoBehaviour, IPowerUps
{
    [SerializeField] bool bulletPowerUp;
    [SerializeField] bool rocketPowerUp;
    [SerializeField] bool shootRateUpgrade;
    [SerializeField] bool starPowerUp;
    [SerializeField] bool healthPowerUp;
    [SerializeField] bool healthUpgrade;
    [SerializeField] bool magazineUpgrade;
    [SerializeField] bool coin;
    

    [SerializeField] float powerUpTime;
    float originalSpeed = 3;

    public int bulletPowerUpAmount;
    public int rocketPowerUpAmount;
    public int shootRateUpgradeAmount;
    public int starPowerUpAmount;
    public int healthPowerUpAmount;
    public int healthUpgradeUpAmount;
    public int magezineUpgradeUpAmount;
    public int coinAmount;

    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && bulletPowerUp)
        {
            //GameManager.instance.bulletPowerUp.SetActive(true);
            GameManager.instance.projectilesScript.magazineSize += bulletPowerUpAmount;
            Destroy(gameObject);


        }
        else if (other.CompareTag("Player") && rocketPowerUp)
        {
           // GameManager.instance.rocketPowerUp.SetActive(true);
            GameManager.instance.enemyAIScript.takeDamage(rocketPowerUpAmount);
            Destroy(gameObject);

        }
        else if (other.CompareTag("Player") && starPowerUp)
        {
           // GameManager.instance.starPowerUp.SetActive(true);
            Destroy(gameObject);

        }
        else if (other.CompareTag("Player") && healthPowerUp)
        {
            GameManager.instance.playerScript.HP += healthPowerUpAmount;

            GameManager.instance.healthBar.fillAmount = GameManager.instance.playerScript.HP / GameManager.instance.playerScript.HPMax;
            Destroy(gameObject);


        }
        else if (other.CompareTag("Player") && shootRateUpgrade)
        {
            
            
            Destroy(gameObject);


        }
        else if(other.CompareTag("Player") && healthUpgrade)
        {
            GameManager.instance.playerScript.HPMax += healthUpgradeUpAmount;
            GameManager.instance.healthBar.fillAmount = GameManager.instance.playerScript.HP / GameManager.instance.playerScript.HPMax;
            Destroy(gameObject);
        }
        else if(other.CompareTag("Player") && magazineUpgrade)
        {
            GameManager.instance.projectilesScript.magazineSize += 30;
            Destroy(gameObject);
        }
        //else if(other.CompareTag("Player") && coin)
        //{
        //    GameManager.instance.playerScript.money += coinAmount;
        //    GameManager.instance.moenyText.text = "Money: " + GameManager.instance.playerScript.money.ToString();
        //    Destroy(gameObject);
            
        //}
    }

    //IEnumerator speedPowerUpTime()
    //{
    //    Debug.Log("Coroutuine Started");
    //    GameManager.instance.playerScript.speed += speedPowerUpAmount;
    //    yield return new WaitForSeconds(powerUpTime);
    //    Debug.Log("Coroutine Ended");
    //    GameManager.instance.playerScript.speed = originalSpeed;
    //    GameManager.instance.speedPowerUp.SetActive(false);
    //}
}
