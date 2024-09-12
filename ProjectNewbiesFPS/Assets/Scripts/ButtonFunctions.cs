using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;


public class ButtonFunctions : MonoBehaviour
{
    public AudioMixer audioMixer;
    public AudioManager audioManager;
    public void resume()
    {
        audioManager.playSFX(audioManager.menuClick);
        GameManager.instance.stateUnpause();
        GameManager.instance.ToggleEnemyHealthBar();
    }

    public void restart()
    {
        audioManager.playSFX(audioManager.menuClick);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.instance.stateUnpause();
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

   

    public void Cancel()
    {
        audioManager.playSFX(audioManager.menuClick);
        GameManager.instance.BuyMenu();
        
    }

    public void OptionsButton()
    {
        audioManager.playSFX(audioManager.menuClick);
        GameManager.instance.Options();
    }

    public void OrderConfirm()
    {
        audioManager.playSFX(audioManager.menuClick);
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
        audioManager.playSFX(audioManager.menuClick);
    }
    public void MenuSliderSound()
    {
        audioManager.playSFX(audioManager.menuSlider);
    }

    public void RefillAmmoToggle()
    {

    }
}
