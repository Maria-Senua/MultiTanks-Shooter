using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class DWGColliderEnabler : NetworkBehaviour {
	public override void OnNetworkSpawn() {
		if (!IsServer) return;
		EnableCollider();
	}

	

	void EnableCollider()
	{
		Debug.Log("DWGColliderEnabler enabled");
		if(gameObject.GetComponent<Collider>()){ // If this game object has a collider, continue
			gameObject.GetComponent<Collider>().enabled = true; // Enable the collider
			Destroy(GetComponent<DWGColliderEnabler>()); // Remove this script from the game object
		}
	}
}
