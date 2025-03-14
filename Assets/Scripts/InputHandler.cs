using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private GameField _gameField;
    
    private InputActions _inputActions;
    
    private Action<InputAction.CallbackContext> _moveUpHandler;
    private Action<InputAction.CallbackContext> _moveDownHandler;
    private Action<InputAction.CallbackContext> _moveLeftHandler;
    private Action<InputAction.CallbackContext> _moveRightHandler;
    
    private void Awake()
    {
        _gameField = FindFirstObjectByType<GameField>();
        _inputActions = new InputActions();
    }
    
    private void OnEnable()
    {
        _inputActions.Gameplay.Enable();
        
        _moveUpHandler = ctx => HandleInput(Vector2.up);
        _moveDownHandler = ctx => HandleInput(Vector2.down);
        _moveLeftHandler = ctx => HandleInput(Vector2.left);
        _moveRightHandler = ctx => HandleInput(Vector2.right);
        
        _inputActions.Gameplay.MoveUp.performed += _moveUpHandler;
        _inputActions.Gameplay.MoveDown.performed += _moveDownHandler;
        _inputActions.Gameplay.MoveLeft.performed += _moveLeftHandler;
        _inputActions.Gameplay.MoveRight.performed += _moveRightHandler;

        _inputActions.Gameplay.Swipe.performed += HandleSwipe;
    }

    private void OnDisable()
    {
        _inputActions.Gameplay.MoveUp.performed -= _moveUpHandler;
        _inputActions.Gameplay.MoveDown.performed -= _moveDownHandler;
        _inputActions.Gameplay.MoveLeft.performed -= _moveLeftHandler;
        _inputActions.Gameplay.MoveRight.performed -= _moveRightHandler;

        _inputActions.Gameplay.Swipe.performed -= HandleSwipe;
        
        _inputActions.Gameplay.Disable();
    }
    
    private void HandleInput(Vector2 direction)
    {
        Debug.Log($"Input received: {direction}");
        _gameField.MoveCells(direction);
    }
    
    private void HandleSwipe(InputAction.CallbackContext context)
    {
        Vector2 delta = context.ReadValue<Vector2>();
        
        Vector2 direction = GetSwipeDirection(delta);

        if (direction != Vector2.zero)
        {
            HandleInput(direction);
        }
    }
    
    private Vector2 GetSwipeDirection(Vector2 delta)
    {
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            return delta.x > 0 ? Vector2.right : Vector2.left;
        }
        if (Mathf.Abs(delta.x) < Mathf.Abs(delta.y))
        {
            return delta.y > 0 ? Vector2.up : Vector2.down;
        }
        
        return Vector2.zero;
    }
}
