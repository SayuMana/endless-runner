using UnityEngine;

namespace EndlessRunner
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class GroundView : MonoBehaviour
    {
        void Awake()
        {
            var renderer = GetComponent<SpriteRenderer>();
            renderer.sprite = RuntimeSpriteFactory.Solid(16, 16, Palette.Ground, 32f, 0f);
            renderer.drawMode = SpriteDrawMode.Simple;
            transform.localScale = new Vector3(28f, 0.36f, 1f);
        }
    }
}
