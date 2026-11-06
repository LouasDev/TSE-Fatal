using UnityEngine;

namespace TSEFatal
{
    /// <summary>Dados de um golpe. Knockback = (Base + Growth * dano%/100) / peso do alvo.</summary>
    public struct HitData
    {
        public float Damage;
        public float BaseKnockback;
        public float GrowthKnockback;
        public float AngleDegrees;  // 0 = horizontal, 90 = para cima
        public float SilenceSeconds; // > 0 bloqueia habilidades do alvo (Censura)

        public HitData(float damage, float baseKb, float growthKb, float angle, float silence = 0f)
        {
            Damage = damage; BaseKnockback = baseKb; GrowthKnockback = growthKb; AngleDegrees = angle; SilenceSeconds = silence;
        }
    }

    /// <summary>Estado global da luta: congelamento de impacto (hitstop) e tremor de câmera.</summary>
    public static class Combat
    {
        public static float HitStop { get; private set; }
        public static float ShakeAmount { get; private set; }
        private static float shakeTime;

        public static bool Frozen => HitStop > 0f;

        public static void AddHitStop(float seconds) => HitStop = Mathf.Max(HitStop, Mathf.Min(seconds, .16f));

        public static void Shake(float amount, float seconds)
        {
            ShakeAmount = Mathf.Max(ShakeAmount, amount);
            shakeTime = Mathf.Max(shakeTime, seconds);
        }

        public static void Tick(float dt)
        {
            HitStop = Mathf.Max(0f, HitStop - dt);
            shakeTime = Mathf.Max(0f, shakeTime - dt);
            if (shakeTime <= 0f) ShakeAmount = 0f;
        }

        public static void Reset() { HitStop = 0f; ShakeAmount = 0f; shakeTime = 0f; }

        public static Color DamageColor(float percent)
        {
            if (percent < 50f) return Color.Lerp(Color.white, new Color(1f, .92f, .35f), percent / 50f);
            if (percent < 100f) return Color.Lerp(new Color(1f, .92f, .35f), new Color(1f, .55f, .15f), (percent - 50f) / 50f);
            if (percent < 150f) return Color.Lerp(new Color(1f, .55f, .15f), new Color(.95f, .15f, .1f), (percent - 100f) / 50f);
            return Color.Lerp(new Color(.95f, .15f, .1f), new Color(.55f, .03f, .05f), Mathf.Clamp01((percent - 150f) / 60f));
        }
    }
}
