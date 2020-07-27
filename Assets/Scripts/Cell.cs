﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    private static Board board = null;

    // Start is called before the first frame update
    void Start()
    {
        if (board == null)
        {
            board = GetComponentInParent<Board>();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseDown()
    {
        board.OnCellSelection(gameObject);
    }
}
