using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun; 
using DG.Tweening;             

public class QuicknessTest : MonoBehaviourPun
{   
    [Header("패널")]
    [SerializeField] Image myPanel;
    [SerializeField] Image otherPanel;

    [Header("텍스트")]
    [SerializeField] TMP_Text myTimeText;
    [SerializeField] TMP_Text otherTimeText;
    [SerializeField] TMP_Text currnetStateText;
    [SerializeField] TMP_Text myScoreText;
    [SerializeField] TMP_Text GetScoreText;

    [Header("돌아가기 버튼")]
    [SerializeField] GameObject returnBtn;

    [Header("스코어 매니저")]
    [SerializeField] PlayerScoreSetting scoreManager;
    
    private float delayTime = 0f;
    private float otherDelayTime = 0f;
    private bool isClickAble = false;
    private bool isStartGame = false;
    private bool isClicked = false;
    private bool isReceivedOther = false;
    private bool myFoul = false;
    private bool otherFoul = false;
    private bool isShowResult = false;

    private Coroutine CRandomTime;
    private Coroutine CWaitOther;

    void Start()
    {
        returnBtn.SetActive(false);
        delayTime = 0f;
        isClickAble = false;
        isStartGame = false;
        isClicked = false;
        myPanel.color = Color.white;
        myTimeText.text = "";
        currnetStateText.text = "";
    }

    //uiManager에서 활성화
    public void StartGame(){
        isStartGame = true;
        myPanel.color = Color.red;
        CRandomTime = StartCoroutine(RandomTime());
    }
    
    void Update()
    {
        if(isStartGame){
            if(Input.GetMouseButtonDown(0)){
                isClicked = true;
                myPanel.color = Color.white;
                if(isClickAble){
                    myTimeText.text = Mathf.FloorToInt(delayTime*1000).ToString()+"ms";
                    isClickAble = false;
                    isStartGame = false;
                    SendDelayTime(delayTime);
                }
                else{
                    myTimeText.text = "실격";
                    StopCoroutine(CRandomTime);
                    myFoul = true;
                    SendDelayTime(999999);
                }
            }
        }
    }
    
    //시간 랜덤으로 초록창 띄우기
    IEnumerator RandomTime(){
        float num = Random.Range(0.4f,3);
        yield return new WaitForSeconds(num);
        isClickAble = true;
        myPanel.color = Color.green;
        StartCoroutine(DelayTimeGesan());
    }

    //지연시간 계산
    IEnumerator DelayTimeGesan(){
        while(isClickAble){
            yield return new WaitForSeconds(0.001f);
            delayTime += 0.001f;
        }
    }

    IEnumerator WaitOther(){
        for(int i = 0; i<6; i++){
            currnetStateText.text = "상대방을 기다리는중..";
            yield return new WaitForSeconds(1f);
        }
        otherFoul = true;
        otherTimeText.text = "통신끊김";
        if(!isShowResult) ShowWinOrLose();
    }

    //상대에게 내 지연시간 보내기
    public void SendDelayTime(float num)
    {
        photonView.RPC("ReceiveDelayTime", RpcTarget.Others, num);
        if(!isReceivedOther) CWaitOther = StartCoroutine(WaitOther());
        else if(!isShowResult) ShowWinOrLose();
    }

    [PunRPC]
    void ReceiveDelayTime(float receivedDelay)
    {
        isReceivedOther = true;
        otherDelayTime = receivedDelay;
        if(otherDelayTime>999991){
            otherTimeText.text = "실격";
            otherFoul = true;
        }
        else{
            otherTimeText.text = Mathf.FloorToInt(receivedDelay*1000).ToString()+"ms";
        }
        Debug.Log($"상대방으로부터 받은 delayTime: {receivedDelay}");
        if(isClicked){
            if(CWaitOther != null)
                StopCoroutine(CWaitOther);
            if(!isShowResult)
                ShowWinOrLose();
        }
        
    }

    void ShowWinOrLose(){
        if(!isShowResult){
            isShowResult = true;
            //Debug.Log($"{myFoul},{otherFoul},{delayTime},{otherDelayTime} ");
            
            if (myPanel == null || !myPanel.gameObject.activeInHierarchy)
            {
                Debug.LogWarning("DOTween 대상이 사라짐: myPanel 트윈 건너뜀");
                return;
            }

            if(myFoul) delayTime = 999999;
            if(otherFoul) otherDelayTime = 999999;
                
            if (delayTime == otherDelayTime)
            {
                myPanel.color = Color.white;
                otherPanel.color = Color.white;
                currnetStateText.text = "무승부";
            }
            else if (delayTime < otherDelayTime)
            {
                myPanel.color = Color.green;
                otherPanel.color = Color.red;
                currnetStateText.text = "승리";
                scoreManager.GetScore(500);
            }
            else if(delayTime > otherDelayTime)
            {
                myPanel.color = Color.red;
                otherPanel.color = Color.green;
                currnetStateText.text = "패배";
                scoreManager.GetScore(-500);
            }

            if (myPanel != null){
                RectTransform rt = myPanel.GetComponent<RectTransform>();

                DOTween.Kill(rt);
                DOTween.To(() => rt.offsetMax, x => rt.offsetMax = x, new Vector2(-960, 0), 1.5f)
                    .SetTarget(rt)
                    .OnComplete(() => returnBtn.SetActive(true));
            }

        }
    }

}
