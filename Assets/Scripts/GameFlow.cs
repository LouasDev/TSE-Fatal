using UnityEngine;
using UnityEngine.U2D;

namespace TSEFatal
{
    public class GameFlow : MonoBehaviour
    {
        private FighterController player;
        private FighterController bot;
        private MainMenuController mainMenu;
        private CharacterSelectController characterSelect;
        private bool fighting;
        private string result;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            CreateCamera();
            CreateArena();
            mainMenu = new GameObject("Main Menu Controller").AddComponent<MainMenuController>();
            mainMenu.Initialize(this);
            mainMenu.Show();
        }

        private void Update()
        {
            if (!fighting) return;
            if (Input.GetKeyDown(KeyCode.Escape)) ReturnToMainMenu();
            if (player != null && bot != null && string.IsNullOrEmpty(result))
            {
                if (player.Health <= 0f) result = $"{CharacterProfile.DisplayName(bot.Candidate)} venceu!";
                if (bot.Health <= 0f) result = $"{CharacterProfile.DisplayName(player.Candidate)} venceu!";
            }
            else if (!string.IsNullOrEmpty(result) && Input.GetKeyDown(KeyCode.Return)) ReturnToMainMenu();
        }

        public void OpenCharacterSelect()
        {
            mainMenu.Hide();
            if (characterSelect == null)
            {
                characterSelect = new GameObject("Character Select Controller").AddComponent<CharacterSelectController>();
                characterSelect.Initialize(this);
            }
            characterSelect.Show();
        }

        public void BeginFight(CandidateId candidate)
        {
            if (characterSelect != null) characterSelect.Hide();
            CandidateId enemy = (CandidateId)(((int)candidate + UnityEngine.Random.Range(1, 3)) % 3);
            result = string.Empty;
            fighting = true;
            player = SpawnFighter("Jogador", candidate, new Vector2(-4f, -2.38f), false, 1);
            bot = SpawnFighter("Bot", enemy, new Vector2(4f, -2.38f), true, -1);
            player.Target = bot;
            bot.Target = player;
        }

        public void ReturnToMainMenu()
        {
            if (player != null) Destroy(player.gameObject);
            if (bot != null) Destroy(bot.gameObject);
            player = null;
            bot = null;
            result = string.Empty;
            fighting = false;
            mainMenu.Show();
        }

        private FighterController SpawnFighter(string objectName, CandidateId candidate, Vector2 position, bool isBot, int direction)
        {
            var fighter = new GameObject(objectName).AddComponent<FighterController>();
            fighter.Initialize(candidate, position, isBot, direction);
            return fighter;
        }

        private void CreateCamera()
        {
            var gameCamera = Camera.main;
            if (gameCamera == null)
            {
                var cameraObject = new GameObject("Main Camera");
                gameCamera = cameraObject.AddComponent<Camera>();
                cameraObject.tag = "MainCamera";
            }
            gameCamera.orthographic = true;
            gameCamera.orthographicSize = 5f;
            gameCamera.transform.position = new Vector3(0f, 0f, -10f);
            gameCamera.backgroundColor = new Color(0.025f, 0.035f, 0.09f);
            var pixelCamera = gameCamera.GetComponent<PixelPerfectCamera>();
            if (pixelCamera == null) pixelCamera = gameCamera.gameObject.AddComponent<PixelPerfectCamera>();
            pixelCamera.assetsPPU = 16;
            pixelCamera.refResolutionX = 480;
            pixelCamera.refResolutionY = 270;
        }

        private void CreateArena()
        {
            CreateBlock("Chao", new Vector2(0, -2.65f), new Vector2(18, .5f), new Color(.15f, .18f, .28f));
            CreateBlock("Painel Debate", new Vector2(0, 2.8f), new Vector2(15, .7f), new Color(.08f, .12f, .25f));
            CreateBlock("Podio Esquerdo", new Vector2(-5.6f, -1.8f), new Vector2(1.1f, 1.5f), new Color(.12f, .15f, .25f));
            CreateBlock("Podio Direito", new Vector2(5.6f, -1.8f), new Vector2(1.1f, 1.5f), new Color(.12f, .15f, .25f));
        }

        private void CreateBlock(string objectName, Vector2 position, Vector2 scale, Color color)
        {
            var block = new GameObject(objectName);
            var renderer = block.AddComponent<SpriteRenderer>();
            renderer.sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), new Vector2(.5f, .5f));
            renderer.color = color;
            block.transform.position = position;
            block.transform.localScale = scale;
            if (objectName == "Chao") block.AddComponent<BoxCollider2D>().size = Vector2.one;
        }

        private void OnGUI()
        {
            if (!fighting || player == null || bot == null) return;
            DrawBar(28, 28, player.Health / player.MaxHealth, CharacterProfile.Tint(player.Candidate), CharacterProfile.DisplayName(player.Candidate));
            DrawBar(Screen.width - 228, 28, bot.Health / bot.MaxHealth, CharacterProfile.Tint(bot.Candidate), CharacterProfile.DisplayName(bot.Candidate));
            GUI.Label(new Rect(0, Screen.height - 28, Screen.width, 20), "A/D mover | W pular | J ataque | K especial | Esc menu", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, normal = { textColor = Color.white } });
            if (!string.IsNullOrEmpty(result)) GUI.Box(new Rect(Screen.width / 2 - 160, Screen.height / 2 - 40, 320, 80), result + "\nEnter para voltar ao menu");
        }

        private void DrawBar(float x, float y, float value, Color color, string label)
        {
            GUI.Label(new Rect(x, y - 18, 200, 18), label);
            GUI.Box(new Rect(x, y, 200, 18), string.Empty);
            Color old = GUI.color; GUI.color = color;
            GUI.Box(new Rect(x + 2, y + 2, 196 * Mathf.Clamp01(value), 14), string.Empty);
            GUI.color = old;
        }
    }
}
