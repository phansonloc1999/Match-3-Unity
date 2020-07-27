using System.Collections;
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
        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = board.ELEMENT_SPRITES[Random.Range(0, board.ELEMENT_SPRITES.Length)];
    }

    // Update is called once per frame
    void Update()
    {

    }
}
