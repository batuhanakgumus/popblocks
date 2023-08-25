using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public int goalAmount;
    public CubeType type;
    public Sprite[] possibleSprites;
    public UnityEngine.UI.Image image;
    public TextMeshProUGUI goalText;
    public GameObject completed;


    public void SetGoal(CubeType type, int goalAmount)
    {
        this.type = type;
        this.goalAmount = goalAmount;
        goalText.text = goalAmount.ToString();
        GetType();
        completed.SetActive(false);
    }

    private new void GetType()
    {
        if (type == CubeType.bo)
        {
            image.sprite = possibleSprites[0];
        }
        if (type == CubeType.s)
        {
            image.sprite = possibleSprites[1];

        }
        if (type == CubeType.v)
        {
            image.sprite = possibleSprites[2];

        }
    }

    public bool SetAmountAndAndCheckGoal(int amount)
    {
        goalAmount= amount;
        goalText.text = goalAmount.ToString();
        if (goalAmount<=0)
        {
            goalText.enabled = false;
            completed.SetActive(true);
            return true;
        }
        return false;
    }
}
