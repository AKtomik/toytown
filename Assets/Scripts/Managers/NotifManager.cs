using System;
using UnityEngine;

namespace ToyTown {
	public class NotifManager : MonoBehaviour
	{
		public static NotifManager Instance { get; private set; }
		
		[SerializeField]
		GameObject notifPrefab;

		[SerializeField]
		Transform notifParent;

		// Start is called once before the first execution of Update after the MonoBehaviour is created
		void Start()
		{
			Instance = this;
			Debug.Log($"mono notifManager started");
			if (notifPrefab == null) throw new Exception($"notifPrefab is not defined, assign it in the unity editor in UnitManager!");
		}

		public void SpawnNotif(string content)
		{
			GameObject notifObject = Instantiate(notifPrefab, notifParent);
			NotifOne notifComponent = notifObject.GetComponent<NotifOne>();
			notifComponent.notifText = content;
		}
	}
}
