using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    [SerializeField] GameObject gateA;
    [SerializeField] GameObject gateB;
    Animator animA;
    Animator animB;
    // Start is called before the first frame update
    void Start()
    {
        gateA = GameObject.FindWithTag("GateA");
        gateB = GameObject.FindWithTag("GateB");
        animA = gateA.GetComponent<Animator>();
        animB = gateB.GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && GameManager.instance.redKeyFound)
        {
            GameManager.instance.redkeySpriteImage.SetActive(false);
            GameManager.instance.objectivesText.text = "OBJECTIVE: Find Green Key";
            GameManager.instance.playerSpawnPos.transform.position = transform.position;
            StartCoroutine(CheckpointPopup());
            animA.SetTrigger("OpenGate");
            animB.SetTrigger("OpenGate");
        }
        else if (other.CompareTag("Player") && GameManager.instance.greenKeyFound)
        {
            GameManager.instance.greenkeySpriteImage.SetActive(false);
            GameManager.instance.objectivesText.text = "OBJECTIVE: Defeat The Boss";
            GameManager.instance.playerSpawnPos.transform.position = transform.position;
            StartCoroutine(CheckpointPopup());
        }
    }

    IEnumerator CheckpointPopup()
    {
        GameManager.instance.checkpointPopUp.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        GameManager.instance.checkpointPopUp.SetActive(false);
    }
}
