using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.AssetImporters;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Drawing;
using UnityEngine;

[CustomEditor(typeof(ShaderGraphImporter))]
public class ShaderGraphImporterEditor : ScriptedImporterEditor
{
    static GenerationMode s_DebugGenerationMode = GenerationMode.ForReals;

    public override void OnInspectorGUI()
    {
        AssetImporter importer = target as AssetImporter;

        if (GUILayout.Button("Open Shader Editor"))
        {
            Debug.Assert(importer != null, "importer != null");
            ShowGraphEditWindow(importer.assetPath);
        }

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        s_DebugGenerationMode = (GenerationMode)EditorGUILayout.EnumPopup("Generated code", s_DebugGenerationMode);
        if (GUILayout.Button("View", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
            ShowGeneratedShader(importer.assetPath, s_DebugGenerationMode);
        EditorGUILayout.EndHorizontal();
    }

    internal static bool ShowGraphEditWindow(string path)
    {
        var guid = AssetDatabase.AssetPathToGUID(path);
        var extension = Path.GetExtension(path);
        if (extension != ".ShaderGraph" && extension != ".LayeredShaderGraph" && extension != ".ShaderSubGraph" && extension != ".ShaderRemapGraph")
            return false;

        var foundWindow = false;
        foreach (var w in Resources.FindObjectsOfTypeAll<MaterialGraphEditWindow>())
        {
            if (w.selectedGuid == guid)
            {
                foundWindow = true;
                w.Focus();
            }
        }

        if (!foundWindow)
        {
            var window = CreateInstance<MaterialGraphEditWindow>();
            window.Show();
            window.Initialize(guid);
        }
        return true;
    }

    internal static void ShowGeneratedShader(string path, GenerationMode generationMode)
    {
        var name = Path.GetFileNameWithoutExtension(path);

        var json = File.ReadAllText(path, System.Text.Encoding.UTF8);
        var graph = JsonUtility.FromJson<MaterialGraph>(json);
        graph.LoadedFromDisk();

        List<PropertyCollector.TextureInfo> textures;
        var shaderName = string.Format("graphs/{0}", name);
        var shaderString = graph.GetShader(shaderName, generationMode, out textures);

        var tempPath = string.Format("Temp/GeneratedShader-{0}-{1}.shader", name, generationMode);
        File.WriteAllText(tempPath, shaderString);

        UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(tempPath, 0);
    }

    [OnOpenAsset(0)]
    public static bool OnOpenAsset(int instanceID, int line)
    {
        var path = AssetDatabase.GetAssetPath(instanceID);
        return ShowGraphEditWindow(path);
    }
}
