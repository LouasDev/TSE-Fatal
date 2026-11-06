# Briefing de pixel art — efeitos das habilidades

Este arquivo é o pedido completo. Leia tudo antes de começar.

## Sua tarefa

Gerar os frames dos efeitos animados das habilidades de um jogo de luta 2D em pixel art, estilo arcade dos anos 90, em tom de sátira cômica (mansão caindo do céu, onça, L gigante, maço de dinheiro etc.). Cada efeito tem alguns frames que formam uma animação curta.

Total: 85 frames em 22 efeitos, 66 já existem e 19 faltam. A lista está na ordem de prioridade.

## Regras fixas (valem para todas as imagens)

1. Quadro quadrado 1024x1024, um único objeto por imagem.
2. Dentro do mesmo efeito, o objeto tem SEMPRE o mesmo tamanho, o mesmo desenho e a mesma posição no quadro; só muda o movimento descrito. Use a imagem anterior do mesmo efeito como base.
3. Quando o objeto tiver frente e trás, ele fica virado para a DIREITA.
4. Fundo de uma cor só, chapada, sem gradiente, sem sombra e sem chão desenhado, na cor indicada em cada efeito. Efeitos com verde no desenho (dinheiro, fralda, fedor) usam MAGENTA #FF00FF, porque o jogo apaga a cor do fundo; os outros usam VERDE #00FF00.
5. Efeitos marcados "apoiado no chão" (onça, mansão) ficam encostados perto da borda de baixo do quadro. Os demais ficam centralizados.
6. Pixel art nítida, contorno escuro de 1 pixel, cores vivas, sem moldura, sem marca d'água e sem texto (as únicas exceções são a palavra CENSURADO na tarja de censura e as letras Z do sono).

## Como entregar

- Gere **uma imagem por resposta**, na ordem da lista, pulando as marcadas "já existe, pule".
- Embaixo de cada imagem escreva só o número e o arquivo, por exemplo: `3 — VFX/exemplo/03.png`. Depois espere.
- Quando eu escrever **continue**, gere o próximo item. Quando eu escrever **refaça**, gere o mesmo item de novo, corrigindo o que eu apontar.
- Quando eu escrever **zip**, junte as imagens já aprovadas num arquivo .zip com a estrutura de pastas exatamente como na coluna "Arquivo" (pasta raiz `VFX/`). Eu vou extrair dentro de `Assets/Resources/Art/`.
- Se você não conseguir gerar um arquivo para baixar, tudo bem: eu salvo a imagem manualmente com o nome indicado.
- Se a animação já tiver um frame marcado "já existe", eu anexo essa imagem: use-a como base para manter o mesmo tamanho e estilo.

## Lista de frames

### Efeito — Mansão caindo do céu (apoiado no chão)

- **Frames:** 2 (em loop: o último emenda no primeiro)
- **Animação:** cai do céu sobre o rival; os dois frames alternam durante a queda.
- **Objeto:** uma mansão luxuosa de dois andares, vista de frente, com colunas brancas, telhado vermelho e piscina azul na lateral, inteira dentro do quadro, SEM grama e SEM árvores.
- **Fundo:** magenta chapado #FF00FF.

| # | Arquivo | O que desenhar | Status |
| --- | --- | --- | --- |
| 1 | `VFX/mansao_queda/01.png` | mansão levemente inclinada para a esquerda, linhas de velocidade brancas saindo para CIMA do telhado, algumas notas de dinheiro voando por cima. | já existe, pule |
| 2 | `VFX/mansao_queda/02.png` | mansão levemente inclinada para a direita, linhas de velocidade em outra posição, as notas de dinheiro em outra posição. | já existe, pule |

### Efeito — Mansão desabando (apoiado no chão)

- **Frames:** 5
- **Animação:** aparece no chão logo após a queda e toca uma vez (menos de 1 segundo).
- **Objeto:** a mesma mansão de dois andares com colunas brancas e telhado vermelho, SEM grama e SEM árvores.
- **Fundo:** magenta chapado #FF00FF.

