using System.Collections.Generic;
using UnityEngine;

namespace TSEFatal
{
    public class StagePlatform
    {
        public float Left, Right, Top, Bottom;
        public bool OneWay; // atravessável por baixo e com S (descer)
        public bool Contains(float x) => x >= Left && x <= Right;
    }

    /// <summary>
    /// Arena "Palácio do Planalto": fundo que cobre a tela, chão na esplanada, três lajes flutuantes
    /// e zonas de nocaute além das bordas. Mundo visível = 30 × 16,875 unidades (16:9).
    /// </summary>
    public class Arena : MonoBehaviour
    {
        public const float ViewWidth = 30f, ViewHeight = 16.875f;
        public const float GroundY = -5.9f;
        public const float BlastX = 19f, BlastTop = 13.5f, BlastBottom = -12f;

        public readonly List<StagePlatform> Platforms = new List<StagePlatform>();
        public StagePlatform Ground { get; private set; }
        private Camera cam;

        public static Arena Create(Transform parent, Camera camera)
        {
            var go = new GameObject("Arena Palácio do Planalto");
            go.transform.SetParent(parent, false);
            var arena = go.AddComponent<Arena>();
            arena.cam = camera;
            arena.Build();
            return arena;
        }

        private void Build()
        {
            Texture2D bg = Resources.Load<Texture2D>("Art/Stages/planalto");
            var bgGo = new GameObject("Fundo Planalto");
            bgGo.transform.SetParent(transform, false);
            var sr = bgGo.AddComponent<SpriteRenderer>();
            sr.sortingOrder = -100;
            if (bg != null)
            {
                // Escala para cobrir 30 × 16,875 (sem faixas), cortando o excesso se a proporção diferir.
                float ppu = Mathf.Min(bg.width / ViewWidth, bg.height / ViewHeight);
                sr.sprite = Sprite.Create(bg, new Rect(0, 0, bg.width, bg.height), new Vector2(.5f, .5f), ppu, 0, SpriteMeshType.FullRect);
            }
            else
            {
                sr.sprite = PixelArt.Pixel;
                sr.color = new Color(.25f, .45f, .85f);
                bgGo.transform.localScale = new Vector3(ViewWidth * PixelArt.PPU, ViewHeight * PixelArt.PPU, 1);
            }

            // Esplanada: chão sólido que termina um pouco antes da borda da tela (dá para cair).
            Ground = AddPlatform(-13.4f, 13.4f, GroundY, false, false);
            AddPlatform(-10.2f, -4.6f, -2.0f, true, true);
            AddPlatform(4.6f, 10.2f, -2.0f, true, true);
            AddPlatform(-2.8f, 2.8f, 1.9f, true, true);

            // Marcas nas pontas do chão para mostrar onde ele acaba.
            foreach (float x in new[] { Ground.Left, Ground.Right })
            {
                var edge = new GameObject("Borda do chão");
                edge.transform.SetParent(transform, false);
                edge.transform.position = new Vector3(x, GroundY - .05f, 0);
                var e = edge.AddComponent<SpriteRenderer>();
                e.sprite = PixelArt.Pixel; e.color = new Color(1f, .82f, .2f, .9f); e.sortingOrder = -9;
                edge.transform.localScale = new Vector3(.18f * PixelArt.PPU, .5f * PixelArt.PPU, 1);
            }
        }

        private StagePlatform AddPlatform(float left, float right, float top, bool oneWay, bool visible)
        {
            var p = new StagePlatform { Left = left, Right = right, Top = top, Bottom = oneWay ? top - .3f : top - 6f, OneWay = oneWay };
            Platforms.Add(p);
            if (visible)
            {
                var go = new GameObject("Laje flutuante");
                go.transform.SetParent(transform, false);
                go.transform.position = new Vector3((left + right) * .5f, top + .02f, 0);
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = PixelArt.Slab(right - left);
                sr.sortingOrder = -10;
                var shade = new GameObject("Sombra da laje");
                shade.transform.SetParent(go.transform, false);
                shade.transform.localPosition = new Vector3(0, -.75f, 0);
                var ss = shade.AddComponent<SpriteRenderer>();
                ss.sprite = PixelArt.Shadow; ss.color = new Color(1, 1, 1, .5f); ss.sortingOrder = -11;
                shade.transform.localScale = new Vector3((right - left) / 1.5f, 1f, 1);
            }
            return p;
        }

        /// <summary>Topo da superfície mais alta abaixo de (x, y), ou null.</summary>
        public StagePlatform SurfaceBelow(float x, float y, bool solidOnly = false)
        {
            StagePlatform best = null;
            foreach (StagePlatform p in Platforms)
                if ((!solidOnly || !p.OneWay) && p.Contains(x) && p.Top <= y + .05f && (best == null || p.Top > best.Top)) best = p;
            return best;
        }

        public bool OutOfBounds(Vector2 p) => p.x < -BlastX || p.x > BlastX || p.y < BlastBottom || p.y > BlastTop;

        private void LateUpdate()
        {
            if (cam == null) return;
            // Enquadra a arena para preencher qualquer proporção de tela sem mostrar fora do fundo.
            float aspect = Mathf.Max(.5f, cam.aspect);
            cam.orthographicSize = Mathf.Min(ViewHeight * .5f, ViewWidth * .5f / aspect);
            Vector2 shake = Combat.ShakeAmount > 0f ? Random.insideUnitCircle * Combat.ShakeAmount : Vector2.zero;
            cam.transform.position = new Vector3(shake.x, shake.y, -10f);
        }
    }
}
