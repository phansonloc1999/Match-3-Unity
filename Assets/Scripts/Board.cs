﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int NUM_OF_ROW;
    public int NUM_OF_COLUMN;

    public GameObject cellPrefab;


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
                Instantiate(cellPrefab, cellPos, transform.rotation, transform);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
