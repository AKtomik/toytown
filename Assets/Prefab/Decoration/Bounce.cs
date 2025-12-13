using UnityEngine;

public class BouncingBall : MonoBehaviour
{
	[SerializeField]
	PhysicsMaterial bounceMaterial;
	
	[SerializeField]
	float baseSpeed;

	private Rigidbody rb;
	private float lastSpeed;

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
		if (rb == null)
			rb = gameObject.AddComponent<Rigidbody>();

		rb.mass = 0.5f;
		rb.linearDamping = 0f;
		rb.angularDamping = 0f;
		rb.useGravity = true;
		rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

		SphereCollider col = GetComponent<SphereCollider>();
		if (col == null)
			col = gameObject.AddComponent<SphereCollider>();

		if (bounceMaterial == null)
		{
			bounceMaterial = new PhysicsMaterial("InfiniteBounce");
			bounceMaterial.bounciness = 1f;
			bounceMaterial.dynamicFriction = 0f;
			bounceMaterial.staticFriction = 0f;
			bounceMaterial.bounceCombine = PhysicsMaterialCombine.Maximum;
			bounceMaterial.frictionCombine = PhysicsMaterialCombine.Minimum;
		}

		col.material = bounceMaterial;
	}

	void FixedUpdate()
	{
		lastSpeed = rb.linearVelocity.magnitude;
	}

	void OnCollisionEnter(Collision collision)
	{
		Vector3 dir = rb.linearVelocity.normalized;
		rb.linearVelocity = dir * lastSpeed;
	}
}