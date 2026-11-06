# Prompts de animação — TSE Fatal

Guia para gerar os frames de pixel art no ChatGPT. O jogo já toca as animações automaticamente, sem precisar mexer em código: basta salvar os PNGs nas pastas certas.

> **Recomendado:** gerar um frame por vez com os prompts de `PromptsAnimacao/` (um `.md` por frame). Este documento descreve a alternativa de gerar a animação inteira numa única folha.

## 1. Onde salvar

```
Assets/Resources/Art/Characters/lula/{estado}.png
Assets/Resources/Art/Characters/renan/{estado}.png
Assets/Resources/Art/Characters/flavio/{estado}.png
```

- **Uma imagem por estado**, com todos os frames **lado a lado numa única linha horizontal** (sprite sheet). O jogo separa os frames sozinho pelas colunas vazias entre eles.
- Alternativa: um arquivo por frame, `walk_0.png`, `walk_1.png`, `walk_2.png`...
- Fundo **transparente** é o ideal. Fundo de **cor sólida e uniforme** (ex.: verde #00FF00) também funciona: o jogo remove a cor do canto da imagem.
- Personagem **sempre virado para a direita**. O jogo espelha quando ele vira.
- Deixe um espaço vazio (pelo menos ~2% da largura) entre um frame e outro, sem nada encostando.
- Os pés devem ficar na mesma altura em todos os frames (o jogo alinha pelo pé).
- Estado que não tiver imagem continua usando a animação procedural atual. Dá para entregar aos poucos.

## 2. Estados e quantidade de frames

| Arquivo | Ação | Frames sugeridos | Repete? |
| --- | --- | --- | --- |
| `idle.png` | Parado em guarda, respirando | 4 | sim |
| `walk.png` | Andando/correndo | 6 | sim |
| `jump.png` | Impulso e subida do pulo | 3 | não |
| `fall.png` | Caindo | 2 | sim |
| `attack.png` | Soco (jab) no chão | 4 | não |
| `air_attack.png` | Chute/golpe no ar | 4 | não |
| `special1.png` | Habilidade K | 5 | não |
| `special2.png` | Habilidade L | 5 | não |
| `special3.png` | Habilidade I (só Lula: Censura) | 5 | não |
| `hurt.png` | Levando golpe | 2 | não |
| `dodge.png` | Esquiva (rolamento/desvio) | 4 | não |
| `ko.png` | Nocauteado, caído | 3 | não |

Ordem de prioridade para gerar: **idle → walk → attack → jump/fall → special1/2/3 → hurt → dodge → ko**.

## 3. Prompt base (cole antes de cada pedido)

> Use a imagem anexada como referência exata do personagem (rosto, roupa, cores e proporções). Crie uma **sprite sheet de pixel art 2D estilo jogo de luta arcade dos anos 90**, vista lateral, personagem **virado para a direita**, corpo inteiro. **[N] frames numa única linha horizontal**, todos do mesmo tamanho, com espaço vazio entre eles e os pés na mesma linha de base. **Fundo transparente** (ou verde chapado #00FF00 sem sombras nem gradiente). Contorno escuro de 1 pixel, paleta limitada, sem texto, sem moldura, sem números, sem chão desenhado. Tom de caricatura satírica e cômica, sem violência gráfica.
>
> Animação: **[DESCRIÇÃO DO ESTADO]**

Anexe sempre a imagem estática do personagem (`lula_idle.png`, `renan_idle.png` ou `flavio_sigma.png`) para manter a consistência.

## 4. Descrições por estado

### Comuns aos três (troque o nome)

- **idle (4):** postura de guarda com os punhos erguidos, respirando: o peito sobe e desce levemente, ombros balançam um pouco.
- **walk (6):** ciclo de caminhada rápida em guarda, braços alternando, passos largos; o 1º e o 4º frames com as pernas mais abertas.
- **jump (3):** agachamento de impulso → saindo do chão com os joelhos dobrados → subindo com um braço para cima.
- **fall (2):** caindo com os braços abertos e a roupa/gravata esvoaçando para cima; os 2 frames variam só a roupa.
- **attack (4):** soco direto: recolhe o braço → soco esticado à frente → braço no alcance máximo com rastro de movimento → volta à guarda.
- **air_attack (4):** no ar, chute voador para a frente: encolhe as pernas → perna esticada → mantém → recolhe.
- **hurt (2):** atingido, cabeça e tronco jogados para trás, olhos fechados de dor cômica, estrelinhas na cabeça no 2º frame.
- **dodge (4):** esquiva rápida: agacha → rola de lado → levanta → volta à guarda. Pode ter um leve brilho branco translúcido.
- **ko (3):** nocauteado: tomba para trás → cai sentado → deitado no chão com olhos em X (tontura cômica).

### Lula

- **special1 — Mensalão (5):** abre uma maleta e arremessa maços de dinheiro para a frente: pega a maleta → abre → braço jogando para frente com notas voando → notas espalhadas → volta à guarda.
- **special2 — Faz o L (5):** faz o gesto de "L" com a mão erguida e um brilho vermelho em volta: prepara → ergue o braço → mão em L bem alto com aura vermelha → aura explodindo → volta.
- **special3 — Censura (5):** puxa uma grande tarja preta escrito "CENSURADO" e arremessa: puxa do bolso do paletó → segura com as duas mãos → gira o corpo → arremessa para frente → volta à guarda. (Se o ChatGPT errar o texto, peça a tarja preta sem texto: a tarja que voa é um efeito separado, em `VFX/censura_tarja.png`.)

### Renan Santos

- **special1 — Corte Viral (5):** investida rápida com um celular na mão, como se gravasse um vídeo, deixando um rastro de movimento: agacha → dispara para frente → corpo inclinado no máximo → rastro de luz → freia.
- **special2 — Onça Liberal (5):** invoca uma onça pintada: aponta para frente → aura amarela → braço esticado com o rugido de uma onça aparecendo atrás → mantém → volta.

### Flávio Bolsonaro

- **special1 — Rachadinha (5):** investida segurando a maleta à frente, moedas voando: segura a maleta → corre → maleta estendida batendo → moedas espalhando → volta.
- **special2 — Mansão de 6 Milhões (5):** aponta para o céu chamando uma mansão que vai cair: olha para cima → ergue o braço → aponta com sorriso confiante → braços cruzados satisfeito → volta à guarda.

## 5. Efeitos visuais (opcional)

Salve em `Assets/Resources/Art/VFX/`. Se o arquivo não existir, o jogo usa o desenho procedural atual.

| Arquivo | O que é | Prompt curto |
| --- | --- | --- |
| `mensalao_maco.png` | Maço de dinheiro arremessado | "maço de notas de dinheiro verde preso com elástico, pixel art, 1 objeto, fundo transparente" |
| `nota.png` | Nota solta | "uma cédula de dinheiro voando, pixel art pequena, fundo transparente" |
| `moeda.png` | Moeda | "moeda dourada brilhante, pixel art 16x16, fundo transparente" |
| `faz_o_l.png` | Letra L gigante | "letra L vermelha enorme e brilhante com contorno branco, pixel art, fundo transparente" |
| `censura_tarja.png` | Tarja de censura | "tarja retangular preta com a palavra CENSURADO em branco, pixel art, fundo transparente" |
| `mansao.png` | Mansão que cai | "mansão luxuosa de dois andares com piscina, vista frontal, pixel art, fundo transparente" |
| `onca_0.png` / `onca_1.png` | Onça correndo (2 frames) | "onça pintada correndo virada para a direita, pixel art, frame [1/2] de um ciclo de corrida, fundo transparente" |

Para efeitos, peça **um único objeto por imagem**, centralizado e sem sombra no chão.

## 6. Checklist antes de colocar no projeto

- [ ] Virado para a direita.
- [ ] Frames numa linha só, com espaço entre eles.
- [ ] Fundo transparente ou cor chapada.
- [ ] Mesmo tamanho de personagem em todos os frames.
- [ ] Nome do arquivo exatamente como na tabela (minúsculas, com `_`).

Depois de copiar, volte ao Unity: a importação já configura a textura. Se algo sair cortado ou desalinhado, me mande o PNG que eu ajusto.
