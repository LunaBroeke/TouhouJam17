using UnityEngine;

public class TargetObject : MonoBehaviour
{
    public Target target = Target.None;
	public bool isIt = false;
	public void RandomizeStart()
	{
		isIt = false;
		target = Target.None;
		while (target == GameManager.instance.target || target == Target.None)
		{
			target = GameManager.instance.SetTargetRandom();
		}
		SetSprite();
	}
	public void ForceTarget(Target _target)
	{
		target = _target;
		isIt = true;
		SetSprite();
	}
	public void SetSprite()
	{
		SpriteRenderer _spriteRender = GetComponent<SpriteRenderer>();
		_spriteRender.sprite = GameManager.instance.sprite[(int)target];
	}
}
