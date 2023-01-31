using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Myd.Platform.Demo
{
    public class Coroutine
    {
		public bool Finished { get; private set; }
		public bool Active { get; set; }
		public Coroutine(IEnumerator functionCall, bool removeOnComplete = true)
		{
			this.enumerators = new Stack<IEnumerator>();
			this.enumerators.Push(functionCall);
			this.Active = true;
			this.RemoveOnComplete = removeOnComplete;
		}

		public Coroutine(bool removeOnComplete = true)
		{
			this.RemoveOnComplete = removeOnComplete;
			this.enumerators = new Stack<IEnumerator>();
			this.Active = false;
		}

		public void Update(float deltaTime)
		{
			this.ended = false;
			if (this.waitTimer > 0f)
			{
				this.waitTimer -= deltaTime;
				return;
			}
			if (this.enumerators.Count > 0)
			{
				IEnumerator enumerator = this.enumerators.Peek();
				if (enumerator.MoveNext() && !this.ended)
				{
					if (enumerator.Current is int)
					{
						this.waitTimer = (float)((int)enumerator.Current);
					}
					if (enumerator.Current is float)
					{
						this.waitTimer = (float)enumerator.Current;
						return;
					}
					if (enumerator.Current is IEnumerator)
					{
						this.enumerators.Push(enumerator.Current as IEnumerator);
						return;
					}
				}
				else if (!this.ended)
				{
					this.enumerators.Pop();
					if (this.enumerators.Count == 0)
					{
						this.Active = false;
						this.Finished = true;
					}
				}
			}
		}

		public void Cancel()
		{
			this.Active = false;
			this.Finished = true;
			this.waitTimer = 0f;
			this.enumerators.Clear();
			this.ended = true;
		}

		public void Replace(IEnumerator functionCall)
		{
			this.Active = true;
			this.Finished = false;
			this.waitTimer = 0f;
			this.enumerators.Clear();
			this.enumerators.Push(functionCall);
			this.ended = true;
		}

		public bool RemoveOnComplete = true;

		public bool UseRawDeltaTime;

		private Stack<IEnumerator> enumerators;

		private float waitTimer;

		private bool ended;
	}
}
