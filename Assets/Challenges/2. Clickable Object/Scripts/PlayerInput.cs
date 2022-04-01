using Challenges._2._Clickable_Object.Scripts;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private RaycastHit _raycastHit;

    private ClickableObject _clickableObject;

    private bool _isInteractable => _raycastHit.transform.TryGetComponent<ClickableObject>(out _clickableObject);
    private bool _isInputReleased => Input.GetMouseButtonUp(0);
    private bool _isDoubleClicked;
    private bool _isTapAndHold;
    private bool _isRaycastHit;

    [Header("Options")]
    [SerializeField]
    private float _doubleClickTimeThreshold = 0.25f;
    [SerializeField]
    private float _tapAndHoldTimeThreshold = 0.6f;
    private float _duration;
    private float _inputFirstTouchTime;

    async UniTaskVoid Start()
    {
        DetectInput();
    }

    private async UniTaskVoid DetectInput()
    {
        try
        {
            while (true)
            {
                _duration = 0;
                _isDoubleClicked = false;
                _isTapAndHold = false;

                if (Input.GetMouseButtonDown(0))
                {
                    Ray rayPoint = Camera.main.ScreenPointToRay(Input.mousePosition);
                    _isRaycastHit = Physics.Raycast(rayPoint, out _raycastHit, Mathf.Infinity);

                    _inputFirstTouchTime = Time.time;
                }

                if (_isInputReleased)
                {
                    float timePassed = Time.time - _inputFirstTouchTime;
                    if (timePassed > _tapAndHoldTimeThreshold)
                    {
                        //Tap and hold detected
                        _isTapAndHold = true;
                        if (_isRaycastHit && _isInteractable)
                            _clickableObject.RegisterToClickableTapAndHold(() => { });
                        Debug.Log($"Holded for: {timePassed} seconds.");
                    }

                    while (_duration < _doubleClickTimeThreshold)
                    {
                        _duration += Time.deltaTime;
                        await UniTask.Delay(2);

                        if (!_isInputReleased) continue;

                        // Double click detected
                        _isDoubleClicked = true;
                        _duration = _doubleClickTimeThreshold;
                        if (_isRaycastHit && _isInteractable)
                            _clickableObject.RegisterToClickableDoubleTap(() => { });
                    }
                    if (!_isDoubleClicked && !_isTapAndHold)
                    {
                        // Single click detected
                        if (_isRaycastHit && _isInteractable)
                            _clickableObject.RegisterToClickableTap(() => { });
                    }
                }
                await UniTask.Yield();
            }
        }
        catch (Exception ex) when (!(ex is OperationCanceledException)) // when (ex is not OperationCanceledException) at C# 9.0
        {
            Debug.LogWarning(ex.Message);
            DetectInput();
        }
    }
}
