using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PowerUps : MonoBehaviour, IPowerUps
{
    public GameObject player;
     public PlayerController controller;

    

    [SerializeField] bool bulletPowerUp;
    [SerializeField] bool rocketPowerUp;
    [SerializeField] bool speedPowerUp;
    [SerializeField] bool starPowerUp;
    [SerializeField] bool healthPowerUp;
    [SerializeField] bool coin;

    [SerializeField] float powerUpTime;
    float originalSpeed;

    public int bulletPowerUpAmount;
    public int rocketPowerUpAmount;
    public int speedPowerUpAmount;
    public int starPowerUpAmount;
    public int healthPowerUpAmount;
    public int coinAmount;

    // Start is called before the first frame update
    void Start()
    {
        originalSpeed = controller.speed;
        player = GameObject.FindWithTag("Player");
        controller = GetComponent<PlayerController>();
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
            
        } 
        else if(other.CompareTag("Player") && rocketPowerUp)
        {
            Destroy(gameObject);
            GameManager.instance.rocketPowerUp.SetActive(true);
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
            if (controller.HP + healthPowerUpAmount < 10)
            {
                Destroy(gameObject);
                controller.HP += healthPowerUpAmount;
                GameManager.instance.healthBar.fillAmount = controller.HP / 10;
            }
        }
        else if(other.CompareTag("Player") && coin)
        {
            Destroy(gameObject);
            controller.money += coinAmount;
            GameManager.instance.moenyText.text = "Money: " + controller.money.ToString();
        }
    }

    IEnumerator speedPowerUpTime()
    {
        controller.speed *= speedPowerUpAmount;
        yield return new WaitForSeconds(powerUpTime);
        controller.speed = originalSpeed;
    }
}
