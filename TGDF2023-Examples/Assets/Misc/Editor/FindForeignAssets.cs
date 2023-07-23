using UnityEditor;
using UnityEngine;

public class AssetDatabaseExamples : MonoBehaviour
{
    [MenuItem("AssetDatabase/Find Foreign Assets")]
    static void FindForeignAssets()
    {
        //Find all foreign assets
        foreach (var guid in AssetDatabase.FindAssets("", new[] { "Assets" }))
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var asset = AssetDatabase.LoadMainAssetAtPath(path);
            var assetIsForeign = AssetDatabase.IsForeignAsset(asset);
            if (assetIsForeign)
                Debug.Log(path);
        }
    }
}