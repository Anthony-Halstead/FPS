using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject characterUI;

    public GameObject bulletPowerUp;
    public GameObject rocketPowerUp;
    public GameObject speedPowerUp;
    public GameObject starPowerUp;

    public GameObject damagePanel;
    public GameObject healthBar;

    public GameObject player;
    public PlayerController playerScript;

    int enemyCount;
    float timeScaleOG;

    public bool isPaused;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        timeScaleOG = Time.timeScale;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel")) 
        {
            if(menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(isPaused);
            }
            else if(menuActive == menuPause)
            {
                stateUnpause();
            }
        }
    }

    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        characterUI.SetActive(false);
    }

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

    public void LoseGame()
    {
        statePause();
        menuActive = menuLose;
        menuLose.SetActive(true);
    }


}
