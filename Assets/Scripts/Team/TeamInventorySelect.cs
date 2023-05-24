using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamInventorySelect : MonoBehaviour
{
    //[SerializeField] private CardSO card;
    private TeamInventorySelected script;

    // Start is called before the first frame update
    void Start()
    {
        Button b = gameObject.GetComponent<Button>();
        b.onClick.AddListener(delegate () { OnCardSlotSelected(); });
    }

    public void OnCardSlotSelected()
    {
        // send message to TeamInventorySelected script
        int id = gameObject.GetComponent<CardIDIdentifier>().cardID;
        if (GameObject.Find("TeamData").GetComponent<TeamManager>().InTeam(id)) return;
        script = FindObjectOfType<TeamInventorySelected>();
        script.OnCardSelected(id);
    }
}
