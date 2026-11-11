using UnityEngine;

namespace EndlessRunner
{
    public class InputReader
    {
        Vector2 touchStart;
        bool tracking;

        public int ReadLaneStep()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) return -1;
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) return 1;

            if (Input.touchCount == 0) return 0;

            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                touchStart = touch.position;
                tracking = true;
                return 0;
            }

            if (!tracking) return 0;
            if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled) return 0;

            tracking = false;
            float delta = touch.position.x - touchStart.x;
            float threshold = Mathf.Max(40f, Screen.width * 0.06f);
            if (Mathf.Abs(delta) < threshold) return 0;
            return delta > 0f ? 1 : -1;
        }

        public bool ReadConfirm()
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) return true;
            if (Input.GetMouseButtonDown(0)) return true;

            for (int i = 0; i < Input.touchCount; i++)
            {
                if (Input.GetTouch(i).phase == TouchPhase.Began) return true;
            }

            return false;
        }
    }
}
