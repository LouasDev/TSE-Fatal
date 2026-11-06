# Prompt único — todos os efeitos num chat só

Jeito mais rápido de gerar os 85 frames dos 22 efeitos.

1. Abra um chat novo no ChatGPT (de preferência anexe a imagem base de um personagem, para combinar o estilo).
2. Cole o bloco abaixo inteiro. O ChatGPT gera o frame 1.
3. Salve a imagem em `Assets/Resources/Art/VFX/` + o nome que ele escrever (ex.: `Assets/Resources/Art/VFX/mansao_queda/01.png`).
4. Mande **próximo** para o frame seguinte, ou **refaça** se não gostou.

Se o estilo começar a se perder depois de muitos frames, abra um chat novo, cole o bloco de novo e escreva "comece pelo item N".

```
Vou gerar sprites de EFEITOS animados para um jogo de luta 2D em pixel art, estilo arcade dos anos 90, em tom de sátira cômica. Abaixo está a lista completa de frames, numerada. Gere UMA imagem por resposta, na ordem da lista, começando pelo item 1.

Depois de cada imagem, escreva apenas o nome do arquivo do item (por exemplo: mansao_queda/01.png) e espere. Quando eu escrever "próximo", gere o item seguinte. Quando eu escrever "refaça", gere o mesmo item de novo.

REGRAS FIXAS (valem para todas as imagens):
1. Quadro quadrado 1024x1024, um único objeto por imagem.
2. Dentro do mesmo efeito, o objeto tem SEMPRE o mesmo tamanho, o mesmo desenho e a mesma posição no quadro; só muda o movimento descrito. Use a imagem anterior do mesmo efeito como referência.
3. Quando o objeto tiver frente e trás, ele fica virado para a DIREITA.
4. Fundo de uma cor só, chapada, sem gradiente, sem sombra e sem chão desenhado, na cor indicada em cada efeito (VERDE #00FF00 ou MAGENTA #FF00FF).
5. "Centralizado" = objeto no meio do quadro. "Apoiado na borda de baixo" = objeto encostado perto da borda de baixo, como se estivesse no chão.
6. Pixel art nítida, contorno escuro de 1 pixel, cores vivas, sem moldura, sem marca d'água e sem texto (as únicas exceções são a palavra CENSURADO na tarja de censura e as letras Z do sono).
7. Nos efeitos em loop, o último frame precisa emendar suavemente com o primeiro.

LISTA DE FRAMES:

EFEITO "Mansão caindo do céu" (Flávio — Mansão de 6 Milhões (L)) — 2 frames, em loop, apoiado na borda de baixo, fundo MAGENTA #FF00FF
Objeto: uma mansão luxuosa de dois andares, vista de frente, com colunas brancas, telhado vermelho e piscina azul na lateral, inteira dentro do quadro, SEM grama e SEM árvores.
  1. mansao_queda/01.png — mansão levemente inclinada para a esquerda, linhas de velocidade brancas saindo para CIMA do telhado, algumas notas de dinheiro voando por cima
  2. mansao_queda/02.png — mansão levemente inclinada para a direita, linhas de velocidade em outra posição, as notas de dinheiro em outra posição

EFEITO "Mansão desabando" (Flávio — Mansão de 6 Milhões (L)) — 5 frames, apoiado na borda de baixo, fundo MAGENTA #FF00FF
Objeto: a mesma mansão de dois andares com colunas brancas e telhado vermelho, SEM grama e SEM árvores.
  3. mansao_impacto/01.png — mansão inteira tocando o chão, achatada um pouco pelo impacto, nuvem de poeira bege saindo dos lados
  4. mansao_impacto/02.png — mansão rachando ao meio, tijolos e telhas voando, poeira maior
  5. mansao_impacto/03.png — mansão desabando: paredes caindo, colunas quebradas, notas de dinheiro e moedas explodindo para cima
  6. mansao_impacto/04.png — um monte de escombros baixo, poeira grande cobrindo tudo, dinheiro caindo
  7. mansao_impacto/05.png — só a poeira se dissipando e alguns tijolos no chão

EFEITO "Onça correndo" (Renan — Onça Liberal (L)) — 6 frames, em loop, apoiado na borda de baixo, fundo VERDE #00FF00
Objeto: uma onça pintada amarela com pintas pretas, musculosa e com cara brava cômica, vista de lado, VIRADA PARA A DIREITA.
  8. onca/01.png — patas da frente esticadas bem à frente e patas de trás esticadas atrás (corpo todo esticado no ar)
  9. onca/02.png — patas da frente tocando o chão, patas de trás ainda atrás
  10. onca/03.png — patas recolhidas embaixo do corpo, costas arqueadas para cima (meio da passada)
  11. onca/04.png — patas de trás empurrando o chão, patas da frente começando a ir para a frente
  12. onca/05.png — patas da frente indo à frente, corpo se esticando
  13. onca/06.png — quase igual ao frame 1, emendando no começo do ciclo

EFEITO "Rugido da onça" (Renan — Onça Liberal (L)) — 3 frames, centralizado, fundo VERDE #00FF00
Objeto: a cabeça de uma onça pintada, vista de lado, VIRADA PARA A DIREITA.
  14. onca_rugido/01.png — boca fechada, olhos estreitos e bravos, orelhas para trás
  15. onca_rugido/02.png — boca bem aberta rugindo, presas à mostra, linhas curvas de som saindo da boca
  16. onca_rugido/03.png — rugido máximo: boca escancarada, ondas de som amarelas maiores, saliva voando

EFEITO "L gigante explodindo" (Lula — Faz o L (L)) — 5 frames, centralizado, fundo VERDE #00FF00
Objeto: uma letra L maiúscula enorme, vermelha, com contorno branco grosso.
  17. faz_o_l/01.png — o L surgindo: pequeno, no centro do quadro, com estrelinhas brancas em volta
  18. faz_o_l/02.png — o L em tamanho cheio, ocupando boa parte do quadro, brilho branco nas bordas
  19. faz_o_l/03.png — o L brilhando forte, raios de luz amarelos saindo de trás dele
  20. faz_o_l/04.png — explosão: o L se partindo em pedaços vermelhos com um clarão amarelo e branco no meio
  21. faz_o_l/05.png — só fagulhas e fumaça vermelha se dissipando no lugar onde estava o L

EFEITO "Maço de dinheiro girando" (Lula — Mensalão (K)) — 4 frames, em loop, centralizado, fundo MAGENTA #FF00FF
Objeto: um maço grosso de cédulas de dinheiro verdes preso com um elástico vermelho.
  22. mensalao_maco/01.png — maço visto de frente, inteiro, retangular e na horizontal
  23. mensalao_maco/02.png — maço girado 45 graus (inclinado), algumas notas levemente abertas como um leque
  24. mensalao_maco/03.png — maço visto de lado, bem fino, quase de perfil, com as bordas das notas aparecendo
  25. mensalao_maco/04.png — maço girado 135 graus, inclinado para o outro lado, notas abertas como um leque

EFEITO "Nota voando" (Lula — Mensalão (K)) — 3 frames, em loop, centralizado, fundo MAGENTA #FF00FF
Objeto: uma única cédula de dinheiro verde.
  26. nota/01.png — nota quase plana, levemente curvada para cima nas pontas
  27. nota/02.png — nota dobrada em S, ondulando no ar
  28. nota/03.png — nota curvada para baixo nas pontas, um pouco de lado

EFEITO "Corte viral" (Renan — Corte Viral (K)) — 4 frames, centralizado, fundo VERDE #00FF00
Objeto: um rastro de corte em arco, roxo e branco, como o golpe de uma espada.
  29. corte_viral/01.png — início do corte: um traço fino roxo começando no alto do quadro
  30. corte_viral/02.png — arco de corte grande e largo, roxo com o miolo branco brilhante, curvando de cima para baixo
  31. corte_viral/03.png — arco completo, com pequenos ícones de "play" e corações roxos voando da ponta
  32. corte_viral/04.png — o arco se desfazendo em pixels roxos que somem

EFEITO "Fralda girando" (Renan — Fralda do Debate (I)) — 4 frames, em loop, centralizado, fundo MAGENTA #FF00FF
Objeto: uma fralda descartável branca fechada, com fitas azuis e uma mancha marrom cômica, com linhas onduladas verdes de mau cheiro.
  33. fralda/01.png — fralda vista de frente, em pé
  34. fralda/02.png — fralda girada 90 graus, deitada de lado
  35. fralda/03.png — fralda de cabeça para baixo
  36. fralda/04.png — fralda girada 270 graus, deitada para o outro lado

EFEITO "Nuvem de fedor" (Renan — Fralda do Debate (I)) — 4 frames, centralizado, fundo MAGENTA #FF00FF
Objeto: uma nuvem de fedor verde-amarelada, com moscas pretas e linhas onduladas.
  37. fedor/01.png — nuvem pequena e densa no centro, verde-amarelada
  38. fedor/02.png — nuvem crescendo, ondas de cheiro subindo, duas moscas voando
  39. fedor/03.png — nuvem grande ocupando quase todo o quadro, várias moscas e linhas onduladas
  40. fedor/04.png — nuvem se desfazendo em fiapos claros, quase transparente

EFEITO "Tarja de censura voando" (Lula — Censura (I)) — 3 frames, em loop, centralizado, fundo VERDE #00FF00
Objeto: uma tarja retangular preta, bem comprida na horizontal, com a palavra CENSURADO em letras brancas de pixel.
  41. censura_tarja/01.png — tarja reta, na horizontal, com 3 linhas de velocidade brancas atrás dela (à esquerda)
  42. censura_tarja/02.png — tarja levemente ondulada como um pano, linhas de velocidade mais longas
  43. censura_tarja/03.png — tarja ondulada para o outro lado, uma faísca branca na ponta da frente

EFEITO "Bombom girando" (Flávio — Chocolate Lavado (I)) — 4 frames, em loop, centralizado, fundo VERDE #00FF00
Objeto: um bombom de chocolate redondo embrulhado em papel dourado torcido nas pontas.
  44. bombom/01.png — bombom na horizontal, pontas torcidas para os lados
  45. bombom/02.png — bombom girado 45 graus
  46. bombom/03.png — bombom na vertical, pontas torcidas para cima e para baixo
  47. bombom/04.png — bombom girado 135 graus

EFEITO "Moeda girando" (Flávio — Rachadinha (K) e picanha) — 4 frames, em loop, centralizado, fundo VERDE #00FF00
Objeto: uma moeda dourada brilhante com um cifrão em relevo.
  48. moeda/01.png — moeda vista de frente, redonda e inteira, brilho branco no canto superior
  49. moeda/02.png — moeda girada, virou uma elipse fina na vertical
  50. moeda/03.png — moeda de perfil: só uma faixa dourada fina na vertical
  51. moeda/04.png — moeda girada mostrando o verso, elipse na vertical, brilho do outro lado

EFEITO "Picanha chiando" (Item de cura (picanha)) — 4 frames, em loop, centralizado, fundo VERDE #00FF00
Objeto: uma peça de picanha assada suculenta com capa de gordura dourada, vista de lado, num espeto curto.
  52. picanha/01.png — picanha com duas gotas de gordura brilhando e um fio de vapor branco subindo
  53. picanha/02.png — mesma picanha, vapor mais alto, uma gota de gordura escorrendo
  54. picanha/03.png — mesma picanha, vapor se abrindo em dois fios, brilho na capa de gordura
  55. picanha/04.png — mesma picanha, vapor curto, quase igual ao frame 1

EFEITO "Drone com sirene" (Cury — Drone com Sirene (K)) — 4 frames, em loop, centralizado, fundo VERDE #00FF00
Objeto: um drone quadricóptero pequeno, corpo preto e cinza, quatro hélices, uma sirene em cima com uma luz vermelha e uma azul, visto de lado.
  56. drone_sirene/01.png — drone com as hélices borradas de movimento, luz vermelha da sirene acesa e a azul apagada
  57. drone_sirene/02.png — mesmo drone levemente inclinado para a frente, as duas luzes da sirene apagadas
  58. drone_sirene/03.png — mesmo drone, luz azul da sirene acesa e a vermelha apagada, pequenos raios de luz azul
  59. drone_sirene/04.png — mesmo drone voltando à posição do frame 1, as duas luzes apagadas

EFEITO "Robozinho de IA" (Cury — Ministério da IA (L)) — 4 frames, em loop, apoiado na borda de baixo, fundo VERDE #00FF00
Objeto: um robozinho quadrado cinza-claro, com uma tela azul no lugar do rosto mostrando dois olhinhos e um sorriso, pernas curtas e um carimbo de madeira na mão, visto de lado e virado para a DIREITA.
  60. robo_ia/01.png — robozinho dando um passo com a perna da frente esticada, carimbo erguido
  61. robo_ia/02.png — robozinho com as duas pernas juntas, corpo um pouco mais alto, carimbo à frente
  62. robo_ia/03.png — robozinho dando um passo com a outra perna, carimbo abaixando
  63. robo_ia/04.png — robozinho com as pernas juntas, quase igual ao frame 2, olhinhos piscando

EFEITO "Zzz de sono" (Cury — Palestra Motivacional (I)) — 3 frames, em loop, centralizado, fundo VERDE #00FF00
Objeto: três letras Z brancas com contorno azul-escuro, de tamanhos diferentes (pequena, média e grande), subindo na diagonal como num desenho animado, com duas bolhinhas de sono.
  64. zzz_sono/01.png — Z pequeno embaixo, Z médio no meio, Z grande em cima, alinhados na diagonal
  65. zzz_sono/02.png — os três Z subiram um pouco e balançam para a direita, uma bolhinha aparecendo embaixo
  66. zzz_sono/03.png — Z grande sumiu, um Z pequeno novo nascendo embaixo, quase igual ao frame 1

EFEITO "Raio da careca" (Moraes — Reflexo da Careca (K)) — 4 frames, em loop, centralizado, fundo VERDE #00FF00
Objeto: um feixe de luz horizontal, reto e comprido, branco no centro e amarelo-claro nas bordas, ocupando a largura do quadro, com pequenas estrelinhas de brilho.
  67. raio_careca/01.png — feixe fino e reto, estrelinhas pequenas nas bordas
  68. raio_careca/02.png — mesmo feixe um pouco mais grosso e mais brilhante, estrelinhas mudando de lugar
  69. raio_careca/03.png — feixe no máximo de grossura, centro branco puro, faíscas saindo para cima e para baixo
  70. raio_careca/04.png — feixe voltando à grossura do frame 1, estrelinhas em outras posições

EFEITO "Bola de feno rolando" (Moraes — Duelo de Faroeste (L)) — 4 frames, em loop, apoiado na borda de baixo, fundo MAGENTA #FF00FF
Objeto: uma bola de feno seco de faroeste, marrom-amarelada, redonda e emaranhada, com uma pequena nuvem de poeira embaixo.
  71. feno_rolando/01.png — bola de feno apoiada no chão, galhos na posição inicial
  72. feno_rolando/02.png — mesma bola girada 90 graus, pouca poeira
  73. feno_rolando/03.png — mesma bola girada 180 graus, nuvem de poeira um pouco maior
  74. feno_rolando/04.png — mesma bola girada 270 graus, poeira quase sumindo

EFEITO "Respingo de tinta" (Moraes — Duelo de Faroeste (L)) — 4 frames, centralizado, fundo VERDE #00FF00
Objeto: um respingo de tinta de caneta azul-escura, em forma de estrela irregular, com gotas voando.
  75. tinta_tiro/01.png — gota de tinta pequena e redonda no centro
  76. tinta_tiro/02.png — respingo se abrindo em estrela, gotas saindo para os lados
  77. tinta_tiro/03.png — respingo no tamanho máximo, gotas mais longe
  78. tinta_tiro/04.png — respingo escorrendo e ficando mais claro, gotas caindo

EFEITO "Fumaça branca do conclave" (Moraes — Habemus Xandão (I)) — 4 frames, apoiado na borda de baixo, fundo VERDE #00FF00
Objeto: uma coluna de fumaça branca e cinza-clara, fofa, subindo do chão como uma chaminé de conclave.
  79. fumaca_conclave/01.png — poucos novelos de fumaça branca junto ao chão
  80. fumaca_conclave/02.png — coluna de fumaça subindo até a metade do quadro
  81. fumaca_conclave/03.png — coluna alta até o topo do quadro, se abrindo em cima
  82. fumaca_conclave/04.png — fumaça se espalhando e ficando transparente nas bordas

EFEITO "Mitra de papa" (Moraes — Habemus Xandão (I)) — 3 frames, em loop, centralizado, fundo VERDE #00FF00
Objeto: uma mitra de papa branca com faixas e cruz douradas, vista de lado, sozinha, sem cabeça.
  83. mitra/01.png — mitra com um brilho dourado pequeno na cruz
  84. mitra/02.png — mesma mitra, brilho dourado maior, duas estrelinhas em volta
  85. mitra/03.png — mesma mitra, brilho voltando ao tamanho do frame 1, estrelinhas em outras posições
```
