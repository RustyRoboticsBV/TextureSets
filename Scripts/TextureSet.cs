using System;
using System.Linq;


#if UNITY_5_3_OR_NEWER
using UnityEngine;
#elif GODOT
using Godot;
#endif

#if GODOT
using Godot.Collections;
#else
using System.Collections.Generic;
#endif

namespace Rusty.Textures
{
    [Serializable]
#if UNITY_5_3_OR_NEWER
    [CreateAssetMenu(menuName = "Assets/Texture Set")]
    public sealed class TextureSet : ScriptableObject
#elif GODOT
    [GlobalClass]
    public sealed partial class TextureSet : Resource
#else
    public sealed class TextureSet
#endif
    {
        /* Public properties. */
#if UNITY_5_3_OR_NEWER
        [field: SerializeField] public List<NamedTexture> Textures { get; private set; } = new List<NamedTexture>();
#elif GODOT
        [Export] public Array<NamedTexture> Textures { get; private set; } = new Array<NamedTexture>();
#else
        public List<NamedTexture> Textures { get; private set; } = new List<NamedTexture>();
#endif

        /* Public properties. */
        public int Count => Textures.Count;

        /* Public indexers. */
        public Texture2D this[string name] => Get(name);
        public Texture2D this[int index] => Textures[index].Texture;

        /* Public methods. */
        public static TextureSet CreateNew()
        {
#if UNITY_5_3_OR_NEWER
            return CreateInstance<TextureSet>();
#else
            return new TextureSet();
#endif
        }

        /// <summary>
        /// Add a texture to the set.
        /// </summary>
        public void Add(string name, Texture2D texture)
        {
            Textures.Add(new NamedTexture(name, texture));
        }

        /// <summary>
        /// Remove a texture from the set.
        /// </summary>
        public void Remove(string name)
        {
            if (!TryRemove(name))
                throw new ArgumentOutOfRangeException(nameof(name));
        }

        /// <summary>
        /// Try to remove a texture from the set. Returns true on success, and false on failure.
        /// </summary>
        public bool TryRemove(string name)
        {
            int index = IndexOf(name);
            if (index == -1)
                return false;
            else
            {
                Textures.RemoveAt(index);
                return true;
            }
        }

        /// <summary>
        /// Clear the texture set.
        /// </summary>
        public void Clear()
        {
            Textures.Clear();
        }

        /// <summary>
        /// Get the index of a texture, using its name.
        /// </summary>
        public int IndexOf(string name)
        {
            for (int i = 0; i < Textures.Count; i++)
            {
                if (Textures[i].Name == name)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Check if a texture with some name is present in the set.
        /// </summary>
        public bool Has(string name) => IndexOf(name) >= 0;

        /// <summary>
        /// Get a texture, using its name.
        /// </summary>
        public Texture2D Get(string name)
        {
            if (!TryGet(name, out Texture2D texture))
                throw new ArgumentOutOfRangeException(nameof(name));
            return texture;
        }

        /// <summary>
        /// Try to get a texture, using its name. Returns true on success, and false on failure.
        /// </summary>
        public bool TryGet(string name, out Texture2D texture)
        {
            int index = IndexOf(name);
            if (index == -1)
            {
                texture = null;
                return false;
            }
            else
            {
                texture = Textures[index].Texture;
                return true;
            }
        }
    }
}