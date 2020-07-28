using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int NUM_OF_ROW;
    public int NUM_OF_COLUMN;

    public GameObject cellPrefab;

    public Sprite[] ELEMENT_SPRITES;

    [SerializeField]
    private GameObject selectedCell;

    public GameObject[,] cellsMatrix;

    private float CELL_WIDTH;
    private float CELL_HEIGHT;

    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer renderer = cellPrefab.GetComponent<SpriteRenderer>();
        CELL_WIDTH = renderer.bounds.size.x;
        CELL_HEIGHT = renderer.bounds.size.y;

        transform.position = new Vector3(-NUM_OF_COLUMN * CELL_WIDTH / 2, NUM_OF_ROW * CELL_HEIGHT / 2, transform.position.z);

        cellsMatrix = new GameObject[NUM_OF_ROW, NUM_OF_COLUMN];
        for (int row = 0; row < NUM_OF_ROW; row++)
        {
            for (int column = 0; column < NUM_OF_COLUMN; column++)
            {
                Vector3 cellPos = new Vector3(transform.position.x + CELL_WIDTH / 2 + column * CELL_WIDTH, transform.position.y - CELL_HEIGHT / 2 - row * CELL_HEIGHT, transform.position.z);
                var cellObj = Instantiate(cellPrefab, cellPos, transform.rotation, transform);
                var elementType = Random.Range(0, ELEMENT_SPRITES.Length);
                cellObj.GetComponentInChildren<Element>().setType(elementType);

                cellsMatrix[row, column] = cellObj;
            }
        }

        selectedCell = null;
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

                var temp = selectedElementScript.getType();
                selectedElementScript.setType(targetElementScript.getType());
                targetElementScript.setType(temp);
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
}
