﻿#region copyright

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

using System.ServiceModel;
using Microsoft.Activities.UnitTesting.Tests.Activities;

namespace Microsoft.Activities.UnitTesting.Tests
{
    [ServiceContract]
    internal interface IServiceWithCorrelation
    {
        [OperationContract]
        DataResponse GetData(int data);

        [OperationContract]
        DataResponse GetMoreData(int data, int key);
    }
}