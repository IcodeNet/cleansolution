// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SilverlightFaultElement.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   The silverlight fault element.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SecurityDoor.Web
{
    using System;
    using System.ServiceModel.Configuration;

    /// <summary>
    /// The silverlight fault element.
    /// </summary>
    public class SilverlightFaultElement : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(SilverlightFaultBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new SilverlightFaultBehavior();
        }
    }
}