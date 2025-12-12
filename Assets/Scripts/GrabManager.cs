using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ToyTown {
	public class GrabManager : MonoBehaviour
	{
		public InputActionReference inputMouseClickPick;
		public InputActionReference inputMouseClickRelease;
		public InputActionReference inputMousePosition;
		public float YgrabPos = .5f;

		RaycastHit? MouseHitedPoint()
		{
			Vector2 mousePos = inputMousePosition.action.ReadValue<Vector2>();
			Ray ray = Camera.main.ScreenPointToRay(mousePos);
			if (Physics.Raycast(ray, out RaycastHit hit))
			{
				return hit;
			}
			return null;
		}
		
		GameObject MouseHitedObject()
		{
			return MouseHitedPoint()?.collider.gameObject;
		}

		void OnClick(InputAction.CallbackContext callbackContext)
		{
			//Debug.Log("click");
			GameObject hitedObject = MouseHitedObject();
			if (hitedObject == null) return;
			if (!hitedObject.TryGetComponent<Unit>(out var unit)) return;
			Debug.Log($"moving unit {unit}");
			EnableDrag(new Unit[] { unit });
		}

		void OnRelease(InputAction.CallbackContext callbackContext)
		{
			//Debug.Log("release");
			if (IsDrag())
			{
				DisableDrag();
			}
		}

		private bool Draging = false;
		private Unit[] DragedUnit;

		bool IsDrag()
		{
			return Draging;
		}
		
		Unit[] Draged()
		{
			if (!Draging) return new Unit[0];
			return DragedUnit;
		}

		void EnableDrag(Unit[] units)
		{
			DragedUnit = units;
			foreach (Unit unit in Draged())
			{
				unit.Grab();
			}
			Draging = true;
		}

		void DisableDrag()
		{
			foreach (Unit unit in Draged())
			{
				unit.Release();
			}
			DragedUnit = new Unit[0];
			Draging = false;
		}

		// Start is called once before the first execution of Update after the MonoBehaviour is created
		void Start()
		{
			Debug.Log($"grabManager started");
			inputMousePosition.action.Enable();
			inputMouseClickPick.action.Enable();
			inputMouseClickRelease.action.Enable();
			inputMouseClickPick.action.performed += OnClick;
			inputMouseClickRelease.action.performed += OnRelease;
		}

		// Update is called once per frame
		void Update()
		{
			if (IsDrag())
			{
				Vector3? mapPosPotential = MouseHitedPoint()?.point;
				if (mapPosPotential == null) return;
				Vector3 mapPos = (Vector3)mapPosPotential;
				
				foreach (Unit unit in Draged())
				{
					unit.transform.position = new Vector3(mapPos.x, YgrabPos, mapPos.z);
				}
			}
		}
	}
}
