﻿// Copyright (c) 2015, William Severance, Jr., WESNet Designs
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
using System.Text;
using System.Xml.Linq;
using DotNetNuke.Common.Utilities;

namespace WESNet.DNN.Modules.ByInvitation
{
    public class LogEntry
    {
        private Dictionary<string, string> _Properties = new Dictionary<string, string>();

        public DateTime Timestamp { get; private set; }
        public int BatchNumber { get; private set; }
        public Dictionary<string, string> Properties
        {
            get
            {
                return _Properties;
            }
        }

        public LogEntry(int batchNumber)
        {
            Timestamp = DateUtils.GetDatabaseTime();
            BatchNumber = batchNumber;
        }

        public LogEntry(string message, int batchNumber) : this(batchNumber)
        {
            Properties.Add("message", message);
        }

        public void AddProperty(string propertyName, string propertyValue)
        {
            Properties.Add(propertyName, propertyValue);
        }

        public override string ToString()
        {
            var sb = new StringBuilder(string.Format("<logEntry timeStamp='{0:O}' batchNumber='{1}'>", Timestamp, BatchNumber));
            sb.AppendLine("  <properties>");
            foreach (var prop in Properties)
            {
                sb.AppendLine("    <logProperty>");
                sb.AppendLine(string.Format("      <propertyName>{0}</propertyName>", prop.Key));
                sb.AppendLine(string.Format("      <propertyValue>{0}</propertyValue>", prop.Value.WrapWithCDATA()));
                sb.AppendLine("    </logProperty>");
            }
            sb.AppendLine("  </properties>");
            sb.AppendLine("</logEntry>");
            return sb.ToString();
        }

        public XElement ToXml()
        {
            return XElement.Parse(ToString(),LoadOptions.PreserveWhitespace);
        }

    }
}