using UnityEngine;

namespace TSEFatal
{
    public enum CandidateId { Lula, RenanSantos, FlavioBolsonaro }

    public static class CharacterProfile
    {
        public static string DisplayName(CandidateId candidate) => candidate switch
        {
            CandidateId.Lula => "Lula",
            CandidateId.RenanSantos => "Renan Santos",
            CandidateId.FlavioBolsonaro => "Flavio Bolsonaro",
            _ => "Candidato"
        };

        public static Color Tint(CandidateId candidate) => candidate switch
        {
            CandidateId.Lula => new Color(0.82f, 0.08f, 0.12f),
            CandidateId.RenanSantos => new Color(0.23f, 0.08f, 0.33f),
            CandidateId.FlavioBolsonaro => new Color(0.04f, 0.38f, 0.18f),
            _ => Color.gray
        };

        public static string SpecialName(CandidateId candidate) => candidate switch
        {
            CandidateId.Lula => "Aura Reversa",
            CandidateId.RenanSantos => "Chamado da Onça",
            CandidateId.FlavioBolsonaro => "Maleta Meteoro",
            _ => "Especial"
        };

        public static string Flavor(CandidateId candidate) => candidate switch
        {
            CandidateId.Lula => "Hipnose cômica, aura drenada e buff temporário.",
            CandidateId.RenanSantos => "Invoca uma onça pixelada para um ataque rápido.",
            CandidateId.FlavioBolsonaro => "Arremessa maletas e chama uma chuva de dinheiro fictícia.",
            _ => string.Empty
        };
    }
}
