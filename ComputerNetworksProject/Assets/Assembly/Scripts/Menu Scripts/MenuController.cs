using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuController : MonoBehaviour
{
    public string username;
    public uint hostPort;
    public string hostIp;

    private GameObject lobbyMenu;
    private GameObject connectingMenu;
    private GameObject previousMenu;

    private void Start()
    {
        Debug.Log("Start");
        lobbyMenu = GetChild(GameObject.Find("MenuCanvas"), "LobbyMenu");
        connectingMenu = GetChild(GameObject.Find("MenuCanvas"), "ConnectingMenu");
    }

    /// <summary>
    /// Get child GameObject from a parent by name.
    /// </summary>
    /// <param name="parent">The parent of the child you wish to find.</param>
    /// <param name="childName">The name of the child you wish to find.</param>
    /// <returns>The child GameObject or null if no child is found.</returns>
    private GameObject GetChild(GameObject parent, string childName)
    {
        if (parent == null)
        {
            throw new System.NullReferenceException("Argument 'parent' is null");
        }
        Transform childTransform = parent.transform.Find(childName);
        if (childTransform != null)
        {
            return childTransform.gameObject;
        }
        return null;
    }

    #region Main Menu  
    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
    #endregion

    #region Create Lobby Menu
    public void CreateLobby()
    {
        // Get current active menu
        GameObject currentMenu = GameObject.FindWithTag("Menu");
        if (currentMenu == null)
        {
            Debug.Log("Could not find game object with tag \"Menu\".");
            return;
        }

        // Validate User Input
        GameObject usernameInputField = GetChild(currentMenu, "UsernameInputField");
        username = usernameInputField.GetComponent<TMP_InputField>().text;
        if (string.IsNullOrEmpty(username))
        {
            Debug.Log("Invalid username.");
            return;
        }

        GameObject hostPortInputField = GetChild(currentMenu, "HostPortInputField");
        if (!uint.TryParse(hostPortInputField.GetComponent<TMP_InputField>().text, out hostPort) || hostPort > 65535)
        {
            Debug.Log("Invalid port number.");
            return;
        }

        // Start server on the player's Ip with the specified port

        // This is just a placeholder for now.
        hostIp = "192.168.1.1";


        // Set the Lobby Menu Title Text
        GetChild(lobbyMenu, "TitleText").GetComponent<TMP_Text>().text = $"{username}'s Lobby";

        // Display player info
        GameObject userInfoHost = GetChild(lobbyMenu, "UserInfoHost");
        GetChild(userInfoHost, "UsernameText").GetComponent<TMP_Text>().text = username;
        GetChild(userInfoHost, "IpText").GetComponent<TMP_Text>().text = hostIp;
        GetChild(userInfoHost, "PortText").GetComponent<TMP_Text>().text = hostPort.ToString();
        userInfoHost.SetActive(true);


        // Switch to the Lobby Menu
        previousMenu = currentMenu;
        previousMenu.SetActive(false);
        lobbyMenu.SetActive(true);
    }
    #endregion

    #region Join Lobby Menu
    public void JoinLobby()
    {
        // Get current active menu
        GameObject currentMenu = GameObject.FindWithTag("Menu");
        if (currentMenu == null)
        {
            Debug.Log("Could not find game object with tag \"Menu\".");
            return;
        }

        // Validate User Input
        GameObject usernameInputField = GetChild(currentMenu, "UsernameInputField");
        username = usernameInputField.GetComponent<TMP_InputField>().text;
        if (string.IsNullOrEmpty(username))
        {
            Debug.Log("Invalid username.");
            return;
        }
        
        // TODO: Make sure it's a valid ip address(w.x.y.z where 0 <= w,x,y,z <= 255)
        GameObject hostIpInputField = GetChild(currentMenu, "HostIpInputField");
        hostIp = hostIpInputField.GetComponent<TMP_InputField>().text;
        if (string.IsNullOrEmpty(hostIp))
        {
            Debug.Log("Invalid host ip.");
            return;
        }

        GameObject hostPortInputField = GetChild(currentMenu, "HostPortInputField");
        if (!uint.TryParse(hostPortInputField.GetComponent<TMP_InputField>().text, out hostPort) || hostPort > 65535)
        {
            Debug.Log("Invalid port number.");
            return;
        }

        // Switch to the Connecting Menu
        previousMenu = currentMenu;
        previousMenu.SetActive(false);
        connectingMenu.SetActive(true);
    }
    #endregion

    // I'm not entirely sure if having a separate menu here is really necessary
    #region Connecting Menu
    public void ConnectToServer()
    {
        GetChild(connectingMenu, "ConnectingIndicator").GetComponent<TMP_Text>().text = "Connecting to server...";
        // Attempt to connect to the specified ip/port combination

        // If unable to connect
        if (false)
        {
            GetChild(GetChild(connectingMenu, "ConnectingIndicator"), "Text").GetComponent<TMP_Text>().text = "Failed to connect to server.";
            return;
        }
        // Else, once connected
        // Get lobby info from server

        // Display lobby info

        // Switch to lobby menu
        connectingMenu.SetActive(false);
        lobbyMenu.SetActive(true);
    }
    #endregion

    #region Lobby Menu
    public void StartGame()
    {
        // If all players are ready, start the game(switch scenes?)

        // Should this function handled by the server?
    }

    public void ExitLobbyMenu()
    {
        // Reset Lobby Menu data values
        GetChild(lobbyMenu, "UserInfoHost").SetActive(false);
        GetChild(lobbyMenu, "UserInfoClient").SetActive(false);

        // Switch to previous menu
        lobbyMenu.SetActive(false);
        previousMenu.SetActive(true);
    }
    #endregion
}
