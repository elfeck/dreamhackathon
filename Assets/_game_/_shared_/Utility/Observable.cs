using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Observer pattern

public interface Observer
{
	void onEvent(int eventID);
}

public class Observable : SASMonoBehaviour
{
	private List<Observer> _observers = new List<Observer>(8);
	
	public void registerObserver(Observer ob)
	{
		_observers.Add(ob);
	}
	
	public void unregisterObserver(Observer ob)
	{
		_observers.Remove(ob);
	}
	
	public void sendEvent(int eventID)
	{
		foreach(Observer ob in _observers)
		{
			ob.onEvent(eventID);
		}
	}
}
