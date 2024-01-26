using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Example4
{
    [InitializeOnLoad]
    public class PlatformVariantsPostprocessor : AssetPostprocessor
    {
        private const string folderPath = "Assets/Example4-PlatformVariantsPostprocessor/Models/";
        private const string customDependencyName = "PlatformVariantsPostprocessor";

        public int callbackOrder => 0;

        static PlatformVariantsPostprocessor()
        {
            // With the "InitializeOnLoad" attribute this static constructor will be called
            // on editor start as well as when switching the active build target. This is all
            // we need to keep our custom dependency up to date with the built target selection.

            var hash = Hash128.Compute((int) EditorUserBuildSettings.activeBuildTarget);
            AssetDatabase.RegisterCustomDependency(customDependencyName, hash);

            Debug.Log($"Registered custom dependency '{customDependencyName}' (hash: {hash}).");
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

            // We register our custom dependency for this asset here. Changing the hash
            // of the custom dependency in the above constructor based on the active build
            // target will trigger a re-import every time the build target changes.
            context.DependsOnCustomDependency(customDependencyName);

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
