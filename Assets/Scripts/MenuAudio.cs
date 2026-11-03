using UnityEngine;

namespace TSEFatal
{
    public class MenuAudio : MonoBehaviour
    {
        private AudioSource sfx;
        private AudioSource music;
        private AudioClip blip;
        private AudioClip confirm;

        public static MenuAudio Ensure(GameObject owner)
        {
            MenuAudio existing = Object.FindAnyObjectByType<MenuAudio>();
            if (existing != null) return existing;
            return owner.AddComponent<MenuAudio>();
        }

        private void Awake()
        {
            sfx = gameObject.AddComponent<AudioSource>();
            music = gameObject.AddComponent<AudioSource>();
            music.loop = true; music.volume = .08f;
            blip = MakeTone("Menu Blip", 880f, .07f, .18f);
            confirm = MakeTone("Menu Confirm", 520f, .16f, .24f);
            music.clip = MakeArcadeLoop();
            music.Play();
        }

        public void Blip() => sfx.PlayOneShot(blip);
        public void Confirm() => sfx.PlayOneShot(confirm);

        private static AudioClip MakeTone(string name, float frequency, float duration, float volume)
        {
            int rate = 44100; int count = Mathf.CeilToInt(rate * duration); float[] data = new float[count];
            for (int i = 0; i < count; i++) data[i] = Mathf.Sign(Mathf.Sin(2f * Mathf.PI * frequency * i / rate)) * volume * (1f - i / (float)count);
            AudioClip clip = AudioClip.Create(name, count, 1, rate, false); clip.SetData(data, 0); return clip;
        }

        private static AudioClip MakeArcadeLoop()
        {
            int rate = 22050; int count = rate * 4; float[] data = new float[count];
            int[] notes = { 196, 247, 294, 247, 220, 262, 330, 262 };
            for (int i = 0; i < count; i++)
            {
                int step = Mathf.FloorToInt(i / (rate * .5f)) % notes.Length;
                float phase = (i % Mathf.FloorToInt(rate * .5f)) / (float)rate;
                float envelope = Mathf.Clamp01(1f - phase * 2.4f);
                data[i] = Mathf.Sign(Mathf.Sin(2f * Mathf.PI * notes[step] * i / rate)) * .045f * envelope;
            }
            AudioClip clip = AudioClip.Create("Placeholder Arcade Loop", count, 1, rate, false); clip.SetData(data, 0); return clip;
        }
    }
}
