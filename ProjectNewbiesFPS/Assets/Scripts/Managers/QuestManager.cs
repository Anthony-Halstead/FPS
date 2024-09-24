using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;
    //Key & Gate Quests
    [Header("Prefabs")]
    public GameObject redKeyPrefab;
    public GameObject greenKeyPrefab;
    public GameObject forestGatePrefab;
    public GameObject industrialGatePrefab;
    public GameObject bossGatePrefab;
    public GameObject tower1Prefab;
    public GameObject tower2Prefab;

    [Header("Markers")]
    public QuestMarkers redKeyMarker;
    public QuestMarkers greenKeyMarker;
    public QuestMarkers forestGateMarker;
    public QuestMarkers industrialGateMarker;
    public QuestMarkers bossGateMarker;
    public QuestMarkers tower1Marker;
    public QuestMarkers tower2Marker;

    public GameObject newMarker;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;

        //Quest Prefabs
        redKeyPrefab = GameObject.FindWithTag("RedKey");
        greenKeyPrefab = GameObject.FindWithTag("GreenKey");
        forestGatePrefab = GameObject.FindWithTag("GateA");
        industrialGatePrefab = GameObject.FindWithTag("IndustrialGate");
        bossGatePrefab = GameObject.FindWithTag("BossGate");
        tower1Prefab = GameObject.FindWithTag("Tower1");
        tower2Prefab = GameObject.FindWithTag("Tower2");

        //Getting Marker Scripts
        redKeyMarker = redKeyPrefab.GetComponent<QuestMarkers>();
        greenKeyMarker = greenKeyPrefab.GetComponent<QuestMarkers>();
        forestGateMarker = forestGatePrefab.GetComponent<QuestMarkers>();
        industrialGateMarker = industrialGatePrefab.GetComponent<QuestMarkers>();
        bossGateMarker = bossGatePrefab.GetComponent<QuestMarkers>();
        tower1Marker = tower1Prefab.GetComponent<QuestMarkers>();
        tower2Marker = tower2Prefab.GetComponent<QuestMarkers>();

    }
    private void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Adding and Removing Methods
    private void AddQuestMarker(QuestMarkers marker)
    {
        if (newMarker == null)
        {
            newMarker = Instantiate(GameManager.instance.questIconPrefab, GameManager.instance.compassImage.transform);
            marker.image = newMarker.GetComponent<Image>();
            marker.image.sprite = marker.icon;

            GameManager.instance.questMarkers.Add(marker);
        }
    }
    private void RemoveQuestMarker(QuestMarkers marker)
    {
        if (GameManager.instance.questMarkers.Count > 0)
        {
            // foreach (QuestMarkers questMarker in GameManager.instance.questMarkers)
            // {
            // if (questMarker == marker)
            // {
            GameManager.instance.questMarkers.Clear();
            Destroy(newMarker);
            newMarker = null;
              //  }
           // }
        }
    }

    //Add and Remove RedKeyMarker
    public void AddRedKeyMarker()
    {
        AddQuestMarker(redKeyMarker);
    }
    public void RemoveRedKeyMarker()
    {
        RemoveQuestMarker(redKeyMarker);
    }

    //Add and Remove GreenKeyMarker
    public void AddGreenKeyMarker()
    {
        //Debug.Log("GreenKey Quest Added");
        AddQuestMarker(greenKeyMarker);
    }
    public void RemoveGreenKeyMarker()
    {
        RemoveQuestMarker(greenKeyMarker);
    }

    //Add and Remove ForestGateMarker
    public void AddForestGateMarker()
    {
        AddQuestMarker(forestGateMarker);
    }
    public void RemoveForestGateMarker()
    {
        RemoveQuestMarker(forestGateMarker);
    }

    //Add and Remove IndustrialGateMarker
    public void AddIndustrialGateMarker()
    {
        AddQuestMarker(industrialGateMarker);
    }
    public void RemoveIndustrialGateMarker()
    {
        RemoveQuestMarker(industrialGateMarker);
    }

    //Add and Remove BossGateMarker
    public void AddBossGateMarker()
    {
        AddQuestMarker(bossGateMarker);
    }
    public void RemoveBossGateMarker()
    {
        RemoveQuestMarker(bossGateMarker);
    }

    //Add and Remove tower1Marker
    public void AddTower1Marker()
    {
        AddQuestMarker(tower1Marker);
    }
    public void RemoveTower1Marker()
    {
        RemoveQuestMarker(tower1Marker);
    }

    //Add and Remove tower2Marker
    public void AddTower2Marker()
    {
        AddQuestMarker(tower2Marker);
    }
    public void RemoveTower2Marker()
    {
        RemoveQuestMarker(tower2Marker);
    }
}