| # | Arquivo | O que desenhar | Status |
| --- | --- | --- | --- |
| 3 | `VFX/mansao_impacto/01.png` | mansão inteira tocando o chão, achatada um pouco pelo impacto, nuvem de poeira bege saindo dos lados. | já existe, pule |
| 4 | `VFX/mansao_impacto/02.png` | mansão rachando ao meio, tijolos e telhas voando, poeira maior. | já existe, pule |
| 5 | `VFX/mansao_impacto/03.png` | mansão desabando: paredes caindo, colunas quebradas, notas de dinheiro e moedas explodindo para cima. | já existe, pule |
| 6 | `VFX/mansao_impacto/04.png` | um monte de escombros baixo, poeira grande cobrindo tudo, dinheiro caindo. | já existe, pule |
| 7 | `VFX/mansao_impacto/05.png` | só a poeira se dissipando e alguns tijolos no chão. | já existe, pule |

### Efeito — Onça correndo (apoiado no chão)

- **Frames:** 6 (em loop: o último emenda no primeiro)
- **Animação:** a onça corre pelo chão até o rival; o jogo repete os frames em loop.
- **Objeto:** uma onça pintada amarela com pintas pretas, musculosa e com cara brava cômica, vista de lado, VIRADA PARA A DIREITA.
- **Fundo:** verde chapado #00FF00.

| # | Arquivo | O que desenhar | Status |
| --- | --- | --- | --- |
| 8 | `VFX/onca/01.png` | patas da frente esticadas bem à frente e patas de trás esticadas atrás (corpo todo esticado no ar). | já existe, pule |
| 9 | `VFX/onca/02.png` | patas da frente tocando o chão, patas de trás ainda atrás. | já existe, pule |
| 10 | `VFX/onca/03.png` | patas recolhidas embaixo do corpo, costas arqueadas para cima (meio da passada). | já existe, pule |
| 11 | `VFX/onca/04.png` | patas de trás empurrando o chão, patas da frente começando a ir para a frente. | já existe, pule |
| 12 | `VFX/onca/05.png` | patas da frente indo à frente, corpo se esticando. | já existe, pule |
| 13 | `VFX/onca/06.png` | quase igual ao frame 1, emendando no começo do ciclo. | já existe, pule |

### Efeito — Rugido da onça

- **Frames:** 3
- **Animação:** aparece em cima do rival quando a onça acerta, uma vez só.
- **Objeto:** a cabeça de uma onça pintada, vista de lado, VIRADA PARA A DIREITA.
- **Fundo:** verde chapado #00FF00.

| # | Arquivo | O que desenhar | Status |
| --- | --- | --- | --- |
| 14 | `VFX/onca_rugido/01.png` | boca fechada, olhos estreitos e bravos, orelhas para trás. | já existe, pule |
| 15 | `VFX/onca_rugido/02.png` | boca bem aberta rugindo, presas à mostra, linhas curvas de som saindo da boca. | já existe, pule |
| 16 | `VFX/onca_rugido/03.png` | rugido máximo: boca escancarada, ondas de som amarelas maiores, saliva voando. | já existe, pule |

### Efeito — L gigante explodindo

- **Frames:** 5
- **Animação:** aparece na frente do Lula e toca uma vez só (cerca de meio segundo).
- **Objeto:** uma letra L maiúscula enorme, vermelha, com contorno branco grosso.
- **Fundo:** verde chapado #00FF00.

| # | Arquivo | O que desenhar | Status |
| --- | --- | --- | --- |
| 17 | `VFX/faz_o_l/01.png` | o L surgindo: pequeno, no centro do quadro, com estrelinhas brancas em volta. | já existe, pule |
| 18 | `VFX/faz_o_l/02.png` | o L em tamanho cheio, ocupando boa parte do quadro, brilho branco nas bordas. | já existe, pule |
| 19 | `VFX/faz_o_l/03.png` | o L brilhando forte, raios de luz amarelos saindo de trás dele. | já existe, pule |
| 20 | `VFX/faz_o_l/04.png` | explosão: o L se partindo em pedaços vermelhos com um clarão amarelo e branco no meio. | já existe, pule |
| 21 | `VFX/faz_o_l/05.png` | só fagulhas e fumaça vermelha se dissipando no lugar onde estava o L. | já existe, pule |

### Efeito — Maço de dinheiro girando

