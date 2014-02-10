﻿namespace Caliburn.Micro.Recipes.Filters
{
    using Caliburn.Micro;
    using Caliburn.Micro.Recipes.Filters.Framework;

    /// <summary>
	/// Simply skip the action execution
	/// </summary>
	public class DoNotExecuteAttribute : ExecutionWrapperBase
	{
        protected override bool CanExecute(CoroutineExecutionContext context)
		{
			return false;
		}
	}
}
