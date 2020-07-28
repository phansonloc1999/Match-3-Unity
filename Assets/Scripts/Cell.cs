using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    static Board boardScript = null;


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
}
