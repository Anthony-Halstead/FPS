using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    //[SerializeField] GameObject forestGate;
    //[SerializeField] GameObject industrialGate;
    //[SerializeField] GameObject bossGate;
    Animator animA;
    Animator animB;

    [Header("Gate Bools")]
    [SerializeField] bool forestGateBool;
    [SerializeField] bool industrialGateBool;
    [SerializeField] bool bossGateBool;
    [SerializeField] bool tower1Bool;
    [SerializeField] bool tower2Bool;

    [Header("Colliders")]
    [SerializeField] BoxCollider collider;

    
   
    // Start is called before the first frame update
    void Start()
    {

       // forestGate = GameObject.FindWithTag("GateA");
        animA = QuestManager.instance.forestGatePrefab.GetComponent<Animator>();
        animB = QuestManager.instance.industrialGatePrefab.GetComponent<Animator>();
       
        collider = QuestManager.instance.industrialGatePrefab.GetComponent<BoxCollider>();

        
    }

    //Collide with triggers and change stats and player pos
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && GameManager.instance.redKeyFound && GameManager.instance.playerSpawnPos.transform.position != transform.position && forestGateBool)
        {
            GameManager.instance.redkeySpriteImage.SetActive(false);
            GameManager.instance.objectivesText.text = "OBJECTIVE: Destroy Communication Tower 2";
            GameManager.instance.playerSpawnPos.transform.position = transform.position;

            animA.SetTrigger("OpenGate");
            QuestManager.instance.RemoveForestGateMarker();
            StartCoroutine(CheckpointPopup());

        }
        else if (other.CompareTag("Player") && GameManager.instance.greenKeyFound && GameManager.instance.playerSpawnPos.transform.position != transform.position && industrialGateBool)
        {
            GameManager.instance.greenkeySpriteImage.SetActive(false);
            GameManager.instance.objectivesText.text = "OBJECTIVE:Find The Warehouse";
            GameManager.instance.playerSpawnPos.transform.position = transform.position;
            animB.SetTrigger("OpenGate");
            QuestManager.instance.RemoveIndustrialGateMarker();

            StartCoroutine(CheckpointPopup());
        }
        else if (other.CompareTag("Player") && GameManager.instance.playerSpawnPos.transform.position != transform.position && bossGateBool)
        {
            GameManager.instance.objectivesText.text = "OBJECTIVE: Defeat The Boss";
            GameManager.instance.playerSpawnPos.transform.position = transform.position;
            QuestManager.instance.RemoveBossGateMarker();
            StartCoroutine(CheckpointPopup());
        }
        else if (other.CompareTag("Player") && tower1Bool)
        {
            GameManager.instance.playerSpawnPos.transform.position = transform.position;
            StartCoroutine(CheckpointPopup());
        }
        else if(other.CompareTag("Player") && tower2Bool)
        {
            GameManager.instance.playerSpawnPos.transform.position = transform.position;
            StartCoroutine(CheckpointPopup());
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player") && forestGateBool)
        {
            QuestManager.instance.AddTower2Marker();
            
        }
        else if(other.CompareTag("Player") && industrialGateBool)
        {
            
            QuestManager.instance.AddBossGateMarker();
        }
        else if(other.CompareTag("Player") && bossGateBool)
        {
            
        }
        else if(other.CompareTag("Player") && tower1Bool && GameManager.instance.isTower1Dead)
        {
            GameManager.instance.objectivesText.text = "OBJECTIVE: Find Red Key";
            QuestManager.instance.redKeyPrefab.SetActive(true);
            QuestManager.instance.AddRedKeyMarker();
        }
        else if (other.CompareTag("Player") && tower2Bool && GameManager.instance.isTower2Dead)
        {
            GameManager.instance.objectivesText.text = "OBJECTIVE: Find Green Key";
            QuestManager.instance.greenKeyPrefab.SetActive(true);
            QuestManager.instance.AddGreenKeyMarker();
        }
    }

    IEnumerator CheckpointPopup()
    {
        GameManager.instance.checkpointPopUp.SetActive(true);
        yield return new WaitForSeconds(0.7f);
        GameManager.instance.checkpointPopUp.SetActive(false);
    }
}
