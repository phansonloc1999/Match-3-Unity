using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Board
{
    /// <summary>
    /// Swapping 2 elements of selected neighbor cells
    /// </summary>
    private void swappingElements(GameObject targetCell)
    {
        int selectedRow, selectedColumn, targetRow, targetColumn;
        getCellPosition(targetCell, out targetRow, out targetColumn);
        getCellPosition(selectedCell, out selectedRow, out selectedColumn);

        if (areNeighborCells(selectedRow, selectedColumn, targetRow, targetColumn))
        {
            var selectedElementScript = selectedCell.GetComponentInChildren<Element>();
            var targetElementScript = targetCell.GetComponentInChildren<Element>();

            // Swapping & updating elementTypesMatrix
            var temp = selectedElementScript.getType();
            elementTypesMatrix[selectedRow, selectedColumn] = elementTypesMatrix[targetRow, targetColumn];
            elementTypesMatrix[targetRow, targetColumn] = temp;

            // Swapping & updating cellsMatrix
            cellsMatrix[selectedRow, selectedColumn] = targetCell;
            cellsMatrix[targetRow, targetColumn] = selectedCell;

            selectedCell.GetComponent<Cell>().onSwapPosTweening(targetCell.transform.position, 1);
            targetCell.GetComponent<Cell>().onSwapPosTweening(selectedCell.transform.position, 2);

            StartCoroutine(onSwappingComplete(Cell.SWAPPING_DURATION, targetCell));
        }
        else
        {
            ignoringUserInput = false;
            selectedCell = null;
        }
    }

    /// <summary>
    /// Regenerate new randomized elements at empty positions after matching
    /// </summary>
    private void regenNewElements()
    {
        for (int row = 0; row < NUM_OF_ROW; row++)
        {
            for (int column = 0; column < NUM_OF_COLUMN; column++)
            {
                if (elementTypesMatrix[row, column] == -1)
                {
                    var randomElementType = Random.Range(0, ELEMENT_SPRITES.Length);
                    elementTypesMatrix[row, column] = randomElementType;
                    cellsMatrix[row, column].GetComponentInChildren<Element>().setType(randomElementType);

                    cellsMatrix[row, column].GetComponent<Cell>().onRegenNewElement();
                }
            }
        }

        var totalMatches = getTotalMatchedPositions();
        // If board has generated new matches
        if (totalMatches.Count > 0) StartCoroutine(shiftDownAndRegenElements(BETWEEN_SWAP_AND_REGEN_INTERVAL, totalMatches));
        else
        {
            ignoringUserInput = false;
        }
    }

    /// <summary>
    /// Shifting down and regenerating elements
    /// </summary>
    /// <param name="time"></param>
    /// <param name="totalMatches"></param>
    /// <returns></returns>
    private IEnumerator shiftDownAndRegenElements(float time, List<CellPosition> totalMatches)
    {
        yield return new WaitForSeconds(time);

        // Set all matches' element type to -1
        foreach (var match in totalMatches)
        {
            cellsMatrix[match.row, match.column].transform.GetChild(0).GetComponent<Element>().setType(-1);
            elementTypesMatrix[match.row, match.column] = -1;
        }

        shiftElementsDown();

        regenNewElements();
    }

    /// <summary>
    /// Search & update changed element in elementTypesMatrix
    /// </summary>
    /// <param name="changedCell"></param>
    /// <param name="changedCellElementType"></param>
    public void updateElementTypesMatrix(GameObject changedCell, int changedCellElementType)
    {
        for (int row = 0; row < NUM_OF_ROW; row++)
        {
            for (int column = 0; column < NUM_OF_COLUMN; column++)
            {
                if (changedCell == cellsMatrix[row, column])
                {
                    elementTypesMatrix[row, column] = changedCellElementType;
                    return;
                }
            }
        }
    }


    /// <summary>
    /// Shifting elements down
    /// </summary>
    private void shiftElementsDown()
    {
        for (int row = NUM_OF_ROW - 2; row >= 0; row--)
        {
            for (int column = 0; column < NUM_OF_COLUMN; column++)
            {
                if (elementTypesMatrix[row, column] != -1)
                {
                    int shiftedRow = row;
                    bool shifted = false;
                    while (shiftedRow <= NUM_OF_ROW - 2 && elementTypesMatrix[shiftedRow + 1, column] == -1)
                    {
                        shifted = true;

                        elementTypesMatrix[shiftedRow + 1, column] = elementTypesMatrix[shiftedRow, column];
                        elementTypesMatrix[shiftedRow, column] = -1;
                        cellsMatrix[shiftedRow, column].GetComponentInChildren<Element>().setType(-1);
                        cellsMatrix[shiftedRow + 1, column].GetComponentInChildren<Element>().setType(elementTypesMatrix[shiftedRow + 1, column]);
                        shiftedRow++;
                    }

                    // Only set element's position to if element is shifted
                    if (shifted)
                    {
                        var currentCellObj = cellsMatrix[shiftedRow, column];
                        var shiftStartPos = cellsMatrix[row, column].transform.position;
                        currentCellObj.GetComponentInChildren<Cell>().onShiftingDown(shiftStartPos);
                    }
                }
            }
        }

        // Disable empty cell objects to make them invisible until shifting down is done
        for (int row = NUM_OF_ROW - 2; row >= 0; row--)
        {
            for (int column = 0; column < NUM_OF_COLUMN; column++)
            {
                if (elementTypesMatrix[row, column] == -1)
                {
                    cellsMatrix[row, column].SetActive(false);
                    StartCoroutine(onShiftingDownComplete(CELL_SHIFTING_DOWN_DURATION, cellsMatrix[row, column]));
                }
            }
        }
    }
}