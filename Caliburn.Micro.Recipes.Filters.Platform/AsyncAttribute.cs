﻿namespace Caliburn.Micro.Recipes.Filters
{
    using System.Threading;

    using Caliburn.Micro;
    using Caliburn.Micro.Recipes.Filters.Framework;

    /// <summary>
	/// Provides asynchronous execution of the action in a background thread
	/// </summary>
	public class AsyncAttribute : ExecutionWrapperBase
	{
        protected override void Execute(IResult inner, CoroutineExecutionContext context)
		{
			ThreadPool.QueueUserWorkItem(state =>
			{
				inner.Execute(context);
			});
		}

	}

	//usage:
	//[Async]
	//public void MyAction() { ... }

}
