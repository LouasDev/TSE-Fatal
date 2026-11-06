using UnityEngine;

namespace TSEFatal
{
    public enum Difficulty { Facil, Normal, Dificil }

    /// <summary>Ajustes da IA por dificuldade. Até o FÁCIL revida; o DIFÍCIL joga quase sem respiro.</summary>
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
            public float SpecialRestMin, SpecialRestMax; // respiro depois de um especial
            public float RetreatChance;              // chance de recuar quando está perto
            public float ReactDodge;                 // chance de esquivar quando o rival começa um golpe por perto
            public float Aggression;                 // chance de perseguir no ar e de pressionar quem está fora da arena
            public float ComboBreak;                 // chance de sair de um golpe leve com a esquiva
            public bool WaitsOutAttacks;             // espera o golpe do rival acabar fora do alcance em vez de entrar nele
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
            Difficulty.Facil => "Bot mais lento, mas já revida. Bom para aprender os golpes.",
            Difficulty.Dificil => "Esquiva na hora, emenda combos e te caça fora da arena.",
            _ => "Reage rápido, emenda combos e pune quem erra."
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
                ReactionMin = .32f, ReactionMax = .5f, ThinkMin = .3f, ThinkMax = .45f,
                AttackChance = .48f, DodgeChance = .08f, ComboChance = .3f,
                PauseMin = .5f, PauseMax = .8f, SpecialScale = .9f, SpecialRestMin = .8f, SpecialRestMax = 1.2f,
                RetreatChance = .15f, ReactDodge = .15f, Aggression = .2f, ComboBreak = .12f
            },
            Difficulty.Dificil => new Tuning
            {
                ReactionMin = .05f, ReactionMax = .1f, ThinkMin = .08f, ThinkMax = .14f,
                AttackChance = .82f, DodgeChance = .12f, ComboChance = .92f,
                PauseMin = .1f, PauseMax = .22f, SpecialScale = 1.9f, SpecialRestMin = .35f, SpecialRestMax = .6f,
                RetreatChance = .04f, ReactDodge = .75f, Aggression = .9f, ComboBreak = .7f, WaitsOutAttacks = true
            },
            _ => new Tuning
            {
                ReactionMin = .14f, ReactionMax = .26f, ThinkMin = .16f, ThinkMax = .26f,
                AttackChance = .65f, DodgeChance = .12f, ComboChance = .6f,
                PauseMin = .25f, PauseMax = .45f, SpecialScale = 1.35f, SpecialRestMin = .55f, SpecialRestMax = .9f,
                RetreatChance = .1f, ReactDodge = .4f, Aggression = .5f, ComboBreak = .35f, WaitsOutAttacks = true
            }
        };
    }
}
