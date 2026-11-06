using UnityEngine;

namespace TSEFatal
{
    public enum CandidateId { Lula, RenanSantos, FlavioBolsonaro, AugustoCury }

    public struct FighterStats
    {
        public float Speed;   // unidades/s no chão
        public float Weight;  // >1 resiste mais ao knockback
        public int Attack, Defense, Agility; // 0-100, só para a tela de seleção
    }

    public static class CharacterProfile
    {
        /// <summary>Quantos candidatos existem (sorteio de rival e da demo).</summary>
        public static readonly int Count = System.Enum.GetValues(typeof(CandidateId)).Length;

        public static string DisplayName(CandidateId candidate) => candidate switch
        {
            CandidateId.Lula => "Lula",
            CandidateId.RenanSantos => "Renan Santos",
            CandidateId.FlavioBolsonaro => "Flavio Bolsonaro",
            CandidateId.AugustoCury => "Augusto Cury",
            _ => "Candidato"
        };

        public static Color Tint(CandidateId candidate) => candidate switch
        {
            CandidateId.Lula => new Color(0.82f, 0.08f, 0.12f),
            CandidateId.RenanSantos => new Color(0.45f, 0.22f, 0.75f),
            CandidateId.FlavioBolsonaro => new Color(0.10f, 0.62f, 0.30f),
            CandidateId.AugustoCury => new Color(0.18f, 0.48f, 0.90f),
            _ => Color.gray
        };

        /// <summary>Pasta em Resources/Art/Characters onde ficam as folhas de animação.</summary>
        public static string AssetFolder(CandidateId candidate) => candidate switch
        {
            CandidateId.Lula => "lula",
            CandidateId.RenanSantos => "renan",
            CandidateId.FlavioBolsonaro => "flavio",
            CandidateId.AugustoCury => "cury",
            _ => "lula"
        };

        /// <summary>Imagem estática usada enquanto não houver folhas de animação.</summary>
        public static string StaticSprite(CandidateId candidate) => candidate switch
        {
            CandidateId.Lula => "Art/Characters/lula_idle",
            CandidateId.RenanSantos => "Art/Characters/renan_idle",
            CandidateId.FlavioBolsonaro => "Art/Characters/flavio_sigma_idle",
            CandidateId.AugustoCury => "Art/Characters/cury_idle",
            _ => "Art/Characters/lula_idle"
        };

        /// <summary>Composição do candidato atrás do púlpito na tela de seleção.</summary>
        public static string SelectionSprite(CandidateId candidate) => candidate switch
        {
            CandidateId.Lula => "Art/Menu/select_lula_podium",
            CandidateId.RenanSantos => "Art/Menu/select_renan_podium",
            CandidateId.FlavioBolsonaro => "Art/Menu/select_flavio_podium",
            CandidateId.AugustoCury => "Art/Menu/select_cury_podium",
            _ => "Art/Menu/select_lula_podium"
        };

        /// <summary>Recorte do rosto (UV) na imagem estática, para o retrato do HUD.</summary>
        public static Rect PortraitUV(CandidateId candidate) => candidate switch
        {
            CandidateId.Lula => new Rect(.30f, .62f, .36f, .36f),
            CandidateId.RenanSantos => new Rect(.32f, .64f, .34f, .34f),
            CandidateId.FlavioBolsonaro => new Rect(.26f, .70f, .32f, .28f),
            CandidateId.AugustoCury => new Rect(.36f, .72f, .26f, .27f),
            _ => new Rect(0, 0, 1, 1)
        };

        public static string Special1Name(CandidateId candidate) => candidate switch
        {
            CandidateId.Lula => "Mensalão",
            CandidateId.RenanSantos => "Corte Viral",
            CandidateId.FlavioBolsonaro => "Rachadinha",
            CandidateId.AugustoCury => "Drone com Sirene",
            _ => "Especial"
        };

        public static string Special2Name(CandidateId candidate) => candidate switch
        {
            CandidateId.Lula => "Faz o L",
            CandidateId.RenanSantos => "Onça Liberal",
            CandidateId.FlavioBolsonaro => "Mansão de 6 Milhões",
            CandidateId.AugustoCury => "Ministério da IA",
            _ => "Especial 2"
        };

        /// <summary>Terceira habilidade (tecla I).</summary>
        public static string Special3Name(CandidateId candidate) => candidate switch
        {
            CandidateId.Lula => "Censura",
            CandidateId.RenanSantos => "Fralda do Debate",
            CandidateId.FlavioBolsonaro => "Chocolate Lavado",
            CandidateId.AugustoCury => "Palestra Motivacional",
            _ => null
        };

        /// <summary>
        /// Nome do arquivo de voz gritado ao usar a habilidade (1 = K, 2 = L, 3 = I), em Resources/Audio/Vozes.
        /// Roteiro de gravação em Docs/DUBLAGEM.md.
        /// </summary>
        public static string SpecialVoice(CandidateId candidate, int slot)
        {
            string[] names = candidate switch
            {
                CandidateId.Lula => new[] { "mensalao", "faz_o_l", "censura" },
                CandidateId.RenanSantos => new[] { "corte_viral", "onca", "fralda" },
                CandidateId.FlavioBolsonaro => new[] { "rachadinha", "mansao", "chocolate" },
                CandidateId.AugustoCury => new[] { "drone", "ministerio_ia", "palestra" },
                _ => new[] { "k", "l", "i" }
            };
            return AssetFolder(candidate) + "_" + names[Mathf.Clamp(slot, 1, 3) - 1];
        }

