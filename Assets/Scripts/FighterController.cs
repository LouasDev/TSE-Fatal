using System.Collections;
using UnityEngine;

namespace TSEFatal
{
    /// <summary>
    /// Lutador estilo plataforma: dano em %, knockback crescente, 3 vidas e nocaute ao sair da arena.
    /// Controles: A/D mover, W/Espaço pular (pulo duplo), S descer/queda rápida, J ataque,
    /// K especial 1, L especial 2, I especial 3 (só Lula), Shift esquiva.
    /// </summary>
    public class FighterController : MonoBehaviour
    {
        public const int StartingStocks = 3;
        private const float Gravity = 32f, MaxFall = 17f, FastFall = 25f;
        private const float JumpSpeed = 13.2f, AirJumpSpeed = 11.6f;
        private const int AirJumps = 2;
        private const float BodyHeight = 3.3f;

        public CandidateId Candidate { get; private set; }
        public FighterController Target { get; set; }
        public float DamagePercent { get; private set; }
        public int Stocks { get; private set; } = StartingStocks;
        public bool IsBot { get; private set; }
        /// <summary>Dificuldade usada quando este lutador é controlado pela IA.</summary>
        public Difficulty BotLevel { get; set; } = Difficulty.Normal;
        public bool OutOfPlay { get; private set; }
        public bool Eliminated => Stocks <= 0 && OutOfPlay;
        public bool ControlsEnabled { get; set; }
        public bool Grounded => grounded;
        public float SilenceRemaining => silence;
        public float ConfusionRemaining => confusion;
        public float SleepRemaining => sleep;
        public float Cooldown(int slot) => slot == 1 ? cd1 : slot == 2 ? cd2 : cd3;
        public Rect Hurtbox => new Rect(transform.position.x - .65f, transform.position.y + .1f, 1.3f, BodyHeight);
        public System.Action<FighterController> OnKnockedOut;
        public System.Action<FighterController> OnHitTaken;

        private Arena arena;
        private MenuAudio sfx;
        private FighterAnimator anim;
        private FighterStats stats;
        private Vector2 vel;
        private bool grounded;
        private StagePlatform floor;
        private int airJumpsLeft;
        private bool airDodgeUsed;
        private int facing = 1;
        private float hitstun, dodgeTimer, dodgeCd, cd1, cd2, cd3, silence, confusion, sleep, zzzClock, dropTimer, respawnTimer, spawnShield, comboWindow, afterImageTimer, deniedTimer, stinkTimer;
        private int comboStep;
        private Coroutine move;
        private float spawnX;
        private SpriteRenderer shadow, silenceBar, zzz;
        private Sprite[] zzzFrames;

        // Entrada do quadro atual (teclado ou bot).
        private float inX;
        private bool inUp, inDown, jumpPress, dropPress, attackPress, sp1Press, sp2Press, sp3Press, dodgePress;
        // IA: tempo de reação, pausa entre ataques e recuo, para o bot não "metralhar" golpes.
        private float botThink, botReaction, botAttackCd, botRetreat;
        private bool botWasClose;

        private bool Busy => move != null;
        private Vector2 Pos => transform.position;

        public void Initialize(CandidateId candidate, Arena stage, MenuAudio audio, float x, bool bot, int initialFacing, int sortingOrder)
        {
            Candidate = candidate;
            arena = stage;
            sfx = audio;
            IsBot = bot;
            facing = initialFacing;
            spawnX = x;
            stats = CharacterProfile.Stats(candidate);
            transform.position = new Vector3(x, Arena.GroundY, 0);
            grounded = true;
            floor = arena.Ground;
            airJumpsLeft = AirJumps;

            anim = gameObject.AddComponent<FighterAnimator>();
            anim.Init(candidate, sortingOrder);
            anim.SetFacing(facing);

            var shadowGo = new GameObject("Sombra");
            shadowGo.transform.SetParent(transform, false);
            shadow = shadowGo.AddComponent<SpriteRenderer>();
            shadow.sprite = PixelArt.Shadow;
            shadow.sortingOrder = -5;

            var bar = new GameObject("Tarja de censura");
            bar.transform.SetParent(transform, false);
            bar.transform.localPosition = new Vector3(0, FighterAnimator.Height * .86f, 0);
            silenceBar = bar.AddComponent<SpriteRenderer>();
            silenceBar.sprite = PixelArt.CensorBar;
            silenceBar.sortingOrder = sortingOrder + 3;
            bar.transform.localScale = Vector3.one * Effects.ScaleToHeight(silenceBar.sprite, .5f);
            silenceBar.enabled = false;

            // Zzz da Palestra Motivacional (Cury): aparece sobre a cabeça enquanto o lutador dorme.
            zzzFrames = PixelArt.SleepZzz;
            var sleepGo = new GameObject("Zzz");
            sleepGo.transform.SetParent(transform, false);
            sleepGo.transform.localPosition = new Vector3(.7f, FighterAnimator.Height * 1.08f, 0);
            zzz = sleepGo.AddComponent<SpriteRenderer>();
            zzz.sprite = zzzFrames[0];
            zzz.sortingOrder = sortingOrder + 3;
            sleepGo.transform.localScale = Vector3.one * Effects.ScaleToHeight(zzzFrames, 1.1f);
            zzz.enabled = false;

            CreateTag(bot ? "CPU" : "P1", bot ? new Color(.75f, .78f, .85f) : CharacterProfile.Tint(candidate));
        }

        private void CreateTag(string text, Color color)
        {
            var go = new GameObject("Marcador " + text);
            go.transform.SetParent(transform, false);
            go.transform.localPosition = new Vector3(0, FighterAnimator.Height + .55f, 0);
            var tm = go.AddComponent<TextMesh>();
            Font font = Resources.Load<Font>("Fonts/PressStart2P-Regular");
            if (font != null) { tm.font = font; go.GetComponent<MeshRenderer>().sharedMaterial = font.material; }
            tm.text = text; tm.color = color; tm.fontSize = 48; tm.characterSize = .045f;
            tm.anchor = TextAnchor.MiddleCenter; tm.alignment = TextAlignment.Center;
            go.GetComponent<MeshRenderer>().sortingOrder = 18;
        }

