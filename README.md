# TSE Fatal

Protótipo de luta 2D arcade em pixel art para Unity 6. O projeto abre no menu principal, gera a seleção de personagem e inicia uma luta contra bot.

## Abrir e testar

1. Abra esta pasta no Unity Hub com Unity `6000.6.8f1`.
2. Espere a Unity importar os pacotes `UGUI` e `2D Pixel Perfect` uma única vez.
3. Abra `Assets/Scenes/Main` e pressione Play.
4. No menu: `W/S` ou setas navegam; `Enter` ou `Espaço` confirma.
5. Na seleção: `A/D` ou setas escolhem; `Enter` confirma e exibe a transição `VS`; `Esc` retorna ao menu.
6. Na luta: `A/D` movem, `W` pula, `Shift` esquiva, `J` ataca, `K`/`L`/`I` usam as habilidades e `Esc` retorna ao menu.

Regras, personagens e habilidades: `Docs/GAME_DESIGN.md`.

## Interface arcade

- Menu principal em palco `480 × 270`, com escala inteira e bordas quando necessário.
- Fundo de arena em pixel art, holofotes oscilantes e luz pulsante.
- Logo com contorno, brilho pulsante e tremor de entrada.
- Menu com `JOGAR`, `CONTROLES`, `OPÇÕES` e `SAIR`, cursor e efeitos sonoros gerados por código.
- Seleção com retratos, atributos, poderes e rival oculto antes da confirmação.
