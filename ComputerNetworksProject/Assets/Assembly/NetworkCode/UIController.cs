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
    public string hostIP;   //IP of the user hosting the server
    public string hostPort; //Port the host server is being hosted on

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
    public GameObject mainMenu;
    public GameObject createLobbyMenu;
    public GameObject joinLobbyMenu;
    public GameObject connectingMenu;
    public GameObject lobbyMenu;
    public GameObject canvasContainer;
    private GameObject previousMenu;

    [Header("Game")]
    public GameObject gameContainer;

    private string localIP;

    private bool isServerResponseRecieved = false;
    private bool readyToWriteTextFields = false;

    private bool player1Ready = false;
    private bool player2Ready = false;

    private bool startGame = false;

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
        if (startGame)
        {
            canvasContainer.SetActive(false);
            gameContainer.SetActive(true);
        }
    }

    public void createLobby()
    {
        previousMenu = createLobbyMenu;

        hostPort = HostPortField.text;
        userName = UserNameHostField.text;

        if(string.IsNullOrWhiteSpace(hostPort) || string.IsNullOrWhiteSpace(userName))
        {
            createLobbyMenu.SetActive(true);
            lobbyMenu.SetActive(false);
            return;
        }

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
        previousMenu = joinLobbyMenu;

        userName = UserNameClientField.text;
        hostIP = HostIPField.text;
        hostPort = HostPortFieldJoin.text;

        if(string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(hostIP) || string.IsNullOrWhiteSpace(hostPort))
        {
            joinLobbyMenu.SetActive(true);
            lobbyMenu.SetActive(false);
            connectingMenu.SetActive(false);
            return;
        }

        client.startClient();
        
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

    public void StartGame()
    {
        startGame = true;
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    public void BackToPreviousMenu()
    {
        previousMenu.SetActive(true);
        lobbyMenu.SetActive(false);
    }
}