        // ---------------------------------------------------------------- ciclo

        private void Update()
        {
            if (Combat.Frozen) return;
            float dt = Time.deltaTime;

            if (OutOfPlay)
            {
                if (Stocks > 0 && (respawnTimer -= dt) <= 0f) Respawn();
                return;
            }

            dodgeCd -= dt; cd1 -= dt; cd2 -= dt; cd3 -= dt; dropTimer -= dt; comboWindow -= dt; deniedTimer -= dt;
            spawnShield -= dt;
            silence = Mathf.Max(0f, silence - dt);
            confusion = Mathf.Max(0f, confusion - dt);
            sleep = Mathf.Max(0f, sleep - dt);
            if (confusion > 0f && (stinkTimer -= dt) <= 0f)
            {
                stinkTimer = .12f;
                var puff = Effects.Sprite(PixelArt.Dust, Pos + new Vector2(Random.Range(-.7f, .7f), Random.Range(2.6f, 3.8f)), .35f, 12);
                puff.Renderer.color = new Color(.55f, .75f, .2f, .8f);
                puff.Velocity = new Vector2(Random.Range(-.4f, .4f), 1.4f); puff.Life = .6f; puff.EndScale = 2f;
            }

            ReadInput(dt);
            // Tonto: esquerda e direita trocadas. O bot não percebe a inversão como uma pessoa, então
            // fora da arena ele ignorava a recuperação e voava para o vazio; ali ele volta a controlar.
            if (confusion > 0f && !(IsBot && OffStage)) inX = -inX;

            if (hitstun > 0f)
            {
                hitstun -= dt;
                vel.x = Mathf.MoveTowards(vel.x, 0f, (grounded ? 30f : 4.5f) * dt);
            }
            else if (dodgeTimer > 0f)
            {
                dodgeTimer -= dt;
                if ((afterImageTimer -= dt) <= 0f) { afterImageTimer = .04f; Effects.AfterImage(anim.Renderer, new Color(.6f, .85f, 1f, .55f)); }
                if (dodgeTimer <= 0f) vel *= .35f;
            }
            else
            {
                HandleControl(dt);
            }

            Physics(dt);
            UpdateAnimation();
            UpdateOverlays();

            if (arena.OutOfBounds(Pos)) KnockOut();
        }

        private void HandleControl(float dt)
        {
            float targetVx = inX * stats.Speed;
            if (grounded) vel.x = Mathf.MoveTowards(vel.x, Busy ? 0f : targetVx, (Busy ? 40f : 90f) * dt);
            else vel.x = Mathf.MoveTowards(vel.x, targetVx, (Busy ? 14f : 38f) * dt);
            if (!Busy && inX != 0f) facing = inX > 0 ? 1 : -1;
            if (Busy) return;

            if (jumpPress)
            {
                if (grounded) Jump(JumpSpeed, true);
                else if (airJumpsLeft > 0) { airJumpsLeft--; Jump(AirJumpSpeed, false); }
            }
            if (dropPress && grounded && floor != null && floor.OneWay)
            {
                dropTimer = .25f; grounded = false; floor = null;
                transform.position += Vector3.down * .08f;
            }
            if (dodgePress && dodgeCd <= 0f && (grounded || !airDodgeUsed)) { StartDodge(); return; }
            if (attackPress) { LightAttack(); return; }

            bool anySpecial = sp1Press || sp2Press || sp3Press;
            if (anySpecial && silence > 0f)
            {
                if (deniedTimer <= 0f)
                {
                    deniedTimer = .6f;
                    Effects.Callout("CENSURADO", Pos + Vector2.up * 4.6f, new Color(.95f, .2f, .2f), .7f, .9f);
                    sfx.Denied();
                }
                return;
            }
            if (sp1Press && cd1 <= 0f) move = StartCoroutine(Special1());
            else if (sp2Press && cd2 <= 0f) move = StartCoroutine(Special2());
            else if (sp3Press && cd3 <= 0f && CharacterProfile.Special3Name(Candidate) != null) move = StartCoroutine(Special3());
        }

        private void Jump(float speed, bool fromGround)
        {
            vel.y = speed;
            grounded = false;
            floor = null;
            anim.Play(FighterAnim.Jump, 0f, true);
            anim.Squash(new Vector2(.85f, 1.15f));
            Effects.Dust(Pos, fromGround ? 4 : 3);
            sfx.Jump();
        }

        private void Physics(float dt)
        {
            Vector2 pos = Pos;
            float prevY = pos.y;
            bool floating = dodgeTimer > 0f && !grounded;
            if (!grounded && !floating)
            {
                bool fastFall = inDown && vel.y < 0f && hitstun <= 0f;
                vel.y -= Gravity * (fastFall ? 1.5f : 1f) * dt;
                float limit = fastFall ? FastFall : MaxFall;
                if (vel.y < -limit) vel.y = -limit;
            }
            pos += vel * dt;

            if (grounded)
            {
                if (floor == null || !floor.Contains(pos.x)) { grounded = false; floor = null; }
                else { pos.y = floor.Top; vel.y = 0f; }
            }
            else if (vel.y <= 0f)
            {
                StagePlatform landing = null;
                foreach (StagePlatform p in arena.Platforms)
                {
                    if (!p.Contains(pos.x) || prevY < p.Top - .02f || pos.y > p.Top) continue;
                    if (p.OneWay && (dropTimer > 0f || (inDown && hitstun <= 0f && !IsBot))) continue;
                    if (landing == null || p.Top > landing.Top) landing = p;
                }
                if (landing != null) { pos.y = landing.Top; Land(landing); }
            }

            // O chão principal é um bloco sólido: não dá para entrar nele pela lateral.
            StagePlatform g = arena.Ground;
            if (!grounded && pos.y < g.Top - .05f && pos.y + BodyHeight > g.Bottom && pos.x > g.Left - .45f && pos.x < g.Right + .45f)
            {
                pos.x = pos.x < 0f ? g.Left - .45f : g.Right + .45f;
                vel.x = 0f;
            }

            transform.position = new Vector3(pos.x, pos.y, 0f);
        }

