using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //Instance for gameManager
    public static GameManager instance;

    //References for menu Objects, characterUI and money Text
    [Header("Menu Objects")]
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject buyMenu;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject tutorialMenu;
    [SerializeField] GameObject characterUI;

    [Header("Compass")]
    public RawImage compassImage;
    public GameObject questIconPrefab;
    //quest markers
    public List<QuestMarkers> questMarkers = new List<QuestMarkers>();

    float compassUnit;
    public float maxDistance = 4000f;

    [Header("InteractUI")]
    [SerializeField] private GameObject interactUI;
    [SerializeField] private TMP_Text interactText;

    [Header("PickUp Objects")]
    [SerializeField] GameObject healthUpgrade;
    [SerializeField] GameObject magazineUpgrade;
    [SerializeField] GameObject shootRateUpgrade;
    [SerializeField] GameObject doubleDamageUpgrade;
    [SerializeField] GameObject killEnemiesUpgrade;
    [SerializeField] GameObject refillUpgrade;



    //Key Sprite Images
    [Header("Key Sprites")]
    public GameObject redkeySpriteImage;
    public GameObject blackkeySpriteImage;
    public GameObject greenkeySpriteImage;

    [Header("Tower Bools")]
    public bool isTower1Dead;
    public bool isTower2Dead;

    [Header("Checkpoint UI")]
    public GameObject checkpointPopUp;

    [Header("DropBox")]
    [SerializeField] GameObject dropBox;
    [SerializeField] Transform dropBoxSpawnPos;
    [SerializeField] Transform upgradeBoughtSpawnPos;

    [Header("Shop Item Toggles")]
    [SerializeField] Toggle healthUpgrageToggle;
    [SerializeField] Toggle magazineUpgrageToggle;
    [SerializeField] Toggle shootRateUpgrageToggle;
    [SerializeField] Toggle doubleDamageUpgrageToggle;
    [SerializeField] Toggle killEnemiesUpgrageToggle;
    [SerializeField] Toggle refillUpgradeToggle;

    [Header("Enemy Health Bar")]
    public List<GameObject> enemyHealthBarVisibility;

    [Header("Texts")]
    public TextMeshProUGUI storeMoneyText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI enemyCountText;
    public TextMeshProUGUI objectivesText;
    public TMP_Text visText;
    //[SerializeField] TMP_Text waveText;

    [Header("Shop Item Costs")]
    [SerializeField] int healthUpgradeCost;
    [SerializeField] int magezineUpgradeCost;
    [SerializeField] int shootRateUpgradeCost;
    [SerializeField] int doubleDamageUpgradeCost;
    [SerializeField] int killEnemiesUpgradeCost;
    [SerializeField] int refillUpgradeCost;

    [Header("Options Menu Items")]
    [SerializeField] Toggle enemyHealthBarToggle;
    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider sfxVolumeSlider;
    [SerializeField] Slider sensitivitySlider;



    [Header("Player UI")]
    //References for taking damage
    public GameObject damagePanel;
    public Image healthBar;
    public TextMeshProUGUI healthBarText;
    public List<Image> enemyHealthBar;

    [Header("Script References")]
    //Player and script references
    public GameObject player;
    public PlayerController playerScript;
    public GameObject projectiles;
    public List<GameObject> enemyAI;
    public List<AIController> enemyAIScript;
    public GameObject mainCamera;
    public CameraController mainCameraController;
    public GameObject dropBoxObjectSpawned;
    public GameObject playerSpawnPos;





    [Header("Waves")]
    //Tracking Waves
    public static Action<int>WaveCount;


    //bools for store upgrades
    bool healthUpgradeBought;
    bool magazineUpgradeBought;
    bool shootRateUpgradeBought;
    bool doubleDamageUpgradeBought;
    bool killEnemiesUpgradeBought;
    bool refillUpgradeBought;

    //bools for keys found
    public bool redKeyFound;
    public bool blackKeyFound;
    public bool greenKeyFound;

    //Enemy Reference
   
   // int wave = 1;
    

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

        //Assigning references
        timeScaleOG = Time.timeScale;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
        mainCamera = GameObject.FindWithTag("MainCamera");
        mainCameraController = mainCamera.GetComponent<CameraController>();
        playerSpawnPos = GameObject.FindWithTag("PlayerSpawnPos");
        

        //Initializing money
        moneyText.text = "" + playerScript.money;
        storeMoneyText.text = "" + playerScript.money;

        //Initializing Objectives Text
        objectivesText.text = "OBJECTIVE: Destory Communication Tower 1";

        //Initializing shop icons as not checked
         healthUpgrageToggle.isOn = false;
         magazineUpgrageToggle.isOn = false;
         shootRateUpgrageToggle.isOn = false;
         doubleDamageUpgrageToggle.isOn = false;
         killEnemiesUpgrageToggle.isOn = false;
        // refillUpgradeToggle.isOn = false;

        //Initializing Slider values on Options menu to a default
        sensitivitySlider.value = 300f;
        masterVolumeSlider.value = 0;
        musicVolumeSlider.value = 0;
        sfxVolumeSlider.value = 0;

        //Show Tutorial Screen at start of game
       TutorialMenu();

        
    }

    private void Start()
    {
        compassUnit = compassImage.rectTransform.rect.width / 360f;
        QuestManager.instance.AddTower1Marker();
    }

    public void ToggleInteractionUI(bool toggle, string text)
    {
        if (toggle)
        {
            interactUI.SetActive(true);
            interactText.text = text;
        }
        else
        {
            interactUI.SetActive(false);
            interactText.text = "";
        }
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
                AudioManager.instance.playSFX(AudioManager.instance.menuUp);

                statePause();
                menuActive = menuPause;
                menuActive.SetActive(isPaused);
            }
            //Unpausing the game
            else if (menuActive == menuPause)
            {
                AudioManager.instance.playSFX(AudioManager.instance.menuDown);

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

        //Updating UI items during game
        ammoText.text = playerScript.bulletsLeft + "/" + playerScript.magazineSize;
      //  waveText.text = "" + wave;
        moneyText.text = "$" + playerScript.money;
       // enemyCountText.text = "" + enemyCount;
        healthBarText.text = "" + playerScript.HP + "/" + playerScript.HPMax;
        //visText.text = playerScript.getPlayerVisibility().ToString("F0") + "%";


        //closing out of tutorial screen
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (menuActive == tutorialMenu)
            {
                stateUnpause();
                
            }
        }

        //Compass UI
        compassImage.uvRect = new Rect(player.transform.localEulerAngles.y / 360, 0f, 1f, 1f);
        if (questMarkers.Count > 0)
        {
            foreach (QuestMarkers marker in questMarkers)
            {
               // if (marker == null) break;
                marker.image.rectTransform.anchoredPosition = GetPosOnCompass(marker);

                float dst = Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.z), marker.position);
                float scale = 0f;

                if (dst < maxDistance)
                {
                    scale = 1f - (dst / maxDistance);
                }

                marker.image.rectTransform.localScale = Vector3.one * scale;
            }
        }
    }

    //Methods for Quest Markers
    

    Vector2 GetPosOnCompass(QuestMarkers marker)
    {
        Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.z);
        Vector2 playerFwd = new Vector2(player.transform.forward.x, player.transform.forward.z);

        float angle = Vector2.SignedAngle(marker.position - playerPos, playerFwd);

        return new Vector2(compassUnit * angle, 0f);
    }

    //Pausing Game Method
    public void statePause()
    {
        if(damagePanel != null)
        {
            damagePanel.SetActive(false);
        }
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
        //if (wave == 10)
        //{
        //    statePause();
        //    menuActive = menuWin;
        //    menuWin.SetActive(true);
        //}
    }

    //public void NextWave()
    //{  
    //    wave++;
    //    SpawnWave();
    //}


    //Losing Game Method
    public void LoseGame()
    {
        statePause();
        menuActive = menuLose;
        menuLose.SetActive(true);
        damagePanel.SetActive(false);

    }

    //Closing Shop
    public void BuyMenu()
    {
        if (menuActive == buyMenu)
        {
            stateUnpause();
        }
    }

    //Opening Options menu
    public void Options()
    {
        optionsMenu.SetActive(true);
        menuActive = optionsMenu;
        menuPause.SetActive(false);
    }

    //Toggling Enemy Health Bar in Options menu
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

    //Submitting order from Shop
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
        if (GameManager.instance.playerScript.money >= refillUpgradeCost)
        {
            if (refillUpgradeToggle.isOn)
            {
                Debug.Log("Refill Chosen");
                GameManager.instance.playerScript.money -= refillUpgradeCost;

                GameManager.instance.storeMoneyText.text = "$" + GameManager.instance.playerScript.money;
                refillUpgradeBought = true;
                refillUpgradeToggle.isOn = false;
            }
        }
        DropBox();
    }

    //Instantiating Drop Box 
    public void DropBox()
    {
        stateUnpause();
        if (healthUpgradeBought || magazineUpgradeBought || shootRateUpgradeBought || doubleDamageUpgradeBought || killEnemiesUpgradeBought || refillUpgradeBought)
        {
            Instantiate(dropBox, player.transform.position + new Vector3(0,6,4), player.transform.localRotation );
            
        }
    }

    //Instantiating Upgrades after Box Drops
   public void InstantiateUpgrades()
    {
        dropBoxObjectSpawned = GameObject.FindWithTag("DropBox");
        if (healthUpgradeBought)
        {
            Instantiate(healthUpgrade, dropBoxObjectSpawned.transform.position + new Vector3(0, 1, 4), dropBoxObjectSpawned.transform.localRotation);

            healthUpgradeBought = false;

        }
         if (magazineUpgradeBought)
        {
            Instantiate(magazineUpgrade, dropBoxObjectSpawned.transform.position + new Vector3(0, 1, 4), dropBoxObjectSpawned.transform.localRotation);

            magazineUpgradeBought = false;
        }
         if (shootRateUpgradeBought)
        {
            Instantiate(shootRateUpgrade, dropBoxObjectSpawned.transform.position + new Vector3(0, 1, 4), dropBoxObjectSpawned.transform.localRotation);

            shootRateUpgradeBought = false;
        }
         if (doubleDamageUpgradeBought)
        {
            Instantiate(doubleDamageUpgrade, dropBoxObjectSpawned.transform.position + new Vector3(0, 1, 4), dropBoxObjectSpawned.transform.localRotation);

            doubleDamageUpgradeBought = false;
        }
         if (killEnemiesUpgradeBought)
        {
            Instantiate(killEnemiesUpgrade, dropBoxObjectSpawned.transform.position + new Vector3(0, 1, 4), dropBoxObjectSpawned.transform.localRotation);

            killEnemiesUpgradeBought = false;
        }
        if (refillUpgradeBought)
        {
            Instantiate(refillUpgrade, dropBoxObjectSpawned.transform.position + new Vector3(0, 1, 4), dropBoxObjectSpawned.transform.localRotation);

            refillUpgradeBought = false;
        }
    }

    //Spawning a wave
    //void SpawnWave()
    //{
    //    WaveCount.Invoke(wave);
        
    //        SpawnManager.instance.TriggerAllSpawnPoints();
    //}

    //Opening Tutorial Menu from Options menu
    public void TutorialMenu()
    {
        if (menuActive == null)
        {
            statePause();
            menuActive = tutorialMenu;
            menuActive.SetActive(true);
        }
       else if (menuActive == menuPause)
        {
            menuActive = tutorialMenu;
            menuActive.SetActive(true);
            menuPause.SetActive(false);
        }
        
    }

    
    

}


