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
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Web;
using DotNetNuke;
using DotNetNuke.Services.Tokens;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Instrumentation;
using WESNet.Utilities.ExpressionEvaluator;
using System.Dynamic;

namespace WESNet.DNN.Modules.ByInvitation
{
    public class Tokenizer : TokenReplace
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(Tokenizer));

        private static Regex conditionalRegex = new Regex(@"((?:\{if\s+\()(?<conditionclause>.+?)(?:\)s*\})(?<trueclause>.+?)((?:\{else\})(?<falseclause>.+?))?\{endif\})|(?<text>[^\}\{]+)",
                                                          RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);

        public bool EnableConditionalReplacement { get; set; }

        public Tokenizer(InvitationInfo invitation)
            : base()
        {          
                   
            if (invitation.PortalId != -1)
            {               
                base.PortalSettings = new PortalSettings(invitation.TabId, invitation.PortalId);
            }

            if (invitation.ModuleId != -1)
            {
                base.ModuleInfo = ModuleController.Instance.GetModule(invitation.ModuleId, invitation.TabId, false);
            }
            PropertySource["invitation"] = invitation;
            PropertySource["configuration"] = invitation.MyConfiguration;
            EnableConditionalReplacement = true;
        }

        public Tokenizer(BulkImportInfo bulkImportInfo)
            : base()
        {
            if (bulkImportInfo.PortalID != -1)
            {
                base.PortalSettings = new PortalSettings(bulkImportInfo.TabID, bulkImportInfo.PortalID);
            }

            if (bulkImportInfo.ModuleID != -1)
            {
                base.ModuleInfo = ModuleController.Instance.GetModule(bulkImportInfo.ModuleID, bulkImportInfo.TabID, false);
            }
            PropertySource["bulkimport"] = bulkImportInfo;
            PropertySource["configuration"] = bulkImportInfo.MyConfiguration;
            EnableConditionalReplacement = true;
        }

        public virtual string ReplaceInvitationTokens(string sourceText)
        {
            string result = string.Empty;
            result = base.ReplaceTokens(sourceText);

            if (EnableConditionalReplacement)
            {
               result = ReplaceConditionals(result);
            }

            return result;
        }

        protected virtual string ReplaceConditionals(string sourceText)
        {
            if (string.IsNullOrEmpty(sourceText))
            {
                return string.Empty;
            }

            var result = new StringBuilder();

            try
            {
                foreach (Match currentMatch in conditionalRegex.Matches(sourceText))
                {
                    string conditionClause = currentMatch.Result("${conditionclause}");
                    if (!String.IsNullOrEmpty(conditionClause))
                    {
                        var evaluator = new Evaluator();
                        var condition = false;

                        var evalResult = evaluator.Evaluate(conditionClause);
                        if (evalResult != null && evalResult.OperandTypeName == "System.Boolean")
                        {
                            condition = evalResult.Value;
                        }

                        string trueClause = currentMatch.Result("${trueclause}");
                        string falseClause = currentMatch.Result("${falseclause}");
                        result.Append(condition ? trueClause : falseClause);
                    }
                    else
                    {
                        string text = currentMatch.Result("${text}");
                        result.Append(text);
                    }
                }
            }
            catch (Exception exc)
            {
                Logger.Error("Unable to evaluate conditional templating expression", exc);
            }
            return result.ToString();
        }
    }
}