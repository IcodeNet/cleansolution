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

using System.Activities;

namespace UnitTestingBookmarks
{
    public sealed class ReadLine : NativeActivity<string>
    {
        private BookmarkCallback _readCompleteCallback;

        [RequiredArgument]
        public InArgument<string> BookmarkName { get; set; }

        protected override bool CanInduceIdle
        {
            get { return true; }
        }

        public BookmarkCallback ReadCompleteCallback
        {
            get { return _readCompleteCallback ?? (_readCompleteCallback = new BookmarkCallback(OnReadComplete)); }
        }

        protected override void Execute(NativeActivityContext context)
        {
            // Inform the host that this activity needs data and wait for the callback
            context.CreateBookmark(BookmarkName.Get(context), ReadCompleteCallback);
        }

        private void OnReadComplete(NativeActivityContext context, Bookmark bookmark, object state)
        {
            // Store the value returned by the host
            context.SetValue(Result, state as string);
        }
    }
}