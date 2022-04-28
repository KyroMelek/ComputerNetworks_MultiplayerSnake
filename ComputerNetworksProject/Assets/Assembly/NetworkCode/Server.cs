using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using System.Threading;

//Clients listen on port 7952
//Server listens on port 7700

//whoever starts the lobby hosts this server
public class Server : MonoBehaviour
{
    //Set once
    public KeyValuePair<string, string> Player1 { get; private set; } //Used by client script to send player/snack locations
    public KeyValuePair<string, string> Player2 { get; private set; } //Used by client script to send player/snack locations
    public bool P1Ready = false;
    public bool P2Ready = false;

    private UdpClient listenServer;
    Thread serverThread;

    //Update when eaten
    public string Snack1Location { get; private set; }
    public string Snack2Location { get; private set; }

    //Update every frame
    public List<Vector2> Player1Locations { get; private set; } = new List<Vector2>();
    public List<Vector2> Player2Locations { get; private set; } = new List<Vector2>();

    bool shouldCreateNewLocations = false;
    string newLocationSnack;
    bool isSnack1 = false;

    public static Server theServer { get; private set; }
    private void Awake()
    {
        theServer = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }


    public void startServer(int port)
    {
        listenServer = new UdpClient(port);
        serverThread = new Thread(() => listner(listenServer));
        serverThread.Start();
    }

