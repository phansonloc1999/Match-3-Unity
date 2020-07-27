using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element : MonoBehaviour
{
    private int type;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setType(int type)
    {
        this.type = type;
        GetComponent<SpriteRenderer>().sprite = GameObject.Find("Board").GetComponent<Board>().getElementSprite(type);
    }

    public int getType()
    {
        return this.type;
    }
}