        private void Land(StagePlatform p)
        {
            bool hard = vel.y < -10f;
            if (vel.y < -6f) sfx.Land();
            grounded = true;
            floor = p;
            vel.y = 0f;
            airJumpsLeft = AirJumps;
            airDodgeUsed = false;
            anim.Squash(new Vector2(1.16f, .84f));
            if (hard) Effects.Dust(Pos, 5);
            if (hitstun > 0f) vel.x *= .6f;
        }

        private void UpdateAnimation()
        {
            anim.SetFacing(facing);
            if (hitstun > 0f || Busy) return; // golpes e dano escolhem a própria animação
            if (dodgeTimer > 0f) return;
            if (!grounded) anim.Play(vel.y > 0f ? FighterAnim.Jump : FighterAnim.Fall);
            else anim.Play(Mathf.Abs(vel.x) > .6f ? FighterAnim.Walk : FighterAnim.Idle);
        }

        private void UpdateOverlays()
        {
            StagePlatform below = arena.SurfaceBelow(Pos.x, Pos.y);
            shadow.enabled = below != null;
            if (below != null)
            {
                float gap = Pos.y - below.Top;
                shadow.transform.position = new Vector3(Pos.x, below.Top + .03f, 0f);
                shadow.transform.localScale = new Vector3(1.4f, 1f, 1f) * Mathf.Clamp(1f - gap / 9f, .35f, 1f);
            }
            silenceBar.enabled = silence > 0f && (silence > 1f || Mathf.Repeat(Time.time * 8f, 1f) > .4f);
            zzz.enabled = sleep > 0f;
            if (zzz.enabled)
            {
                zzzClock += Time.deltaTime;
                zzz.sprite = zzzFrames[Mathf.FloorToInt(zzzClock * 5f) % zzzFrames.Length];
            }
            Color tint = Color.white;
            if (spawnShield > 0f) tint.a = Mathf.Repeat(Time.time * 10f, 1f) > .5f ? .45f : 1f;
            anim.SetTint(tint);
        }

        // ---------------------------------------------------------------- entrada

