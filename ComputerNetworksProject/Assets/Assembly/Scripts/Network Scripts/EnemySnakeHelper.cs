using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySnakeHelper : MonoBehaviour
{
    public static EnemySnakeHelper theHelper { get; private set; }
    public GameObject enemySnake;
    public GameObject snackGenerator;

    private void Awake()
    {
        theHelper = this;
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
        enemySnake.GetComponent<Snake>().updateHeadAndBodyPositions(snakeCoords);
    }

    public void recieveAndRenderSnackCoords(Vector2 snackCoords)
    {
        snackGenerator.GetComponent<SnackGenerator>().generateSnack(snackCoords);
    }
}
