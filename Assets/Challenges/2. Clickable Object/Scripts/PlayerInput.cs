using Challenges._2._Clickable_Object.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private RaycastHit _raycastHit;

    private Vector3 _firstInputPosition;

    private ClickableObject _clickableObject;

    private bool _isInteractable => _raycastHit.transform.TryGetComponent<ClickableObject>(out _clickableObject);
    private bool _inputReleased => Input.GetMouseButtonUp(0);


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _firstInputPosition = Input.mousePosition;
        }

        CheckInputForSelecting();
    }

    private void CheckInputForSelecting()
    {
        if (!_inputReleased) return;

        Ray rayPoint = Camera.main.ScreenPointToRay(Input.mousePosition);

        bool ishit = Physics.Raycast(rayPoint, out _raycastHit, Mathf.Infinity);
        if (ishit && _isInteractable)
        {
            _clickableObject.RegisterToClickableDoubleTap(() => { });
        }
    }
}
