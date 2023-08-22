using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private void Awake()
    {
        DontDestroyOnLoad(this);
        Instance = this;
    }

    public void Win()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void Lose()
    {
        
    }
    public void GoGameScene()
    {
        SceneManager.LoadSceneAsync(1);
    }
}
