using System.Collections.Generic;
using UnityEngine;

namespace EndlessRunner
{
    public class ObstacleSpawner : MonoBehaviour
    {
        static readonly Color ObstacleColor = new Color(0.95f, 0.42f, 0.36f);

        [SerializeField] GameBalance balance;

        readonly List<GameObject> obstacles = new List<GameObject>();

        IDifficultyCurve curve;
        float timer;
        float elapsed;
        int lastFreeLane = -1;
        int freeLaneStreak;

        public bool IsPlaying { get; set; }
        public float Elapsed => elapsed;
        public float CurrentSpeed => curve == null ? balance.startSpeed : curve.Speed(elapsed);
        public float CurrentGap => curve == null ? balance.startGap : curve.TargetGap(elapsed);

        void Awake()
        {
            curve = new LinearDifficultyCurve(balance);
        }

        public void ResetRun()
        {
            for (int i = 0; i < obstacles.Count; i++)
            {
                if (obstacles[i] != null) Destroy(obstacles[i]);
            }

            obstacles.Clear();
            timer = 0f;
            elapsed = 0f;
            lastFreeLane = -1;
            freeLaneStreak = 0;
        }

        void Update()
        {
            if (!IsPlaying) return;

            elapsed += Time.deltaTime;
            float speed = curve.Speed(elapsed);
            float interval = Mathf.Max(0.15f, curve.TargetGap(elapsed) / speed);

            timer += Time.deltaTime;
            if (timer < interval) return;

            timer -= interval;
            SpawnRow(speed);
        }

        void SpawnRow(float speed)
        {
            int laneCount = balance.laneX.Length;
            float normalized = curve.Normalized(elapsed);
            float doubleChance = Mathf.Lerp(balance.doubleLaneChanceStart, balance.doubleLaneChanceMax, normalized);

            int blockCount = Random.value < doubleChance ? 2 : 1;
            blockCount = Mathf.Clamp(blockCount, 1, laneCount - 1);

            int freeLane = PickFreeLane(laneCount);

            var candidates = new List<int>(laneCount);
            for (int lane = 0; lane < laneCount; lane++)
            {
                if (lane != freeLane) candidates.Add(lane);
            }

            Shuffle(candidates);

            for (int i = 0; i < blockCount && i < candidates.Count; i++)
            {
                SpawnObstacle(candidates[i], speed);
            }

            PruneDead();
        }

        int PickFreeLane(int laneCount)
        {
            int lane = Random.Range(0, laneCount);

            if (lane == lastFreeLane)
            {
                freeLaneStreak++;
                if (freeLaneStreak >= 2)
                {
                    lane = (lastFreeLane + Random.Range(1, laneCount)) % laneCount;
                    freeLaneStreak = 0;
                }
            }
            else
            {
                freeLaneStreak = 0;
            }

            lastFreeLane = lane;
            return lane;
        }

        void SpawnObstacle(int lane, float speed)
        {
            var go = new GameObject("Obstacle");
            go.transform.SetParent(transform, false);
            go.transform.position = new Vector3(balance.laneX[lane], balance.spawnY, 0f);

            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = RuntimeSpriteFactory.Solid(32, 32, ObstacleColor, 32f, 6f);
            renderer.sortingOrder = 5;

            var collider = go.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(0.9f, 0.9f);

            var obstacle = go.AddComponent<Obstacle>();
            obstacle.Init(speed, balance.despawnY);

            obstacles.Add(go);
        }

        void PruneDead()
        {
            obstacles.RemoveAll(item => item == null);
        }

        static void Shuffle(List<int> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                int temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
    }
}
