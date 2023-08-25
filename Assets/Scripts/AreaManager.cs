using System.Collections.Generic;
using TMPro;
using FullSerializer;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AreaManager : MonoBehaviour
{
    public AreaItem[] areas;
    public Button levelButton;
    public TextMeshProUGUI levelButtonText;
    private int _currentLevel;
    public List<AreaState> areaProgression;
    
    private void Awake()
    {
        CheckAreaProgression();
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
                levelButtonText.text = _currentLevel.ToString();
                if (_currentLevel > 1)
                {
                    areas[_currentLevel-2].areaState = AreaState.Unlocked;
                    areas[_currentLevel-2].Initialize();
                }
            }
        }
        else
        {
            PlayerPrefs.SetInt("currentLevel" , 1);
        }
    }
    

    private void CheckAreaProgression()
    {
        if (PlayerPrefs.HasKey("areaProgression"))
        {
            areaProgression = JsonConvert.DeserializeObject<List<AreaState>>(PlayerPrefs.GetString("areaProgression"));
            for (int i = 0; i < areas.Length; i++)
            {
                areas[i].areaState = areaProgression[i];
                areas[i].Initialize();
            }
        }
    }

    private void SaveProgression()
    {
        for (int i = 0; i < areas.Length; i++)
        {
           areaProgression[i]=areas[i].areaState;
        }
        var progression = JsonConvert.SerializeObject(areaProgression);
        PlayerPrefs.SetString("areaProgression", progression);
        Debug.Log(progression);
    }

    public void GoGameScene()
    {
        SaveProgression();
        SceneManager.LoadScene(1);
    }
}