        public static string Special3Description(CandidateId candidate) => candidate switch
        {
            CandidateId.Lula => "Tarja preta que deixa o rival 3,5 s sem usar habilidades.",
            CandidateId.RenanSantos => "Fralda cheia em arco: o rival fica tonto e com os controles invertidos.",
            CandidateId.FlavioBolsonaro => "Três bombons que quicam pelo chão. Cada acerto lava 2% do dano de Flávio.",
            CandidateId.AugustoCury => "Palestra tão longa que o rival dorme em pé. Um golpe acorda.",
            _ => string.Empty
        };

        // ---------------------------------------------------------------- falas (sátira)

        /// <summary>Grito quando o personagem perde uma vida.</summary>
        public static string[] KnockedOutLines(CandidateId candidate) => candidate switch
        {
            CandidateId.Lula => new[] { "EU NÃO SABIA DE NADA!", "É PERSEGUIÇÃO!", "CADÊ MEU PEDALINHO?", "O TRIPLEX NÃO É MEU!" },
            CandidateId.RenanSantos => new[] { "ISSO VAI VIRAR CORTE!", "É O SISTEMA!", "VOU PROCESSAR!", "FOI MONTAGEM!" },
            CandidateId.FlavioBolsonaro => new[] { "CADÊ O QUEIROZ?", "FOI O ASSESSOR!", "NÃO TENHO NADA A VER!", "PAGUEI EM ESPÉCIE!" },
            CandidateId.AugustoCury => new[] { "FALTOU GESTÃO DA EMOÇÃO!", "CADÊ MEU DRONE?", "ISSO VAI VIRAR LIVRO!", "RESPIRA... RESPIRA..." },
            _ => new[] { "CASSADO!" }
        };

        /// <summary>Fala ao voltar para a arena.</summary>
        public static string[] RespawnLines(CandidateId candidate) => candidate switch
        {
            CandidateId.Lula => new[] { "VOLTEI, COMPANHEIRO!", "TÔ ELEGÍVEL!", "QUERIDO, VOLTEI!" },
            CandidateId.RenanSantos => new[] { "MISSÃO CONTINUA!", "VOLTEI PRO DEBATE!", "BORA VIRALIZAR!" },
            CandidateId.FlavioBolsonaro => new[] { "COMPREI OUTRA MANSÃO!", "HABEAS CORPUS!", "VOLTEI COM O CHOCOLATE!" },
            CandidateId.AugustoCury => new[] { "VOLTEI MAIS SERENO!", "O DRONE CHEGOU ANTES!", "CAPÍTULO 2!" },
            _ => new[] { "VOLTEI!" }
        };

        /// <summary>Frase na tela de vitória.</summary>
        public static string VictoryLine(CandidateId candidate) => candidate switch
        {
            CandidateId.Lula => "A PICANHA É POR MINHA CONTA!",
            CandidateId.RenanSantos => "JÁ VIROU CORTE NO YOUTUBE!",
            CandidateId.FlavioBolsonaro => "PODE MANDAR O PIX PRO QUEIROZ!",
            CandidateId.AugustoCury => "NÃO TEM DRONEFOBIA QUE ME PARE!",
            _ => string.Empty
        };

        /// <summary>Faixa grande quando alguém perde uma vida.</summary>
        public static readonly string[] KnockoutBanners = { "CASSADO!", "INELEGÍVEL!", "IMPEACHMENT!", "DELATADO!", "CPI NELE!", "VAI PRA PAPUDA!" };

        public static string Pick(string[] options) => options[Random.Range(0, options.Length)];

        public static string Special1Description(CandidateId candidate) => candidate switch
        {
            CandidateId.Lula => "Arremessa três maços de dinheiro em leque.",
            CandidateId.RenanSantos => "Avança cortando tudo pela frente, pronto para viralizar.",
            CandidateId.FlavioBolsonaro => "Investida que fica com parte do dano: cura Flávio.",
            CandidateId.AugustoCury => "Drone de sirene que persegue o rival pelo ar.",
            _ => string.Empty
        };

        public static string Special2Description(CandidateId candidate) => candidate switch
        {
            CandidateId.Lula => "Um L gigante explode na frente dele.",
            CandidateId.RenanSantos => "Solta uma onça que corre pelo chão.",
            CandidateId.FlavioBolsonaro => "Uma mansão cai do céu em cima do rival.",
            CandidateId.AugustoCury => "Três robôs de IA marcham pelo chão e empurram o rival.",
            _ => string.Empty
        };

        public static string Flavor(CandidateId candidate) => candidate switch
        {
            CandidateId.Lula => "Peso-pesado. Distribui dinheiro, censura o rival e fecha o debate com um L.",
            CandidateId.RenanSantos => "O mais rápido. Corta, viraliza, solta a onça e leva fralda pro debate.",
            CandidateId.FlavioBolsonaro => "Equilibrado. Racha o dano, lava chocolate e derruba mansões.",
            CandidateId.AugustoCury => "Controle. Manda drone, terceiriza a briga para a IA e faz o rival dormir.",
            _ => string.Empty
        };

        public static FighterStats Stats(CandidateId candidate) => candidate switch
        {
            CandidateId.Lula => new FighterStats { Speed = 6.6f, Weight = 1.15f, Attack = 90, Defense = 92, Agility = 62 },
            CandidateId.RenanSantos => new FighterStats { Speed = 8.4f, Weight = .9f, Attack = 74, Defense = 66, Agility = 96 },
            CandidateId.FlavioBolsonaro => new FighterStats { Speed = 7.3f, Weight = 1f, Attack = 84, Defense = 78, Agility = 78 },
            CandidateId.AugustoCury => new FighterStats { Speed = 7f, Weight = 1.05f, Attack = 72, Defense = 84, Agility = 70 },
            _ => new FighterStats { Speed = 7f, Weight = 1f, Attack = 80, Defense = 80, Agility = 80 }
        };
    }
}
