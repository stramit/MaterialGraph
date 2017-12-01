using System.Linq;
using UnityEngine;
using UnityEditor.Graphing;

namespace UnityEditor.ShaderGraph
{
    [Title("Artistic/Normal/Normal Create")]
    public class NormalCreateNode : AbstractMaterialNode, IGeneratesBodyCode, IGeneratesFunction, IMayRequireMeshUV
    {
        public const int TextureInputId = 0;
        public const int UVInputId = 1;
        public const int OffsetInputId = 2;
        public const int StrengthInputId = 3;
        public const int OutputSlotId = 4;

        const string kTextureInputName = "Texture";
        const string kUVInputName = "UV";
        const string kOffsetInputName = "Offset";
        const string kStrengthInputName = "Strength";
        const string kOutputSlotName = "Out";

        public NormalCreateNode()
        {
            name = "Normal Create";
            UpdateNodeAfterDeserialization();
        }

        string GetFunctionName()
        {
            return string.Format("Unity_NormalCreate_{0}", precision);
        }

        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new Texture2DInputMaterialSlot(TextureInputId, kTextureInputName, kTextureInputName));
            AddSlot(new UVMaterialSlot(UVInputId, kUVInputName, kUVInputName, UVChannel.uv0));
            AddSlot(new Vector1MaterialSlot(OffsetInputId, kOffsetInputName, kOffsetInputName, SlotType.Input, 0.005f));
            AddSlot(new Vector1MaterialSlot(StrengthInputId, kStrengthInputName, kStrengthInputName, SlotType.Input, 8f));
            AddSlot(new Vector3MaterialSlot(OutputSlotId, kOutputSlotName, kOutputSlotName, SlotType.Output, Vector3.zero));
        }

        string GetFunctionPrototype(string textureIn, string uvIn, string offsetIn, string strengthIn, string argOut)
        {
            return string.Format("void {0} ({1} {2}, {3} {4}, {5} {6}, {7} {8}, out {9} {10})", GetFunctionName(),
                ConvertConcreteSlotValueTypeToString(precision, FindInputSlot<MaterialSlot>(TextureInputId).concreteValueType), textureIn,
                ConvertConcreteSlotValueTypeToString(precision, FindInputSlot<MaterialSlot>(UVInputId).concreteValueType), uvIn,
                ConvertConcreteSlotValueTypeToString(precision, FindInputSlot<MaterialSlot>(OffsetInputId).concreteValueType), offsetIn,
                ConvertConcreteSlotValueTypeToString(precision, FindInputSlot<MaterialSlot>(StrengthInputId).concreteValueType), strengthIn,
                ConvertConcreteSlotValueTypeToString(precision, FindOutputSlot<MaterialSlot>(OutputSlotId).concreteValueType), argOut);
        }

        public void GenerateNodeCode(ShaderGenerator visitor, GenerationMode generationMode)
        {
            string textureValue = GetSlotValue(TextureInputId, generationMode);
            string uvValue = GetSlotValue(UVInputId, generationMode);
            string offsetValue = GetSlotValue(OffsetInputId, generationMode);
            string strengthValue = GetSlotValue(StrengthInputId, generationMode);
            string outputValue = GetSlotValue(OutputSlotId, generationMode);
            visitor.AddShaderChunk(string.Format("{0} {1};", ConvertConcreteSlotValueTypeToString(precision, FindOutputSlot<MaterialSlot>(OutputSlotId).concreteValueType), GetVariableNameForSlot(OutputSlotId)), true);
            visitor.AddShaderChunk(GetFunctionCallBody(textureValue, uvValue, offsetValue, strengthValue, outputValue), true);
        }

        string GetFunctionCallBody(string textureValue, string uvValue, string offsetValue, string strengthValue, string outputValue)
        {
            return GetFunctionName() + " (" + textureValue + ", " + uvValue + ", " + offsetValue + ", " + strengthValue + ", " + outputValue + ");";
        }

        public void GenerateNodeFunction(ShaderGenerator visitor, GenerationMode generationMode)
        {
            visitor.AddShaderChunk(GetFunctionPrototype("Texture", "UV", "Offset", "Strength", "Out"), false);
            visitor.AddShaderChunk("{", false);
            visitor.Indent();

            visitor.AddShaderChunk("Offset = pow(Offset, 3) * 0.1;", true);
            visitor.AddShaderChunk(string.Format("{0}2 offsetU = float2(UV.x + Offset, UV.y);", precision), true);
            visitor.AddShaderChunk(string.Format("{0}2 offsetV = float2(UV.x, UV.y + Offset);", precision), true);

            visitor.AddShaderChunk(string.Format("{0} normalSample = UNITY_SAMPLE_TEX2D(Texture, UV);", precision), true);
            visitor.AddShaderChunk(string.Format("{0} uSample = UNITY_SAMPLE_TEX2D(Texture, offsetU);", precision), true);
            visitor.AddShaderChunk(string.Format("{0} vSample = UNITY_SAMPLE_TEX2D(Texture, offsetV);", precision), true);

            visitor.AddShaderChunk(string.Format("{0}3 va = float3(1, 0, (uSample - normalSample) * Strength);", precision), true);
            visitor.AddShaderChunk(string.Format("{0}3 vb = float3(0, 1, (vSample - normalSample) * Strength);", precision), true);
            visitor.AddShaderChunk("Out = cross(va, vb);", true);

            visitor.Deindent();
            visitor.AddShaderChunk("}", false);
        }

        public override bool hasPreview
        {
            get { return true; }
        }

        public bool RequiresMeshUV(UVChannel channel)
        {
            foreach (var slot in GetInputSlots<MaterialSlot>().OfType<IMayRequireMeshUV>())
            {
                if (slot.RequiresMeshUV(channel))
                    return true;
            }
            return false;
        }
    }
}