- **Frames:** 4 (em loop: o último emenda no primeiro)
- **Animação:** projétil arremessado em leque; o jogo toca os frames em loop enquanto ele voa.
- **Objeto:** um maço grosso de cédulas de dinheiro verdes preso com um elástico vermelho.
- **Fundo:** magenta chapado #FF00FF.

| # | Arquivo | O que desenhar | Status |
| --- | --- | --- | --- |
| 22 | `VFX/mensalao_maco/01.png` | maço visto de frente, inteiro, retangular e na horizontal. | já existe, pule |
| 23 | `VFX/mensalao_maco/02.png` | maço girado 45 graus (inclinado), algumas notas levemente abertas como um leque. | já existe, pule |
| 24 | `VFX/mensalao_maco/03.png` | maço visto de lado, bem fino, quase de perfil, com as bordas das notas aparecendo. | já existe, pule |
| 25 | `VFX/mensalao_maco/04.png` | maço girado 135 graus, inclinado para o outro lado, notas abertas como um leque. | já existe, pule |

### Efeito — Nota voando

- **Frames:** 3 (em loop: o último emenda no primeiro)
- **Animação:** notas que espirram quando o maço acerta; ficam em loop enquanto caem.
- **Objeto:** uma única cédula de dinheiro verde.
- **Fundo:** magenta chapado #FF00FF.

| # | Arquivo | O que desenhar | Status |
| --- | --- | --- | --- |
| 26 | `VFX/nota/01.png` | nota quase plana, levemente curvada para cima nas pontas. | já existe, pule |
| 27 | `VFX/nota/02.png` | nota dobrada em S, ondulando no ar. | já existe, pule |
| 28 | `VFX/nota/03.png` | nota curvada para baixo nas pontas, um pouco de lado. | já existe, pule |

### Efeito — Corte viral

- **Frames:** 4
- **Animação:** aparece na frente do Renan durante a investida e toca uma vez (cerca de 1/3 de segundo).
- **Objeto:** um rastro de corte em arco, roxo e branco, como o golpe de uma espada.
- **Fundo:** verde chapado #00FF00.

| # | Arquivo | O que desenhar | Status |
| --- | --- | --- | --- |
| 29 | `VFX/corte_viral/01.png` | início do corte: um traço fino roxo começando no alto do quadro. | já existe, pule |
| 30 | `VFX/corte_viral/02.png` | arco de corte grande e largo, roxo com o miolo branco brilhante, curvando de cima para baixo. | já existe, pule |
| 31 | `VFX/corte_viral/03.png` | arco completo, com pequenos ícones de "play" e corações roxos voando da ponta. | já existe, pule |
| 32 | `VFX/corte_viral/04.png` | o arco se desfazendo em pixels roxos que somem. | já existe, pule |

### Efeito — Fralda girando

- **Frames:** 4 (em loop: o último emenda no primeiro)
- **Animação:** projétil jogado em arco; fica em loop enquanto voa.
- **Objeto:** uma fralda descartável branca fechada, com fitas azuis e uma mancha marrom cômica, com linhas onduladas verdes de mau cheiro.
- **Fundo:** magenta chapado #FF00FF.

| # | Arquivo | O que desenhar | Status |
| --- | --- | --- | --- |
| 33 | `VFX/fralda/01.png` | fralda vista de frente, em pé. | já existe, pule |
| 34 | `VFX/fralda/02.png` | fralda girada 90 graus, deitada de lado. | já existe, pule |
| 35 | `VFX/fralda/03.png` | fralda de cabeça para baixo. | já existe, pule |
| 36 | `VFX/fralda/04.png` | fralda girada 270 graus, deitada para o outro lado. | já existe, pule |

### Efeito — Nuvem de fedor

- **Frames:** 4
- **Animação:** aparece em cima do rival quando a fralda acerta, uma vez só (menos de 1 segundo).
- **Objeto:** uma nuvem de fedor verde-amarelada, com moscas pretas e linhas onduladas.
- **Fundo:** magenta chapado #FF00FF.

