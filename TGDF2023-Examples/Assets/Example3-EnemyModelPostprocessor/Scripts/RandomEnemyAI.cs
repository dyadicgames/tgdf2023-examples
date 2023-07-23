using UnityEngine;

namespace Example3
{
    [RequireComponent(typeof(Enemy))]
    public class RandomEnemyAI : MonoBehaviour
    {
        private Enemy enemy;

        private void Awake()
        {
            enemy = GetComponent<Enemy>();
        }

        private void Update()
        {
            // do some enemy AI stuff
        }
    }
}
