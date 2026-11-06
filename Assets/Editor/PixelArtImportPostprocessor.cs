using UnityEditor;
using UnityEngine;

namespace TSEFatal.EditorTools
{
    public class PixelArtImportPostprocessor : AssetPostprocessor
    {
        private void OnPreprocessTexture()
        {
            if (!assetPath.StartsWith("Assets/Resources/Art/")) return;

            var importer = (TextureImporter)assetImporter;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.alphaIsTransparency = true;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.maxTextureSize = 4096;
            // As artes geradas têm ~1300 px e aparecem bem menores na tela.
            // Mipmaps + trilinear evitam o serrilhado/cintilação ao reduzir.
            importer.mipmapEnabled = true;
            importer.filterMode = FilterMode.Trilinear;
            // Leitura em CPU: necessária para recortar quadros de animação e achar o pé do personagem.
            importer.isReadable = true;

            // Frames soltos (Characters/{personagem}/{estado}/01.png): o jogo recorta e copia cada um
            // para uma textura própria, então o original só precisa ser lido uma vez e pode ser menor.
            // O personagem aparece com ~250 px de altura em 1080p; 512 px de quadro sobram.
            if (IsFrameFile(assetPath))
            {
                importer.maxTextureSize = 512;
                importer.mipmapEnabled = false;
            }
        }

        private static bool IsFrameFile(string path)
        {
            const string characters = "Assets/Resources/Art/Characters/";
            const string effects = "Assets/Resources/Art/VFX/";
            if (path.StartsWith(characters)) return path.Substring(characters.Length).Split('/').Length == 3; // personagem/estado/arquivo
            if (path.StartsWith(effects)) return path.Substring(effects.Length).Split('/').Length == 2;        // efeito/arquivo
            return false;
        }
    }
}
