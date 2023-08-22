using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreaItem : MonoBehaviour
{
    public GameObject task;
    public GameObject lockIcon;
    public Button unlockBtn;

    public void Unlock()
    {
        lockIcon.SetActive(false);
        unlockBtn.gameObject.SetActive(true);
    }

    public void BuildButtonClicked()
    {
        lockIcon.SetActive(false);
        unlockBtn.gameObject.SetActive(false);
        task.SetActive(true);
    }
}
 