| # | Arquivo | O que desenhar | Status |
| --- | --- | --- | --- |
| 37 | `VFX/fedor/01.png` | nuvem pequena e densa no centro, verde-amarelada. | já existe, pule |
| 38 | `VFX/fedor/02.png` | nuvem crescendo, ondas de cheiro subindo, duas moscas voando. | já existe, pule |
| 39 | `VFX/fedor/03.png` | nuvem grande ocupando quase todo o quadro, várias moscas e linhas onduladas. | já existe, pule |
| 40 | `VFX/fedor/04.png` | nuvem se desfazendo em fiapos claros, quase transparente. | já existe, pule |

### Efeito — Tarja de censura voando

- **Frames:** 3 (em loop: o último emenda no primeiro)
- **Animação:** projétil que voa reto para a direita; fica em loop enquanto voa.
- **Objeto:** uma tarja retangular preta, bem comprida na horizontal, com a palavra CENSURADO em letras brancas de pixel.
- **Fundo:** verde chapado #00FF00.

| # | Arquivo | O que desenhar | Status |
| --- | --- | --- | --- |
| 41 | `VFX/censura_tarja/01.png` | tarja reta, na horizontal, com 3 linhas de velocidade brancas atrás dela (à esquerda). | já existe, pule |
| 42 | `VFX/censura_tarja/02.png` | tarja levemente ondulada como um pano, linhas de velocidade mais longas. | já existe, pule |
| 43 | `VFX/censura_tarja/03.png` | tarja ondulada para o outro lado, uma faísca branca na ponta da frente. | já existe, pule |

### Efeito — Bombom girando

- **Frames:** 4 (em loop: o último emenda no primeiro)
- **Animação:** três bombons quicam pelo chão; ficam em loop enquanto voam.
- **Objeto:** um bombom de chocolate redondo embrulhado em papel dourado torcido nas pontas.
- **Fundo:** verde chapado #00FF00.

| # | Arquivo | O que desenhar | Status |
| --- | --- | --- | --- |
| 44 | `VFX/bombom/01.png` | bombom na horizontal, pontas torcidas para os lados. | já existe, pule |
| 45 | `VFX/bombom/02.png` | bombom girado 45 graus. | já existe, pule |
| 46 | `VFX/bombom/03.png` | bombom na vertical, pontas torcidas para cima e para baixo. | já existe, pule |
| 47 | `VFX/bombom/04.png` | bombom girado 135 graus. | já existe, pule |

### Efeito — Moeda girando

- **Frames:** 4 (em loop: o último emenda no primeiro)
- **Animação:** moedas que espirram nos acertos; ficam em loop enquanto caem.
- **Objeto:** uma moeda dourada brilhante com um cifrão em relevo.
- **Fundo:** verde chapado #00FF00.

| # | Arquivo | O que desenhar | Status |
| --- | --- | --- | --- |
| 48 | `VFX/moeda/01.png` | moeda vista de frente, redonda e inteira, brilho branco no canto superior. | já existe, pule |
| 49 | `VFX/moeda/02.png` | moeda girada, virou uma elipse fina na vertical. | já existe, pule |
| 50 | `VFX/moeda/03.png` | moeda de perfil: só uma faixa dourada fina na vertical. | já existe, pule |
| 51 | `VFX/moeda/04.png` | moeda girada mostrando o verso, elipse na vertical, brilho do outro lado. | já existe, pule |

### Efeito — Picanha chiando

- **Frames:** 4 (em loop: o último emenda no primeiro)
- **Animação:** fica parada na plataforma até alguém pegar; o jogo repete os frames em loop.
- **Objeto:** uma peça de picanha assada suculenta com capa de gordura dourada, vista de lado, num espeto curto.
- **Fundo:** verde chapado #00FF00.

| # | Arquivo | O que desenhar | Status |
| --- | --- | --- | --- |
| 52 | `VFX/picanha/01.png` | picanha com duas gotas de gordura brilhando e um fio de vapor branco subindo. | já existe, pule |
| 53 | `VFX/picanha/02.png` | mesma picanha, vapor mais alto, uma gota de gordura escorrendo. | já existe, pule |
| 54 | `VFX/picanha/03.png` | mesma picanha, vapor se abrindo em dois fios, brilho na capa de gordura. | já existe, pule |
| 55 | `VFX/picanha/04.png` | mesma picanha, vapor curto, quase igual ao frame 1. | já existe, pule |

