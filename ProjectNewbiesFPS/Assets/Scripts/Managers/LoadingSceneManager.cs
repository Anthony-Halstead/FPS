using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneManager : MonoBehaviour
{
    [SerializeField] Image loadingBar;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadAsynchroulsy());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator LoadAsynchroulsy()
    {
        yield return new WaitForSeconds(0.35f);
        AsyncOperation operation = SceneManager.LoadSceneAsync(PlayerPrefs.GetString("Scene to go to"));

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            loadingBar.fillAmount = progress / 100f;

            yield return null;
        }
    }
}
