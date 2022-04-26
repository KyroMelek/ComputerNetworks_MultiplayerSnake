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
    public Vector2 Snack1Location { get; private set; } = new Vector2();
    public Vector2 Snack2Location { get; private set; } = new Vector2();

    //Update every frame
    public List<Vector2> Player1Locations { get; private set; } = new List<Vector2>();
    public List<Vector2> Player2Locations { get; private set; } = new List<Vector2>();

    public static Server theServer { get; private set; }
    private void Awake()
    {
        theServer = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }


    public void startServer()
    {
        listenServer = new UdpClient(7700);
        serverThread = new Thread(() => listner(listenServer));
        serverThread.Start();
    }

    void Update()
    {
        
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
                    Snack1Location = createNewSnackLocations();
                    string data = "Snack1 location:" + Snack1Location.ToString();
                    sendDataToAllClients(data);
                }
                else if (receivedText.Contains("Snack2"))
                {
                    Snack2Location = createNewSnackLocations();
                    string data = "Snack2 location:" + Snack2Location.ToString();
                    sendDataToAllClients(data);
                }
                else if (receivedText.Contains("Ready"))
                {
                    string[] playerReadyStatus = receivedText.Split(':');
                    if (playerReadyStatus[0] == Player1.Value)
                    {
                        P1Ready = true;
                    }
                    else if (playerReadyStatus[0] == Player2.Value)
                    {
                        P2Ready = true;
                    }
                    if (P1Ready && P2Ready)
                    {
                        sendDataToAllClients("Start");
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

    private Vector2 createNewSnackLocations()
    {
        //TODO: Get actual play area borders from Nathan
        //TODO: Add checks to ensure snack doesn't spawn in existing snake coords
        float minY = 0;
        float maxY= 10;

        float minX = 0;
        float maxX = 10;
        float newSnackY = UnityEngine.Random.Range(minY, maxY);
        float newSnackX = UnityEngine.Random.Range(minX, maxX);

        return new Vector2(newSnackX, newSnackY);
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
