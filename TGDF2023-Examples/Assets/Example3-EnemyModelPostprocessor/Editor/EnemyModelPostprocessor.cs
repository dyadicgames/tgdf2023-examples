using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Example3
{
    public class EnemyModelPostprocessor : AssetPostprocessor
    {
        private void OnPostprocessModel(GameObject gameObject)
        {
            // We only want to post-process specific models. In this example, we ignore all models
            // that are not inside a specific folder.
            if (!assetPath.StartsWith("Assets/Example3-EnemyModelPostprocessor/Models/"))
            {
                return;
            }

            // Skip post-processing of models labeled "Ignore".
            if (AssetDatabase.GetLabels(assetImporter).Contains("Ignore"))
            {
                return;
            }

            gameObject.tag = "Enemy";
            gameObject.layer = LayerMask.NameToLayer("Enemy");

            // Adding main components.
            gameObject.AddComponent<Enemy>();
            gameObject.AddComponent<RandomEnemyAI>();
            gameObject.AddComponent<SpawnItemAfterDefeat>();

            // Setting up character controller.
            var characterController = gameObject.AddComponent<CharacterController>();
            var renderer = gameObject.GetComponentInChildren<Renderer>();
            characterController.height = renderer.bounds.extents.y * 2.0f;
            characterController.radius = Mathf.Min(renderer.bounds.extents.x, renderer.bounds.extents.z);
            characterController.center = Vector3.up * renderer.bounds.extents.y;
            characterController.skinWidth = characterController.radius * 0.1f;
        }
    }
}
