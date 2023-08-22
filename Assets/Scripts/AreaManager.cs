using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AreaManager : MonoBehaviour
{
    public AreaItem[] areas;
    public List<bool> areaProgression;
    public Button levelButton;
    public TextMeshProUGUI levelButtonText;
    private int _currentLevel=1;
    private void Awake()
    {
        CheckLevel();
    }

    private void CheckLevel()
    {
        if (PlayerPrefs.HasKey("currentLevel"))
        {
            _currentLevel = PlayerPrefs.GetInt("currentLevel");
            if (_currentLevel>10)
            {
                levelButtonText.text = "Finished";
                levelButton.interactable = false;
            }
            else
            {
                if (_currentLevel>1)
                {
                    areaProgression[_currentLevel - 1] = true;
                }
                levelButtonText.text = _currentLevel.ToString();
                areas[_currentLevel].Unlock();
            }
        }
        else
        {
            PlayerPrefs.SetInt("currentLevel" , 1);
        }
    }
    
}
