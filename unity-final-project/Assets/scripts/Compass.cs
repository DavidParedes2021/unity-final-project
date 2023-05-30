using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Compass : MonoBehaviour
{
	public GameObject targetPrefab;
	public RawImage CompassImage;
	private Transform playerTransform;
	private MainPlayer _mainPlayer;
	public TextMeshProUGUI CompassDirectionText;
	[FormerlySerializedAs("eventController")] public EC ec;
	private List<(GameObject,(Transform,TextMeshProUGUI, Color))> targets;
	public static (int, Color)[] idColors = { (1, Color.black), (2, Color.red), (3, Color.yellow), (4, Color.blue) };

	public static Color GetColorById(int id)
	{
		foreach (var (colorId, color) in idColors)
		{
			if (colorId == id)
			{
				return color;
			}
		}
		// Return a default color if the ID is not found
		return Color.white;
	}

	public static int GetIdByColor(Color color)
	{
		foreach (var (colorId, col) in idColors)
		{
			if (col == color)
			{
				return colorId;
			}
		}

		// Return -1 if the color is not found
		return -1;
	}
	private void Awake()
	{
		if (ec == null)
		{
			Debug.LogError("To use Compass you need an EvenController set up!.");
			return;
		}
	}

	private void Start()
	{
		gameObject.SetActive(true);
		_mainPlayer = this.ec.MainPlayer;
		playerTransform = this.ec.MainPlayer.transform;
		
		List<RepairObject> repairObjects = ec.RepairObjects;
		int sampleSize = (int)(repairObjects.Count * 0.1)+1;

	}

	public void AddTargets(List<GameObject> targetGameObjects, int id)
	{
		List<int> ids = targetGameObjects.Select(_ => id).ToList();
		AddTargets(targetGameObjects,ids);
	}
	public void AddTargets(List<GameObject> targetGameObjects, List<int> ids)
	{
		if (targets == null) targets = new List<(GameObject, (Transform,TextMeshProUGUI,Color))>();
		for (int i = 0; i < targetGameObjects.Count; i++)
		{
			Color targetColor = GetColorById(ids[i]);
			var transform1 = Instantiate(targetPrefab,transform).transform;
			transform1.gameObject.GetComponent<Image>().color = targetColor;
			targets.Add((targetGameObjects[i].gameObject,(transform1,transform1.Find("Distance").gameObject.GetOrAddComponent<TextMeshProUGUI>(),targetColor)));
		}
	}

	public void Update()
	{
		if (ec == null)
		{
			gameObject.SetActive(false);
		}
		//Get a handle on the Image's uvRect
		CompassImage.uvRect = new Rect(playerTransform.localEulerAngles.y / 360, 0, 1, 1);

		// Get a copy of your forward vector
		Vector3 forward = playerTransform.transform.forward;

		// Zero out the y component of your forward vector to only get the direction in the X,Z plane
		forward.y = 0;

		//Clamp our angles to only 5 degree increments
		float headingAngle = Quaternion.LookRotation(forward).eulerAngles.y;
		headingAngle = 5 * (Mathf.RoundToInt(headingAngle / 5.0f));
		
		foreach (var (gameObj, (targetTransform,TextMeshProUGUI,targetColor)) in targets)
		{
			if (gameObj.IsDestroyed() || !gameObj.activeSelf)
			{
				targetTransform.gameObject.SetActive(false);
				continue;
			}
			// Get the angle of the gameObj relative to the player's forward direction
			Vector3 toTarget = gameObj.transform.position - playerTransform.transform.position;
			float angle = Vector3.SignedAngle(toTarget, playerTransform.transform.forward, Vector3.up);
			TextMeshProUGUI.SetText((Vector3.Distance(gameObj.transform.position, playerTransform.transform.position)).ToString("F1"));

			// Check if the target is within the visible range of the compass
			if (Mathf.Abs(angle) <= 180f / 2f)
			{
				targetTransform.gameObject.SetActive(true);
				// set the position based on the angle
				float x = (CompassImage.rectTransform.rect.width/2f)*-1*angle / (180f / 2f);
				targetTransform.localPosition = new Vector3(x, targetPrefab.transform.position.y, targetPrefab.transform.position.z);
			}
			else
			{
				targetTransform.gameObject.SetActive(false);
			}
		}


		SetAngleText(headingAngle);
	}

	private void SetAngleText(float headingAngle)
	{
		//Convert float to int for switch
		int displayangle;
		displayangle = Mathf.RoundToInt(headingAngle);

		//Set the text of Compass Degree Text to the clamped value, but change it to the letter if it is a True direction
		switch (displayangle)
		{
			case 0:
				//Do this
				CompassDirectionText.text = "N";
				break;
			case 360:
				//Do this
				CompassDirectionText.text = "N";
				break;
			case 45:
				//Do this
				CompassDirectionText.text = "NE";
				break;
			case 90:
				//Do this
				CompassDirectionText.text = "E";
				break;
			case 130:
				//Do this
				CompassDirectionText.text = "SE";
				break;
			case 180:
				//Do this
				CompassDirectionText.text = "S";
				break;
			case 225:
				//Do this
				CompassDirectionText.text = "SW";
				break;
			case 270:
				//Do this
				CompassDirectionText.text = "W";
				break;
			default:
				CompassDirectionText.text = headingAngle.ToString();
				break;
		}
	}

	public void RemoveTargets(Color type)
	{
		if (targets != null)
		{
			
			var targetsToRemove = new List<(GameObject, (Transform, TextMeshProUGUI, Color))>();
			for (var i = 0; i < targets.Count; i++)
			{
				if (targets[i].Item2.Item3 == type)
				{
					targetsToRemove.Add(targets[i]);
					Destroy(targets[i].Item1);
				}
			}
			// Remove the targets that match the specified color from the 'targets' list
			foreach (var targetToRemove in targetsToRemove)
			{
				targets.Remove(targetToRemove);
			}
		}
    
	}

	public void RemoveTargets()
	{
		if (targets != null)
		{
			for (var i = 0; i < targets.Count; i++) {
				Destroy(targets[i].Item1.gameObject);
			}
		}
		targets?.RemoveRange(0,targets.Count);
	}
}