# Sistema de menus — TSE Fatal

## Arquitetura

| Arquivo | Responsabilidade |
| --- | --- |
| `MainMenuController.cs` | Canvas arcade, fundo animado, logo, opções e navegação do menu principal. |
| `CharacterSelectController.cs` | Grade de candidatos, cartões, atributos, poderes, rival oculto e transição VS. |
| `MenuAudio.cs` | Blips, confirmação e música placeholder sintetizada em código. |
| `GameFlow.cs` | Liga menu, seleção, combate, retorno ao menu e câmera. |

## Ajustes técnicos

- O menu principal usa um palco ancorado de `480 × 270` com escala inteira: `4×` em `1920 × 1080` e `2×` em `1280 × 720` ou `1366 × 768`.
- A fonte Press Start 2P é rasterizada com filtro `Point`; os PNGs do menu também usam `Point`, sem mipmaps ou compressão.
- O importador `PixelArtImportPostprocessor.cs` aplica `FilterMode.Point`, sem mipmaps e sem compressão aos PNGs de arte.
- `GameFlow` configura `PixelPerfectCamera` na câmera principal com referência `480 × 270`.

## Trocar sons depois

Substitua as chamadas a `MakeTone` e `MakeArcadeLoop` em `MenuAudio.cs` por clipes de áudio importados. O restante do menu continuará usando `Blip()` e `Confirm()` normalmente.
