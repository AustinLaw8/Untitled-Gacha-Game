using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    // [SerializeField] private string sceneName;

    public void OnChangeScene(string sceneName)
    {
        if (GetComponent<Button>() != null) CardManager.cardManager.PlayButtonSFX();
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
