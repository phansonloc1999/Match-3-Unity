using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element : MonoBehaviour
{
    [SerializeField]
    private int type;

    private SpriteRenderer spriteRenderer;

    static Board boardScript = null;

    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (boardScript == null)
        {
            boardScript = GameObject.Find("Board").GetComponent<Board>();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setType(int type)
    {
        this.type = type;
        if (type == -1) spriteRenderer.sprite = null;
        else spriteRenderer.sprite = boardScript.getElementSprite(type);
    }

    public int getType()
    {
        return this.type;
    }

    public void setSortingOrder(int order)
    {
        spriteRenderer.sortingOrder = order;
    }
}
