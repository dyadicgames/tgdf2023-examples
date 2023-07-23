using System.IO;
using System;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Example1
{
    [ScriptedImporter(1, "star")]
    public class StarImporter : ScriptedImporter
    {
        public Color color = Color.yellow;

        public float depth = 1.0f;

        [Range(0.01f, 10.0f)]
        public float uniformScale = 1.0f;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            var importedStarJson = File.ReadAllText(FileUtil.GetPhysicalPath(ctx.assetPath));
            var importedStar = Star.FromJsonString(importedStarJson);

            var mesh = GenerateStarMesh(importedStar.innerRadius, importedStar.outerRadius, importedStar.corners);
            var material = new Material(Shader.Find("Standard"));
            var gameObject = new GameObject();
            var meshFilter = gameObject.AddComponent<MeshFilter>();
            var meshRenderer = gameObject.AddComponent<MeshRenderer>();

            mesh.name = Path.GetFileNameWithoutExtension(ctx.assetPath);
            material.name = mesh.name;
            material.color = color;
            gameObject.transform.localScale = Vector3.one * uniformScale;
            meshFilter.sharedMesh = mesh;
            meshRenderer.sharedMaterial = material;

            ctx.AddObjectToAsset("mesh", mesh);
            ctx.AddObjectToAsset("material", material);
            ctx.AddObjectToAsset("gameObject", gameObject);
            ctx.SetMainObject(gameObject);
        }

        private Mesh GenerateStarMesh(float innerRadius, float outerRadius, int corners)
        {
            var mesh = new Mesh();

            var innerR = innerRadius;
            var outerR = outerRadius;
            innerRadius = Mathf.Min(innerR, outerR);
            outerRadius = Mathf.Max(innerR, outerR);

            var vertexCount = corners * 2;
            var triangleCount = corners * 3 + (corners - 2) * 3;
            var angle = 360.0f / corners;

            var vertices = new Vector3[(vertexCount + 4 * corners) * 2];
            var triangles = new int[triangleCount * 2 + 4 * corners * 3];

            for (var i = 0; i < corners; i++)
            {
                var vertexStartIdx = i * 2;
                var triangleStartIdx = i * 3;

                // Add outer point first
                // Top vertices
                vertices[vertexStartIdx] = Quaternion.Euler(0.0f, i * angle, 0.0f) * Vector3.forward * outerRadius + Vector3.up * depth / 2; // First vertex to add
                vertices[vertexStartIdx + 1] = Quaternion.Euler(0.0f, i * angle + angle / 2, 0.0f) * Vector3.forward * innerRadius + Vector3.up * depth / 2; // Second vertex to add

                // Top triangles
                triangles[triangleStartIdx] = vertexStartIdx;
                triangles[triangleStartIdx + 1] = vertexStartIdx + 1;
                triangles[triangleStartIdx + 2] = (vertexStartIdx - 1 < 0 ? vertexCount : vertexStartIdx) - 1;

                // Bottom vertices
                vertices[vertexCount + vertexStartIdx] = Quaternion.Euler(0.0f, i * angle, 0.0f) * Vector3.forward * outerRadius + Vector3.up * depth / -2; // First vertex to add
                vertices[vertexCount + vertexStartIdx + 1] = Quaternion.Euler(0.0f, i * angle + angle / 2, 0.0f) * Vector3.forward * innerRadius + Vector3.up * depth / -2; // Second vertex to add

                // Bottom triangles
                triangles[triangleCount + triangleStartIdx] = vertexCount + vertexStartIdx;
                triangles[triangleCount + triangleStartIdx + 1] = vertexCount + (vertexStartIdx - 1 < 0 ? vertexCount : vertexStartIdx) - 1;
                triangles[triangleCount + triangleStartIdx + 2] = vertexCount + (vertexStartIdx + 1);
            }

            for (var i = 0; i < corners; i++)
            {
                var triangleStartIdx = triangleCount * 2 + i * 12;
                var vertexStartIdx = (vertexCount * 2) + i * 8;

                // Side face vertices
                var sideVertex0 = vertexCount + i * 2 + 1;
                var sideVertex1 = i * 2 + 1;
                var sideVertex2 = vertexCount + i * 2;
                var sideVertex3 = i * 2;
                var sideVertex4 = vertexCount + (i * 2 + 2) % vertexCount;
                var sideVertex5 = (i * 2 + 2) % vertexCount;

                vertices[vertexStartIdx] = vertices[vertexStartIdx + 6] = vertices[sideVertex0];
                vertices[vertexStartIdx + 1] = vertices[vertexStartIdx + 7] = vertices[sideVertex1];
                vertices[vertexStartIdx + 2] = vertices[sideVertex2];
                vertices[vertexStartIdx + 3] = vertices[sideVertex3];
                vertices[vertexStartIdx + 4] = vertices[sideVertex4];
                vertices[vertexStartIdx + 5] = vertices[sideVertex5];
                

                // Side Triangle 1 & 2
                triangles[triangleStartIdx] = vertexStartIdx;
                triangles[triangleStartIdx + 1] = triangles[triangleStartIdx + 3] = vertexStartIdx + 1;
                triangles[triangleStartIdx + 2] = triangles[triangleStartIdx + 5] = vertexStartIdx + 2;
                triangles[triangleStartIdx + 4] = vertexStartIdx + 3;

                // Side Triangle 3 & 4
                triangles[triangleStartIdx + 6] = vertexStartIdx + 4;
                triangles[triangleStartIdx + 7] = triangles[triangleStartIdx + 9] = vertexStartIdx + 5;
                triangles[triangleStartIdx + 8] = triangles[triangleStartIdx + 11] = vertexStartIdx + 6;
                triangles[triangleStartIdx + 10] = vertexStartIdx + 7;

            }

            // Triangularize inner shape
            for (var i = 0; i < corners - 2; i++)
            {
                var triangleStartIdx = corners + i;
                triangles[triangleStartIdx * 3] = 1;
                triangles[triangleStartIdx * 3 + 1] = 1 + 2 * (i + 1);
                triangles[triangleStartIdx * 3 + 2] = 1 + 2 * (i + 1) + 2;

                triangles[triangleCount + triangleStartIdx * 3] = vertexCount + 1;
                triangles[triangleCount + triangleStartIdx * 3 + 1] = vertexCount + 1 + 2 * (i + 1) + 2;
                triangles[triangleCount + triangleStartIdx * 3 + 2] = vertexCount + 1 + 2 * (i + 1);
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }
    }
}
