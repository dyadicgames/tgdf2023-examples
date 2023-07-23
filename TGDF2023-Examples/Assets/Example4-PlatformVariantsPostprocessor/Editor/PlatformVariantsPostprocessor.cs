using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Example4
{
    public class PlatformVariantsPostprocessor : AssetPostprocessor, IActiveBuildTargetChanged
    {
        private const string folderPath = "Assets/Example4-PlatformVariantsPostprocessor/Models/";

        public int callbackOrder => 0;

        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            var assets = AssetDatabase.FindAssets($"t:model", new[] { folderPath });
            foreach (var guid in assets)
            {
                AssetDatabase.ImportAsset(AssetDatabase.GUIDToAssetPath(guid));
            }
        }

        private void OnPostprocessModel(GameObject gameObject)
        {
            if (!assetPath.StartsWith(folderPath))
            {
                return;
            }

            // Skip post-processing of models labeled "Ignore".
            if (AssetDatabase.GetLabels(assetImporter).Contains("Ignore"))
            {
                return;
            }

            var removeList = new List<GameObject>();
            var suffix = string.Empty;

            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.Android:
                case BuildTarget.iOS:
                    //Mobile
                    suffix = "-Mobile";
                    break;
                case BuildTarget.Switch:
                case BuildTarget.PS4:
                case BuildTarget.PS5:
                case BuildTarget.GameCoreXboxSeries:
                case BuildTarget.GameCoreXboxOne:
                    //Console
                    suffix = "-Console";
                    break;
                case BuildTarget.StandaloneLinux64:
                case BuildTarget.StandaloneOSX:
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    //Standalone
                    suffix = "-Standalone";
                    break;
            }

            foreach (Transform child in gameObject.transform)
            {
                removeList.Add(child.gameObject);
                if (!child.gameObject.name.EndsWith(suffix))
                {
                    continue;
                }

                var meshFilter = child.gameObject.GetComponent<MeshFilter>();
                var meshRenderer = child.gameObject.GetComponent<MeshRenderer>();

                var newMeshFilter = gameObject.AddComponent<MeshFilter>();
                var newMeshRenderer = gameObject.AddComponent<MeshRenderer>();

                EditorUtility.CopySerialized(meshFilter, newMeshFilter);
                EditorUtility.CopySerialized(meshRenderer, newMeshRenderer);
            }

            foreach (var obj in removeList)
            {
                var meshFilter = obj.GetComponent<MeshFilter>();

                if (obj.name.EndsWith(suffix))
                {
                    meshFilter.sharedMesh.name = meshFilter.sharedMesh.name.Replace(suffix, "");
                }
                else
                {
                    Object.DestroyImmediate(meshFilter.sharedMesh);
                }

                Object.DestroyImmediate(obj);
            }
        }
    }
}
