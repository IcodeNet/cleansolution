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
using System.Linq;
using System.Threading;

namespace UnitTestingBookmarks
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            const int workflowCompleted = 0;
            const int workflowIdle = 1;

            var syncEvents = new[]
                                 {
                                     new AutoResetEvent(false),
                                     new AutoResetEvent(false),
                                 };
            var wfApp = new WorkflowApplication(new TestReadLine());
            string bookmarkName = string.Empty;
            bool workflowComplete = false;

            // Signal the main thread we are done
            wfApp.Completed = (e) => syncEvents[workflowCompleted].Set();

            // When the host detects an idle
            wfApp.Idle = (e) =>
                             {
                                 // Search the bookmarks
                                 foreach (
                                     var bi in
                                         e.Bookmarks.Where(
                                             bi => bi.BookmarkName == "FirstName" || bi.BookmarkName == "LastName"))
                                 {
                                     Console.WriteLine("Found bookmark {0}", bi.BookmarkName);
                                     // For FirstName or LastName bookmarks prompt with a readline
                                     bookmarkName = bi.BookmarkName;
                                     syncEvents[workflowIdle].Set();
                                 }
                             };

            wfApp.Run();

            while (!workflowComplete)
            {
                // Wait for events
                switch (WaitHandle.WaitAny(syncEvents, TimeSpan.FromSeconds(5)))
                {
                    case WaitHandle.WaitTimeout:
                        Console.WriteLine("Sorry you took too long");
                        wfApp.Terminate("Timeout");
                        break;

                    case workflowCompleted:
                        workflowComplete = true;
                        break;

                    case workflowIdle:
                        Console.WriteLine("Reading response for bookmark {0}", bookmarkName);
                        // Resume with the response from the user
                        wfApp.ResumeBookmark(bookmarkName, Console.ReadLine());
                        break;
                }
            }

            Console.WriteLine("Sample Completed - press any key to exit");
            Console.ReadKey(true);
        }
    }
}