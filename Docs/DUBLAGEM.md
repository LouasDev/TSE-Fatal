# Dublagem — roteiro de gravação

O jogo toca falas gravadas por vocês nas habilidades, no nocaute, na volta à arena e na vitória. Basta colocar os arquivos na pasta certa com o nome certo. Arquivo que não existir fica em silêncio; o resto do jogo funciona igual.

> As vozes são imitações caricatas feitas por vocês, como parte da sátira. Não usem clonagem de voz por IA nem trechos de áudio reais dos políticos.

## Onde salvar

```
Assets/Resources/Audio/Vozes/
```

Crie a pasta se ela não existir. O nome do arquivo (sem a extensão) é a chave que o jogo procura, por exemplo `lula_mensalao.wav`.

- **Formatos:** `.wav`, `.ogg` ou `.mp3`. Gravação de celular em `.m4a` precisa ser convertida (o Audacity faz isso: Arquivo → Exportar).
- **Variações:** para não repetir sempre a mesma fala, grave versões extras com `_2`, `_3`... (até `_9`). Exemplo: `lula_mensalao.wav`, `lula_mensalao_2.wav`, `lula_mensalao_3.wav`. O jogo sorteia uma delas.

## Dicas de gravação

- Grave em lugar silencioso, com o celular a um palmo da boca.
- Corte o silêncio do começo e do fim; a fala deve começar no primeiro instante do arquivo.
- Normalize o volume (Audacity: Efeito → Normalizar) para todas as falas ficarem parecidas.
- Mono basta. Falas curtas funcionam melhor: grito de habilidade com até 1 s, frases com até 2,5 s.
- Exagere. Enquanto alguém fala, o jogo abaixa a música sozinho.

## Lista de arquivos

Cada personagem tem 12 falas (fora as variações). Os textos de nocaute, volta e vitória são os mesmos que aparecem na tela.

### Lula (`lula_`)

| Arquivo | Quando toca | Fala sugerida |
| --- | --- | --- |
| `lula_mensalao` | Habilidade K | "MENSALÃO!" |
| `lula_faz_o_l` | Habilidade L | "FAZ O L!" |
| `lula_censura` | Habilidade I | "CENSURA!" |
| `lula_nocaute_1` | Perdeu uma vida | "EU NÃO SABIA DE NADA!" |
| `lula_nocaute_2` | Perdeu uma vida | "É PERSEGUIÇÃO!" |
| `lula_nocaute_3` | Perdeu uma vida | "CADÊ MEU PEDALINHO?" |
| `lula_nocaute_4` | Perdeu uma vida | "O TRIPLEX NÃO É MEU!" |
| `lula_volta_1` | Voltou à arena | "VOLTEI, COMPANHEIRO!" |
| `lula_volta_2` | Voltou à arena | "TÔ ELEGÍVEL!" |
| `lula_volta_3` | Voltou à arena | "QUERIDO, VOLTEI!" |
| `lula_vitoria` | Venceu a luta | "A PICANHA É POR MINHA CONTA!" |
| `lula_dano` | Levou golpe forte (às vezes) | gemido curto ("ai!", "ugh!") |

### Renan Santos (`renan_`)

| Arquivo | Quando toca | Fala sugerida |
| --- | --- | --- |
| `renan_corte_viral` | Habilidade K | "CORTE VIRAL!" |
| `renan_onca` | Habilidade L | "SOLTA A ONÇA!" |
| `renan_fralda` | Habilidade I | "TOMA A FRALDA!" |
| `renan_nocaute_1` | Perdeu uma vida | "ISSO VAI VIRAR CORTE!" |
| `renan_nocaute_2` | Perdeu uma vida | "É O SISTEMA!" |
| `renan_nocaute_3` | Perdeu uma vida | "VOU PROCESSAR!" |
| `renan_nocaute_4` | Perdeu uma vida | "FOI MONTAGEM!" |
| `renan_volta_1` | Voltou à arena | "MISSÃO CONTINUA!" |
| `renan_volta_2` | Voltou à arena | "VOLTEI PRO DEBATE!" |
| `renan_volta_3` | Voltou à arena | "BORA VIRALIZAR!" |
| `renan_vitoria` | Venceu a luta | "JÁ VIROU CORTE NO YOUTUBE!" |
| `renan_dano` | Levou golpe forte (às vezes) | gemido curto |

### Flávio Bolsonaro (`flavio_`)

| Arquivo | Quando toca | Fala sugerida |
| --- | --- | --- |
| `flavio_rachadinha` | Habilidade K | "RACHADINHA!" |
| `flavio_mansao` | Habilidade L | "MANSÃO DE 6 MILHÕES!" |
| `flavio_chocolate` | Habilidade I | "CHOCOLATE LAVADO!" |
| `flavio_nocaute_1` | Perdeu uma vida | "CADÊ O QUEIROZ?" |
| `flavio_nocaute_2` | Perdeu uma vida | "FOI O ASSESSOR!" |
| `flavio_nocaute_3` | Perdeu uma vida | "NÃO TENHO NADA A VER!" |
| `flavio_nocaute_4` | Perdeu uma vida | "PAGUEI EM ESPÉCIE!" |
| `flavio_volta_1` | Voltou à arena | "COMPREI OUTRA MANSÃO!" |
| `flavio_volta_2` | Voltou à arena | "HABEAS CORPUS!" |
| `flavio_volta_3` | Voltou à arena | "VOLTEI COM O CHOCOLATE!" |
| `flavio_vitoria` | Venceu a luta | "PODE MANDAR O PIX PRO QUEIROZ!" |
| `flavio_dano` | Levou golpe forte (às vezes) | gemido curto |

### Augusto Cury (`cury_`)

| Arquivo | Quando toca | Fala sugerida |
| --- | --- | --- |
| `cury_drone` | Habilidade K | "DRONE COM SIRENE!" |
| `cury_ministerio_ia` | Habilidade L | "MINISTÉRIO DA IA!" |
| `cury_palestra` | Habilidade I | "RESPIRA... E ESCUTA." (calmo, de palestrante) |
| `cury_nocaute_1` | Perdeu uma vida | "FALTOU GESTÃO DA EMOÇÃO!" |
| `cury_nocaute_2` | Perdeu uma vida | "CADÊ MEU DRONE?" |
| `cury_nocaute_3` | Perdeu uma vida | "ISSO VAI VIRAR LIVRO!" |
| `cury_nocaute_4` | Perdeu uma vida | "RESPIRA... RESPIRA..." |
| `cury_volta_1` | Voltou à arena | "VOLTEI MAIS SERENO!" |
| `cury_volta_2` | Voltou à arena | "O DRONE CHEGOU ANTES!" |
| `cury_volta_3` | Voltou à arena | "CAPÍTULO 2!" |
| `cury_vitoria` | Venceu a luta | "NÃO TEM DRONEFOBIA QUE ME PARE!" |
| `cury_dano` | Levou golpe forte (às vezes) | gemido curto |

## Atalhos

- **Pouco tempo para gravar?** Grave só os 3 gritos de habilidade e a vitória de cada um (12 arquivos). É o que mais aparece.
- **Uma fala genérica para o nocaute:** se `lula_nocaute_2` não existir, o jogo tenta `lula_nocaute`. O mesmo vale para `volta`. Assim, um único `lula_nocaute.wav` cobre todos os nocautes (mas aí o áudio pode não bater com o texto na tela).
- **Conferir:** dê Play, use a habilidade e escute. Se não tocar, confira o nome do arquivo (minúsculas, sem acento, com `_`).
