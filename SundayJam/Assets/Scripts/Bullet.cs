using UnityEngine;

public class Bullet : MonoBehaviour
{
    public PlayerController Owner;
    public AudioClip hitSound;
    void OnCollisionEnter(Collision collision)
    {
        var hit = collision.gameObject;
        var health = hit.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(10, Owner);
        }

        AudioSource.PlayClipAtPoint(hitSound,this.transform.position);

        Destroy(gameObject);
    }
}
