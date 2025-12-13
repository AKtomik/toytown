using UnityEngine;

public class BouncingBall : MonoBehaviour
{
    [Header("Param�tres de rebond")]
    [SerializeField] private float bounceForce = 5f;
    [SerializeField] private float gravity = 9.81f;

    [Header("Physique")]
    [SerializeField] private PhysicsMaterial bounceMaterial;

    private Rigidbody rb;

    void Start()
    {
        // R�cup�re ou ajoute un Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        // Configuration du Rigidbody
        rb.mass = 0.5f; // Masse du ballon
        rb.linearDamping = 0.1f; // R�sistance de l'air

        // R�cup�re ou ajoute un Collider sph�rique
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        if (sphereCollider == null)
        {
            sphereCollider = gameObject.AddComponent<SphereCollider>();
        }

        // Cr�e ou applique le mat�riau physique
        if (bounceMaterial == null)
        {
            bounceMaterial = new PhysicsMaterial("BounceMaterial");
            bounceMaterial.bounciness = 0.8f; // Coefficient de rebond (0 � 1)
            bounceMaterial.dynamicFriction = 0.3f;
            bounceMaterial.staticFriction = 0.3f;
            bounceMaterial.bounceCombine = PhysicsMaterialCombine.Maximum;
        }

        sphereCollider.material = bounceMaterial;

        // Lance le ballon vers le haut au d�marrage (optionnel)
        rb.AddForce(Vector3.up * bounceForce, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Optionnel : ajoute un peu de force suppl�mentaire au rebond
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Calcule la direction du rebond
            Vector3 bounceDirection = Vector3.Reflect(rb.linearVelocity.normalized, collision.contacts[0].normal);

            // Ajoute une petite force pour accentuer le rebond
            rb.AddForce(bounceDirection * bounceForce * 0.1f, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        // Applique une gravit� personnalis�e si n�cessaire
        // (Par d�faut Unity utilise Physics.gravity)
        // rb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
    }
}