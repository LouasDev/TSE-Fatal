using UnityEngine;

namespace TSEFatal
{
    /// <summary>
    /// Áudio do jogo, todo sintetizado em código (sem arquivos): efeitos de menu e de luta, sons
    /// temáticos das habilidades e duas músicas chiptune (menu e batalha).
    /// Vários AudioSources em rodízio permitem variar o tom de cada golpe sem cortar o anterior.
    /// </summary>
    public class MenuAudio : MonoBehaviour
    {
        private const int Rate = 44100;
        private const int Voices = 8;

        private AudioSource[] voices;
        private int nextVoice;
        private AudioSource music;
        private AudioClip menuLoop, battleLoop;

        private AudioClip blip, confirm, swing, hit, heavyHit, ko, jump, whoosh, special, denied, beep, beepFinal;
        private AudioClip cashRegister, coin, bleep, zap, plim, roar, fallWhistle, crash, fart, plop, cheer, land, pickup, paper, siren, robot, yawn;

        public static MenuAudio Ensure(GameObject owner)
        {
            MenuAudio existing = Object.FindAnyObjectByType<MenuAudio>();
            if (existing != null) return existing;
            return owner.AddComponent<MenuAudio>();
        }

        private void Awake()
        {
            voices = new AudioSource[Voices];
            for (int i = 0; i < Voices; i++) { voices[i] = gameObject.AddComponent<AudioSource>(); voices[i].playOnAwake = false; }
            music = gameObject.AddComponent<AudioSource>();
            music.loop = true; music.volume = .12f;

            blip = MakeTone("Menu Blip", 880f, .07f, .18f);
            confirm = MakeTone("Menu Confirm", 520f, .16f, .24f);
            swing = MakeNoise("Swing", .07f, .10f, 0.55f);
            whoosh = MakeNoise("Whoosh", .14f, .08f, 0.35f);
            hit = MakeImpact("Hit", 180f, .09f, .32f);
            heavyHit = MakeImpact("Heavy Hit", 110f, .18f, .42f);
            ko = MakeSweep("KO", 700f, 70f, .55f, .30f);
            jump = MakeSweep("Jump", 300f, 620f, .08f, .10f);
            special = MakeSweep("Special", 440f, 990f, .16f, .14f);
            denied = MakeTone("Denied", 140f, .18f, .16f);
            beep = MakeTone("Urna Beep", 1046f, .12f, .16f);
            beepFinal = MakeUrna();

            cashRegister = MakeCashRegister();
            coin = MakeBell("Moeda", 2637f, 3951f, .22f, .16f);
            bleep = MakeSine("Bip de censura", 1000f, .38f, .2f);
            zap = MakeZap();
            plim = MakePlim();
            roar = MakeRoar();
            fallWhistle = MakeWhistle();
            crash = MakeCrash();
            fart = MakeFart();
            plop = MakePlop();
            cheer = MakeCrowd();
            land = MakeImpact("Pouso", 70f, .07f, .22f);
            pickup = MakeArpeggio();
            paper = MakeNoise("Papel", .05f, .08f, .9f);
            siren = MakeSiren();
            robot = MakeRobot();
            yawn = MakeYawn();

            menuLoop = MakeMenuLoop();
            battleLoop = MakeBattleLoop();
            PlayMenuMusic();
        }

        // ---------------------------------------------------------------- API

        public void Blip() => Play(blip);
        public void Confirm() => Play(confirm);
        public void Swing() => Play(swing, .9f, .15f);
        public void Whoosh() => Play(whoosh, 1f, .1f);
        public void Hit(bool heavy) => Play(heavy ? heavyHit : hit, 1f, .12f);
        public void KO() { Play(ko); Play(crash, .7f); Play(cheer, .9f); }
        public void Jump() => Play(jump, .6f, .08f);
        public void Special() => Play(special, 1f, .05f);
        public void Denied() { Play(denied); Play(bleep, .5f); }
        /// <summary>Bip curto da contagem; final = som de "CONFIRMA" da urna.</summary>
        public void Beep(bool final) => Play(final ? beepFinal : beep);

