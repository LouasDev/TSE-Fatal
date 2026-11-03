using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TSEFatal
{
    public class CharacterSelectController : MonoBehaviour
    {
        private readonly CandidateId[] candidates = { CandidateId.Lula, CandidateId.RenanSantos, CandidateId.FlavioBolsonaro };
        private readonly List<Image> frames = new List<Image>();
        private GameFlow flow;
        private GameObject root;
        private Image largePortrait;
        private Text nameText;
        private Text styleText;
        private Text powersText;
        private Text versusText;
        private int selected;
        private bool visible;
        private MenuAudio audioMenu;

        public void Initialize(GameFlow owner)
        {
            flow = owner;
            audioMenu = MenuAudio.Ensure(gameObject);
        }

        public void Show()
        {
            if (root == null) Build();
            root.SetActive(true);
            selected = 0;
            visible = true;
            Refresh();
        }

        public void Hide()
        {
            if (root != null) root.SetActive(false);
            visible = false;
        }

        private void Build()
        {
            root = ArcadeUI.CreateRoot("Character Select Canvas");
            root.GetComponent<Canvas>().sortingOrder = 21;
            Image bg = ArcadeUI.Panel(root.transform, "Seleção Fundo", new Color(.015f, .025f, .08f, .98f)); ArcadeUI.Stretch(bg.rectTransform);
            Text header = ArcadeUI.Text(root.transform, "Header", "SELECIONE SEU CANDIDATO", 21, new Color(1f, .8f, .2f), TextAnchor.MiddleCenter);
            ArcadeUI.Anchor(header.rectTransform, new Vector2(.5f, .92f), new Vector2(.5f, .92f), Vector2.zero, new Vector2(450, 28)); header.gameObject.AddComponent<Outline>().effectColor = new Color(.4f, .04f, .08f);

            largePortrait = ArcadeUI.Panel(root.transform, "Sprite Idle", Color.white);
            ArcadeUI.Anchor(largePortrait.rectTransform, new Vector2(.22f, .47f), new Vector2(.22f, .47f), Vector2.zero, new Vector2(180, 190));
            nameText = ArcadeUI.Text(root.transform, "Nome", "", 20, Color.white, TextAnchor.MiddleCenter);
            ArcadeUI.Anchor(nameText.rectTransform, new Vector2(.22f, .12f), new Vector2(.22f, .12f), Vector2.zero, new Vector2(190, 25));

            Image info = ArcadeUI.Panel(root.transform, "Info", new Color(.06f, .10f, .20f, .9f));
            ArcadeUI.Anchor(info.rectTransform, new Vector2(.52f, .48f), new Vector2(.52f, .48f), Vector2.zero, new Vector2(190, 170)); info.gameObject.AddComponent<Outline>().effectColor = new Color(.25f, .45f, .8f);
            styleText = ArcadeUI.Text(info.transform, "Estilo", "", 12, Color.white, TextAnchor.UpperCenter);
            ArcadeUI.Anchor(styleText.rectTransform, new Vector2(.5f, .84f), new Vector2(.5f, .84f), Vector2.zero, new Vector2(174, 38));
            powersText = ArcadeUI.Text(info.transform, "Poderes", "", 12, new Color(1f, .85f, .35f), TextAnchor.UpperLeft);
            ArcadeUI.Anchor(powersText.rectTransform, new Vector2(.5f, .31f), new Vector2(.5f, .31f), Vector2.zero, new Vector2(160, 68));
            CreateStat(info.transform, "ATAQUE", 100); CreateStat(info.transform, "DEFESA", 76); CreateStat(info.transform, "VELOCIDADE", 88);

            Image rival = ArcadeUI.Panel(root.transform, "Rival Silhueta", Color.black);
            ArcadeUI.Anchor(rival.rectTransform, new Vector2(.84f, .47f), new Vector2(.84f, .47f), Vector2.zero, new Vector2(105, 176));
            Text question = ArcadeUI.Text(rival.transform, "Pergunta", "?", 66, new Color(.22f, .25f, .34f), TextAnchor.MiddleCenter); ArcadeUI.Stretch(question.rectTransform);
            Text rivalLabel = ArcadeUI.Text(root.transform, "Rival", "RIVAL: ?", 14, new Color(.7f, .75f, .9f), TextAnchor.MiddleCenter);
            ArcadeUI.Anchor(rivalLabel.rectTransform, new Vector2(.84f, .13f), new Vector2(.84f, .13f), Vector2.zero, new Vector2(120, 22));

            for (int i = 0; i < candidates.Length; i++)
            {
                Image frame = ArcadeUI.Panel(root.transform, "Moldura " + i, new Color(.09f, .12f, .22f));
                ArcadeUI.Anchor(frame.rectTransform, new Vector2(.30f + i * .20f, .77f), new Vector2(.30f + i * .20f, .77f), Vector2.zero, new Vector2(82, 68));
                frame.gameObject.AddComponent<Outline>().effectColor = new Color(.32f, .4f, .68f);
                Image portrait = ArcadeUI.Panel(frame.transform, "Retrato", Color.white);
                ArcadeUI.Stretch(portrait.rectTransform); portrait.rectTransform.offsetMin = new Vector2(5, 5); portrait.rectTransform.offsetMax = new Vector2(-5, -5);
                portrait.sprite = SpriteFor(candidates[i]); portrait.preserveAspect = true;
                frames.Add(frame);
            }
            Text help = ArcadeUI.Text(root.transform, "Ajuda", "← → / A D: ESCOLHER     ENTER: CONFIRMAR     ESC: MENU", 10, new Color(.75f, .82f, 1f), TextAnchor.MiddleCenter);
            ArcadeUI.Anchor(help.rectTransform, new Vector2(.5f, .035f), new Vector2(.5f, .035f), Vector2.zero, new Vector2(460, 18));
        }

        private void CreateStat(Transform parent, string label, int value)
        {
            int index = parent.childCount;
            Text title = ArcadeUI.Text(parent, label, label, 9, new Color(.7f, .8f, 1f), TextAnchor.MiddleLeft);
            title.rectTransform.anchoredPosition = new Vector2(-78, 5 - (index - 2) * 19); title.rectTransform.sizeDelta = new Vector2(58, 13);
            Image track = ArcadeUI.Panel(parent, label + " Track", new Color(.015f, .02f, .06f));
            track.rectTransform.anchoredPosition = new Vector2(34, 5 - (index - 2) * 19); track.rectTransform.sizeDelta = new Vector2(82, 8);
            Image fill = ArcadeUI.Panel(track.transform, "Fill", new Color(1f, .65f, .12f));
            fill.rectTransform.anchorMin = new Vector2(0, 0); fill.rectTransform.anchorMax = new Vector2(value / 100f, 1); fill.rectTransform.offsetMin = fill.rectTransform.offsetMax = Vector2.zero;
        }

        private void Update()
        {
            if (!visible) return;
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) Change(1);
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) Change(-1);
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0)) StartCoroutine(ShowVsThenFight());
            if (Input.GetKeyDown(KeyCode.Escape)) { Hide(); flow.ReturnToMainMenu(); }
        }

        private void Change(int delta)
        {
            selected = (selected + delta + candidates.Length) % candidates.Length;
            audioMenu.Blip();
            Refresh();
        }

        private void Refresh()
        {
            CandidateId candidate = candidates[selected];
            Sprite sprite = SpriteFor(candidate);
            largePortrait.sprite = sprite; largePortrait.preserveAspect = true;
            nameText.text = CharacterProfile.DisplayName(candidate);
            nameText.color = CharacterProfile.Tint(candidate);
            styleText.text = CharacterProfile.Flavor(candidate);
            powersText.text = "PODER 1: " + CharacterProfile.SpecialName(candidate) + "\nPODER 2: " + SecondaryPower(candidate);
            for (int i = 0; i < frames.Count; i++)
            {
                bool chosen = i == selected;
                frames[i].color = chosen ? CharacterProfile.Tint(candidates[i]) : new Color(.09f, .12f, .22f);
                frames[i].transform.localScale = chosen ? Vector3.one * 1.08f : Vector3.one;
            }
        }

        private IEnumerator ShowVsThenFight()
        {
            visible = false;
            audioMenu.Confirm();
            GameObject vsRoot = ArcadeUI.CreateRoot("VS Transition"); vsRoot.GetComponent<Canvas>().sortingOrder = 30;
            Image bg = ArcadeUI.Panel(vsRoot.transform, "VS Fundo", new Color(.03f, .005f, .03f, .96f)); ArcadeUI.Stretch(bg.rectTransform);
            versusText = ArcadeUI.Text(vsRoot.transform, "VS", "VS", 92, new Color(1f, .25f, .1f), TextAnchor.MiddleCenter); ArcadeUI.Stretch(versusText.rectTransform); versusText.gameObject.AddComponent<Outline>().effectColor = new Color(1f, .85f, .15f);
            Text label = ArcadeUI.Text(vsRoot.transform, "Confronto", CharacterProfile.DisplayName(candidates[selected]) + "  VS  RIVAL", 17, Color.white, TextAnchor.MiddleCenter);
            ArcadeUI.Anchor(label.rectTransform, new Vector2(.5f, .26f), new Vector2(.5f, .26f), Vector2.zero, new Vector2(420, 24));
            yield return new WaitForSeconds(1.1f);
            Destroy(vsRoot);
            flow.BeginFight(candidates[selected]);
        }

        private static string SecondaryPower(CandidateId candidate) => candidate switch
        {
            CandidateId.Lula => "DRENO DE AURA",
            CandidateId.RenanSantos => "GARRAS DA ONÇA",
            CandidateId.FlavioBolsonaro => "CHUVA DE DINHEIRO",
            _ => "ESPECIAL"
        };

        private static Sprite SpriteFor(CandidateId candidate)
        {
            string path = candidate == CandidateId.Lula ? "Art/Characters/lula_idle" : candidate == CandidateId.RenanSantos ? "Art/Characters/renan_idle" : "Art/Characters/flavio_sigma";
            Texture2D texture = Resources.Load<Texture2D>(path);
            return texture == null ? null : Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f), 100f);
        }
    }
}
