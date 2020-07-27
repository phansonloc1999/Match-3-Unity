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

    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer renderer = cellPrefab.GetComponent<SpriteRenderer>();
        var cellWidth = renderer.bounds.size.x;
        var cellHeight = renderer.bounds.size.y;

        transform.position = new Vector3(-NUM_OF_COLUMN * cellWidth / 2, NUM_OF_ROW * cellHeight / 2, transform.position.z);

        for (int row = 0; row < NUM_OF_ROW; row++)
        {
            for (int column = 0; column < NUM_OF_COLUMN; column++)
            {
                Vector3 cellPos = new Vector3(transform.position.x + cellWidth / 2 + column * cellWidth, transform.position.y - cellHeight / 2 - row * cellHeight, transform.position.z);
                var cellObj = Instantiate(cellPrefab, cellPos, transform.rotation, transform);
                var elementType = Random.Range(0, ELEMENT_SPRITES.Length);
                cellObj.GetComponentInChildren<Element>().setType(elementType);
                cellObj.GetComponent<Cell>().savePositionInBoard(row, column);
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
            var selectedRow = selectedCell.GetComponent<Cell>().row;
            var selectedColumn = selectedCell.GetComponent<Cell>().column;
            var targetRow = target.GetComponent<Cell>().row;
            var targetColumn = target.GetComponent<Cell>().column;

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
}
