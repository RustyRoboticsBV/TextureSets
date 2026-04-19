#if UNITY_EDITOR
using UnityEditor.AssetImporters;

namespace Rusty.Textures.Editor
{
    [ScriptedImporter(1, "zip")]
    public class TextureSetImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext context)
        {
            TextureSet set = TextureSetLoader.Load(context.assetPath);

            for (int i = 0; i < set.Count; i++)
            {
                context.AddObjectToAsset(set[i].name, set[i]);
            }

            context.AddObjectToAsset("TextureSet", set);
            context.SetMainObject(set);
        }
    }
}
#endif