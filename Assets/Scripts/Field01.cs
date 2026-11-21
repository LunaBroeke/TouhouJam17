using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field01 : Field
{
    [SerializeField] private int cols = 10;
    [SerializeField] private int rows = 5;
    public override void Place()
    {
        base.Place();
        List<Vector2> _slots = new List<Vector2>(cols * rows);
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                _slots.Add(new Vector2(startX + x * cellW, startY + y * cellH));
                //Debug.Log($"Added {x} and {y}");
            }
        }
        List<GameObject> _objects = new List<GameObject>(GameManager.instance.inactiveObjs);
        int _limit = Mathf.Min(_objects.Count, _slots.Count);
        for (int i = 0; i < _limit; i++)
        {
            GameObject _obj = _objects[i];
            int _randomSlot = Random.Range(0,_slots.Count);
            PlaceAtPosition(_obj,_slots[_randomSlot]);
            _slots.RemoveAt(_randomSlot);
			TargetObject _targetObj = _obj.GetComponent<TargetObject>();
			_targetObj.RandomizeStart();
			GameManager.instance.SetObjectActive(_obj);
		}
    }
}
