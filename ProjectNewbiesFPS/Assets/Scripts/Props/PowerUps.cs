using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PowerUps : MonoBehaviour, IPowerUps
{
    [SerializeField] bool bulletPowerUp;
    [SerializeField] bool rocketPowerUp;
    [SerializeField] bool speedPowerUp;
    [SerializeField] bool starPowerUp;
    [SerializeField] bool healthPowerUp;
    [SerializeField] bool coin;
    

    [SerializeField] float powerUpTime;
    float originalSpeed = 3;

    public int bulletPowerUpAmount;
    public int rocketPowerUpAmount;
    public int speedPowerUpAmount;
    public int starPowerUpAmount;
    public int healthPowerUpAmount;
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
            Destroy(gameObject);
            GameManager.instance.bulletPowerUp.SetActive(true);
            GameManager.instance.projectilesScript.magazineSize += bulletPowerUpAmount;
            
        } 
        else if(other.CompareTag("Player") && rocketPowerUp)
        {
            Destroy(gameObject);
            GameManager.instance.rocketPowerUp.SetActive(true);
            GameManager.instance.enemyAIScript.takeDamage(rocketPowerUpAmount);
        }
        else if(other.CompareTag("Player") && speedPowerUp)
        {
            Destroy(gameObject);
            GameManager.instance.speedPowerUp.SetActive(true);
            StartCoroutine(speedPowerUpTime());

        }
        else if(other.CompareTag("Player") &&  starPowerUp)
        {
            Destroy(gameObject);
            GameManager.instance.starPowerUp.SetActive(true);
        }
        else if(other.CompareTag("Player") && healthPowerUp)
        {
            
                Destroy(gameObject);
            GameManager.instance.playerScript.HP += healthPowerUpAmount;
                
                GameManager.instance.healthBar.fillAmount = GameManager.instance.playerScript.HP / 10;
            
        }
        else if(other.CompareTag("Player") && coin)
        {
            Destroy(gameObject);
            GameManager.instance.playerScript.money += coinAmount;
            GameManager.instance.moenyText.text = "Money: " + GameManager.instance.playerScript.money.ToString();
        }
    }

    IEnumerator speedPowerUpTime()
    {
        GameManager.instance.playerScript.speed *= speedPowerUpAmount;
        yield return new WaitForSeconds(powerUpTime);
        GameManager.instance.playerScript.speed = originalSpeed;
    }
}
