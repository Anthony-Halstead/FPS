using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //Instance for gameManager
    public static GameManager instance;

    //References for menu Objects, characterUI and money Text
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject buyMenu;
    [SerializeField] GameObject characterUI;
    public TextMeshProUGUI moenyText;


    //References for Power Ups
    public GameObject bulletPowerUp;
    public GameObject rocketPowerUp;
    public GameObject speedPowerUp;
    public GameObject starPowerUp;

    //References for taking damage
    public GameObject damagePanel;
    public Image healthBar;

    //Player and script references
    public GameObject player;
    public PlayerController playerScript;
    public GameObject projectiles;
    public Projectiles projectilesScript;
    public GameObject enemyAI;
    public EnemyAI enemyAIScript;

    //Enemy Reference
    int enemyCount;

    //Time Reference
    float timeScaleOG;

    //bool for if game is paused
    public bool isPaused;
    // Start is called before the first frame update
    void Awake()
    {
        //Setting Instance, original time and player references
        instance = this;
        timeScaleOG = Time.timeScale;
        //player = GameObject.FindWithTag("Player");
        //playerScript = player.GetComponent<PlayerController>();
        //enemyAI = GameObject.FindWithTag("Enemy");
        //enemyAIScript = enemyAI.GetComponent<EnemyAI>();
        //projectiles = GameObject.FindWithTag("Projectiles");
        //projectilesScript = projectiles.GetComponent<Projectiles>();
    }

    // Update is called once per frame
    void Update()
    {
        //Finding out if menu is paused at any moment
        if (Input.GetButtonDown("Cancel")) 
        {
            Debug.Log("Pause");
            //pausing the game
            if(menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(isPaused);
            }
            //Unpausing the game
            else if(menuActive == menuPause)
            {
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
            }
            

        }
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
            menuWin.SetActive(isPaused);
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

}
