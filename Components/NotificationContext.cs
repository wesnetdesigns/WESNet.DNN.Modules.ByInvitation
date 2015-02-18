//Copyright (c) 2007-2014, William Severance, Jr., WESNet Designs
//All rights reserved.

//Redistribution and use in source and binary forms, with or without modification, are permitted
//provided that the following conditions are met:

//Redistributions of source code must retain the above copyright notice, this list of conditions
//and the following disclaimer.

//Redistributions in binary form must reproduce the above copyright notice, this list of conditions
//and the following disclaimer in the documentation and/or other materials provided with the distribution.

//Neither the name of William Severance, Jr. or WESNet Designs may be used to endorse or promote
//products derived from this software without specific prior written permission.

//Disclaimer: THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS
//            OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
//            AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER BE LIABLE
//            FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
//            LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
//            INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
//            OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN
//            IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

//Although I will try to answer questions regarding the installation and use of this software when
//such questions are submitted via e-mail to the below address, no promise of further
//support or enhancement is made nor should be assumed.

//Developer Contact Information:
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
using System.Web;

using DotNetNuke.Entities.Portals;

namespace WESNet.DNN.Modules.ByInvitation
{
    public class NotificationContext
    {
        private object[] properties = {-1, -1, ""};
        private string[] elementNames = {"PortalID", "ItemID", "ObjectType"};

        public int PortalID
        { 
            get
            {
                return (int)properties[0];
            }
        }

        public int ItemID
        {
            get
            {
                return (int)properties[1];
            }
        }

        public string ObjectType
        {
            get
            {
                return (string)properties[2];
            }
        }

        public NotificationContext(int portalID, int itemID, string objectType)
        {
            properties[0] = portalID;
            properties[1] = itemID;
            properties[2] = objectType;
        }

        public NotificationContext(PortalSettings portalSettings, int itemID, string objectType) :
            this(portalSettings.PortalId, itemID, objectType) { }

        public NotificationContext(InvitationInfo invitation)
        {
            DotNetNuke.Common.Guard.Against(invitation == null, "Cannot create NotificationContext as invitation is null");
            properties[0] = invitation.PortalId;
            properties[1] = invitation.InvitationID;
            properties[2] = "InvitationInfo";
        }

        public NotificationContext(BulkImportInfo bulkImport)
        {
            DotNetNuke.Common.Guard.Against(bulkImport == null, "Cannot create NotificationContext as bulkImport is null");
            properties[0] = bulkImport.PortalID;
            properties[1] = bulkImport.BulkImportID;
            properties[2] = "BulkImportInfo";
        }

        public NotificationContext(Object obj)
        {
            var typeName = obj.GetType().Name;
            switch (typeName)
            {
                case "InvitationInfo":
                    var invitation = (InvitationInfo)obj;
                    properties[0] = invitation.PortalId;
                    properties[1] = invitation.InvitationID;
                    break;
                case "BulkImportInfo":
                    var bulkImport = (BulkImportInfo)obj;
                    properties[0] = bulkImport.PortalID;
                    properties[1] = bulkImport.BulkImportID;
                    break;
                default:
                    throw new InvalidCastException("Object passed to a NotificationContext must be either 'InvitationInfo' or 'BulkImportInfo'.");
            }
            properties[2] = typeName;
        }
        
        public NotificationContext(string context)
        {
            var elements = context.Split(':');
            if (elements.Length != 3)
            {
                throw new ArgumentException("Notification context string must contain 2 integer and 1 string values separated by colons.", "context");
            }

            var i = 0;

            try
            {
                properties[i] = int.Parse(elements[i]); i++;
                properties[i] = int.Parse(elements[1]); i++;
                properties[i] = elements[i];
            }
            catch (Exception)
            {
                throw new ArgumentException("Invalid element '" + elementNames[i] + "' having value '" + elements[i] + "' found in notification context string.", "context");
            }
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}:{2}", properties);
        }
    }
}