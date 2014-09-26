using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BitFlagArray
{
	[SerializeField] private byte[] _flags;
	[SerializeField] private bool _supportQueue = false;
	[SerializeField] private List<int> _queue;
	
	public BitFlagArray(int flagCount, bool supportQueue)
	{
		int size = flagCount / 8 + 1;
		_flags = new byte[size];
		_supportQueue = supportQueue;
		if(_supportQueue) _queue = new List<int>(32);
		clear();
	}
	
	public List<int> getQueue() {return _queue;}
	
	//sets all flags to zero, and empties queue
	public void clear()
	{
		for(int i = 0; i < _flags.Length; ++i)
			_flags[i] = 0;
		if(_supportQueue) _queue.Clear();
	}
	
	public bool isFlagSet(int index)
	{
		int elem = index >> 3; // index / 8
		int bit = index % 8;
		int mask = 0x01 << bit;
		return (_flags[elem] & (byte)mask) > 0;
	}
	//sets the flag to one, and adds the index to the queue
	public void setFlag(int index)
	{
		if(isFlagSet(index)) return;
		int elem = index >> 3;
		int bit = index % 8;
		int mask = 0x01 << bit;
		_flags[elem] |= (byte)mask;
		if(_supportQueue) _queue.Add(index);
	}
	//this method sets the dirty-flag to zero
	public void unsetFlag(int index, bool removeFromQueue)
	{
		if(removeFromQueue && isFlagSet(index))
			_queue.Remove(index);
		int elem = index >> 3;
		int bit = index % 8;
		int mask = 0xFF ^ (0x01 << bit);
		_flags[elem] &= (byte)mask;
	}
}