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
    /// Regenerate new randomized elements at removed positions after matching
    /// </summary>
    private void regenRemovedElements()
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

                    cellsMatrix[row, column].GetComponent<Cell>().onRegenFallDownElement();
                }
            }
        }
    }

    /// <summary>
    /// Regenerating all board elements
    /// </summary>
    private void regenAllElements()
    {
        for (int row = 0; row < NUM_OF_ROW; row++)
        {
            for (int column = 0; column < NUM_OF_COLUMN; column++)
            {
                var randomElementType = Random.Range(0, ELEMENT_SPRITES.Length);
                elementTypesMatrix[row, column] = randomElementType;
                cellsMatrix[row, column].GetComponentInChildren<Element>().setType(randomElementType);
            }
        }
    }

    /// <summary>
    /// Clear matches in game board
    /// </summary>
    /// <param name="totalMatches"></param>
    private void clearMatches(List<CellPosition> totalMatches)
    {
        // Set all matches' element type to -1
        foreach (var match in totalMatches)
        {
            cellsMatrix[match.row, match.column].transform.GetChild(0).GetComponent<Element>().setType(-1);
            elementTypesMatrix[match.row, match.column] = -1;
        }
    }

    /// <summary>
    /// Processing board elements: clearing, shifting down and regenerating board elements
    /// </summary>
    /// <param name="executeAfterTime"></param>
    /// <param name="totalMatches"></param>
    /// <returns></returns>
    private IEnumerator processBoardElements(float executeAfterTime, List<CellPosition> totalMatches)
    {
        yield return new WaitForSeconds(executeAfterTime);

        clearMatches(totalMatches);

        shiftElementsDown();

        yield return new WaitForSeconds(CELL_SHIFTING_DOWN_DURATION); // Wait for cells & elements shifting down to be finished

        regenRemovedElements();

        totalMatches = getTotalMatchedPositions();
        // If board has generated new matches, start this processBoardMatches() over again
        if (totalMatches.Count > 0)
        {
            StartCoroutine(processBoardElements(LOOP_PROCESSING_AFTER_REGEN_REMOVED, totalMatches));
            yield break;
        }

        // Otherwise...
        /// If board has 0 matches and generated 0 potential matches, regenerate all of its elements until it has some matches or potential matches
        yield return new WaitForSeconds(BETWEEN_REGEN_REMOVED_AND_REGEN_ALL_INTERVAL);
        while (totalMatches.Count == 0 && !hasPotentialMatches())
        {
            regenAllElements();
            totalMatches = getTotalMatchedPositions();
        }

        /// If board has some matches afterwards, loop back processBoardMatches() 
        if (totalMatches.Count > 0)
        {
            StartCoroutine(processBoardElements(LOOP_PROCESSING_AFTER_0_POTENTIAL_MATCHES_INTERVAL, totalMatches));
            yield break;
        }

        /// Return input control to user
        ignoringUserInput = false;
    }

    /// <summary>
    /// Search & update changed element in elementTypesMatrix
    /// </summary>
    /// <param name="changedCell"></param>
    /// <param name="changedCellElementType"></param>
    public void changeElementTypeInMatrix(GameObject changedCell, int changedCellElementType)
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
    /// Shifting elements down for Board.CELL_SHIFTING_DOWN_DURATION seconds
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

    /// <summary>
    /// Prevent initial matches at the start of the game
    /// </summary>
    private void preventInitialMatches()
    {
        var totalMatches = getTotalMatchedPositions();

        while (totalMatches.Count > 0)
        {
            foreach (var match in totalMatches)
            {
                var newType = Random.Range(0, ELEMENT_SPRITES.Length);
                elementTypesMatrix[match.row, match.column] = newType;
                cellsMatrix[match.row, match.column].GetComponentInChildren<Element>().setType(newType);
            }

            totalMatches = getTotalMatchedPositions();
        }
    }
}