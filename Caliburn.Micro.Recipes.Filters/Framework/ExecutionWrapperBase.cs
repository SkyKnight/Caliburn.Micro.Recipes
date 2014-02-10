﻿namespace Caliburn.Micro.Recipes.Filters.Framework
{
    using System;

    using Caliburn.Micro;

    public abstract class ExecutionWrapperBase : Attribute, IExecutionWrapper, IResult
	{
		public int Priority { get; set; }

		/// <summary>
		/// Check prerequisites
		/// </summary>
        protected virtual bool CanExecute(CoroutineExecutionContext context) { return true; }
		/// <summary>
		/// Called just before execution (if prerequisites are met)
		/// </summary>
        protected virtual void BeforeExecute(CoroutineExecutionContext context) { }
		/// <summary>
		/// Called after execution (if prerequisites are met)
		/// </summary>
        protected virtual void AfterExecute(CoroutineExecutionContext context) { }
		/// <summary>
		/// Allows to customize the dispatch of the execution
		/// </summary>
		protected virtual void Execute(IResult inner, CoroutineExecutionContext context)
		{
			inner.Execute(context);
		}
		/// <summary>
		/// Called when an exception was thrown during the action execution
		/// </summary>
        protected virtual bool HandleException(CoroutineExecutionContext context, Exception ex) { return false; }



		IResult _inner;
		IResult IExecutionWrapper.Wrap(IResult inner)
		{
			this._inner = inner;
			return this;
		}


        void IResult.Execute(CoroutineExecutionContext context)
		{
			if (!this.CanExecute(context))
			{
				this._completedEvent.Invoke(this, new ResultCompletionEventArgs { WasCancelled = true });
				return;
			}


			try
			{

				EventHandler<ResultCompletionEventArgs> onCompletion = null;
				onCompletion = (o, e) =>
				{
					this._inner.Completed -= onCompletion;
					this.AfterExecute(context);
					this.FinalizeExecution(context, e.WasCancelled, e.Error);
				};
				this._inner.Completed += onCompletion;

				this.BeforeExecute(context);
				this.Execute(this._inner, context);

			}
			catch (Exception ex)
			{
				this.FinalizeExecution(context, false, ex);
			}
		}

        void FinalizeExecution(CoroutineExecutionContext context, bool wasCancelled, Exception ex)
		{
			if (ex != null && this.HandleException(context, ex))
				ex = null;

			this._completedEvent.Invoke(this, new ResultCompletionEventArgs { WasCancelled = wasCancelled, Error = ex });
		}

		event EventHandler<ResultCompletionEventArgs> _completedEvent = delegate { };
		event EventHandler<ResultCompletionEventArgs> IResult.Completed
		{
			add { this._completedEvent += value; }
			remove { this._completedEvent -= value; }
		}
	}
}
