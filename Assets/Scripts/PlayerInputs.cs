using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInputs : MonoBehaviour
{

	public GameObject prefabFingerTrail;

	bool drawing;
	bool canDraw;
	Vector3 lastMousePosition;
	Stack poolPoints;
	Transform trailsNode;
	List<GameObject> inputPositions;
	Vector4 touchZone;


	void setOriginalSize(Image image)
	{
		Vector2 sizeDelta = image.rectTransform.sizeDelta;
		sizeDelta.x = image.sprite.texture.width;
		sizeDelta.y = image.sprite.texture.height;
		image.rectTransform.sizeDelta = sizeDelta;
	}



	void Start()
	{
		canDraw = true;
		drawing = false;
		inputPositions = new List<GameObject>();
		poolPoints = new Stack();

		if (prefabFingerTrail == null)
		{
			Debug.LogError("Te olvidaste un public param en PlayerInputs");
			Debug.Break();
		}
		prefabFingerTrail.GetComponent<Image>().color = Constants.drawTrailColor;

		trailsNode = transform.Find("Trails");
		RectTransform rectTransform = GetComponent<RectTransform>();

		Image inputFieldBorder = transform.Find("InputFieldBorder").GetComponent<Image>();
		setOriginalSize(inputFieldBorder);
		float UIScale = (float)Screen.width / inputFieldBorder.sprite.texture.width;
		float totalHeight = inputFieldBorder.sprite.texture.height * UIScale;

		Vector3 borderLocalScale = rectTransform.localScale;
		borderLocalScale.x = UIScale;
		borderLocalScale.y = UIScale;
		rectTransform.localScale = borderLocalScale;
		Vector3 localPosition = rectTransform.localPosition;
		localPosition.x = 0;
		localPosition.y = -Screen.height / 2 + totalHeight / 2;
		rectTransform.localPosition = localPosition;

		Image inputFieldCanvas = transform.Find("InputFieldCanvas").GetComponent<Image>();
		setOriginalSize(inputFieldCanvas);

		Vector3 canvasLocalScale = inputFieldCanvas.rectTransform.localScale;
		canvasLocalScale.x = 488f / inputFieldCanvas.sprite.texture.width;
		canvasLocalScale.y = 290f / inputFieldCanvas.sprite.texture.height;
		inputFieldCanvas.rectTransform.localScale = canvasLocalScale;
		inputFieldCanvas.material.mainTextureScale = new Vector2(canvasLocalScale.x / UIScale, canvasLocalScale.y / UIScale);

		touchZone = new Vector4(30 * UIScale, 30 * UIScale, 452 * UIScale, 254 * UIScale);


	}

	bool isInside(Vector4 bounds, Vector3 mousePos)
	{
		return mousePos.x >= bounds.x && mousePos.x < bounds.x + bounds.z && mousePos.y >= bounds.y && mousePos.y < bounds.y + bounds.w;
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Vector3 mousePosition = Input.mousePosition;
			if (isInside(touchZone, mousePosition))
			{
				drawing = true;
				GameObject finger = getPoint();
				finger.transform.position = mousePosition;
				finger.transform.SetParent(trailsNode);
				finger.transform.localScale = Vector3.one * Constants.fingerDrawSpriteScale;
				inputPositions.Add(finger);

			}

		}
		if (Input.GetMouseButtonUp(0))
		{
			drawing = false;

			Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
			player.updateLegs(inputPositions);



			foreach (GameObject point in inputPositions)
			{
				pointToPool(point);
			}
			inputPositions.Clear();
		}
		if (drawing)
		{
			Vector3 mousePosition = Input.mousePosition;
			if (mousePosition != lastMousePosition)
			{
				if (isInside(touchZone, mousePosition))
				{
					Vector3 lastPosition = inputPositions[inputPositions.Count - 1].transform.position;
					Vector3 diference = mousePosition - lastPosition;
					float distance = Vector3.Magnitude(diference);
					Vector3 normalizedDiference = diference / distance;
					int count = Mathf.FloorToInt(distance / Constants.pixelPerFingerSample);

					for (int i = 0; i < count; i++)
					{
						Vector3 position = lastPosition + normalizedDiference * (distance * i / count);
						GameObject finger = getPoint();
						finger.transform.position = position;
						finger.transform.SetParent(trailsNode);
						finger.transform.localScale = Vector3.one * Constants.fingerDrawSpriteScale;
						inputPositions.Add(finger);
					}

					lastMousePosition = mousePosition;
				}
				else
				{
					drawing = false;
				}
			}
		}
	}

	GameObject getPoint()
	{
		if (poolPoints.Count > 0)
		{
			GameObject top = poolPoints.Pop() as GameObject;
			top.SetActive(true);
			return top;
		}
		GameObject point = Instantiate(prefabFingerTrail);
		return point;
	}

	void pointToPool(GameObject point)
	{
		point.SetActive(false);
		poolPoints.Push(point);
	}

}
