using UnityEngine;

namespace Assets.Scripts.Weapons
{
    public class Collectable : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            Debug.Log("Collision");
            var hit = other.gameObject;
            var player = hit.GetComponent<PlayerController>();
            if (player != null)
            {
                CollectedBy(player);
                Destroy(gameObject);
            }
        }

        public virtual void CollectedBy(PlayerController player)
        {
            
        }
    }
}
