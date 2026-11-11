using UnityEngine;

namespace EndlessRunner
{
    [CreateAssetMenu(menuName = "Endless Runner/Balance", fileName = "GameBalance")]
    public class GameBalance : ScriptableObject
    {
        [Header("Lane")]
        public float[] laneX = { -1.8f, 0f, 1.8f };
        public float laneMoveSpeed = 14f;

        [Header("Speed")]
        public float startSpeed = 6.5f;
        public float maxSpeed = 13f;
        public float rampDuration = 90f;

        [Header("Gap")]
        public float startGap = 9f;
        public float minGap = 6.5f;

        [Header("Spawn")]
        public float spawnY = 6.5f;
        public float despawnY = -7f;
        public float doubleLaneChanceStart = 0.15f;
        public float doubleLaneChanceMax = 0.55f;

        [Header("Score")]
        public float pointsPerSecond = 10f;

        [Header("Hitbox")]
        public float hitboxScale = 0.85f;
    }
}
