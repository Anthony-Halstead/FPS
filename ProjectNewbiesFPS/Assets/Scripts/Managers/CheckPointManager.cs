using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && GameManager.instance.redKeyFound)
        {
            GameManager.instance.redkeySpriteImage.SetActive(false);
            GameManager.instance.objectivesText.text = "OBJECTIVE: Find Blue Key";
            GameManager.instance.playerSpawnPos.transform.position = transform.position;
        }
        
    }

    IEnumerator CheckpointPopup()
    {
        GameManager.instance.checkpointPopUp.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        GameManager.instance.checkpointPopUp.SetActive(false);
    }
}
