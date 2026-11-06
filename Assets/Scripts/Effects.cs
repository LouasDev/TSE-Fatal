using UnityEngine;

namespace TSEFatal
{
    /// <summary>Efeitos visuais curtos da luta. Tudo fica sob Effects.Root e some ao sair da luta.</summary>
    public static class Effects
    {
        public static Transform Root;
        private static Font calloutFont;
        private static Material silhouette;

        /// <summary>Material que pinta o sprite numa cor sólida (piscar branco, rastros).</summary>
        public static Material Silhouette
        {
            get
            {
                if (silhouette == null)
                {
                    Shader shader = Shader.Find("GUI/Text Shader");
                    silhouette = shader != null ? new Material(shader) : new Material(Shader.Find("Sprites/Default"));
                }
                return silhouette;
            }
        }

        public static float ScaleToHeight(Sprite sprite, float height) => sprite == null ? 1f : height / Mathf.Max(.01f, sprite.bounds.size.y);

        /// <summary>Escala pelo frame mais alto: numa explosão que cresce, o primeiro frame continua pequeno.</summary>
        public static float ScaleToHeight(Sprite[] frames, float height)
        {
            float tallest = .01f;
            if (frames != null) foreach (Sprite s in frames) if (s != null) tallest = Mathf.Max(tallest, s.bounds.size.y);
            return height / tallest;
        }

        /// <summary>
        /// Efeito com frames. loop = false toca a sequência uma vez ao longo de <paramref name="life"/>
        /// (explosão, impacto); loop = true repete em <paramref name="fps"/> (algo que gira ou corre).
        /// </summary>
        public static FxParticle Anim(Sprite[] frames, Vector2 position, float height, float life, int order = 10, bool loop = false, float fps = 12f)
        {
            if (frames == null || frames.Length == 0) return null;
            FxParticle fx = Sprite(frames[0], position, height, order);
            fx.BaseScale = ScaleToHeight(frames, height);
            fx.transform.localScale = Vector3.one * fx.BaseScale;
            fx.Frames = frames; fx.LoopFrames = loop; fx.FrameRate = fps;
            fx.Life = life;
            return fx;
        }

        public static FxParticle Sprite(Sprite sprite, Vector2 position, float height, int order = 10)
        {
            var go = new GameObject("FX " + (sprite != null ? sprite.name : "?"));
            if (Root != null) go.transform.SetParent(Root, false);
            go.transform.position = position;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = order;
            var fx = go.AddComponent<FxParticle>();
            fx.Renderer = sr;
            fx.BaseScale = ScaleToHeight(sprite, height);
            go.transform.localScale = Vector3.one * fx.BaseScale;
            return fx;
        }

        public static void Spark(Vector2 position, Color color, float size)
        {
            var core = Sprite(PixelArt.Spark, position, size, 12);
            core.Renderer.color = color;
            core.Life = .16f; core.EndScale = 1.6f; core.Spin = Random.Range(-200f, 200f);
            var ring = Sprite(PixelArt.Ring, position, size * .8f, 11);
            ring.Renderer.color = new Color(color.r, color.g, color.b, .8f);
            ring.Life = .22f; ring.EndScale = 2.6f;
            for (int i = 0; i < 5; i++)
            {
                var bit = Sprite(PixelArt.Pixel, position, .14f, 12);
                bit.Renderer.color = color;
                bit.Velocity = Random.insideUnitCircle.normalized * Random.Range(5f, 10f);
                bit.Gravity = 18f; bit.Life = Random.Range(.2f, .35f);
            }
        }

        public static void Burst(Sprite sprite, Vector2 position, int count, float speed, float height, float gravity = 22f) =>
            Burst(new[] { sprite }, position, count, speed, height, gravity);

        /// <summary>Chuva de objetos (notas, moedas). Com vários frames, cada um gira/tremula em loop.</summary>
        public static void Burst(Sprite[] frames, Vector2 position, int count, float speed, float height, float gravity = 22f)
        {
            bool animated = frames.Length > 1;
            for (int i = 0; i < count; i++)
            {
                var p = animated ? Anim(frames, position, height, 1f, 9, true, Random.Range(10f, 16f)) : Sprite(frames[0], position, height, 9);
                Vector2 dir = Random.insideUnitCircle.normalized;
                dir.y = Mathf.Abs(dir.y) + .4f;
                p.Velocity = dir.normalized * Random.Range(speed * .5f, speed);
                p.Gravity = gravity;
                p.Spin = animated ? Random.Range(-90f, 90f) : Random.Range(-540f, 540f); // os frames já mostram o giro
                p.Life = Random.Range(.6f, 1.1f);
            }
        }

        public static void Dust(Vector2 position, int count = 4)
        {
            for (int i = 0; i < count; i++)
            {
                var d = Sprite(PixelArt.Dust, position + new Vector2(Random.Range(-.4f, .4f), .1f), .35f, 3);
                d.Velocity = new Vector2(Random.Range(-2.5f, 2.5f), Random.Range(.3f, 1.4f));
                d.Life = Random.Range(.25f, .45f); d.EndScale = 1.8f;
            }
        }

        /// <summary>Tempo padrão das frases de piada: dá para ler com calma.</summary>
        public const float JokeTime = 2.4f;
        /// <summary>Avisos curtos (nome da habilidade, "TONTO!").</summary>
        public const float ShortTime = 1.4f;

        private static readonly System.Collections.Generic.List<FxParticle> liveCallouts = new System.Collections.Generic.List<FxParticle>();

