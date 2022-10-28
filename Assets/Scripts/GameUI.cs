using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI timeText;
    public GameObject win;
    public TextMeshProUGUI winText;

    public static GameUI instance;
    void Awake()
    {
        instance = this;
    }

    public void UpdateGoldText(int gold)
    {
        goldText.text = "<b>Players Left:</b> " + gold;
    }

    public void UpdateTime(float time, bool shrunk)
    {
        if(!shrunk)
            timeText.text = "<b>Time Until Stage Shrinks:</b> " + time;
        else
            timeText.text = "<b>No more shrinking! Fight!</b>";
    }

    public void SetWinText(string playerName)
    {
        win.SetActive(true);
        winText.text = playerName + " Wins!";
    }

    public void SetTieText()
    {
        win.SetActive(true);
        winText.text ="Nobody Wins!";
    }
}
