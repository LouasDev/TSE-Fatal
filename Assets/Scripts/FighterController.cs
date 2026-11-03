using UnityEngine;

namespace TSEFatal
{
    public class FighterController : MonoBehaviour
    {
        public CandidateId Candidate { get; private set; }
        public FighterController Target { get; set; }
        public float MaxHealth => 100f;
        public float Health { get; private set; } = 100f;

        private bool bot;
        private int facing;
        private float attackCooldown;
        private float specialCooldown;
        private float botDecision;
        private SpriteRenderer renderer;
        private Rigidbody2D body;

        public void Initialize(CandidateId candidate, Vector2 position, bool isBot, int initialFacing)
        {
            Candidate = candidate;
            bot = isBot;
            facing = initialFacing;
            transform.position = position;
            renderer = gameObject.AddComponent<SpriteRenderer>();
            var portrait = Resources.Load<Texture2D>($"Art/Characters/{CharacterAssetName(candidate)}");
            renderer.sprite = portrait == null
                ? Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), new Vector2(.5f, .5f))
                : Sprite.Create(portrait, new Rect(0, 0, portrait.width, portrait.height), new Vector2(.5f, 0f), 410f);
            renderer.color = portrait == null ? CharacterProfile.Tint(candidate) : Color.white;
            transform.localScale = Vector3.one;
            body = gameObject.AddComponent<Rigidbody2D>();
            body.gravityScale = 3.5f;
            body.freezeRotation = true;
            var collider = gameObject.AddComponent<BoxCollider2D>();
            collider.size = renderer.sprite.bounds.size;
            collider.offset = new Vector2(0f, renderer.sprite.bounds.size.y * .5f);
            CreateNameTag();
        }

        private void Update()
        {
            if (Health <= 0) return;
            attackCooldown -= Time.deltaTime;
            specialCooldown -= Time.deltaTime;
            if (bot) BotUpdate(); else PlayerUpdate();
            ClampArena();
        }

        private void PlayerUpdate()
        {
            float horizontal = 0f;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) horizontal = -1f;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) horizontal = 1f;
            body.linearVelocity = new Vector2(horizontal * 5f, body.linearVelocity.y);
            if (horizontal != 0) facing = horizontal > 0 ? 1 : -1;
            if (Input.GetKeyDown(KeyCode.W) && IsGrounded()) body.linearVelocity = new Vector2(body.linearVelocity.x, 8.5f);
            if (Input.GetKeyDown(KeyCode.J)) Attack(16f, 1.65f, 0.35f);
            if (Input.GetKeyDown(KeyCode.K)) Special();
        }

        private void BotUpdate()
        {
            if (Target == null || Target.Health <= 0) return;
            float distance = Target.transform.position.x - transform.position.x;
            facing = distance > 0 ? 1 : -1;
            body.linearVelocity = new Vector2(Mathf.Abs(distance) > 1.45f ? Mathf.Sign(distance) * 3f : 0f, body.linearVelocity.y);
            botDecision -= Time.deltaTime;
            if (botDecision <= 0f)
            {
                botDecision = Random.Range(.45f, 1.1f);
                if (Mathf.Abs(distance) < 1.8f) Attack(13f, 1.7f, .35f);
                else if (Mathf.Abs(distance) < 5f && Random.value > .5f) Special();
            }
        }

        private void Attack(float damage, float range, float cooldown)
        {
            if (attackCooldown > 0f || Target == null) return;
            attackCooldown = cooldown;
            if (Mathf.Abs(Target.transform.position.x - transform.position.x) <= range && Mathf.Sign(Target.transform.position.x - transform.position.x) == facing)
                Target.TakeDamage(damage, facing);
        }

        private void Special()
        {
            if (specialCooldown > 0f || Target == null) return;
            specialCooldown = 2.2f;
            if (Candidate == CandidateId.FlavioBolsonaro)
            {
                BriefcaseProjectile.Launch(transform.position + new Vector3(facing * 1.1f, 1.3f), Target, facing);
                StartCoroutine(FlashSpecial());
                return;
            }
            float range = Candidate == CandidateId.RenanSantos ? 2.8f : 4.6f;
            float damage = 20f;
            if (Mathf.Abs(Target.transform.position.x - transform.position.x) <= range) Target.TakeDamage(damage, facing);
            StartCoroutine(FlashSpecial());
        }

        private System.Collections.IEnumerator FlashSpecial()
        {
            Color old = renderer.color;
            renderer.color = Color.white;
            yield return new WaitForSeconds(.12f);
            renderer.color = old;
        }

        public void TakeDamage(float amount, int direction)
        {
            Health = Mathf.Max(0f, Health - amount);
            body.AddForce(new Vector2(direction * 5f, 2.5f), ForceMode2D.Impulse);
            if (Health <= 0) renderer.color = Color.gray;
        }

        private bool IsGrounded() => transform.position.y <= -2.36f;
        private void ClampArena() => transform.position = new Vector3(Mathf.Clamp(transform.position.x, -7.5f, 7.5f), transform.position.y, 0f);

        private static string CharacterAssetName(CandidateId candidate) => candidate switch
        {
            CandidateId.Lula => "lula_idle",
            CandidateId.RenanSantos => "renan_idle",
            CandidateId.FlavioBolsonaro => "flavio_sigma",
            _ => ""
        };

        private void CreateNameTag()
        {
            var label = new GameObject("Nome");
            label.transform.SetParent(transform);
            label.transform.localPosition = new Vector3(0, .75f, 0);
            var text = label.AddComponent<TextMesh>();
            text.text = CharacterProfile.DisplayName(Candidate);
            text.characterSize = .18f;
            text.anchor = TextAnchor.MiddleCenter;
            text.alignment = TextAlignment.Center;
            text.color = Color.white;
        }
    }
}
