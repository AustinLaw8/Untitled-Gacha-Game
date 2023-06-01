using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reset : MonoBehaviour
{
    [SerializeField] TeamManager script;

    void Start()
    {
        Button b = gameObject.GetComponent<Button>();
        b.onClick.AddListener(delegate () { ResetTeam(); });
    }

    public void ResetTeam()
    {
        script.ClearTeam();
    }
}
