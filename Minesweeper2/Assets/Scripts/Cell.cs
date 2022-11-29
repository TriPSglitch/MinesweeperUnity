using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerClickHandler
{
    public Field field;
    public GameManager gameManager;
    public GameObject flagsCount;
    public Sprite flagSprite;

    public bool IsMine { get; set; } = false;
    public bool IsOpen { get; set; } = false;
    public bool IsFlagged { get; set; } = false;

    private int mineArround = 0;
    private Sprite withoutFlagSprite;

    public void Start()
    {
        withoutFlagSprite = this.GetComponent<Image>().sprite;
    }

    public void ClickOnCell()
    {
        #region Нажатие на клетку левой кнопкой мыши

        #region Генерация поля после первого нажатия на любую клетку
        if (field.ClickCount == 0)
        {
            field.ClickCount++;
            field.FieldGeneration(this);

            for (int i = 0; i < field.CellsMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < field.CellsMatrix.GetLength(1); j++)
                {
                    field.CellsMatrix[i, j].MineAroundCounter();
                }
            }
        }
        #endregion

        if (gameManager.IsGameEnd)
        {
            return;
        }

        if (gameManager.IsFlagMode)
        {
            this.SetFlag();
            return;
        }

        if (!this.IsMine && !this.IsFlagged)
        {
            if (this.IsOpen)
            {
                if (this.mineArround == this.GetCountFlaggedCellsArround())
                {
                    field.OpenCellsArround(this);
                }
            }
        }

        if (!this.IsFlagged)
        {
            OpenCell();
        }
        #endregion
    }

    private void MineAroundCounter()
    {
        #region Подсчёт количества мин вокруг
        List<Cell> cellsArroundList = this.GetCellsArround();

        foreach (Cell cellItem in cellsArroundList)
        {
            if (cellItem.IsMine)
            {
                this.mineArround++;
            }
        }

        this.TextColorChanger();
        #endregion
    }

    public List<Cell> GetCellsArround()
    {
        #region Получение клеток вокруг
        List<Cell> cellsArroundList = new List<Cell>();

        this.GetCellPosition(out int cellRow, out int cellColumn);

        if (cellRow == 0)
        {
            if (cellColumn == 0)
            {
                for (int i = cellRow; i <= cellRow + 1; i++)
                {
                    for (int j = cellColumn; j <= cellColumn + 1; j++)
                    {
                        cellsArroundList.Add(field.CellsMatrix[i, j]);
                    }
                }
            }
            else if (cellColumn == 9)
            {
                for (int i = cellRow; i <= cellRow + 1; i++)
                {
                    for (int j = cellColumn - 1; j <= cellColumn; j++)
                    {
                        cellsArroundList.Add(field.CellsMatrix[i, j]);
                    }
                }
            }
            else
            {
                for (int i = cellRow; i <= cellRow + 1; i++)
                {
                    for (int j = cellColumn - 1; j <= cellColumn + 1; j++)
                    {
                        cellsArroundList.Add(field.CellsMatrix[i, j]);
                    }
                }
            }
        }
        else if (cellRow == 9)
        {
            if (cellColumn == 0)
            {
                for (int i = cellRow - 1; i <= cellRow; i++)
                {
                    for (int j = cellColumn; j <= cellColumn + 1; j++)
                    {
                        cellsArroundList.Add(field.CellsMatrix[i, j]);
                    }
                }
            }
            else if (cellColumn == 9)
            {
                for (int i = cellRow - 1; i <= cellRow; i++)
                {
                    for (int j = cellColumn - 1; j <= cellColumn; j++)
                    {
                        cellsArroundList.Add(field.CellsMatrix[i, j]);
                    }
                }
            }
            else
            {
                for (int i = cellRow - 1; i <= cellRow; i++)
                {
                    for (int j = cellColumn - 1; j <= cellColumn + 1; j++)
                    {
                        cellsArroundList.Add(field.CellsMatrix[i, j]);
                    }
                }
            }
        }
        else
        {
            if (cellColumn == 0)
            {
                for (int i = cellRow - 1; i <= cellRow + 1; i++)
                {
                    for (int j = cellColumn; j <= cellColumn + 1; j++)
                    {
                        cellsArroundList.Add(field.CellsMatrix[i, j]);
                    }
                }
            }
            else if (cellColumn == 9)
            {
                for (int i = cellRow - 1; i <= cellRow + 1; i++)
                {
                    for (int j = cellColumn - 1; j <= cellColumn; j++)
                    {
                        cellsArroundList.Add(field.CellsMatrix[i, j]);
                    }
                }
            }
            else
            {
                for (int i = cellRow - 1; i <= cellRow + 1; i++)
                {
                    for (int j = cellColumn - 1; j <= cellColumn + 1; j++)
                    {
                        cellsArroundList.Add(field.CellsMatrix[i, j]);
                    }
                }
            }
        }

        return cellsArroundList;
        #endregion
    }

    public void GetCellPosition(out int cellRow, out int cellColumn)
    {
        #region Получение позиции клетки
        cellRow = 0;
        cellColumn = 0;

        for (int i = 0; i < field.CellsMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < field.CellsMatrix.GetLength(1); j++)
            {
                if (field.CellsMatrix[i, j] == this)
                {
                    cellRow = i;
                    cellColumn = j;
                    return;
                }
            }
        }
        #endregion
    }

    public void OpenCell()
    {
        #region Открытие клетки
        if (this.IsMine && !this.IsFlagged)
        {
            gameManager.Loss();
            return;
        }

        this.IsOpen = true;
        this.GetComponentInChildren<Text>().text = Convert.ToString(this.mineArround);
        this.GetComponent<Image>().color = Color.white;

        if (this.mineArround == 0)
        {
            this.GetComponentInChildren<Text>().text = "";

            field.OpenZeroCells(this);
        }
        #endregion
    }

    private int GetCountFlaggedCellsArround()
    {
        #region Подсчёт отмеченных клеток вокруг
        int countFlaggedCellsArround = 0;

        List<Cell> cellsArroundList = this.GetCellsArround();

        foreach (Cell cellItem in cellsArroundList)
        {
            if (cellItem.IsFlagged)
            {
                countFlaggedCellsArround++;
            }
        }

        return countFlaggedCellsArround;
        #endregion
    }

    private void TextColorChanger()
    {
        #region Установка цвета текста клетки
        if (this.mineArround == 1)
        {
            this.GetComponentInChildren<Text>().color = new Color(0, 0, 255);
        }
        else if (this.mineArround == 2)
        {
            this.GetComponentInChildren<Text>().color = new Color(0, 100, 0);
        }
        else if (this.mineArround == 3)
        {
            this.GetComponentInChildren<Text>().color = new Color(255, 0, 0);
        }
        else if (this.mineArround == 4)
        {
            this.GetComponentInChildren<Text>().color = new Color(0, 0, 139);
        }
        else if (this.mineArround == 5)
        {
            this.GetComponentInChildren<Text>().color = new Color(139, 0, 0);
        }
        else if (this.mineArround == 6)
        {
            this.GetComponentInChildren<Text>().color = new Color(0, 50, 0);
        }
        #endregion
    }

    private void SetFlag()
    {
        #region Установка флага
        if (!this.IsOpen)
        {
            this.IsFlagged = this.IsFlagged ? false : true;
            this.GetComponent<Image>().sprite = this.IsFlagged ? flagSprite : withoutFlagSprite;

            gameManager.SettedFlagsCount = this.IsFlagged ? ++gameManager.SettedFlagsCount : --gameManager.SettedFlagsCount;

            if (this.IsFlagged)
            {
                if (this.IsMine)
                {
                    gameManager.MineCellsFlaggedCount++;
                }
                else
                {
                    gameManager.OtherCellsFlaggedCount++;
                }
            }
            else
            {
                if (this.IsMine)
                {
                    gameManager.MineCellsFlaggedCount--;
                }
                else
                {
                    gameManager.OtherCellsFlaggedCount--;
                }
            }

            flagsCount.GetComponent<Text>().text = Convert.ToString(gameManager.SettedFlagsCount);

            if (gameManager.MineCellsFlaggedCount == field.MineCount && gameManager.OtherCellsFlaggedCount == 0)
            {
                gameManager.Win();
            }
        }
        #endregion
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right && !gameManager.IsGameEnd)
        {
            this.SetFlag();
        }
    }
}