# Como gerar as pixel arts com o ChatGPT

Cada arquivo desta pasta é um pedido completo: regras, lista de frames, nome de cada arquivo e o que já está pronto. Você só anexa o arquivo e manda o ChatGPT seguir.

## Passo a passo

1. Abra **um chat novo** no ChatGPT para cada arquivo.
2. Anexe o arquivo `.md` e a imagem indicada na coluna "Anexar junto". Se o efeito ou a animação já tiver frames prontos, anexe também o último pronto, para o ChatGPT manter o mesmo tamanho e estilo.
3. Envie esta mensagem:

   ```
   Siga o briefing do arquivo anexado. Comece pelo primeiro item que não está marcado "já existe, pule".
   ```

4. A cada imagem, confira e responda **continue** (próxima) ou **refaça** (dizendo o que corrigir).
5. Salve cada imagem no caminho indicado, ou peça **zip** e extraia o arquivo na pasta indicada no briefing.
6. Volte ao Unity e dê Play: o jogo carrega os frames sozinho.

## Arquivos

| Arquivo | O quê | Faltam | Anexar junto |
| --- | --- | --- | --- |
| `personagem_moraes.md` | Alexandre de Moraes (personagem) | 47 de 47 | `lula_idle.png`, `renan_idle.png` e uma foto do Alexandre de Moraes (a imagem base é o passo 0) |
| `efeitos.md` | Efeitos das habilidades | 19 de 85 | opcional: a imagem base de um personagem, para combinar o estilo |

Já completos (sem arquivo aqui): Renan Santos, Lula, Flávio Bolsonaro, Augusto Cury.

O campo "Faltam" e as marcas "já existe, pule" foram calculados olhando as imagens que já estão no projeto. Depois de salvar novas imagens, peça para o Claude regerar esta pasta, que a lista se atualiza.

## Se der problema

- **Estilo ou tamanho começou a mudar:** abra um chat novo, anexe o mesmo arquivo e o último frame bom, e diga "continue a partir do item N".
- **Veio virado para a esquerda:** peça para espelhar.
- **Fundo xadrez "falso":** peça fundo chapado na cor do briefing.
