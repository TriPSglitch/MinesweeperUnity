using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    public Cell[] Cells;
    public GameManager gameManager;

    public int ClickCount { get; set; } = 0;
    public Cell[,] CellsMatrix { get; set; } = new Cell[10, 10];
    public int MineCount { get; set; } = 15;

    private int mineSettingsCount = 0;

    private void CreateMatrix()
    {
        #region Создание матрицы из одномерного массива
        for (int i = 0; i < Mathf.Sqrt(Cells.Length); i++)
        {
            for (int j = 0; j < Mathf.Sqrt(Cells.Length); j++)
            {
                CellsMatrix[i, j] = Cells[i * 10 + j];
            }
        }
        #endregion
    }

    public void FieldGeneration(Cell cell)
    {
        #region Генерация поля
        if (mineSettingsCount == 0)
        {
            CreateMatrix();
        }

        cell.GetCellPosition(out int cellRow, out int cellColumn);

        for (int i = 0; i < CellsMatrix.GetLength(0); i++)
        {
            int minePosition = Random.Range(0, CellsMatrix.GetLength(1));
            int startingRow = i;

            if (i == cellRow && minePosition == cellColumn)
            {
                continue;
            }

            if (CellsMatrix[i, minePosition].IsMine)
            {
                minePosition = 0;
                while (CellsMatrix[i, minePosition].IsMine && minePosition <= CellsMatrix.GetLength(1))
                {
                    if (minePosition == CellsMatrix.GetLength(1) - 1 && i == cellRow)
                    {
                        if (i == CellsMatrix.GetLength(0) - 1)
                        {
                            i = 0;
                        }
                        else
                        {
                            i++;
                        }

                        minePosition = 0;
                    }
                    minePosition++;
                }
            }

            CellsMatrix[i, minePosition].IsMine = true;
            mineSettingsCount++;

            if (mineSettingsCount == MineCount)
            {
                #region Делаем нули вокруг нажатой клетки
                List<Cell> cellsArroundList = cell.GetCellsArround();

                foreach (Cell cellItem in cellsArroundList)
                {
                    if (cellItem.IsMine)
                    {
                        cellItem.IsMine = false;
                        mineSettingsCount--;
                    }
                }
                #endregion

                break;
            }

            i = startingRow;
        }

        if (mineSettingsCount != MineCount)
        {
            FieldGeneration(cell);
        }
        #endregion
    }

    public void OpenZeroCells(Cell cell)
    {
        #region Открытие клеток с нулями
        List<Cell> cellsArroundList = cell.GetCellsArround();

        foreach (Cell cellItem in cellsArroundList)
        {
            if (cellItem.IsOpen)
            {
                continue;
            }

            cellItem.OpenCell();
        }
        #endregion
    }

    public void OpenCellsArround(Cell cell)
    {
        #region Открытие клеток вокруг
        List<Cell> cellsArroundList = cell.GetCellsArround();

        foreach (Cell cellItem in cellsArroundList)
        {
            if (cellItem.IsOpen || cellItem.IsFlagged)
            {
                continue;
            }

            if (cellItem.IsMine)
            {
                gameManager.Loss();
            }

            cellItem.OpenCell();
        }
        #endregion
    }
}