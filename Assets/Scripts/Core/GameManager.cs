using UnityEngine;
using UnityEngine.SceneManagement;

namespace EndlessRunner
{
    public enum GameState
    {
        Menu,
        Playing,
        GameOver
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] GameBalance balance;
        [SerializeField] LaneController player;
        [SerializeField] ObstacleSpawner spawner;
        [SerializeField] ScoreTracker score;
        [SerializeField] HudView hud;

        readonly InputReader confirmInput = new InputReader();

        float stateEnterTime;

        public GameState State { get; private set; }
        public GameBalance Balance => balance;
        public ObstacleSpawner Spawner => spawner;

        void Awake()
        {
            Instance = this;
            Application.targetFrameRate = 60;
        }

        void Start()
        {
            EnterMenu();
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        void Update()
        {
            if (State == GameState.Playing)
            {
                hud.SetScore(score.Score);
                return;
            }

            if (Time.unscaledTime - stateEnterTime < 0.4f) return;
            if (!confirmInput.ReadConfirm()) return;

            if (State == GameState.Menu) StartRun();
            else RestartRun();
        }

        void EnterMenu()
        {
            State = GameState.Menu;
            stateEnterTime = Time.unscaledTime;
            SetSystemsPlaying(false);
            player.ResetLane();
            spawner.ResetRun();
            score.ResetRun();
            hud.ShowMenu();
        }

        public void StartRun()
        {
            State = GameState.Playing;
            stateEnterTime = Time.unscaledTime;
            SetSystemsPlaying(true);
            hud.ShowPlaying();
        }

        public void PlayerDied()
        {
            if (State != GameState.Playing) return;

            State = GameState.GameOver;
            stateEnterTime = Time.unscaledTime;
            SetSystemsPlaying(false);
            hud.ShowGameOver(score.Score);
        }

        void RestartRun()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        void SetSystemsPlaying(bool value)
        {
            player.IsPlaying = value;
            spawner.IsPlaying = value;
            score.IsPlaying = value;
        }
    }
}