### Efeito — Drone com sirene

- **Frames:** 4 (em loop: o último emenda no primeiro)
- **Animação:** voa até o rival; o jogo repete os frames em loop enquanto ele voa.
- **Objeto:** um drone quadricóptero pequeno, corpo preto e cinza, quatro hélices, uma sirene em cima com uma luz vermelha e uma azul, visto de lado.
- **Fundo:** verde chapado #00FF00.

| # | Arquivo | O que desenhar | Status |
| --- | --- | --- | --- |
| 56 | `VFX/drone_sirene/01.png` | drone com as hélices borradas de movimento, luz vermelha da sirene acesa e a azul apagada. | já existe, pule |
| 57 | `VFX/drone_sirene/02.png` | mesmo drone levemente inclinado para a frente, as duas luzes da sirene apagadas. | já existe, pule |
| 58 | `VFX/drone_sirene/03.png` | mesmo drone, luz azul da sirene acesa e a vermelha apagada, pequenos raios de luz azul. | já existe, pule |
| 59 | `VFX/drone_sirene/04.png` | mesmo drone voltando à posição do frame 1, as duas luzes apagadas. | já existe, pule |

### Efeito — Robozinho de IA (apoiado no chão)

- **Frames:** 4 (em loop: o último emenda no primeiro)
- **Animação:** três deles atravessam a arena andando e empurram o rival; o jogo repete os frames em loop.
- **Objeto:** um robozinho quadrado cinza-claro, com uma tela azul no lugar do rosto mostrando dois olhinhos e um sorriso, pernas curtas e um carimbo de madeira na mão, visto de lado e virado para a DIREITA.
- **Fundo:** verde chapado #00FF00.

| # | Arquivo | O que desenhar | Status |
| --- | --- | --- | --- |
| 60 | `VFX/robo_ia/01.png` | robozinho dando um passo com a perna da frente esticada, carimbo erguido. | já existe, pule |
| 61 | `VFX/robo_ia/02.png` | robozinho com as duas pernas juntas, corpo um pouco mais alto, carimbo à frente. | já existe, pule |
| 62 | `VFX/robo_ia/03.png` | robozinho dando um passo com a outra perna, carimbo abaixando. | já existe, pule |
| 63 | `VFX/robo_ia/04.png` | robozinho com as pernas juntas, quase igual ao frame 2, olhinhos piscando. | já existe, pule |

### Efeito — Zzz de sono

- **Frames:** 3 (em loop: o último emenda no primeiro)
- **Animação:** flutua sobre a cabeça do rival enquanto ele dorme em pé; o jogo repete os frames em loop.
- **Objeto:** três letras Z brancas com contorno azul-escuro, de tamanhos diferentes (pequena, média e grande), subindo na diagonal como num desenho animado, com duas bolhinhas de sono.
- **Fundo:** verde chapado #00FF00.

| # | Arquivo | O que desenhar | Status |
| --- | --- | --- | --- |
| 64 | `VFX/zzz_sono/01.png` | Z pequeno embaixo, Z médio no meio, Z grande em cima, alinhados na diagonal. | já existe, pule |
| 65 | `VFX/zzz_sono/02.png` | os três Z subiram um pouco e balançam para a direita, uma bolhinha aparecendo embaixo. | já existe, pule |
| 66 | `VFX/zzz_sono/03.png` | Z grande sumiu, um Z pequeno novo nascendo embaixo, quase igual ao frame 1. | já existe, pule |

### Efeito — Raio da careca

- **Frames:** 4 (em loop: o último emenda no primeiro)
- **Animação:** sai da careca e atravessa a tela em linha reta; o jogo estica o feixe e repete os frames em loop.
- **Objeto:** um feixe de luz horizontal, reto e comprido, branco no centro e amarelo-claro nas bordas, ocupando a largura do quadro, com pequenas estrelinhas de brilho.
- **Fundo:** verde chapado #00FF00.

