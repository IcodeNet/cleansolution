#region copyright

//  ----------------------------------------------------------------------------------
//  Microsoft
//  
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
//  EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
//  ----------------------------------------------------------------------------------
//  The example companies, organizations, products, domain names,
//  e-mail addresses, logos, people, places, and events depicted
//  herein are fictitious.  No association with any real company,
//  organization, product, domain name, email address, logo, person,
//  places, or events is intended or should be inferred.
//  ----------------------------------------------------------------------------------

#endregion

using System;
using System.Activities;

namespace WorkflowExtensionExample
{
    public sealed class GetAverage : CodeActivity<string>
    {
        public InArgument<int> Number { get; set; }


        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            // This activity requires the average extension
            metadata.RequireExtension(typeof (AverageExtension));

            // This lambda will create one if it is not already added
            metadata.AddDefaultExtensionProvider(() => new AverageExtension());

            base.CacheMetadata(metadata);
        }

        protected override string Execute(CodeActivityContext context)
        {
            // Get the average extension
            var average = context.GetExtension<AverageExtension>();

            if (average == null)
                throw new InvalidOperationException("Cannot access AverageExtension");

            var number = Number.Get(context);

            // Store this number
            average.StoreNumber(number);

            return string.Format("Stored {0}, Average:{1}", number, average.GetAverage());
        }
    }
}