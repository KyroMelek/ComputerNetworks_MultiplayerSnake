using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    public GameObject headSegment;
    public GameObject bodySegmentContainer;
    public GameObject bodySegmentPrefab;
    private List<GameObject> bodySegmentObjects;
    private Vector2 positionBehindLastSegment;

    public int x = 10;
    public int y = 20;

    public int size = 4;
    public float baseSpeed = 0.5f;
    public float speedPerBlock = 0.01f;
    private float calculatedSpeed;
    private float timer;

    public Direction direction = Direction.EAST;

    private bool started;
    private bool moving;

    private void Awake()
    {
        bodySegmentObjects = new List<GameObject>();

        started = false;
        moving = false;

        calculatedSpeed = baseSpeed - speedPerBlock * size;
        timer = calculatedSpeed;
    }

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < size; i++)
        {
            addBodySegment(new Vector2(-i - 1, 0));
        }
    }

    // Update is called once per frame
    void Update()
    {
        calculatedSpeed = baseSpeed - speedPerBlock * size;

        if (!started && !moving && Input.anyKey)
        {
            started = true;
            moving = true;
        }

        if (started && moving)
        {
            if(Input.GetKeyDown(KeyCode.UpArrow))
            {
                if(direction != Direction.SOUTH)
                {
                    direction = Direction.NORTH;
                }
            }
            else if(Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (direction != Direction.NORTH)
                {
                    direction = Direction.SOUTH;
                }
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (direction != Direction.WEST)
                {
                    direction = Direction.EAST;
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (direction != Direction.EAST)
                {
                    direction = Direction.WEST;
                }
            }

            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                moveSnake();
                timer = calculatedSpeed;
            }
        }
    }

    private void moveSnake()
    {
        int startingX = x;
        int startingY = y;

        int endingX = Mathf.RoundToInt(bodySegmentObjects[bodySegmentObjects.Count - 1].transform.position.x);
        int endingY = Mathf.RoundToInt(bodySegmentObjects[bodySegmentObjects.Count - 1].transform.position.y);
        positionBehindLastSegment = new Vector2(endingX, endingY);

        switch (direction)
        {
            case Direction.NORTH:

                y++;
                headSegment.transform.rotation = Quaternion.Euler(0, 0, 0);

                break;

            case Direction.SOUTH:

                y--;
                headSegment.transform.rotation = Quaternion.Euler(0, 0, 180);

                break;

            case Direction.WEST:

                x--;
                headSegment.transform.rotation = Quaternion.Euler(0, 0, 90);

                break;

            case Direction.EAST:

                x++;
                headSegment.transform.rotation = Quaternion.Euler(0, 0, 270);

                break;
        }

        headSegment.transform.position = new Vector2(x, y);

        for (int i = bodySegmentObjects.Count - 1; i > 0; i--)
            bodySegmentObjects[i].transform.position = bodySegmentObjects[i - 1].transform.position;

        bodySegmentObjects[0].transform.position = new Vector2(startingX, startingY);
    }

    private void addBodySegment(Vector2 position)
    {
        bodySegmentObjects.Add(Instantiate(bodySegmentPrefab, position, Quaternion.identity, bodySegmentContainer.transform));
    }

    private void OnTriggerEnter2D(Collider2D collision) // this will fire from the children if the parent is the one with the rigidbody; thus, get rid of children's rigidbodies
    {
        addBodySegment(positionBehindLastSegment);
    }
}
