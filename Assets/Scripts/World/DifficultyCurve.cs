using UnityEngine;

namespace EndlessRunner
{
    public interface IDifficultyCurve
    {
        float Normalized(float elapsed);
        float Speed(float elapsed);
        float TargetGap(float elapsed);
    }

    public class LinearDifficultyCurve : IDifficultyCurve
    {
        readonly GameBalance balance;

        public LinearDifficultyCurve(GameBalance balance)
        {
            this.balance = balance;
        }

        public float Normalized(float elapsed)
        {
            if (balance.rampDuration <= 0f) return 1f;
            return Mathf.Clamp01(elapsed / balance.rampDuration);
        }

        public float Speed(float elapsed)
        {
            return Mathf.Lerp(balance.startSpeed, balance.maxSpeed, Normalized(elapsed));
        }

        public float TargetGap(float elapsed)
        {
            return Mathf.Lerp(balance.startGap, balance.minGap, Normalized(elapsed));
        }
    }
}
