using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;


public class ButtonFunctions : MonoBehaviour
{
    public AudioMixer audioMixer;
    public void resume()
    {
        GameManager.instance.stateUnpause();
        GameManager.instance.ToggleEnemyHealthBar();
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

   

    public void Cancel()
    {
        GameManager.instance.BuyMenu();
        
    }

    public void OptionsButton()
    {
        GameManager.instance.Options();
    }

    public void OrderConfirm()
    {
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
        audioMixer.SetFloat("SfxVolume", volume);
    }
}
