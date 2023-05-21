using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine;
using Random = UnityEngine.Random;

public class Compass : MonoBehaviour
{
	public GameObject targetPrefab;
	public RawImage CompassImage;
	private Transform playerTransform;
	private MainPlayer _mainPlayer;
	public TextMeshProUGUI CompassDirectionText;
	public EventController eventController;
	private List<(GameObject,(Transform,TextMeshProUGUI, Color))> targets;

	private void Awake()
	{
		if (eventController == null)
		{
			Debug.LogError("To use Compass you need an EvenController set up!.");
			return;
		}

		gameObject.SetActive(true);
		_mainPlayer = this.eventController.MainPlayer;
		playerTransform = this.eventController.MainPlayer.transform;
	}

	private void Start()
	{
		List<RepairObject> repairObjects = eventController.RepairObjects;
		int sampleSize = (int)(repairObjects.Count * 0.1)+1;

		List<RepairObject> randomSampleRepairObjects = repairObjects.OrderBy(item => Random.Range(0, repairObjects.Count)).Take(sampleSize).ToList();

		AddRepairObjectTargets(randomSampleRepairObjects);
	}

	private void AddRepairObjectTargets(List<RepairObject> mainPlayerBoatParts)
	{
		List<int> ids = new List<int>();
		List<GameObject> targetGo = new List<GameObject>();
		foreach (var o in mainPlayerBoatParts)
		{
			targetGo.Add(o.gameObject);
			ids.Add(1);
		}
		addTargets(targetGo, ids);
	}

	public void addTargets(List<GameObject> targetGameObjects, List<int> ids)
	{
		if (targets == null) targets = new List<(GameObject, (Transform,TextMeshProUGUI,Color))>();
		for (int i = 0; i < targetGameObjects.Count; i++)
		{
			Color targetColor;
			switch (ids[i])
			{
				case 1:
					targetColor = Color.black;
					break;
				case 2:
					targetColor = Color.blue;
					break;
				case 3:
					targetColor = Color.yellow;
					break;
				default:
					targetColor = Color.red;
					break;
			}

			var transform1 = Instantiate(targetPrefab,transform).transform;
			transform1.gameObject.GetComponent<Image>().color = targetColor;
			targets.Add((targetGameObjects[i].gameObject,(transform1,transform1.Find("Distance").gameObject.GetOrAddComponent<TextMeshProUGUI>(),targetColor)));
		}
	}

	public void Update()
	{
		if (eventController == null)
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
			if (gameObj.IsDestroyed())
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
}