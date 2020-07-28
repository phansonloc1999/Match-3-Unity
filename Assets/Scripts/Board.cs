﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int NUM_OF_ROW;
    public int NUM_OF_COLUMN;
    private float CELL_WIDTH;
    private float CELL_HEIGHT;


    public GameObject cellPrefab;
    public Sprite[] ELEMENT_SPRITES;

    [SerializeField]
    private GameObject selectedCell;


    private GameObject[,] cellsMatrix;
    private int[,] elementTypesMatrix;

    private struct CellPosition
    {
        public int row;
        public int column;

        public CellPosition(int row, int column)
        {
            this.row = row;
            this.column = column;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer renderer = cellPrefab.GetComponent<SpriteRenderer>();
        CELL_WIDTH = renderer.bounds.size.x;
        CELL_HEIGHT = renderer.bounds.size.y;

        transform.position = new Vector3(-NUM_OF_COLUMN * CELL_WIDTH / 2, NUM_OF_ROW * CELL_HEIGHT / 2, transform.position.z);

        cellsMatrix = new GameObject[NUM_OF_ROW, NUM_OF_COLUMN];
        elementTypesMatrix = new int[NUM_OF_ROW, NUM_OF_COLUMN];
        for (int row = 0; row < NUM_OF_ROW; row++)
        {
            for (int column = 0; column < NUM_OF_COLUMN; column++)
            {
                Vector3 cellPos = new Vector3(transform.position.x + CELL_WIDTH / 2 + column * CELL_WIDTH, transform.position.y - CELL_HEIGHT / 2 - row * CELL_HEIGHT, transform.position.z);
                var cellObj = Instantiate(cellPrefab, cellPos, transform.rotation, transform);
                var elementType = Random.Range(0, ELEMENT_SPRITES.Length);
                cellObj.GetComponentInChildren<Element>().setType(elementType);

                cellsMatrix[row, column] = cellObj;
                elementTypesMatrix[row, column] = elementType;
            }
        }

        selectedCell = null;
    }


    void Update()
    {
        for (int row = 0; row < NUM_OF_ROW; row++)
        {
            List<CellPosition> matches = getRowMatchedPositions(row);
            foreach (var match in matches)
            {
                var randomizedNewType = Random.Range(0, ELEMENT_SPRITES.Length);
                cellsMatrix[match.row, match.column].transform.GetChild(0).GetComponent<Element>().setType(randomizedNewType);
                elementTypesMatrix[match.row, match.column] = randomizedNewType;
            }
        }

        for (int column = 0; column < NUM_OF_COLUMN; column++)
        {
            List<CellPosition> matches = getColumnMatchedPositions(column);
            foreach (var match in matches)
            {
                var randomizedNewType = Random.Range(0, ELEMENT_SPRITES.Length);
                cellsMatrix[match.row, match.column].transform.GetChild(0).GetComponent<Element>().setType(randomizedNewType);
                elementTypesMatrix[match.row, match.column] = randomizedNewType;
            }
        }
    }

    public void onCellSelection(GameObject target)
    {
        if (selectedCell == null)
        {
            selectedCell = target;
        }
        else
        {
            int selectedRow, selectedColumn, targetRow, targetColumn;
            getCellPosition(target, out targetRow, out targetColumn);
            getCellPosition(selectedCell, out selectedRow, out selectedColumn);

            if (areNeighborCells(selectedRow, selectedColumn, targetRow, targetColumn))
            {
                var selectedElementScript = selectedCell.GetComponentInChildren<Element>();
                var targetElementScript = target.GetComponentInChildren<Element>();

                // Updating swapped element game objects' element field and SpriteRenderer
                var temp = selectedElementScript.getType();
                selectedElementScript.setType(targetElementScript.getType());
                targetElementScript.setType(temp);

                // Swapping & updating element types matrix
                elementTypesMatrix[selectedRow, selectedColumn] = elementTypesMatrix[targetRow, targetColumn];
                elementTypesMatrix[targetRow, targetColumn] = temp;
            }

            selectedCell = null;
        }
    }

    public Sprite getElementSprite(int index)
    {
        return ELEMENT_SPRITES[index];
    }

    private bool areNeighborCells(int row, int column, int row1, int column1)
    {
        return ((row == row1) && (column == column1 + 1 || column == column1 - 1)) || ((column == column1) && (row == row1 + 1 || (row == row1 - 1)));
    }

    private void getCellPosition(GameObject cell, out int outRow, out int outColumn)
    {
        for (int row = 0; row < NUM_OF_ROW; row++)
        {
            for (int column = 0; column < NUM_OF_COLUMN; column++)
            {
                if (cell == cellsMatrix[row, column])
                {
                    outRow = row;
                    outColumn = column;
                    return;
                }
            }
        }
        outRow = -1;
        outColumn = -1;
    }

    private List<CellPosition> getRowMatchedPositions(int row)
    {
        var matches = new List<CellPosition>();
        for (int column = 0; column < NUM_OF_COLUMN; column++)
        {
            int matchedElementCount = 0, i = column + 1;
            while (i < NUM_OF_COLUMN && elementTypesMatrix[row, column] == elementTypesMatrix[row, i])
            {
                matchedElementCount++;
                i++;
            }

            if (matchedElementCount >= 2)
            {
                for (int j = column; j < i; j++)
                {
                    matches.Add(new CellPosition(row, j));
                }
            }
        }
        return matches;
    }

    private List<CellPosition> getColumnMatchedPositions(int column)
    {
        var matches = new List<CellPosition>();
        for (int row = 0; row < NUM_OF_ROW; row++)
        {
            int matchedElementCount = 0, i = row + 1;
            while (i < NUM_OF_ROW && elementTypesMatrix[row, column] == elementTypesMatrix[i, column])
            {
                matchedElementCount++;
                i++;
            }

            if (matchedElementCount >= 2)
            {
                for (int j = row; j < i; j++)
                {
                    matches.Add(new CellPosition(j, column));
                }
            }
        }
        return matches;
    }
}
