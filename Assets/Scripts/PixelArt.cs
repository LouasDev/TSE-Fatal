using System.Collections.Generic;
using UnityEngine;

namespace TSEFatal
{
    /// <summary>
    /// Sprites de efeitos desenhados em código (placeholder). Se existir Resources/Art/VFX/{nome}.png,
    /// a arte importada é usada no lugar (ver Docs/ANIMATION_PROMPTS.md).
    /// </summary>
    public static class PixelArt
    {
        public const float PPU = 16f;
        private static readonly Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();
        private static readonly Color32 Clear = new Color32(0, 0, 0, 0);
        private static readonly Color32 Ink = new Color32(24, 14, 20, 255);

        /// <summary>Usa Art/VFX/{name}.png se existir; senão o desenho procedural.</summary>
        private static Sprite Vfx(string name, float worldHeight, System.Func<Sprite> fallback, bool pivotAtBottom = false)
        {
            if (cache.TryGetValue(name, out Sprite s)) return s;
            AnimClip art = SpriteSheetLibrary.Single("Art/VFX/" + name, worldHeight, pivotAtBottom);
            s = art != null ? art.Frames[0] : fallback();
            cache[name] = s;
            return s;
        }

        // ---------------------------------------------------------------- efeitos animados
        // Cada efeito procura frames em Resources/Art/VFX/{pasta}/01.png, 02.png...
        // Sem a pasta, cai na imagem única ou no desenho em código (o jogo nunca fica sem efeito).
        // Os prompts para gerar cada frame estão em PromptsAnimacao/vfx/.

        private static Sprite[] AnimFrames(string folder, float worldHeight, bool pivotAtBottom, System.Func<Sprite[]> fallback)
        {
            AnimClip clip = SpriteSheetLibrary.EffectClip(folder, worldHeight, pivotAtBottom);
            return clip != null ? clip.Frames : fallback();
        }

        public static Sprite[] CashSpin => AnimFrames("mensalao_maco", .62f, false, () => new[] { Cash });
        public static Sprite[] BillFlutter => AnimFrames("nota", .4f, false, () => new[] { Bill });
        public static Sprite[] CoinSpin => AnimFrames("moeda", .4f, false, () => new[] { Coin });
        public static Sprite[] LetterLBlast => AnimFrames("faz_o_l", 3.2f, false, () => new[] { LetterL });
        public static Sprite[] CensorFly => AnimFrames("censura_tarja", .7f, false, () => new[] { CensorBar });
        public static Sprite[] ViralCut => AnimFrames("corte_viral", 1.6f, false, () => new[] { Slash });
        public static Sprite[] JaguarRun => AnimFrames("onca", 1.3f, true, () => new[] { Jaguar(0), Jaguar(1) });
        /// <summary>Cabeça de onça rugindo no acerto. Sem frames, não aparece (null).</summary>
        public static Sprite[] JaguarRoar => AnimFrames("onca_rugido", 1.8f, false, () => null);
        public static Sprite[] DiaperSpin => AnimFrames("fralda", .8f, false, () => new[] { Diaper });
        /// <summary>Nuvem de fedor no acerto da fralda. Sem frames, o jogo usa fumacinhas verdes.</summary>
        public static Sprite[] Stink => AnimFrames("fedor", 1.8f, false, () => null);
        public static Sprite[] MansionFall => AnimFrames("mansao_queda", 3f, true, () => new[] { Mansion });
        /// <summary>Mansão desabando no chão. Sem frames, só poeira.</summary>
        public static Sprite[] MansionImpact => AnimFrames("mansao_impacto", 3.6f, true, () => null);
        public static Sprite[] BonbonSpin => AnimFrames("bombom", .5f, false, () => new[] { Bonbon });
        public static Sprite[] PicanhaSizzle => AnimFrames("picanha", .75f, false, () => new[] { Picanha });
        public static Sprite[] DroneFly => AnimFrames("drone_sirene", .9f, false, () => new[] { Drone });
        public static Sprite[] RobotWalk => AnimFrames("robo_ia", 1.1f, true, () => new[] { Robot });
        public static Sprite[] SleepZzz => AnimFrames("zzz_sono", 1.1f, false, () => new[] { Zzz });

