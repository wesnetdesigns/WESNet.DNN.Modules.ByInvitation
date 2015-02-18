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
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Entities.Host;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Mail;
using DotNetNuke.Security.Roles;

namespace WESNet.DNN.Modules.ByInvitation
{
    public class MailManager
    {
        public enum MailSendMethods
        {
            Default,
            To,
            CC,
            BCC
        }

        private static Regex UserIdRegex = new Regex(@"^\[(\d+)\]$", RegexOptions.Compiled);
        private static Regex RoleIdRegex = new Regex(@"^(\d+)$", RegexOptions.Compiled);
        private UserInfo _AccessingUser = null;

        private List<string> _RecipientEmails;
        private List<RoleInfo> _RecipientRoles;
        private List<DotNetNuke.Entities.Users.UserInfo> _RecipientUsers;

        private List<string> _ALL = new List<string>();
        private List<string> _TO = new List<string>();
        private List<string> _CC = new List<string>();
        private List<string> _BCC = new List<string>();


        public bool Debug { get; set; }

        public int PortalId { get; private set; }

        private PortalSettings PortalSettings { get; set; }

        private Configuration MyConfiguration { get; set; }

        public MailSendMethods MailSendMethod { get; set; }

        public MailPriority MailPriority { get; set; }

        public UserInfo AccessingUser
        {
            get
            {
                if (_AccessingUser == null)
                {
                    if (HttpContext.Current != null)
                    {
                        _AccessingUser = UserController.Instance.GetCurrentUserInfo();
                    }
                    else
                    {
                        _AccessingUser = Invitation.InvitedByUser;
                    }
                }
                return _AccessingUser;
            }
            set
            {
                _AccessingUser = value;
            }
        }

        public List<string> RecipientEmails
        {
            get
            {
                if (_RecipientEmails == null)
                {
                    _RecipientEmails = new List<string>();
                }
                return _RecipientEmails;
            }
        }

        public List<RoleInfo> RecipientRoles
        {
            get
            {
                if (_RecipientRoles == null)
                {
                    _RecipientRoles = new List<RoleInfo>();
                }
                return _RecipientRoles;
            }
        }

        public List<DotNetNuke.Entities.Users.UserInfo> RecipientUsers
        {
            get
            {
                if (_RecipientUsers == null)
                {
                    _RecipientUsers = new List<DotNetNuke.Entities.Users.UserInfo>();
                }
                return _RecipientUsers;
            }
        }

        public string SendFromAddr { get; set; }
        public string ReplyToAddr { get; set; }
        public string SendToAddr { get; set; }
        public string SendCCAddr { get; set; }
        public string SendBCCAddr { get; set; }

        public string SendToRoles { get; set; }

        public char Separator { get; set; }

        public bool RemoveDuplicates { get; set; }

        public bool ReplaceTokens { get; set; }

