using UnityEngine;
using UnityEngine.InputSystem;
public enum GameState
{
    None = 0,
    Title,
    Game,
    End,
}
public enum Target
{
    None = 0,
    Flandre,
    Remilia,
    Sakuya,
    Patchy,
    Koakuma,
    Meiling,
    Cirno,
    Rumia,
    Daiyousei,
    Reimu,
    Marisa,
}
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("Gameplay Tracking")]
    public Target target = Target.None;
    public GameObject[] targetObjs;
    [Header("State Tracking")]
    public GameState State = GameState.None;
    [Header("Score Tracking")]
    public int score = 0;
    public int[] hiscore = new int[5] {0,0,0,0,0};
    [Header("Mouse Tracking")]
    public Vector2 mouseScreenPosition;
    public Vector2 mouseWorldPosition;
    private PlayerInput inputActions;
    [SerializeField] private LayerMask layerMask;
    [Header("SpriteList")]
    public Sprite[] sprite;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(gameObject);
        inputActions = new PlayerInput();
	}
	private void Update()
	{
        if (Mouse.current != null)
        {
            mouseScreenPosition = Mouse.current.position.ReadValue();
            mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        }
	}
    [ContextMenu("Start Game")]
	public void StartGame()
    {
        State = GameState.Game;
		targetObjs = GameObject.FindGameObjectsWithTag("fumo");
        target = SetTargetRandom();
        foreach (GameObject _obj in targetObjs)
        {
            TargetObject _targetObj = _obj.GetComponent<TargetObject>();
            _targetObj.RandomizeStart();
            _obj.SetActive(true);
        }
        int i = Random.Range(0, targetObjs.Length);
        targetObjs[i].GetComponent<TargetObject>().ForceTarget(target);
	}
    public void ConfirmCheck(InputAction.CallbackContext context)
    {
        if (Mouse.current == null) return;

        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero, Mathf.Infinity,layerMask);
        if (hit.collider != null)
        {
            if (hit.collider.TryGetComponent(out TargetObject tarObj))
            {
                Target t = tarObj.target;
                CheckTarget(t);
                Debug.Log($"Grabbed {t} from {tarObj.name}");
            }
        }
    }
    public void CheckTarget(Target t = Target.None)
    {
        if (t == target)
        {
            CorrectTarget();
        }
        else
        {
            WrongTarget();
        }
    }
    private void CorrectTarget()
    {
        SoundManager sound = SoundManager.instance;
        sound.PlaySound(sound.jingleSuccess);
    }
    private void WrongTarget()
    {

    }
    public Target SetTargetRandom()
    {
        int i = Random.Range(1, 9);
        return (Target)i;
    }
    public void EnableGameInput()
    {
        inputActions.Player.Enable();
        inputActions.Player.Confirm.performed += ConfirmCheck;
    }
    public void DisableGameInput()
    {
        inputActions.Player.Confirm.performed -= ConfirmCheck;
        inputActions.Player.Disable();
    }
	private void OnDisable()
	{
		DisableGameInput();
	}
	private void OnEnable()
	{
		EnableGameInput();
	}
}
