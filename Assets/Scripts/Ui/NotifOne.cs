using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotifOne : MonoBehaviour
{
	[SerializeField]
	GameObject OkButton;
	
	[SerializeField]
	TextMeshProUGUI TextComponent;
	
	[SerializeField]
	Image ImageComponent;

	public string notifText
	{
		get { return TextComponent.text; }
		set { TextComponent.text = value; }
	}
	
	public Color notifColor
	{
		get { return ImageComponent.color; }
		set { ImageComponent.color = value; }
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	public void SetOkVisibility(bool visible)
	{
		OkButton.SetActive(visible);
	}
	
	public void ClickOkButton()
	{
		Destroy(gameObject);
	}
}
