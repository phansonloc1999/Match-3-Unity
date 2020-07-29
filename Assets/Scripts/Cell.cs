using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Cell : MonoBehaviour
{
    [SerializeField]
    private const float SWAPPING_DURATION = 1.0f;

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


    public void onSwapPosTweening(Vector3 endPos)
    {
        transform.DOMove(endPos, SWAPPING_DURATION);
    }
}
