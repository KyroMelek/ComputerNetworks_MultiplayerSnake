using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System;
using System.Threading;

public class Client : MonoBehaviour
{
    //Set once
    public string opponentUserName;

    // Update every frame
    public List<Vector2> clientPlayerLocations { get; private set; } = new List<Vector2>();

    private UdpClient listenServer;
    Thread clientThread;
    public string hostIP;

    EnemySnakeHelper helper;
    UIController uiController;

    public bool startGame = false;

    public static Client theClient { get; private set; }
    private void Awake()
    {
        theClient = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        helper = EnemySnakeHelper.theHelper;
        uiController = UIController.theUIController;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startClient()
    {
        listenServer = new UdpClient(7952);
        clientThread = new Thread(() => listner(listenServer));
        clientThread.Start();
    }

    //place in player movement code
    public void sendLocations(List<Vector2> snake)
    {
        string stringToSend = "PlayerLocations:" + snake.ToString();
        int UDP_PORT = 7700;
        UdpClient udpClient = new UdpClient();
        Debug.Log(stringToSend);
        var data = Encoding.UTF8.GetBytes(stringToSend);
        udpClient.Send(data, data.Length, hostIP, UDP_PORT);
    }
    //place in player collision code
    //Need to add a flag to collision code to determine which snack was eaten (1 or 2)
    public void sendSnackStatus(string whichSnack)
    {
        string stringToSend = whichSnack;
        int UDP_PORT = 7700;
        UdpClient udpClient = new UdpClient();
        Debug.Log(stringToSend);
        var data = Encoding.UTF8.GetBytes(stringToSend);
        udpClient.Send(data, data.Length, hostIP, UDP_PORT);
    }

    private void listner(UdpClient client)
    {
        byte[] recData;
        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                Debug.Log("About to receive data from server");
                recData = client.Receive(ref anyIP);
                string receivedText = Encoding.UTF8.GetString(recData);
                if (receivedText.Contains("Snack1 location:")) //This means if player 1 (host) is joining this server -This always happens first
                {
                    string [] snackCoords = receivedText.Split(':')[1].Split(',');
                    Vector2 Snack1 = new Vector2( int.Parse(snackCoords[0]), int.Parse(snackCoords[1])) ;
                    helper.recieveAndRenderSnackCoords(Snack1);
                }
                else if (receivedText.Contains("Snack2 location:")) //This means if player 1 (host) is joining this server -This always happens first
                {
                    string[] snackCoords = receivedText.Split(':')[1].Split(',');
                    Vector2 Snack2 = new Vector2(int.Parse(snackCoords[0]), int.Parse(snackCoords[1]));
                    helper.recieveAndRenderSnackCoords(Snack2);
                }
                else if (receivedText.Contains("PlayerLocations:"))
                {
                    clientPlayerLocations.Clear();
                    string locations = receivedText.Split(':')[1];
                    string[] splitLocations = locations.Split(',');
                    int numOfCoords = splitLocations.GetLength(0);

                    for (int i = 0; i < numOfCoords; ++i)
                    {
                        clientPlayerLocations.Add(new Vector2(int.Parse(splitLocations[i]), int.Parse(splitLocations[++i])));
                    }
                    helper.recieveAndRenderOpposingSnakeCoords(clientPlayerLocations);

                }
                else if (receivedText.Contains("UserName"))
                {
                    opponentUserName = receivedText.Split(':')[1];
                    if(receivedText.Contains("Host"))
                        uiController.serverResponseRecieved(opponentUserName, true);
                    else
                        uiController.serverResponseRecieved(opponentUserName, false);
                }
                else if (receivedText.Contains("Start"))
                {
                    uiController.startGame();
                }

            }
            catch (ObjectDisposedException)
            {
                Debug.Log("Server disposed");
                return;
            }
/*            catch (Exception err)
            {
                Debug.Log("UDP Exception: " + err.ToString());

            }*/
        }
    }

    private void OnApplicationQuit()
    {
        if (listenServer != null)
        {
            listenServer.Close();
            clientThread.Abort();
        }        
    }


}
