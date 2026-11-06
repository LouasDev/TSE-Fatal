using UnityEngine;

namespace TSEFatal
{
    public enum FighterAnim { Idle, Walk, Jump, Fall, Attack, AirAttack, Special1, Special2, Special3, Hurt, Dodge, KO }

    /// <summary>
    /// Toca as folhas de Resources/Art/Characters/{pasta}/{estado}.png quando existem.
    /// Sem folha, anima a imagem estática de forma procedural (respiração, inclinação, avanço, squash).
    /// </summary>
    public class FighterAnimator : MonoBehaviour
    {
        public const float Height = 3.6f;

        private static readonly string[] Names = { "idle", "walk", "jump", "fall", "attack", "air_attack", "special1", "special2", "special3", "hurt", "dodge", "ko" };
        private static readonly float[] HeightFactor = { 1f, 1f, .95f, .95f, 1f, 1f, 1f, 1f, 1f, .95f, .85f, .6f };
        private static readonly float[] Fps = { 6f, 12f, 14f, 7f, 14f, 14f, 12f, 12f, 12f, 10f, 14f, 8f };
        private static readonly bool[] Loops = { true, true, false, true, false, false, false, false, false, false, false, false };
        // Se não houver folha para o estado, tenta este outro (-1 = imagem estática).
        private static readonly int[] Fallback = { -1, 0, 3, 2, -1, 4, 4, 6, 6, -1, 2, 9 };

        private AnimClip[] clips;
        private AnimClip staticClip;
        private SpriteRenderer body, flash;
        private Transform visual;
        private FighterAnim state = FighterAnim.Idle;
        private AnimClip current;
        private float stateTime, oneShot;
        private int facing = 1;
        private Vector2 squash = Vector2.one;
        private float flashTimer, flashLength;
        private Color flashColor = Color.white;
        private Color tint = Color.white;

        public SpriteRenderer Renderer => body;
        public FighterAnim State => state;

        public void Init(CandidateId candidate, int sortingOrder)
        {
            visual = new GameObject("Visual").transform;
            visual.SetParent(transform, false);
            body = visual.gameObject.AddComponent<SpriteRenderer>();
            body.sortingOrder = sortingOrder;
            var flashGo = new GameObject("Flash");
            flashGo.transform.SetParent(visual, false);
            flash = flashGo.AddComponent<SpriteRenderer>();
            flash.sharedMaterial = Effects.Silhouette;
            flash.sortingOrder = sortingOrder + 1;
            flash.color = Color.clear;

            string folder = CharacterProfile.AssetFolder(candidate);
            clips = new AnimClip[Names.Length];
            for (int i = 0; i < Names.Length; i++)
                clips[i] = SpriteSheetLibrary.CharacterClip(folder, Names[i], Height * HeightFactor[i], Height);
            staticClip = SpriteSheetLibrary.Single(CharacterProfile.StaticSprite(candidate), Height)
                         ?? new AnimClip { Frames = new[] { PixelArt.Pixel } };
            current = Resolve(FighterAnim.Idle);
        }

        /// <summary>Troca de estado. duration > 0 ajusta a velocidade de um golpe para caber no tempo dele.</summary>
        public void Play(FighterAnim next, float duration = 0f, bool restart = false)
        {
            if (next == state && !restart) return;
            state = next;
            stateTime = 0f;
            oneShot = duration;
            current = Resolve(next);
        }

        public void SetFacing(int value) => facing = value >= 0 ? 1 : -1;
        public void Squash(Vector2 amount) => squash = amount;
        public void SetTint(Color value) => tint = value;
        public void Flash(Color color, float seconds) { flashColor = color; flashTimer = flashLength = seconds; }
        public void SetVisible(bool value) => visual.gameObject.SetActive(value);

        private AnimClip Resolve(FighterAnim s)
        {
            int i = (int)s;
            for (int guard = 0; i >= 0 && guard < 6; guard++)
            {
                if (clips[i] != null) return clips[i];
                i = Fallback[i];
            }
            return staticClip;
        }

