using UnityEngine;
using UnityEngine.UI;

namespace EndlessRunner
{
    public class HudView : MonoBehaviour
    {
        [SerializeField] Text scoreText;
        [SerializeField] GameObject menuPanel;
        [SerializeField] GameObject gameOverPanel;
        [SerializeField] Text gameOverScoreText;

        public void ShowMenu()
        {
            menuPanel.SetActive(true);
            gameOverPanel.SetActive(false);
            scoreText.text = "0";
        }

        public void ShowPlaying()
        {
            menuPanel.SetActive(false);
            gameOverPanel.SetActive(false);
        }

        public void ShowGameOver(int score)
        {
            gameOverPanel.SetActive(true);
            gameOverScoreText.text = "SCORE " + score;
        }

        public void SetScore(int score)
        {
            scoreText.text = score.ToString();
        }
    }
}
