using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Ring buffer of size S can hold up to S elements. If you add elements afterwards the first ones are going to be overwritten
/// </summary>
public class RingBuffer<T> : IEnumerable<T>
{
	private T[] _array = null;
	private int _size = 0;
	private int _count = 0;
	private int _start = 0;
	private int _end;
	
	public int Count { get {return _count;} }
	public int Capacity { get {return _size;} }
	
	public RingBuffer(int size)
	{
		_size = size;
		_array = new T[size];
	}
	
	public void Add(T item)
	{
		_array[_end] = item;
		
		_end = (++_end) % _size;
		//check if buffer is full, if so start overwriting
		if(_count >= _size) _start = (++_start) % _size;
		
		if(_count < _size) _count++;
	}
	
	//implement foreach access
	public IEnumerator<T> GetEnumerator()
	{
		int curr = _start;
		for(int i = 0; i < _count; ++i)
		{
			yield return _array[curr];
			curr = (++curr) % _size;
		}
	}
	IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
	
	//transverse reversed
	public IEnumerable<T> Reverse()
	{
		int curr = (_end - 1) < 0 ? _size-1 : _end - 1;
		for(int i = 0; i < _count; ++i)
		{
			yield return _array[curr];
			curr = (--curr) < 0 ? _size-1 : curr;
		}
	}
}
