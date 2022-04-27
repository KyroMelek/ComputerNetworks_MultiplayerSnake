using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    public GameObject snake;
    public GameObject snakeEnemy;

    private Snake player;
    private EnemySnake Enemy;

    UIController uiController;

    // Start is called before the first frame update
    void Start()
    {
        player = snake.GetComponent<Snake>();
        Enemy = snakeEnemy.GetComponent<EnemySnake>();
        uiController = UIController.theUIController;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.isAlive && !Enemy.isAlive)
        {
            Debug.Log("WINNNNN");
            uiController.canvasContainer.SetActive(true);            
            uiController.gameContainer.SetActive(false);
            uiController.winScreen.SetActive(true);
            Debug.Log("WINNNNN2");
        }
        else if (!player.isAlive && Enemy.isAlive)
        {
            Debug.Log("LOSEE");
            uiController.canvasContainer.SetActive(true);
            uiController.gameContainer.SetActive(false);
            uiController.loseScreen.SetActive(true);
        }
        else if (!player.isAlive && !Enemy.isAlive)
        {
            Debug.Log("LOSEEE");
            uiController.canvasContainer.SetActive(true);
            uiController.gameContainer.SetActive(false);
            uiController.loseScreen.SetActive(true);
        }
    }
}
