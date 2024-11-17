using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Management;

//pifight lobby manager
public class LobbyManager : NetworkSceneManager<LobbyManager> {
    private GameObject[] fileds = new GameObject[2];
    public Text txtStatus;
    public InputField ifRoomName;
    public Button btnCreateRoom, btnJoinRoom, btnJoinRandomRoom, btnStart, btnLeaveRoom;

    protected override void Awake() {
        base.Awake();
        SetScreenTransitionEffect("NetworkFade", "Canvas");

        fileds[0] = Resources.Load("PIFight/Fields/Square") as GameObject;
        fileds[1] = Resources.Load("PIFight/Fileds/Circle") as GameObject;

        btnCreateRoom.interactable = false;
        btnJoinRoom.interactable = false;
        btnJoinRandomRoom.interactable = false;
        btnStart.interactable = false;
        btnLeaveRoom.interactable = false;
        txtStatus.text = "Connecting to server...";
        Debug.Log("Connecting to server...");

        PhotonNetwork.GameVersion = PIAManager.Inst.GameVersion;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    private void Start() {
        Instantiate(fileds[0], Vector3.zero, Quaternion.identity);
        if (PhotonNetwork.InRoom) PhotonNetwork.LeaveRoom();
    }

    private void Update() {
        if (PhotonNetwork.InRoom) {
            if (PhotonNetwork.IsMasterClient) {
                if (PhotonNetwork.CurrentRoom.PlayerCount == 2) btnStart.interactable = true;
                else btnStart.interactable = false;
            }
        }
    }

    //network connected complete
    public override void OnConnectedToMaster() {
        btnCreateRoom.interactable = true;
        btnJoinRoom.interactable = true;
        btnJoinRandomRoom.interactable = true;
        btnStart.interactable = false;
        btnLeaveRoom.interactable = false;

        txtStatus.text = "Connected to server!";
        Debug.Log("Connected to server!");
    }

    //create room
    public void CreateRoom() {
        string strName = ifRoomName.text;

        btnCreateRoom.interactable = false;
        btnJoinRoom.interactable = false;
        btnJoinRandomRoom.interactable = false;
        btnStart.interactable = false;
        btnLeaveRoom.interactable = false;
        ifRoomName.interactable = false;

        if (PhotonNetwork.IsConnected) {
            if (strName == "") {
                ImpossibleName();
                return;
            }
            foreach (char c in strName) {
                if (c == ' ' || c == '\t' || c == '\n') {
                    ImpossibleName();
                    return;
                }
            }

            txtStatus.text = "Creating a room...";
            Debug.Log("Creating a room...");

            PhotonNetwork.CreateRoom(ifRoomName.text, new RoomOptions { MaxPlayers = 2 });
        }
        else {
            txtStatus.text = "Connection Disabled...";
            Debug.Log("Connection Disabled...");

            PhotonNetwork.ConnectUsingSettings();
        }
    }

    //create room complete
    public override void OnCreatedRoom() {
        btnCreateRoom.interactable = false;
        btnJoinRoom.interactable = false;
        btnJoinRandomRoom.interactable = false;
        btnLeaveRoom.interactable = true;
        ifRoomName.interactable = false;

        txtStatus.text = "Created a room!";
        Debug.Log("Created a room!");
    }

    //create room failed
    public override void OnCreateRoomFailed(short returnCode, string message) {
        btnCreateRoom.interactable = true;
        btnJoinRoom.interactable = true;
        btnJoinRandomRoom.interactable = true;
        btnLeaveRoom.interactable = false;
        ifRoomName.interactable = true;

        txtStatus.text = "Creation Disabled...";
        Debug.Log("Creation Disabled...");
    }

    //join room
    public void JoinRoom() {
        string strName = ifRoomName.text;

        btnCreateRoom.interactable = false;
        btnJoinRoom.interactable = false;
        btnJoinRandomRoom.interactable = false;
        btnStart.interactable = false;
        btnLeaveRoom.interactable = false;
        ifRoomName.interactable = false;

        if (PhotonNetwork.IsConnected) {
            if (strName == "") {
                ImpossibleName();
                return;
            }
            foreach (char c in strName) {
                if (c == ' ' || c == '\t' || c == '\n') {
                    ImpossibleName();
                    return;
                }
            }

            txtStatus.text = "Connecting to room...";
            Debug.Log("Connecting to room...");

            PhotonNetwork.JoinRoom(strName);
        }
        else {
            txtStatus.text = "Connection Disabled...";
            Debug.Log("Connection Disabled...");

            PhotonNetwork.ConnectUsingSettings();
        }
    }

    //join random room
    public void JoinRandomRoom() {
        btnCreateRoom.interactable = false;
        btnJoinRoom.interactable = false;
        btnJoinRandomRoom.interactable = false;
        btnStart.interactable = false;
        btnLeaveRoom.interactable = false;
        ifRoomName.interactable = false;

        if (PhotonNetwork.IsConnected) {
            txtStatus.text = "Connecting to room...";
            Debug.Log("Connecting to room...");

            PhotonNetwork.JoinRandomRoom();
        }
        else {
            txtStatus.text = "Connection Disabled...";
            Debug.Log("Connection Disabled...");

            PhotonNetwork.ConnectUsingSettings();
        }
    }

    //join room complete
    public override void OnJoinedRoom() {
        btnCreateRoom.interactable = false;
        btnJoinRoom.interactable = false;
        btnJoinRandomRoom.interactable = false;
        btnLeaveRoom.interactable = true;
        ifRoomName.interactable = false;

        txtStatus.text = PhotonNetwork.CurrentRoom.Name;
        Debug.Log("Connected to room!");
    }

    //join room failed
    public override void OnJoinRoomFailed(short returnCode, string message) {
        btnCreateRoom.interactable = true;
        btnJoinRoom.interactable = true;
        btnJoinRandomRoom.interactable = true;
        btnLeaveRoom.interactable = false;
        ifRoomName.interactable = true;

        txtStatus.text = "Room connection Disabled...";
        Debug.Log("Room connection Disabled...");
    }

    //join random room failed
    public override void OnJoinRandomFailed(short returnCode, string message) {
        btnCreateRoom.interactable = true;
        btnJoinRoom.interactable = true;
        btnJoinRandomRoom.interactable = true;
        btnLeaveRoom.interactable = false;
        ifRoomName.interactable = true;

        txtStatus.text = "Room connection Disabled...";
        Debug.Log("Room connection Disabled...");
    }


    //leave room
    public void LeaveRoom() {
        btnCreateRoom.interactable = false;
        btnJoinRoom.interactable = false;
        btnJoinRandomRoom.interactable = false;
        btnStart.interactable = false;
        btnLeaveRoom.interactable = false;
        ifRoomName.interactable = false;

        if (PhotonNetwork.InRoom) PhotonNetwork.LeaveRoom();
        else {
            txtStatus.text = "Not in the room...";
            Debug.Log("Not in the room...");
        }
    }

    //leave room complete
    public override void OnLeftRoom() {
        btnCreateRoom.interactable = true;
        btnJoinRoom.interactable = true;
        btnJoinRandomRoom.interactable = true;
        btnStart.interactable = false;
        btnLeaveRoom.interactable = false;
        ifRoomName.interactable = true;

        txtStatus.text = "Disconnecting the room";
        Debug.Log("Disconnecting the room");
    }

    //network disconnect
    public void Disconnect() {
        btnCreateRoom.interactable = false;
        btnJoinRoom.interactable = false;
        btnJoinRandomRoom.interactable = false;
        btnStart.interactable = false;
        btnLeaveRoom.interactable = false;

        txtStatus.text = "Disconnecting to server..";
        Debug.Log("Disconnecting to server..");

        PhotonNetwork.Disconnect();
        LoadScene(2);
    }

    //network disconnected
    public override void OnDisconnected(DisconnectCause cause) {
        btnCreateRoom.interactable = false;
        btnJoinRoom.interactable = false;
        btnJoinRandomRoom.interactable = false;
        btnStart.interactable = false;
        btnLeaveRoom.interactable = false;

        txtStatus.text = "Connection Disabled...";
        Debug.Log("Connection Disabled...");
    }

    //name check
    private void ImpossibleName() {
        btnCreateRoom.interactable = true;
        btnJoinRoom.interactable = true;
        btnJoinRandomRoom.interactable = true;
        btnLeaveRoom.interactable = false;
        ifRoomName.interactable = true;

        txtStatus.text = "Impossible Room Name...";
        Debug.Log("Impossible Room Name...");
    }

    public override void GameStart() {
        photonView.RPC("NetworkLoadScene", RpcTarget.All, 10);
    }

    public override void GameOver() { }

    public override void LoadScene(int sceneIdx) {
        base.LoadScene(sceneIdx);

        screenTransitionEffect.GetComponent<NetworkScreenTransitionEffect>().sceneIdx = sceneIdx;
        screenTransitionEffect.GetComponent<NetworkScreenTransitionEffect>().EndEffectFirst();
    }

    [PunRPC]
    public override void NetworkLoadScene(int sceneIdx) {
        base.NetworkLoadScene(sceneIdx);

        screenTransitionEffect.GetComponent<NetworkScreenTransitionEffect>().sceneIdx = sceneIdx;
        screenTransitionEffect.GetComponent<NetworkScreenTransitionEffect>().EndEffectFirst();
    }
}
