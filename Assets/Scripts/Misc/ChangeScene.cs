using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    // [SerializeField] private string sceneName;

    private IEnumerator Load(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
    public void OnChangeScene(string sceneName)
    {
        if (GetComponent<Button>() != null) CardManager.cardManager.PlayButtonSFX();
        StartCoroutine(Load(sceneName));
    }
}
