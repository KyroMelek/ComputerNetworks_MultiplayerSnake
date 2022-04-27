using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class SnakeHead : MonoBehaviour
{
    public Snake snake;
    private UIController uiController;

    private void Start()
    {
        uiController = UIController.theUIController;
    }

    private void OnTriggerEnter2D(Collider2D collision) // this will fire from the children if the parent is the one with the rigidbody; thus, get rid of children's rigidbodies
    {
        //Debug.Log("Collision Happened");

        if (collision.CompareTag("Player Body") || collision.CompareTag("Wall"))
        {
            snake.kill();
        }
        else if (collision.CompareTag("Enemy Player Head"))
        {
            if (snake.size <= collision.gameObject.GetComponent<EnemySnakeHead>().snake.size)
                snake.kill();
        }
        else if (collision.CompareTag("Snack"))
        {
            snake.addBodySegment(snake.positionBehindLastSegment);
            snake.size++;
            sendDataToServer("Snack1", uiController.hostIP );
        }
        else if (collision.CompareTag("Snack2"))
        {
            snake.addBodySegment(snake.positionBehindLastSegment);
            snake.size++;
            sendDataToServer("Snack2", uiController.hostIP );
        }
              
    }

    private void sendDataToServer(string stringToSend, string IP)
    {
        int UDP_PORT = 7700;
        UdpClient udpClient = new UdpClient();
        var data = Encoding.UTF8.GetBytes(stringToSend);
        udpClient.Send(data, data.Length, IP, UDP_PORT);        
    }
}
