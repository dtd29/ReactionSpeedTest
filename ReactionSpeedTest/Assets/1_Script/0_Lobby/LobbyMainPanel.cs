using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine.UI;
using KoreanTyper;
using System.Collections.Generic;
using ExitGames.Client.Photon;

public class LobbyMainPanel : MonoBehaviourPunCallbacks{
    public GameObject LoginPanel;
    public GameObject MatchingPanel;
    public GameObject StartPanel;
    public GameObject LoadingPanel;
    public GameObject exitBtn;
    public Text LoadingText;
    
    private Coroutine matchingTypingCoroutine;

    void Awake()
    {
        PhotonNetwork.NetworkingClient.LoadBalancingPeer.TransportProtocol = ConnectionProtocol.WebSocketSecure;
    }
    
    void Start()
    {
        SetActivePanel(LoginPanel);
        PhotonNetwork.GameVersion = "1.0.0";
        PhotonNetwork.AutomaticallySyncScene = true;
        exitBtn.SetActive(false);
        LoadingText.text = "로딩중입니다. 잠시만 기다려주세요.";
        Debug.Log("준비 완료");
    }

    //로그인 버튼 클릭
    public void ConnectServer(){
        PhotonNetwork.LocalPlayer.NickName = PlayerInfoManager.Instance.myUserData.name;
        PhotonNetwork.ConnectUsingSettings();
        SetActivePanel(LoadingPanel);
        Debug.Log("서버 연결 시도");
    }

    //ConnectUsingSettings 실행 후 자동 실행
    public override void OnConnectedToMaster(){
        SetActivePanel(StartPanel);
        Debug.Log("서버 연결 완료");
    }

    //시작 버튼 클릭
    public void StartBtn(){
        SetActivePanel(LoadingPanel);
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("방 참가 시도");
    }

    //방 입장 성공시 실행하는 콜백함수
    public override void OnJoinedRoom(){
        Debug.Log("방 참가 성공");
    }

    //JoinRandomRoom이 실패하면 호출하는 콜백함수
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("현재 방 존재 안함 새로 만듬");

        RoomOptions options = new RoomOptions { MaxPlayers = 2 };
        PhotonNetwork.CreateRoom(null, options, null);
    }

    //방 생성 성공하면 호출되는 콜백함수
    public override void OnCreatedRoom()
    {
        Debug.Log("방 생성 성공");
        Debug.Log("플레이어 탐색 시작");
        LoadingText.text = "";
        SetActivePanel(MatchingPanel);
        matchingTypingCoroutine = StartCoroutine(TypeLoop());
        exitBtn.SetActive(true);
    }

    //타이핑 해주는 코루틴 함수
    IEnumerator TypeLoop(){
        
        string message = "상대방을 찾는 중입니다..";
        while (true)
        {
            int totalSteps = message.GetTypingLength();

            for (int i = 0; i <= totalSteps; i++)
            {
                LoadingText.text = message.Typing(i);
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(0.8f);
            LoadingText.text = "";
            yield return new WaitForSeconds(0.07f);
        }
    }

    //상대방이 방에 들어오면 호출되는 콜백함수
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("상대 발견");
        if (matchingTypingCoroutine != null)
            StopCoroutine(matchingTypingCoroutine);
        LoadingText.text = "매칭완료";
        
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        StartCoroutine(Wait2Seconds());
    }

    //씬이 바로넘어가지 않기 위한 코루틴
    IEnumerator Wait2Seconds(){
        Debug.Log("상대가 방에 들어옴");
        if (photonView != null)
            photonView.RPC("ShowMatchingComplete", RpcTarget.All);
        yield return new WaitForSeconds(2f);
        AllPlayerFind();
    }

    //상대방이 호출하는 함수수
    [PunRPC]
    void ShowMatchingComplete()
    {
        LoadingText.text = "매칭완료";
    }

    public void AllPlayerFind()
    {
        Debug.Log("모두 찾았으니 게임 시작함");
        PhotonNetwork.LoadLevel("1_Main"); //상대와 함께 씬 이동
    }

    private void SetActivePanel(GameObject target)
    {
        target.SetActive(true);
        LoginPanel.SetActive(target == LoginPanel);
        LoadingPanel.SetActive(target == LoadingPanel);
        StartPanel.SetActive(target == StartPanel);
        MatchingPanel.SetActive(target == MatchingPanel);
    }

    //매칭 돌아가기 버튼
    public void LeaveMatchingBtn(){
        PhotonNetwork.AutomaticallySyncScene = false;
        if (matchingTypingCoroutine != null)
            StopCoroutine(matchingTypingCoroutine);
        LoadingText.text = "";
        PhotonNetwork.LeaveRoom();
    }

    //방에서 나왔을 때 자동으로 호출하는 콜백함수
    public override void OnLeftRoom()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        SetActivePanel(StartPanel);
        Debug.Log("매칭종료");
    }

    //서버 연결이 끊켰을 때 자동으로 호출되는 콜백함수
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError($"서버 연결 해제됨: {cause}");
        SetActivePanel(LoginPanel);
    }
}
