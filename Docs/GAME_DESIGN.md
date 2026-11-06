# TSE Fatal — design do jogo

## Proposta

Jogo de luta 2D em plataforma, inspirado no Brawlhalla, em pixel art e tom de sátira ficcional. O jogador escolhe Lula, Renan Santos, Flávio Bolsonaro ou Augusto Cury; o adversário é controlado por um bot.

## Regras da luta

- Arena: Palácio do Planalto, com chão principal e três plataformas atravessáveis por baixo.
- Dano acumulado em %: quanto maior, mais longe o golpe arremessa.
- Cada lutador tem 3 vidas. Sair dos limites da tela tira uma vida ("CASSADO!").
- Vence quem eliminar todas as vidas do rival ("ELEITO!").

## Controles

| Tecla | Ação |
| --- | --- |
| A / D | Mover |
| W / Espaço | Pular (até 2 pulos no ar) |
| S | Descer da plataforma / queda rápida |
| Shift | Esquiva (invulnerável por um instante) |
| J | Ataque (combo de 3 socos no chão, chute no ar) |
| K / L / I | Habilidades especiais |
| Esc | Voltar ao menu |

## Habilidades

| Candidato | K | L | I |
| --- | --- | --- | --- |
| Lula | **Mensalão**: arremessa 3 maços de dinheiro em leque. | **Faz o L**: explosão vermelha de curto alcance. | **Censura**: tarja que deixa o rival sem habilidades por 3,5 s. |
| Renan Santos | **Corte Viral**: investida muito rápida. | **Onça Liberal**: onça que corre pelo chão. | **Fralda do Debate**: fralda em arco; o rival fica tonto, com esquerda e direita invertidas por 3 s. |
| Flávio Bolsonaro | **Rachadinha**: investida com a maleta que recupera 4% de dano. | **Mansão de 6 Milhões**: mansão cai sobre o rival (atravessa as plataformas finas). | **Chocolate Lavado**: 3 bombons que quicam; cada acerto recupera 2% de dano. |
| Augusto Cury | **Drone com Sirene**: drone que persegue o rival pelo ar. | **Ministério da IA**: 3 robôs que marcham pelo chão e empurram o rival. | **Palestra Motivacional**: o rival próximo e no chão dorme em pé por 2,2 s; um golpe acorda. |

## Eventos e humor

- **Picanha:** a cada 15–22 s cai uma picanha numa plataforma. Quem pegar recupera 15% de dano.
- **Faixas de nocaute:** sorteadas entre "CASSADO!", "INELEGÍVEL!", "IMPEACHMENT!", "DELATADO!", "CPI NELE!" e "VAI PRA PAPUDA!".
- **Falas:** cada personagem tem frases ao perder uma vida, ao voltar e ao vencer (`CharacterProfile`). As referências vêm de casos noticiados (Mensalão, triplex, sítio de Atibaia, rachadinha, Queiroz, loja de chocolate, mansão em Brasília, a fralda da convenção do Missão) e são tratadas como piada, não como afirmação de fatos.
- **Som:** música chiptune própria no menu e na batalha (acelera quando alguém está na última vida), além de efeitos por habilidade: caixa registradora, bip de censura, rugido, assobio de queda, torcida etc.
- **Frases na tela:** piadas ficam ~2,4 s (paradas antes de subir e sumir); avisos curtos, ~1,4 s. Uma frase nova perto de outra empurra a antiga para cima, sem sobrepor. O Mensalão solta uma piada por arremesso, não uma por maço.
- **Dublagem:** opcional, gravada pelo grupo. Gritos das habilidades, nocaute, volta, vitória e gemido de dano, com variações sorteadas e a música abaixando durante a fala. Roteiro e nomes dos arquivos em `Docs/DUBLAGEM.md`.
- **Efeitos animados:** cada efeito procura frames em `Resources/Art/VFX/{efeito}/`; sem frames, usa o desenho em código. Prompts em `PromptsAnimacao/vfx/`.

## Menu

- **Jogar:** seleção de candidato → tela de VS com o rival sorteado → luta.
- **Demonstração:** bot contra bot com personagens sorteados. Ao fim de cada luta, a próxima começa em 5 s. ESC volta ao menu. Feito para apresentar o jogo sem precisar jogar.
- **Dificuldade:** FÁCIL, NORMAL ou DIFÍCIL (A/D ou Enter alterna). Fica salva entre sessões e vale também para a demonstração.
- **Controles:** painel com as teclas.

## IA

O bot tem tempo de reação ao entrar no alcance, pausa entre ataques, chance de recuar ou hesitar e usa especiais com menos frequência que socos. Os valores mudam com a dificuldade (`BotDifficulty`):

| | Fácil | Normal | Difícil |
| --- | --- | --- | --- |
| Reação ao entrar no alcance | 0,5–0,8 s | 0,25–0,45 s | 0,12–0,24 s |
| Chance de atacar de perto | 32% | 50% | 66% |
| Pausa entre socos | 0,85–1,3 s | 0,5–0,9 s | 0,3–0,55 s |
| Emenda o combo | 5% | 25% | 55% |
| Uso de especiais | metade | base | 1,5× |

## Limites criativos

Todos os elementos devem permanecer claramente satíricos e ficcionais: evitar alegações factuais, discursos reais, imagens reais de manifestações e violência gráfica.
