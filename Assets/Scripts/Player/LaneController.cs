using UnityEngine;

namespace EndlessRunner
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class LaneController : MonoBehaviour
    {
        [SerializeField] GameBalance balance;

        Rigidbody2D body;
        InputReader input;
        int laneIndex;
        float targetX;

        public bool IsPlaying { get; set; }
        public int LaneIndex => laneIndex;

        void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            input = new InputReader();

            var renderer = GetComponent<SpriteRenderer>();
            renderer.sprite = RuntimeSpriteFactory.Solid(28, 40, Palette.Player, 32f, 8f);
        }

        public void ResetLane()
        {
            laneIndex = Mathf.Clamp(1, 0, balance.laneX.Length - 1);
            targetX = balance.laneX[laneIndex];
            body.position = new Vector2(targetX, transform.position.y);
            body.velocity = Vector2.zero;
            transform.position = new Vector3(targetX, transform.position.y, 0f);
        }

        void Update()
        {
            if (!IsPlaying)
            {
                body.velocity = Vector2.zero;
                return;
            }

            int step = input.ReadLaneStep();
            if (step == 0) return;

            laneIndex = Mathf.Clamp(laneIndex + step, 0, balance.laneX.Length - 1);
            targetX = balance.laneX[laneIndex];
        }

        void FixedUpdate()
        {
            if (!IsPlaying) return;

            float vx = (targetX - body.position.x) * balance.laneMoveSpeed;
            body.velocity = new Vector2(vx, 0f);
        }
    }
}
