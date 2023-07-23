using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Demo1
{
    public class CubeModelPostprocessor : AssetPostprocessor
    {
        private void OnPostprocessModel(GameObject gameObject)
        {
            var assetGuid = AssetDatabase.GUIDFromAssetPath(assetImporter.assetPath);
            if (!AssetDatabase.GetLabels(assetGuid).Contains("Cube"))
            {
                return;
            }

            var nonCubes = gameObject.GetComponentsInChildren<MeshFilter>()
                .Where(meshFilter => meshFilter.name != "Cube")
                .ToList();

            foreach (var nonCube in nonCubes)
            {
                Object.DestroyImmediate(nonCube.sharedMesh);
                Object.DestroyImmediate(nonCube.gameObject);
            }
        }
    }
}
