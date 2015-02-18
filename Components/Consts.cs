// Copyright (c) 2015, William Severance, Jr., WESNet Designs
// All rights reserved.

// Redistribution and use in source and binary forms, with or without modification, are permitted
// provided that the following conditions are met:

// Redistributions of source code must retain the above copyright notice, this list of conditions
// and the following disclaimer.

// Redistributions in binary form must reproduce the above copyright notice, this list of conditions
// and the following disclaimer in the documentation and/or other materials provided with the distribution.

// Neither the name of William Severance, Jr. or WESNet Designs may be used to endorse or promote
// products derived from this software without specific prior written permission.

// Disclaimer: THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS
//             OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
//             AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER BE LIABLE
//             FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
//             LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
//             INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
//             OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN
//             IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

// Although I will try to answer questions regarding the installation and use of this software when
// such questions are submitted via e-mail to the below address or through the Issue Tracker on this project's
// CodePlex project site (http://dnnbyinvitation.codeplex.com  no promise of further support or enhancement
// is made nor should be assumed.

// Developer Contact Information:
//     William Severance, Jr.
//     WESNet Designs
//     679 Upper Ridge Road
//     Bridgton, ME 04009
//     Phone: 207-317-1365
//     E-Mail: bill@wesnetdesigns.com
//     Website: www.wesnetdesigns.com

using System;
using System.Collections.Generic;

namespace WESNet.DNN.Modules.ByInvitation
{
    public class Consts
    {
        public const string ImportFolderPath = "App_Data\\WESNet_ByInvitation\\";   
     
        public const string ValidationGroup = "WESNet_ByInvitation";

        public const string DefaultOleDbProvider = "Microsoft.ACE.OLEDB.12.0";
        public const string MSJet4OleDbProvider = "Microsoft.Jet.OLEDB.4.0";
        public const string MSJet12OleDbProvider = "Microsoft.ACE.OLEDB.12.0";

        public static TimeSpan MinimumAllocationPeriod = TimeSpan.FromSeconds(30.0);
        public static TimeSpan DefaultAllocationPeriod = TimeSpan.FromDays(1.0);

        public static List<string[]> ValidFileExtensions = new List<string[]>() { new[] { ".csv", ".txt" }, new[]{ ".xls", ".xlsx" }, new[]{ ".xml" }, new[]{ ".mdb", ".accdb" }, new[]{ "" } };

        public const double InvitationCacheDuration = 5.0; // 5 minutes

        public static string[] InvitationFieldNames = new string[] {"RecipientCultureCode", "RecipientEmail", "RecipientFirstName", "RecipientLastName", "AssignedUsername",
                                                                    "AssignedDisplayName", "TemporaryPassword", "PersonalNote", "RedirectOnFirstLogin", "AssignedRoles"};

        public static string[] InvitationFieldTypes = new string[] { "String", "String", "String", "String", "String", "String", "String", "String", "String", "String" };

        public static int[] InvitationFieldWidths = { 10, 256, 50, 50, 100, 128, 128, 1000, 200, 128 };

        public static bool[] InvitationFieldRequired = new bool[] { false, true, true, true, false, false, false, false, false, false };

        //Notification Types
        public static NotificationDescriptor SentNotification = new NotificationDescriptor("SentNotification", "Invitation sent notification");

        public static NotificationDescriptor ModerationRequestedNotification = new NotificationDescriptor("ModerationRequested", "Invitation needing moderation created notification");

        public static NotificationDescriptor InvitationStatusChangeNotification = new NotificationDescriptor("InvitationStatusChange", "Invitation status change notification");

        public static NotificationDescriptor BulkImportStatusChangeNotification = new NotificationDescriptor("BulkImportStatusChange", "Bulk import status change notification");

    }
}