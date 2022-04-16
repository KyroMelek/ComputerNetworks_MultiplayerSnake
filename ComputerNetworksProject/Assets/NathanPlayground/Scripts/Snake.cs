using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    public GameObject bodySegmentContainer;

    public int x = 10;
    public int y = 20;

    public int size = 4;
    public float baseSpeed = 1;
    public float speedPerBlock = 0.01f;

    public Direction direction = Direction.EAST;

    private bool started;
    private bool moving;

    private void Awake()
    {
        started = false;
        moving = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(started && moving)
        {
            if(Input.GetKeyDown(KeyCode.UpArrow))
            {

            }
            else if(Input.GetKeyDown(KeyCode.DownArrow))
            {

            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {

            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {

            }
        }

        if (!started && !moving && Input.anyKey)
        {
            started = true;
            moving = true;
        }
    }
}
