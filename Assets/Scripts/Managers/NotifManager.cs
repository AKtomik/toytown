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


		public NotifOne SpawnNotif()
		{
			GameObject notifObject = Instantiate(notifPrefab, notifParent);
			NotifOne notifComponent = notifObject.GetComponent<NotifOne>();
			return notifComponent;
		}
		public NotifOne SpawnNotif(string content)
		{
			NotifOne notifComponent = SpawnNotif();
			notifComponent.notifText = content;
			return notifComponent;
		}
		public NotifOne SpawnNotif(string content, Color color)
		{
			NotifOne notifComponent = SpawnNotif(content);
			notifComponent.notifColor = color;
			return notifComponent;
		}
		
		public NotifOne SpawnGoodNews(string content)
		{
			return SpawnNotif(content, new Color(0, .8f, 0, .6f));
		}
		public NotifOne SpawnBadNews(string content)
		{
			return SpawnNotif(content, new Color(.8f, 0, 0, .6f));
		}
		public NotifOne SpawnMidNews(string content)
		{
			return SpawnNotif(content, new Color(0, .8f, 0, .6f));
		}
		public NotifOne SpawnInfo(string content)
		{
			return SpawnNotif(content, new Color(0, 0, .3f, .6f));
		}
	}
}