        public static Sprite Drone => Vfx("drone_sirene", .9f, () => FromRows("drone", new[]
        {
            "      KRCK      ",
            "KKKKKKKKKKKKKKKK",
            "   K KwwwwK K   ",
            "   KKwwwwwwKK   ",
            "     KwwwwK     ",
            "      K  K      ",
        }));

        public static Sprite Robot => Vfx("robo_ia", 1.1f, () => FromRows("robot", new[]
        {
            "    KR    ",
            "    KK    ",
            "KKKKKKKKKK",
            "KwwKCCCCKK",
            "KwwKCKCKKb",
            "KwwKCCCCKb",
            "KKKKKKKKKK",
            " KwwwwwwK ",
            " KK    KK ",
        }), true);

        public static Sprite Zzz => Vfx("zzz_sono", 1.1f, () => FromRows("zzz", new[]
        {
            "    KKKKK",
            "    KWWWK",
            "     KWK ",
            "KKKKKWWWK",
            "KWWWKKKKK",
            " KWK     ",
            "KWWWK    ",
            "KKKKK    ",
        }));

        public static Sprite Cash => Vfx("mensalao_maco", .62f, () => FromRows("cash", new[]
        {
            " KKKKKKKKKKKK ",
            "KGGGGGYYGGGGGK",
            "KGgggGYYGgggGK",
            "KGgGgGYYGgGgGK",
            "KGgggGYYGgggGK",
            "KGGGGGYYGGGGGK",
            "KDDDDDYYDDDDDK",
            "KDDDDDDDDDDDDK",
            " KKKKKKKKKKKK ",
        }));

        public static Sprite Bill => Vfx("nota", .4f, () => FromRows("bill", new[]
        {
            "KKKKKKKK",
            "KGGgGGGK",
            "KGgYgGGK",
            "KGGgGGGK",
            "KKKKKKKK",
        }));

        public static Sprite Coin => Vfx("moeda", .4f, () => FromRows("coin", new[]
        {
            " KKKK ",
            "KOOOOK",
            "KOoOOK",
            "KOoOOK",
            "KOOOOK",
            " KKKK ",
        }));

        public static Sprite Bonbon => Vfx("bombom", .5f, () => FromRows("bonbon", new[]
        {
            "OO  KKKK  OO",
            "OOKKBBBBKKOO",
            " OKBbbBBBKO ",
            "  KBbBBBBK  ",
            " OKBBBBBBKO ",
            "OOKKBBBBKKOO",
            "OO  KKKK  OO",
        }));

        public static Sprite Diaper => Vfx("fralda", .8f, () => FromRows("diaper", new[]
        {
            "KKKKKKKKKKKKKK",
            "KCWWWWWWWWWWCK",
            "KCWWWWWWWWWWCK",
            " KWWwwwwwwWWK ",
            " KWWwSSSSwWWK ",
            "  KWwSSSSwWK  ",
            "   KWwwwwWK   ",
            "    KKKKKK    ",
        }));

        public static Sprite Picanha => Vfx("picanha", .75f, () => FromRows("picanha", new[]
        {
            "    KKKKKKKK  ",
            "  KKmmmmmmmmK ",
            " KMMMMMMMMMMmK",
            "KMMrMMMMrMMMMK",
            "KMMMMMrMMMMMMK",
            " KMMMMMMMMMMK ",
            "  KKKKKKKKKK  ",
        }));

        public static Sprite LetterL => Vfx("faz_o_l", 3.2f, () => FromRows("L", new[]
        {
            "KKKKK       ",
            "KrRRK       ",
            "KrRRK       ",
            "KrRRK       ",
            "KrRRK       ",
            "KrRRK       ",
            "KrRRK       ",
            "KrRRK       ",
            "KrRRK       ",
            "KrRRKKKKKKKK",
            "KrRRRRRRRRRK",
            "KrRRRRRRRRRK",
            "KDDDDDDDDDDK",
            "KKKKKKKKKKKK",
        }));

