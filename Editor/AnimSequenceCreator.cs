using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System.Linq;

#if UNITY_EDITOR
public class AnimSequenceCreator : Editor
{
    [MenuItem("Assets/Create Sprite List", true)]
    private static bool ValidateCreateSpriteList()
    {
        // Validate if the selected assets are sprites
        Object[] selectedAssets = Selection.GetFiltered(typeof(Texture2D), SelectionMode.Assets);
        return selectedAssets.Length > 0;
    }

    [MenuItem("Assets/Create Sprite List")]
    private static void CreateSpriteList()
    {
        // Ensure that at least one asset is selected
        if (Selection.objects.Length == 0)
        {
            Debug.LogWarning("No assets selected.");
            return;
        }

        // Get the selected assets
        Object[] selectedAssets = Selection.GetFiltered(typeof(Texture2D), SelectionMode.Assets);

        if (selectedAssets.Length == 0)
        {
            Debug.LogWarning("No sprites selected.");
            return;
        }

        // Create a new SpriteList instance
        ImageAnimSequence spriteList = ScriptableObject.CreateInstance<ImageAnimSequence>();

        // Convert selected assets to a List<Sprite>
        List<Sprite> sprites = selectedAssets.
          Select(sprite => AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GetAssetPath(sprite))).
          Select((sprite, index) => new { sprite, filename = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(selectedAssets[index])) }).
          OrderBy(item => item.filename).
          Select(item => item.sprite).
          ToList();



        spriteList.AnimationSequence = sprites;

        string defaultName = "NewImageAnimation";
        string assetName = EditorUtility.SaveFilePanelInProject("Save Image Animation Sequence", defaultName, "asset", "Enter a name for the animation sequence");

        // Determine the path for the new asset
        string path = AssetDatabase.GetAssetPath(selectedAssets[0]);
        string folderPath = Path.GetDirectoryName(path);
        Debug.Log(assetName);
        // Create the asset and save it
        string assetPath = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/{assetName}");
        AssetDatabase.CreateAsset(spriteList, assetName);
        AssetDatabase.SaveAssets();

        // Refresh the asset database to show the new asset
        AssetDatabase.Refresh();

        // Focus on the newly created asset
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = spriteList;
    }

    private static Sprite TextureToSprite(Texture2D texture)
    {
        if (texture == null)
        {
            Debug.LogError("Texture2D is null.");
            return null;
        }

        // Create a sprite from the texture
        Rect rect = new Rect(0, 0, texture.width, texture.height);
        Vector2 pivot = new Vector2(0.5f, 0.5f); // Center pivot
        return Sprite.Create(texture, rect, pivot);
    }
}
#endif