        /// <summary>"Cha-ching!" de caixa registradora (Mensalão).</summary>
        public void CashRegister() => Play(cashRegister, .9f, .04f);
        public void Coin() => Play(coin, .8f, .2f);
        /// <summary>O bip clássico da TV que censura palavrão.</summary>
        public void Bleep() => Play(bleep);
        /// <summary>Acorde elétrico do "Faz o L".</summary>
        public void Zap() => Play(zap);
        /// <summary>Notificação de celular (Corte Viral).</summary>
        public void Plim() => Play(plim, .9f, .03f);
        public void Roar() => Play(roar, 1f, .08f);
        /// <summary>Assobio de algo caindo do céu (Mansão).</summary>
        public void FallWhistle() => Play(fallWhistle, .8f);
        public void Crash() => Play(crash, 1f, .08f);
        public void Fart() => Play(fart, 1f, .15f);
        public void Plop() => Play(plop, .9f, .2f);
        public void Cheer() => Play(cheer);
        public void Land() => Play(land, .7f, .15f);
        public void Pickup() => Play(pickup);
        public void Paper() => Play(paper, 1f, .2f);
        /// <summary>Sirene de dois tons (Drone com Sirene).</summary>
        public void Siren() => Play(siren, .8f, .04f);
        /// <summary>"Bip-bop" de robozinho (Ministério da IA).</summary>
        public void Robot() => Play(robot, .9f, .1f);
        /// <summary>Bocejo descendente (Palestra Motivacional).</summary>
        public void Yawn() => Play(yawn, .9f, .05f);

        public void PlayMenuMusic() => SetMusic(menuLoop, .10f);
        public void PlayBattleMusic() => SetMusic(battleLoop, .13f);

        /// <summary>Última vida em jogo: a música acelera um pouco.</summary>
        public void SetIntensity(bool tense) => music.pitch = tense ? 1.1f : 1f;

        // ---------------------------------------------------------------- dublagem
        // Áudios gravados ficam em Resources/Audio/Vozes/{chave}.ogg (ou .wav/.mp3).
        // Variações da mesma fala: {chave}_2, {chave}_3... (o jogo sorteia uma).
        // Lista de chaves e o que gritar em cada uma: Docs/DUBLAGEM.md.

        private const string VoiceFolder = "Audio/Vozes/";
        private readonly System.Collections.Generic.Dictionary<string, AudioClip[]> voiceClips = new System.Collections.Generic.Dictionary<string, AudioClip[]>();
        private readonly System.Collections.Generic.Dictionary<Object, AudioSource> speakers = new System.Collections.Generic.Dictionary<Object, AudioSource>();
        private float musicVolume = .12f;

        /// <summary>True se existe pelo menos um áudio gravado para a chave.</summary>
        public bool HasVoice(string key) => VoiceClips(key).Length > 0;

        /// <summary>
        /// Toca a fala de <paramref name="speaker"/> (cada lutador tem o próprio canal, então os dois podem
        /// falar juntos). Sem <paramref name="interrupt"/>, não corta uma fala que ainda está tocando.
        /// Devolve false quando não há áudio gravado para a chave.
        /// </summary>
        public bool Voice(Object speaker, string key, bool interrupt = false, float volume = 1f)
        {
            AudioClip[] clips = VoiceClips(key);
            if (clips.Length == 0) return false;
            AudioSource source = Speaker(speaker);
            if (source.isPlaying && !interrupt) return true;
            source.clip = clips[Random.Range(0, clips.Length)];
            source.volume = volume;
            source.Play();
            return true;
        }

        /// <summary>Tenta as chaves em ordem e toca a primeira que tiver áudio.</summary>
        public bool Voice(Object speaker, bool interrupt, params string[] keys)
        {
            foreach (string key in keys) if (HasVoice(key)) return Voice(speaker, key, interrupt);
            return false;
        }