        /// <summary>Tarja preta "CENSURADO" da Censura do Lula.</summary>
        public static Sprite CensorBar => Vfx("censura_tarja", .9f, () =>
        {
            const int w = 40, h = 11;
            var p = Canvas(w, h);
            Rect(p, w, 0, 0, w, h, new Color32(12, 12, 14, 255));
            Rect(p, w, 0, h - 1, w, 1, new Color32(60, 60, 66, 255));
            // Letras mínimas 3x5 em vermelho: "CENSURADO"
            string text = "CENSURADO";
            int x = 2;
            foreach (char c in text) { Glyph(p, w, x, 3, c, new Color32(230, 40, 40, 255)); x += 4; }
            return Make("censor", p, w, h, new Vector2(.5f, .5f));
        });

        public static Sprite Mansion => Vfx("mansao", 3.0f, () =>
        {
            const int w = 38, h = 28;
            var p = Canvas(w, h);
            Color32 wall = new Color32(238, 232, 220, 255), wallShade = new Color32(205, 196, 182, 255);
            Color32 roof = new Color32(176, 46, 40, 255), roofLight = new Color32(214, 76, 58, 255);
            Color32 glass = new Color32(90, 150, 220, 255), door = new Color32(110, 70, 40, 255), gold = new Color32(255, 200, 40, 255);
            Rect(p, w, 2, 0, 34, 2, new Color32(150, 146, 140, 255));
            Rect(p, w, 4, 2, 30, 14, wall);
            Rect(p, w, 4, 2, 30, 2, wallShade);
            for (int i = 0; i < 4; i++) Rect(p, w, 7 + i * 7, 2, 2, 14, wallShade); // colunas
            for (int i = 0; i < 3; i++) { Rect(p, w, 10 + i * 7, 9, 3, 4, glass); Rect(p, w, 10 + i * 7, 4, 3, 3, glass); }
            Rect(p, w, 17, 2, 4, 6, door);
            for (int y = 0; y < 11; y++)
            {
                int half = 18 - y * 18 / 11;
                Rect(p, w, 19 - half, 16 + y, half * 2, 1, y % 3 == 0 ? roofLight : roof);
            }
            // placa "$6M"
            Rect(p, w, 15, 19, 8, 5, gold);
            Glyph(p, w, 16, 19, '6', Ink);
            Glyph(p, w, 19, 19, 'M', Ink);
            return Make("mansion", p, w, h, new Vector2(.5f, 0f));
        }, true);

        public static Sprite Jaguar(int frame) => Vfx("onca_" + frame, 1.1f, () =>
        {
            const int w = 28, h = 14;
            var p = Canvas(w, h);
            Color32 fur = new Color32(232, 152, 44, 255), belly = new Color32(250, 214, 150, 255), spot = new Color32(60, 32, 16, 255);
            Ellipse(p, w, 13, 7, 9, 3.4f, fur);
            Rect(p, w, 7, 4, 12, 2, belly);
            Ellipse(p, w, 23, 8.5f, 3.6f, 3.1f, fur);
            Rect(p, w, 24, 11, 1, 2, fur); Rect(p, w, 21, 11, 1, 2, fur);
            p[8 * w + 24] = Ink; p[7 * w + 26] = spot; p[6 * w + 25] = belly;
            int[] spots = { 8, 9, 11, 7, 14, 9, 16, 6, 18, 8, 10, 6, 6, 8, 20, 9 };
            for (int i = 0; i < spots.Length; i += 2) p[spots[i + 1] * w + spots[i]] = spot;
            int lift = frame == 0 ? 0 : 1;
            int[] legs = frame == 0 ? new[] { 6, 10, 16, 20 } : new[] { 8, 12, 14, 18 };
            for (int i = 0; i < legs.Length; i++) Rect(p, w, legs[i], (i % 2 == 0 ? lift : 1 - lift), 2, 4, fur);
            for (int i = 0; i < 5; i++) p[(8 + i / 2) * w + (4 - i)] = fur; // rabo
            return Make("jaguar" + frame, p, w, h, new Vector2(.5f, 0f));
        }, true);

