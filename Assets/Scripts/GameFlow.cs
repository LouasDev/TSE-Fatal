using System.Collections;
using UnityEngine;

namespace TSEFatal
{
    public class GameFlow : MonoBehaviour
    {
        private enum Phase { Menu, Select, Countdown, Fight, Result }

        private Phase phase = Phase.Menu;
        private Camera gameCamera;
        private MainMenuController mainMenu;
        private CharacterSelectController characterSelect;
        private MenuAudio sfx;
        private GameObject battleRoot;
        private Arena arena;
        private BattleHUD hud;
        private FighterController player, bot;
        private CandidateId lastPlayer, lastEnemy;
        private bool demo;          // bot contra bot, em loop, até apertar ESC
        private float demoRestart;  // contagem até a próxima luta da demonstração

        private static int inputLockFrame = -1;

        /// <summary>
        /// Chamado em toda troca de tela. A tecla que abriu a tela nova (ENTER, ESPAÇO) continua
        /// "apertada" até o fim do quadro; sem a trava, a tela seguinte lia a mesma tecla e
        /// confirmava sozinha (menu → seleção → luta sem o jogador escolher).
        /// </summary>
        public static void LockInputThisFrame() => inputLockFrame = Time.frameCount;
        public static bool InputLocked => Time.frameCount == inputLockFrame;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            CreateCamera();
            mainMenu = new GameObject("Main Menu Controller").AddComponent<MainMenuController>();
            mainMenu.Initialize(this);
            sfx = MenuAudio.Ensure(mainMenu.gameObject);
            mainMenu.Show();
        }