| # | Arquivo | O que desenhar | Status |
| --- | --- | --- | --- |
| 67 | `VFX/raio_careca/01.png` | feixe fino e reto, estrelinhas pequenas nas bordas. |  |
| 68 | `VFX/raio_careca/02.png` | mesmo feixe um pouco mais grosso e mais brilhante, estrelinhas mudando de lugar. |  |
| 69 | `VFX/raio_careca/03.png` | feixe no máximo de grossura, centro branco puro, faíscas saindo para cima e para baixo. |  |
| 70 | `VFX/raio_careca/04.png` | feixe voltando à grossura do frame 1, estrelinhas em outras posições. |  |

### Efeito — Bola de feno rolando (apoiado no chão)

- **Frames:** 4 (em loop: o último emenda no primeiro)
- **Animação:** passa rolando pelo chão durante o encaramento do duelo; o jogo repete os frames em loop.
- **Objeto:** uma bola de feno seco de faroeste, marrom-amarelada, redonda e emaranhada, com uma pequena nuvem de poeira embaixo.
- **Fundo:** magenta chapado #FF00FF.

| # | Arquivo | O que desenhar | Status |
| --- | --- | --- | --- |
| 71 | `VFX/feno_rolando/01.png` | bola de feno apoiada no chão, galhos na posição inicial. |  |
| 72 | `VFX/feno_rolando/02.png` | mesma bola girada 90 graus, pouca poeira. |  |
| 73 | `VFX/feno_rolando/03.png` | mesma bola girada 180 graus, nuvem de poeira um pouco maior. |  |
| 74 | `VFX/feno_rolando/04.png` | mesma bola girada 270 graus, poeira quase sumindo. |  |

### Efeito — Respingo de tinta

- **Frames:** 4
- **Animação:** aparece no rival quando o saque da caneta acerta; os frames tocam uma vez.
- **Objeto:** um respingo de tinta de caneta azul-escura, em forma de estrela irregular, com gotas voando.
- **Fundo:** verde chapado #00FF00.

| # | Arquivo | O que desenhar | Status |
| --- | --- | --- | --- |
| 75 | `VFX/tinta_tiro/01.png` | gota de tinta pequena e redonda no centro. |  |
| 76 | `VFX/tinta_tiro/02.png` | respingo se abrindo em estrela, gotas saindo para os lados. |  |
| 77 | `VFX/tinta_tiro/03.png` | respingo no tamanho máximo, gotas mais longe. |  |
| 78 | `VFX/tinta_tiro/04.png` | respingo escorrendo e ficando mais claro, gotas caindo. |  |

### Efeito — Fumaça branca do conclave (apoiado no chão)

- **Frames:** 4
- **Animação:** sobe em volta dele quando ativa a habilidade; os frames tocam uma vez.
- **Objeto:** uma coluna de fumaça branca e cinza-clara, fofa, subindo do chão como uma chaminé de conclave.
- **Fundo:** verde chapado #00FF00.

| # | Arquivo | O que desenhar | Status |
| --- | --- | --- | --- |
| 79 | `VFX/fumaca_conclave/01.png` | poucos novelos de fumaça branca junto ao chão. |  |
| 80 | `VFX/fumaca_conclave/02.png` | coluna de fumaça subindo até a metade do quadro. |  |
| 81 | `VFX/fumaca_conclave/03.png` | coluna alta até o topo do quadro, se abrindo em cima. |  |
| 82 | `VFX/fumaca_conclave/04.png` | fumaça se espalhando e ficando transparente nas bordas. |  |

### Efeito — Mitra de papa

- **Frames:** 3 (em loop: o último emenda no primeiro)
- **Animação:** fica sobre a cabeça dele enquanto a superarmadura dura; o jogo repete os frames em loop.
- **Objeto:** uma mitra de papa branca com faixas e cruz douradas, vista de lado, sozinha, sem cabeça.
- **Fundo:** verde chapado #00FF00.

| # | Arquivo | O que desenhar | Status |
| --- | --- | --- | --- |
| 83 | `VFX/mitra/01.png` | mitra com um brilho dourado pequeno na cruz. |  |
| 84 | `VFX/mitra/02.png` | mesma mitra, brilho dourado maior, duas estrelinhas em volta. |  |
| 85 | `VFX/mitra/03.png` | mesma mitra, brilho voltando ao tamanho do frame 1, estrelinhas em outras posições. |  |
