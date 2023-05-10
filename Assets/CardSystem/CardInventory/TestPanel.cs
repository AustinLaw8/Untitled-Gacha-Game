using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPanel : MonoBehaviour
{
    public void OpenProfile()
    {
        ModalManager.instance.ShowModal("Modal Header");
    }
}