        private void LateUpdate()
        {
            float dt = Combat.Frozen ? 0f : Time.deltaTime;
            stateTime += dt;
            int s = (int)state;

            Sprite[] frames = current.Frames;
            int index = 0;
            if (frames.Length > 1)
            {
                float fps = oneShot > 0f && !Loops[s] ? frames.Length / oneShot : Fps[s];
                int raw = Mathf.FloorToInt(stateTime * fps);
                index = Loops[s] ? raw % frames.Length : Mathf.Min(frames.Length - 1, raw);
            }
            body.sprite = frames[index];
            flash.sprite = body.sprite;

            Vector3 offset = Vector3.zero;
            float rotation = 0f;
            Vector2 scale = Vector2.one;
            if (!current.FromSheet) Procedural(ref offset, ref rotation, ref scale);

            squash = Vector2.Lerp(squash, Vector2.one, (Combat.Frozen ? 0f : Time.deltaTime) * 12f);
            visual.localPosition = offset;
            visual.localRotation = Quaternion.Euler(0, 0, rotation);
            visual.localScale = new Vector3(scale.x * squash.x * facing, scale.y * squash.y, 1f);

            body.color = tint;
            if (flashTimer > 0f)
            {
                flashTimer -= Time.deltaTime;
                flash.color = new Color(flashColor.r, flashColor.g, flashColor.b, Mathf.Clamp01(flashTimer / flashLength) * .85f);
            }
            else flash.color = Color.clear;
        }

        /// <summary>Animação "de papelão" sobre a imagem estática, até chegarem os quadros reais.</summary>
        private void Procedural(ref Vector3 offset, ref float rotation, ref Vector2 scale)
        {
            float t = stateTime;
            float forward = -facing; // rotação negativa inclina para a direita
            switch (state)
            {
                case FighterAnim.Idle:
                    scale = new Vector2(1f - .012f * Mathf.Sin(t * 3.2f), 1f + .025f * Mathf.Sin(t * 3.2f));
                    break;
                case FighterAnim.Walk:
                    offset.y = Mathf.Abs(Mathf.Sin(t * 11f)) * .14f;
                    rotation = forward * 5f + Mathf.Sin(t * 11f) * 3f;
                    break;
                case FighterAnim.Jump:
                    rotation = forward * 6f;
                    scale = Vector2.Lerp(new Vector2(.86f, 1.16f), new Vector2(.97f, 1.03f), Mathf.Clamp01(t * 4f));
                    break;
                case FighterAnim.Fall:
                    rotation = forward * 3f;
                    scale = new Vector2(1.02f, .99f);
                    break;
                case FighterAnim.Attack:
                case FighterAnim.AirAttack:
                {
                    float p = t / Mathf.Max(.1f, oneShot > 0f ? oneShot : .3f);
                    if (p < .25f) { rotation = facing * 8f * (p / .25f); offset.x = -facing * .12f; }
                    else if (p < .6f) { rotation = forward * 16f; offset.x = facing * .5f; scale = new Vector2(1.1f, .94f); }
                    else { float k = (p - .6f) / .4f; rotation = Mathf.Lerp(forward * 16f, 0f, k); offset.x = Mathf.Lerp(facing * .5f, 0f, k); }
                    break;
                }
                case FighterAnim.Special1:
                case FighterAnim.Special2:
                case FighterAnim.Special3:
                {
                    float p = t / Mathf.Max(.1f, oneShot > 0f ? oneShot : .5f);
                    if (p < .4f) { offset.x = Mathf.Sin(t * 90f) * .07f; scale = Vector2.one * (1f + .08f * p / .4f); rotation = facing * 6f; }
                    else if (p < .75f) { rotation = forward * 12f; offset.x = facing * .35f; scale = new Vector2(1.12f, .95f); }
                    else { float k = (p - .75f) / .25f; rotation = Mathf.Lerp(forward * 12f, 0f, k); offset.x = Mathf.Lerp(facing * .35f, 0f, k); }
                    break;
                }
                case FighterAnim.Hurt:
                    rotation = facing * 16f;
                    offset.x = -facing * .15f;
                    scale = new Vector2(.95f, 1.04f);
                    break;
                case FighterAnim.Dodge:
                    rotation = forward * 10f;
                    scale = new Vector2(1.12f, .86f);
                    break;
                case FighterAnim.KO:
                    rotation = t * 900f;
                    break;
            }
        }
    }
}
