using System;
using UnityEditor.Experimental.UIElements;
using UnityEditor.Graphing;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using Object = UnityEngine.Object;

namespace UnityEditor.ShaderGraph.Drawing.Slots
{
    public class TextureArraySlotControlView : VisualElement
    {
        Texture2DArrayInputMaterialSlot m_Slot;

        public TextureArraySlotControlView(Texture2DArrayInputMaterialSlot slot)
        {
            m_Slot = slot;
            AddStyleSheetPath("Styles/Controls/TextureArraySlotControlView");
            var objectField = new ObjectField { objectType = typeof(Texture2DArray), value = m_Slot.texture };
            objectField.OnValueChanged(OnValueChanged);
            Add(objectField);
        }

        void OnValueChanged(ChangeEvent<Object> evt)
        {
            var texture = evt.newValue as Texture2DArray;
            if (texture != m_Slot.texture)
            {
                m_Slot.owner.owner.owner.RegisterCompleteObjectUndo("Change Texture Array");
                m_Slot.texture = texture;
                m_Slot.owner.Dirty(ModificationScope.Node);
            }
        }
    }
}
