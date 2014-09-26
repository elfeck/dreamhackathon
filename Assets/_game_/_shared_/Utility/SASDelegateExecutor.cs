using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This is an awesome SASDelegateExecutor. It provides exception-friendly execution of multicast delegates
/// (i.e. if there are more than one delegates in the invocation list)
/// \authors Martin Kolb
/// </summary>
public static class SASDelegateExecutor
{
	//------------------------- Property declaration -----------------------------//
	#region Property declaration

	#endregion
	//--------------------------- Property access --------------------------------//
	#region Property access

	#endregion
	//-------------------------- Method declarations -----------------------------//
	#region Method declarations
	
	/// <summary>
	/// Execute delegates in a exception-friendly way. All delegates will be executed as well as their exception will be printed to console if one thrown.
	/// </summary>
	/// <typeparam name="T">The type of delegae (i.e. the System.Action type)"/></typeparam>
	/// <param name="self">The System.Action which carries multiple delegates to execute</param>
	/// <param name="reverse">Optional parameter to execute the delegate list in a reverse order</param>
	public static void Execute<T>(this System.Action<T> self, T param, bool reverse = false)
	{
		System.Delegate[] delegates = self.GetInvocationList();

		if(reverse)
		{
			for(int i = delegates.Length-1; i >= 0; --i)
				ExecuteItemFromDelegateList<T>(delegates, i, param);
		}
		else
		{
			for(int i = 0; i < delegates.Length; ++i)
				ExecuteItemFromDelegateList<T>(delegates, i, param);
		}
	}
	private static void ExecuteItemFromDelegateList<T>(System.Delegate[] delegates, int i, T param)
	{
		try
		{
			System.Action<T> action = (System.Action<T>) delegates[i];
			action(param);
		}
		catch (System.Exception e)
		{
			Debug.LogException(e);
		}
	}

	//---------------------------------------------------------------------------//

	/// <summary>
	/// Execute delegates in a exception-friendly way. All delegates will be executed as well as their exception will be printed to console if one thrown.
	/// </summary>
	/// <param name="self">The System.Action which carries multiple delegates to execute</param>
	/// <param name="reverse">Optional parameter to execute the delegate list in a reverse order</param>
	public static void Execute(this System.Action self, bool reverse = false)
	{
		System.Delegate[] delegates = self.GetInvocationList();

		if(reverse)
		{
			for(int i = delegates.Length-1; i >= 0; --i)
				ExecuteItemFromDelegateList(delegates, i);
		}
		else
		{
			for(int i = 0; i < delegates.Length; ++i)
				ExecuteItemFromDelegateList(delegates, i);
		}
	}
	private static void ExecuteItemFromDelegateList(System.Delegate[] delegates, int i)
	{
		try
		{
			System.Action action = (System.Action)delegates[i];
			action();
		}
		catch(System.Exception e)
		{
			Debug.LogException(e);
		}
	}
	
	#endregion
}
