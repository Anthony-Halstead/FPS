using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //Instance for gameManager
    public static GameManager instance;
    public AudioManager audioManager;

    //References for menu Objects, characterUI and money Text
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject buyMenu;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject characterUI;

    [SerializeField] GameObject healthUpgrade;
    [SerializeField] GameObject magazineUpgrade;
    [SerializeField] GameObject shootRateUpgrade;
    [SerializeField] GameObject doubleDamageUpgrade;
    [SerializeField] GameObject killEnemiesUpgrade;


    [SerializeField] GameObject dropBox;

    [SerializeField] Transform dropBoxSpawnPos;
    [SerializeField] Transform upgradeBoughtSpawnPos;

    [SerializeField] Toggle healthUpgrageToggle;
    [SerializeField] Toggle magazineUpgrageToggle;
    [SerializeField] Toggle shootRateUpgrageToggle;
    [SerializeField] Toggle doubleDamageUpgrageToggle;
    [SerializeField] Toggle killEnemiesUpgrageToggle;


    [SerializeField] Toggle enemyHealthBarToggle;


    [SerializeField] GameObject enemyHealthBarVisibility;

    public TextMeshProUGUI storeMoneyText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI ammoText;


    [SerializeField] int healthUpgradeCost;
    [SerializeField] int magezineUpgradeCost;
    [SerializeField] int shootRateUpgradeCost;
    [SerializeField] int doubleDamageUpgradeCost;
    [SerializeField] int killEnemiesUpgradeCost;


    //References for taking damage
    public GameObject damagePanel;
    public Image healthBar;
    public Image enemyHealthBar;

    //Player and script references
    public GameObject player;
    public PlayerController playerScript;
    public GameObject projectiles;
    public GameObject enemyAI;
    public enemyAI enemyAIScript;

    bool healthUpgradeBought;
    bool magazineUpgradeBought;
    bool shootRateUpgradeBought;
    bool doubleDamageUpgradeBought;
    bool killEnemiesUpgradeBought;

    //Enemy Reference
    int enemyCount;

    //Time Reference
    float timeScaleOG;

    //bool for if game is paused
    public bool isPaused;

    bool isGrounded;
    // Start is called before the first frame update
    void Awake()
    {
        //Setting Instance, original time and player references
        instance = this;
        timeScaleOG = Time.timeScale;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
        enemyAI = GameObject.FindWithTag("Enemy");
        enemyAIScript = enemyAI.GetComponent<enemyAI>();
        projectiles = GameObject.FindWithTag("Projectiles");
        // projectilesScript = projectiles.GetComponent<Projectiles>();
        audioManager = GameObject.FindWithTag("Audio").GetComponent<AudioManager>();
        moneyText.text = "" + playerScript.money;
        storeMoneyText.text = "" + playerScript.money;
         healthUpgrageToggle.isOn = false;
         magazineUpgrageToggle.isOn = false;
         shootRateUpgrageToggle.isOn = false;
         doubleDamageUpgrageToggle.isOn = false;
         killEnemiesUpgrageToggle.isOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Finding out if menu is paused at any moment
        if (Input.GetButtonDown("Cancel"))
        {
            Debug.Log("Pause");
            //pausing the game
            if (menuActive == null)
            {
                audioManager.playSFX(audioManager.menuUp);

                statePause();
                menuActive = menuPause;
                menuActive.SetActive(isPaused);
            }
            //Unpausing the game
            else if (menuActive == menuPause)
            {
                audioManager.playSFX(audioManager.menuDown);

                stateUnpause();
            }
        }
        //finding if BuyMenu is requested
        else if (Input.GetButtonDown("Buy"))
        {
            Debug.Log("Buy");
            //setting screen active
            if (menuActive == null)
            {
                statePause();
                menuActive = buyMenu;
                menuActive.SetActive(true);
                storeMoneyText.text = "$" + playerScript.money;
            }

            

        }
      //  ammoText.text = "" + projectilesScript.magazineSize;
    }

    //Pausing Game Method
    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        characterUI.SetActive(false);
    }

    //Unpausing Game Method
    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOG;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(isPaused);
        menuActive = null;
        characterUI.SetActive(true);
    }

    //Winning Game Method
    public void WinGame(int amount)
    {
        enemyCount += amount;

        if (enemyCount <= 0)
        {


            statePause();
            menuActive = menuWin;
            menuWin.SetActive(true);
        }
    }

    //Losing Game Method
    public void LoseGame()
    {
        statePause();
        menuActive = menuLose;
        menuLose.SetActive(true);

    }

    public void BuyMenu()
    {
        if (menuActive == buyMenu)
        {
            stateUnpause();
        }
    }

    public void Options()
    {
        optionsMenu.SetActive(true);
        menuActive = optionsMenu;
        menuPause.SetActive(false);
    }

    public void ToggleEnemyHealthBar()
    {
        if (enemyHealthBarToggle.isOn)
        {
            enemyHealthBarVisibility.SetActive(true);
        }
        else
        {
            enemyHealthBarVisibility.SetActive(false);
        }
    }

    public void StoreOrder()
    {
        if (GameManager.instance.playerScript.money >= healthUpgradeCost)
        {
            if (healthUpgrageToggle.isOn)
            {
                Debug.Log("Health Chosen");
                GameManager.instance.playerScript.money -= healthUpgradeCost;
                
                GameManager.instance.storeMoneyText.text = "$" + GameManager.instance.playerScript.money;
                healthUpgradeBought = true;
                healthUpgrageToggle.isOn = false;
            }

        }
        if (GameManager.instance.playerScript.money >= magezineUpgradeCost)
        {
            if (magazineUpgrageToggle.isOn)
            {
                Debug.Log("Magazine Chosen");
                GameManager.instance.playerScript.money -= magezineUpgradeCost;

                GameManager.instance.storeMoneyText.text = "$" + GameManager.instance.playerScript.money;
                magazineUpgradeBought = true;
                magazineUpgrageToggle.isOn = false;
            }
        }
         if (GameManager.instance.playerScript.money >= shootRateUpgradeCost)
        {
            if (shootRateUpgrageToggle.isOn)
            {
                Debug.Log("Shoot Rate Chosen");
                GameManager.instance.playerScript.money -= shootRateUpgradeCost;

                GameManager.instance.storeMoneyText.text = "$" + GameManager.instance.playerScript.money;
                shootRateUpgradeBought = true;
                shootRateUpgrageToggle.isOn = false;
            }
        }
         if (GameManager.instance.playerScript.money >= doubleDamageUpgradeCost)
        {
            if (doubleDamageUpgrageToggle.isOn)
            {
                Debug.Log("Double Damage Chosen");
                GameManager.instance.playerScript.money -= doubleDamageUpgradeCost;

                GameManager.instance.storeMoneyText.text = "$" + GameManager.instance.playerScript.money;
                doubleDamageUpgradeBought = true;
                doubleDamageUpgrageToggle.isOn = false;
            }
        }
         if (GameManager.instance.playerScript.money >= killEnemiesUpgradeCost)
        {
            if (killEnemiesUpgrageToggle.isOn)
            {
                Debug.Log("Kill Enemies Chosen");
                GameManager.instance.playerScript.money -= killEnemiesUpgradeCost;

                GameManager.instance.storeMoneyText.text = "$" + GameManager.instance.playerScript.money;
                killEnemiesUpgradeBought = true;
                killEnemiesUpgrageToggle.isOn = false;
            }
        }
        DropBox();
    }


    public void DropBox()
    {
        stateUnpause();
        if (healthUpgradeBought || magazineUpgradeBought || shootRateUpgradeBought || doubleDamageUpgradeBought || killEnemiesUpgradeBought)
        {
            Instantiate(dropBox, Vector3.up + Vector3.forward, Quaternion.identity);
            
        }
    }

   public void InstantiateUpgrades()
    {
        
        if (healthUpgradeBought)
        {
            Instantiate(healthUpgrade, Vector3.forward, Quaternion.identity);

            healthUpgradeBought = false;

        }
        else if (magazineUpgradeBought)
        {
            Instantiate(magazineUpgrade, Vector3.forward, Quaternion.identity);

            magazineUpgradeBought = false;
        }
        else if (shootRateUpgradeBought)
        {
            Instantiate(shootRateUpgrade, Vector3.forward, Quaternion.identity);

            shootRateUpgradeBought = false;
        }
        else if (doubleDamageUpgradeBought)
        {
            Instantiate(doubleDamageUpgrade, Vector3.forward, Quaternion.identity);

            doubleDamageUpgradeBought = false;
        }
        else if (killEnemiesUpgradeBought)
        {
            Instantiate(killEnemiesUpgrade, Vector3.forward, Quaternion.identity);

            killEnemiesUpgradeBought = false;
        }
    }

}