        private void Update()
        {
            Combat.Tick(Time.deltaTime);
            if (InputLocked && phase != Phase.Fight) return;
            switch (phase)
            {
                case Phase.Countdown:
                case Phase.Fight:
                    if (Input.GetKeyDown(KeyCode.Escape)) { ReturnToMainMenu(); return; }
                    if (phase == Phase.Fight)
                    {
                        UpdatePickups();
                        sfx.SetIntensity(player.Stocks == 1 || bot.Stocks == 1);
                        CheckEnd();
                    }
                    break;
                case Phase.Result:
                    if (demo)
                    {
                        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return)) { ReturnToMainMenu(); return; }
                        demoRestart -= Time.deltaTime;
                        hud.SetResultHelp($"PRÓXIMA LUTA EM {Mathf.CeilToInt(Mathf.Max(0f, demoRestart))}...     ESC: SAIR");
                        if (demoRestart <= 0f) StartDemo();
                    }
                    else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape)) ReturnToMainMenu();
                    else if (Input.GetKeyDown(KeyCode.R)) StartBattle(lastPlayer, lastEnemy, false);
                    break;
            }
        }

        /// <summary>Modo apresentação: dois bots com personagens sorteados, uma luta atrás da outra.</summary>
        public void StartDemo()
        {
            if (characterSelect != null) characterSelect.Hide();
            mainMenu.Hide();
            var a = (CandidateId)Random.Range(0, CharacterProfile.Count);
            StartBattle(a, RandomRival(a), true);
        }

        public void OpenCharacterSelect()
        {
            mainMenu.Hide();
            CleanupBattle();
            demo = false;
            if (characterSelect == null)
            {
                characterSelect = new GameObject("Character Select Controller").AddComponent<CharacterSelectController>();
                characterSelect.Initialize(this);
            }
            phase = Phase.Select;
            characterSelect.Show();
        }

        /// <summary>Sorteia um rival diferente do candidato escolhido.</summary>
        public static CandidateId RandomRival(CandidateId candidate) =>
            (CandidateId)(((int)candidate + Random.Range(1, CharacterProfile.Count)) % CharacterProfile.Count);

        public void BeginFight(CandidateId candidate, CandidateId enemy)
        {
            if (characterSelect != null) characterSelect.Hide();
            mainMenu.Hide();
            StartBattle(candidate, enemy, false);
        }

        /// <summary>demoMode decide se o lado esquerdo é bot. Sempre explícito: nunca herda o modo da luta anterior.</summary>
        private void StartBattle(CandidateId playerCandidate, CandidateId enemyCandidate, bool demoMode)
        {
            CleanupBattle();
            LockInputThisFrame();
            demo = demoMode;
            lastPlayer = playerCandidate;
            lastEnemy = enemyCandidate;
            Combat.Reset();

            battleRoot = new GameObject("Batalha");
            Effects.Root = battleRoot.transform;
            arena = Arena.Create(battleRoot.transform, gameCamera);
            player = SpawnFighter(demo ? "Bot 1" : "Jogador", playerCandidate, -6f, demo, 1, 2);
            bot = SpawnFighter(demo ? "Bot 2" : "Bot", enemyCandidate, 6f, true, -1, 0);
            player.BotLevel = bot.BotLevel = BotDifficulty.Current;
            player.Target = bot;
            bot.Target = player;
            player.OnKnockedOut += OnKnockedOut;
            bot.OnKnockedOut += OnKnockedOut;
            hud = BattleHUD.Create(battleRoot.transform, player, bot, demo);
            nextPickup = Random.Range(12f, 16f);
            sfx.PlayBattleMusic();
            StartCoroutine(Countdown());
        }

        private float nextPickup;

        /// <summary>De tempos em tempos cai uma picanha na arena (promessa de campanha cumprida!).</summary>
        private void UpdatePickups()
        {
            nextPickup -= Time.deltaTime;
            if (nextPickup > 0f) return;
            nextPickup = Random.Range(15f, 22f);
            Pickup.Spawn(arena, sfx, new[] { player, bot });
        }

        private FighterController SpawnFighter(string objectName, CandidateId candidate, float x, bool isBot, int facing, int order)
        {
            var go = new GameObject(objectName);
            go.transform.SetParent(battleRoot.transform, false);
            var fighter = go.AddComponent<FighterController>();
            fighter.Initialize(candidate, arena, sfx, x, isBot, facing, order);
            return fighter;
        }

        private IEnumerator Countdown()
        {
            phase = Phase.Countdown;
            GameObject root = battleRoot;
            string[] steps = { "3", "2", "1" };
            foreach (string step in steps)
            {
                hud.ShowBig(step, Color.white, .7f);
                sfx.Beep(false);
                yield return new WaitForSeconds(.75f);
                if (battleRoot != root) yield break; // saiu da luta durante a contagem
            }
            hud.ShowBig("CONFIRMA!", new Color(.3f, 1f, .4f), 1f);
            sfx.Beep(true);
            player.ControlsEnabled = bot.ControlsEnabled = true;
            phase = Phase.Fight;
        }

        private void OnKnockedOut(FighterController fighter)
        {
            if (fighter.Stocks > 0) hud.ShowBig(CharacterProfile.Pick(CharacterProfile.KnockoutBanners), new Color(1f, .3f, .2f), 1.1f);
        }

        private void CheckEnd()
        {
            if (player == null || bot == null) return;
            if (!player.Eliminated && !bot.Eliminated) return;
            FighterController winner = player.Eliminated ? bot : player;
            player.ControlsEnabled = bot.ControlsEnabled = false;
            hud.ShowResult(winner);
            sfx.Beep(true);
            sfx.Cheer();
            sfx.Voice(winner, winner.VoicePrefix + "vitoria", true);
            sfx.SetIntensity(false);
            demoRestart = 5f;
            phase = Phase.Result;
        }

        public void ReturnToMainMenu()
        {
            LockInputThisFrame();
            demo = false;
            CleanupBattle();
            if (characterSelect != null) characterSelect.Hide();
            phase = Phase.Menu;
            sfx.PlayMenuMusic();
            mainMenu.Show();
        }

        private void CleanupBattle()
        {
            StopAllCoroutines();
            if (battleRoot != null) Destroy(battleRoot);
            battleRoot = null;
            Effects.Root = null;
            arena = null; hud = null; player = null; bot = null;
            Combat.Reset();
            if (gameCamera != null)
            {
                gameCamera.transform.position = new Vector3(0f, 0f, -10f);
                gameCamera.orthographicSize = Arena.ViewHeight * .5f;
            }
        }

        private void CreateCamera()
        {
            gameCamera = Camera.main;
            if (gameCamera == null)
            {
                var cameraObject = new GameObject("Main Camera");
                gameCamera = cameraObject.AddComponent<Camera>();
                cameraObject.tag = "MainCamera";
            }
            // Sem AudioListener a Unity toca as fontes mas ninguém "ouve": o jogo ficava mudo.
            if (FindAnyObjectByType<AudioListener>() == null) gameCamera.gameObject.AddComponent<AudioListener>();
            gameCamera.orthographic = true;
            gameCamera.orthographicSize = Arena.ViewHeight * .5f;
            gameCamera.transform.position = new Vector3(0f, 0f, -10f);
            // Cor sólida: antes aparecia o céu padrão da Unity em volta do menu.
            gameCamera.clearFlags = CameraClearFlags.SolidColor;
            gameCamera.backgroundColor = new Color(0.025f, 0.035f, 0.09f);
            // O PixelPerfectCamera travava o zoom em múltiplos inteiros e mostrava área demais
            // em resoluções comuns. A Arena agora enquadra a câmera para preencher a tela.
            var pixelCamera = gameCamera.GetComponent<UnityEngine.U2D.PixelPerfectCamera>();
            if (pixelCamera != null) Destroy(pixelCamera);
        }
    }
}
