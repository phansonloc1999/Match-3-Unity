using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Cell : MonoBehaviour
{
    public static float SWAPPING_DURATION = 1.0f;

    static Board boardScript = null;

    private Element elementScript;

    // Start is called before the first frame update
    void Start()
    {
        if (boardScript == null)
        {
            boardScript = GameObject.Find("Board").GetComponent<Board>();
        }
        elementScript = GetComponentInChildren<Element>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseDown()
    {
        boardScript.onCellSelection(gameObject);
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            var oldType = elementScript.getType();
            var newType = oldType < Board.getInstance().ELEMENT_SPRITES.Length - 1 ? oldType + 1 : 0;
            elementScript.setType(newType);

            Board.getInstance().updateElementTypesMatrix(gameObject, newType);
        }
    }

    public void onSwapPosTweening(Vector3 swapEndPos, int newSortingOrder)
    {
        // Tweening this transform position to swapEndPosition
        transform.DOMove(swapEndPos, SWAPPING_DURATION).SetEase(Ease.InOutExpo).OnComplete(() =>
        {
            GetComponent<SpriteRenderer>().sortingOrder = 0;
            elementScript.setSortingOrder(1);
        });

        GetComponent<SpriteRenderer>().sortingOrder = newSortingOrder - 1; // Cell's sorting layer has to be behind element's.
        elementScript.setSortingOrder(newSortingOrder);
    }

    public void onShiftingDown()
    {
        var endPos = transform.position;
        transform.position = new Vector3(transform.position.x, transform.position.y + 20, transform.position.z);

        transform.DOMove(endPos, Board.CELL_SHIFTING_DOWN_DURATION);
    }

    public void onRegenNewElement()
    {
        var endPos = transform.position;
        transform.position = new Vector3(transform.position.x, transform.position.y + 20, transform.position.z);

        transform.DOMove(endPos, Board.CELL_SHIFTING_DOWN_DURATION);
    }
}
