using UnityEngine;

namespace Example3
{
    [RequireComponent(typeof(Enemy))]
    public class SpawnItemAfterDefeat : MonoBehaviour
    {
        private void Awake()
        {
            var enemy = GetComponent<Enemy>();

            // listen for enemy defeat event
        }
    }
}
