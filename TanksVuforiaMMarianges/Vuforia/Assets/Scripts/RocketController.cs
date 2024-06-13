using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketController : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;
    public float explosionRadius = 4f; 
    public LayerMask tankLayer; 

    void OnCollisionEnter(Collision collision)
    {
        // Instancia la explosión en la posición del rocket al momento de la colisión
        GameObject instance = Instantiate(explosionPrefab, transform.position, transform.rotation);

        // Crea una esfera superpuesta en la posición del proyectil y comprueba si el tanque rival está dentro de ella
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, tankLayer);
        foreach (var hitCollider in hitColliders)
        {
            if (IsValidTarget(hitCollider))
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                float damage = CalculateDamage(distance);
                Debug.Log(hitCollider.gameObject.name + " impacted! Dameage done: " + damage);
                hitCollider.gameObject.GetComponent<TankController>().TakeDamage(damage);
            }
        }

      
        Destroy(gameObject);
        Destroy(instance, 1.6f);
    }

    bool IsValidTarget(Collider collider)
    {
        return collider.gameObject.CompareTag("Player1") && gameObject.tag != "Player1" ||
               collider.gameObject.CompareTag("Player2") && gameObject.tag != "Player2";
    }

    float CalculateDamage(float distance)
    {
        // Escala el daño basado en la distancia
        if (distance < explosionRadius)
        {
            // Por ejemplo, el daño máximo es 40 y el mínimo es 10
            float maxDamage = 30f;
            float minDamage = 5f;
            // Cuanto más cerca, mayor es el daño
            return minDamage + (maxDamage - minDamage) * ((explosionRadius - distance) / explosionRadius);
        }
        return 0; // Fuera de rango no hay daño
    }

    // Visualización del radio de explosión en el editor de Unity
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
