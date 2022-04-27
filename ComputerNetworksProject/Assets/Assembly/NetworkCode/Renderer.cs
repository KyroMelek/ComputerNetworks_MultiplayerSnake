using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Renderer : MonoBehaviour
{
    public static Renderer theRenderer { get; private set; }
    private List<SpriteRenderer> opposingSnakeSpriteRenderers;
    private SpriteRenderer snackSpriteRenderer;
    private int previousSnakeLength = 0;
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
        snackSpriteRenderer = gameObject.AddComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public void recieveAndRenderOpposingSnakeCoords(List<Vector2> snakeCoords)
    {
        for(int i = previousSnakeLength; i < snakeCoords.Count; ++i)
        {
            SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.color = Color.red;
            if (i == 0)
            {
                spriteRenderer.sprite = opposingSnakeHead;
            }
            else
            {
                spriteRenderer.sprite = opposingSnakeBody;
            }
            opposingSnakeSpriteRenderers.Add(spriteRenderer);
        }
        opposingSnakeSpriteRenderers[0].transform.rotation = getOpposingSnakeRotationFromCoords(snakeCoords);
        opposingSnakeSpriteRenderers[0].transform.position = snakeCoords[0];

        for(int i = 1; i < snakeCoords.Count; ++i)
        {
            opposingSnakeSpriteRenderers[i].transform.position = snakeCoords[i];
        }
        previousSnakeLength = snakeCoords.Count;
    }

    public void recieveAndRenderSnackCoords(Vector2 snackCoords)
    {
        snackSpriteRenderer.transform.position = snackCoords;
    }
}