        /// <summary>
        /// Frase flutuante. Aparece com um "pop", fica parada quase todo o tempo e só no fim sobe e some.
        /// Se já houver outra frase no mesmo lugar, a antiga sobe para não ficar uma em cima da outra.
        /// </summary>
        public static void Callout(string text, Vector2 position, Color color, float size = 1f, float life = JokeTime)
        {
            if (calloutFont == null) calloutFont = Resources.Load<Font>("Fonts/PressStart2P-Regular");
            liveCallouts.RemoveAll(c => c == null);
            foreach (FxParticle other in liveCallouts)
            {
                Vector2 p = other.transform.position;
                if (Mathf.Abs(p.x - position.x) < 3.5f && Mathf.Abs(p.y - position.y) < .9f)
                    other.transform.position = new Vector3(p.x, position.y + .95f, 0f);
            }
            var go = new GameObject("Callout " + text);
            if (Root != null) go.transform.SetParent(Root, false);
            go.transform.position = position;
            var shadowGo = new GameObject("Sombra");
            shadowGo.transform.SetParent(go.transform, false);
            shadowGo.transform.localPosition = new Vector3(.06f, -.06f, .01f);
            TextMesh shadow = MakeText(shadowGo, text, new Color(0, 0, 0, .85f), size, 19);
            TextMesh main = MakeText(go, text, color, size, 20);
            var fx = go.AddComponent<FxParticle>();
            fx.Text = main; fx.ShadowText = shadow;
            fx.Velocity = new Vector2(0, 1.4f); fx.Life = life; fx.Pop = true;
            liveCallouts.Add(fx);
        }

        private static TextMesh MakeText(GameObject go, string text, Color color, float size, int order)
        {
            var tm = go.AddComponent<TextMesh>();
            tm.text = text; tm.color = color;
            tm.anchor = TextAnchor.MiddleCenter; tm.alignment = TextAlignment.Center;
            tm.fontSize = 48; tm.characterSize = .11f * size;
            if (calloutFont != null)
            {
                tm.font = calloutFont;
                go.GetComponent<MeshRenderer>().sharedMaterial = calloutFont.material;
            }
            go.GetComponent<MeshRenderer>().sortingOrder = order;
            return tm;
        }

        /// <summary>Cópia do sprite atual que desbota (esquiva / investidas).</summary>
        public static void AfterImage(SpriteRenderer source, Color color)
        {
            if (source == null || source.sprite == null) return;
            var go = new GameObject("Rastro");
            if (Root != null) go.transform.SetParent(Root, false);
            go.transform.SetPositionAndRotation(source.transform.position, source.transform.rotation);
            go.transform.localScale = source.transform.lossyScale;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = source.sprite; sr.sharedMaterial = Silhouette; sr.color = color; sr.sortingOrder = source.sortingOrder - 1;
            var fx = go.AddComponent<FxParticle>();
            fx.Renderer = sr; fx.Life = .22f; fx.KeepScale = true;
        }

        /// <summary>Explosão de nocaute na borda da tela, apontando para dentro.</summary>
        public static void KOBlast(Vector2 position, Color color)
        {
            for (int i = 0; i < 3; i++)
            {
                var ring = Sprite(PixelArt.Ring, position, 2f + i, 15);
                ring.Renderer.color = i == 1 ? Color.white : color;
                ring.Life = .45f + i * .1f; ring.EndScale = 4f;
            }
            Burst(PixelArt.Spark, position, 14, 16f, .5f, 0f);
        }
    }

    public class FxParticle : MonoBehaviour
    {
        public SpriteRenderer Renderer;
        public TextMesh Text, ShadowText;
        public Vector2 Velocity;
        public float Gravity, Spin, Life = .3f, EndScale = 1f, BaseScale = 1f;
        public bool Pop, KeepScale;
        public Sprite[] Frames;
        public bool LoopFrames;
        public float FrameRate = 12f;
        private float age;
        private Color startColor, startText, startShadow;

        private void Start()
        {
            if (Renderer != null) startColor = Renderer.color;
            if (Text != null) startText = Text.color;
            if (ShadowText != null) startShadow = ShadowText.color;
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            age += dt;
            float t = Mathf.Clamp01(age / Life);
            Velocity.y -= Gravity * dt;
            // Frases: quase paradas enquanto dá para ler; no último terço sobem e somem.
            float drift = Pop ? (t < .66f ? .15f : 1.6f) : 1f;
            transform.position += (Vector3)(Velocity * dt * drift);
            transform.Rotate(0, 0, Spin * dt);
            if (!KeepScale)
            {
                // O "pop" de entrada dura ~0,25 s, independente da duração total.
                float popIn = Mathf.Min(.12f, .25f / Life);
                float s = Pop ? (t < popIn ? Mathf.Lerp(.4f, 1.25f, t / popIn) : Mathf.Lerp(1.25f, 1f, (t - popIn) / popIn)) : Mathf.Lerp(1f, EndScale, t);
                transform.localScale = Vector3.one * BaseScale * s;
            }
            if (Frames != null && Frames.Length > 1 && Renderer != null)
            {
                int i = LoopFrames ? Mathf.FloorToInt(age * FrameRate) % Frames.Length : Mathf.Min(Frames.Length - 1, Mathf.FloorToInt(t * Frames.Length));
                Renderer.sprite = Frames[i];
            }
            bool sequence = Frames != null && Frames.Length > 1 && !LoopFrames;
            float alpha = Pop ? Mathf.Clamp01((1f - t) * 3f) : sequence ? Mathf.Clamp01((1f - t) * 6f) : 1f - t * t;
            if (Renderer != null) Renderer.color = new Color(startColor.r, startColor.g, startColor.b, startColor.a * alpha);
            if (Text != null) Text.color = new Color(startText.r, startText.g, startText.b, alpha);
            if (ShadowText != null) ShadowText.color = new Color(startShadow.r, startShadow.g, startShadow.b, startShadow.a * alpha);
            if (age >= Life) Destroy(gameObject);
        }
    }
}
