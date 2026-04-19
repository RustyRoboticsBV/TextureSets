#if UNITY_EDITOR
using UnityEditor;

namespace Rusty.Textures.Editor
{
    public class TextureSetPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
        {
            foreach (string path in importedAssets)
            {
                TextureSet set = AssetDatabase.LoadAssetAtPath<TextureSet>(path);
                if (set == null)
                    continue;

                if (set.Count > 0)
                {
                    EditorGUIUtility.SetIconForObject(set, set[0]);
                    EditorUtility.SetDirty(set);
                }
            }
        }
    }
}
#endif