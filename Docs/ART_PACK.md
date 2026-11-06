# Pacote de arte — TSE Fatal

Todos os PNGs ficam em `Assets/Resources/Art`. A importação é automática (`PixelArtImportPostprocessor`): sprite sem compressão, com mipmaps e leitura habilitada para o recorte das sprite sheets. Fundo verde (ou magenta, quando o desenho tem verde) é removido pelo jogo ao carregar os frames.

| Caminho | Uso |
| --- | --- |
| `Characters/lula_idle.png` | Imagem estática, retrato e animação procedural de Lula. |
| `Characters/renan_idle.png` | Idem para Renan Santos. |
| `Characters/flavio_sigma_idle.png` | Idem para Flávio Bolsonaro. |
| `Characters/cury_idle.png` | Idem para Augusto Cury. |
| `Characters/{lula,renan,flavio,cury}/{estado}/01.png...` | Frames de animação, um arquivo por frame (prompts em `PromptsAnimacao/{personagem}/`). |
| `Stages/planalto.png` | Fundo da arena (Palácio do Planalto). |
| `VFX/{efeito}/01.png...` | Frames opcionais dos efeitos das habilidades (prompts em `PromptsAnimacao/vfx/`); sem eles, o jogo desenha versões procedurais. |
| `Menu/menu_debate_arena.png` | Fundo da seleção de personagem. |
| `Menu/menu_debate_arena_personagens.png` | Fundo do menu principal, com os candidatos nos púlpitos. |
| `Menu/select_{lula,renan,flavio,cury}_podium.png` | Candidato atrás do púlpito na seleção. |

Os efeitos de status são fantásticos e cômicos; eles não representam alegações sobre pessoas reais.
