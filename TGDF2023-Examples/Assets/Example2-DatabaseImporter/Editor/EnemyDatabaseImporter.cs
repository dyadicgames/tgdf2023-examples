using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Example2
{
    [ScriptedImporter(1, "enemies.csv")]
    public class EnemyDatabaseImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var importedCsv = File.ReadAllText(FileUtil.GetPhysicalPath(ctx.assetPath));

            var textAsset = new TextAsset(importedCsv);
            ctx.AddObjectToAsset($"RawData", textAsset);
            ctx.SetMainObject(textAsset);

            var processedData = ProcessRawData(importedCsv);
            foreach(var data in processedData)
            {
                data.name = $"EnemyData #{data.id}";
                ctx.AddObjectToAsset($"EnemyData_{data.id}", data);
            }
        }

        private EnemyData[] ProcessRawData(string csvString)
        {
            var fieldIdx = new int[] { -1, -1, -1, -1 };
            var rawData = csvString.Split('\n').Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();
            var regex = new Regex("(?<=^|,)(\"(?:[^\"]|\"\")*\"|[^,]*)");
            var enemyData = new EnemyData[rawData.Length - 1];

            if (rawData.Length > 0)
            {
                for (var i = 0; i < rawData.Length; i++)
                {
                    var dataLine = rawData[i].Trim(new char[] { '\n', '\r', ' ' });
                    var data = regex.Matches(dataLine).ToArray();

                    if (i == 0)
                    {
                        fieldIdx[0] = ArrayUtility.FindIndex(data, match => match.Value == "ID");
                        fieldIdx[1] = ArrayUtility.FindIndex(data, match => match.Value == "NAME");
                        fieldIdx[2] = ArrayUtility.FindIndex(data, match => match.Value == "HEALTH");
                        fieldIdx[3] = ArrayUtility.FindIndex(data, match => match.Value == "STRENGTH");
                        continue;
                    }

                    if (data.Length == 4)
                    {
                        EnemyData newEnemyData = ScriptableObject.CreateInstance<EnemyData>();
                        if (fieldIdx[0] >= 0)
                        {
                            int.TryParse(data[fieldIdx[0]].Value, out newEnemyData.id);
                        }
                        if (fieldIdx[1] >= 0)
                        {
                            newEnemyData.displayName = data[fieldIdx[1]].Value;
                        }
                        if (fieldIdx[2] >= 0)
                        {
                            int.TryParse(data[fieldIdx[2]].Value, out newEnemyData.health);
                        }
                        if (fieldIdx[3] >= 0)
                        {
                            int.TryParse(data[fieldIdx[3]].Value, out newEnemyData.strength);
                        }
                        enemyData[i - 1] = newEnemyData;
                    }
                }
            }

            Debug.Log($"Imported {rawData.Length - 1} enemy data");

            return enemyData;
        }
    }
}
