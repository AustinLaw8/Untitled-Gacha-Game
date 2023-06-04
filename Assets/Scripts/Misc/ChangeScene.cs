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
        if (load) load.SetActive(true);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void OnChangeScene(string sceneName)
    {
        if (GetComponent<Button>() != null) CardManager.cardManager.PlayButtonSFX();
        StartCoroutine(TryLoadNext(sceneName));
    }
}
