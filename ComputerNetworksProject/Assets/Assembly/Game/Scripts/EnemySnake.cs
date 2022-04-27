using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class EnemySnake : MonoBehaviour
{
    UIController uiController;

    public GameObject headSegment;
    public GameObject bodySegmentContainer;
    public GameObject bodySegmentPrefab;
    private List<GameObject> bodySegmentObjects;
    public Vector2 positionBehindLastSegment;

    public int x = 10;
    public int y = 20;

    public int size = 4;
    public float baseSpeed = 0.5f;
    public float speedPerBlock = 0.01f;
    private float calculatedSpeed;
    private float timer;

    public Direction direction = Direction.EAST;
    public Direction targetDirection = Direction.EAST;

    private bool started;
    private bool moving;

    public bool isAlive;

    private void Awake()
    {
        bodySegmentObjects = new List<GameObject>();

        started = false;
        moving = false;

        calculatedSpeed = baseSpeed - speedPerBlock * size;
        timer = calculatedSpeed;
        Debug.Log("Timer on awake: " + timer);

        isAlive = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        uiController = UIController.theUIController;

        for (int i = 0; i < size; i++)
        {
            addBodySegment(new Vector2(i + 1, 0));
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*
        calculatedSpeed = baseSpeed - speedPerBlock * size;

        if (!started && !moving && Input.anyKey)
        {
            started = true;
            moving = true;
        }

        if (started && moving)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (direction != Direction.SOUTH)
                {
                    targetDirection = Direction.NORTH;
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (direction != Direction.NORTH)
                {
                    targetDirection = Direction.SOUTH;
                }
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (direction != Direction.WEST)
                {
                    targetDirection = Direction.EAST;
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (direction != Direction.EAST)
                {
                    targetDirection = Direction.WEST;
                }
            }

            //Debug.Log("Timer is Before: " + timer);
            timer -= Time.deltaTime;
            //Debug.Log("Timer is during: " + timer);
            //Debug.Log("Calc speed: " + calculatedSpeed);
            if (timer <= 0)
            {
                moveSnake();
                timer = calculatedSpeed;
                //Debug.Log("Timer is After: " + timer);
            }
        }
        */
    }

    private void moveSnake()
    {
        //Debug.Log("Move snake called");
        int startingX = x;
        int startingY = y;

        int endingX = Mathf.RoundToInt(bodySegmentObjects[bodySegmentObjects.Count - 1].transform.position.x);
        int endingY = Mathf.RoundToInt(bodySegmentObjects[bodySegmentObjects.Count - 1].transform.position.y);
        positionBehindLastSegment = new Vector2(endingX, endingY);

        direction = targetDirection;

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

        List<Vector2> snakeLocations = new List<Vector2>();
        snakeLocations.Add(new Vector2(headSegment.transform.position.x, headSegment.transform.position.y));
        for (int i = 0; i < bodySegmentObjects.Count; ++i)
        {
            snakeLocations.Add(new Vector2(bodySegmentObjects[i].transform.position.x, bodySegmentObjects[i].transform.position.y));
        }
        sendLocations(snakeLocations);
    }

    public void addBodySegment(Vector2 position)
    {
        bodySegmentObjects.Add(Instantiate(bodySegmentPrefab, position, Quaternion.identity, bodySegmentContainer.transform));
    }

    public void kill()
    {
        isAlive = false;
        moving = false;
    }

    //place in player movement code
    public void sendLocations(List<Vector2> snake)
    {
        string vectorListCSV = "";
        foreach (Vector2 coord in snake)
        {
            vectorListCSV += coord.x.ToString();
            vectorListCSV += ',';
            vectorListCSV += coord.y.ToString();
            vectorListCSV += ',';
        }
        string vectorListCSVChopped = vectorListCSV.Remove(vectorListCSV.Length - 1);

        Debug.Log(vectorListCSVChopped);
        string stringToSend = "PlayerLocations:" + vectorListCSVChopped;
        UdpClient udpClient = new UdpClient();
        Debug.Log(stringToSend);
        var data = Encoding.UTF8.GetBytes(stringToSend);
        udpClient.Send(data, data.Length, uiController.hostIP, 7700);
    }
    //place in player collision code
    //Need to add a flag to collision code to determine which snack was eaten (1 or 2)
    public void sendSnackStatus(string whichSnack)
    {
        string stringToSend = whichSnack;
        UdpClient udpClient = new UdpClient();
        Debug.Log(stringToSend);
        var data = Encoding.UTF8.GetBytes(stringToSend);
        udpClient.Send(data, data.Length, uiController.hostIP, 7700);
    }

    public void updatePositions(List<Vector2> snakeCoords, Quaternion headRotation)
    {
        while (bodySegmentObjects.Count < snakeCoords.Count - 1)
            addBodySegment(Vector2.zero);

        headSegment.transform.position = snakeCoords[0];
        headSegment.transform.rotation = headRotation;

        for (int i = 1; i < snakeCoords.Count; i++)
            bodySegmentObjects[i - 1].transform.position = snakeCoords[i];
    }
}
