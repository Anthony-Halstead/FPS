using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;


public class ButtonFunctions : MonoBehaviour
{
    public AudioMixer audioMixer;
    public GameObject AudioManager;
    public AudioManager audioManagerScript;

    private void Start()
    {
        AudioManager = GameObject.FindWithTag("AudioManager");
        audioManagerScript = AudioManager.GetComponent<AudioManager>();
    }
    public void resume()
    {
        audioManagerScript.playSFX(audioManagerScript.menuClick);
        GameManager.instance.stateUnpause();
        GameManager.instance.ToggleEnemyHealthBar();
    }

    public void restart()
    {
        audioManagerScript.playSFX(audioManagerScript.menuClick);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.instance.stateUnpause();
    }

    public void quit()
    {
        audioManagerScript.playSFX(audioManagerScript.menuClick);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();

#endif
    }

   

    public void Cancel()
    {
        audioManagerScript.playSFX(audioManagerScript.menuClick);
        GameManager.instance.BuyMenu();
        
    }

    public void OptionsButton()
    {
        audioManagerScript.playSFX(audioManagerScript.menuClick);
        GameManager.instance.Options();
    }

    public void OrderConfirm()
    {
        audioManagerScript.playSFX(audioManagerScript.menuClick);
        GameManager.instance.StoreOrder();
    }

    public void SetSensitivity(float sensitivity)
    {
        GameManager.instance.mainCameraController.sensitivity = sensitivity;
    }
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", volume);
    }
    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
    }
    public void SetSfxVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", volume);
    }
    public void ToggleEnemyHealthBarSound()
    {
        audioManagerScript.playSFX(audioManagerScript.menuClick);
    }
    public void MenuSliderSound()
    {
        audioManagerScript.playSFX(audioManagerScript.menuSlider);
    }

    public void TutorialMenu()
    {
        GameManager.instance.TutorialMenu();
    }
}
