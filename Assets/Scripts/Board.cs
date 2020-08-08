using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

struct CellPosition
{
    public int row;
    public int column;

    public CellPosition(int row, int column)
    {
        this.row = row;
        this.column = column;
    }
}

public partial class Board : MonoBehaviour
{
    public int NUM_OF_ROW;
    public int NUM_OF_COLUMN;
    private static float CELL_WIDTH;
    private static float CELL_HEIGHT;
    private const float BOARD_Y = -20f;
    private const float SWAP_COMPLETE_PROCESS_BOARD_INTERVAL = 0.2f;
    public const float CELL_SHIFTING_DOWN_DURATION = 0.7f;
    public const float BETWEEN_REGEN_REMOVED_AND_REGEN_ALL_INTERVAL = 1.0f;
    public const float LOOP_PROCESSING_AFTER_0_POTENTIAL_MATCHES_INTERVAL = 1.0f;
    public const float LOOP_PROCESSING_AFTER_REGEN_REMOVED = 0.7f;
    private static bool ignoringUserInput = false;

    public GameObject cellPrefab;
    public Sprite[] ELEMENT_SPRITES;

    [SerializeField]
    private GameObject selectedCell;

    private GameObject[,] cellsMatrix;
    private int[,] elementTypesMatrix;

    private static Board instance;

    void Start()
    {
        instance = this;

        SpriteRenderer renderer = cellPrefab.GetComponent<SpriteRenderer>();
        CELL_WIDTH = renderer.bounds.size.x;
        CELL_HEIGHT = renderer.bounds.size.y;

        transform.position = new Vector3(-NUM_OF_COLUMN * CELL_WIDTH / 2, BOARD_Y + NUM_OF_ROW * CELL_HEIGHT / 2, transform.position.z);

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

        preventInitialMatches();

        while (!hasPotentialMatches())
        {
            regenAllElements();
        }
    }

    public void onCellSelection(GameObject targetCell)
    {
        if (selectedCell == null)
        {
            selectedCell = targetCell;
        }
        else
        {
            ignoringUserInput = true;
            swappingElements(targetCell);
        }
    }

    private IEnumerator onSwappingComplete(float time, GameObject targetCell)
    {
        yield return new WaitForSeconds(time);

        var totalMatches = getTotalMatchedPositions();

        if (totalMatches.Count > 0)
        {
            StartCoroutine(processBoardElements(SWAP_COMPLETE_PROCESS_BOARD_INTERVAL, totalMatches));
        }
        else // Swapping 2 elements back to their original position if no new match is found
        {
            int selectedRow, selectedColumn, targetRow, targetColumn;
            getCellPosition(targetCell, out targetRow, out targetColumn);
            getCellPosition(selectedCell, out selectedRow, out selectedColumn);

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

            yield return new WaitForSeconds(Cell.SWAPPING_DURATION);

            ignoringUserInput = false;
        }

        selectedCell = null;
    }

    private IEnumerator onShiftingDownComplete(float time, GameObject cell)
    {
        yield return new WaitForSeconds(time);

        cell.SetActive(true);
    }
}