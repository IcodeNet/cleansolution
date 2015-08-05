// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityWithOptionalArgsContext.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Activities;
using System.ComponentModel;

using Microsoft.Activities.Extensions;

/// <summary>
/// An activity with optional args and context
/// </summary>
// ReSharper disable CheckNamespace
public sealed class ActivityWithOptionalArgsContext : CodeActivity<string>
{
    // ReSharper restore CheckNamespace
    #region Constants

    /// <summary>
    /// The default optional value
    /// </summary>
    public const int DefaultOptionalValue = 1;

    #endregion

    // Prevent serialization of null value
    #region Public Properties

    /// <summary>
    /// Gets or sets OptionalArg.
    /// </summary>
    [DefaultValue(null)]
    public InArgument<int> OptionalArg { get; set; }

    /// <summary>
    /// Gets or sets RequiredArg.
    /// </summary>
    [RequiredArgument]
    public InArgument<string> RequiredArg { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// When implemented in a derived class, performs the execution of the activity.
    /// </summary>
    /// <returns>
    /// The result of the activity’s execution.
    /// </returns>
    /// <param name="context">The execution context under which the activity executes.</param>
    protected override string Execute(CodeActivityContext context)
    {
        var num = context.GetValue(this.OptionalArg, DefaultOptionalValue);

        return string.Format("{0}: {1}", this.RequiredArg.Get(context), num);
    }

    #endregion
}