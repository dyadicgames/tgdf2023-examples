using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Example5
{
    public class PropModelPostprocessor : AssetPostprocessor
    {
        private void OnPostprocessModel(GameObject gameObject)
        {
            // We only want to post-process specific models. In this example, we ignore all models
            // that are not inside a specific folder.
            if (!assetPath.StartsWith("Assets/Example5-PropModelPostprocessor/Models/"))
            {
                return;
            }

            // Skip post-processing of models labeled "Ignore".
            if (AssetDatabase.GetLabels(assetImporter).Contains("Ignore"))
            {
                return;
            }

            var removeList = new List<GameObject>();
            foreach (Transform child in gameObject.transform)
            {
                var childGameObject = child.gameObject;
                if (childGameObject.name.EndsWith("-Collider"))
                {
                    var collider = gameObject.AddComponent<MeshCollider>();
                    var colliderMeshFilter = childGameObject.GetComponent<MeshFilter>();
                    if (colliderMeshFilter)
                    {
                        collider.sharedMesh = colliderMeshFilter.sharedMesh;
                    }

                    removeList.Add(childGameObject);
                    continue;
                }

                if (childGameObject.name.EndsWith("-Shadow_Proxy"))
                {
                    var baseMeshRenderer = gameObject.GetComponent<MeshRenderer>();
                    var shadowMeshRenderer = childGameObject.GetComponent<MeshRenderer>();
                    baseMeshRenderer.shadowCastingMode = ShadowCastingMode.Off;
                    shadowMeshRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                    shadowMeshRenderer.receiveShadows = false;
                    continue;
                }
            }

            foreach (var obj in removeList)
            {
                Object.DestroyImmediate(obj);
            }
        }
    }
}
