using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] private GameObject load;
    
    private IEnumerator TryLoadNext(string sceneName)
    {
        load.SetActive(true);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        asyncLoad.allowSceneActivation = false;
        while (!asyncLoad.isDone)
        {
            if(asyncLoad.progress >= .9f) asyncLoad.allowSceneActivation = true;
            yield return null;
        }
    }

    public void OnChangeScene(string sceneName)
    {
        load.SetActive(true);
        if (GetComponent<Button>() != null) CardManager.cardManager.PlayButtonSFX();
        StartCoroutine(TryLoadNext(sceneName));
    }
}
