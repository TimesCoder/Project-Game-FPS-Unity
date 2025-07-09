using UnityEngine;

namespace scgFullBodyController
{
    public class registerHit : MonoBehaviour
    {
        public GameObject impactParticle;
        public GameObject impactBloodParticle;
        public float impactDespawnTime;
        [HideInInspector] public int damage;

        void OnCollisionEnter(Collision col)
        {
            // Cari HealthController (baik player atau zombie)
            HealthController hc = col.transform.root.GetComponent<HealthController>();

            if (hc != null)
            {
                hc.TakeDamage(damage);

                // Efek darah jika target ada impact blood
                if (impactBloodParticle != null)
                {
                    GameObject tempImpact = Instantiate(impactBloodParticle, transform.position, transform.rotation);
                    tempImpact.transform.Rotate(Vector3.left * 90);
                    Destroy(tempImpact, impactDespawnTime);
                }
            }
            else
            {
                // Efek hit biasa
                if (impactParticle != null)
                {
                    GameObject tempImpact = Instantiate(impactParticle, transform.position, transform.rotation);
                    tempImpact.transform.Rotate(Vector3.left * 90);
                    Destroy(tempImpact, impactDespawnTime);
                }
            }

            Destroy(gameObject); // Hancurkan peluru
        }
    }
}
