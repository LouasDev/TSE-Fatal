using UnityEngine;

namespace TSEFatal
{
    /// <summary>
    /// "Picanha e cervejinha": cai do céu numa plataforma e cura 15% de quem pegar primeiro.
    /// Some sozinha depois de alguns segundos (piscando no fim).
    /// </summary>
    public class Pickup : MonoBehaviour
    {
        private const float Heal = 15f, Lifetime = 9f;

        private FighterController[] fighters;
        private MenuAudio sfx;
        private Arena arena;
        private SpriteRenderer sr;
        private Sprite[] frames;
        private float vy, age, landedAt = -1f;
        private bool landed;

        public static Pickup Spawn(Arena arena, MenuAudio sfx, FighterController[] fighters)
        {
            // Escolhe uma plataforma ao acaso e um ponto dentro dela.
            StagePlatform p = arena.Platforms[Random.Range(0, arena.Platforms.Count)];
            float x = Random.Range(p.Left + 1f, p.Right - 1f);
            if (p == arena.Ground) x = Random.Range(-9f, 9f);

            var go = new GameObject("Picanha");
            if (Effects.Root != null) go.transform.SetParent(Effects.Root, false);
            go.transform.position = new Vector3(x, 9.5f, 0f);
            var pickup = go.AddComponent<Pickup>();
            pickup.sr = go.AddComponent<SpriteRenderer>();
            pickup.frames = PixelArt.PicanhaSizzle;
            pickup.sr.sprite = pickup.frames[0];
            pickup.sr.sortingOrder = 5;
            go.transform.localScale = Vector3.one * Effects.ScaleToHeight(pickup.frames, .75f);
            pickup.arena = arena; pickup.sfx = sfx; pickup.fighters = fighters;
            Effects.Callout("PICANHA CAINDO!", new Vector2(x, 6.5f), new Color(1f, .55f, .45f), .8f, 1.8f);
            return pickup;
        }

        private void Update()
        {
            if (Combat.Frozen) return;
            float dt = Time.deltaTime;
            age += dt;
            Vector2 pos = transform.position;

            if (!landed)
            {
                vy = Mathf.Max(vy - 20f * dt, -12f);
                pos.y += vy * dt;
                StagePlatform floor = arena.SurfaceBelow(pos.x, transform.position.y + .05f);
                if (floor != null && pos.y <= floor.Top + .3f)
                {
                    pos.y = floor.Top + .3f;
                    landed = true; landedAt = age;
                    Effects.Dust(new Vector2(pos.x, floor.Top), 4);
                    sfx.Plop();
                }
                if (pos.y < Arena.BlastBottom) { Destroy(gameObject); return; }
            }
            else
            {
                // Flutua de leve e chia na brasa.
                float t = age - landedAt;
                pos.y += Mathf.Sin(age * 5f) * .003f;
                if (t > Lifetime - 2.5f) sr.enabled = Mathf.Repeat(age * 8f, 1f) > .35f;
                if (t > Lifetime) { Destroy(gameObject); return; }
                if (Random.value < dt * 6f)
                {
                    var steam = Effects.Sprite(PixelArt.Dust, pos + new Vector2(Random.Range(-.3f, .3f), .2f), .2f, 6);
                    steam.Velocity = new Vector2(0f, 1.2f); steam.Life = .5f; steam.EndScale = 1.8f;
                    steam.Renderer.color = new Color(1f, 1f, 1f, .5f);
                }
            }
            transform.position = pos;
            if (frames.Length > 1) sr.sprite = frames[Mathf.FloorToInt(age * 6f) % frames.Length];

            var box = new Rect(pos - new Vector2(.45f, .35f), new Vector2(.9f, .7f));
            foreach (FighterController f in fighters)
            {
                if (f == null || f.OutOfPlay || !box.Overlaps(f.Hurtbox)) continue;
                f.Heal(Heal, "PICANHA! -15%", new Color(1f, .6f, .5f));
                sfx.Pickup();
                Effects.Burst(PixelArt.CoinSpin, pos, 5, 6f, .3f);
                Destroy(gameObject);
                return;
            }
        }
    }
}