        public static Sprite Spark => FromCode("spark", 11, 11, (p, w) =>
        {
            Color32 core = new Color32(255, 255, 255, 255), glow = new Color32(255, 220, 90, 255);
            for (int i = 0; i < 11; i++) { p[5 * w + i] = glow; p[i * w + 5] = glow; }
            for (int i = 2; i < 9; i++) { p[i * w + i] = glow; p[i * w + (10 - i)] = glow; }
            Rect(p, w, 4, 4, 3, 3, core);
        }, false);

        public static Sprite Slash => FromCode("slash", 40, 12, (p, w) =>
        {
            for (int x = 0; x < 40; x++)
            {
                float t = x / 39f;
                int y = Mathf.RoundToInt(2 + Mathf.Sin(t * Mathf.PI) * 7);
                int thick = Mathf.RoundToInt(1 + Mathf.Sin(t * Mathf.PI) * 2.5f);
                for (int k = 0; k < thick; k++) if (y - k >= 0) p[(y - k) * w + x] = new Color32(255, 255, 255, (byte)(140 + 115 * t));
            }
        }, false, new Vector2(.5f, .5f));

        public static Sprite Ring => FromCode("ring", 24, 24, (p, w) =>
        {
            for (int y = 0; y < 24; y++)
                for (int x = 0; x < 24; x++)
                {
                    float d = Vector2.Distance(new Vector2(x + .5f, y + .5f), new Vector2(12, 12));
                    if (d > 9.5f && d < 12f) p[y * w + x] = new Color32(255, 255, 255, 255);
                }
        }, false, new Vector2(.5f, .5f));

        public static Sprite Shadow => FromCode("shadow", 24, 6, (p, w) => Ellipse(p, w, 12, 3, 12, 3, new Color32(0, 0, 0, 110)), false, new Vector2(.5f, .5f));

        public static Sprite Dust => FromCode("dust", 7, 7, (p, w) => Ellipse(p, w, 3.5f, 3.5f, 3.5f, 3.5f, new Color32(235, 225, 205, 220)), false, new Vector2(.5f, .5f));

        public static Sprite Pixel => FromCode("pixel", 1, 1, (p, w) => p[0] = new Color32(255, 255, 255, 255), false, new Vector2(.5f, .5f));

        /// <summary>Laje de concreto branco no estilo do Planalto, usada nas plataformas flutuantes.</summary>
        public static Sprite Slab(float worldWidth)
        {
            int w = Mathf.Max(8, Mathf.RoundToInt(worldWidth * PPU));
            string key = "slab" + w;
            if (cache.TryGetValue(key, out Sprite s)) return s;
            const int h = 9;
            var p = Canvas(w, h);
            Rect(p, w, 0, 0, w, h, new Color32(222, 214, 200, 255));
            Rect(p, w, 0, h - 3, w, 2, new Color32(248, 244, 234, 255));
            Rect(p, w, 0, 0, w, 3, new Color32(168, 158, 146, 255));
            for (int x = 12; x < w - 4; x += 16) Rect(p, w, x, 3, 1, h - 6, new Color32(190, 180, 166, 255));
            // pilares curvos estilizados nas pontas
            Rect(p, w, 2, 0, 2, 3, new Color32(140, 130, 120, 255));
            Rect(p, w, w - 4, 0, 2, 3, new Color32(140, 130, 120, 255));
            s = Make(key, Outline(p, w, h), w, h, new Vector2(.5f, 1f));
            cache[key] = s;
            return s;
        }

        // ---------- utilitários ----------

        private static readonly Dictionary<char, Color32> Palette = new Dictionary<char, Color32>
        {
            { 'K', Ink }, { ' ', Clear },
            { 'G', new Color32(120, 200, 110, 255) }, { 'g', new Color32(70, 150, 70, 255) }, { 'D', new Color32(40, 100, 50, 255) },
            { 'Y', new Color32(240, 220, 150, 255) },
            { 'O', new Color32(255, 196, 40, 255) }, { 'o', new Color32(255, 240, 160, 255) },
            { 'R', new Color32(220, 30, 40, 255) }, { 'r', new Color32(255, 110, 100, 255) },
            { 'B', new Color32(92, 52, 30, 255) }, { 'b', new Color32(150, 92, 56, 255) },
            { 'W', new Color32(246, 246, 240, 255) }, { 'w', new Color32(205, 205, 215, 255) },
            { 'C', new Color32(110, 200, 240, 255) }, { 'S', new Color32(150, 120, 40, 255) },
            { 'M', new Color32(170, 50, 50, 255) }, { 'm', new Color32(250, 225, 190, 255) },
        };

