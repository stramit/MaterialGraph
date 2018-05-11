using System;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    public class SerializableTextureArray
    {
        [SerializeField]
        string m_SerializedTexture;

        [SerializeField]
        string m_Guid;

        [NonSerialized]
        Texture2DArray m_Texture;

        [Serializable]
        class TextureHelper
        {
#pragma warning disable 649
            public Texture2DArray texture;
#pragma warning restore 649
        }

        public Texture2DArray texture
        {
            get
            {
                if (!string.IsNullOrEmpty(m_SerializedTexture))
                {
                    var textureHelper = new TextureHelper();
                    EditorJsonUtility.FromJsonOverwrite(m_SerializedTexture, textureHelper);
                    m_SerializedTexture = null;
                    m_Guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(textureHelper.texture));
                    m_Texture = textureHelper.texture;
                }
                else if (!string.IsNullOrEmpty(m_Guid) && m_Texture == null)
                {
                    m_Texture = AssetDatabase.LoadAssetAtPath<Texture2DArray>(AssetDatabase.GUIDToAssetPath(m_Guid));
                }

                return m_Texture;
            }
            set
            {
                m_SerializedTexture = null;
                m_Guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(value));
                m_Texture = value;
            }
        }
    }
}
