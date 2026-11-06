<p align="center">
  <img src="Docs/readme/banner.png" alt="TSE Fatal logo next to four pixel art candidates standing behind debate podiums" width="100%">
</p>

<h3 align="center">A Brazilian election debate, settled as a 2D platform fighter.</h3>

<p align="center">
  Pick a candidate, take the stage at the Planalto Palace, and knock your rival off the screen before they do it to you.
</p>

> [!NOTE]
> TSE Fatal is satire and fiction. The moves are jokes built on internet memes and stories that made the news; they are not accusations or statements of fact. No real voices, photos, or speeches are used. The game is in Brazilian Portuguese.

## How a match works

TSE Fatal plays like Brawlhalla with an arcade coat of paint.

- **Damage is a percentage.** There is no health bar. The higher your damage, the farther each hit sends you flying.
- **Three lives each.** Fly off the screen and the game stamps you *CASSADO!* (impeached).
- **Win the election.** Take all three of your rival's lives and you are *ELEITO!* (elected).
- **One player against a bot,** with easy, normal, and hard difficulty. A demo mode runs bot against bot nonstop.

<p align="center">
  <img src="Docs/readme/versus.png" alt="Versus screen: Lula on the left against Augusto Cury on the right, in front of the Planalto Palace" width="85%">
</p>

## Candidates

<p align="center">
  <img src="Docs/readme/elenco.png" alt="The four candidates behind their podiums: Lula, Renan Santos, Flávio Bolsonaro, and Augusto Cury" width="100%">
</p>

Each candidate has their own stats and three abilities on **K**, **L**, and **I**. Ability names are Brazilian political memes, so they stay in Portuguese.

| | Candidate | Plays like | K | L | I |
| :---: | --- | --- | --- | --- | --- |
| <img src="Docs/readme/face_lula.png" alt="Lula" width="64"> | **Lula** | Heavyweight, hard to launch | Mensalão | Faz o L | Censura |
| <img src="Docs/readme/face_renan.png" alt="Renan Santos" width="64"> | **Renan Santos** | Fastest, light on his feet | Corte Viral | Onça Liberal | Fralda do Debate |
| <img src="Docs/readme/face_flavio.png" alt="Flávio Bolsonaro" width="64"> | **Flávio Bolsonaro** | Balanced, heals himself | Rachadinha | Mansão de 6 Milhões | Chocolate Lavado |
| <img src="Docs/readme/face_cury.png" alt="Augusto Cury" width="64"> | **Augusto Cury** | Control, fights from range | Drone com Sirene | Ministério da IA | Palestra Motivacional |

<details>
<summary><b>What each ability does</b></summary>
<br>

<img src="Docs/readme/santinho_lula.png" alt="Lula's campaign card: Mensalão throws three bundles of cash in a fan; Faz o L makes a giant L explode in front of him; Censura stops the rival from using abilities for 3.5 seconds" width="100%">

<img src="Docs/readme/santinho_renan.png" alt="Renan Santos's campaign card: Corte Viral is a fast dash; Onça Liberal releases a jaguar that runs along the ground; Fralda do Debate makes the rival dizzy with reversed controls" width="100%">

<img src="Docs/readme/santinho_flavio.png" alt="Flávio Bolsonaro's campaign card: Rachadinha is a dash that heals him; Mansão de 6 Milhões drops a mansion on the rival; Chocolate Lavado throws three bouncing bonbons that heal him on hit" width="100%">

<img src="Docs/readme/santinho_cury.png" alt="Augusto Cury's campaign card: Drone com Sirene sends a drone that chases the rival; Ministério da IA sends three robots marching along the ground; Palestra Motivacional puts a nearby rival to sleep" width="100%">

</details>

## Controls

| Key | Action |
| --- | --- |
| `A` `D` | Move |
| `W` or `Space` | Jump, twice in the air |
| `S` | Drop through a platform, fast fall |
| `Shift` | Dodge. Also breaks out of light hits after a moment |
| `J` | Punch: a three-hit combo on the ground, a kick in the air |
| `K` `L` `I` | Abilities |
| `Esc` | Back to the menu |

