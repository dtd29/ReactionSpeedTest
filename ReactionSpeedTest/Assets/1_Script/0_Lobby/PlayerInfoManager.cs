using UnityEngine;
using System.Runtime.InteropServices;

[System.Serializable]
public class UserData
{
    public string email;
    public string name;
    public string uid;
    public int score;
}

public class PlayerInfoManager : Singleton<PlayerInfoManager>
{
    public UserData myUserData;

    [DllImport("__Internal")]
    private static extern void SaveScoreToFirestore(string uid, string name, int score);

    void Start()
    {
        myUserData = null;
    }
    
    //점수 저장
    public void SaveScore()
    {
        if (myUserData != null)
        {
            SaveScoreToFirestore(myUserData.uid, myUserData.name, myUserData.score); //jslib
            Debug.Log("점수 저장 시도: " + myUserData.uid + myUserData.score);
        }
        else
        {
            Debug.LogWarning("사용자 정보 없음: 로그인 먼저 해야 함");
        }
    }
}
