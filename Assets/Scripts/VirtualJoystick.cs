using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler {

    public Image background;
    public Image joystick;
    public Vector3 inputVector;

    //when joystick is pressed, run the onDrag method
    public virtual void OnPointerDown(PointerEventData ped)
    {
        OnDrag(ped);
    }

    //when joystick is not pressed, return joystick to the middle
    public virtual void OnPointerUp(PointerEventData ped)
    {
        inputVector = Vector3.zero;
        joystick.rectTransform.anchoredPosition = Vector3.zero;
    }

    public virtual void OnDrag(PointerEventData ped)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(background.rectTransform, ped.position, ped.pressEventCamera, out pos))
        {
            pos.x = (pos.x / background.rectTransform.sizeDelta.x);
            pos.y = (pos.y / background.rectTransform.sizeDelta.y);

            //Debug.Log(pos.x);
            //Debug.Log(pos.y);

            inputVector = new Vector3(pos.x * 2, 0f, pos.y * 2);
            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

            //Debug.Log(inputVector);

            joystick.rectTransform.anchoredPosition = new Vector2 (inputVector.x * (background.rectTransform.sizeDelta.x / 3), inputVector.z * (background.rectTransform.sizeDelta.y / 3));
        }
    }
    // Use this for initialization
    void Start () {
        background = GetComponent<Image>();
        joystick = transform.GetChild(0).GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
	}

    //return horizontal reference for player movement
    public Vector3 getInput()
    {
        return inputVector;
    }
}
