using System;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#elif GODOT
using Godot;
#endif

namespace Rusty.Textures
{
    /// <summary>
    /// A texture with a name.
    /// </summary>
    [Serializable]
#if GODOT
    [GlobalClass]
    public sealed partial class NamedTexture : Resource
#else
    public sealed class NamedTexture
#endif
    {
        /* Fields. */
#if UNITY_5_3_OR_NEWER
        [SerializeField]
#elif GODOT
        [Export]
#endif
        private string name;

#if UNITY_5_3_OR_NEWER
        [SerializeField]
#elif GODOT
        [Export]
#endif
        private Texture2D texture;

        /* Public properties. */
        /// <summary>
        /// The name of the texture.
        /// </summary>
        public string Name
        {
            get => name;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                name = value;
            }
        }

        /// <summary>
        /// The texture.
        /// </summary>
        public Texture2D Texture
        {
            get => texture;
            set
            {
                texture = value;
#if GODOT
                ResourceName = texture.ResourceName;
#endif
            }
        }

        /* Conversion operators. */
        public static explicit operator Texture2D(NamedTexture texture) => texture?.Texture;

        /* Constructors. */
        public NamedTexture() : this("", null) { }

        public NamedTexture(string name, Texture2D texture)
        {
            Name = name;
            Texture = texture;
        }
    }
}