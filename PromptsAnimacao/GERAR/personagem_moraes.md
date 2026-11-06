# Briefing de pixel art — Alexandre de Moraes

Este arquivo é o pedido completo. Leia tudo antes de começar.

## Sua tarefa

Gerar os frames de animação do personagem Alexandre de Moraes para um jogo de luta 2D. A referência oficial do personagem será a imagem base `moraes_idle.png`, que você gera no passo 0.

Total: 47 frames, nenhum feito ainda.

## Passo 0 — imagem base (faça antes de tudo)

A imagem base (`moraes_idle.png`) ainda não existe. Gere ela primeiro, a partir das imagens anexadas (`lula_idle.png`, `renan_idle.png` e uma foto do Alexandre de Moraes). Só passe para os frames depois que eu aprovar.

- **Arquivo:** `moraes_idle.png`

```
As duas primeiras imagens anexadas são personagens do meu jogo de luta 2D em pixel art, estilo arcade dos anos 90, em tom de sátira cômica. A foto anexada é o Alexandre de Moraes.

Crie um NOVO personagem no MESMO estilo, tamanho e proporções das duas imagens do jogo: uma caricatura do Alexandre de Moraes, homem de uns 55 anos, careca e com a careca brilhando, sobrancelhas grossas e escuras, expressão séria e fechada, toga preta de ministro aberta por cima de um terno preto, camisa branca, gravata escura, sapatos pretos.

Regras:
1. Corpo inteiro, em pé, em pose de guarda de lutador (punhos erguidos, joelhos levemente flexionados), vista de 3/4 virada para a DIREITA, igual aos personagens anexados.
2. Mesma altura relativa dos personagens anexados e mesmo enquadramento vertical, com os pés perto da borda de baixo.
3. Fundo verde chapado #00FF00, sem sombra, sem gradiente e sem chão desenhado.
4. Pixel art nítida, contorno escuro de 1 pixel, cores vivas, sem texto, sem moldura, sem marca d'água.
5. Caricatura bem-humorada, sem deboche ofensivo.
```

Depois de aprovada, essa imagem passa a ser a referência oficial de todos os frames abaixo.

## Regras fixas (valem para todos os frames)

A imagem base é a referência oficial do personagem Alexandre de Moraes (homem de uns 55 anos, careca e com a careca brilhando, sobrancelhas grossas e escuras, expressão séria e fechada, toga preta de ministro aberta por cima de um terno preto, camisa branca, gravata escura, sapatos pretos) para um jogo de luta 2D em pixel art, estilo arcade dos anos 90, em tom de sátira cômica.
1. Mesmo personagem da referência: mesmo rosto, roupa, cores, paleta e proporções.
2. Mesmo tamanho do personagem em todas as imagens: ele em pé ocupa cerca de 80% da altura do quadro.
3. Quadro quadrado 1024x1024, personagem inteiro, centralizado, com os pés encostando perto da borda de baixo (pequena margem).
4. Vista lateral, personagem VIRADO PARA A DIREITA.
5. Fundo totalmente transparente. Se não conseguir, use verde chapado #00FF00, sem sombra, sem gradiente e sem chão desenhado.
6. Pixel art nítida, contorno escuro de 1 pixel, sem texto, sem números, sem moldura, sem marca d'água.
7. Um único personagem por imagem, uma única pose por imagem.

Além disso:
- Os frames de uma mesma animação formam um movimento contínuo: use a imagem anterior como base e mude só a pose descrita.
- Não desenhe projéteis, efeitos grandes ou objetos voando longe do personagem; o jogo desenha esses efeitos à parte. Pequenos brilhos e linhas de movimento junto ao corpo podem.

## Como entregar

- Gere **uma imagem por resposta**, na ordem da lista, pulando as marcadas "já existe, pule".
- Embaixo de cada imagem escreva só o número e o arquivo, por exemplo: `3 — moraes/exemplo/03.png`. Depois espere.
- Quando eu escrever **continue**, gere o próximo item. Quando eu escrever **refaça**, gere o mesmo item de novo, corrigindo o que eu apontar.
- Quando eu escrever **zip**, junte as imagens já aprovadas num arquivo .zip com a estrutura de pastas exatamente como na coluna "Arquivo" (pasta raiz `moraes/`). Eu vou extrair dentro de `Assets/Resources/Art/Characters/`.
- Se você não conseguir gerar um arquivo para baixar, tudo bem: eu salvo a imagem manualmente com o nome indicado.
- Se a animação já tiver um frame marcado "já existe", eu anexo essa imagem: use-a como base para manter o mesmo tamanho e estilo.

## Lista de frames

### Alexandre de Moraes — Parado (guarda)

- **Frames:** 4 (em loop: o último emenda no primeiro)
- **Animação:** respiração em guarda de lutador, em loop.

