using UnityEngine;

public class PlayerInputHandler : MonoBehaviour, ICharacterInputSource
{
    [SerializeField] private Camera inputCamera;

    private void Awake()
    {
        if (inputCamera == null)
        {
            inputCamera = Camera.main;
        }
    }

    public CharacterInputData GetInput()
    {
        var input = new CharacterInputData
        {
            Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")),
            AttackPressed = Input.GetKeyDown(KeyCode.Mouse0),
            DodgePressed = Input.GetKeyDown(KeyCode.Space),
            ParryPressed = Input.GetKeyDown(KeyCode.E)
        };

        if (inputCamera != null)
        {
            var mouseScreenPos = Input.mousePosition;
            var mouseWorldPos = inputCamera.ScreenToWorldPoint(mouseScreenPos);
            mouseWorldPos.z = 0f;
            input.AimPosition = mouseWorldPos;
            input.HasAimPosition = true;
        }

        return input;
    }
}
