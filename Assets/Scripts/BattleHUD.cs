using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TSEFatal
{
    /// <summary>HUD da luta: retrato, dano em %, vidas, recargas, avisos grandes e tela de resultado.</summary>
    public class BattleHUD : MonoBehaviour
    {
        private class Card
        {
            public FighterController Fighter;
            public RectTransform Root;
            public TMP_Text Damage;
            public readonly List<Image> Stocks = new List<Image>();
            public readonly List<Image> Cooldowns = new List<Image>();
            public float Shake;
        }

        private readonly List<Card> cards = new List<Card>();
        private GameObject root;
        private TMP_Text big, bigShadow, hint;
        private GameObject resultPanel;
        private TMP_Text resultTitle, resultQuote, resultHelp;
        private Coroutine bigRoutine;

        public static BattleHUD Create(Transform parent, FighterController player, FighterController bot, bool demo)
        {
            var go = new GameObject("Battle HUD");
            go.transform.SetParent(parent, false);
            var hud = go.AddComponent<BattleHUD>();
            hud.Build(player, bot, demo);
            return hud;
        }

        private TMP_Text modeLabel;
        private bool demoMode;

        private void Build(FighterController player, FighterController bot, bool demo)
        {
            demoMode = demo;
            root = ArcadeUI.CreateRoot("Battle HUD Canvas");
            root.transform.SetParent(transform, false);
            root.GetComponent<Canvas>().sortingOrder = 15;

            cards.Add(BuildCard(player, false));
            cards.Add(BuildCard(bot, true));

            // Topo, entre os cartões: modo e dificuldade do bot.
            Difficulty level = BotDifficulty.Current;
            string mode = demo ? "DEMONSTRAÇÃO" : "BOT " + BotDifficulty.Label(level);
            modeLabel = ArcadeUI.PixelText(root.transform, "Modo", mode, demo ? 7 : 5, demo ? new Color(1f, .82f, .3f) : BotDifficulty.Tint(level), TextAlignmentOptions.Center);
            ArcadeUI.Anchor(modeLabel.rectTransform, new Vector2(.5f, 1f), new Vector2(.5f, 1f), new Vector2(0, -12), new Vector2(150, 12));
            modeLabel.gameObject.AddComponent<Outline>().effectColor = Color.black;
            if (demo)
            {
                TMP_Text sub = ArcadeUI.PixelText(root.transform, "Dificuldade demo", "BOTS NO " + BotDifficulty.Label(level), 4, new Color(.85f, .9f, 1f), TextAlignmentOptions.Center);
                ArcadeUI.Anchor(sub.rectTransform, new Vector2(.5f, 1f), new Vector2(.5f, 1f), new Vector2(0, -23), new Vector2(150, 8));
            }

            // Faixa de dicas de controle no rodapé.
            Image bar = ArcadeUI.Panel(root.transform, "Faixa de controles", new Color(.02f, .01f, .06f, .72f));
            ArcadeUI.Anchor(bar.rectTransform, new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 7), new Vector2(0, 14));
            string line = demo ? "MODO DEMONSTRAÇÃO: OS DOIS LADOS SÃO BOTS.     ESC: VOLTAR AO MENU" : ControlsLine(player.Candidate);
            hint = ArcadeUI.PixelText(bar.transform, "Controles", line, 5, new Color(.85f, .9f, 1f), TextAlignmentOptions.Center);
            ArcadeUI.Stretch(hint.rectTransform);
            hint.enableAutoSizing = true; hint.fontSizeMax = 5; hint.fontSizeMin = 3; // nomes longos cabem na faixa

            bigShadow = ArcadeUI.PixelText(root.transform, "Aviso sombra", "", 30, new Color(0, 0, 0, .8f), TextAlignmentOptions.Center);
            ArcadeUI.Anchor(bigShadow.rectTransform, new Vector2(.5f, .58f), new Vector2(.5f, .58f), new Vector2(2, -2), new Vector2(460, 60));
            big = ArcadeUI.PixelText(root.transform, "Aviso", "", 30, Color.white, TextAlignmentOptions.Center);
            ArcadeUI.Anchor(big.rectTransform, new Vector2(.5f, .58f), new Vector2(.5f, .58f), Vector2.zero, new Vector2(460, 60));

            Image panel = ArcadeUI.Panel(root.transform, "Resultado", new Color(.04f, .01f, .1f, .92f));
            ArcadeUI.Anchor(panel.rectTransform, new Vector2(.5f, .5f), new Vector2(.5f, .5f), Vector2.zero, new Vector2(300, 112));
            panel.gameObject.AddComponent<Outline>().effectColor = new Color(1f, .72f, .13f);
            resultTitle = ArcadeUI.PixelText(panel.transform, "Vencedor", "", 15, new Color(1f, .8f, .2f), TextAlignmentOptions.Center);
            ArcadeUI.Anchor(resultTitle.rectTransform, new Vector2(.5f, .7f), new Vector2(.5f, .7f), Vector2.zero, new Vector2(290, 30));
            resultQuote = ArcadeUI.PixelText(panel.transform, "Frase", "", 6, new Color(.85f, .9f, 1f), TextAlignmentOptions.Center);
            ArcadeUI.Anchor(resultQuote.rectTransform, new Vector2(.5f, .36f), new Vector2(.5f, .36f), Vector2.zero, new Vector2(290, 14));
            resultHelp = ArcadeUI.PixelText(panel.transform, "Ajuda", "ENTER: MENU     R: REVANCHE", 6, Color.white, TextAlignmentOptions.Center);
            ArcadeUI.Anchor(resultHelp.rectTransform, new Vector2(.5f, .14f), new Vector2(.5f, .14f), Vector2.zero, new Vector2(290, 16));
            resultPanel = panel.gameObject;
            resultPanel.SetActive(false);
        }

        private static string ControlsLine(CandidateId c)
        {
            string line = $"A/D MOVER  W PULAR  S DESCER  SHIFT ESQUIVA  J ATAQUE  K {CharacterProfile.Special1Name(c).ToUpperInvariant()}  L {CharacterProfile.Special2Name(c).ToUpperInvariant()}";
            string third = CharacterProfile.Special3Name(c);
            return third != null ? line + $"  I {third.ToUpperInvariant()}" : line;
        }

        private Card BuildCard(FighterController fighter, bool right)
        {
            var card = new Card { Fighter = fighter };
            CandidateId c = fighter.Candidate;
            Color tint = CharacterProfile.Tint(c);
            Vector2 anchor = right ? new Vector2(1, 1) : new Vector2(0, 1);
            float sign = right ? -1f : 1f;

            Image bg = ArcadeUI.Panel(root.transform, "Cartão " + CharacterProfile.DisplayName(c), new Color(.03f, .02f, .08f, .82f));
            ArcadeUI.Anchor(bg.rectTransform, anchor, anchor, new Vector2(sign * 84f, -26f), new Vector2(152, 40));
            bg.gameObject.AddComponent<Outline>().effectColor = tint;
            card.Root = bg.rectTransform;

            Image frame = ArcadeUI.Panel(bg.transform, "Moldura retrato", tint);
            ArcadeUI.Anchor(frame.rectTransform, new Vector2(right ? 1 : 0, .5f), new Vector2(right ? 1 : 0, .5f), new Vector2(sign * 21f, 0), new Vector2(34, 34));
            Texture2D tex = Resources.Load<Texture2D>(CharacterProfile.StaticSprite(c));
            RawImage portrait = ArcadeUI.Raw(frame.transform, "Retrato", tex, Color.white);
            ArcadeUI.Stretch(portrait.rectTransform);
            portrait.rectTransform.offsetMin = new Vector2(2, 2); portrait.rectTransform.offsetMax = new Vector2(-2, -2);
            portrait.uvRect = CharacterProfile.PortraitUV(c);
            if (right) portrait.rectTransform.localScale = new Vector3(-1, 1, 1); // olha para o centro

            TextAlignmentOptions align = right ? TextAlignmentOptions.Right : TextAlignmentOptions.Left;
            float textX = right ? -42f : 42f;
            string label = (fighter.IsBot ? "CPU  " : "P1  ") + CharacterProfile.DisplayName(c).ToUpperInvariant();
            TMP_Text name = ArcadeUI.PixelText(bg.transform, "Nome", label, 6,Color.Lerp(tint, Color.white, .55f), align);
            PlaceText(name.rectTransform, right, textX, 12f, new Vector2(104, 8));
            name.enableAutoSizing = true; name.fontSizeMax = 6; name.fontSizeMin = 4; // "CPU FLAVIO BOLSONARO" cabe no cartão

            card.Damage = ArcadeUI.PixelText(bg.transform, "Dano", "0%", 14, Color.white, align);
            PlaceText(card.Damage.rectTransform, right, textX, -1f, new Vector2(104, 18));
            card.Damage.gameObject.AddComponent<Outline>().effectColor = Color.black;

            for (int i = 0; i < FighterController.StartingStocks; i++)
            {
                Image stock = ArcadeUI.Panel(bg.transform, "Vida " + i, tint);
                Vector2 a = new Vector2(right ? 1 : 0, .5f);
                ArcadeUI.Anchor(stock.rectTransform, a, a, new Vector2(textX + sign * (3 + i * 9f), -14f), new Vector2(7, 7));
                stock.gameObject.AddComponent<Outline>().effectColor = Color.black;
                card.Stocks.Add(stock);
            }

            int slots = CharacterProfile.Special3Name(c) != null ? 3 : 2;
            for (int i = 0; i < slots; i++)
            {
                Image cd = ArcadeUI.Panel(bg.transform, "Recarga " + (i + 1), new Color(1f, .8f, .2f));
                Vector2 a = new Vector2(right ? 0 : 1, .5f);
                ArcadeUI.Anchor(cd.rectTransform, a, a, new Vector2(-sign * (8 + i * 9f), -14f), new Vector2(7, 7));
                card.Cooldowns.Add(cd);
            }

            fighter.OnHitTaken += f => card.Shake = .25f;
            return card;
        }

        private static void PlaceText(RectTransform rect, bool right, float x, float y, Vector2 size)
        {
            Vector2 a = new Vector2(right ? 1 : 0, .5f);
            rect.anchorMin = rect.anchorMax = a;
            rect.pivot = new Vector2(right ? 1 : 0, .5f);
            rect.anchoredPosition = new Vector2(x, y);
            rect.sizeDelta = size;
        }

        private void Update()
        {
            if (demoMode && modeLabel != null) modeLabel.alpha = .65f + .35f * Mathf.Sin(Time.unscaledTime * 4f);
            foreach (Card card in cards)
            {
                FighterController f = card.Fighter;
                if (f == null) continue;
                card.Damage.text = f.OutOfPlay ? "--" : Mathf.FloorToInt(f.DamagePercent) + "%";
                card.Damage.color = Combat.DamageColor(f.DamagePercent);
                for (int i = 0; i < card.Stocks.Count; i++) card.Stocks[i].enabled = i < f.Stocks;
                for (int i = 0; i < card.Cooldowns.Count; i++)
                {
                    bool ready = f.Cooldown(i + 1) <= 0f && f.SilenceRemaining <= 0f;
                    if (f.ConfusionRemaining > 0f) card.Damage.color = Color.Lerp(card.Damage.color, new Color(.6f, .85f, .25f), .5f);
                    card.Cooldowns[i].color = f.SilenceRemaining > 0f ? new Color(.15f, .15f, .15f) : ready ? new Color(1f, .8f, .2f) : new Color(.35f, .3f, .25f);
                }
                card.Shake = Mathf.Max(0f, card.Shake - Time.deltaTime);
                card.Damage.rectTransform.localScale = Vector3.one * (1f + card.Shake * 1.2f);
                card.Root.localRotation = Quaternion.Euler(0, 0, card.Shake > 0f ? Random.Range(-3f, 3f) : 0f);
            }
        }

        public void ShowBig(string text, Color color, float seconds)
        {
            if (bigRoutine != null) StopCoroutine(bigRoutine);
            bigRoutine = StartCoroutine(BigRoutine(text, color, seconds));
        }

        private IEnumerator BigRoutine(string text, Color color, float seconds)
        {
            big.text = bigShadow.text = text;
            big.color = color;
            float t = 0f;
            while (t < seconds)
            {
                t += Time.unscaledDeltaTime;
                float pop = t < .1f ? Mathf.Lerp(1.8f, 1f, t / .1f) : 1f;
                big.rectTransform.localScale = bigShadow.rectTransform.localScale = Vector3.one * pop;
                float alpha = Mathf.Clamp01((seconds - t) * 5f);
                big.color = new Color(color.r, color.g, color.b, alpha);
                bigShadow.color = new Color(0, 0, 0, .8f * alpha);
                yield return null;
            }
            big.text = bigShadow.text = "";
        }

        public void SetResultHelp(string text) { if (resultHelp.text != text) resultHelp.text = text; }

        public void ShowResult(FighterController winner)
        {
            if (demoMode) resultHelp.text = "PRÓXIMA LUTA EM 5...     ESC: SAIR";
            resultPanel.SetActive(true);
            resultPanel.transform.SetAsLastSibling();
            resultTitle.text = CharacterProfile.DisplayName(winner.Candidate).ToUpperInvariant() + "\nELEITO!";
            resultTitle.color = Color.Lerp(CharacterProfile.Tint(winner.Candidate), new Color(1f, .85f, .3f), .5f);
            resultQuote.text = "\"" + CharacterProfile.VictoryLine(winner.Candidate) + "\"";
        }

        private void OnDestroy()
        {
            if (root != null) Destroy(root);
        }
    }
}
