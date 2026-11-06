using System.Collections.Generic;
using UnityEngine;

namespace TSEFatal
{
    public sealed class AnimClip
    {
        public Sprite[] Frames;
        /// <summary>True quando veio de quadros reais (não da imagem estática).</summary>
        public bool FromSheet;
    }

    /// <summary>
    /// Carrega animações geradas por IA sem exigir grade exata. Dois formatos:
    /// - pasta com um PNG por frame: Characters/{pasta}/{estado}/01.png, 02.png... (recomendado);
    /// - folha única com os frames lado a lado: Characters/{pasta}/{estado}.png ou {estado}_N.png.
    /// Fundo transparente ou sólido (a cor do canto vira transparente; fundo verde recebe chroma key
    /// com remoção do "vazamento" verde nas bordas). O pivô de cada quadro fica no pé do personagem.
    /// </summary>
    public static class SpriteSheetLibrary
    {
        private const byte AlphaCut = 40;
        private static readonly Dictionary<string, AnimClip> cache = new Dictionary<string, AnimClip>();
        /// <summary>Altura em pixels do personagem em pé (frames do idle), por pasta: escala comum a todas as animações.</summary>
        private static readonly Dictionary<string, int> standingHeight = new Dictionary<string, int>();

        /// <summary>
        /// Animação de um estado. worldHeight vale para folhas únicas; para frames soltos a escala é a do
        /// personagem inteiro (idle = baseHeight), então poses agachadas ou com aura mantêm o tamanho real.
        /// </summary>
        public static AnimClip CharacterClip(string folder, string state, float worldHeight, float baseHeight = 0f)
        {
            string key = $"{folder}/{state}@{worldHeight}@{baseHeight}";
            if (cache.TryGetValue(key, out AnimClip cached)) return cached;

            AnimClip clip = FrameFolder(folder, state, baseHeight > 0f ? baseHeight : worldHeight);
            if (clip == null)
            {
                string basePath = $"Art/Characters/{folder}/{state}";
                Texture2D sheet = Resources.Load<Texture2D>(basePath);
                int expected = 0;
                for (int n = 2; sheet == null && n <= 16; n++)
                {
                    sheet = Resources.Load<Texture2D>($"{basePath}_{n}");
                    expected = n;
                }
                if (sheet != null) clip = Slice(sheet, expected, worldHeight, true);
            }
            cache[key] = clip;
            return clip;
        }

        /// <summary>Imagem única (personagem estático ou efeito), recortada e com pivô no pé.</summary>
        public static AnimClip Single(string resourcePath, float worldHeight, bool pivotAtFeet = true)
        {
            string key = $"{resourcePath}@{worldHeight}@{pivotAtFeet}";
            if (cache.TryGetValue(key, out AnimClip cached)) return cached;
            Texture2D texture = Resources.Load<Texture2D>(resourcePath);
            AnimClip clip = texture == null ? null : Slice(texture, 1, worldHeight, false, pivotAtFeet);
            cache[key] = clip;
            return clip;
        }

        // ---------------------------------------------------------------- frames soltos

        /// <summary>SourceCenter: centro do quadro original, em coordenadas normalizadas do recorte.</summary>
        private struct Frame { public Texture2D Texture; public float PivotX; public int Height; public Vector2 SourceCenter; }
        private static readonly Dictionary<string, List<Frame>> frameCache = new Dictionary<string, List<Frame>>();

        /// <summary>
        /// Efeito animado: Resources/Art/VFX/{nome}/01.png, 02.png... O frame mais alto fica com
        /// <paramref name="worldHeight"/> e os outros mantêm a proporção entre si.
        /// Devolve null se a pasta não existir (o jogo usa a imagem única ou o desenho em código).
        /// </summary>
        public static AnimClip EffectClip(string name, float worldHeight, bool pivotAtBottom)
        {
            string key = $"vfx/{name}@{worldHeight}@{pivotAtBottom}";
            if (cache.TryGetValue(key, out AnimClip cached)) return cached;
            List<Frame> frames = LoadFramesAt($"Art/VFX/{name}", "vfx_" + name);
            AnimClip clip = null;
            if (frames != null)
            {
                int tallest = 1;
                foreach (Frame f in frames) tallest = Mathf.Max(tallest, f.Height);
                float ppu = tallest / worldHeight;
                var sprites = new Sprite[frames.Count];
                for (int i = 0; i < frames.Count; i++)
                {
                    Texture2D t = frames[i].Texture;
                    // Pivô no centro do quadro original: o objeto fica onde foi desenhado e não "treme"
                    // quando o recorte de cada frame muda de tamanho (patas da onça, explosão do L).
                    Vector2 c = frames[i].SourceCenter;
                    Vector2 pivot = pivotAtBottom ? new Vector2(c.x, 0f) : c;
                    sprites[i] = Sprite.Create(t, new Rect(0, 0, t.width, t.height), pivot, ppu, 0, SpriteMeshType.FullRect);
                    sprites[i].name = $"vfx_{name}_{i}";
                }
                clip = new AnimClip { Frames = sprites, FromSheet = true };
            }
            cache[key] = clip;
            return clip;
        }

