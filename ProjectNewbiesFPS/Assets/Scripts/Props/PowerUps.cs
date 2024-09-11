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

 
    public int killEnemiesUpgradeAmount;
    public int shootRateUpgradeAmount;
    public int doubleDamageUpgradeAmount;
    public int healthUpgradeUpAmount;
    public int magezineUpgradeUpAmount;

    [SerializeField] AudioSource audioSource;
    

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {

        
         if (other.CompareTag("Player") && doubleDamageUpgrade)
        {
            audioSource.Play();
            //audioManager.playSFX(audioManager.UpgradePickUp);
            GameManager.instance.playerScript.shootDamage *= doubleDamageUpgradeAmount;

            Destroy(gameObject);

        }
        else if (other.CompareTag("Player") && shootRateUpgrade)
        {
            audioSource.Play();
            // audioManager.playSFX(audioManager.UpgradePickUp);
            GameManager.instance.playerScript.shootRate += shootRateUpgradeAmount;
            
            Destroy(gameObject);


        }
        else if(other.CompareTag("Player") && healthUpgrade)
        {
            audioSource.Play();
            //audioManager.playSFX(audioManager.UpgradePickUp);
            GameManager.instance.playerScript.HPMax += healthUpgradeUpAmount;
            GameManager.instance.healthBar.fillAmount = GameManager.instance.playerScript.HP / GameManager.instance.playerScript.HPMax;
            Destroy(gameObject);
        }
        else if(other.CompareTag("Player") && magazineUpgrade)
        {
            audioSource.Play();
            //audioManager.playSFX(audioManager.UpgradePickUp);
            GameManager.instance.playerScript.magazineSize += 30;
            
            Destroy(gameObject);
        }
        if (other.CompareTag("Player") && killEnemiesUpgrade)
        {
            audioSource.Play();
            //audioManager.playSFX(audioManager.UpgradePickUp);
            GameManager.instance.enemyAIScript.takeDamage(killEnemiesUpgradeAmount, Vector3.zero);
            Destroy(gameObject);

        }
    }
}
