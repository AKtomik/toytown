using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ToyTown {
	public class GrabManager : MonoBehaviour
	{
		public InputActionReference inputMouseClick;
		public InputActionReference inputMousePosition;

		GameObject MouseHited()
		{
			Vector2 mousePos = inputMousePosition.action.ReadValue<Vector2>();
			Ray ray = Camera.main.ScreenPointToRay(mousePos);
			if (Physics.Raycast(ray, out RaycastHit hit))
			{
				return hit.collider.gameObject;
			}
			return null;
		}

		void OnClick(InputAction.CallbackContext callbackContext)
		{
			Debug.Log("click");
			GameObject hitedObject = MouseHited();
			if (hitedObject == null) return;
			Unit unit = hitedObject.GetComponent<Unit>();
			if (unit == null) return;
			Debug.Log($"moving unit {unit}");
			EnableDrag(new Unit[] { unit });
		}

		void OnRelease(InputAction.CallbackContext callbackContext)
		{
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
			Draging = true;
		}

		void DisableDrag()
		{
			Draging = false;
		}

		// Start is called once before the first execution of Update after the MonoBehaviour is created
		void Start()
		{
			inputMousePosition.action.Enable();
			inputMouseClick.action.Enable();
			inputMouseClick.action.performed += OnClick;
			inputMouseClick.action.canceled += OnRelease;
		}

		// Update is called once per frame
		void Update()
		{
			if (IsDrag())
			{
				Vector2 mousePos = inputMousePosition.action.ReadValue<Vector2>();
				foreach (Unit unit in Draged())
				{
					unit.transform.position = new Vector3(mousePos.x, mousePos.y, unit.transform.position.z);
				}
			}
		}
	}
}
