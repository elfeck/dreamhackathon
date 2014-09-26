/**
 * This class is a messeging system which allows classes to register their Actions
 * and on the other hand to register listeners to certain events.
 * The code is inspired by:
 * Messenger.cs v0.1 (20090925) by Rod Hyde (badlydrawnrod).
 */

using System;
using System.Collections.Generic;

static public class Messenger
{
	private static Dictionary<Object, Dictionary<string, Delegate>> eventTable = new Dictionary<Object, Dictionary<string, Delegate>>(64);
	static private Object _currSender = null;
	static public Object getCurrentSender() {return _currSender;}

	//---------------------------------------------------------------------------//
	
	static public void AddListener(string eventType, Object eventOrigin, Action handler)
	{
		//Obtain a lock on the event table to keep this thread-safe.
		lock(eventTable)
		{
			//Create an entry for this event type if it doesn't exist
			if(!eventTable.ContainsKey(eventOrigin))
				eventTable[eventOrigin] = new Dictionary<string, Delegate>(8);
			if(!eventTable[eventOrigin].ContainsKey(eventType))
				eventTable[eventOrigin].Add(eventType, null);
			//Add the handler to the event.
			eventTable[eventOrigin][eventType] = (Action)eventTable[eventOrigin][eventType] + handler;
		}
	}
	
	static public void RemoveListener(string eventType, Object eventOrigin, Action handler)
	{
		lock(eventTable)
		{
			//Only take action if this event type exists
			if(!eventTable.ContainsKey(eventOrigin)) return;
			Dictionary<string, Delegate> dic = eventTable[eventOrigin];
			if(dic.ContainsKey(eventType))
			{
				//Remove the event handler from this event.
				dic[eventType] = (Action)dic[eventType] - handler;
				
				//If there's nothing left then remove the event type from the event table.
				if(dic[eventType] == null) dic.Remove(eventType);
				if(dic.Count <= 0) eventTable.Remove(eventOrigin);
			}
		}
	}
	
	//---------------------------------------------------------------------------//

	static public void Invoke(string eventType, Object sender)
	{
		Delegate d;
		Dictionary<string, Delegate> dic;
		if(!eventTable.TryGetValue(sender, out dic)) return;
		if(dic.TryGetValue(eventType, out d))
		{
			_currSender = sender;
			//Take a local copy to prevent a race condition if another thread
			//were to unsubscribe from this event.
			Action callback = (Action)d;
			if(callback != null) callback.Execute();
			_currSender = null;
		}
	}

	//---------------------------------------------------------------------------//
}

//---------------------------------------------------------------------------//

static public class Messenger<T>
{
	private static object _anonymousEventSource = new object();
	private static Dictionary<Object, Dictionary<string, Delegate>> eventTable = new Dictionary<Object, Dictionary<string, Delegate>>(64);
	private static Dictionary<string, Delegate> globalEventTable = new Dictionary<string,Delegate>(32);

	static private Object _currSender = null;
	static public Object getCurrentSender() {return _currSender;}

	//---------------------------------------------------------------------------//

	/// <summary>
	/// Listens to all invokes, no matter which origin (including anonymous invokes).
	/// </summary>
	/// <param name="eventName">The event to listen</param>
	/// <param name="handler">The Action to handle the invokes.</param>
	static public void AddGlobalListener(string eventName, Action<T> handler)
	{
		//Obtain a lock on the event table to keep this thread-safe.
		lock(globalEventTable)
		{
			if (!globalEventTable.ContainsKey(eventName))
				globalEventTable.Add(eventName, null);
			//Add the handler to the event.
			globalEventTable[eventName] = (Action<T>)globalEventTable[eventName] + handler;
		}
	}

	/// <summary>
	/// Listens only to calls made by anonymous invokes.
	/// </summary>
	/// <param name="eventName">The event to listen</param>
	/// <param name="handler">The callback to handle the invokes.</param>
	static public void AddAnonymousListener(string eventName, Action<T> handler)
	{
		AddListener(eventName, _anonymousEventSource, handler);
	}
	
	static public void AddListener(string eventName, Object eventOrigin, Action<T> handler)
	{
		//Obtain a lock on the event table to keep this thread-safe.
		lock(eventTable)
		{
			//Create an entry for this event type if it doesn't exist
			if(!eventTable.ContainsKey(eventOrigin))
				eventTable[eventOrigin] = new Dictionary<string, Delegate>(8);
			if(!eventTable[eventOrigin].ContainsKey(eventName))
				eventTable[eventOrigin].Add(eventName, null);
			//Add the handler to the event.
			eventTable[eventOrigin][eventName] = (Action<T>)eventTable[eventOrigin][eventName] + handler;
		}
	}

	//---------------------------------------------------------------------------//

	static public void RemoveGlobalListener(string eventName, Action<T> handler)
	{
		lock (globalEventTable)
		{
			if (!globalEventTable.ContainsKey(eventName)) return;

			globalEventTable[eventName] = (Action<T>)globalEventTable[eventName] - handler;
			//If there's nothing left then remove the event type from the event table.
			if (globalEventTable[eventName] == null) globalEventTable.Remove(eventName);
		}
	}

	static public void RemoveAnonymousListener(string eventName, Action<T> handler)
	{
		RemoveListener(eventName, _anonymousEventSource, handler);
	}
	
	static public void RemoveListener(string eventName, Object eventOrigin, Action<T> handler)
	{
		lock(eventTable)
		{
			//Only take action if this event type exists
			if(!eventTable.ContainsKey(eventOrigin)) return;
			Dictionary<string, Delegate> dic = eventTable[eventOrigin];
			if(dic.ContainsKey(eventName))
			{
				//Remove the event handler from this event.
				dic[eventName] = (Action<T>)dic[eventName] - handler;
				
				//If there's nothing left then remove the event type from the event table.
				if(dic[eventName] == null) dic.Remove(eventName);
				if(dic.Count <= 0) eventTable.Remove(eventOrigin);
			}
		}
	}

	//---------------------------------------------------------------------------//

	static public void InvokeAnonymously(string eventName, T arg0)
	{
		Invoke(eventName, _anonymousEventSource, arg0);
	}

	static public void Invoke(string eventName, Object sender, T arg0)
	{
		//use a try-catch block since the invocation of an event, and thus arbitrary callbacks
		//should never abort the execution of the invoker!

		Delegate d, d2;
		Dictionary<string, Delegate> dic;
		//If there is a global listener defined, call it.
		if(globalEventTable.TryGetValue(eventName, out d2))
		{
			Action<T> callback = (Action<T>)d2;
			if(callback != null) callback.Execute(arg0);
		}
		if(!eventTable.TryGetValue(sender, out dic)) return;
		if(dic.TryGetValue(eventName, out d))
		{
			_currSender = sender;
			//Take a local copy to prevent a race condition if another thread
			//were to unsubscribe from this event.
			Action<T> callback = (Action<T>)d;
			if(callback != null) callback.Execute(arg0);
			_currSender = null;
		}
	}

	//---------------------------------------------------------------------------//
}