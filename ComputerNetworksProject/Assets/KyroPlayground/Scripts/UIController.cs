using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using TMPro;
using System.Net;

public class UIController : MonoBehaviour
{
    string userName;
    string hostIP;

    Server server;
    Client client;

    public TMP_InputField UserNameHostField;
    public TMP_InputField HostPortField;

    public TMP_InputField UserNameClientField;
    public TMP_InputField HostIPField;
    public TMP_InputField HostPortFieldJoin;

    public TMP_Text LobbyName;
    public TMP_Text userNameHostLobby;
    public TMP_Text userNameClientLobby;

    public GameObject userInfoHost;
    public TMP_Text HostIP;
    public TMP_Text ClientIP;
    public GameObject userInfoClient;

    public GameObject connectingMenu;
    public GameObject lobbyMenu;

    public string localIP;

    private bool isServerResponseRecieved = false;

    public static UIController theUIController { get; private set; }
    private void Awake()
    {
        theUIController = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        server = Server.theServer;
        client = Client.theClient;


        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);        
        socket.Connect("8.8.8.8", 65530);
        IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
        localIP = endPoint.Address.ToString();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isServerResponseRecieved)
        {
            connectingMenu.SetActive(false);
            lobbyMenu.SetActive(true);
            userInfoHost.SetActive(true);            
            userInfoClient.SetActive(true);
            isServerResponseRecieved = false;
        }
    }


    public void submitUserName(string user)
    {
        userName = user;
    }

    public void submitHostIP(string IP)
    {
        hostIP = IP;
    }

    public void submitHostPort(string IP)
    {
        hostIP = IP;
    }

    public void displayOpponentUserName(string userName)
    {
        //Set some text field in the ui to this user name
    }

    public void createLobby()
    {
        client.startClient();
        server.startServer();

        string stringToSend = "UserName-Host:" + userName;
        int UDP_PORT = int.Parse(HostPortField.text);
        UdpClient udpClient = new UdpClient();
        
        var data = Encoding.UTF8.GetBytes(stringToSend);
        udpClient.Send(data, data.Length, "127.0.0.1", UDP_PORT);
        hostIP = "127.0.0.1";

        LobbyName.text = userName + "\'s Lobby";
        userNameHostLobby.text = userName;
        userInfoHost.SetActive(true);
        HostIP.text = localIP;
    }

    public void joinLobby()
    {
        client.startClient();

        userName = UserNameClientField.text;
        hostIP = HostIPField.text;

        string stringToSend = "UserName-Client:" + userName;
        int UDP_PORT = int.Parse(HostPortFieldJoin.text);
        UdpClient udpClient = new UdpClient();

        var data = Encoding.UTF8.GetBytes(stringToSend);
        udpClient.Send(data, data.Length, hostIP, UDP_PORT);

    }

    public void serverResponseRecieved(string hostUserName, bool isClient)
    {
        isServerResponseRecieved = true;
        //connectingMenu.SetActive(false);
        //lobbyMenu.SetActive(true);
        if (isClient)
        {
            //userInfoHost.SetActive(true);
            LobbyName.text = hostUserName + "\'s Lobby";
            userNameHostLobby.text = userName;            
        }

        //userInfoClient.SetActive(true);
        userNameClientLobby.text = hostUserName;        
    }

    public void sendReadyStatus()
    {
        string stringToSend = userName + ":Ready";
        int UDP_PORT = 7700;
        UdpClient udpClient = new UdpClient();

        var data = Encoding.UTF8.GetBytes(stringToSend);
        udpClient.Send(data, data.Length, hostIP, UDP_PORT);
    }
}
