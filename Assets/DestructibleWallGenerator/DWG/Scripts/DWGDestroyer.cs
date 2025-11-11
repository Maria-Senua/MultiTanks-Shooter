using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class DWGDestroyer : NetworkBehaviour {

	public float radius = 2;
	public float force = 50f;
	
	void OnCollisionEnter(Collision col){
		if (!IsServer) return;
		ExplodeForceServerRpc();
	}
	
	[ServerRpc(RequireOwnership = false)]
	private void ExplodeForceServerRpc(ServerRpcParams rpcParams = default)
	{
		ExplodeForce();

		Destroy(this);
	}
	
	// Explode force by radius only if a destructible tag is found
	void ExplodeForce(){	
		Vector3 explodePos = transform.position;
		Collider[] colliders = Physics.OverlapSphere(explodePos, radius); 
		foreach (Collider hit in colliders){
			if(hit.GetComponent<Collider>().tag == "Destructible"){
				if(hit.GetComponent<Rigidbody>()){
					hit.GetComponent<Rigidbody>().isKinematic = false; 
					hit.GetComponent<Rigidbody>().AddExplosionForce(force, explodePos,radius);
				}
			}
		}
	}
}