        public MailFormat MailFormat { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public InvitationInfo Invitation { get; set; }

        public List<string> ALL
        {
            get
            {
                return _ALL;
            }
        }
        public List<string> TO
        {
            get
            {
                return _TO;
            }
        }

        public List<string> CC
        {
            get
            {
                return _CC;
            }
        }

        public List<string> BCC
        {
            get
            {
                return _BCC;
            }
        }


        #region Constructors

        public MailManager(int invitationID) : this(InvitationController.GetInvitation(invitationID)) {}

        public MailManager(InvitationInfo invitation)
        {
            Invitation = invitation;
            PortalId = invitation.PortalId;
            PortalSettings = new PortalSettings(invitation.TabId, PortalId);
            MyConfiguration = InvitationController.GetConfiguration(PortalId);

            Separator = ',';
            MailSendMethod = MailSendMethods.To;
            MailPriority = MailPriority.Normal;
            RemoveDuplicates = true;
            ReplaceTokens = true;
            MailFormat = MailFormat.Html;
        }

        #endregion


        #region Public Methods

        public void ClearTORecipients()
        {
            TO.Clear();
        }

        public void ClearCCRecipients()
        {
            CC.Clear();
        }

        public void ClearBCCRecipients()
        {
            BCC.Clear();
        }

        private void ClearALLRecipients()
        {
            ALL.Clear();
        }

        public void ClearAllRecipientLists()
        {
            ClearTORecipients();
            ClearCCRecipients();
            ClearBCCRecipients();
            ClearALLRecipients();
        }

        public void AddUserEmailToRecipients(List<string> recipients, int userId)
        {
            if (userId > 0)
            {
                var user = UserController.GetUserById(PortalId, userId);
                if (user != null && user.Membership.Approved && !(user.IsDeleted || user.Membership.LockedOut))
                {
                    AddEmailToRecipients(recipients, user.Email);
                }
            }
        }

        public void AddEmailToRecipients(List<string> recipients, string email)
        {
            if (!string.IsNullOrEmpty(email) && !IsDuplicate(email))
            {
                recipients.Add(email);
                ALL.Add(email);
            }
        }

        public void AddEmailListToRecipients(List<string> recipients, IEnumerable<string> emails)
        {
            foreach (string email in emails)
            {
                AddEmailToRecipients(recipients, email);
            }
        }

        public void AddUsersInRolesToRecipients(List<string> recipients, IEnumerable<string> roles)
        {
            foreach (string role in roles)
            {
                int uid = -1;
                int roleid = -1;

                if (role != string.Empty)
                {
                    Match m = UserIdRegex.Match(role);
                    if (m.Success)
                    {
                        uid = int.Parse(m.Groups[1].Value);
                        AddUserEmailToRecipients(recipients, uid);
                    }
                    else
                    {
                        m = RoleIdRegex.Match(role);
                        if (m.Success)
                        {
                            roleid = int.Parse(m.Groups[1].Value);
                            var rc = new RoleController();
                            var objRole = rc.GetRoleById(PortalId, roleid);
                            if (objRole != null)
                            {
                                foreach (UserInfo user in RoleController.Instance.GetUsersByRole(PortalId, objRole.RoleName))
                                {
                                    var userRole = rc.GetUserRole(PortalId, user.UserID, objRole.RoleID);
                                    if (userRole.EffectiveDate <= DateTime.Now
                                         && (!Null.IsNull(userRole.ExpiryDate) && userRole.ExpiryDate >= DateTime.Now))
                                    {
                                        AddUserEmailToRecipients(recipients, userRole.UserID);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void AppendEmails(StringBuilder sb, List<string> recipients)
        {
            AppendEmails(sb, recipients, 0, true);
        }

        private void AppendEmails(StringBuilder sb, List<string> recipients, bool trimTrailingSeparator)
        {
            AppendEmails(sb, recipients, 0, trimTrailingSeparator);
        }

        private void AppendEmails(StringBuilder sb, List<string> recipients, int startIndex, bool trimTrailingSeparator)
        {
            for (int i = startIndex; i < recipients.Count; i++)
            {
                sb.Append(recipients[i]);
                sb.Append(Separator);
            }
            if (sb.Length > 0 && trimTrailingSeparator) sb.Length--;
        }

        public string SendBulkMail(string messageKey)
        {
            var result = string.Empty;
            var to = new StringBuilder();
            var bcc = new StringBuilder();
            var cc = new StringBuilder();

            var subject = string.Empty;
            var body = string.Empty;
            MailFormat mailFormat;
            try
            {
                if (string.IsNullOrEmpty(Subject) && string.IsNullOrEmpty(Body))
                {
                    switch (messageKey.ToLowerInvariant())
                    {
                        case "send":
                        case "resend":

                            ClearAllRecipientLists();
                            var localizedConfiguration = MyConfiguration.LocalizedConfigurations[Invitation.RecipientCultureCode];
                            subject = localizedConfiguration.InvitationSubject;
                            body = HttpUtility.HtmlDecode(localizedConfiguration.InvitationBody);

                            SendToAddr = Invitation.RecipientEmail;
                            if (MyConfiguration.SendCopiesTo != "" || MyConfiguration.SendCopiesToRoles != "")
                            {

                                SendCCAddr = MyConfiguration.SendCopiesTo;
                                SendToRoles = MyConfiguration.SendCopiesToRoles;
                                MailSendMethod = MailManager.MailSendMethods.BCC;
                            }
                            else
                            {
                                MailSendMethod = MailManager.MailSendMethods.To;
                            }
                            break;
                        case "approved":
                            ClearAllRecipientLists();
                            subject = Localization.GetString("Approved.Subject", Configuration.LocalSharedResourceFile);
                            body = Localization.GetString("Approved.Body", Configuration.LocalSharedResourceFile);
                            SendToAddr = Invitation.InvitedByUserEmail;
                            MailSendMethod = MailSendMethods.To;
                            break;
                        case "disapproved":
                            ClearAllRecipientLists();
                            subject = Localization.GetString("Disapproved.Subject", Configuration.LocalSharedResourceFile);
                            body = Localization.GetString("Disapproved.Body", Configuration.LocalSharedResourceFile);
                            SendToAddr = Invitation.InvitedByUserEmail;
                            MailSendMethod = MailSendMethods.To;
                            break;
                        case "":
                            return "MailManager Error: messageKey cannot be empty in call to SendMail unless Subject and Body have also been set.";
                        default:
                            subject = GetLocalizedString(messageKey + ".Subject");
                            body = GetLocalizedString(messageKey + ".Body");
                            break;
                    }
                }

                mailFormat = (MailFormat == MailFormat.Text ? MailFormat.Text : (HtmlUtils.IsHtml(body) ? MailFormat.Html : MailFormat.Text));
                body = (MailFormat == MailFormat.Html ? Globals.ManageUploadDirectory(body, PortalSettings.HomeDirectory) : body);

                if (string.IsNullOrEmpty(subject)) return "MailManager Error: Subject cannot be empty or null";
                if (string.IsNullOrEmpty(body)) return "MailManager Error: Body cannot be empty or null";

                string tokenizedSubject = subject;
                string tokenizedBody = body;

                if (ReplaceTokens)
                {
                    var tokenizer = new Tokenizer(Invitation);
                    tokenizedSubject = tokenizer.ReplaceInvitationTokens(subject);
                    tokenizedBody = tokenizer.ReplaceInvitationTokens(body);
                }

                var portalAdminUser = UserController.GetUserById(PortalId, PortalSettings.AdministratorId);
                var portalAdminEmail = portalAdminUser.Email;

                if (string.IsNullOrEmpty(MyConfiguration.SendFrom))
                {
                    SendFromAddr = portalAdminEmail;
                }
                else
                {
                    SendFromAddr = MyConfiguration.SendFrom;
                }

                ReplyToAddr = Invitation.InvitedByUserEmail;

                if (!string.IsNullOrEmpty(SendToAddr))
                {
                    AddEmailListToRecipients(TO, EscapedString.Seperate(SendToAddr, Separator));
                }

                if (!string.IsNullOrEmpty(SendCCAddr))
                {
                    AddEmailListToRecipients(CC, EscapedString.Seperate(SendCCAddr, Separator));
                }

                if (!string.IsNullOrEmpty(SendBCCAddr))
                {
                    AddEmailListToRecipients(BCC, EscapedString.Seperate(SendBCCAddr, Separator));
                }

                if (!string.IsNullOrEmpty(SendToRoles))
                {
                    List<string> recipientList = null;
                    switch (MailSendMethod)
                    {
                        case MailSendMethods.Default:
                        case MailSendMethods.To:
                            recipientList = TO;
                            break;
                        case MailSendMethods.CC:
                            recipientList = CC;
                            break;
                        case MailSendMethods.BCC:
                            recipientList = BCC;
                            break;
                    }

                    AddUsersInRolesToRecipients(recipientList, EscapedString.Seperate(SendToRoles, Separator));
                }

                if (TO.Count == 0)
                    if (MailSendMethod == MailSendMethods.CC && CC.Count > 0)
                    {
                        var tmp = CC[0];
                        TO.Add(tmp);
                        CC.Remove(tmp);
                    }
                    else if (MailSendMethod == MailSendMethods.BCC && BCC.Count > 0)
                    {
                        var tmp = BCC[0];
                        TO.Add(tmp);
                        BCC.Remove(tmp);
                    }


                switch (MailSendMethod)
                {
                    case MailSendMethods.Default:
                        AppendEmails(to, TO);
                        AppendEmails(cc, CC);
                        AppendEmails(bcc, BCC);
                        break;
                    case MailSendMethods.To:
                        AppendEmails(to, TO.Concat(CC.Concat(BCC)).ToList<string>(), true);
                        break;
                    case MailSendMethods.CC:
                        AppendEmails(to, TO);
                        AppendEmails(cc, CC.Concat(BCC).ToList<string>(), true);
                        break;
                    case MailSendMethods.BCC:
                        AppendEmails(to, TO);
                        AppendEmails(bcc, CC.Concat(BCC).ToList<string>(), true);
                        break;
                }

                if (TO.Count == 0 && CC.Count == 0 && BCC.Count == 0) return "MailManager Error: No recipients were specified to receive email(s)";

                if (Debug) // Create or append log of each call to SendMail to file 'EmailLog.txt' in Portal Home Directory
                {
                    using (var fs = new System.IO.StreamWriter(PortalSettings.HomeDirectoryMapPath + "EmailLog.txt", true))
                    {
                        var sb = new StringBuilder();
                        sb.AppendLine("SendMail(" + messageKey + ") ------------------------------");
                        sb.AppendLine("  MailFormat: " + mailFormat.ToString());
                        sb.AppendLine("  From: " + SendFromAddr);
                        sb.AppendLine("  To: " + to.ToString());
                        sb.AppendLine("  CC: " + cc.ToString());
                        sb.AppendLine("  BCC: " + bcc.ToString());
                        sb.AppendLine("  ReplyTo: " + ReplyToAddr);
                        sb.AppendLine("  Subject: " + tokenizedSubject);
                        sb.AppendLine("  Body:");
                        sb.AppendLine("    " + tokenizedBody);
                        sb.AppendLine("----------------------------------------------------------");
                        fs.Write(sb.ToString());
                        fs.Flush();
                        result = "";
                    }
                }
                else
                {
                    if (MailSendMethod == MailSendMethods.To)
                    {
                        foreach (string mailTo in TO)
                        {
                            result += SendMail(SendFromAddr, mailTo, cc.ToString(), bcc.ToString(), ReplyToAddr, MailPriority.Normal, tokenizedSubject, mailFormat, tokenizedBody);
                        }
                    }
                    else
                    {
                        result += SendMail(SendFromAddr, to.ToString(), cc.ToString(), bcc.ToString(), ReplyToAddr, MailPriority.Normal, tokenizedSubject, mailFormat, tokenizedBody);
                    }

                }
            }
            catch (Exception exc)
            {
                result += "Mail Manager Exception: " + exc.Message;
            }

            return result;
        }

        #endregion

        #region Private Methods

        private bool IsDuplicate(string email)
        {
            return RemoveDuplicates && ALL.Contains(email);
        }

        private string GetLocalizedString(string key)
        {
            return Localization.GetString(key, Localization.LocalSharedResourceFile);
        }

        // Core DotNetNuke.Services.Mail.Mail does not provide method having simple signature with mailCC or replyTo parameters without attachments
        private string SendMail(string mailFrom, string mailTo, string mailCC, string mailBCC, string replyTo,
                                MailPriority priority, string subject, MailFormat bodyFormat, string body)
        {
            var attachments = new List<System.Net.Mail.Attachment>();
            return Mail.SendMail(mailFrom, mailTo, mailCC, mailBCC, replyTo, priority, subject, bodyFormat, Encoding.UTF8, body, attachments, "", "", "", "", Host.EnableSMTPSSL);
        }

        #endregion


    }
}