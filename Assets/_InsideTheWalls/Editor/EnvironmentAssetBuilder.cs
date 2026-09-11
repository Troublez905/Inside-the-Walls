using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace InsideTheWalls.Editor
{
    public static class EnvironmentAssetBuilder
    {
        private const string Input = "Assets/_InsideTheWalls/Art/Environment/NewResources01/";
        private const string Output = "Assets/_InsideTheWalls/Resources/Environment/NewResources01/";
        private static readonly string[] Names = { "Bench", "WallLight", "OutdoorTileA", "OutdoorTileB", "SecureDoor", "YardFence", "WallPanel" };

        public static void EnsureAssets()
        {
            Directory.CreateDirectory(Output);
            AssetDatabase.Refresh();
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) throw new InvalidOperationException("Environment assets require URP Lit.");
            foreach (string name in Names)
            {
                string path = Input + name + ".fbx";
                var importer = AssetImporter.GetAtPath(path) as ModelImporter;
                if (importer == null) throw new FileNotFoundException("Run import_new_resources.py in Blender first: " + path);
                if (importer.userData != "ITW.Environment.v1")
                {
                    importer.importAnimation = false;
                    importer.importCameras = false;
                    importer.importLights = false;
                    importer.addCollider = false;
                    importer.materialImportMode = ModelImporterMaterialImportMode.None;
                    importer.isReadable = false;
                    importer.userData = "ITW.Environment.v1";
                    importer.SaveAndReimport();
                }
                string texturePath = Input + name + "_BaseColor.png";
                var textureImporter = AssetImporter.GetAtPath(texturePath) as TextureImporter;
                if (textureImporter == null) throw new FileNotFoundException(texturePath);
                if (textureImporter.userData != "ITW.Environment.Texture.v1")
                {
                    textureImporter.sRGBTexture = true;
                    textureImporter.maxTextureSize = 1024;
                    textureImporter.mipmapEnabled = true;
                    textureImporter.textureCompression = TextureImporterCompression.Compressed;
                    textureImporter.userData = "ITW.Environment.Texture.v1";
                    textureImporter.SaveAndReimport();
                }
                string materialPath = Output + name + ".mat";
                Material material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
                if (material == null)
                {
                    material = new Material(shader) { name = name + " Runtime" };
                    AssetDatabase.CreateAsset(material, materialPath);
                }
                material.shader = shader;
                material.SetTexture("_BaseMap", AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath));
                material.SetColor("_BaseColor", Color.white);
                material.SetFloat("_Smoothness", name == "WallLight" ? 0.35f : 0.16f);
                EditorUtility.SetDirty(material);
                GameObject source = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (source == null) throw new FileNotFoundException(path);
                var instance = (GameObject)PrefabUtility.InstantiatePrefab(source);
                try
                {
                    instance.name = name;
                    foreach (Renderer renderer in instance.GetComponentsInChildren<Renderer>(true))
                        renderer.sharedMaterials = Enumerable.Repeat(material, renderer.sharedMaterials.Length).ToArray();
                    if (instance.GetComponentsInChildren<Collider>(true).Length != 0)
                        throw new InvalidOperationException("Environment decoration must not add colliders: " + name);
                    if (PrefabUtility.SaveAsPrefabAsset(instance, Output + name + ".prefab") == null)
                        throw new IOException("Could not save " + name);
                }
                finally { UnityEngine.Object.DestroyImmediate(instance); }
            }
            AssetDatabase.SaveAssets();
            Debug.Log("ENVIRONMENT_ASSETS_OK seven optimized textured decoration prefabs");
        }
    }
}
