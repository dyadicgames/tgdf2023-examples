using UnityEngine;

namespace Example2
{
    [CreateAssetMenu(menuName = "Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        public int id;
        public string displayName;
        public int health;
        public int strength;
    }
}