        private static Sprite FromRows(string key, string[] rows)
        {
            int h = rows.Length, w = rows[0].Length;
            var p = Canvas(w, h);
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    char c = x < rows[y].Length ? rows[y][x] : ' ';
                    p[(h - 1 - y) * w + x] = Palette.TryGetValue(c, out Color32 col) ? col : Clear;
                }
            return Make(key, p, w, h, new Vector2(.5f, .5f));
        }

        private static Sprite FromCode(string key, int w, int h, System.Action<Color32[], int> draw, bool outline, Vector2? pivot = null)
        {
            if (cache.TryGetValue(key, out Sprite s)) return s;
            var p = Canvas(w, h);
            draw(p, w);
            s = Make(key, outline ? Outline(p, w, h) : p, w, h, pivot ?? new Vector2(.5f, .5f));
            cache[key] = s;
            return s;
        }

        private static Color32[] Canvas(int w, int h)
        {
            var p = new Color32[w * h];
            for (int i = 0; i < p.Length; i++) p[i] = Clear;
            return p;
        }

        private static Sprite Make(string key, Color32[] p, int w, int h, Vector2 pivot)
        {
            var t = new Texture2D(w, h, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp, name = key };
            t.SetPixels32(p);
            t.Apply();
            return Sprite.Create(t, new Rect(0, 0, w, h), pivot, PPU, 0, SpriteMeshType.FullRect);
        }

        private static void Rect(Color32[] p, int w, int x0, int y0, int rw, int rh, Color32 c)
        {
            int h = p.Length / w;
            for (int y = Mathf.Max(0, y0); y < Mathf.Min(h, y0 + rh); y++)
                for (int x = Mathf.Max(0, x0); x < Mathf.Min(w, x0 + rw); x++) p[y * w + x] = c;
        }

        private static void Ellipse(Color32[] p, int w, float cx, float cy, float rx, float ry, Color32 c)
        {
            int h = p.Length / w;
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    float dx = (x + .5f - cx) / rx, dy = (y + .5f - cy) / ry;
                    if (dx * dx + dy * dy <= 1f) p[y * w + x] = c;
                }
        }

        /// <summary>Contorno escuro de 1 px em volta de tudo que é opaco: deixa os efeitos legíveis sobre o fundo.</summary>
        private static Color32[] Outline(Color32[] p, int w, int h)
        {
            var o = (Color32[])p.Clone();
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    if (p[y * w + x].a > 0) continue;
                    bool edge = (x > 0 && p[y * w + x - 1].a > 128) || (x < w - 1 && p[y * w + x + 1].a > 128)
                             || (y > 0 && p[(y - 1) * w + x].a > 128) || (y < h - 1 && p[(y + 1) * w + x].a > 128);
                    if (edge) o[y * w + x] = Ink;
                }
            return o;
        }

        // Fonte 3x5 mínima para placas e tarjas.
        private static readonly Dictionary<char, string> Font3x5 = new Dictionary<char, string>
        {
            { 'C', "111100100100111" }, { 'E', "111100110100111" }, { 'N', "101111111111101" },
            { 'S', "111100111001111" }, { 'U', "101101101101111" }, { 'R', "110101110101101" },
            { 'A', "010101111101101" }, { 'D', "110101101101110" }, { 'O', "111101101101111" },
            { 'M', "101111111101101" }, { '6', "111100111101111" },
        };

        private static void Glyph(Color32[] p, int w, int x0, int y0, char c, Color32 color)
        {
            if (!Font3x5.TryGetValue(c, out string bits)) return;
            for (int row = 0; row < 5; row++)
                for (int col = 0; col < 3; col++)
                    if (bits[row * 3 + col] == '1')
                    {
                        int x = x0 + col, y = y0 + 4 - row;
                        if (x >= 0 && x < w && y >= 0 && y * w + x < p.Length) p[y * w + x] = color;
                    }
        }
    }
}