| # | Arquivo | O que desenhar | Status |
| --- | --- | --- | --- |
| 1 | `moraes/idle/01.png` | pose de guarda de lutador, corpo levemente de lado, punhos erguidos na altura do queixo, joelhos levemente flexionados, peito em posição neutra. |  |
| 2 | `moraes/idle/02.png` | mesma guarda, inspirando: peito e ombros sobem um pouco, punhos sobem levemente. |  |
| 3 | `moraes/idle/03.png` | mesma guarda, peito no ponto mais alto, leve sorriso confiante. |  |
| 4 | `moraes/idle/04.png` | mesma guarda, expirando: ombros descendo, quase igual ao frame 1. |  |

### Alexandre de Moraes — Andar flutuando

- **Frames:** 6 (em loop: o último emenda no primeiro)
- **Animação:** ele não anda: desliza flutuando um pouco acima do chão (o poder de voar, limitado a 10 cm), com a toga ondulando, em loop.

| # | Arquivo | O que desenhar | Status |
| --- | --- | --- | --- |
| 5 | `moraes/walk/01.png` | flutuando com os pés juntos e as pernas paradas, os pés um pouco acima da posição normal (pequeno vão entre os pés e a borda de baixo), corpo levemente inclinado para a frente, toga ondulando para trás. |  |
| 6 | `moraes/walk/02.png` | mesma pose, corpo subindo um pouquinho, barra da toga balançando para cima. |  |
| 7 | `moraes/walk/03.png` | mesma pose no ponto mais alto (ainda bem perto do chão), toga esticada para trás. |  |
| 8 | `moraes/walk/04.png` | mesma pose, corpo descendo um pouquinho, toga ondulando. |  |
| 9 | `moraes/walk/05.png` | mesma pose no ponto mais baixo, pés quase tocando a posição normal, mas sem encostar. |  |
| 10 | `moraes/walk/06.png` | mesma pose, voltando a subir, quase igual ao frame 1. |  |

### Alexandre de Moraes — Soco (jab)

- **Frames:** 4
- **Animação:** soco direto rápido, executado uma vez.

| # | Arquivo | O que desenhar | Status |
| --- | --- | --- | --- |
| 11 | `moraes/attack/01.png` | antecipação: tronco gira um pouco para trás, punho recolhido junto ao corpo. |  |
| 12 | `moraes/attack/02.png` | soco saindo: braço meio esticado para a frente, corpo inclinando à frente. |  |
| 13 | `moraes/attack/03.png` | impacto: braço totalmente esticado à frente na altura do rosto, linhas de velocidade atrás do punho, expressão de esforço. |  |
| 14 | `moraes/attack/04.png` | recuperação: braço voltando, quase de volta à guarda. |  |

### Alexandre de Moraes — Pulo (subida)

- **Frames:** 3
- **Animação:** impulso e subida do pulo. Desenhe o personagem no mesmo lugar do quadro: a altura do pulo é feita pelo jogo.

| # | Arquivo | O que desenhar | Status |
| --- | --- | --- | --- |
| 15 | `moraes/jump/01.png` | agachamento de impulso: joelhos bem dobrados, braços para baixo e para trás. |  |
| 16 | `moraes/jump/02.png` | decolagem: corpo todo esticado, pernas esticadas para baixo com as pontas dos pés, braços subindo. |  |
| 17 | `moraes/jump/03.png` | subindo: joelhos encolhidos, um punho erguido para cima. |  |

### Alexandre de Moraes — Queda

- **Frames:** 2 (em loop: o último emenda no primeiro)
- **Animação:** caindo no ar, em loop curto.

| # | Arquivo | O que desenhar | Status |
| --- | --- | --- | --- |
| 18 | `moraes/fall/01.png` | caindo: braços abertos para cima, pernas levemente separadas, roupa esvoaçando para cima, expressão assustada cômica. |  |
| 19 | `moraes/fall/02.png` | igual ao frame 1, mas com a roupa esvoaçando de outro jeito e os braços um pouco mais altos. |  |

### Alexandre de Moraes — Chute no ar

- **Frames:** 4
- **Animação:** chute voador no ar, executado uma vez.

| # | Arquivo | O que desenhar | Status |
| --- | --- | --- | --- |
| 20 | `moraes/air_attack/01.png` | no ar, encolhendo as pernas, joelhos perto do peito. |  |
| 21 | `moraes/air_attack/02.png` | começando o chute voador: perna da frente esticando para a frente. |  |
| 22 | `moraes/air_attack/03.png` | chute voador: perna totalmente esticada para a frente, corpo inclinado para trás, linhas de velocidade. |  |
| 23 | `moraes/air_attack/04.png` | recolhendo a perna, pose de queda. |  |

### Alexandre de Moraes — Reflexo da Careca (K)

- **Frames:** 5
- **Animação:** inclina a cabeça, o sol reflete na careca e sai um raio de luz em linha reta.