        private void ReadInput(float dt)
        {
            inX = 0f; inUp = inDown = jumpPress = dropPress = attackPress = sp1Press = sp2Press = sp3Press = dodgePress = false;
            if (!ControlsEnabled || sleep > 0f) return; // dormindo na palestra: não faz nada
            if (IsBot) { BotInput(dt); return; }

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) inX -= 1f;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) inX += 1f;
            inUp = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
            inDown = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
            jumpPress = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space);
            dropPress = Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow);
            attackPress = Input.GetKeyDown(KeyCode.J);
            sp1Press = Input.GetKeyDown(KeyCode.K);
            sp2Press = Input.GetKeyDown(KeyCode.L);
            sp3Press = Input.GetKeyDown(KeyCode.I);
            dodgePress = Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift);
        }

        private void BotInput(float dt)
        {
            if (Target == null || Target.OutOfPlay)
            {
                if (Mathf.Abs(Pos.x) > 3f) inX = -Mathf.Sign(Pos.x);
                return;
            }

            Vector2 me = Pos, them = Target.Pos;
            float dx = them.x - me.x, dy = them.y - me.y;
            float adx = Mathf.Abs(dx), ady = Mathf.Abs(dy);
            StagePlatform g = arena.Ground;

            // Recuperação: fora do chão, volta para o centro gastando pulos e esquiva.
            bool offStage = me.x < g.Left || me.x > g.Right || me.y < g.Top - .3f;
            if (!grounded && offStage)
            {
                inX = -Mathf.Sign(me.x);
                botThink -= dt;
                if (vel.y < 1.5f && airJumpsLeft > 0 && botThink <= 0f) { jumpPress = true; botThink = .25f; }
                else if (vel.y < -3f && airJumpsLeft == 0 && !airDodgeUsed && dodgeCd <= 0f) { dodgePress = true; inUp = true; }
                return;
            }

            botAttackCd -= dt;
            botRetreat -= dt;
            bool close = adx < 2.3f && ady < 1.7f;
            // Tempo de reação "humano": ao entrar no alcance, demora um pouco antes do primeiro golpe.
            BotDifficulty.Tuning ai = BotDifficulty.Get(BotLevel);
            if (close && !botWasClose) botReaction = Random.Range(ai.ReactionMin, ai.ReactionMax);
            botWasClose = close;
            botReaction -= dt;

            float desired = adx > 1.6f ? Mathf.Sign(dx) : 0f;
            if (botRetreat > 0f) desired = -Mathf.Sign(dx);
            if (grounded && floor == g && Mathf.Abs(me.x + desired * .8f) > g.Right - .8f) desired = 0f;
            inX = desired;
            if (!Busy && adx > .2f && botRetreat <= 0f) facing = dx > 0 ? 1 : -1;

            botThink -= dt;
            if (botThink > 0f) return;
            botThink = Random.Range(ai.ThinkMin, ai.ThinkMax);

            if (dy > 2.2f && adx < 5f && Random.value < .5f) jumpPress = true;
            if (dy < -2.5f && grounded && floor != null && floor.OneWay && Random.value < .5f) dropPress = true;
            if (botAttackCd > 0f || botReaction > 0f || botRetreat > 0f) return;

            // Especiais: chance menor e uma pausa depois de cada um.
            float r = Random.value / ai.SpecialScale; // dificuldade maior = mais especiais
            bool special = false;
            switch (Candidate)
            {
                case CandidateId.Lula:
                    if (cd3 <= 0f && adx < 12f && ady < 1.4f && r < .12f) special = sp3Press = true;
                    else if (cd2 <= 0f && adx < 3.2f && ady < 2f && r < .22f) special = sp2Press = true;
                    else if (cd1 <= 0f && adx > 3f && adx < 11f && ady < 2.2f && r < .25f) special = sp1Press = true;
                    break;
                case CandidateId.RenanSantos:
                    if (cd3 <= 0f && adx > 3f && adx < 9f && r < .12f) special = sp3Press = true;
                    else if (cd1 <= 0f && adx < 5.5f && ady < 1.2f && r < .22f) special = sp1Press = true;
                    else if (cd2 <= 0f && adx > 3f && ady < 1f && grounded && r < .22f) special = sp2Press = true;
                    break;
                case CandidateId.FlavioBolsonaro:
                    if (cd3 <= 0f && adx > 2.5f && adx < 9f && ady < 1.5f && r < .14f) special = sp3Press = true;
                    else if (cd1 <= 0f && adx < 4.5f && ady < 1.2f && r < .22f) special = sp1Press = true;
                    else if (cd2 <= 0f && r < .1f) special = sp2Press = true;
                    break;
                case CandidateId.AugustoCury:
                    if (cd3 <= 0f && adx < 5.5f && ady < 1.5f && Target.Grounded && r < .14f) special = sp3Press = true;
                    else if (cd2 <= 0f && adx > 2.5f && adx < 12f && ady < 1f && grounded && r < .2f) special = sp2Press = true;
                    else if (cd1 <= 0f && adx > 3f && r < .22f) special = sp1Press = true;
                    break;
            }
            if (special) { botAttackCd = Random.Range(.9f, 1.5f); return; }

            if (close)
            {
                float roll = Random.value;
                if (roll < ai.AttackChance)
                {
                    attackPress = true;
                    // De vez em quando emenda o combo; no resto, respira entre um soco e outro.
                    botAttackCd = Random.value < ai.ComboChance ? .18f : Random.Range(ai.PauseMin, ai.PauseMax);
                }
                else if (roll < ai.AttackChance + ai.DodgeChance && dodgeCd <= 0f) dodgePress = true;
                else if (roll < ai.AttackChance + ai.DodgeChance + .18f) botRetreat = Random.Range(.25f, .5f);
                else botAttackCd = Random.Range(.2f, .4f); // hesita
            }
        }

        // ---------------------------------------------------------------- dano e nocaute

        public bool TakeHit(HitData hit, int direction, Vector2 point)
        {
            if (OutOfPlay || spawnShield > 0f || dodgeTimer > 0f) return false;
            DamagePercent += hit.Damage;
            float kb = (hit.BaseKnockback + hit.GrowthKnockback * DamagePercent / 100f) / stats.Weight;
            float a = hit.AngleDegrees * Mathf.Deg2Rad;
            vel = new Vector2(Mathf.Cos(a) * direction, Mathf.Sin(a)) * kb;
            if (vel.y > 0f) { grounded = false; floor = null; }
            hitstun = .12f + kb * .022f;
            CancelMove();
            facing = -direction;
            anim.Play(FighterAnim.Hurt, 0f, true);
            anim.Flash(Color.white, .14f);
            anim.Squash(new Vector2(.85f, 1.12f));
            if (hit.SilenceSeconds > 0f)
            {
                silence = Mathf.Max(silence, hit.SilenceSeconds);
                Effects.Callout("CENSURADO!", Pos + Vector2.up * 4.6f, new Color(1f, .2f, .2f), 1.1f, 1.8f);
                sfx.Bleep();
            }
            Combat.AddHitStop(.035f + hit.Damage * .004f);
            Combat.Shake(Mathf.Min(.5f, kb * .018f), .15f);
            sfx.Hit(kb > 11f);
            // Gemido gravado de vez em quando, nos golpes mais fortes (não toda hora, para não cansar).
            if (kb > 8f && Random.value < .4f) sfx.Voice(this, VoicePrefix + "dano");
            if (sleep > 0f)
            {
                sleep = 0f;
                Effects.Callout("ACORDOU!", Pos + Vector2.up * 4.6f, new Color(.6f, .8f, 1f), .8f, .9f);
            }
            OnHitTaken?.Invoke(this);
            return true;
        }

        private void KnockOut()
        {
            OutOfPlay = true;
            Stocks--;
            CancelMove();
            silence = 0f;
            anim.SetVisible(false);
            shadow.enabled = false;
            silenceBar.enabled = false;
            Vector2 edge = new Vector2(Mathf.Clamp(Pos.x, -14.3f, 14.3f), Mathf.Clamp(Pos.y, -7.8f, 7.8f));
            Effects.KOBlast(edge, CharacterProfile.Tint(Candidate));
            Vector2 quipAt = new Vector2(Mathf.Clamp(edge.x, -9f, 9f), Mathf.Clamp(edge.y, -3f, 4.5f));
            // A fala na tela e o áudio gravado são a mesma frase (nocaute_1 = primeira da lista etc.).
            string[] lines = CharacterProfile.KnockedOutLines(Candidate);
            int line = Random.Range(0, lines.Length);
            Effects.Callout(lines[line], quipAt, Color.Lerp(CharacterProfile.Tint(Candidate), Color.white, .4f), .85f);
            sfx.Voice(this, true, $"{VoicePrefix}nocaute_{line + 1}", VoicePrefix + "nocaute");
            confusion = 0f;
            sleep = 0f;
            Combat.Shake(.6f, .4f);
            Combat.AddHitStop(.1f);
            sfx.KO();
            respawnTimer = 1.4f;
            OnKnockedOut?.Invoke(this);
        }

        private void Respawn()
        {
            OutOfPlay = false;
            transform.position = new Vector3(spawnX * .5f, 5.5f, 0f);
            vel = Vector2.zero;
            DamagePercent = 0f;
            hitstun = dodgeTimer = 0f;
            spawnShield = 2f;
            airJumpsLeft = AirJumps;
            airDodgeUsed = false;
            grounded = false;
            floor = null;
            anim.SetVisible(true);
            anim.Play(FighterAnim.Fall, 0f, true);
            string[] lines = CharacterProfile.RespawnLines(Candidate);
            int line = Random.Range(0, lines.Length);
            Effects.Callout(lines[line], Pos + Vector2.up * 4.4f, Color.Lerp(CharacterProfile.Tint(Candidate), Color.white, .5f), .75f);
            sfx.Voice(this, true, $"{VoicePrefix}volta_{line + 1}", VoicePrefix + "volta");
        }

        /// <summary>Começo do nome dos arquivos de voz deste personagem: "lula_", "renan_", "flavio_".</summary>
        public string VoicePrefix => CharacterProfile.AssetFolder(Candidate) + "_";

        /// <summary>Fralda do Debate: controles invertidos por alguns segundos.</summary>
        /// <summary>No ar e fora do chão principal (abaixo dele ou além das bordas).</summary>
        private bool OffStage
        {
            get
            {
                StagePlatform g = arena.Ground;
                return !grounded && (Pos.x < g.Left || Pos.x > g.Right || Pos.y < g.Top - .3f);
            }
        }

        public void Confuse(float seconds)
        {
            if (OutOfPlay) return;
            confusion = Mathf.Max(confusion, seconds);
            Effects.Callout("TONTO!", Pos + Vector2.up * 4.6f, new Color(.7f, .9f, .3f), .9f, 1.8f);
        }

        /// <summary>
        /// Palestra Motivacional: dorme em pé por alguns segundos. Só pega quem está no chão
        /// (no ar, dormir seria cair no vazio sem chance de reagir). Esquiva e escudo de volta evitam.
        /// </summary>
        public bool Sleep(float seconds)
        {
            if (OutOfPlay || !grounded || dodgeTimer > 0f || spawnShield > 0f) return false;
            CancelMove();
            sleep = Mathf.Max(sleep, seconds);
            zzzClock = 0f;
            Effects.Callout("DORMIU NA PALESTRA!", Pos + Vector2.up * 4.8f, new Color(.6f, .8f, 1f), .8f, 1.6f);
            return true;
        }

        /// <summary>Reduz o dano acumulado (Rachadinha, Chocolate Lavado, picanha).</summary>
        public void Heal(float amount, string label, Color color)
        {
            if (OutOfPlay) return;
            DamagePercent = Mathf.Max(0f, DamagePercent - amount);
            if (!string.IsNullOrEmpty(label)) Effects.Callout(label, Pos + Vector2.up * 4.3f, color, .8f);
        }

        private void Quip(string[] options, Vector2 at, Color color) =>
            Effects.Callout(CharacterProfile.Pick(options), at, color, .8f);

        // ---------------------------------------------------------------- esquiva e ataques

        private void StartDodge()
        {
            Vector2 dir = grounded ? new Vector2(inX, 0f) : new Vector2(inX, (inUp ? 1f : 0f) - (inDown ? 1f : 0f));
            dodgeTimer = .28f;
            dodgeCd = .9f;
            if (!grounded) airDodgeUsed = true;
            vel = dir.sqrMagnitude > 0f ? dir.normalized * 13f : Vector2.zero;
            anim.Play(FighterAnim.Dodge, .28f, true);
            sfx.Whoosh();
        }

        private void LightAttack()
        {
            if (!grounded) { move = StartCoroutine(AirAttack()); return; }
            comboStep = comboWindow > 0f ? comboStep % 3 + 1 : 1;
            move = StartCoroutine(Jab(comboStep));
        }

        private IEnumerator Jab(int step)
        {
            bool finisher = step == 3;
            float startup = .06f, active = .08f, recovery = finisher ? .26f : .12f;
            anim.Play(FighterAnim.Attack, startup + active + recovery, true);
            sfx.Swing();
            vel.x = facing * (finisher ? 4.5f : 2f);
            yield return Wait(startup);
            HitData hit = finisher ? new HitData(8f, 6.5f, 7.5f, 40f) : new HitData(4f, 3.5f, 1.8f, 25f);
            yield return Active(active, new Vector2(1.05f, 1.7f), new Vector2(1.8f, 1.3f), hit, Color.white);
            yield return Wait(recovery);
            comboWindow = finisher ? 0f : .35f;
            move = null;
        }

        private IEnumerator AirAttack()
        {
            anim.Play(FighterAnim.AirAttack, .34f, true);
            sfx.Swing();
            yield return Wait(.07f);
            yield return Active(.12f, new Vector2(.95f, 1.4f), new Vector2(1.9f, 1.8f), new HitData(6f, 5f, 5.5f, 35f), Color.white);
            yield return Wait(.15f);
            move = null;
        }

        private static readonly string[] MensalaoHits = { "PROPINA!", "MENSALINHO!", "É CAIXA 2!", "TÁ NA CONTA!" };

        private IEnumerator Special1()
        {
            Color color = CharacterProfile.Tint(Candidate);
            Shout(CharacterProfile.Special1Name(Candidate), color, 1);
            switch (Candidate)
            {
                case CandidateId.Lula: // Mensalão: três maços de dinheiro em leque
                {
                    cd1 = 1.6f;
                    anim.Play(FighterAnim.Special1, .6f, true);
                    sfx.CashRegister();
                    yield return Wait(.24f);
                    // Uma piada por arremesso: antes cada maço gerava uma frase e elas se atropelavam.
                    var throwState = new bool[1];
                    for (int i = 0; i < 3; i++)
                    {
                        float a = (8f + (i - 1) * 12f) * Mathf.Deg2Rad;
                        var p = Projectile.Spawn(this, arena, PixelArt.CashSpin, Pos + new Vector2(facing * 1.1f, 2.1f), .55f, new HitData(4.5f, 4f, 4.5f, 30f), 14f);
                        p.Velocity = new Vector2(Mathf.Cos(a) * facing, Mathf.Sin(a)) * 15f;
                        p.Gravity = 6f; p.Spin = p.Animated ? 0f : -facing * 520f; p.Life = 1.1f; p.HitSize = new Vector2(.9f, .7f);
                        p.Face(facing);
                        p.SparkColor = new Color(.5f, 1f, .5f);
                        p.OnHit = t =>
                        {
                            Effects.Burst(PixelArt.BillFlutter, t.Hurtbox.center, 6, 7f, .35f);
                            sfx.Coin();
                            if (throwState[0]) return;
                            throwState[0] = true;
                            Quip(MensalaoHits, t.Pos + Vector2.up * 4.2f, new Color(.55f, 1f, .5f));
                        };
                        sfx.Paper();
                        yield return Wait(.06f);
                    }
                    yield return Wait(.18f);
                    break;
                }

                case CandidateId.RenanSantos: // Corte Viral: investida filmando com o celular
                    cd1 = 1.4f;
                    anim.Play(FighterAnim.Special1, .45f, true);
                    sfx.Plim();
                    yield return Wait(.08f);
                    Vector2 start = Pos;
                    yield return Dash(.16f, 28f, new HitData(7f, 5f, 7f, 30f), new Color(.85f, .7f, 1f), t =>
                    {
                        sfx.Plim();
                        Effects.Callout("VIROU CORTE!", t.Pos + Vector2.up * 4.2f, new Color(.85f, .7f, 1f), .8f);
                    });
                    Sprite[] cut = PixelArt.ViralCut;
                    if (cut.Length > 1)
                    {
                        var slash = Effects.Anim(cut, (start + Pos) * .5f + Vector2.up * 1.6f, 1.6f, .35f, 11);
                        slash.Renderer.flipX = facing < 0;
                    }
                    else
                    {
                        var slash = Effects.Sprite(cut[0], (start + Pos) * .5f + Vector2.up * 1.6f, 1.1f, 11);
                        slash.Life = .25f; slash.EndScale = 1.3f;
                    }
                    yield return Wait(.2f);
                    vel.x *= .2f;
                    break;

                case CandidateId.FlavioBolsonaro: // Rachadinha: investida que fica com parte do dano
                    cd1 = 1.8f;
                    anim.Play(FighterAnim.Special1, .5f, true);
                    yield return Wait(.1f);
                    yield return Dash(.22f, 20f, new HitData(8f, 5.5f, 6f, 30f), new Color(1f, .85f, .3f), target =>
                    {
                        Heal(4f, "RACHOU! -4%", new Color(.4f, 1f, .4f));
                        Effects.Burst(PixelArt.CoinSpin, target.Hurtbox.center, 8, 9f, .35f);
                        sfx.Coin(); sfx.CashRegister();
                    });
                    yield return Wait(.2f);
                    break;

                case CandidateId.AugustoCury: // Drone com Sirene: persegue o rival pelo ar
                {
                    cd1 = 2.6f;
                    anim.Play(FighterAnim.Special1, .65f, true);
                    yield return Wait(.38f); // frame do botão do controle
                    sfx.Siren();
                    var drone = Projectile.Spawn(this, arena, PixelArt.DroneFly, Pos + new Vector2(facing * 1.2f, 3.3f), .9f, new HitData(7f, 5f, 5.5f, 40f), 14f);
                    drone.Velocity = new Vector2(facing * 7f, 0f);
                    drone.Life = 2.4f; drone.HitSize = new Vector2(1.3f, .8f);
                    drone.Face(facing);
                    drone.SparkColor = new Color(.4f, .6f, 1f);
                    drone.OnHit = t =>
                    {
                        sfx.Siren();
                        Effects.Callout("DRONEFOBIA?", t.Pos + Vector2.up * 4.4f, new Color(.55f, .75f, 1f), .8f);
                    };
                    StartCoroutine(DroneChase(drone));
                    yield return Wait(.25f);
                    break;
                }
            }
            move = null;
        }

        /// <summary>O drone corrige a rota aos poucos na direção do rival: dá para fugir ou esquivar.</summary>
        private IEnumerator DroneChase(Projectile drone)
        {
            while (drone != null)
            {
                if (!Combat.Frozen && Target != null && !Target.OutOfPlay)
                {
                    Vector2 to = Target.Hurtbox.center + Vector2.up * .6f - (Vector2)drone.transform.position;
                    drone.Velocity = Vector2.MoveTowards(drone.Velocity, to.normalized * 8f, 14f * Time.deltaTime);
                    if (Mathf.Abs(drone.Velocity.x) > .5f) drone.Face(drone.Velocity.x > 0f ? 1 : -1);
                }
                yield return null;
            }
        }

        private IEnumerator Special2()
        {
            Color color = CharacterProfile.Tint(Candidate);
            Shout(CharacterProfile.Special2Name(Candidate), color, 2);
            switch (Candidate)
            {
                case CandidateId.Lula: // Faz o L: L gigante à frente
                    cd2 = 3.2f;
                    anim.Play(FighterAnim.Special2, .75f, true);
                    yield return Wait(.3f);
                    Sprite[] lFrames = PixelArt.LetterLBlast;
                    if (lFrames.Length > 1) Effects.Anim(lFrames, Pos + new Vector2(facing * 1.9f, 1.7f), 3.2f, .55f, 8);
                    else
                    {
                        var l = Effects.Sprite(lFrames[0], Pos + new Vector2(facing * 1.9f, 1.7f), 3.2f, 8);
                        l.Life = .45f; l.EndScale = 1.35f;
                    }
                    Combat.Shake(.25f, .2f);
                    sfx.Zap();
                    yield return Active(.14f, new Vector2(1.9f, 1.7f), new Vector2(3f, 3.2f), new HitData(13f, 8f, 10f, 42f), new Color(1f, .3f, .3f));
                    yield return Wait(.3f);
                    break;

                case CandidateId.RenanSantos: // Onça Liberal: corre pelo chão
                    cd2 = 3f;
                    anim.Play(FighterAnim.Special2, .55f, true);
                    yield return Wait(.25f);
                    sfx.Roar();
                    Sprite[] run = PixelArt.JaguarRun;
                    var onca = Projectile.Spawn(this, arena, run, Pos + new Vector2(facing * 1.2f, 0f), run.Length > 2 ? 1.3f : 1.1f, new HitData(10f, 7f, 8.5f, 45f), run.Length > 2 ? 14f : 10f);
                    onca.Velocity = new Vector2(facing * 13f, 0f);
                    onca.RunsOnGround = true; onca.Life = 1.6f;
                    onca.HitSize = new Vector2(1.8f, 1f); onca.HitOffset = new Vector2(0f, .5f);
                    onca.SparkColor = new Color(1f, .65f, .2f);
                    int roarSide = facing;
                    onca.OnHit = t =>
                    {
                        sfx.Roar();
                        Sprite[] roar = PixelArt.JaguarRoar;
                        if (roar != null)
                        {
                            var head = Effects.Anim(roar, t.Hurtbox.center + new Vector2(-roarSide * .4f, .6f), 1.8f, .6f, 13);
                            head.Renderer.flipX = roarSide < 0;
                        }
                        Effects.Callout("RAWR LIBERAL!", t.Pos + Vector2.up * 4.2f, new Color(1f, .75f, .3f), .8f);
                    };
                    onca.Face(facing);
                    yield return Wait(.3f);
                    break;

                case CandidateId.FlavioBolsonaro: // Mansão de 6 Milhões: cai do céu em cima do rival
                    cd2 = 4.2f;
                    anim.Play(FighterAnim.Special2, .6f, true);
                    yield return Wait(.3f);
                    if (Target != null && !Target.OutOfPlay) StartCoroutine(MansionDrop(Target.Pos));
                    yield return Wait(.2f);
                    break;

                case CandidateId.AugustoCury: // Ministério da IA: três robozinhos marcham pelo chão
                {
                    cd2 = 4.5f;
                    anim.Play(FighterAnim.Special2, .8f, true);
                    yield return Wait(.35f); // frame do tablet erguido
                    Sprite[] walk = PixelArt.RobotWalk;
                    var firstHit = new bool[1];
                    for (int i = 0; i < 3; i++)
                    {
                        sfx.Robot();
                        var robo = Projectile.Spawn(this, arena, walk, Pos + new Vector2(facing * 1.3f, 0f), 1.1f, new HitData(5f, 6f, 6.5f, 35f), 8f);
                        robo.Velocity = new Vector2(facing * 7.5f, 0f);
                        robo.RunsOnGround = true; robo.Life = 2f;
                        robo.HitSize = new Vector2(1f, 1f); robo.HitOffset = new Vector2(0f, .5f);
                        robo.SparkColor = new Color(.5f, .8f, 1f);
                        robo.Face(facing);
                        robo.OnHit = t =>
                        {
                            sfx.Robot();
                            Effects.Callout(firstHit[0] ? "DEMITIDO!" : "CORTE DE 30%!", t.Pos + Vector2.up * 4.4f, new Color(.6f, .85f, 1f), .8f);
                            firstHit[0] = true;
                        };
                        yield return Wait(.2f);
                    }
                    break;
                }
            }
            move = null;
        }

        private IEnumerator Special3()
        {
            Color color = CharacterProfile.Tint(Candidate);
            switch (Candidate)
            {
                case CandidateId.Lula: // Censura: tarja preta que bloqueia as habilidades do rival
                {
                    cd3 = 9f;
                    Shout(CharacterProfile.Special3Name(Candidate), new Color(.15f, .15f, .18f), 3);
                    anim.Play(FighterAnim.Special3, .6f, true);
                    yield return Wait(.32f); // frame do arremesso
                    var bar = Projectile.Spawn(this, arena, PixelArt.CensorFly, Pos + new Vector2(facing * 1.3f, 2.6f), .7f, new HitData(3f, 3f, 1f, 20f, 3.5f), 12f);
                    bar.Face(facing);
                    bar.Velocity = new Vector2(facing * 17f, 0f);
                    bar.Life = .9f; bar.HitSize = new Vector2(1.7f, .8f);
                    bar.SparkColor = new Color(1f, .25f, .25f);
                    sfx.Whoosh();
                    yield return Wait(.25f);
                    break;
                }

                case CandidateId.RenanSantos: // Fralda do Debate: fralda cheia em arco, deixa o rival tonto
                {
                    cd3 = 7f;
                    Shout(CharacterProfile.Special3Name(Candidate), color, 3);
                    anim.Play(FighterAnim.Special3, .5f, true);
                    yield return Wait(.22f);
                    var fralda = Projectile.Spawn(this, arena, PixelArt.DiaperSpin, Pos + new Vector2(facing * 1.1f, 2.5f), 1.4f, new HitData(6f, 4f, 3f, 55f), 12f);
                    fralda.Velocity = new Vector2(facing * 11f, 8f);
                    fralda.Gravity = 24f; fralda.Spin = fralda.Animated ? 0f : -facing * 360f; fralda.Life = 1.6f;
                    fralda.Face(facing);
                    fralda.HitSize = new Vector2(1.3f, 1.2f); fralda.Bounce = .25f;
                    fralda.SparkColor = new Color(.6f, .8f, .2f);
                    fralda.OnLand = _ => sfx.Plop();
                    fralda.OnHit = t =>
                    {
                        sfx.Fart();
                        t.Confuse(3f);
                        Sprite[] stink = PixelArt.Stink;
                        if (stink != null) Effects.Anim(stink, t.Hurtbox.center + Vector2.up * .4f, 2.8f, 1.1f, 12);
                        else for (int i = 0; i < 6; i++)
                        {
                            var puff = Effects.Sprite(PixelArt.Dust, t.Hurtbox.center + Random.insideUnitCircle * .6f, .6f, 12);
                            puff.Renderer.color = new Color(.55f, .75f, .2f, .9f);
                            puff.Velocity = Random.insideUnitCircle * 2.5f + Vector2.up; puff.Life = .7f; puff.EndScale = 2.4f;
                        }
                        Effects.Callout("QUE FEDOR!", t.Pos + Vector2.up * 5.2f, new Color(.7f, .9f, .3f), .8f);
                    };
                    sfx.Whoosh();
                    yield return Wait(.25f);
                    break;
                }

                case CandidateId.FlavioBolsonaro: // Chocolate Lavado: bombons que quicam e "lavam" o dano
                {
                    cd3 = 5f;
                    Shout(CharacterProfile.Special3Name(Candidate), new Color(.6f, .35f, .2f), 3);
                    anim.Play(FighterAnim.Special3, .5f, true);
                    yield return Wait(.2f);
                    for (int i = 0; i < 3; i++)
                    {
                        var bombom = Projectile.Spawn(this, arena, PixelArt.BonbonSpin, Pos + new Vector2(facing * 1f, 2.2f), .5f, new HitData(4f, 3.5f, 3f, 35f), 14f);
                        bombom.Velocity = new Vector2(facing * (7f + i * 2.5f), 7f + i);
                        bombom.Gravity = 26f; bombom.Spin = bombom.Animated ? 0f : -facing * 600f; bombom.Life = 1.8f;
                        bombom.Face(facing);
                        bombom.HitSize = new Vector2(.55f, .55f); bombom.Bounce = .6f;
                        bombom.SparkColor = new Color(.75f, .5f, .3f);
                        bombom.OnLand = _ => sfx.Plop();
                        bombom.OnHit = t => { sfx.Coin(); Heal(2f, "LAVADO! -2%", new Color(.9f, .7f, .45f)); };
                        sfx.Plop();
                        yield return Wait(.07f);
                    }
                    yield return Wait(.15f);
                    break;
                }

                case CandidateId.AugustoCury: // Palestra Motivacional: o rival perto dorme em pé
                {
                    cd3 = 9f;
                    Shout(CharacterProfile.Special3Name(Candidate), color, 3);
                    anim.Play(FighterAnim.Special3, 1f, true);
                    yield return Wait(.45f); // braços abertos: começa a "palestra"
                    // Onda azul no chão mostra o alcance da palestra.
                    StagePlatform here = floor ?? arena.SurfaceBelow(Pos.x, Pos.y) ?? arena.Ground;
                    var wave = Effects.Sprite(PixelArt.Shadow, new Vector2(Pos.x, here.Top + .03f), .5f, -4);
                    wave.Renderer.color = new Color(.45f, .7f, 1f, .8f);
                    wave.Life = .5f; wave.EndScale = 9f;
                    FighterController listener = Target;
                    if (listener != null && Mathf.Abs(listener.Pos.x - Pos.x) < 5.5f && Mathf.Abs(listener.Pos.y - Pos.y) < 1.5f && listener.Sleep(2.2f)) sfx.Yawn();
                    yield return Wait(.5f);
                    break;
                }
            }
            move = null;
        }

        private IEnumerator MansionDrop(Vector2 targetPos)
        {
            float x = targetPos.x;
            // A marca aparece onde o rival está pisando; a mansão atravessa as plataformas finas no caminho.
            StagePlatform surface = arena.SurfaceBelow(x, targetPos.y) ?? arena.Ground;
            var marker = Effects.Sprite(PixelArt.Shadow, new Vector2(x, surface.Top + .03f), .45f, -4);
            marker.Renderer.color = new Color(1f, .2f, .2f, .9f);
            marker.Life = .55f; marker.EndScale = 2.6f;
            sfx.FallWhistle();
            yield return Wait(.5f);
            Sprite[] fall = PixelArt.MansionFall;
            var m = Projectile.Spawn(this, arena, fall, new Vector2(x, 9.5f), 3f, new HitData(15f, 8f, 10f, 65f), 10f);
            m.Velocity = new Vector2(0f, -4f);
            m.Gravity = 45f; m.LandsAndBreaks = true; m.CrashesThrough = true; m.Life = 3f;
            m.HitSize = new Vector2(3.2f, 2.4f); m.HitOffset = new Vector2(0f, 1.2f);
            m.SparkColor = new Color(1f, .85f, .3f);
            m.OnLand = at =>
            {
                sfx.Crash();
                Sprite[] impact = PixelArt.MansionImpact;
                // Mesma escala de pixel da mansão que caiu: a casa não "cresce" ao tocar o chão.
                if (impact != null) Effects.Anim(impact, at, fall.Length > 1 ? 3f * TallestPixels(impact) / TallestPixels(fall) : 3.6f, .8f, 7);
            };
            m.OnHit = t => Effects.Callout("6 MILHÕES!", t.Pos + Vector2.up * 5.2f, new Color(1f, .85f, .3f), .9f);
        }

        // ---------------------------------------------------------------- utilitários de golpe

        private static float TallestPixels(Sprite[] frames)
        {
            float tallest = 1f;
            foreach (Sprite s in frames) if (s != null) tallest = Mathf.Max(tallest, s.rect.height);
            return tallest;
        }

        /// <summary>Nome da habilidade na tela + o grito gravado (se existir). slot: 1 = K, 2 = L, 3 = I.</summary>
        private void Shout(string text, Color color, int slot)
        {
            Effects.Callout(text.ToUpperInvariant() + "!", Pos + Vector2.up * 4.4f, Color.Lerp(color, Color.white, .35f), text.Length > 12 ? .8f : 1f, Effects.ShortTime);
            sfx.Special();
            sfx.Voice(this, CharacterProfile.SpecialVoice(Candidate, slot), true);
        }

        private IEnumerator Wait(float seconds)
        {
            while (seconds > 0f)
            {
                if (!Combat.Frozen) seconds -= Time.deltaTime;
                yield return null;
            }
        }

        /// <summary>Janela ativa de um golpe corpo a corpo: acerta no máximo uma vez.</summary>
        private IEnumerator Active(float seconds, Vector2 offset, Vector2 size, HitData hit, Color spark)
        {
            bool done = false;
            while (seconds > 0f)
            {
                if (!Combat.Frozen)
                {
                    if (!done) done = Strike(offset, size, hit, spark);
                    seconds -= Time.deltaTime;
                }
                yield return null;
            }
        }

        private IEnumerator Dash(float seconds, float speed, HitData hit, Color spark, System.Action<FighterController> onHit)
        {
            bool done = false;
            while (seconds > 0f)
            {
                if (!Combat.Frozen)
                {
                    vel = new Vector2(facing * speed, 0f);
                    if ((afterImageTimer -= Time.deltaTime) <= 0f) { afterImageTimer = .03f; Effects.AfterImage(anim.Renderer, new Color(spark.r, spark.g, spark.b, .6f)); }
                    if (!done && Strike(new Vector2(.5f, 1.6f), new Vector2(1.8f, 2.4f), hit, spark))
                    {
                        done = true;
                        onHit?.Invoke(Target);
                        vel.x *= .3f;
                        yield break;
                    }
                    seconds -= Time.deltaTime;
                }
                yield return null;
            }
        }

        private bool Strike(Vector2 offset, Vector2 size, HitData hit, Color spark)
        {
            if (Target == null) return false;
            Vector2 center = Pos + new Vector2(offset.x * facing, offset.y);
            var box = new Rect(center - size * .5f, size);
            Rect hurt = Target.Hurtbox;
            if (!box.Overlaps(hurt)) return false;
            if (!Target.TakeHit(hit, facing, center)) return false;
            Effects.Spark(new Vector2(Mathf.Clamp(center.x, hurt.xMin, hurt.xMax), Mathf.Clamp(center.y, hurt.yMin, hurt.yMax)), spark, 1.1f);
            return true;
        }

        private void CancelMove()
        {
            if (move != null) StopCoroutine(move);
            move = null;
        }
    }
}