        private static AnimClip FrameFolder(string folder, string state, float baseHeight)
        {
            List<Frame> frames = LoadFrames(folder, state);
            if (frames == null) return null;

            // Escala comum: altura do personagem em pé (idle). Sem idle, usa o frame mais alto deste estado.
            if (!standingHeight.TryGetValue(folder, out int reference))
            {
                List<Frame> idle = state == "idle" ? frames : LoadFrames(folder, "idle");
                reference = 1;
                foreach (Frame f in idle ?? frames) reference = Mathf.Max(reference, f.Height);
                if (idle != null) standingHeight[folder] = reference;
            }

            float ppu = reference / baseHeight;
            var sprites = new Sprite[frames.Count];
            for (int i = 0; i < frames.Count; i++)
            {
                Texture2D t = frames[i].Texture;
                sprites[i] = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(frames[i].PivotX, 0f), ppu, 0, SpriteMeshType.FullRect);
                sprites[i].name = $"{folder}_{state}_{i}";
            }
            return new AnimClip { Frames = sprites, FromSheet = true };
        }

        /// <summary>Lê, tira o fundo e recorta cada PNG numa textura pequena; libera o original da memória.</summary>
        private static List<Frame> LoadFrames(string folder, string state) =>
            LoadFramesAt($"Art/Characters/{folder}/{state}", $"{folder}_{state}");

        private static List<Frame> LoadFramesAt(string path, string label)
        {
            if (frameCache.TryGetValue(path, out List<Frame> done)) return done;

            Texture2D[] textures = Resources.LoadAll<Texture2D>(path);
            List<Frame> frames = null;
            if (textures != null && textures.Length > 0)
            {
                System.Array.Sort(textures, (a, b) => FrameOrder(a.name).CompareTo(FrameOrder(b.name)));
                frames = new List<Frame>();
                foreach (Texture2D source in textures)
                {
                    if (!source.isReadable)
                    {
                        Debug.LogWarning($"[TSE Fatal] '{path}/{source.name}' não está legível; reimporte a pasta Art.");
                        continue;
                    }
                    int w = source.width, h = source.height;
                    Color32[] px = source.GetPixels32();
                    RemoveBackground(px, w, h);
                    List<Vector2Int> segments = FindColumnSegments(px, w, h);
                    if (segments.Count > 0)
                    {
                        RectInt r = ContentBounds(px, w, h, segments[0].x, segments[segments.Count - 1].y);
                        if (r.width > 0)
                        {
                            frames.Add(new Frame
                            {
                                Texture = Crop(px, w, r, $"{label}_{source.name}"),
                                PivotX = (FeetCenter(px, w, r) - r.x) / r.width,
                                Height = r.height,
                                SourceCenter = new Vector2((w * .5f - r.x) / r.width, (h * .5f - r.y) / r.height),
                            });
                        }
                    }
                    Resources.UnloadAsset(source);
                }
                if (frames.Count == 0) frames = null;
            }
            frameCache[path] = frames;
            return frames;
        }

        private static Texture2D Crop(Color32[] px, int w, RectInt r, string name)
        {
            var sub = new Color32[r.width * r.height];
            for (int y = 0; y < r.height; y++)
                System.Array.Copy(px, (r.y + y) * w + r.x, sub, y * r.width, r.width);
            var texture = new Texture2D(r.width, r.height, TextureFormat.RGBA32, true)
            {
                name = name, filterMode = FilterMode.Trilinear, wrapMode = TextureWrapMode.Clamp
            };
            texture.SetPixels32(sub);
            texture.Apply(true, true); // sem cópia em CPU: economiza memória
            return texture;
        }

