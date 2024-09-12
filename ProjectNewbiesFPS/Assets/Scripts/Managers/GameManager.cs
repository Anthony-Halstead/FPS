using System;
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


    public List<GameObject> enemyHealthBarVisibility;

    public TextMeshProUGUI storeMoneyText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI enemyCountText;
    


    [SerializeField] int healthUpgradeCost;
    [SerializeField] int magezineUpgradeCost;
    [SerializeField] int shootRateUpgradeCost;
    [SerializeField] int doubleDamageUpgradeCost;
    [SerializeField] int killEnemiesUpgradeCost;

    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider sfxVolumeSlider;
    [SerializeField] Slider sensitivitySlider;

    [SerializeField] TMP_Text waveText;

    //References for taking damage
    public GameObject damagePanel;
    public Image healthBar;
    public List<Image> enemyHealthBar;

    //Player and script references
    public GameObject player;
    public PlayerController playerScript;
    public GameObject projectiles;
    public List<GameObject> enemyAI;
    public List<enemyAI> enemyAIScript;
    public GameObject mainCamera;
    public CameraController mainCameraController;
    public GameObject dropBoxObjectSpawned;


    public static Action<int>WaveCount;

    bool healthUpgradeBought;
    bool magazineUpgradeBought;
    bool shootRateUpgradeBought;
    bool doubleDamageUpgradeBought;
    bool killEnemiesUpgradeBought;

    //Enemy Reference
    private int enemyCount;
    public int EnemyCount { get { return enemyCount; } set { 
        enemyCount = value;
            if(enemyCount <= 0)
            {
                NextWave();
                WinGame();
            }
        } }
    int wave = 1;
    

    //Time Reference
    float timeScaleOG;

    //bool for if game is paused
    public bool isPaused;

    bool isGrounded;
    // Start is called before the first frame update
    void Awake()
    {
        //Setting Instance, original time and player references
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        timeScaleOG = Time.timeScale;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
        
        // projectiles = GameObject.FindWithTag("Projectiles");
        // projectilesScript = projectiles.GetComponent<Projectiles>();
        mainCamera = GameObject.FindWithTag("MainCamera");
        mainCameraController = mainCamera.GetComponent<CameraController>();

        moneyText.text = "" + playerScript.money;
        storeMoneyText.text = "" + playerScript.money;
         healthUpgrageToggle.isOn = false;
         magazineUpgrageToggle.isOn = false;
         shootRateUpgrageToggle.isOn = false;
         doubleDamageUpgrageToggle.isOn = false;
         killEnemiesUpgrageToggle.isOn = false;


        sensitivitySlider.value = 300f;
        masterVolumeSlider.value = 0;
        musicVolumeSlider.value = 0;
        sfxVolumeSlider.value = 0;

        
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
        ammoText.text = "" + playerScript.magazineSize;
       // waveText.text = "" + wave;
        moneyText.text = "$" + playerScript.money;
        enemyCountText.text = "" + enemyCount;
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
    public void WinGame()
    {
        
        
        if (wave == 10)
        {
            statePause();
            menuActive = menuWin;
            menuWin.SetActive(true);
        }


    }

    public void NextWave()
    {  
        wave++;
        SpawnWave();
    }


    //Losing Game Method
    public void LoseGame()
    {
        statePause();
        menuActive = menuLose;
        menuLose.SetActive(true);
        damagePanel.SetActive(false);

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
            foreach (GameObject bar in enemyHealthBarVisibility)
            {
                bar.SetActive(true);
            }
        }
        else
        {
            foreach (GameObject bar in enemyHealthBarVisibility)
            {
                bar.SetActive(false);
            }
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
            Instantiate(dropBox, player.transform.position + new Vector3(0,6,4), player.transform.localRotation );
            
        }
    }

   public void InstantiateUpgrades()
    {
        dropBoxObjectSpawned = GameObject.FindWithTag("DropBox");
        if (healthUpgradeBought)
        {
            Instantiate(healthUpgrade, dropBoxObjectSpawned.transform.position + new Vector3(0, 0, 4), dropBoxObjectSpawned.transform.localRotation);

            healthUpgradeBought = false;

        }
         if (magazineUpgradeBought)
        {
            Instantiate(magazineUpgrade, dropBoxObjectSpawned.transform.position + new Vector3(0, 0, 4), dropBoxObjectSpawned.transform.localRotation);

            magazineUpgradeBought = false;
        }
         if (shootRateUpgradeBought)
        {
            Instantiate(shootRateUpgrade, dropBoxObjectSpawned.transform.position + new Vector3(0, 0, 4), dropBoxObjectSpawned.transform.localRotation);

            shootRateUpgradeBought = false;
        }
         if (doubleDamageUpgradeBought)
        {
            Instantiate(doubleDamageUpgrade, dropBoxObjectSpawned.transform.position + new Vector3(0, 0, 4), dropBoxObjectSpawned.transform.localRotation);

            doubleDamageUpgradeBought = false;
        }
         if (killEnemiesUpgradeBought)
        {
            Instantiate(killEnemiesUpgrade, dropBoxObjectSpawned.transform.position + new Vector3(0, 0, 4), dropBoxObjectSpawned.transform.localRotation);

            killEnemiesUpgradeBought = false;
        }
    }

    void SpawnWave()
    {
        WaveCount.Invoke(wave);
        
            SpawnManager.instance.TriggerAllSpawnPoints();
          
        
        
    }

}


