using UnityEngine;

namespace EndlessRunner
{
    public class ScoreTracker : MonoBehaviour
    {
        [SerializeField] GameBalance balance;

        public bool IsPlaying { get; set; }
        public int Score { get; private set; }
        public float Elapsed { get; private set; }

        public void ResetRun()
        {
            Score = 0;
            Elapsed = 0f;
        }

        void Update()
        {
            if (!IsPlaying) return;

            Elapsed += Time.deltaTime;
            Score = Mathf.FloorToInt(Elapsed * balance.pointsPerSecond);
        }
    }
}
