using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TSEFatal
{
    /// <summary>
    /// Seleção de candidato. Esquerda: o personagem em destaque. Direita: ficha com estilo,
    /// as três habilidades e os atributos. Embaixo: miniaturas para trocar.
    /// </summary>
    public class CharacterSelectController : MonoBehaviour
    {
        private readonly CandidateId[] candidates = { CandidateId.Lula, CandidateId.RenanSantos, CandidateId.FlavioBolsonaro, CandidateId.AugustoCury };
        private readonly List<Image> thumbFrames = new List<Image>();
        private readonly List<RectTransform> statFills = new List<RectTransform>();
        private readonly TMP_Text[] skillNames = new TMP_Text[3];
        private readonly TMP_Text[] skillDescriptions = new TMP_Text[3];
        private GameFlow flow;
        private GameObject root;
        private RectTransform stage;
        private Image heroPanel, heroShadow;
        private RawImage hero;
        private TMP_Text nameText, flavorText, botChip, podiumName;
        private Image cardOutlineHolder;
        private int selected;
        private bool visible;
        private float clock, slide;
        private MenuAudio audioMenu;
        private Coroutine vsRoutine;
        private GameObject vsRoot;

        private static readonly Color Gold = new Color(1f, .72f, .13f);
        private static readonly Color Ink = new Color(.10f, .035f, .20f);
        private static readonly Color Muted = new Color(.72f, .76f, .92f);
        private static readonly Color CardColor = new Color(.035f, .012f, .085f, .92f);

        public void Initialize(GameFlow owner)
        {
            flow = owner;
            audioMenu = MenuAudio.Ensure(gameObject);
        }

        public void Show()
        {
            if (root == null) Build();
            root.SetActive(true);
            selected = 0;
            visible = true;
            GameFlow.LockInputThisFrame();
            Refresh();
        }

        public void Hide()
        {
            if (root != null) root.SetActive(false);
            visible = false;
            CancelVs();
        }

        private void OnDisable() => CancelVs();

        /// <summary>Interrompe uma transição VS em andamento (para nunca começar uma luta "fantasma").</summary>
        private void CancelVs()
        {
            if (vsRoutine != null) StopCoroutine(vsRoutine);
            vsRoutine = null;
            if (vsRoot != null) Destroy(vsRoot);
            vsRoot = null;
        }

        // ---------------------------------------------------------------- montagem

        private void Build()
        {
            root = ArcadeUI.CreateRoot("Character Select Canvas");
            root.GetComponent<Canvas>().sortingOrder = 21;
            ArcadeUI.Cover(root.transform, "Fundo", Resources.Load<Texture2D>("Art/Menu/menu_debate_arena"));
            Image veil = ArcadeUI.Panel(root.transform, "Véu escuro", new Color(.02f, .01f, .06f, .82f));
            ArcadeUI.Stretch(veil.rectTransform);
            stage = ArcadeUI.Stage(root.transform);

            // Cabeçalho
            TMP_Text header = ArcadeUI.PixelText(stage, "Título", "ESCOLHA SEU CANDIDATO", 10, Gold, TextAlignmentOptions.Center);
            ArcadeUI.Place(header.rectTransform, new Vector2(240, 20), new Vector2(300, 14));
            header.gameObject.AddComponent<Outline>().effectColor = new Color(.45f, .03f, .08f);
            botChip = ArcadeUI.PixelText(stage, "Dificuldade", "", 5, Color.white, TextAlignmentOptions.Right);
            ArcadeUI.Place(botChip.rectTransform, new Vector2(418, 20), new Vector2(100, 8));

            // Esquerda: personagem em destaque
            heroPanel = ArcadeUI.Panel(stage, "Destaque", Color.white);
            ArcadeUI.Place(heroPanel.rectTransform, new Vector2(112, 138), new Vector2(184, 196));
            heroShadow = ArcadeUI.Panel(stage, "Sombra", new Color(0, 0, 0, .6f));
            heroShadow.sprite = ArcadeUI.SoftEllipse;
            ArcadeUI.Place(heroShadow.rectTransform, new Vector2(112, 226), new Vector2(120, 14));
            hero = ArcadeUI.Raw(stage, "Personagem", null, Color.white);
            hero.raycastTarget = false;
            podiumName = ArcadeUI.PixelText(hero.transform, "Nome no púlpito", "", 4, Color.white, TextAlignmentOptions.Center);
            RectTransform podiumNameRect = podiumName.rectTransform;
            podiumNameRect.anchorMin = podiumNameRect.anchorMax = new Vector2(.5f, .415f);
            podiumNameRect.anchoredPosition = Vector2.zero;
            podiumNameRect.sizeDelta = new Vector2(76, 8);
            podiumName.enableAutoSizing = true;
            podiumName.fontSizeMin = 3;
            podiumName.fontSizeMax = 4;
            podiumName.gameObject.AddComponent<Outline>().effectColor = new Color(0, 0, 0, .9f);

            // Direita: ficha
            Image card = ArcadeUI.Panel(stage, "Ficha", CardColor);
            ArcadeUI.Place(card.rectTransform, new Vector2(340, 122), new Vector2(232, 164));
            cardOutlineHolder = card;
            card.gameObject.AddComponent<Outline>();

            nameText = ArcadeUI.PixelText(stage, "Nome", "", 10, Color.white, TextAlignmentOptions.Left);
            ArcadeUI.Place(nameText.rectTransform, new Vector2(340, 52), new Vector2(212, 14));
            flavorText = ArcadeUI.PixelText(stage, "Estilo", "", 5, Muted, TextAlignmentOptions.TopLeft);
            flavorText.textWrappingMode = TextWrappingModes.Normal;
            flavorText.lineSpacing = 18;
            ArcadeUI.Place(flavorText.rectTransform, new Vector2(340, 72), new Vector2(212, 16));

            Image rule = ArcadeUI.Panel(stage, "Divisor", new Color(1f, 1f, 1f, .12f));
            ArcadeUI.Place(rule.rectTransform, new Vector2(340, 86), new Vector2(212, 1));

            string[] keys = { "K", "L", "I" };
            for (int i = 0; i < 3; i++)
            {
                float y = 99 + i * 23;
                Image badge = ArcadeUI.Panel(stage, "Tecla " + keys[i], Gold);
                ArcadeUI.Place(badge.rectTransform, new Vector2(240, y + 3), new Vector2(12, 12));
                TMP_Text letter = ArcadeUI.PixelText(badge.transform, "Letra", keys[i], 6, Ink, TextAlignmentOptions.Center);
                ArcadeUI.Stretch(letter.rectTransform);
                letter.rectTransform.offsetMin = new Vector2(1, 0); // a fonte pixel fica 1px à esquerda

                skillNames[i] = ArcadeUI.PixelText(stage, "Habilidade", "", 6, Gold, TextAlignmentOptions.Left);
                ArcadeUI.Place(skillNames[i].rectTransform, new Vector2(350, y), new Vector2(196, 8));
                skillDescriptions[i] = ArcadeUI.PixelText(stage, "Descrição", "", 4, Muted, TextAlignmentOptions.TopLeft);
                skillDescriptions[i].textWrappingMode = TextWrappingModes.Normal;
                skillDescriptions[i].lineSpacing = 22;
                ArcadeUI.Place(skillDescriptions[i].rectTransform, new Vector2(350, y + 11), new Vector2(196, 12));
            }

            string[] stats = { "ATAQUE", "DEFESA", "AGILIDADE" };
            for (int i = 0; i < 3; i++)
            {
                float y = 172 + i * 10;
                TMP_Text label = ArcadeUI.PixelText(stage, stats[i], stats[i], 4, Muted, TextAlignmentOptions.Left);
                ArcadeUI.Place(label.rectTransform, new Vector2(270, y), new Vector2(72, 6));
                Image track = ArcadeUI.Panel(stage, stats[i] + " trilho", new Color(1f, 1f, 1f, .08f));
                ArcadeUI.Place(track.rectTransform, new Vector2(377, y), new Vector2(138, 5));
                Image fill = ArcadeUI.Panel(track.transform, "Preenchimento", Gold);
                fill.rectTransform.anchorMin = Vector2.zero; fill.rectTransform.anchorMax = Vector2.one;
                fill.rectTransform.offsetMin = fill.rectTransform.offsetMax = Vector2.zero;
                statFills.Add(fill.rectTransform);
            }

            // Miniaturas, centralizadas sob a ficha (x = 340) para caber qualquer quantidade.
            float firstThumb = 340f - (candidates.Length - 1) * 22f;
            for (int i = 0; i < candidates.Length; i++)
            {
                CandidateId c = candidates[i];
                Image frame = ArcadeUI.Panel(stage, "Miniatura " + CharacterProfile.DisplayName(c), Color.white);
                ArcadeUI.Place(frame.rectTransform, new Vector2(firstThumb + i * 44, 228), new Vector2(36, 36));
                RawImage face = ArcadeUI.Raw(frame.transform, "Rosto", Resources.Load<Texture2D>(CharacterProfile.StaticSprite(c)), Color.white);
                ArcadeUI.Stretch(face.rectTransform);
                face.rectTransform.offsetMin = new Vector2(2, 2); face.rectTransform.offsetMax = new Vector2(-2, -2);
                face.uvRect = CharacterProfile.PortraitUV(c);
                thumbFrames.Add(frame);
            }
            TMP_Text left = ArcadeUI.PixelText(stage, "Seta esquerda", "<", 8, Gold, TextAlignmentOptions.Center);
            ArcadeUI.Place(left.rectTransform, new Vector2(firstThumb - 34f, 229), new Vector2(12, 12));
            TMP_Text right = ArcadeUI.PixelText(stage, "Seta direita", ">", 8, Gold, TextAlignmentOptions.Center);
            ArcadeUI.Place(right.rectTransform, new Vector2(firstThumb + (candidates.Length - 1) * 44f + 34f, 229), new Vector2(12, 12));

            TMP_Text help = ArcadeUI.PixelText(stage, "Ajuda", "A/D ESCOLHER     ENTER CONFIRMAR     ESC VOLTAR", 5, new Color(1f, 1f, 1f, .55f), TextAlignmentOptions.Center);
            ArcadeUI.Place(help.rectTransform, new Vector2(240, 258), new Vector2(460, 8));
        }

        // ---------------------------------------------------------------- entrada

        private void Update()
        {
            if (!visible) return;
            clock += Time.unscaledDeltaTime;
            slide = Mathf.MoveTowards(slide, 0f, Time.unscaledDeltaTime * 6f);
            Vector2 feet = new Vector2(112 - slide * 30f, 230);
            ArcadeUI.PlaceFeet(hero.rectTransform, feet);
            hero.color = new Color(1, 1, 1, 1f - slide);
            hero.rectTransform.localScale = new Vector3(1f, 1f + .012f * Mathf.Sin(clock * 2.4f), 1f);
            thumbFrames[selected].rectTransform.localScale = Vector3.one * (1.1f + .03f * Mathf.Sin(clock * 6f));
            if (GameFlow.InputLocked || vsRoutine != null) return;

            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) Change(1);
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) Change(-1);
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0)) vsRoutine = StartCoroutine(ShowVsThenFight());
            if (Input.GetKeyDown(KeyCode.Escape)) { Hide(); flow.ReturnToMainMenu(); }
        }

        private void Change(int delta)
        {
            selected = (selected + delta + candidates.Length) % candidates.Length;
            audioMenu.Blip();
            slide = 1f;
            Refresh();
        }

        private void Refresh()
        {
            CandidateId c = candidates[selected];
            Color tint = CharacterProfile.Tint(c);
            Color light = Color.Lerp(tint, Color.white, .55f);

            Texture2D tex = Resources.Load<Texture2D>(CharacterProfile.SelectionSprite(c));
            if (tex == null) tex = Resources.Load<Texture2D>(CharacterProfile.StaticSprite(c));
            hero.texture = tex;
            float aspect = tex != null ? tex.width / (float)tex.height : .6f;
            hero.rectTransform.sizeDelta = new Vector2(176 * aspect, 176);
            podiumName.text = CharacterProfile.DisplayName(c).ToUpperInvariant();
            heroPanel.color = new Color(tint.r, tint.g, tint.b, .16f);
            cardOutlineHolder.GetComponent<Outline>().effectColor = new Color(tint.r, tint.g, tint.b, .9f);

            nameText.text = CharacterProfile.DisplayName(c).ToUpperInvariant();
            nameText.color = light;
            flavorText.text = CharacterProfile.Flavor(c);

            string[] names = { CharacterProfile.Special1Name(c), CharacterProfile.Special2Name(c), CharacterProfile.Special3Name(c) };
            string[] descriptions = { CharacterProfile.Special1Description(c), CharacterProfile.Special2Description(c), CharacterProfile.Special3Description(c) };
            for (int i = 0; i < 3; i++)
            {
                skillNames[i].text = (names[i] ?? "").ToUpperInvariant();
                skillDescriptions[i].text = descriptions[i];
            }

            FighterStats stats = CharacterProfile.Stats(c);
            int[] values = { stats.Attack, stats.Defense, stats.Agility };
            for (int i = 0; i < statFills.Count; i++)
            {
                statFills[i].anchorMax = new Vector2(values[i] / 100f, 1f);
                statFills[i].GetComponent<Image>().color = light;
            }

            for (int i = 0; i < thumbFrames.Count; i++)
            {
                bool chosen = i == selected;
                thumbFrames[i].color = chosen ? Gold : new Color(.25f, .22f, .38f);
                thumbFrames[i].rectTransform.localScale = Vector3.one;
                thumbFrames[i].GetComponentInChildren<RawImage>().color = chosen ? Color.white : new Color(.55f, .55f, .6f);
            }

            Difficulty level = BotDifficulty.Current;
            botChip.text = "BOT: " + BotDifficulty.Label(level);
            botChip.color = BotDifficulty.Tint(level);
        }

        // ---------------------------------------------------------------- VS

        private IEnumerator ShowVsThenFight()
        {
            visible = false;
            audioMenu.Confirm();
            CandidateId me = candidates[selected];
            CandidateId rival = GameFlow.RandomRival(me);

            vsRoot = ArcadeUI.CreateRoot("VS Transition");
            vsRoot.GetComponent<Canvas>().sortingOrder = 30;
            Image bg = ArcadeUI.Panel(vsRoot.transform, "VS Fundo", new Color(.03f, .005f, .05f, 1f));
            ArcadeUI.Stretch(bg.rectTransform);
            RectTransform vsStage = ArcadeUI.Stage(vsRoot.transform);

            RectTransform leftSide = VsSide(vsStage, me, 120, false);
            RectTransform rightSide = VsSide(vsStage, rival, 360, true);

            TMP_Text vs = ArcadeUI.PixelText(vsStage, "VS", "VS", 34, new Color(1f, .3f, .12f), TextAlignmentOptions.Center);
            ArcadeUI.Place(vs.rectTransform, new Vector2(240, 128), new Vector2(120, 50));
            var outline = vs.gameObject.AddComponent<Outline>();
            outline.effectColor = Gold; outline.effectDistance = new Vector2(2, -2);
            TMP_Text botLine = ArcadeUI.PixelText(vsStage, "Dificuldade", "BOT " + BotDifficulty.Label(BotDifficulty.Current), 5, BotDifficulty.Tint(BotDifficulty.Current), TextAlignmentOptions.Center);
            ArcadeUI.Place(botLine.rectTransform, new Vector2(240, 158), new Vector2(120, 8));

            float t = 0f;
            bool hitPlayed = false;
            while (t < 1.7f)
            {
                t += Time.unscaledDeltaTime;
                float enter = 1f - Mathf.Pow(1f - Mathf.Clamp01(t / .3f), 3f);
                leftSide.anchoredPosition = new Vector2(Mathf.Lerp(-240f, 0f, enter), 0f);
                rightSide.anchoredPosition = new Vector2(Mathf.Lerp(240f, 0f, enter), 0f);
                float pop = Mathf.Clamp01((t - .3f) / .12f);
                vs.rectTransform.localScale = Vector3.one * (pop <= 0f ? 0f : Mathf.Lerp(2.2f, 1f, pop));
                if (pop > 0f && !hitPlayed) { audioMenu.Hit(true); hitPlayed = true; }
                yield return null;
            }
            Destroy(vsRoot);
            vsRoot = null;
            vsRoutine = null;
            flow.BeginFight(me, rival);
        }

        private static RectTransform VsSide(RectTransform parent, CandidateId c, float x, bool mirrored)
        {
            var holder = new GameObject("Lado " + CharacterProfile.DisplayName(c), typeof(RectTransform));
            holder.transform.SetParent(parent, false);
            var rect = (RectTransform)holder.transform;
            ArcadeUI.Stretch(rect);

            Color tint = CharacterProfile.Tint(c);
            Image band = ArcadeUI.Panel(rect, "Faixa", new Color(tint.r, tint.g, tint.b, .28f));
            ArcadeUI.Place(band.rectTransform, new Vector2(x, 135), new Vector2(200, 270));
            Texture2D tex = Resources.Load<Texture2D>(CharacterProfile.StaticSprite(c));
            RawImage figure = ArcadeUI.Figure(rect, "Personagem", tex, 190);
            ArcadeUI.PlaceFeet(figure.rectTransform, new Vector2(x, 236));
            if (mirrored) figure.rectTransform.localScale = new Vector3(-1, 1, 1); // os dois se encaram
            Image plate = ArcadeUI.Panel(rect, "Placa", new Color(0, 0, 0, .75f));
            ArcadeUI.Place(plate.rectTransform, new Vector2(x, 248), new Vector2(200, 20));
            TMP_Text name = ArcadeUI.PixelText(rect, "Nome", CharacterProfile.DisplayName(c).ToUpperInvariant(), 8, Color.Lerp(tint, Color.white, .55f), TextAlignmentOptions.Center);
            ArcadeUI.Place(name.rectTransform, new Vector2(x, 248), new Vector2(196, 12));
            return rect;
        }
    }
}