        private static int FrameOrder(string name)
        {
            string digits = "";
            foreach (char c in name) if (char.IsDigit(c)) digits += c;
            return int.TryParse(digits, out int n) ? n : int.MaxValue;
        }

        // ---------------------------------------------------------------- folha única

        private static AnimClip Slice(Texture2D source, int expected, float worldHeight, bool fromSheet, bool pivotAtFeet = true)
        {
            if (!source.isReadable)
            {
                Debug.LogWarning($"[TSE Fatal] '{source.name}' não está legível; reimporte a pasta Art.");
                return null;
            }
            int w = source.width, h = source.height;
            Color32[] px = source.GetPixels32();
            Texture2D texture = source;
            if (RemoveBackground(px, w, h))
            {
                texture = new Texture2D(w, h, TextureFormat.RGBA32, true) { name = source.name + " (sem fundo)", filterMode = FilterMode.Trilinear, wrapMode = TextureWrapMode.Clamp };
                texture.SetPixels32(px);
                texture.Apply(true);
            }

            List<Vector2Int> segments = FindColumnSegments(px, w, h);
            if (!fromSheet || expected == 1)
            {
                if (segments.Count == 0) return null;
                segments = new List<Vector2Int> { new Vector2Int(segments[0].x, segments[segments.Count - 1].y) };
            }
            else if (expected > 1 && segments.Count != expected)
            {
                segments.Clear();
                for (int i = 0; i < expected; i++) segments.Add(new Vector2Int(i * w / expected, (i + 1) * w / expected - 1));
            }

            var rects = new List<RectInt>();
            int tallest = 1;
            foreach (Vector2Int seg in segments)
            {
                RectInt r = ContentBounds(px, w, h, seg.x, seg.y);
                if (r.width <= 0) continue;
                rects.Add(r);
                tallest = Mathf.Max(tallest, r.height);
            }
            if (rects.Count == 0) return null;

            float ppu = tallest / worldHeight;
            var frames = new Sprite[rects.Count];
            for (int i = 0; i < rects.Count; i++)
            {
                RectInt r = rects[i];
                Vector2 pivot = pivotAtFeet ? new Vector2((FeetCenter(px, w, r) - r.x) / r.width, 0f) : new Vector2(.5f, .5f);
                frames[i] = Sprite.Create(texture, new Rect(r.x, r.y, r.width, r.height), pivot, ppu, 0, SpriteMeshType.FullRect);
                frames[i].name = $"{source.name}_{i}";
            }
            return new AnimClip { Frames = frames, FromSheet = fromSheet };
        }

        // ---------------------------------------------------------------- fundo