In menus, `W` `S` or the arrow keys move the cursor, `A` `D` switch candidates, and `Enter` confirms.

## On the campaign trail

<p align="center">
  <img src="Docs/readme/efeitos.png" alt="The twelve ability effects in a row: cash, a giant L, a censor bar, a slash, a jaguar, a diaper, a coin, a mansion, a bonbon, a drone, a robot, and floating Zs" width="100%">
</p>

- **Picanha** (Brazilian barbecue) drops onto a platform every so often. Grab it to heal 15% of your damage.
- **Knockout banners** are drawn at random: *CASSADO!*, *INELEGÍVEL!*, *IMPEACHMENT!*, *DELATADO!*, *CPI NELE!*
- **Each candidate talks** when they lose a life, come back, and win.
- **Voice acting is optional.** Record the lines and drop the files in a folder; the game plays them and lowers the music. The script is in [`Docs/DUBLAGEM.md`](Docs/DUBLAGEM.md).
- **Music and sound effects** are synthesized in code. The project has no audio files.

## Run the project

1. Install Unity **6000.6.8f1** from Unity Hub and open this folder.
2. Wait for the first import. It brings in UGUI, 2D Pixel Perfect, and about 265 art files.
3. Open `Assets/Scenes/Main` and press **Play**.

The scene is nearly empty on purpose. Menus, the arena, the HUD, and the fighters are built in code at startup.

## Project layout

```
Assets/
  Scenes/Main.unity            single entry scene
  Scripts/                     the whole game, namespace TSEFatal
    GameFlow.cs                menu, character select, versus, fight
    FighterController.cs       movement, attacks, abilities, bot AI
    CharacterProfile.cs        each candidate's stats, abilities, and lines
    ...                        arena, projectiles, effects, HUD, audio
  Editor/                      automatic pixel art import settings
  Resources/
    Art/Characters/            base image + one file per frame: {id}/{state}/01.png
    Art/VFX/                   ability effect frames: {effect}/01.png
    Art/Menu/                  menu backgrounds and podiums
    Art/Stages/                the Planalto Palace
    Fonts/                     Press Start 2P
Docs/                          design notes, art guide, voice script, README images
PromptsAnimacao/               one image-generation prompt per frame
```

## How the art is made

Every frame is generated one at a time from its own prompt in [`PromptsAnimacao/`](PromptsAnimacao/README.md). Each prompt attaches the candidate's base image and the previous frame, so the character stays consistent across all 47 frames.

The image generator returns frames on a solid green background, or magenta when the drawing itself has green in it. The game removes that background when it loads the frame.

<p align="center">
  <img src="Docs/readme/chroma.png" alt="Before and after: a frame on a green background, and the same frame with a transparent background as it appears in the game" width="75%">
</p>

To add a new candidate:

1. Fill in their profile in `CharacterProfile.cs`.
2. Write their three abilities in `FighterController.cs`.
3. Save their frames under `Assets/Resources/Art/Characters/{id}/`.

More detail, in Portuguese: [`Docs/GAME_DESIGN.md`](Docs/GAME_DESIGN.md), [`Docs/ART_PACK.md`](Docs/ART_PACK.md), and [`Docs/ANIMATION_PROMPTS.md`](Docs/ANIMATION_PROMPTS.md).

## License

All rights reserved. TSE Fatal's code, art, text, and audio are **not** open source. You may read this repository and run the game locally for personal or academic use. You may not redistribute it, sell it, publish it on any store, or reuse its material in other projects. See [`LICENSE`](LICENSE) for the full terms.

The Press Start 2P font is third-party work under the SIL Open Font License 1.1 ([`Assets/Resources/Fonts/OFL.txt`](Assets/Resources/Fonts/OFL.txt)).

<br>

<p align="center">
  <img src="Docs/readme/nocauteados.png" alt="All four candidates knocked out on the floor" width="85%">
  <br>
  <sub>Polls are closed. No recount.</sub>
</p>
