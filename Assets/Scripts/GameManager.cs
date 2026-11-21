using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public int amountToSpawnOnStart = 100;
    public Target target = Target.None;
    public GameObject targetPrefab;
    public List<GameObject> targetObjs;
    public List<GameObject> activeObjs;
    public List<GameObject> inactiveObjs;
    [Header("Field Management")]
    public Field activeField;
    public Field[] fields;
    [Header("State Tracking")]
    public GameState state = GameState.None;
    public float timer = 60f;
    public bool countTimer = false;
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
        if (countTimer && state == GameState.Game)
        {
            timer -= Time.deltaTime;
        }
        if (timer <= 0 && state == GameState.Game)
        {
            EndGame();
        }
	}
    [ContextMenu("Start Game")]
	public void StartGame()
    {
        for (int i = 0; i < amountToSpawnOnStart; i++)
        {
            GameObject _go = Instantiate(targetPrefab);
            targetObjs.Add(_go);
            SetObjectInactive(_go);
        }
        state = GameState.Game;
        StartRound();
	}
    private void EndGame()
    {
        state = GameState.End;
        DisableGameInput();
    }
    public void StartRound()
    {
		target = SetTargetRandom();
        activeField.Place();
		int i = Random.Range(0, activeObjs.Count);
		activeObjs[i].GetComponent<TargetObject>().ForceTarget(target);
        EnableGameInput();
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
        score += 1;
        StartCoroutine(StartNextRound());
    }
    private void WrongTarget()
    {
        timer -= 2;
    }
    public Target SetTargetRandom()
    {
        int i = Random.Range(1, 9);
        return (Target)i;
    }
    private IEnumerator StartNextRound()
    {
        if (state != GameState.Game) yield break;
        DisableGameInput();
        countTimer = false;
        GameObject _isIt = null;
        List<GameObject> _activeObjs = new List<GameObject>(activeObjs);
        foreach (GameObject _obj in _activeObjs)
        {
            if (_obj.GetComponent<TargetObject>().isIt)
            {
                _isIt = _obj;
                continue;
            }
            SetObjectInactive(_obj);
        }
        Debug.Log($"IsIt is {_isIt.name}");
        yield return new WaitForSeconds(2f);
        StartCoroutine(AddTimeOverTime(5, 0.25f));
        yield return new WaitForSeconds(1f);
        SetObjectInactive(_isIt);
        StartRound();
        countTimer = true;
        yield return new WaitForEndOfFrame();
    }
    private IEnumerator AddTimeOverTime(float bonusTime, float duration)
    {
        float elapsed = 0f;
        float addedSoFar = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float targetAdded = Mathf.Lerp(0f, bonusTime, t);
            float deltaThisFrame = targetAdded - addedSoFar;

            timer += deltaThisFrame;
            addedSoFar = targetAdded;
            yield return null;
        }
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
    public void SetObjectInactive(GameObject obj)
    {
        if (inactiveObjs.Contains(obj)) return;
        activeObjs.Remove(obj);
        inactiveObjs.Add(obj);
        obj.SetActive(false);
    }
    public void SetObjectActive(GameObject obj)
    {
        if (activeObjs.Contains(obj)) return;
        inactiveObjs.Remove(obj);
        activeObjs.Add(obj);
        obj.SetActive(true);
    }
}
