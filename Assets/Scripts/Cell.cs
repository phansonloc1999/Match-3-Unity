using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    static Board boardScript = null;

    public int row;
    public int column;

    // Start is called before the first frame update
    void Start()
    {
        if (boardScript == null)
        {
            boardScript = GameObject.Find("Board").GetComponent<Board>();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseDown()
    {
        boardScript.onCellSelection(gameObject);
    }

    public void savePositionInBoard(int row, int column)
    {
        this.row = row;
        this.column = column;
    }
}
