using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreaItem : MonoBehaviour
{
    public AreaState areaState;
    public GameObject task;
    public GameObject lockIcon;
    public Button unlockBtn;

    public void Initialize()
    {
        switch (areaState)
        {
            case AreaState.Unlocked:
                Unlock();
                break;
            case AreaState.Built:
                isAlreadyBuilt();
                break;
        }
    }

    public void Unlock()
    {
        lockIcon.SetActive(false);
        unlockBtn.gameObject.SetActive(true);
    }

    public void BuildButtonClicked()
    {
        areaState = AreaState.Built;
        lockIcon.SetActive(false);
        unlockBtn.gameObject.SetActive(false);
        task.SetActive(true);
    }

    public void isAlreadyBuilt()
    {
        task.SetActive(true);
        lockIcon.SetActive(false);
        unlockBtn.gameObject.SetActive(false);
    }
}

public enum AreaState
{
    Locked,
    Unlocked,
    Built
}
 