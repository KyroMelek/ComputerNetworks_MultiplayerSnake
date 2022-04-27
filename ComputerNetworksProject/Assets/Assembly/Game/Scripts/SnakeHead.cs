using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeHead : MonoBehaviour
{
    public Snake snake;

    private void OnTriggerEnter2D(Collider2D collision) // this will fire from the children if the parent is the one with the rigidbody; thus, get rid of children's rigidbodies
    {
        //Debug.Log("Collision Happened");

        if (collision.CompareTag("Player Body") || collision.CompareTag("Wall"))
        {
            snake.kill();
        }
        else if (collision.CompareTag("Enemy Player Head"))
        {
            if (snake.size <= collision.gameObject.GetComponent<SnakeHead>().snake.size)
                snake.kill();
        }
        else if (collision.CompareTag("Snack"))
        {
            snake.addBodySegment(snake.positionBehindLastSegment);
            snake.size++;
        }
    }
}
