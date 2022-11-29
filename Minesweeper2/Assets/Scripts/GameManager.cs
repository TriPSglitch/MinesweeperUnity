using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject mineCount;
    public GameObject result;
    public Field field;
    public Sprite mineSprite;

    public int MineCellsFlaggedCount { get; set; } = 0;
    public int OtherCellsFlaggedCount { get; set; } = 0;
    public int SettedFlagsCount { get; set; } = 0;
    public bool IsFlagMode { get; set; } = false;
    public bool IsGameEnd { get; set; } = false;

    public void Start()
    {
        mineCount.GetComponent<Text>().text = Convert.ToString(field.MineCount);
    }

    public void Win()
    {
        result.GetComponent<Text>().text = "Вы победили!";
        this.IsGameEnd = true;
    }

    public void Loss()
    {
        field.Cells.Where(item => item.IsMine && !item.IsFlagged).ToList().ForEach(item => item.GetComponent<Image>().sprite = mineSprite);

        field.Cells.Where(item => !item.IsMine && item.IsFlagged).ToList().ForEach(item => item.GetComponent<Image>().color = Color.black);

        result.GetComponent<Text>().text = "Вы проиграли!";
        this.IsGameEnd = true;
    }
}