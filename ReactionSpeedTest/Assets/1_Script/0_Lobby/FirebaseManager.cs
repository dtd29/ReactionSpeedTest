using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    [SerializeField] LobbyMainPanel lobby;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text scoreText;

    [DllImport("__Internal")]
    private static extern void StartGoogleLogin();

    [DllImport("__Internal")]
    private static extern void LoadScoreFromFirestore(string uid);

    private UserData user;

    void Start()
    {
        if (PlayerInfoManager.Instance.myUserData != null)
        {
            user = PlayerInfoManager.Instance.myUserData;
            nameText.text = user.name;
            GetScore();
        }
    }

//구글 로그인
#region 
    public void LoginWithGoogle()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        StartGoogleLogin(); //jslib로 이동
#else
        Debug.Log("웹에서만 로그인 가능");
#endif
    }

    //js에서 호출
    public void OnGoogleLogin(string jsonData)
    {
        PlayerInfoManager.Instance.myUserData
        = JsonUtility.FromJson<UserData>(jsonData);
        user = PlayerInfoManager.Instance.myUserData;

        lobby.ConnectServer();
        nameText.text = user.name;
        GetScore();
    }

    //js에서 호출
    public void OnLoginFailed(string error)
    {
        Debug.LogWarning("로그인 실패: " + error);
    }

#endregion

//점수 불러오기, 다시 불러올 때 사용.
#region 
    public void GetScore()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        LoadScoreFromFirestore(user.uid); //jslib로 이동
#else
        Debug.Log("WebGL 환경에서만 작동합니다.");
#endif
    }

    // JS에서 호출
    public void OnScoreLoaded(int score)
    {
        Debug.Log("불러온 점수: " + score);
        PlayerInfoManager.Instance.myUserData.score = score;
        scoreText.text = score.ToString();
    }

    public void OnScoreLoadFailed(string error)
    {
        Debug.LogWarning("점수 불러오기 실패: " + error);
    }

#endregion
}
