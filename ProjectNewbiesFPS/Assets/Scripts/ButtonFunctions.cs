using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    public void resume()
    {
        GameManager.instance.stateUnpause();
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.instance.stateUnpause();
    }

    public void quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();

#endif
    }

    public void BuyHealth()
    {
        if(GameManager.instance.playerScript.money >= 20)
        {
            GameManager.instance.playerScript.money -= 20;
            GameManager.instance.playerScript.HP += 20;
        }
    }

    public void BuyMagazineUpgrade()
    {
        if(GameManager.instance.playerScript.money >= 30)
        {
            GameManager.instance.playerScript.money -= 30;
            GameManager.instance.projectilesScript.magazineSize += 30;
        }
    }

    public void Cancel()
    {
        GameManager.instance.BuyMenu();
    }
}