    void Update()
    {
        if (shouldCreateNewLocations)
        {
            int minX = -19;
            int maxX = 19;
            int minY = -19;
            int maxY = 19;

            int newSnackX = UnityEngine.Random.Range(minX, maxX);
            int newSnackY = UnityEngine.Random.Range(minY, maxY);

            bool locationValid = false;
            while (!locationValid)
            {
                locationValid = true;

                foreach (Vector2 position in Player1Locations)
                    if (newSnackX == Mathf.RoundToInt(position.x) && newSnackY == Mathf.RoundToInt(position.y))
                        locationValid = false;

                foreach (Vector2 position in Player2Locations)
                    if (newSnackX == Mathf.RoundToInt(position.x) && newSnackY == Mathf.RoundToInt(position.y))
                        locationValid = false;
            }

            newLocationSnack = newSnackX + "," + newSnackY;
            shouldCreateNewLocations = false;

            if (isSnack1)
            {
                Snack1Location = newLocationSnack;

                string data = "Snack1 location:" + Snack1Location;
                sendDataToAllClients(data);
            }
            else
            {
                Snack2Location = newLocationSnack;
                string data = "Snack2 location:" + Snack2Location;
                sendDataToAllClients(data);
            }
            isSnack1 = false;
        }
    }
    private void listner (UdpClient client)
    {
        byte[] recData;        
        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                Debug.Log("About to receive Client data");
                recData = client.Receive(ref anyIP);          
                string receivedText = Encoding.UTF8.GetString(recData);
                Debug.Log("SERVER REC"+ receivedText);
                if (receivedText.Contains("PlayerLocations:"))
                {
                    if (anyIP.Address.ToString() == Player1.Key) //This means player 1 is sending locations 
                    {
                        string dataToSend = receivedText;
                        sendDataToClient(dataToSend, Player2.Key);
                        Player1Locations.Clear();
                        string[] splitLocations = receivedText.Split(':')[1].Split(',');
                        int numOfCoords = splitLocations.GetLength(0);
                        for (int i = 0; i < numOfCoords; ++i)
                        {
                            Player1Locations.Add(new Vector2(int.Parse(splitLocations[i]), int.Parse(splitLocations[++i])));
                        }
                    }
                    else if (anyIP.Address.ToString() == Player2.Key) //This means player 2 is sending locations 
                    {
                        string dataToSend = receivedText;
                        sendDataToClient(dataToSend, Player1.Key);
                        Player2Locations.Clear();
                        string[] splitLocations = receivedText.Split(':')[1].Split(',');
                        int numOfCoords = splitLocations.GetLength(0);
                        for (int i = 0; i < numOfCoords; ++i)
                        {
                            Player2Locations.Add(new Vector2(int.Parse(splitLocations[i]), int.Parse(splitLocations[++i])));
                        }
                    }
                }
                else if (receivedText.Contains("UserName-Host")) //This means if player 1 (host) is joining this server -This always happens first
                {
                    Player1 = new KeyValuePair<string, string>(anyIP.Address.ToString(), receivedText.Split(':')[1]);
                }
                else if (receivedText.Contains("UserName-Client")) //This means if player 1 is joining this server
                {
                    Player2 = new KeyValuePair<string, string>(anyIP.Address.ToString(), receivedText.Split(':')[1]);
                    sendResponse();
                }
                //Following cases recieved during game runtime
                //Third case, snack 1 or 2 has been eaten. recievedText = "Snack1" or "Snack2"                    
                else if (receivedText.Contains("Snack1"))
                {
                    isSnack1 = true;
                    createNewSnackLocations();

                }
                else if (receivedText.Contains("Snack2"))
                {
                    isSnack1 = false;
                    createNewSnackLocations();
                }
                else if (receivedText.Contains("Ready"))
                {
                    string[] playerReadyStatus = receivedText.Split(':');
                    if (playerReadyStatus[0] == Player1.Value)
                    {
                        P1Ready = bool.Parse(playerReadyStatus[2]);
                        if(Player2.Key != null)
                            sendDataToClient("P1Ready:" + P1Ready, Player2.Key);
                    }
                    else if (playerReadyStatus[0] == Player2.Value)
                    {
                        P2Ready = bool.Parse(playerReadyStatus[2]);
                        sendDataToClient("P2Ready:" + P2Ready, Player1.Key);
                    }

                    if (P1Ready && P2Ready)
                    {
                        sendDataToAllClients("Start");
                    }
                }
                else if (receivedText.Contains("Dead"))
                {
                    string[] playerReadyStatus = receivedText.Split(':');
                    if (playerReadyStatus[0] == Player1.Value)
                    {
                        P1Ready = true;
                        if (Player2.Key != null)
                            sendDataToClient("P1Dead", Player2.Key);
                    }
                    else if (playerReadyStatus[0] == Player2.Value)
                    {
                        P2Ready = true;
                        sendDataToClient("P2Dead", Player1.Key);
                    }

                    if (P1Ready && P2Ready)
                    {
                        sendDataToAllClients("Game Over");
                    }
                }

                //Fourth case, recievedText = player coordinates
                
            }
            catch (ObjectDisposedException)
            {
                Debug.Log("Server disposed");
                return;
            }
            catch (Exception err)
            {
                Debug.Log("UDP Exception: " + err.ToString());
            }
        }
    }

    // Will send user name for the joining client to the other client and the username of the other client
    // to the joining client so they can display each other
    private void sendResponse()
    {
        int UDP_PORT = 7952;
        UdpClient udpClient = new UdpClient();
        string stringToSend = "UserNameHost:" + Player1.Value; //Username
        var data = Encoding.UTF8.GetBytes(stringToSend);
        udpClient.Send(data, data.Length, Player2.Key, UDP_PORT);
        if(P1Ready)
            sendDataToClient("P1Ready", Player2.Key);

        string stringToSend2 = "UserNameClient:" + Player2.Value; //Username
        var data2 = Encoding.UTF8.GetBytes(stringToSend2);
        udpClient.Send(data2, data2.Length, Player1.Key, UDP_PORT);
    }

    private void sendDataToAllClients(string stringToSend)
    {
        int UDP_PORT = 7952;
        UdpClient udpClient = new UdpClient();        
        var data = Encoding.UTF8.GetBytes(stringToSend);
        udpClient.Send(data, data.Length, Player1.Key, UDP_PORT);
        udpClient.Send(data, data.Length, Player2.Key, UDP_PORT);
    }

    private void sendDataToClient(string stringToSend, string IP)
    {
        int UDP_PORT = 7952;
        UdpClient udpClient = new UdpClient();
        var data = Encoding.UTF8.GetBytes(stringToSend);
        udpClient.Send(data, data.Length, IP, UDP_PORT);        
    }

   private void createNewSnackLocations()
    {
        shouldCreateNewLocations = true;

    }

    private void OnApplicationQuit()
    {
        if (listenServer != null)
        {
            listenServer.Close();
            serverThread.Abort();
        }        
    }
}
