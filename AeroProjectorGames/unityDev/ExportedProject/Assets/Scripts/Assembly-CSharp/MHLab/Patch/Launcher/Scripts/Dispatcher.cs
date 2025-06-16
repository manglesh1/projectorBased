using System;
using System.Collections.Generic;
using UnityEngine;

namespace MHLab.Patch.Launcher.Scripts
{
	public sealed class Dispatcher : MonoBehaviour
	{
		private readonly Queue<Action> _actions = new Queue<Action>();

		public void Invoke(Action action)
		{
			Queue<Action> actions = _actions;
			lock (actions)
			{
				_actions.Enqueue(action);
			}
		}

		private void Update()
		{
			Queue<Action> actions = _actions;
			lock (actions)
			{
				while (_actions.Count > 0)
				{
					_actions.Dequeue()();
				}
			}
		}
	}
}