        private AudioClip[] VoiceClips(string key)
        {
            if (voiceClips.TryGetValue(key, out AudioClip[] cached)) return cached;
            var list = new System.Collections.Generic.List<AudioClip>();
            AudioClip first = Resources.Load<AudioClip>(VoiceFolder + key);
            if (first != null) list.Add(first);
            for (int i = 2; i <= 9; i++)
            {
                AudioClip variant = Resources.Load<AudioClip>($"{VoiceFolder}{key}_{i}");
                if (variant != null) list.Add(variant);
            }
            AudioClip[] clips = list.ToArray();
            voiceClips[key] = clips;
            return clips;
        }

        private AudioSource Speaker(Object speaker)
        {
            if (speakers.TryGetValue(speaker, out AudioSource source) && source != null) return source;
            // Lutadores de lutas anteriores já foram destruídos: reaproveita o canal deles.
            Object stale = null;
            bool found = false;
            // "== null" aqui é o da Unity: vale para objetos já destruídos.
            foreach (var pair in speakers) if (pair.Key == null) { stale = pair.Key; source = pair.Value; found = true; break; }
            if (found) speakers.Remove(stale);
            if (source == null) { source = gameObject.AddComponent<AudioSource>(); source.playOnAwake = false; }
            source.Stop();
            speakers[speaker] = source;
            return source;
        }

        private void Update()
        {
            // Música abaixa enquanto alguém fala, para a dublagem aparecer.
            bool talking = false;
            foreach (AudioSource s in speakers.Values) if (s != null && s.isPlaying) { talking = true; break; }
            float target = musicVolume * (talking ? .35f : 1f);
            music.volume = Mathf.MoveTowards(music.volume, target, Time.unscaledDeltaTime * (talking ? 1.5f : .4f));
        }

        private void SetMusic(AudioClip clip, float volume)
        {
            music.pitch = 1f;
            musicVolume = volume;
            music.volume = volume;
            if (music.clip == clip && music.isPlaying) return;
            music.clip = clip;
            music.Play();
        }

        private void Play(AudioClip clip, float volume = 1f, float pitchVariation = 0f)
        {
            AudioSource source = voices[nextVoice];
            nextVoice = (nextVoice + 1) % voices.Length;
            source.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
            source.PlayOneShot(clip, volume);
        }

        // ---------------------------------------------------------------- síntese básica

