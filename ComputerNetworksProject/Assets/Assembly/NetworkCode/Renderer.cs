using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Renderer : MonoBehaviour
{
    public static Renderer theRenderer { get; private set; }

    private int previousSnakeLength = 0;
    public EnemySnake snake;
    public GameObject Snack;
    public GameObject Snack2;
    List<Vector2> snakeCoords = new List<Vector2>();

    bool shouldRender = false;

    GameObject head;
    List<GameObject> body;
     
    private void Awake()
    {
        theRenderer = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        head = new GameObject();
        body = new List<GameObject>();

    }

    // Update is called once per frame
    void Update()
    {
        if (shouldRender)
        {
            snake.updatePositions(snakeCoords, getOpposingSnakeRotationFromCoords(snakeCoords));
            shouldRender = false;
        }
    }

    private Quaternion getOpposingSnakeRotationFromCoords(List<Vector2> snakeCoords)
    {
        float headX = snakeCoords[0].x;
        float headY = snakeCoords[0].y;
        float bodyX = snakeCoords[1].x;
        float bodyY = snakeCoords[1].y;

        if (headY > bodyY)
        {
            // Face NORTH
            return Quaternion.Euler(0, 0, 0);
        }
        else if (headY < bodyY)
        {
            // Face SOUTH
            return Quaternion.Euler(0, 0, 180);
        }
        else if (headX > bodyX)
        {
            // Face EAST
            return Quaternion.Euler(0, 0, 270);
        }
        else if (headX < bodyX)
        {
            // Face WEST
            return Quaternion.Euler(0, 0, 90);
        }
        
        return Quaternion.Euler(0, 0, 0);
    }

    public void recieveAndRenderOpposingSnakeCoords(List<Vector2> _snakeCoords)
    {
        snakeCoords = _snakeCoords;
        shouldRender = true;        
    }

    public void recieveAndRenderSnackCoords(Vector2 snackCoords, bool isSnack1)
    {
        if(isSnack1)
        {
            Snack.transform.position = new Vector3(snackCoords.x, snackCoords.y, 0);            
        }
        else 
        {
            Snack2.transform.position = new Vector3(snackCoords.x, snackCoords.y, 0);
        }

    }
}
