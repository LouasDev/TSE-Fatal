using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TSEFatal
{
    /// <summary>
    /// Menu principal. Layout em duas áreas que não se sobrepõem: coluna de opções à esquerda
    /// e os três candidatos agrupados à direita, sobre a arena escurecida.
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        private enum Option { Play, Demo, Difficulty, Controls, Quit }
        private static readonly string[] Labels = { "JOGAR", "DEMONSTRAÇÃO", "DIFICULDADE", "CONTROLES", "SAIR" };

        private const float SideWidth = 190f;
        private const float RowTop = 92f, RowGap = 21f, RowHeight = 17f;

        private readonly List<Image> rowBars = new List<Image>();
        private readonly List<TMP_Text> rowLabels = new List<TMP_Text>();
        private readonly List<RectTransform> figures = new List<RectTransform>();
        private GameFlow flow;
        private GameObject root;
        private RectTransform stage;
        private RectTransform logo;
        private TMP_Text logoText, difficultyValue, description;
        private RectTransform backdropHolder;
        private GameObject controlsPanel;
        private MenuAudio sound;
        private int selected;
        private float clock;
        private bool visible;
        private float axisLatchY, axisLatchX;

        private static readonly Color Ink = new Color(.10f, .035f, .20f);
        private static readonly Color Gold = new Color(1f, .72f, .13f);
        private static readonly Color Sidebar = new Color(.035f, .012f, .085f, .93f);
        private static readonly Color Muted = new Color(.72f, .76f, .92f);

        public void Initialize(GameFlow owner) { flow = owner; sound = MenuAudio.Ensure(gameObject); }

        public void Show()
        {
            if (root == null) Build();
            root.SetActive(true);
            controlsPanel.SetActive(false);
            visible = true;
            selected = 0;
            clock = 0;
            GameFlow.LockInputThisFrame();
            // Começa com a posição atual do analógico, para um controle encostado não mover a seleção sozinho.
            axisLatchY = Input.GetAxisRaw("Vertical");
            axisLatchX = Input.GetAxisRaw("Horizontal");
            Refresh();
        }

        public void Hide() { visible = false; if (root != null) root.SetActive(false); }

        // ---------------------------------------------------------------- montagem

        private void Build()
        {
            root = ArcadeUI.CreateRoot("Main Menu 480x270");
            root.GetComponent<Canvas>().sortingOrder = 20;

            // Fundo: palco do debate com os três candidatos já desenhados à direita e o lado
            // esquerdo escuro, reservado para a coluna do menu. Sem a arte nova, volta para a arena antiga.
            Texture2D art = Resources.Load<Texture2D>("Art/Menu/menu_debate_arena_personagens");
            bool artHasFighters = art != null;
            if (!artHasFighters) art = Resources.Load<Texture2D>("Art/Menu/menu_debate_arena");
            RawImage backdrop = ArcadeUI.Cover(root.transform, "Fundo tela cheia", art);
            backdropHolder = (RectTransform)backdrop.rectTransform.parent;
            Image veil = ArcadeUI.Panel(root.transform, "Véu escuro", new Color(.02f, .01f, .06f, artHasFighters ? .12f : .5f));
            ArcadeUI.Stretch(veil.rectTransform);

            stage = ArcadeUI.Stage(root.transform);

            if (!artHasFighters) BuildFighters(); // a arte nova já tem os personagens no palco
            BuildSidebar(artHasFighters ? .7f : Sidebar.a);
            BuildControlsPanel();
        }

        private void BuildFighters()
        {
            // Grupo à direita: Renan e Flávio atrás, Lula na frente. Uma sombra só no chão.
            float floorY = 244f;
            float center = SideWidth + (480f - SideWidth) * .5f;
            Image shadow = ArcadeUI.Panel(stage, "Sombra do grupo", new Color(0, 0, 0, .55f));
            shadow.sprite = ArcadeUI.SoftEllipse;
            ArcadeUI.Place(shadow.rectTransform, new Vector2(center, floorY), new Vector2(250, 22));

            var back = new (CandidateId id, float x, float h, float shade)[]
            {
                (CandidateId.RenanSantos, center - 62f, 150f, .78f),
                (CandidateId.FlavioBolsonaro, center + 64f, 156f, .78f),
                (CandidateId.Lula, center, 170f, 1f),
            };
            foreach (var f in back)
            {
                Texture2D tex = Resources.Load<Texture2D>(CharacterProfile.StaticSprite(f.id));
                RawImage img = ArcadeUI.Figure(stage, "Lutador " + CharacterProfile.DisplayName(f.id), tex, f.h);
                img.color = new Color(f.shade, f.shade, f.shade);
                ArcadeUI.PlaceFeet(img.rectTransform, new Vector2(f.x, floorY + 4f));
                figures.Add(img.rectTransform);
            }
        }

        private void BuildSidebar(float opacity)
        {
            // Mais transparente com a arte nova: o palco aparece atrás sem atrapalhar a leitura.
            Image side = ArcadeUI.Panel(stage, "Coluna do menu", new Color(Sidebar.r, Sidebar.g, Sidebar.b, opacity));
            // Mais alta e mais larga que o palco: em telas que não são 16:9 a coluna ainda encosta nas bordas.
            const float bleed = 300f;
            ArcadeUI.Place(side.rectTransform, new Vector2((SideWidth - bleed) * .5f, 135), new Vector2(SideWidth + bleed, 270 + 2 * bleed));
            Image edge = ArcadeUI.Panel(stage, "Borda dourada", Gold);
            ArcadeUI.Place(edge.rectTransform, new Vector2(SideWidth, 135), new Vector2(1.5f, 270 + 2 * bleed));

            // Logo
            logoText = ArcadeUI.PixelText(stage, "Logo", "TSE FATAL", 18, Gold, TextAlignmentOptions.Center);
            ArcadeUI.Place(logoText.rectTransform, new Vector2(SideWidth * .5f, 36), new Vector2(SideWidth - 8, 24));
            logoText.enableVertexGradient = true;
            logoText.colorGradient = new VertexGradient(new Color(1f, .95f, .43f), new Color(1f, .95f, .43f), new Color(1f, .39f, .08f), new Color(1f, .39f, .08f));
            var outline = logoText.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(.45f, .03f, .08f);
            outline.effectDistance = new Vector2(1.5f, -1.5f);
            logo = logoText.rectTransform;

            TMP_Text tagline = ArcadeUI.PixelText(stage, "Subtítulo", "O DEBATE SAIU DO CONTROLE", 5, Muted, TextAlignmentOptions.Center);
            ArcadeUI.Place(tagline.rectTransform, new Vector2(SideWidth * .5f, 56), new Vector2(SideWidth - 8, 8));
            Image rule = ArcadeUI.Panel(stage, "Divisor", new Color(Gold.r, Gold.g, Gold.b, .45f));
            ArcadeUI.Place(rule.rectTransform, new Vector2(SideWidth * .5f, 70), new Vector2(SideWidth - 32, 1));

            // Opções
            for (int i = 0; i < Labels.Length; i++)
            {
                float y = RowTop + i * RowGap;
                Image bar = ArcadeUI.Panel(stage, "Destaque " + Labels[i], Gold);
                ArcadeUI.Place(bar.rectTransform, new Vector2(SideWidth * .5f, y), new Vector2(SideWidth - 20, RowHeight));
                rowBars.Add(bar);
                TMP_Text label = ArcadeUI.PixelText(stage, Labels[i], Labels[i], 7, Color.white, TextAlignmentOptions.Left);
                ArcadeUI.Place(label.rectTransform, new Vector2(SideWidth * .5f, y), new Vector2(SideWidth - 28, RowHeight));
                rowLabels.Add(label);
            }
            difficultyValue = ArcadeUI.PixelText(stage, "Valor da dificuldade", "", 7, Color.white, TextAlignmentOptions.Right);
            ArcadeUI.Place(difficultyValue.rectTransform, new Vector2(SideWidth * .5f, RowTop + (int)Option.Difficulty * RowGap), new Vector2(SideWidth - 28, RowHeight));

            // Descrição da opção selecionada
            Image box = ArcadeUI.Panel(stage, "Caixa de descrição", new Color(1f, 1f, 1f, .05f));
            ArcadeUI.Place(box.rectTransform, new Vector2(SideWidth * .5f, 213), new Vector2(SideWidth - 20, 30));
            description = ArcadeUI.PixelText(stage, "Descrição", "", 5, Muted, TextAlignmentOptions.Center);
            description.textWrappingMode = TextWrappingModes.Normal;
            description.lineSpacing = 18;
            ArcadeUI.Place(description.rectTransform, new Vector2(SideWidth * .5f, 213), new Vector2(SideWidth - 30, 26));

            // Rodapé
            TMP_Text keys = ArcadeUI.PixelText(stage, "Teclas", "W/S ESCOLHER   ENTER CONFIRMAR", 4, new Color(1f, 1f, 1f, .55f), TextAlignmentOptions.Center);
            ArcadeUI.Place(keys.rectTransform, new Vector2(SideWidth * .5f, 241), new Vector2(SideWidth - 8, 6));
            TMP_Text disclaimer = ArcadeUI.PixelText(stage, "Aviso", "Obra de ficção e sátira.\nNão representa fatos reais.", 4, new Color(1f, 1f, 1f, .4f), TextAlignmentOptions.Center);
            disclaimer.lineSpacing = 25;
            ArcadeUI.Place(disclaimer.rectTransform, new Vector2(SideWidth * .5f, 257), new Vector2(SideWidth - 8, 14));
        }

        private void BuildControlsPanel()
        {
            Image dim = ArcadeUI.Panel(stage, "Painel de controles", new Color(0, 0, 0, .72f));
            ArcadeUI.Place(dim.rectTransform, new Vector2(240, 135), new Vector2(480 * 3, 270 * 3)); // cobre também as sobras da tela
            controlsPanel = dim.gameObject;

            Image card = ArcadeUI.Panel(dim.transform, "Cartão", new Color(.045f, .012f, .11f, 1f));
            ArcadeUI.Anchor(card.rectTransform, new Vector2(.5f, .5f), new Vector2(.5f, .5f), Vector2.zero, new Vector2(300, 190));
            card.gameObject.AddComponent<Outline>().effectColor = Gold;

            TMP_Text title = ArcadeUI.PixelText(card.transform, "Título", "CONTROLES", 10, Gold, TextAlignmentOptions.Center);
            ArcadeUI.Anchor(title.rectTransform, new Vector2(.5f, 1f), new Vector2(.5f, 1f), new Vector2(0, -16), new Vector2(280, 14));

            var rows = new (string key, string action)[]
            {
                ("A / D", "MOVER"),
                ("W / ESPAÇO", "PULAR (2X NO AR)"),
                ("S", "DESCER DA PLATAFORMA"),
                ("SHIFT", "ESQUIVA"),
                ("J", "ATAQUE (COMBO 3X)"),
                ("K / L / I", "HABILIDADES"),
                ("ESC", "VOLTAR AO MENU"),
            };
            for (int i = 0; i < rows.Length; i++)
            {
                float y = -38 - i * 15;
                TMP_Text key = ArcadeUI.PixelText(card.transform, "Tecla", rows[i].key, 6, Gold, TextAlignmentOptions.Right);
                ArcadeUI.Anchor(key.rectTransform, new Vector2(.5f, 1f), new Vector2(.5f, 1f), new Vector2(-58, y), new Vector2(110, 10));
                TMP_Text act = ArcadeUI.PixelText(card.transform, "Ação", rows[i].action, 6, Color.white, TextAlignmentOptions.Left);
                ArcadeUI.Anchor(act.rectTransform, new Vector2(.5f, 1f), new Vector2(.5f, 1f), new Vector2(72, y), new Vector2(130, 10));
            }

            TMP_Text goal = ArcadeUI.PixelText(card.transform, "Objetivo", "Tire o rival da arena 3 vezes.\nQuanto maior o %, mais longe ele voa.", 5, Muted, TextAlignmentOptions.Center);
            goal.lineSpacing = 20;
            ArcadeUI.Anchor(goal.rectTransform, new Vector2(.5f, 0f), new Vector2(.5f, 0f), new Vector2(0, 34), new Vector2(280, 18));
            TMP_Text close = ArcadeUI.PixelText(card.transform, "Fechar", "ENTER OU ESC: FECHAR", 4, new Color(1f, 1f, 1f, .55f), TextAlignmentOptions.Center);
            ArcadeUI.Anchor(close.rectTransform, new Vector2(.5f, 0f), new Vector2(.5f, 0f), new Vector2(0, 12), new Vector2(280, 8));
            controlsPanel.SetActive(false);
        }

        // ---------------------------------------------------------------- entrada e animação

        private void Update()
        {
            if (!visible) return;
            clock += Time.unscaledDeltaTime;
            Animate();
            if (GameFlow.InputLocked) return;

            if (controlsPanel.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
                {
                    controlsPanel.SetActive(false);
                    sound.Blip();
                }
                return;
            }

            float vertical = Input.GetAxisRaw("Vertical"), horizontal = Input.GetAxisRaw("Horizontal");
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || (vertical < -.55f && axisLatchY >= -.55f)) Move(1);
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || (vertical > .55f && axisLatchY <= .55f)) Move(-1);
            if (selected == (int)Option.Difficulty)
            {
                if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || (horizontal > .55f && axisLatchX <= .55f)) CycleDifficulty(1);
                if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || (horizontal < -.55f && axisLatchX >= -.55f)) CycleDifficulty(-1);
            }
            axisLatchY = vertical;
            axisLatchX = horizontal;
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0)) Confirm();
        }

        private void Animate()
        {
            float shake = Mathf.Max(0, 1 - clock * 2) * 2;
            logo.anchoredPosition = new Vector2(Mathf.Round(Mathf.Sin(clock * 34) * shake), 0);
            backdropHolder.localScale = Vector3.one * 1.03f;
            backdropHolder.anchoredPosition = new Vector2(Mathf.Sin(clock * .3f) * 4f, 0);
            for (int i = 0; i < figures.Count; i++)
                figures[i].localScale = new Vector3(1f, 1f + .012f * Mathf.Sin(clock * 2.4f + i * 1.7f), 1f); // respiração
            // A barra selecionada "pulsa" de leve.
            rowBars[selected].color = Color.Lerp(Gold, new Color(1f, .86f, .4f), .5f + .5f * Mathf.Sin(clock * 5f));
        }

        private void Move(int delta)
        {
            selected = (selected + delta + Labels.Length) % Labels.Length;
            sound.Blip();
            Refresh();
        }

        private void CycleDifficulty(int delta)
        {
            BotDifficulty.Current = BotDifficulty.Next(BotDifficulty.Current, delta);
            sound.Blip();
            Refresh();
        }

        private void Refresh()
        {
            Difficulty level = BotDifficulty.Current;
            for (int i = 0; i < Labels.Length; i++)
            {
                bool active = i == selected;
                rowBars[i].enabled = active;
                rowBars[i].color = Gold;
                rowLabels[i].color = active ? Ink : Color.white;
            }
            bool onDifficulty = selected == (int)Option.Difficulty;
            difficultyValue.text = onDifficulty ? "<" + BotDifficulty.Label(level) + ">" : BotDifficulty.Label(level);
            difficultyValue.color = onDifficulty ? Ink : BotDifficulty.Tint(level);

            description.text = (Option)selected switch
            {
                Option.Play => "Escolha seu candidato e enfrente o bot.",
                Option.Demo => "Bot contra bot, sem parar. Ideal para apresentar. ESC sai.",
                Option.Difficulty => BotDifficulty.Description(level) + " (A/D muda)",
                Option.Controls => "Teclas de movimento, ataque e habilidades.",
                _ => "Fecha o jogo."
            };
        }

        private void Confirm()
        {
            switch ((Option)selected)
            {
                case Option.Play: sound.Confirm(); flow.OpenCharacterSelect(); break;
                case Option.Demo: sound.Confirm(); flow.StartDemo(); break;
                case Option.Difficulty: CycleDifficulty(1); break;
                case Option.Controls: sound.Confirm(); controlsPanel.SetActive(true); controlsPanel.transform.SetAsLastSibling(); break;
                default:
                    sound.Confirm();
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                    break;
            }
        }
    }

    public static class ArcadeUI
    {
        private static Font font;
        public static GameObject CreateRoot(string name)
        {
            var root = new GameObject(name, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            root.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(480, 270);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            return root;
        }
        public static Font Font => font ??= Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        private static TMP_FontAsset pixelFont;

        /// <summary>Press Start 2P como fonte TMP, compartilhada por menu, seleção e HUD.</summary>
        public static TMP_FontAsset PixelFont
        {
            get
            {
                if (pixelFont != null) return pixelFont;
                Font source = Resources.Load<Font>("Fonts/PressStart2P-Regular");
                if (source == null) return null;
                pixelFont = TMP_FontAsset.CreateFontAsset(source, 14, 2, UnityEngine.TextCore.LowLevel.GlyphRenderMode.RASTER_HINTED, 1024, 1024);
                pixelFont.material.mainTexture.filterMode = FilterMode.Point;
                return pixelFont;
            }
        }

        public static TMP_Text PixelText(Transform parent, string name, string value, float size, Color color, TextAlignmentOptions align)
        {
            var go = new GameObject(name, typeof(TextMeshProUGUI));
            go.transform.SetParent(parent, false);
            var text = go.GetComponent<TextMeshProUGUI>();
            text.font = PixelFont; text.text = value; text.fontSize = size; text.color = color; text.alignment = align;
            text.textWrappingMode = TextWrappingModes.NoWrap; text.overflowMode = TextOverflowModes.Overflow;
            text.raycastTarget = false;
            return text;
        }

        /// <summary>Área fixa de 480x270 centralizada; use <see cref="Place"/> com coordenadas a partir do topo.</summary>
        public static RectTransform Stage(Transform parent)
        {
            var go = new GameObject("Palco 480x270", typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rect = (RectTransform)go.transform;
            rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f);
            rect.sizeDelta = new Vector2(480, 270);
            rect.anchoredPosition = Vector2.zero;
            return rect;
        }

        /// <summary>Posiciona pelo centro, em coordenadas do palco 480x270 contadas a partir do topo.</summary>
        public static void Place(RectTransform rect, Vector2 centerFromTop, Vector2 size)
        {
            rect.anchorMin = rect.anchorMax = new Vector2(centerFromTop.x / 480f, 1f - centerFromTop.y / 270f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = size;
        }

        /// <summary>Posiciona pelos pés (pivô embaixo, no centro).</summary>
        public static void PlaceFeet(RectTransform rect, Vector2 feetFromTop)
        {
            Vector2 size = rect.sizeDelta;
            Place(rect, feetFromTop, size);
            rect.pivot = new Vector2(.5f, 0f);
        }

        /// <summary>Imagem de personagem com a proporção da textura e altura fixa.</summary>
        public static RawImage Figure(Transform parent, string name, Texture2D texture, float height)
        {
            RawImage image = Raw(parent, name, texture, Color.white);
            float aspect = texture != null ? texture.width / (float)texture.height : .6f;
            image.rectTransform.sizeDelta = new Vector2(height * aspect, height);
            image.raycastTarget = false;
            return image;
        }

        private static Sprite softEllipse;

        /// <summary>Elipse com borda suave, para sombras no chão.</summary>
        public static Sprite SoftEllipse
        {
            get
            {
                if (softEllipse != null) return softEllipse;
                const int w = 64, h = 16;
                var tex = new Texture2D(w, h, TextureFormat.RGBA32, false) { filterMode = FilterMode.Bilinear, wrapMode = TextureWrapMode.Clamp };
                for (int y = 0; y < h; y++)
                    for (int x = 0; x < w; x++)
                    {
                        float dx = (x + .5f) / w * 2f - 1f, dy = (y + .5f) / h * 2f - 1f;
                        float d = Mathf.Sqrt(dx * dx + dy * dy);
                        tex.SetPixel(x, y, new Color(1, 1, 1, Mathf.Clamp01(1f - d) * 1.4f));
                    }
                tex.Apply();
                softEllipse = Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(.5f, .5f));
                return softEllipse;
            }
        }

        /// <summary>Imagem que cobre a tela inteira mantendo a proporção (corta o excesso, sem faixas).</summary>
        public static RawImage Cover(Transform parent, string name, Texture texture)
        {
            var holder = new GameObject(name, typeof(RectTransform));
            holder.transform.SetParent(parent, false);
            Stretch((RectTransform)holder.transform);
            RawImage image = Raw(holder.transform, "Imagem", texture, Color.white);
            image.rectTransform.anchorMin = image.rectTransform.anchorMax = new Vector2(.5f, .5f);
            var fitter = image.gameObject.AddComponent<AspectRatioFitter>();
            fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            fitter.aspectRatio = texture != null ? texture.width / (float)texture.height : 16f / 9f;
            return image;
        }
        public static Text Text(Transform parent, string name, string value, int size, Color color, TextAnchor align)
        {
            var go = new GameObject(name, typeof(Text)); go.transform.SetParent(parent, false);
            var text = go.GetComponent<Text>(); text.font = Font; text.text = value; text.fontSize = size; text.fontStyle = FontStyle.Bold; text.color = color; text.alignment = align;
            return text;
        }
        public static Image Panel(Transform parent, string name, Color color)
        {
            var go = new GameObject(name, typeof(Image)); go.transform.SetParent(parent, false);
            var image = go.GetComponent<Image>(); image.color = color; image.raycastTarget = false; return image;
        }
        public static RawImage Raw(Transform parent, string name, Texture texture, Color color)
        {
            var go = new GameObject(name, typeof(RawImage)); go.transform.SetParent(parent, false);
            var image = go.GetComponent<RawImage>(); image.texture = texture; image.color = color; return image;
        }
        public static void Stretch(RectTransform rect) { rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.offsetMin = rect.offsetMax = Vector2.zero; }
        public static void Anchor(RectTransform rect, Vector2 min, Vector2 max, Vector2 position, Vector2 size) { rect.anchorMin = min; rect.anchorMax = max; rect.anchoredPosition = position; rect.sizeDelta = size; }
    }
}
