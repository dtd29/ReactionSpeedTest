using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviourPunCallbacks
{

    public void ReturnLobby()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        Debug.Log("방 나감 완료, 로비로 이동");
        SceneManager.LoadScene(0);
    }
}