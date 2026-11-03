using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TSEFatal
{
    public class MainMenuController : MonoBehaviour
    {
        private static readonly string[] Options = { "JOGAR", "CONTROLES", "OPÇÕES", "SAIR" };
        private readonly List<Image> buttons = new List<Image>();
        private readonly List<TMP_Text> labels = new List<TMP_Text>();
        private readonly List<RectTransform> fighters = new List<RectTransform>();
        private readonly List<RectTransform> shadows = new List<RectTransform>();
        private readonly List<RectTransform> beams = new List<RectTransform>();
        private GameFlow flow;
        private GameObject root;
        private RectTransform logo;
        private TMP_Text logoText;
        private RectTransform cursor;
        private GameObject overlayPanel;
        private TMP_Text overlay;
        private Image tint;
        private RawImage backdrop;
        private RectTransform stage;
        private CanvasScaler scaler;
        private MenuAudio sound;
        private int selected;
        private float clock;
        private bool visible;
        private float axisLatch;
        private static TMP_FontAsset pixelFont;
        private static Sprite square;

        private static readonly Color Purple = new Color(.10f, .035f, .20f);
        private static readonly Color Red = new Color(.65f, .055f, .12f);
        private static readonly Color Gold = new Color(1f, .72f, .13f);

        public void Initialize(GameFlow owner) { flow = owner; sound = MenuAudio.Ensure(gameObject); }
        public void Show()
        {
            if (root == null) Build();
            root.SetActive(true);
            visible = true;
            selected = 0;
            clock = 0;
            Refresh();
        }
        public void Hide() { visible = false; if (root != null) root.SetActive(false); }

        private void Build()
        {
            root = ArcadeUI.CreateRoot("Main Menu 480x270");
            root.GetComponent<Canvas>().sortingOrder = 20;
            scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            scaler.scaleFactor = Mathf.Max(1, Mathf.FloorToInt(Mathf.Min(Screen.width / 480f, Screen.height / 270f)));
            var stageObject = new GameObject("Pixel Stage", typeof(RectTransform));
            stageObject.transform.SetParent(root.transform, false);
            stage = stageObject.GetComponent<RectTransform>();
            stage.anchorMin = stage.anchorMax = new Vector2(.5f, .5f);
            stage.sizeDelta = new Vector2(480, 270);
            stage.anchoredPosition = Vector2.zero;
            Font source = Resources.Load<Font>("Fonts/PressStart2P-Regular");
            if (source != null && pixelFont == null)
            {
                pixelFont = TMP_FontAsset.CreateFontAsset(source, 14, 2, UnityEngine.TextCore.LowLevel.GlyphRenderMode.RASTER_HINTED, 1024, 1024);
                pixelFont.material.mainTexture.filterMode = FilterMode.Point;
            }
            Texture2D scene = Resources.Load<Texture2D>("Art/Menu/menu_debate_arena");
            backdrop = ArcadeUI.Raw(stage, "Arena de debate", scene, Color.white);
            ArcadeUI.Stretch(backdrop.rectTransform);
            // A dark violet veil keeps the art detailed while the foreground remains legible.
            tint = ArcadeUI.Panel(stage, "Luz roxa e vermelha", new Color(.18f, .02f, .28f, .20f));
            ArcadeUI.Stretch(tint.rectTransform);
            for (int i = 0; i < 2; i++)
            {
                var beam = Panel("Holofote lateral", new Color(1f, .32f, .25f, .12f), new Vector2(i == 0 ? 20 : 460, 112), new Vector2(13, 205), stage);
                beams.Add(beam.rectTransform);
            }
            Panel("Faixa do logo", new Color(.045f, .012f, .11f, .84f), new Vector2(240, 50), new Vector2(480, 86), stage);
            Panel("Borda superior", Gold, new Vector2(240, 9), new Vector2(480, 2), stage);
            Panel("Borda inferior", Red, new Vector2(240, 91), new Vector2(480, 2), stage);
            logoText = Text("TSE FATAL", 34, Gold, new Vector2(240, 50), new Vector2(450, 64), stage);
            logoText.enableVertexGradient = true;
            logoText.colorGradient = new VertexGradient(new Color(1f, .95f, .43f), new Color(1f, .95f, .43f), new Color(1f, .39f, .08f), new Color(1f, .39f, .08f));
            logo = logoText.rectTransform;
            var outline = logoText.gameObject.AddComponent<Outline>();
            outline.effectColor = Color.black;
            outline.effectDistance = new Vector2(2, -2);
            var shadow = logoText.gameObject.AddComponent<Shadow>();
            shadow.effectColor = Red;
            shadow.effectDistance = new Vector2(4, -4);

            for (int i = 0; i < 4; i++)
            {
                float y = 126 + i * 29;
                var frame = Panel("Botão " + Options[i], new Color(.09f, .025f, .18f, .96f), new Vector2(110, y), new Vector2(180, 25), stage);
                frame.gameObject.AddComponent<Outline>().effectColor = Gold;
                buttons.Add(frame);
                labels.Add(Text(Options[i], 12, Color.white, new Vector2(110, y), new Vector2(166, 20), stage));
            }
            var cursorObject = new GameObject("Seta pixelada", typeof(Image));
            cursorObject.transform.SetParent(stage, false);
            var arrowTexture = new Texture2D(10, 12, TextureFormat.RGBA32, false);
            arrowTexture.filterMode = FilterMode.Point;
            for (int y = 0; y < 12; y++)
            {
                int width = 10 - Mathf.Abs(y - 5) * 2;
                for (int x = 0; x < 10; x++) arrowTexture.SetPixel(x, y, x < width ? Purple : Color.clear);
            }
            arrowTexture.Apply();
            cursorObject.GetComponent<Image>().sprite = Sprite.Create(arrowTexture, new Rect(0, 0, 10, 12), new Vector2(.5f, .5f));
            cursor = cursorObject.GetComponent<RectTransform>();
            Place(cursor, new Vector2(31, 126), new Vector2(10, 12));

            string[] paths = { "Art/Characters/lula_idle", "Art/Characters/renan_idle", "Art/Characters/flavio_sigma" };
            for (int i = 0; i < 3; i++)
            {
                float x = 271 + i * 75;
                shadows.Add(Panel("Sombra no chão", new Color(0, 0, 0, .72f), new Vector2(x, 226), new Vector2(66, 8), stage).rectTransform);
                Texture2D tex = Resources.Load<Texture2D>(paths[i]);
                var portrait = ArcadeUI.Raw(stage, "Lutador " + i, tex, Color.white);
                Place(portrait.rectTransform, new Vector2(x, 164), new Vector2(75, 132));
                portrait.uvRect = new Rect(0, 0, 1, 1);
                fighters.Add(portrait.rectTransform);
            }
            Panel("Faixa do rodapé", new Color(.035f, .008f, .08f, .94f), new Vector2(240, 257), new Vector2(480, 26), stage);
            Text("Obra de ficção e sátira. Não representa fatos reais.", 6, Color.white, new Vector2(240, 256), new Vector2(466, 17), stage);
            var overlayFrame = Panel("Painel de informações", new Color(.045f, .012f, .11f, 1f), new Vector2(240, 169), new Vector2(426, 142), stage);
            overlayFrame.gameObject.AddComponent<Outline>().effectColor = Gold;
            overlayPanel = overlayFrame.gameObject;
            overlay = Text("", 8, Gold, new Vector2(240, 169), new Vector2(406, 118), overlayFrame.transform);
            overlay.rectTransform.anchorMin = overlay.rectTransform.anchorMax = new Vector2(.5f, .5f);
            overlay.rectTransform.anchoredPosition = Vector2.zero;
            overlayPanel.SetActive(false);
        }

        private void Update()
        {
            if (!visible) return;
            clock += Time.unscaledDeltaTime;
            scaler.scaleFactor = Mathf.Max(1, Mathf.FloorToInt(Mathf.Min(Screen.width / 480f, Screen.height / 270f)));
            float vertical = Input.GetAxisRaw("Vertical");
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || (vertical < -.55f && axisLatch >= -.55f)) Change(1);
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || (vertical > .55f && axisLatch <= .55f)) Change(-1);
            axisLatch = vertical;
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0)) Confirm();
            if (Input.GetKeyDown(KeyCode.Escape)) overlayPanel.SetActive(false);
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.F12))
                ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "outputs", "TSE-Fatal-menu.png"));
