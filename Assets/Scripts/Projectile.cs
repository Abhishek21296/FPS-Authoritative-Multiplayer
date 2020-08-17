using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public static Dictionary<int, Projectile> projectiles = new Dictionary<int, Projectile>();
    private static int nextProjectileId = 1;

    public int id;
    public Rigidbody rigidbody;
    public int thrownByPlayer;
    public Vector3 initialiForce;
    public float explosionRadius = 1.5f;
    public float explosionDamage = 75f;

    private void Start()
    {
        id = nextProjectileId;
        nextProjectileId++;
        projectiles.Add(id, this);

        ServerSend.SpawnProjectile(this, thrownByPlayer);

        rigidbody.AddForce(initialiForce);
        StartCoroutine(ExplodeAfterTime());
    }

    private void FixedUpdate()
    {
        ServerSend.ProjectilePosition(this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    public void Initialize(Vector3 _initialMovementDirection, float _initialForceStrength, int _thrownByPlayer)
    {
        initialiForce = _initialMovementDirection * _initialForceStrength;
        thrownByPlayer = _thrownByPlayer;
    }

    private void Explode()
    {
        ServerSend.ProjectileExploded(this);
        Collider[] _colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach(Collider c in _colliders)
        {
            if(c.CompareTag("Player"))
            {
                c.GetComponent<Player>().TakeDamage(explosionDamage);
            }
            else if(c.CompareTag("Enemy"))
            {
                c.GetComponent<Enemy>().TakeDamage(explosionDamage);
            }
        }
        projectiles.Remove(id);
        Destroy(gameObject);
    }

    private IEnumerator ExplodeAfterTime()
    {
        yield return new WaitForSeconds(7f);
        Explode();
    }
}