        /// <summary>
        /// Fundo sólido (o ChatGPT costuma ignorar "transparente"): a cor do canto vira transparência.
        /// Fundo verde ganha chroma key de verdade: só as bordas encostadas no fundo perdem o tom verde,
        /// então objetos verdes no meio do desenho (dinheiro) continuam intactos.
        /// </summary>
        private static bool RemoveBackground(Color32[] px, int w, int h)
        {
            Color32 key = px[0];
            if (key.a <= 200) return false;
            bool green = key.g > 150 && key.g > key.r + 80 && key.g > key.b + 80;
            int hard = green ? 95 : 60, soft = green ? 190 : 100;

            var background = new bool[px.Length];
            for (int i = 0; i < px.Length; i++)
            {
                int d = Mathf.Abs(px[i].r - key.r) + Mathf.Abs(px[i].g - key.g) + Mathf.Abs(px[i].b - key.b);
                if (d < hard) { px[i].a = 0; background[i] = true; }
                else if (!green && d < soft) px[i].a = (byte)Mathf.Min(px[i].a, (d - hard) * 6);
            }
            if (MagentaKey(key)) { Despill(px, background); return true; }
            if (!green) return true;

            // Bordas (até 2 px do fundo): suaviza e remove o vazamento verde.
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int i = y * w + x;
                    if (background[i] || !NearBackground(background, w, h, x, y)) continue;
                    Color32 c = px[i];
                    int spill = c.g - Mathf.Max(c.r, c.b);
                    if (spill <= 8) continue;
                    int d = Mathf.Abs(c.r - key.r) + Mathf.Abs(c.g - key.g) + Mathf.Abs(c.b - key.b);
                    if (d < soft) c.a = (byte)Mathf.Clamp((d - hard) * 255 / (soft - hard), 0, 255);
                    c.g = (byte)Mathf.Max(c.r, c.b);
                    px[i] = c;
                }
            }
            return true;
        }

        private static bool MagentaKey(Color32 key) => key.r > 150 && key.b > 150 && key.g < 100;

        /// <summary>
        /// Fundo magenta (usado nos efeitos que têm verde, como dinheiro e fedor): tira o tom rosado
        /// que a imagem mistura nas bordas e nas partes semitransparentes (poeira, fumaça, linhas).
        /// Só mexe em pixels onde vermelho E azul passam do verde; vermelho puro (telhado) e azul
        /// puro (piscina) ficam intactos.
        /// </summary>
        private static void Despill(Color32[] px, bool[] background)
        {
            for (int i = 0; i < px.Length; i++)
            {
                if (background[i]) continue;
                Color32 c = px[i];
                int spill = Mathf.Min(c.r, c.b) - c.g;
                if (spill <= 12) continue;
                c.r = (byte)(c.r - spill);
                c.b = (byte)(c.b - spill);
                c.a = (byte)(c.a * Mathf.Clamp01(1f - spill / 200f));
                px[i] = c;
            }
        }

        private static bool NearBackground(bool[] background, int w, int h, int x, int y)
        {
            for (int dy = -2; dy <= 2; dy++)
            {
                int yy = y + dy;
                if (yy < 0 || yy >= h) continue;
                for (int dx = -2; dx <= 2; dx++)
                {
                    int xx = x + dx;
                    if (xx >= 0 && xx < w && background[yy * w + xx]) return true;
                }
            }
            return false;
        }

        // ---------------------------------------------------------------- recorte

        /// <summary>Faixas de colunas com conteúdo; ignora brilhos soltos e junta frestas pequenas.</summary>
        private static List<Vector2Int> FindColumnSegments(Color32[] px, int w, int h)
        {
            var counts = new int[w];
            for (int y = 0; y < h; y++)
            {
                int row = y * w;
                for (int x = 0; x < w; x++) if (px[row + x].a > AlphaCut) counts[x]++;
            }
            int minCount = Mathf.Max(1, h / 300);
            int minGap = Mathf.Max(2, w / 80);
            int minWidth = Mathf.Max(4, w / 40);

            var raw = new List<Vector2Int>();
            int start = -1, lastFilled = -1;
            for (int x = 0; x < w; x++)
            {
                if (counts[x] < minCount) continue;
                if (start < 0) start = x;
                else if (x - lastFilled > minGap) { raw.Add(new Vector2Int(start, lastFilled)); start = x; }
                lastFilled = x;
            }
            if (start >= 0) raw.Add(new Vector2Int(start, lastFilled));

            var result = new List<Vector2Int>();
            foreach (Vector2Int seg in raw)
                if (seg.y - seg.x + 1 >= minWidth) result.Add(seg);
            return result;
        }

        private static RectInt ContentBounds(Color32[] px, int w, int h, int x0, int x1)
        {
            int minX = int.MaxValue, maxX = -1, minY = int.MaxValue, maxY = -1;
            for (int y = 0; y < h; y++)
            {
                int row = y * w;
                for (int x = x0; x <= x1; x++)
                {
                    if (px[row + x].a <= AlphaCut) continue;
                    if (x < minX) minX = x; if (x > maxX) maxX = x;
                    if (y < minY) minY = y; if (y > maxY) maxY = y;
                }
            }
            return maxX < 0 ? new RectInt(0, 0, 0, 0) : new RectInt(minX, minY, maxX - minX + 1, maxY - minY + 1);
        }

        /// <summary>Centro horizontal dos pixels do quinto inferior (pernas/pés): base estável para o pivô.</summary>
        private static float FeetCenter(Color32[] px, int w, RectInt r)
        {
            int band = Mathf.Max(1, r.height / 5);
            double sum = 0; int count = 0;
            for (int y = r.y; y < r.y + band; y++)
            {
                int row = y * w;
                for (int x = r.x; x < r.xMax; x++)
                    if (px[row + x].a > AlphaCut) { sum += x; count++; }
            }
            return count == 0 ? r.x + r.width * .5f : (float)(sum / count);
        }
    }
}
