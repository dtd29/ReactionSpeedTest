using TMPro;
using UnityEngine;
using Photon.Pun;

public class PlayerNameSetting : MonoBehaviour
{
    public TMP_Text myNameText;
    public TMP_Text otherNameText;

    void Start()
    {
        myNameText.text = PhotonNetwork.LocalPlayer.NickName;
         if (PhotonNetwork.PlayerListOthers.Length > 0)
        {
            otherNameText.text = PhotonNetwork.PlayerListOthers[0].NickName;
        }
    }
}
