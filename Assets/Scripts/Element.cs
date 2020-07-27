using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element : MonoBehaviour
{
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
        spriteRenderer.sprite = boardScript.getElementSprite(type);
    }

    public int getType()
    {
        return this.type;
    }
}
