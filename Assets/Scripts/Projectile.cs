using UnityEngine;

namespace TSEFatal
{
    /// <summary>Projétil genérico: voa, gira, corre pelo chão ou cai do céu, e acerta o rival uma vez.</summary>
    public class Projectile : MonoBehaviour
    {
        public FighterController Owner;
        public HitData Hit;
        public Vector2 Velocity;
        public float Gravity, Spin, Life = 1.5f;
        public Vector2 HitSize = new Vector2(.8f, .6f);
        public Vector2 HitOffset;
        public bool RunsOnGround;     // onça: anda colada na superfície
        public bool LandsAndBreaks;   // mansão: some com poeira ao tocar o chão
        public bool CrashesThrough;   // mansão: atravessa as plataformas finas e só para no chão firme
        public Sprite[] Frames;
        public float FrameRate = 10f;
        public float Bounce;          // bombom: >0 quica no chão (fração da velocidade que sobra)
        public System.Action<FighterController> OnHit;
        public System.Action<Vector2> OnLand; // quique ou queda (som, poeira)
        public Color SparkColor = Color.white;

        private SpriteRenderer sr;
        private bool spent;
        private float age;
        private Arena arena;

        public static Projectile Spawn(FighterController owner, Arena arena, Sprite sprite, Vector2 position, float height, HitData hit)
        {
            var go = new GameObject("Projétil " + (sprite != null ? sprite.name : ""));
            if (Effects.Root != null) go.transform.SetParent(Effects.Root, false);
            go.transform.position = position;
            var p = go.AddComponent<Projectile>();
            p.sr = go.AddComponent<SpriteRenderer>();
            p.sr.sprite = sprite;
            p.sr.sortingOrder = 6;
            go.transform.localScale = Vector3.one * Effects.ScaleToHeight(sprite, height);
            p.Owner = owner; p.Hit = hit; p.arena = arena;
            return p;
        }

        /// <summary>Cria o projétil já animado; a altura vale para o frame mais alto.</summary>
        public static Projectile Spawn(FighterController owner, Arena arena, Sprite[] frames, Vector2 position, float height, HitData hit, float fps = 12f)
        {
            Projectile p = Spawn(owner, arena, frames[0], position, height, hit);
            p.transform.localScale = Vector3.one * Effects.ScaleToHeight(frames, height);
            p.Frames = frames; p.FrameRate = fps;
            return p;
        }

        /// <summary>True quando há frames desenhados: aí o giro já está no desenho e o código não deve girar o sprite.</summary>
        public bool Animated => Frames != null && Frames.Length > 1;

        public void Face(int direction)
        {
            Vector3 s = transform.localScale;
            transform.localScale = new Vector3(Mathf.Abs(s.x) * (direction >= 0 ? 1 : -1), s.y, s.z);
        }

        private void Update()
        {
            if (Combat.Frozen) return;
            float dt = Time.deltaTime;
            age += dt;
            if (age >= Life || Owner == null) { Destroy(gameObject); return; }

            Velocity.y -= Gravity * dt;
            Vector2 pos = (Vector2)transform.position + Velocity * dt;

            if (RunsOnGround && arena != null)
            {
                StagePlatform floor = arena.SurfaceBelow(pos.x, transform.position.y + .05f);
                if (floor != null && pos.y <= floor.Top + .02f) { pos.y = floor.Top; Velocity.y = 0; Gravity = 0f; }
                else Gravity = 30f; // no ar ou caiu da borda
            }
            if (LandsAndBreaks && arena != null && Velocity.y < 0)
            {
                StagePlatform floor = arena.SurfaceBelow(pos.x, transform.position.y, CrashesThrough);
                if (floor != null && pos.y <= floor.Top)
                {
                    pos.y = floor.Top;
                    transform.position = pos;
                    TryHit();
                    Effects.Dust(pos, 10);
                    Combat.Shake(.35f, .25f);
                    OnLand?.Invoke(pos);
                    Destroy(gameObject, .05f);
                    LandsAndBreaks = false;
                    Velocity = Vector2.zero;
                    return;
                }
            }

            if (Bounce > 0f && arena != null && Velocity.y < 0)
            {
                float radius = HitSize.y * .5f;
                StagePlatform floor = arena.SurfaceBelow(pos.x, transform.position.y - radius + .05f);
                if (floor != null && pos.y - radius <= floor.Top)
                {
                    pos.y = floor.Top + radius;
                    Velocity.y = -Velocity.y * Bounce;
                    Velocity.x *= .9f;
                    OnLand?.Invoke(pos);
                }
            }

            transform.position = pos;
            transform.Rotate(0, 0, Spin * dt);
            if (Frames != null && Frames.Length > 1) sr.sprite = Frames[Mathf.FloorToInt(age * FrameRate) % Frames.Length];
            if (arena != null && arena.OutOfBounds(pos)) { Destroy(gameObject); return; }
            TryHit();
        }

        private void TryHit()
        {
            if (spent || Owner == null || Owner.Target == null) return;
            FighterController target = Owner.Target;
            var box = new Rect((Vector2)transform.position + HitOffset - HitSize * .5f, HitSize);
            if (!box.Overlaps(target.Hurtbox)) return;
            int dir = Mathf.Abs(Velocity.x) > .5f ? (int)Mathf.Sign(Velocity.x) : (target.transform.position.x >= transform.position.x ? 1 : -1);
            if (!target.TakeHit(Hit, dir, box.center)) return;
            spent = true;
            Effects.Spark(box.center, SparkColor, 1.2f);
            OnHit?.Invoke(target);
            if (!RunsOnGround) Destroy(gameObject);
        }
    }
}
