using DG.Tweening;
using TMPro;
using UnityEngine;

public class PlayerScoreSetting : MonoBehaviour
{
    [SerializeField]TMP_Text myScoreText;
    [SerializeField]TMP_Text GetScoreText;

    private int myScore;
    
    void Start()
    {
        myScore = PlayerInfoManager.Instance.myUserData.score;
        Debug.Log("현재 점수 : " + myScore);
        myScoreText.text = myScore.ToString();
        GetScoreText.text ="";
    }

    //점수 획득 (애니메이션 포함)
    public void GetScore(int num)
    {
        PlayerInfoManager.Instance.myUserData.score += num;
        TextMeshProUGUI tmpro = myScoreText.gameObject.GetComponent<TextMeshProUGUI>();
        GetScoreText.text = num.ToString();

        Sequence sequence = DOTween.Sequence();
        sequence.PrependInterval(1f);
        sequence.Append(GetScoreText.gameObject.transform.DOMove(myScoreText.gameObject.transform.position, 1f)
        .SetEase(Ease.OutCubic));
        sequence.Join(GetScoreText.DOScale(0, 1f).OnComplete(() => { GetScoreText.text = ""; }));
        sequence.Append(tmpro.DOCounter(myScore, myScore + num, 1f).SetDelay(0.3f));

        PlayerInfoManager.Instance.SaveScore();
    }
}
