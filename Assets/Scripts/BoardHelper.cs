using System.Collections.Generic;
using UnityEngine;

public partial class Board
{
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

    List<CellPosition> getTotalMatchedPositions()
    {
        var totalMatches = new List<CellPosition>();

        for (int row = 0; row < NUM_OF_ROW; row++)
        {
            List<CellPosition> matches = getRowMatchedPositions(row);
            foreach (var match in matches)
            {
                totalMatches.Add(match);
            }
        }

        for (int column = 0; column < NUM_OF_COLUMN; column++)
        {
            List<CellPosition> matches = getColumnMatchedPositions(column);
            foreach (var match in matches)
            {
                totalMatches.Add(match);
            }
        }

        return totalMatches;
    }

    public static bool isIgnoringUserInput()
    {
        return ignoringUserInput;
    }

    public static Board getInstance()
    {
        return instance;
    }

    public int getElementType(int row, int column)
    {
        if (row >= 0 && row < NUM_OF_ROW && column >= 0 && column < NUM_OF_COLUMN)
            return elementTypesMatrix[row, column];
        return -1;
    }

    public bool hasPotentialMatches()
    {
        for (int row = 0; row < NUM_OF_ROW; row++)
        {
            for (int column = 0; column < NUM_OF_COLUMN; column++)
            {
                if (
                    (getElementType(row, column) == getElementType(row + 2, column) &&
                        getElementType(row + 2, column) != -1) &&
                    (getElementType(row, column) == getElementType(row + 1, column - 1) ||
                        getElementType(row, column) == getElementType(row + 1, column + 1))
                )
                    return true;

                if (
                    (getElementType(row, column) == getElementType(row, column + 2) &&
                        getElementType(row, column + 2) != -1) &&
                    (getElementType(row, column) == getElementType(row - 1, column + 1) ||
                        getElementType(row, column) == getElementType(row + 1, column + 1))
                )
                    return true;

                if ((getElementType(row, column) == getElementType(row, column + 1) && getElementType(row, column + 1) != -1) &&
                    (getElementType(row, column - 2) == getElementType(row, column) || getElementType(row, column + 3) == getElementType(row, column) ||
                        getElementType(row + 1, column - 1) == getElementType(row, column) ||
                        getElementType(row - 1, column - 1) == getElementType(row, column) ||
                        getElementType(row + 1, column + 2) == getElementType(row, column) ||
                        getElementType(row - 1, column + 2) == getElementType(row, column)))
                    return true;

                if (getElementType(row, column) == getElementType(row + 1, column) &&
                    (getElementType(row, column) == getElementType(row + 3, column) ||
                        getElementType(row, column) == getElementType(row - 2, column)))
                    return true;
            }
        }
        return false;
    }
}