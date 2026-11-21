using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
	[SerializeField] private TextMeshProUGUI scoreText;
	[SerializeField] private Image targetImage;
	private void Update()
	{
		timerText.text = GameManager.instance.timer.ToString("F0");
		scoreText.text = GameManager.instance.score.ToString();
		targetImage.sprite = GameManager.instance.sprite[(int)GameManager.instance.target];
	}
}
