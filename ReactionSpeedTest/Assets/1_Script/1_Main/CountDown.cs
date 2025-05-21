using TMPro;
using UnityEngine;
using DG.Tweening;

public class CountDown : MonoBehaviour
{
    [SerializeField]QuicknessTest quicknessTest;
    private TMP_Text countdownText;

    void Awake()
    {
        countdownText = GetComponent<TMP_Text>();
    }

    void Start()
    {
        StartCountdown(3);
    }
    public void StartCountdown(int num)
    {
        if (num > 0)
        {
            transform.DOLocalMoveY(25, 0f);
            countdownText.DOFade(1, 0f);
            countdownText.text = num.ToString();

            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOLocalMoveY(-25f, 0.4f)).Append(countdownText.DOFade(0, 0.3f)) .OnComplete(() => StartCountdown(num - 1));
        }
        else
        {
            quicknessTest.StartGame();
            Debug.Log("게임시작");
        }
    }
}
