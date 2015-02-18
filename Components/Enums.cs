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
using System.Linq;

using DotNetNuke;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Security.Membership;

namespace WESNet.DNN.Modules.ByInvitation
{
    public enum InvitationSubmissionStatus
    {
        Undefined = 0,
        SubmittedTooSoon,
        InvitationAlreadyPending,
        InvitationAlreadyAccepted,
        InvitationAlreadyDeclined,
        InvalidEmail,
        DuplicateEmail,
        InvalidUsername,
        UsernameAlreadyExists,
        UserAlreadyRegistered,
        InvalidDisplayName,
        DuplicateDisplayName,
        TemporaryPasswordRequired,
        InvalidPassword,
        InvalidRedirectOnLogin,
        InvalidRole,
        InvalidRoleName,
        InvalidRoleEffectiveDate,
        InvalidRoleExpiryDate,
        FailureSavingInvitation,
        RequiredFieldDataMissing,
        Success
    }

    [Flags()]
    public enum RoleValidityStatus
    {
        ValidRole = 0,
        InvalidRoleName = 1,
        InvalidRoleEffectiveDate = 2,
        InvalidRoleExpiryDate = 4
    }

    [Flags()]
    public enum InvitationStatus
    {
        None = 0,
        Created = 1,
        Approved = 2,
        Disapproved = 4,
        Sent = 8,
        Pending = 16,
        Resent = 32,
        Retracted = 64,
        LockedOut = 128,
        Expired = 256,
        Declined = 512,
        Accepted = 1024,
        All = Created | Approved | Disapproved | Sent | Pending | Resent | Retracted | LockedOut | Expired | Declined | Accepted
    }

    [Flags()]
    public enum Notifications
    {
        None = 0,
        Created = 1,
        PendingModeration = 2,
        Approved = 4,
        Disapproved = 8,
        Sent = 16,
        Errored = 32,
        Resent = 64,
        Retracted = 128,
        LockedOut = 256,
        Expired = 512,
        Declined = 1024,
        Accepted = 2048,
        Finished = 4096,
        DigestOnly = 8192,
        All = Created | PendingModeration |Approved | Disapproved | Sent | Errored | Resent | Retracted | LockedOut | Expired | Declined | Accepted | Finished
    }

    public enum ExpiredInvitationActions
    {
        None = 0,
        Delete,
        Archive
    }

    public enum Mode
    {
        View = 0,
        Add,
        Edit,
        EditApprove,
        ManageInvitations,
        BulkImport
    }

    public enum CaptchaUsage
    {
        NoCaptcha = 0,
        AnonymousUsersOnly,
        AllUsers
    }

    public enum TemporaryPasswordMode
    {
        NoTemporaryPassword = 0,
        AutoGenerateTemporaryPassword,
        OptionalTemporaryPassword,
        RequiredTemporaryPassword
    }

    public enum InvitationFilter
    {
        NoFiltering,
        SenderUserID,
        SenderIPAddr,
        SenderEmail
    }

    public enum ImportSourceTypes
    {
        CSV = 0,
        Excel,
        XML,
        DatabaseFile,
        DatabaseConnection
    }

    public enum SendMethods
    {
        Sync = 0,
        ASync,
        Scheduled
    }
}