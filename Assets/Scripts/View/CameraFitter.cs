using UnityEngine;

namespace EndlessRunner
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class CameraFitter : MonoBehaviour
    {
        [SerializeField] float baseSize = 6f;
        [SerializeField] float minHalfWidth = 2.6f;

        UnityEngine.Camera cam;

        void Awake()
        {
            cam = GetComponent<UnityEngine.Camera>();
            Apply();
        }

        void Update()
        {
            if (!Mathf.Approximately(cam.orthographicSize, TargetSize())) Apply();
        }

        float TargetSize()
        {
            float aspect = Mathf.Max(0.1f, cam.aspect);
            return Mathf.Max(baseSize, minHalfWidth / aspect);
        }

        void Apply()
        {
            cam.orthographicSize = TargetSize();
        }
    }
}
