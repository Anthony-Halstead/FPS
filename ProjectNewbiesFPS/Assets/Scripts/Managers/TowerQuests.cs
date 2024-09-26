
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerQuests : MonoBehaviour, IDamage
{
    [Header("References")]
    [SerializeField] Renderer model;
    

    [SerializeField] int HP;
    [SerializeField] int HPMax;
    [SerializeField] GameObject healthBarVis;
    [SerializeField] Image healthBar;

    Color colorOG;

    

    // Start is called before the first frame update
    void Start()
    {
        HP = HPMax;
        model.GetComponentInChildren<Renderer>();
        colorOG = model.material.color;
        healthBar.fillAmount = (float)HP;
        GameManager.instance.isTower1Dead = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int amount, Vector3 origin)
    {
       
            HP -= amount;
            healthBar.fillAmount = (float)HP / HPMax;
            StartCoroutine(DamageFlash());

            if (HP <= 0)
            {
                healthBar.fillAmount = (float)HP / HPMax;
                GameManager.instance.playerScript.money += 15;
                
                GameManager.instance.isTower1Dead = true;
                healthBarVis.SetActive(false);
            QuestManager.instance.RemoveTower1Marker();
        }
      
       
        
    }

    IEnumerator DamageFlash()
    {
        model.material.color = Color.red;
        
        yield return new WaitForSeconds(0.1f);

        model.material.color = colorOG;
    }
    
}
