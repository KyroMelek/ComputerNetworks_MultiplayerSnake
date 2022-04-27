using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using TMPro;
using System.Net;

public class UIController : MonoBehaviour
{
    Server server;
    Client client;

    private string userName; //This local user
    private string hostIP;   //IP of the user hosting the server
    private string hostPort; //Port the host server is being hosted on

    //Create Lobby menu
    [Header("Create Lobby menu")]
    public TMP_InputField UserNameHostField;
    public TMP_InputField HostPortField;

    //Join Lobby Menu
    [Header("Join Lobby Menu")]
    public TMP_InputField UserNameClientField;
    public TMP_InputField HostIPField;
    public TMP_InputField HostPortFieldJoin;

    //Lobby Menu
    [Header("Lobby Menu")]
    public TMP_Text LobbyName;        

    //Lobby Menu Host Info
    [Header("Lobby Menu Host Info")]
    public GameObject userInfoHost;
    public TMP_Text userNameHostLobby;
    public TMP_Text HostIP;
    public TMP_Text userInfoPortHost;
    public GameObject ReadyIconHost;
    public GameObject NotReadyIconHost;

    //Lobby Menu Client Info
    [Header("Lobby Menu Client Info")]
    public GameObject userInfoClient;
    public TMP_Text userNameClientLobby;
    public TMP_Text ClientIP;
    public TMP_Text userInfoPortClient;
    public GameObject ReadyIconClient;
    public GameObject NotReadyIconClient;

    //Menus
    [Header("Menu")]
    public GameObject connectingMenu;
    public GameObject lobbyMenu;

    private string localIP;

    private bool isServerResponseRecieved = false;
    private bool readyToWriteTextFields = false;

    private bool player1Ready = false;
    private bool player2Ready = false;

    private string opponentUserName;
    private bool isClient = false;

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
            readyToWriteTextFields = true;

            StartCoroutine(setText(opponentUserName, isClient));
        }

        if (player1Ready)
        {
            NotReadyIconHost.SetActive(false);
            ReadyIconHost.SetActive(true);
            player1Ready = false;
        }
        if (player2Ready)
        {
            NotReadyIconClient.SetActive(false);
            ReadyIconClient.SetActive(true);
            player2Ready = false;
        }

    }


    //public void submitUserName(string user)
    //{
    //    userName = user;
    //}

    //public void submitHostIP(string IP)
    //{
    //    hostIP = IP;
    //}

    //public void submitHostPort(string Port)
    //{
    //    hostPort = Port;
    //}

    public void createLobby()
    {

        hostPort = HostPortField.text;
        userName = UserNameHostField.text;

        int UDP_PORT = int.Parse(hostPort);

        client.startClient();
        server.startServer(UDP_PORT);
                
        UdpClient udpClient = new UdpClient();

        string stringToSend = "UserName-Host:" + userName;
        var data = Encoding.UTF8.GetBytes(stringToSend);
        udpClient.Send(data, data.Length, "127.0.0.1", UDP_PORT);
        hostIP = "127.0.0.1";

        userInfoHost.SetActive(true);
        LobbyName.text = userName + "\'s Lobby";
        userNameHostLobby.text = userName;        
        HostIP.text = localIP;
        userInfoPortHost.text = hostPort;

        isClient = false;
    }

    public void joinLobby()
    {
        client.startClient();

        userName = UserNameClientField.text;
        hostIP = HostIPField.text;
        hostPort = HostPortFieldJoin.text;
        
        int UDP_PORT = int.Parse(hostPort);
        UdpClient udpClient = new UdpClient();
        string stringToSend = "UserName-Client:" + userName;
        var data = Encoding.UTF8.GetBytes(stringToSend);
        udpClient.Send(data, data.Length, hostIP, UDP_PORT);
        isClient = true;
    }

    public void serverResponseRecieved(string _UserName, bool _isClient)
    {
        opponentUserName = _UserName;
        isClient = _isClient;
        isServerResponseRecieved = true;
    }

    IEnumerator setText(string opponentUserName, bool isClient)
    {
        yield return new WaitUntil(() => readyToWriteTextFields);
        if (isClient)
        {
            LobbyName.text = opponentUserName + "\'s Lobby";

            userNameHostLobby.text = opponentUserName;
            HostIP.text = hostIP;
            userInfoPortHost.text = hostPort;

            userNameClientLobby.text = userName;
            ClientIP.text = localIP;
            userInfoPortClient.text = "7952";
        }
        else
        {
            userNameClientLobby.text = opponentUserName;
            ClientIP.text = server.Player2.Key;            
            userInfoPortClient.text = "7952";
        }
    }

    public void sendReadyStatus()
    {
        string stringToSend = userName + ":Ready";
        int UDP_PORT = 7700;
        UdpClient udpClient = new UdpClient();

        var data = Encoding.UTF8.GetBytes(stringToSend);
        udpClient.Send(data, data.Length, hostIP, UDP_PORT);

        if (!isClient)
        {
            NotReadyIconHost.SetActive(false);
            ReadyIconHost.SetActive(true);
        }
        else 
        {
            NotReadyIconClient.SetActive(false);
            ReadyIconClient.SetActive(true);
        }
    }

    public void setReadyStatus(int player)
    {
        if (player == 1)        
            player1Ready = true;
        else
            player2Ready = true;
    }
}
