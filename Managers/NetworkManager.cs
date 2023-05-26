using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{   //PUBLIC

    //Panel References
    [Header("Connection Status")]
    public Text ConnectionStatusText;

    [Header("Login UI Panel")]
    public InputField PlayerNameInput;
    public GameObject Login_UI_Panel;

    [Header("Game Options  UI Panel")]
    public GameObject GameOptions_UI_Panel;

    [Header("Create Room UI Panel")]
    public GameObject CreateRoom_UI_Panel;
    public InputField RoomNameInputField;
    public InputField MaxPlayersInputField;

    [Header("Inside Room UI Panel")]
    public GameObject InsideRoom_UI_Panel;
    public Text RoomInfoText;
    public GameObject PlayerListPrefab;
    public GameObject PlayerListContentParent;
    public GameObject StartGameButton;


    [Header("Room List UI Panel")]
    public GameObject RoomList_UI_Panel;
    public GameObject RoomListEntryPrefab;
    public GameObject RoomListParentGameObject;

    [Header("Join Random Room UI Panel")]
    public GameObject JoinRandomRoom_UI_Panel;

    [Header("Fader")]
    public CanvasGroup Fader;
    public GameObject FaderObject;
    private Coroutine fadeRoutine;

    //PRIVATE

    //private data containers
    private Dictionary<string, RoomInfo> cachedRoomDict;
    private Dictionary<string, GameObject> roomListGameObjects;
    private Dictionary<int, GameObject> playerListGameObjectsDict;

    #region __MonoBehaviour Methods__

    private void Start()
    {
        //Initialize dictionaries
        cachedRoomDict = new Dictionary<string, RoomInfo>();
        roomListGameObjects = new Dictionary<string, GameObject>();

        //Auto load master client's scene
        PhotonNetwork.AutomaticallySyncScene = true;

        //Activatate Login Panel on Startup
        if (!PlayerPrefs.HasKey(PrefKeys.PlayerName))
        {
            ActivatePanel(Login_UI_Panel.name);
        }
        else
        {
            ActivatePanel(GameOptions_UI_Panel.name);
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LocalPlayer.NickName = PlayerPrefs.GetString(PrefKeys.PlayerName);
                PhotonNetwork.ConnectUsingSettings();
            }

        }

    }

    private void Update()
    {
        //update status text
        //ConnectionStatusText.text = "Connection Status:  " + PhotonNetwork.NetworkClientState;

        if (PhotonNetwork.NetworkClientState == ClientState.PeerCreated)
        {
            ConnectionStatusText.text = "    Connection Status:  " + "OK!";
        }

        else if (PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterserver)
        {
            ConnectionStatusText.text = "    Connection Status:  " + "Connected to the Server";
        }
        else
        {
            ConnectionStatusText.text = "    Connection Status:  " + PhotonNetwork.NetworkClientState;
        }
    }
    #endregion

    #region __Public Methods__

    //activate and deactivate panels via panel name input
    public void ActivatePanel(string __panelToBeActivated)
    {
        Login_UI_Panel.SetActive(__panelToBeActivated.Equals(Login_UI_Panel.name));
        GameOptions_UI_Panel.SetActive(__panelToBeActivated.Equals(GameOptions_UI_Panel.name));
        GameOptions_UI_Panel.SetActive(__panelToBeActivated.Equals(GameOptions_UI_Panel.name));
        CreateRoom_UI_Panel.SetActive(__panelToBeActivated.Equals(CreateRoom_UI_Panel.name));
        InsideRoom_UI_Panel.SetActive(__panelToBeActivated.Equals(InsideRoom_UI_Panel.name));
        RoomList_UI_Panel.SetActive(__panelToBeActivated.Equals(RoomList_UI_Panel.name));
        JoinRandomRoom_UI_Panel.SetActive(__panelToBeActivated.Equals(JoinRandomRoom_UI_Panel.name));
    }
    #endregion

    #region __UI Callbacks__
    //Called from first login button
    public void OnLoginButtonClicked()
    {
        string _playerName = PlayerNameInput.text;
        if (!string.IsNullOrEmpty(_playerName))
        {
            PlayerPrefs.SetString(PrefKeys.PlayerName, _playerName);
            PhotonNetwork.LocalPlayer.NickName = _playerName;
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            print("Playername is invalid");
        }
    }

    //Create room, set name with guid if it not specified
    public void OnCreateRoomButtonClicked()
    {
        string _roomName = RoomNameInputField.text;

        if (string.IsNullOrEmpty(_roomName))
        {
            _roomName = "Room " + System.Guid.NewGuid();
        }

        RoomOptions _roomOptions = new RoomOptions();
        _roomOptions.MaxPlayers = 2;

        PhotonNetwork.CreateRoom(_roomName, _roomOptions);
    }

    //return to Game Options Panel
    public void OnCancelButtonClicked()
    {
        ActivatePanel(GameOptions_UI_Panel.name);
    }

    //Join lobby to be able to see rooms, and activate roomlist UI panel
    public void OnShowRoomListButtonClicked()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
        ActivatePanel(RoomList_UI_Panel.name);
    }

    //leave lobby and get back to Game Options Panel
    public void OnShowRoomListBackButtonClicked()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        ActivatePanel(GameOptions_UI_Panel.name);
    }

    //leave room from Inside Room Panel
    public void OnLeaveButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    //Try to join random room, if there is no room, create one
    public void OnJoinRandomRoomClicked()
    {
        ActivatePanel(JoinRandomRoom_UI_Panel.name);
        PhotonNetwork.JoinRandomRoom();
    }

    //Load game scene on button click
    public void OnStartGameButtonClicked()
    {
        //check if clicker is master client
        photonView.RPC("RPC_StartGame", RpcTarget.AllBufferedViaServer);
        PhotonNetwork.CurrentRoom.IsOpen = false;
    }

    [PunRPC]
    void RPC_StartGame()
    {
        if (photonView.IsMine)
        {
            FaderObject.SetActive(true);
            if (fadeRoutine != null)
                StopCoroutine(fadeRoutine);

            fadeRoutine = StartCoroutine(LoadSceneFadeRoutine());
        }
    }

    IEnumerator LoadSceneFadeRoutine()
    {
        bool _fadeActive = true;

        while (_fadeActive)
        {
            yield return new WaitForSecondsRealtime(0.02f);

            Fader.alpha += 0.05f;

            if (Mathf.Approximately(Fader.alpha, 1f))
            {
                _fadeActive = false;
            }
        }

        if (PhotonNetwork.IsMasterClient)
        {
            //load scene
            PhotonNetwork.LoadLevel(1);
        }
    }

    #endregion

    #region __Photon Callbacks__

    //First callaback on connection
    public override void OnConnected()
    {
        print("Connected to Internet");

    }

    //Activate Gameoptions UI panel
    public override void OnConnectedToMaster()
    {
        print(PhotonNetwork.LocalPlayer.NickName + "  Connected to photon");
        ActivatePanel(GameOptions_UI_Panel.name);
    }

    public override void OnCreatedRoom()
    {
        print(PhotonNetwork.CurrentRoom.Name + "  is created");
    }

    //activate Inside Room Panel and instantia playerlist prefab to indicate name and if it is local player
    public override void OnJoinedRoom()
    {
        print(PhotonNetwork.LocalPlayer + " joined to " + PhotonNetwork.CurrentRoom.Name);

        //set room info text
        ActivatePanel(InsideRoom_UI_Panel.name);

        //Activate start button if local player is master client
        if (PhotonNetwork.LocalPlayer.IsMasterClient && PhotonNetwork.CurrentRoom.MaxPlayers == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            StartGameButton.SetActive(true);
        }
        else
        {
            StartGameButton.SetActive(false);
        }

        RoomInfoText.text = "Room Name:  " + PhotonNetwork.CurrentRoom.Name + " - " + PhotonNetwork.CurrentRoom.PlayerCount +
                            " / " + PhotonNetwork.CurrentRoom.MaxPlayers + " Players";

        //create dictionary if it not already
        if (playerListGameObjectsDict == null)
        {
            playerListGameObjectsDict = new Dictionary<int, GameObject>();
        }

        //Instantiate Player list prefab gameobjects
        foreach (Player _player in PhotonNetwork.PlayerList)
        {
            GameObject _playerListGameObject = Instantiate(PlayerListPrefab, PlayerListContentParent.transform);
            _playerListGameObject.transform.localScale = Vector3.one;
            //Set Player Name
            _playerListGameObject.transform.Find(UIPrefabStrings.PlayerName).GetComponent<Text>().text = _player.NickName;

            //activate and deactivate YOU indicator
            if (_player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                _playerListGameObject.transform.Find(UIPrefabStrings.PlayerIndicator).gameObject.SetActive(true);
            }
            else
            {
                _playerListGameObject.transform.Find(UIPrefabStrings.PlayerIndicator).gameObject.SetActive(false);
            }

            //add to player list dictionary
            playerListGameObjectsDict.Add(_player.ActorNumber, _playerListGameObject);
        }
    }

    //Same as above, but just for __newPlayer
    public override void OnPlayerEnteredRoom(Player __newPlayer)
    {
        //update room info text
        RoomInfoText.text = "Room Name:  " + PhotonNetwork.CurrentRoom.Name + " - " + PhotonNetwork.CurrentRoom.PlayerCount +
                            " / " + PhotonNetwork.CurrentRoom.MaxPlayers + " Players";

        GameObject _playerListGameObject = Instantiate(PlayerListPrefab, PlayerListContentParent.transform);
        _playerListGameObject.transform.localScale = Vector3.one;
        //Set Player Name
        _playerListGameObject.transform.Find(UIPrefabStrings.PlayerName).GetComponent<Text>().text = __newPlayer.NickName;

        //activate and deactivate YOU indicator
        if (__newPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            _playerListGameObject.transform.Find(UIPrefabStrings.PlayerIndicator).gameObject.SetActive(true);
        }
        else
        {
            _playerListGameObject.transform.Find(UIPrefabStrings.PlayerIndicator).gameObject.SetActive(false);
        }

        //add to player list dictionary
        playerListGameObjectsDict.Add(__newPlayer.ActorNumber, _playerListGameObject);

        //Activate start button if local player is master client
        if (PhotonNetwork.LocalPlayer.IsMasterClient && PhotonNetwork.CurrentRoom.MaxPlayers == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            StartGameButton.SetActive(true);
        }
        else
        {
            StartGameButton.SetActive(false);
        }
    }

    //destroy __otherplayer list prefab gameobject and remove from playerlist dictionary on player leave
    //Activate start button if host left the game and local player is the new host
    public override void OnPlayerLeftRoom(Player __otherPlayer)
    {
        //update room info text
        RoomInfoText.text = "Room Name:  " + PhotonNetwork.CurrentRoom.Name + " - " + PhotonNetwork.CurrentRoom.PlayerCount +
                            " / " + PhotonNetwork.CurrentRoom.MaxPlayers + " Players";

        Destroy(playerListGameObjectsDict[__otherPlayer.ActorNumber].gameObject);
        playerListGameObjectsDict.Remove(__otherPlayer.ActorNumber);

        //Activate start button if local player is the new master client
        if (PhotonNetwork.LocalPlayer.IsMasterClient && PhotonNetwork.CurrentRoom.MaxPlayers == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            StartGameButton.SetActive(true);
        }
        else
        {
            StartGameButton.SetActive(false);
        }
    }

    //destroy all list prefab gameobjects and clear player list dictionary
    public override void OnLeftRoom()
    {
        ActivatePanel(GameOptions_UI_Panel.name);

        foreach (GameObject _playerListGameObject in playerListGameObjectsDict.Values)
        {
            Destroy(_playerListGameObject);
        }

        playerListGameObjectsDict.Clear();
        playerListGameObjectsDict = null;
    }

    //create room lists, instantiate prefabs to show other players all rooms, destroy unused room objects
    public override void OnRoomListUpdate(List<RoomInfo> __roomList)
    {
        //clear dicts before entering lobby
        ClearRoomListView();

        //for every room in lobby, check if its unused and then remove, || update room || create a new listing if it is new
        foreach (RoomInfo _room in __roomList)
        {
            print(_room.Name);

            //remove from dictionary if room is inactive
            if (!_room.IsOpen || !_room.IsVisible || _room.RemovedFromList)
            {
                if (cachedRoomDict.ContainsKey(_room.Name))
                {
                    cachedRoomDict.Remove(_room.Name);
                }
            }
            else
            {
                //update if it still exists
                if (cachedRoomDict.ContainsKey(_room.Name))
                {
                    cachedRoomDict[_room.Name] = _room;
                }
                else
                {
                    //add to dictionary if it is new
                    cachedRoomDict.Add(_room.Name, _room);
                }
            }
        }

        //create a room listing to fit in scroll view content section, instantiate a room listing prefab gameobject in scrollview/content,
        //set its name, set/update player count, add functionality to join button, add prefab gameobject to room listing dictionary
        foreach (RoomInfo _roomInfo in cachedRoomDict.Values)
        {
            //instantiate roomlist prefab as scroolview/content's child
            GameObject _roomListEntryGameObject = Instantiate(RoomListEntryPrefab, RoomListParentGameObject.transform);
            //set its scale to 1 to avoid scaling issues
            _roomListEntryGameObject.transform.localScale = Vector3.one;

            //set room name
            _roomListEntryGameObject.transform.Find(UIPrefabStrings.RoomNameText).GetComponent<Text>().text = _roomInfo.Name;
            // set || update player count text
            _roomListEntryGameObject.transform.Find(UIPrefabStrings.RoomPlayersText).GetComponent<Text>().text = _roomInfo.PlayerCount + " / " + _roomInfo.MaxPlayers;
            //add functionality to join button
            _roomListEntryGameObject.transform.Find(UIPrefabStrings.JoinRoomButton).GetComponent<Button>().onClick.AddListener(() => OnJoinRoomButtonClicked(_roomInfo.Name));

            //add to room listing prefabs gameobject dictionary
            roomListGameObjects.Add(_roomInfo.Name, _roomListEntryGameObject);
        }
    }

    //clear room list dict, destroy lobby prefabs and clear room list dict
    public override void OnLeftLobby()
    {
        ClearRoomListView();
        cachedRoomDict.Clear();
    }

    //create random room if  join random room fails
    public override void OnJoinRandomFailed(short returnCode, string __message)
    {
        print(__message);

        string _roomName = "Room " + System.Guid.NewGuid();

        RoomOptions _roomOptions = new RoomOptions();
        _roomOptions.MaxPlayers = 2;

        PhotonNetwork.CreateRoom(_roomName, _roomOptions);
    }

    #endregion

    #region __Private Methods__

    //leave lobby to be able to join rooms, and enter room by name (called from room listing prefab gameobject)
    private void OnJoinRoomButtonClicked(string __roomName)
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        PhotonNetwork.JoinRoom(__roomName);
    }

    //clear room listing prefab dictionary, and destroy its gameobjects
    private void ClearRoomListView()
    {
        foreach (GameObject _roomListGamePrefabGameObject in roomListGameObjects.Values)
        {
            Destroy(_roomListGamePrefabGameObject);
        }
        roomListGameObjects.Clear();
    }

    #endregion
}
