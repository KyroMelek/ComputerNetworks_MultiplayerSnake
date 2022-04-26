using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Renderer : MonoBehaviour
{
    public static Renderer theRenderer { get; private set; }
    public Sprite opposingSnakeHead;
    public Sprite opposingSnakeBody;
    public Sprite Snack;

    private void Awake()
    {
        theRenderer = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void recieveAndRenderOpposingSnakeCoords(List<Vector2> snakeCoords)
    {
        
    }

    public void recieveAndRenderSnackCoords(Vector2 snackCoords)
    {

    }
}
