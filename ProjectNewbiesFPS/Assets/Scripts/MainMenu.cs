using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioManager audioManager;
    public void Play()
    {
        audioManager.playSFX(audioManager.menuClick);
        PlayerPrefs.SetString("Scene to go to", "TaterTony");
        SceneManager.LoadScene("LoadingScene");
    }
    public void quit()
    {
        audioManager.playSFX(audioManager.menuClick);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();

#endif
    }


}
