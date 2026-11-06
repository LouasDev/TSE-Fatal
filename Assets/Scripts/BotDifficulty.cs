using UnityEngine;

namespace TSEFatal
{
    public enum Difficulty { Facil, Normal, Dificil }

    /// <summary>Ajustes da IA por dificuldade. NORMAL repete os valores que o bot já usava.</summary>
    public static class BotDifficulty
    {
        public struct Tuning
        {
            public float ReactionMin, ReactionMax;   // atraso ao entrar no alcance
            public float ThinkMin, ThinkMax;         // intervalo entre decisões
            public float AttackChance;               // chance de socar quando está perto
            public float DodgeChance;                // chance de esquivar quando está perto
            public float ComboChance;                // chance de emendar o próximo soco
            public float PauseMin, PauseMax;         // respiro depois de um soco
            public float SpecialScale;               // multiplica a chance de usar especiais
        }

        private const string PrefKey = "tse_fatal_dificuldade";
        private static Difficulty? current;

        public static Difficulty Current
        {
            get => current ??= (Difficulty)Mathf.Clamp(PlayerPrefs.GetInt(PrefKey, (int)Difficulty.Normal), 0, 2);
            set { current = value; PlayerPrefs.SetInt(PrefKey, (int)value); }
        }

        public static Difficulty Next(Difficulty d, int delta) => (Difficulty)(((int)d + delta + 3) % 3);

        public static string Label(Difficulty d) => d switch
        {
            Difficulty.Facil => "FÁCIL",
            Difficulty.Dificil => "DIFÍCIL",
            _ => "NORMAL"
        };

        public static string Description(Difficulty d) => d switch
        {
            Difficulty.Facil => "Bot lento e distraído. Bom para aprender os golpes.",
            Difficulty.Dificil => "Reage rápido, emenda combos e abusa dos especiais.",
            _ => "Reage como um jogador comum e respira entre os golpes."
        };

        public static Color Tint(Difficulty d) => d switch
        {
            Difficulty.Facil => new Color(.45f, .95f, .45f),
            Difficulty.Dificil => new Color(1f, .35f, .3f),
            _ => new Color(1f, .82f, .3f)
        };

        public static Tuning Get(Difficulty d) => d switch
        {
            Difficulty.Facil => new Tuning
            {
                ReactionMin = .5f, ReactionMax = .8f, ThinkMin = .45f, ThinkMax = .75f,
                AttackChance = .32f, DodgeChance = .04f, ComboChance = .05f,
                PauseMin = .85f, PauseMax = 1.3f, SpecialScale = .5f
            },
            Difficulty.Dificil => new Tuning
            {
                ReactionMin = .12f, ReactionMax = .24f, ThinkMin = .18f, ThinkMax = .32f,
                AttackChance = .66f, DodgeChance = .18f, ComboChance = .55f,
                PauseMin = .3f, PauseMax = .55f, SpecialScale = 1.5f
            },
            _ => new Tuning
            {
                ReactionMin = .25f, ReactionMax = .45f, ThinkMin = .28f, ThinkMax = .5f,
                AttackChance = .5f, DodgeChance = .12f, ComboChance = .25f,
                PauseMin = .5f, PauseMax = .9f, SpecialScale = 1f
            }
        };
    }
}
