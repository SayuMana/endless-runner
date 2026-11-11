using UnityEngine;

namespace EndlessRunner
{
    public class Obstacle : MonoBehaviour
    {
        float speed;
        float despawnY;

        void Awake()
        {
            var renderer = GetComponent<SpriteRenderer>();
            if (renderer != null) renderer.sprite = RuntimeSpriteFactory.Solid(32, 32, Palette.Obstacle, 32f, 6f);
        }

        public void Init(float moveSpeed, float despawn)
        {
            speed = moveSpeed;
            despawnY = despawn;
        }

        void Update()
        {
            transform.position += Vector3.down * (speed * Time.deltaTime);
            if (transform.position.y < despawnY) Destroy(gameObject);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            if (GameManager.Instance == null) return;
            GameManager.Instance.PlayerDied();
        }
    }
}