#endif
            Animate();
        }
        private void Animate()
        {
            float shake = Mathf.Max(0, 1 - clock * 2) * 2;
            logo.anchoredPosition = new Vector2(Mathf.Round(Mathf.Sin(clock * 34) * shake), 0);
            logoText.color = Color.Lerp(Gold, new Color(1f, .42f, .08f), .5f + .5f * Mathf.Sin(clock * 3));
            if (Mathf.Repeat(clock, 3f) < .24f) logoText.color = Color.white;
            tint.color = Color.Lerp(new Color(.23f, .01f, .27f, .17f), new Color(.45f, .025f, .065f, .27f), .5f + .5f * Mathf.Sin(clock * 1.5f));
            backdrop.rectTransform.anchoredPosition = new Vector2(Mathf.Round(Mathf.Sin(clock * .35f) * 3), 0);
            backdrop.rectTransform.sizeDelta = new Vector2(8, 0);
            for (int i = 0; i < beams.Count; i++) beams[i].localRotation = Quaternion.Euler(0, 0, (i == 0 ? -1 : 1) * (18 + 14 * Mathf.Sin(clock * .8f + i)));
            for (int i = 0; i < fighters.Count; i++)
            {
                fighters[i].anchoredPosition = new Vector2(0, Mathf.Round(Mathf.Sin(clock * 2.3f + i) * 2));
                shadows[i].sizeDelta = new Vector2(66 - Mathf.Sin(clock * 2.3f + i) * 2, 8);
            }
            cursor.anchorMin = cursor.anchorMax = new Vector2(31f / 480f, 1f - (126 + selected * 29f) / 270f);
            cursor.anchoredPosition = new Vector2(Mathf.Round(Mathf.Sin(clock * 8) * 2), 0);
        }
        private void Change(int delta) { selected = (selected + delta + 4) % 4; sound.Blip(); Refresh(); }
        private void Refresh()
        {
            for (int i = 0; i < 4; i++)
            {
                bool active = i == selected;
                buttons[i].color = active ? Gold : new Color(.09f, .025f, .18f, .96f);
                labels[i].color = active ? Purple : Color.white;
                buttons[i].rectTransform.localScale = active ? Vector3.one * 1.05f : Vector3.one;
            }
        }
        private void Confirm()
        {
            sound.Confirm();
            if (selected == 0) flow.OpenCharacterSelect();
            else if (selected == 1) ShowOverlay("CONTROLES\nA/D: MOVER   W: PULAR\nJ: ATAQUE   K: ESPECIAL\nESC: VOLTAR");
            else if (selected == 2) ShowOverlay("OPÇÕES\nÁUDIO E VÍDEO EM BREVE\nESC: VOLTAR");
            else Application.Quit();
        }
        private void ShowOverlay(string message) { overlay.text = message; overlayPanel.SetActive(true); overlayPanel.transform.SetAsLastSibling(); }
        private static Image Panel(string name, Color color, Vector2 topLeftCenter, Vector2 size, Transform parent)
        {
            var image = ArcadeUI.Panel(parent, name, color);
            Place(image.rectTransform, topLeftCenter, size);
            return image;
        }
        private static TMP_Text Text(string value, float size, Color color, Vector2 topLeftCenter, Vector2 dimensions, Transform parent)
        {
            var go = new GameObject(value, typeof(TextMeshProUGUI));
            go.transform.SetParent(parent, false);
            var text = go.GetComponent<TextMeshProUGUI>();
            text.font = pixelFont;
            text.text = value;
            text.fontSize = size;
            text.color = color;
            text.alignment = TextAlignmentOptions.Center;
            text.textWrappingMode = TextWrappingModes.NoWrap;
            text.overflowMode = TextOverflowModes.Overflow;
            Place(text.rectTransform, topLeftCenter, dimensions);
            return text;
        }
        private static void Place(RectTransform rect, Vector2 centerFromTop, Vector2 size)
        {
            rect.anchorMin = rect.anchorMax = new Vector2(centerFromTop.x / 480f, 1f - centerFromTop.y / 270f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = size;
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
        public static Text Text(Transform parent, string name, string value, int size, Color color, TextAnchor align)
        {
            var go = new GameObject(name, typeof(Text)); go.transform.SetParent(parent, false);
            var text = go.GetComponent<Text>(); text.font = Font; text.text = value; text.fontSize = size; text.fontStyle = FontStyle.Bold; text.color = color; text.alignment = align;
            return text;
        }
        public static Image Panel(Transform parent, string name, Color color)
        {
            var go = new GameObject(name, typeof(Image)); go.transform.SetParent(parent, false);
            var image = go.GetComponent<Image>(); image.color = color; return image;
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