| # | Arquivo | O que desenhar | Status |
| --- | --- | --- | --- |
| 24 | `moraes/special1/01.png` | olha para o alto com as mãos na cintura, expressão séria. |  |
| 25 | `moraes/special1/02.png` | inclina a cabeça para a frente mostrando o topo da careca, um pequeno brilho branco na careca. |  |
| 26 | `moraes/special1/03.png` | brilho na careca mais forte, olhos fechados, concentrado. |  |
| 27 | `moraes/special1/04.png` | disparo: cabeça inclinada para a frente, clarão branco-amarelado curto saindo da careca (só junto à cabeça). |  |
| 28 | `moraes/special1/05.png` | volta à guarda passando a mão na careca, satisfeito. |  |

### Alexandre de Moraes — Duelo de Faroeste (L)

- **Frames:** 5
- **Animação:** põe um chapéu de cowboy, encara o rival e saca a caneta mais rápido que ele.

| # | Arquivo | O que desenhar | Status |
| --- | --- | --- | --- |
| 29 | `moraes/special2/01.png` | coloca um chapéu de cowboy marrom na cabeça com uma mão, olhar fixo para a frente. |  |
| 30 | `moraes/special2/02.png` | postura de duelo: pernas afastadas, mão aberta parada perto do bolso da toga, olhos semicerrados. |  |
| 31 | `moraes/special2/03.png` | saque: puxa uma caneta-tinteiro preta do bolso num movimento rápido, linhas de velocidade junto ao braço. |  |
| 32 | `moraes/special2/04.png` | braço esticado apontando a caneta para a frente como um revólver, um pequeno brilho na ponta da caneta. |  |
| 33 | `moraes/special2/05.png` | sopra a ponta da caneta como quem sopra um revólver, chapéu de cowboy ainda na cabeça. |  |

### Alexandre de Moraes — Habemus Xandão (I)

- **Frames:** 5
- **Animação:** sobe uma fumaça branca de conclave e ele ganha uma mitra de papa: por alguns segundos não recua quando apanha.

| # | Arquivo | O que desenhar | Status |
| --- | --- | --- | --- |
| 34 | `moraes/special3/01.png` | mãos juntas em oração na frente do peito, olhos fechados, sério. |  |
| 35 | `moraes/special3/02.png` | braços abertos para os lados com as palmas para cima, um pouco de fumaça branca junto aos pés. |  |
| 36 | `moraes/special3/03.png` | mitra de papa branca e dourada aparecendo na cabeça, olhar solene para o alto. |  |
| 37 | `moraes/special3/04.png` | de mitra, uma mão erguida dando a bênção, brilho dourado em volta do corpo. |  |
| 38 | `moraes/special3/05.png` | volta à guarda ainda de mitra, peito estufado. |  |

### Alexandre de Moraes — Levando golpe

- **Frames:** 2
- **Animação:** reação cômica a um golpe.

| # | Arquivo | O que desenhar | Status |
| --- | --- | --- | --- |
| 39 | `moraes/hurt/01.png` | atingido: cabeça e tronco jogados para trás, olhos fechados, boca aberta de dor cômica, braços abertos. |  |
| 40 | `moraes/hurt/02.png` | ainda recuando, corpo mais curvado para trás, estrelinhas amarelas girando acima da cabeça. |  |

### Alexandre de Moraes — Esquiva

- **Frames:** 4
- **Animação:** esquiva rápida com rolamento.

| # | Arquivo | O que desenhar | Status |
| --- | --- | --- | --- |
| 41 | `moraes/dodge/01.png` | agachando rápido, corpo bem abaixado. |  |
| 42 | `moraes/dodge/02.png` | rolamento: corpo encolhido como uma bola, de lado. |  |
| 43 | `moraes/dodge/03.png` | saindo do rolamento: um joelho no chão, levantando. |  |
| 44 | `moraes/dodge/04.png` | de pé novamente, em guarda. |  |

### Alexandre de Moraes — Nocaute

- **Frames:** 3
- **Animação:** nocaute cômico.

| # | Arquivo | O que desenhar | Status |
| --- | --- | --- | --- |
| 45 | `moraes/ko/01.png` | atingido com força, tombando para trás, pés saindo do chão, olhos em X. |  |
| 46 | `moraes/ko/02.png` | caído sentado no chão, cabeça tonta com espiral sobre ela. |  |
| 47 | `moraes/ko/03.png` | deitado de costas no chão, na horizontal, olhos em X, língua para fora (cômico). |  |

## Conferência (faça antes de entregar cada imagem)

- Virado para a direita, mesmo rosto, roupa e cores da referência.
- Mesmo tamanho dos outros frames; pés perto da borda de baixo; personagem inteiro, sem cortes.
- Fundo transparente ou verde chapado #00FF00, sem chão nem sombra, sem texto.
