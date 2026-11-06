# Prompts de animação — um arquivo por frame

> **Jeito mais rápido:** use a pasta `GERAR/`. Cada arquivo é um pedido completo para anexar no ChatGPT, com as imagens já prontas marcadas para pular. Veja `GERAR/LEIA-ME.md`.

Cada `.md` desta pasta é **um frame**: tem o prompt pronto para colar no ChatGPT e o caminho exato onde salvar a imagem. O jogo carrega os frames sozinho, sem mexer em código.

## Como usar

1. Abra **um chat novo por personagem** no ChatGPT.
2. Anexe a imagem base do personagem e envie o `00_inicio.md` da pasta dele. Ele fixa as regras: tamanho, direção, fundo.
3. Siga as pastas na ordem (`01_idle`, `02_walk`...). Em cada frame:
   - anexe a imagem base **e o frame anterior** (ajuda a manter o mesmo tamanho e estilo);
   - cole o bloco "Prompt";
   - confira o checklist e salve no caminho indicado em "Salvar como".
4. Volte ao Unity: a importação configura a imagem sozinha. Dê Play para ver.

Não precisa gerar tudo de uma vez. A animação que ainda não tiver frames continua usando o movimento automático atual.

## Onde os frames ficam

```
Assets/Resources/Art/Characters/{lula|renan|flavio|cury|moraes}/{estado}/01.png, 02.png, ...
Assets/Resources/Art/VFX/{efeito}/01.png, 02.png, ...
```

O jogo toca os frames em ordem numérica. Use sempre dois dígitos (`01`, `02`...).

Efeito sem frames continua com o desenho atual do jogo (feito em código), então dá para gerar aos poucos.

## Conteúdo

| Pasta | Frames |
| --- | --- |
| `lula/` | 47: idle 4, walk 6, attack 4, jump 3, fall 2, air_attack 4, Mensalão 5, Faz o L 5, Censura 5, hurt 2, dodge 4, ko 3 |
| `renan/` | 47: os mesmos, com Corte Viral 5, Onça Liberal 5 e Fralda do Debate 5 |
| `flavio/` | 47: os mesmos, com Rachadinha 5, Mansão de 6 Milhões 5 e Chocolate Lavado 5 |
| `cury/` | 47: os mesmos, com Drone com Sirene 5, Ministério da IA 5 e Palestra Motivacional 5 |
| `moraes/` | 47: os mesmos, com andar flutuando 6, Reflexo da Careca 5, Duelo de Faroeste 5 e Habemus Xandão 5 |
| `vfx/` | 85 frames de 22 efeitos (tabela abaixo) |

**Augusto Cury:** a imagem base (`cury_idle.png`) já existe. Comece pelo `cury/00_inicio.md`, como nos outros.

**Alexandre de Moraes:** ainda não tem imagem base. Gere primeiro o `moraes/00_base.md` (anexe `lula_idle.png`, `renan_idle.png` e uma foto dele) e salve como `Assets/Resources/Art/Characters/moraes_idle.png`. Depois siga o `moraes/00_inicio.md`.

**Comece por:** idle → walk → attack. São as animações que mais aparecem na tela.

### Efeitos (`vfx/`)

**Mais rápido:** use o `vfx/PROMPT_UNICO.md`. Você cola um bloco só e depois manda "próximo" a cada imagem.

**Frame a frame:** envie primeiro o `vfx/00_inicio.md`. Cada pasta é um efeito; cada `.md`, um frame.

| Pasta | Habilidade | Frames | Fundo |
| --- | --- | --- | --- |
| `01_mensalao_maco` | Lula — Mensalão (K) | 4 | magenta |
| `02_nota` | Lula — Mensalão (K) | 3 | magenta |
| `03_moeda` | Flávio — Rachadinha (K) e picanha | 4 | verde |
| `04_faz_o_l` | Lula — Faz o L (L) | 5 | verde |
| `05_censura_tarja` | Lula — Censura (I) | 3 | verde |
| `06_corte_viral` | Renan — Corte Viral (K) | 4 | verde |
| `07_onca` | Renan — Onça Liberal (L) | 6 | verde |
| `08_onca_rugido` | Renan — Onça Liberal (L) | 3 | verde |
| `09_fralda` | Renan — Fralda do Debate (I) | 4 | magenta |
| `10_fedor` | Renan — Fralda do Debate (I) | 4 | magenta |
| `11_mansao_queda` | Flávio — Mansão de 6 Milhões (L) | 2 | magenta |
| `12_mansao_impacto` | Flávio — Mansão de 6 Milhões (L) | 5 | magenta |
| `13_bombom` | Flávio — Chocolate Lavado (I) | 4 | verde |
| `14_picanha` | Item de cura (picanha) | 4 | verde |
| `15_drone_sirene` | Cury — Drone com Sirene (K) | 4 | verde |
| `16_robo_ia` | Cury — Ministério da IA (L) | 4 | verde |
| `17_zzz_sono` | Cury — Palestra Motivacional (I) | 3 | verde |
| `18_raio_careca` | Moraes — Reflexo da Careca (K) | 4 | verde |
| `19_feno_rolando` | Moraes — Duelo de Faroeste (L) | 4 | magenta |
| `20_tinta_tiro` | Moraes — Duelo de Faroeste (L) | 4 | verde |
| `21_fumaca_conclave` | Moraes — Habemus Xandão (I) | 4 | verde |
| `22_mitra` | Moraes — Habemus Xandão (I) | 3 | verde |

**Mais visíveis primeiro:** mansão (queda + impacto), onça, Faz o L e maço do Mensalão.

**Fundo magenta:** o jogo apaga a cor do canto da imagem. Efeito com verde ou amarelo-esverdeado (dinheiro, fralda, fedor, feno) em fundo verde perderia partes do desenho, por isso usa magenta #FF00FF.

## Dicas

- **Personagem mudou de tamanho entre frames:** peça "deixe o personagem exatamente do mesmo tamanho do frame anterior". O jogo usa a mesma escala para todos os frames de uma animação, então diferenças grandes aparecem na tela.
- **Veio virado para a esquerda:** peça para espelhar. O jogo espelha sozinho quando o personagem vira, mas o arquivo tem que estar virado para a direita.
- **Fundo xadrez cinza "falso" em vez de transparente:** peça fundo verde chapado #00FF00. O jogo remove a cor do canto da imagem.
- **Ficou cortado ou desalinhado no jogo:** me mande o PNG que eu ajusto o recorte.