        private static AudioClip Clip(string name, float[] data, int rate = Rate)
        {
            AudioClip clip = AudioClip.Create(name, data.Length, 1, rate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private static float Square(float phase) => Mathf.Sin(phase) >= 0f ? 1f : -1f;
        private static float Triangle(float phase) => Mathf.PingPong(phase / Mathf.PI, 2f) - 1f;

        private static AudioClip MakeNoise(string name, float duration, float volume, float lowpass)
        {
            int count = Mathf.CeilToInt(Rate * duration); float[] data = new float[count];
            float last = 0f; var rng = new System.Random(name.GetHashCode());
            for (int i = 0; i < count; i++)
            {
                float n = (float)rng.NextDouble() * 2f - 1f;
                last = Mathf.Lerp(last, n, lowpass);
                float t = i / (float)count;
                data[i] = last * volume * Mathf.Sin(t * Mathf.PI);
            }
            return Clip(name, data);
        }

        private static AudioClip MakeImpact(string name, float frequency, float duration, float volume)
        {
            int count = Mathf.CeilToInt(Rate * duration); float[] data = new float[count];
            var rng = new System.Random(name.GetHashCode());
            for (int i = 0; i < count; i++)
            {
                float t = i / (float)count;
                float env = Mathf.Pow(1f - t, 2.2f);
                float tone = Mathf.Sin(2f * Mathf.PI * frequency * (1f - t * .5f) * i / Rate);
                float noise = ((float)rng.NextDouble() * 2f - 1f) * Mathf.Max(0f, 1f - t * 3f);
                data[i] = (tone * .7f + noise * .6f) * env * volume;
            }
            return Clip(name, data);
        }

        private static AudioClip MakeSweep(string name, float from, float to, float duration, float volume)
        {
            int count = Mathf.CeilToInt(Rate * duration); float[] data = new float[count];
            float phase = 0f;
            for (int i = 0; i < count; i++)
            {
                float t = i / (float)count;
                phase += 2f * Mathf.PI * Mathf.Lerp(from, to, t) / Rate;
                data[i] = Square(phase) * volume * (1f - t);
            }
            return Clip(name, data);
        }

        private static AudioClip MakeTone(string name, float frequency, float duration, float volume)
        {
            int count = Mathf.CeilToInt(Rate * duration); float[] data = new float[count];
            for (int i = 0; i < count; i++) data[i] = Square(2f * Mathf.PI * frequency * i / Rate) * volume * (1f - i / (float)count);
            return Clip(name, data);
        }

        private static AudioClip MakeSine(string name, float frequency, float duration, float volume)
        {
            int count = Mathf.CeilToInt(Rate * duration); float[] data = new float[count];
            for (int i = 0; i < count; i++)
            {
                float t = i / (float)count;
                float env = Mathf.Clamp01(t * 60f) * Mathf.Clamp01((1f - t) * 30f); // liga/desliga seco, sem estalo
                data[i] = Mathf.Sin(2f * Mathf.PI * frequency * i / Rate) * volume * env;
            }
            return Clip(name, data);
        }

        /// <summary>Sino metálico: duas parciais inarmônicas com decaimento.</summary>
        private static AudioClip MakeBell(string name, float f1, float f2, float duration, float volume)
        {
            int count = Mathf.CeilToInt(Rate * duration); float[] data = new float[count];
            for (int i = 0; i < count; i++)
            {
                float t = i / (float)Rate;
                float env = Mathf.Exp(-t * 14f);
                data[i] = (Mathf.Sin(2f * Mathf.PI * f1 * t) * .6f + Mathf.Sin(2f * Mathf.PI * f2 * t) * .4f) * env * volume;
            }
            return Clip(name, data);
        }

        // ---------------------------------------------------------------- sons temáticos

        /// <summary>Trilo agudo e longo inspirado no fim da votação na urna eletrônica.</summary>
        private static AudioClip MakeUrna()
        {
            int count = Mathf.CeilToInt(Rate * .9f); float[] data = new float[count];
            for (int i = 0; i < count; i++)
            {
                float t = i / (float)Rate;
                float f = (Mathf.FloorToInt(t / .045f) % 2 == 0) ? 1975f : 1568f;
                float env = t < .7f ? 1f : 1f - (t - .7f) / .2f;
                data[i] = Square(2f * Mathf.PI * f * t) * .11f * env;
            }
            return Clip("Urna Confirma", data);
        }

        private static AudioClip MakeCashRegister()
        {
            int count = Mathf.CeilToInt(Rate * .55f); float[] data = new float[count];
            var rng = new System.Random(7);
            for (int i = 0; i < count; i++)
            {
                float t = i / (float)Rate;
                float clack = t < .04f ? ((float)rng.NextDouble() * 2f - 1f) * (1f - t / .04f) * .35f : 0f; // gaveta
                float ding = 0f;
                if (t > .05f)
                {
                    float u = t - .05f;
                    ding = (Mathf.Sin(2f * Mathf.PI * 2093f * u) * .5f + Mathf.Sin(2f * Mathf.PI * 3136f * u) * .35f + Mathf.Sin(2f * Mathf.PI * 4186f * u) * .15f) * Mathf.Exp(-u * 6f);
                }
                data[i] = (clack + ding * .3f);
            }
            return Clip("Caixa Registradora", data);
        }

        private static AudioClip MakeZap()
        {
            int count = Mathf.CeilToInt(Rate * .5f); float[] data = new float[count];
            var rng = new System.Random(11);
            float p1 = 0f, p2 = 0f, p3 = 0f;
            for (int i = 0; i < count; i++)
            {
                float t = i / (float)count;
                float bend = 1f + t * .6f;
                p1 += 2f * Mathf.PI * 110f * bend / Rate;
                p2 += 2f * Mathf.PI * 165f * bend / Rate; // quinta: acorde "de guitarra"
                p3 += 2f * Mathf.PI * 220f * bend / Rate;
                float crackle = ((float)rng.NextDouble() * 2f - 1f) * (rng.NextDouble() < .2 ? 1f : .15f);
                float env = Mathf.Clamp01(t * 40f) * (1f - t);
                data[i] = ((Square(p1) + Square(p2) * .8f + Square(p3) * .6f) * .09f + crackle * .12f) * env;
            }
            return Clip("Zap do L", data);
        }

        private static AudioClip MakePlim()
        {
            int count = Mathf.CeilToInt(Rate * .35f); float[] data = new float[count];
            for (int i = 0; i < count; i++)
            {
                float t = i / (float)Rate;
                float f = t < .09f ? 1318f : 1760f;
                float u = t < .09f ? t : t - .09f;
                data[i] = Mathf.Sin(2f * Mathf.PI * f * t) * Mathf.Exp(-u * 18f) * .22f;
            }
            return Clip("Plim", data);
        }

        private static AudioClip MakeRoar()
        {
            int count = Mathf.CeilToInt(Rate * .7f); float[] data = new float[count];
            var rng = new System.Random(3);
            float last = 0f, phase = 0f;
            for (int i = 0; i < count; i++)
            {
                float t = i / (float)count;
                float n = (float)rng.NextDouble() * 2f - 1f;
                last = Mathf.Lerp(last, n, .18f);
                phase += 2f * Mathf.PI * Mathf.Lerp(160f, 70f, t) * (1f + .08f * Mathf.Sin(t * 70f)) / Rate;
                float env = Mathf.Clamp01(t * 8f) * Mathf.Pow(1f - t, 1.4f);
                data[i] = (Mathf.Sin(phase) * .5f + Square(phase) * .2f) * (.6f + last * 1.6f) * env * .35f;
            }
            return Clip("Rugido", data);
        }

        private static AudioClip MakeWhistle()
        {
            int count = Mathf.CeilToInt(Rate * .55f); float[] data = new float[count];
            float phase = 0f;
            for (int i = 0; i < count; i++)
            {
                float t = i / (float)count;
                phase += 2f * Mathf.PI * Mathf.Lerp(1600f, 380f, t * t) / Rate;
                data[i] = Mathf.Sin(phase) * .16f * Mathf.Clamp01(t * 10f);
            }
            return Clip("Assobio de queda", data);
        }

        private static AudioClip MakeCrash()
        {
            int count = Mathf.CeilToInt(Rate * .6f); float[] data = new float[count];
            var rng = new System.Random(5);
            float low = 0f;
            for (int i = 0; i < count; i++)
            {
                float t = i / (float)count;
                float n = (float)rng.NextDouble() * 2f - 1f;
                low = Mathf.Lerp(low, n, .08f);
                float boom = Mathf.Sin(2f * Mathf.PI * 55f * i / Rate) * Mathf.Exp(-t * 6f);
                data[i] = (low * 1.4f + n * .25f * Mathf.Exp(-t * 9f) + boom * .7f) * Mathf.Pow(1f - t, 1.5f) * .4f;
            }
            return Clip("Desabamento", data);
        }

        private static AudioClip MakeFart()
        {
            int count = Mathf.CeilToInt(Rate * .45f); float[] data = new float[count];
            var rng = new System.Random(13);
            float phase = 0f;
            for (int i = 0; i < count; i++)
            {
                float t = i / (float)count;
                float f = Mathf.Lerp(105f, 62f, t) * (1f + .25f * Mathf.Sin(t * 95f)) + (float)rng.NextDouble() * 18f;
                phase += 2f * Mathf.PI * f / Rate;
                float saw = (phase / Mathf.PI) % 2f - 1f;
                data[i] = saw * .3f * Mathf.Clamp01(t * 25f) * Mathf.Pow(1f - t, .7f);
            }
            return Clip("Pum", data);
        }

        private static AudioClip MakeSiren()
        {
            int count = Mathf.CeilToInt(Rate * .9f); float[] data = new float[count];
            float phase = 0f;
            for (int i = 0; i < count; i++)
            {
                float t = i / (float)Rate;
                float f = (Mathf.FloorToInt(t / .225f) % 2 == 0) ? 960f : 720f;
                phase += 2f * Mathf.PI * f / Rate;
                float env = Mathf.Clamp01(t * 30f) * Mathf.Clamp01((.9f - t) * 8f);
                data[i] = (Square(phase) * .5f + Mathf.Sin(phase) * .5f) * .1f * env;
            }
            return Clip("Sirene", data);
        }

        private static AudioClip MakeRobot()
        {
            float[] notes = { 620f, 930f, 470f };
            int per = Mathf.CeilToInt(Rate * .06f);
            float[] data = new float[per * notes.Length];
            for (int i = 0; i < data.Length; i++)
            {
                float f = notes[i / per];
                float u = (i % per) / (float)per;
                data[i] = Square(2f * Mathf.PI * f * i / Rate) * .09f * (1f - u * .6f);
            }
            return Clip("Robozinho", data);
        }

        private static AudioClip MakeYawn()
        {
            int count = Mathf.CeilToInt(Rate * .7f); float[] data = new float[count];
            float phase = 0f;
            for (int i = 0; i < count; i++)
            {
                float t = i / (float)count;
                phase += 2f * Mathf.PI * Mathf.Lerp(420f, 150f, t) * (1f + .03f * Mathf.Sin(t * 40f)) / Rate;
                float env = Mathf.Clamp01(t * 6f) * Mathf.Pow(1f - t, .8f);
                data[i] = (Mathf.Sin(phase) * .7f + Triangle(phase) * .3f) * .18f * env;
            }
            return Clip("Bocejo", data);
        }

        private static AudioClip MakePlop()
        {
            int count = Mathf.CeilToInt(Rate * .12f); float[] data = new float[count];
            float phase = 0f;
            for (int i = 0; i < count; i++)
            {
                float t = i / (float)count;
                phase += 2f * Mathf.PI * Mathf.Lerp(900f, 180f, t) / Rate;
                data[i] = Mathf.Sin(phase) * .3f * (1f - t);
            }
            return Clip("Plop", data);
        }

        /// <summary>Torcida: ruído filtrado com "ondas" de vozes que sobe e desce.</summary>
        private static AudioClip MakeCrowd()
        {
            int count = Mathf.CeilToInt(Rate * 1.3f); float[] data = new float[count];
            var rng = new System.Random(17);
            float a = 0f, b = 0f;
            for (int i = 0; i < count; i++)
            {
                float t = i / (float)count;
                float n = (float)rng.NextDouble() * 2f - 1f;
                a = Mathf.Lerp(a, n, .12f);
                b = Mathf.Lerp(b, a, .3f);
                float swell = Mathf.Clamp01(t * 6f) * Mathf.Pow(1f - t, 1.2f);
                float voices = .7f + .3f * Mathf.Sin(t * 37f) * Mathf.Sin(t * 13f);
                data[i] = (a - b) * 2.2f * swell * voices * .5f;
            }
            return Clip("Torcida", data);
        }

        private static AudioClip MakeArpeggio()
        {
            float[] notes = { 784f, 988f, 1175f, 1568f };
            int per = Mathf.CeilToInt(Rate * .06f);
            float[] data = new float[per * notes.Length + per * 2];
            for (int i = 0; i < data.Length; i++)
            {
                int n = Mathf.Min(notes.Length - 1, i / per);
                float t = i / (float)data.Length;
                data[i] = Square(2f * Mathf.PI * notes[n] * i / Rate) * .1f * (1f - t);
            }
            return Clip("Coleta", data);
        }

        // ---------------------------------------------------------------- músicas

        private const int MusicRate = 22050;

        private static AudioClip MakeMenuLoop()
        {
            // Marcha lenta em lá menor: baixo + melodia simples.
            int[] lead = { 69, -1, 72, 76, 74, 72, 71, -1, 69, -1, 72, 76, 79, 77, 76, -1, 74, -1, 77, 81, 79, 77, 76, 74, 72, 71, 72, 74, 76, -1, -1, -1 };
            int[] bass = { 45, 45, 41, 41, 43, 43, 40, 40 };
            return Song("Música do Menu", 112f, lead, bass, 4, .05f, false);
        }

        private static AudioClip MakeBattleLoop()
        {
            // Rápida, em ré menor, com bateria: clima de debate pegando fogo.
            int[] lead =
            {
                62, -1, 65, 62, 69, -1, 67, 65, 64, -1, 65, 67, 69, -1, -1, -1,
                62, -1, 65, 62, 70, -1, 69, 67, 69, 67, 65, 64, 62, -1, -1, -1,
                74, -1, 72, 70, 69, -1, 67, 65, 67, -1, 65, 64, 65, -1, 62, -1,
                70, 69, 67, 65, 64, 65, 67, 69, 74, -1, 73, -1, 74, -1, -1, -1,
            };
            int[] bass = { 38, 38, 34, 34, 36, 36, 33, 33, 38, 38, 34, 34, 36, 36, 33, 37 };
            return Song("Música da Batalha", 150f, lead, bass, 2, .055f, true);
        }

        /// <summary>
        /// Sequenciador mínimo: lead em colcheias (quadrada), baixo triangular (um valor a cada
        /// bassEvery colcheias, tocado em colcheias) e, opcionalmente, bumbo/caixa/chimbal de ruído.
        /// </summary>
        private static AudioClip Song(string name, float bpm, int[] lead, int[] bass, int bassEvery, float volume, bool drums)
        {
            int stepLen = Mathf.RoundToInt(MusicRate * 60f / bpm / 2f); // colcheia
            int steps = lead.Length;
            float[] data = new float[stepLen * steps];
            var rng = new System.Random(21);
            for (int s = 0; s < steps; s++)
            {
                int note = lead[s];
                int bassNote = bass[(s / bassEvery) % bass.Length];
                for (int k = 0; k < stepLen; k++)
                {
                    int i = s * stepLen + k;
                    float t = i / (float)MusicRate;
                    float local = k / (float)stepLen;
                    float sample = 0f;
                    if (note > 0)
                    {
                        float f = Midi(note);
                        float vib = 1f + .006f * Mathf.Sin(t * 30f);
                        sample += Square(2f * Mathf.PI * f * vib * t) * .55f * Mathf.Clamp01(1.2f - local * .7f);
                    }
                    sample += Triangle(2f * Mathf.PI * Midi(bassNote) * t) * .9f * Mathf.Clamp01(1f - local * .5f);
                    if (drums)
                    {
                        float noise = (float)rng.NextDouble() * 2f - 1f;
                        int beatPos = s % 4;
                        if (beatPos == 0) sample += Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(150f, 45f, local * 2f) * local * stepLen / MusicRate) * Mathf.Exp(-local * 7f) * 1.1f; // bumbo
                        if (beatPos == 2) sample += noise * Mathf.Exp(-local * 9f) * .55f;  // caixa
                        sample += noise * Mathf.Exp(-local * 40f) * .18f;                   // chimbal
                    }
                    data[i] = sample * volume;
                }
            }
            return Clip(name, data, MusicRate);
        }

        private static float Midi(int note) => 440f * Mathf.Pow(2f, (note - 69) / 12f);
    }
}
