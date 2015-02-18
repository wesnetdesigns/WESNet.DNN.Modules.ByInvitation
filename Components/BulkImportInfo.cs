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
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.IO;

using DotNetNuke;
using DotNetNuke.Entities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Security.Membership;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Tokens;
using DotNetNuke.Services.SystemDateTime;
using Excel;

using System.Data.OleDb;

namespace WESNet.DNN.Modules.ByInvitation
{
    public class BulkImportInfo : IHydratable, IPropertyAccess
    {
        private const int __wizardCompletionStep = 4;
        private readonly string emptyBulkImportLog = "<bulkImportLog>" + Environment.NewLine +
                                                     "  <logEntries>" + Environment.NewLine +
                                                     "  </logEntries>" + Environment.NewLine +
                                                     "</bulkImportLog>";

        private List<ImportField> _ImportFields = null;
        private Dictionary<String, InvitationField> _InvitationFields = null;
        private System.Data.Common.DbConnectionStringBuilder _dbConnectionStringBuilder = new System.Data.Common.DbConnectionStringBuilder();
        private Configuration _MyConfiguration = null;
        private Security _ModuleSecurity = null;

        public Configuration MyConfiguration
        {
            get
            {
                if (_MyConfiguration == null)
                {
                    _MyConfiguration = InvitationController.GetConfiguration(PortalID);
                }
                return _MyConfiguration;
            }
        }

        public Security ModuleSecurity
        {
            get
            {
                if (_ModuleSecurity == null)
                {
                    _ModuleSecurity = new Security(TabID, ModuleID, ImportedByUserID);
                }
                return _ModuleSecurity;
            }
        }

        public int BulkImportID { get; set; }
        public int PortalID { get; set; }
        public int TabID { get; set; }
        public int ModuleID { get; set; }
        public int ImportedByUserID { get; set; }

        public UserInfo ImportedByUser
        {
            get
            {
                return ImportedByUserID == -1 ? new UserInfo() : UserController.GetUserById(PortalID, ImportedByUserID);
            }
        }

        public string ImportedByUsername
        {
            get
            {
                return ImportedByUser.Username;
            }
        }

        public string ImportedByUserFullName
        {
            get
            {
                return ImportedByUser.FirstName + " " + ImportedByUser.LastName;
            }
        }

        public string ImportedByUserDisplayName
        {
            get
            {
                return ImportedByUser.DisplayName;
            }
        }

        public string ImportedByUserIPAddr { get; set; }
        public DateTime ImportedOnDate { get; set; }
        public int WizardStepLastCompleted { get; set; }
        public ImportSourceTypes ImportSourceType { get; set; }
        public string ImportFilename { get; set; }
        public string ConnectionStringKey { get; set; }

        public string DatabaseConnectionString
        {
            get
            {
                return _dbConnectionStringBuilder.ConnectionString;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _dbConnectionStringBuilder.Clear();
                }
                else
                {
                    _dbConnectionStringBuilder.ConnectionString = value;
                }
            }
        }

        public string DatabaseTableName { get; set; }
        public string OrderBy { get; set; }
        public char CSVDelimiter { get; set; }
        public bool FirstRowIsHeader { get; set; }
        public string ItemNodeName { get; set; }

        public List<ImportField> ImportFields
        {
            get
            {
                if (_ImportFields == null)
                {
                    _ImportFields = new List<ImportField>();
                }
                return _ImportFields;
            }
            set
            {
                _ImportFields = value;
            }
        }

        public Dictionary<String, InvitationField> InvitationFields
        {
            get
            {
                if (_InvitationFields == null)
                {
                    _InvitationFields = new Dictionary<String, InvitationField>();
                    for (int i = 0; i < Consts.InvitationFieldNames.Length; i++)
                    {
                        var fieldInfo = new InvitationField { FieldIndex = i, FieldName = Consts.InvitationFieldNames[i], FieldType = Consts.InvitationFieldTypes[i], FieldWidth = Consts.InvitationFieldWidths[i], IsRequired = Consts.InvitationFieldRequired[i] };
                        fieldInfo.LocalizedFieldName = Localization.GetString(fieldInfo.FieldName + ".Header", Configuration.LocalSharedResourceFile);
                        _InvitationFields.Add(fieldInfo.FieldName, fieldInfo);
                    }
                    _InvitationFields["TemporaryPassword"].IsRequired = MyConfiguration.TemporaryPasswordMode == TemporaryPasswordMode.RequiredTemporaryPassword;
                    _InvitationFields["RecipientFirstName"].IsRequired = ProfileController.GetPropertyDefinitionByName(PortalID, UserProfile.USERPROFILE_FirstName).Required;
                    _InvitationFields["RecipientLastName"].IsRequired = ProfileController.GetPropertyDefinitionByName(PortalID, UserProfile.USERPROFILE_LastName).Required;
                    _InvitationFields["AssignedDisplayName"].IsRequired = ModuleSecurity.DisplayNameFormat == "";
                }
                return _InvitationFields;
            }
            set
            {
                _InvitationFields = value;
            }
        }

        public string SenderEmail { get; set; }
        public string FallbackCultureCode { get; set; }
        public string PersonalNote { get; set; }
        public string RedirectOnFirstLogin { get; set; }
        public SendMethods SendMethod { get; set; }
        public int SendBatchSize { get; set; }
        public int SendBatchInteval { get; set; }     // in minutes

        public int BatchNumber { get; set; }
        public int RecordsRead { get; set; }
        public int RecordsSkipped { get; set; }
        public int InvitationsCreated { get; set; }
        public int InvitationsSent { get; set; }
        public int ErrorCount { get; set; }

        public DateTime ScheduledJobStartsAt { get; set; }
        public DateTime ReadStartedOnDate { get; set; }
        public DateTime ReadBatchFinishedOnDate { get; set; }
        public DateTime ReadFinishedOnDate { get; set; }
        public DateTime SendStartedOnDate { get; set; }
        public DateTime SendBatchFinishedOnDate { get; set; }
        public DateTime SendFinishedOnDate { get; set; }
        public DateTime AbortedOnDate { get; set; }
        public DateTime CanceledOnDate { get; set; }
        public DateTime FinishedOnDate { get; set; }

        public XElement BulkImportLog { get; set; }

        public string AttachDBFilename
        {
            get
            {
                return _dbConnectionStringBuilder.GetValue("AttachDBFilename", "");
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _dbConnectionStringBuilder.Remove("AttachDBFilename");
                    DBIntegratedSecurity = false;
                    DBUserInstance = false;
                }
                else
                {
                    _dbConnectionStringBuilder["AttachDBFilename"] = value;
                    DBIntegratedSecurity = true;
                    DBUserInstance = true;
                }
            }
        }

        public string DBDataSource
        {
            get
            {

                return _dbConnectionStringBuilder.GetValueOrDefault("Data Source", "");
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _dbConnectionStringBuilder.Remove("Data Source");
                }
                else
                {
                    _dbConnectionStringBuilder["Data Source"] = value;
                }
            }
        }

        public string DBInitialCatalog
        {
            get
            {
                return _dbConnectionStringBuilder.GetValueOrDefault("Initial Catalog", "");
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _dbConnectionStringBuilder.Remove("Initial Catalog");
                }
                else
                {
                    _dbConnectionStringBuilder["Initial Catalog"] = value;
                }
            }
        }

        public string DBUser
        {
            get
            {
                return _dbConnectionStringBuilder.GetValueOrDefault("User ID", "");
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _dbConnectionStringBuilder.Remove("User ID");
                }
                else
                {
                    _dbConnectionStringBuilder["User ID"] = value;
                    _dbConnectionStringBuilder.Remove("Integrated Security");
                }
            }
        }

        public string DBPassword
        {
            get
            {
                return _dbConnectionStringBuilder.GetValueOrDefault("Password", "");
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _dbConnectionStringBuilder.Remove("Password");
                }
                else
                {
                    _dbConnectionStringBuilder["Password"] = value;
                    _dbConnectionStringBuilder.Remove("Integrated Security");
                }
            }
        }

        public bool DBIntegratedSecurity
        {
            get
            {
                return _dbConnectionStringBuilder.GetValueOrDefault("Integrated Security", false);
            }
            set
            {
                _dbConnectionStringBuilder["Integrated Security"] = value;
                if (value)
                {
                    _dbConnectionStringBuilder.Remove("User ID");
                    _dbConnectionStringBuilder.Remove("Password");
                }
            }
        }

        public bool DBUserInstance
        {
            get
            {
                return _dbConnectionStringBuilder.GetValueOrDefault("User Instance", false);
            }
            set
            {
                _dbConnectionStringBuilder["User Instance"] = value;
            }
        }

        public string DBProvider
        {
            get
            {
                return _dbConnectionStringBuilder.GetValue("Provider", "");
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _dbConnectionStringBuilder.Remove("Provider");
                }
                else
                {
                    _dbConnectionStringBuilder["Provider"] = value;
                }
            }
        }

        public string DBFilename
        {
            get
            {
                return _dbConnectionStringBuilder.GetValueOrDefault("FileName", "");
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _dbConnectionStringBuilder.Remove("FileName");
                }
                else
                {
                    _dbConnectionStringBuilder["FileName"] = value;
                }               
            }
        }

        public bool IsWizardFinished
        {
            get
            {
                return WizardStepLastCompleted >= __wizardCompletionStep;
            }
        }

        public bool IsDoneReading
        {
            get
            {
                return ReadFinishedOnDate != DateTime.MinValue;
            }
        }

        public bool IsDoneSending
        {
            get
            {
                return SendFinishedOnDate != DateTime.MinValue || CanceledOnDate != DateTime.MinValue || AbortedOnDate != DateTime.MinValue;
            }
        }

        public bool IsFinished
        {
            get
            {
                return AbortedOnDate != DateTime.MinValue || CanceledOnDate != DateTime.MinValue || FinishedOnDate != DateTime.MinValue;
            }
        }

        public BulkImportInfo()
        {
            BulkImportID = -1;
            WizardStepLastCompleted = -1;
            ImportFilename = string.Empty;
            BulkImportLog = XElement.Parse(emptyBulkImportLog, LoadOptions.PreserveWhitespace);
        }

        public BulkImportInfo(int portalID, int tabID, int moduleID, int importedByUserID, string importedByUserIPAddr)
            : this()
        {
            PortalID = portalID;
            TabID = tabID;
            ModuleID = moduleID;
            ImportedByUserID = importedByUserID;
            ImportedByUserIPAddr = ImportedByUserIPAddr;
        }

        public void ClearDatabaseConnectionString()
        {
            switch (ImportSourceType)
            {
                case ImportSourceTypes.DatabaseFile:
                    _dbConnectionStringBuilder = new OleDbConnectionStringBuilder();
                    break;
                case ImportSourceTypes.DatabaseConnection:
                    _dbConnectionStringBuilder = new SqlConnectionStringBuilder();
                    break;
                default:
                    _dbConnectionStringBuilder = new System.Data.Common.DbConnectionStringBuilder();
                    break;
            }
            
        }

        public string CreateInvitations(out int recordsRead, out int recordsSkipped, out int invitationsCreated, 
                                        out int notificationsSent, out int errorCount, out bool aborted)
        {
            var msg = new StringBuilder();

            recordsRead = 0;
            recordsSkipped = 0;
            invitationsCreated = 0;
            notificationsSent = 0;
            errorCount = 0;
            aborted = false;

            int fldIndex;
            var importFieldCount = ImportFields.Count;

            InvitationSubmissionStatus status;

            string action = "appendlogentry";

            if (ImportSourceType == ImportSourceTypes.DatabaseConnection || ImportSourceType == ImportSourceTypes.DatabaseFile)
            {
                if (DatabaseConnectionString == string.Empty)
                {
                    msg.AppendLine("No database connection string specified.");
                    errorCount++;
                    aborted = true;
                    goto done;
                }
                if (DatabaseTableName == string.Empty)
                {
                    msg.AppendLine("No database table name specified.");
                    errorCount++;
                    aborted = true;
                    goto done;
                }

                var errorMsg = string.Empty;

                var fieldNames = EscapedString.Combine(ImportFields.Select(fld => fld.ImportFieldName.WrapWithBrackets()), ',');

                string dataQuery;

                if (SendBatchSize == 0)
                {
                    dataQuery = string.Format("select " +
                                                  "{0} " +
                                                  "from {1} " +
                                                  "order by {2}", fieldNames, DatabaseTableName.WrapWithBrackets(), OrderBy);
                }
                else
                {
                    if (ImportSourceType == ImportSourceTypes.DatabaseConnection)
                    {
                        //MS SQL paged query using CTE and ROW_NUMBER
                        dataQuery = string.Format(";with cte as " +
                                                     "(select " +
                                                         "{0}, " +
                                                         "ROW_NUMBER() OVER(order by {1}) AS rn " +
                                                         " from {2}" +
                                                      ") " +
                                                      "select * from cte " +
                                                      "where rn between {3} and {4}",
                                                    fieldNames, OrderBy, DatabaseTableName.WrapWithBrackets(), RecordsRead + 1, RecordsRead + SendBatchSize);
                    }
                    else
                    {
                        //MS Access paged query
                        var subOrderByReversed = ModifyOrderBy("sub", true);
                        var subOrderedOrderBy = ModifyOrderBy("subOrdered", false);
                        
                        dataQuery = string.Format("SELECT * " + 
                                                  "FROM (" +   
                                                         "SELECT TOP {0} *" +
                                                         "FROM (" +
                                                                "SELECT TOP {2} " +
                                                                    "{3} " +
                                                                "FROM {4} " +
                                                                "ORDER BY {5}" +
                                                              ") sub " +
                                                         "ORDER BY sub.{6} DESC" +
                                                       ") subOrdered " +
                                                 "ORDER BY subOrdered.{7}", 
                                                 SendBatchSize, RecordsRead + SendBatchSize, fieldNames, DatabaseTableName.WrapWithBrackets(), OrderBy, subOrderByReversed, subOrderedOrderBy);

                    }
                }

                System.Data.Common.DbConnection connection = null;
                System.Data.Common.DbCommand command = null;
                System.Data.IDataReader dr = null;

                if (ImportSourceType == ImportSourceTypes.DatabaseConnection)
                {
                    connection = new SqlConnection(DatabaseConnectionString);
                    command = new SqlCommand(dataQuery, (SqlConnection)connection);
                }
                else
                {
                    connection = new OleDbConnection(DatabaseConnectionString);
                    command = new OleDbCommand(dataQuery, (OleDbConnection)connection);
                }

                
                try
                {
                    connection.Open();
                    dr = command.ExecuteReader(System.Data.CommandBehavior.CloseConnection);

                    if (RecordsRead == 0) InvitationController.UpdateBulkImportStatus(this, "readstarted", 0, 0, 0, 0, errorCount, new LogEntry(BatchNumber).ToXml());

                    string[] values = new string[importFieldCount];
                    while (dr.Read())
                    {
                        recordsRead++;
                        for (fldIndex = 0; fldIndex < importFieldCount; fldIndex++)
                        {
                            var value = dr[ImportFields[fldIndex].ImportFieldName];
                            values[fldIndex] = value is DBNull ? string.Empty : value.ToString();
                        }
                        msg.Append(ProcessImportedInvitationData(values, ref recordsRead, ref recordsSkipped, ref invitationsCreated, ref notificationsSent, ref errorCount));
                    }
                }
                catch (Exception exc)
                {
                    msg.AppendLine("Error reading imported invitations from database table - Error: " + exc.Message);
                    errorCount++;
                    aborted = true;
                    goto done;
                }
                finally
                {
                    if (dr != null) dr.Dispose();
                    if (command != null) command.Dispose();
                }
            }
            else
            {
                if (string.IsNullOrEmpty(ImportFilename))
                {
                    msg.AppendLine(LocalizeSharedResourceString("NoImportFileSpecified"));
                    errorCount++;
                    aborted = true;
                    goto done;
                }
                var importFilePhysicalPath = Utilities.GetImportFilePhysicalPath(ImportFilename);
                if (importFilePhysicalPath == string.Empty)
                {
                    msg.AppendLine(string.Format(LocalizeSharedResourceString("ImportFileNotFound"), ImportFilename));
                    errorCount++;
                    aborted = true;
                    goto done;
                }

                if (RecordsRead == 0) InvitationController.UpdateBulkImportStatus(this, "readstarted", 0, 0, 0, 0, errorCount, new LogEntry(BatchNumber).ToXml());

                using (var fs = new FileStream(importFilePhysicalPath, FileMode.Open))
                {
                    if (fs == null || fs.Length == 0)
                    {
                        msg.AppendLine(string.Format(LocalizeSharedResourceString("ImportFileOpenFailure"), importFilePhysicalPath));
                        errorCount++;
                        aborted = true;
                        goto done;
                    }

                    switch (ImportSourceType)
                    {
                        case ImportSourceTypes.CSV:

                            using (var reader = new System.IO.StreamReader(fs, Encoding.UTF8))
                            {
                                string line;

                                do  // skip empty lines at beginning
                                {
                                    line = reader.ReadLine();
                                } while (!reader.EndOfStream && line.Length == 0);

                                if (FirstRowIsHeader)
                                {
                                    if (reader.EndOfStream)
                                    {
                                        msg.AppendLine(LocalizeSharedResourceString("MissingCSVHeader"));
                                        errorCount++;
                                        aborted = true;
                                        goto done;
                                    }
                                    else
                                    {
                                        line = reader.ReadLine(); // skip the header row which was processed when setting up the import
                                    }
                                }

                                // skip records already read in previous batches
                                if (RecordsRead != 0)
                                {
                                    var recordSkipCount = 0;
                                    while (recordSkipCount < RecordsRead && line != null)
                                    {
                                        if (line.Length > 0) recordSkipCount++;
                                        line = reader.ReadLine();
                                    }
                                    if (line == null)
                                    {
                                        action = "readfinished";
                                        goto done;
                                    }
                                }

                                do
                                {
                                    recordsRead++;
                                    string[] values = EscapedString.Seperate(line, CSVDelimiter).ToArray();
                                    if (values.Length == importFieldCount)
                                    {
                                        msg.Append(ProcessImportedInvitationData(values, ref recordsRead, ref recordsSkipped, ref invitationsCreated, ref notificationsSent, ref errorCount));
                                    }
                                    else
                                    {
                                        ProcessValueCountError(values, msg, recordsRead, ref recordsSkipped, ref errorCount);
                                    }

                                } while (((line = reader.ReadLine()) != null) && (SendBatchSize == 0 || recordsRead < SendBatchSize));

                            }

                            break; //case CSV

                        case ImportSourceTypes.Excel:
                            IExcelDataReader excelReader = null;
                            var extension = Path.GetExtension(ImportFilename);
                            if (extension == ".xls")
                            {
                                excelReader = Excel.ExcelReaderFactory.CreateBinaryReader(fs);
                            }
                            else if (extension == ".xlsx")
                            {
                                excelReader = Excel.ExcelReaderFactory.CreateOpenXmlReader(fs);
                            }
                            else
                            {
                                break;
                            }

                            // skip over any header row
                            if (FirstRowIsHeader) excelReader.Read();

                            // skip over records previously read
                            if (RecordsRead != 0)
                            {
                                var recordSkipCount = 0;
                                while (recordSkipCount < RecordsRead && excelReader.Read())
                                {
                                    recordSkipCount++;
                                }
                            }

                            while ((SendBatchSize == 0 || recordsRead < SendBatchSize) && excelReader.Read())
                            {
                                recordsRead++;
                                string[] values;
                                string value;

                                if (importFieldCount == excelReader.FieldCount)
                                {
                                    values = new string[importFieldCount];
                                    for (fldIndex = 0; fldIndex < importFieldCount; fldIndex++)
                                    {
                                        value = excelReader.GetString(fldIndex);
                                        values[fldIndex] = value == null ? string.Empty : value;
                                    }
                                    msg.Append(ProcessImportedInvitationData(values, ref recordsRead, ref recordsSkipped, ref invitationsCreated, ref notificationsSent, ref errorCount));
                                }
                                else
                                {
                                    values = new string[excelReader.FieldCount];
                                    for (fldIndex = 0; fldIndex < excelReader.FieldCount; fldIndex++)
                                    {
                                        value = excelReader.GetString(fldIndex);
                                        values[fldIndex] = value == null ? string.Empty : value;
                                    }
                                    ProcessValueCountError(values, msg, recordsRead, ref recordsSkipped, ref errorCount);
                                }
                            }

                            break; // case Excel

                        case ImportSourceTypes.XML:
                            var xDoc = XDocument.Load(fs);
                            var rootNode = xDoc.Root;
                            if (rootNode != null)
                            {
                                var rootNodeName = rootNode.Name.LocalName;
                                var items = rootNode.Elements(ItemNodeName).Skip(RecordsRead);
                                if (SendBatchSize > 0)
                                {
                                    items = items.Take(SendBatchSize);
                                }

                                foreach (XElement item in items)
                                {
                                    recordsRead++;
                                    var kvps = item.Elements().Select(el => new KeyValuePair<string, string>(el.Name.LocalName, el.Value)).OrderBy(k => k.Key);
                                    string[] values;

                                    if (kvps.Count() == importFieldCount)
                                    {
                                        values = kvps.Select(k => k.Value).ToArray();
                                    }
                                    else
                                    {
                                        values = new string[importFieldCount];
                                        var kvpDictionary = kvps.ToDictionary(k => k.Key);
                                        foreach (ImportField importField in ImportFields)
                                        {
                                            values[importField.ImportFieldIndex] = kvpDictionary[importField.ImportFieldName].Value;
                                        }
                                    }
                                    msg.Append(ProcessImportedInvitationData(values, ref recordsRead, ref recordsSkipped, ref invitationsCreated, ref notificationsSent, ref errorCount));
                                }
                            }
                            break; // case XML
                    } //switch
                } //using fs
            }

            action = ((SendBatchSize == 0 || recordsRead < SendBatchSize) ? "readfinished" : "readbatchfinished");

        done:
            msg.AppendLine(LocalizeSharedResourceString("BatchCompletionSummary"));
            msg.AppendLine(string.Format(LocalizeSharedResourceString("BatchNumber"), BatchNumber));
            msg.AppendLine(string.Format(LocalizeSharedResourceString("RecordsRead"), recordsRead));
            msg.AppendLine(string.Format(LocalizeSharedResourceString("RecordsSkipped"), recordsSkipped));
            msg.AppendLine(string.Format(LocalizeSharedResourceString("InvitationsCreated"), invitationsCreated));
            msg.AppendLine(string.Format(LocalizeSharedResourceString("NotificationsSent"), notificationsSent));
            msg.AppendLine(string.Format(LocalizeSharedResourceString("ErrorCount"), errorCount));
            msg.AppendLine(string.Format(LocalizeSharedResourceString("BatchAborted"), aborted));
            msg.AppendLine(LocalizeSharedResourceString("OverallCompletionSummary"));
            msg.AppendLine(string.Format(LocalizeSharedResourceString("RecordsRead"), RecordsRead + recordsRead));
            msg.AppendLine(string.Format(LocalizeSharedResourceString("RecordsSkipped"), RecordsSkipped + recordsSkipped));
            msg.AppendLine(string.Format(LocalizeSharedResourceString("InvitationsCreated"), InvitationsCreated + invitationsCreated));
            msg.AppendLine(string.Format(LocalizeSharedResourceString("ErrorCount"), ErrorCount + errorCount));

            var logEntry = new LogEntry(msg.ToString(), BatchNumber);
            action = aborted ? "aborted" : action;

            InvitationController.UpdateBulkImportStatus(this, action, recordsRead, recordsSkipped, invitationsCreated, 0, errorCount, logEntry.ToXml());
            return msg.ToString();
        }

        private  string ModifyOrderBy (string aliasToPrepend, bool reverseDirection)
        {
            var parts = OrderBy.Split(',');
            var sb = new StringBuilder();
            foreach (var part in parts)
            {
                if (!string.IsNullOrEmpty(aliasToPrepend)) sb.Append(aliasToPrepend + ".");
                if (reverseDirection)
                {
                    var parts2 = part.Split(' ');
                    if (parts2.Length == 1 || parts2.Length == 2 && parts2[1].Trim() == "ASC")
                    {
                        sb.Append(parts2[0] + " DESC");
                    }
                    else if (parts2[1].Trim() == "DESC")
                    {
                        sb.Append(parts2[0]);
                    }
                }
                else
                {
                    sb.Append(parts[0]);
                }
                sb.Append(", ");
            }
            sb.Length -= 2;
            return sb.ToString();
        }

        private string ProcessImportedInvitationData(string[] values, ref int recordsRead, ref int recordsSkipped, ref int invitationsCreated, ref int notificationsSent, ref int errorCount)
        {
            InvitationSubmissionStatus status;
            var msg = new StringBuilder();
            var createdInvitation = CreateInvitation(values, out status);

            if (status != InvitationSubmissionStatus.Success)
            {
                errorCount++;
                msg.AppendLine(LocalizeSharedResourceString("InvitationCreationError"));
                msg.AppendLine(string.Format(LocalizeSharedResourceString("RecordNumber"), RecordsRead + recordsRead));
                msg.AppendLine(string.Format(LocalizeSharedResourceString("RecordData"), EscapedString.Combine(values, ',')));
                msg.AppendFormat(LocalizeSharedResourceString("Error"), Utilities.LocalizeInvitationSubmissionStatus(status));
                if (status.ToString().Contains("Role"))
                {
                    msg.Append(" - " + GetImportedValue("AssignedRoles", values, ""));
                    status = InvitationSubmissionStatus.Success;
                }
                else
                {
                    recordsSkipped++;
                }
                notificationsSent += NotificationsHelper.SendNotifications(createdInvitation, Notifications.Errored, msg.ToString());
            }

            if (status == InvitationSubmissionStatus.Success && createdInvitation != null)
            {
                invitationsCreated++;
                notificationsSent += NotificationsHelper.SendNotifications(createdInvitation, Notifications.Created);
                msg.AppendFormat(LocalizeSharedResourceString("InvitationCreated"), createdInvitation.RecipientEmail);
            }
            msg.AppendLine();
            return msg.ToString();
        }

        private void ProcessValueCountError(string[] values, StringBuilder msg, int recordsRead, ref int recordsSkipped, ref int errorCount)
        {
            msg.AppendLine(string.Format(LocalizeSharedResourceString("ValuesCountError"), values.Length, ImportFields.Count));
            msg.AppendLine(string.Format(LocalizeSharedResourceString("RecordNumber"), RecordsRead + recordsRead));
            msg.AppendLine(string.Format(LocalizeSharedResourceString("RecordData"), EscapedString.Combine(values, ',')));
            msg.AppendLine();
            errorCount++;
            recordsSkipped++;
        }

        private InvitationSubmissionStatus LoadData(InvitationInfo invitation, string[] values)
        {
            var dataBaseDateTime = DateUtils.GetDatabaseTime();
            var status = InvitationSubmissionStatus.Undefined;

            invitation.BulkImportID = BulkImportID;
            invitation.InvitedOnDate = ImportedOnDate;
            invitation.ApprovedOnDate = ImportedOnDate;
            invitation.ApprovedByUserID = ImportedByUserID;
            invitation.InvitedByUserID = ImportedByUserID;
            invitation.InvitedByUserIPAddr = ImportedByUserIPAddr;
            invitation.InvitedByUserFullName = ImportedByUserFullName;
            invitation.InvitedByUserEmail = SenderEmail;
            try
            {
                invitation.RecipientEmail = GetImportedValue("RecipientEmail", values, string.Empty);
                invitation.RecipientFirstName = GetImportedValue("RecipientFirstName", values, string.Empty);
                invitation.RecipientLastName = GetImportedValue("RecipientLastName", values, string.Empty);
                invitation.RecipientCultureCode = GetImportedValue("RecipientCultureCode", values, FallbackCultureCode);
                invitation.PersonalNote = GetImportedValue("PersonalNote", values, PersonalNote);
                invitation.AssignedUsername = GetImportedValue("AssignedUsername", values, string.Empty);
                invitation.AssignedDisplayName = GetImportedValue("AssignedDisplayName", values, string.Empty);
                invitation.TemporaryPassword = GetImportedValue("TemporaryPassword", values, string.Empty);
                var redirectTabName = GetImportedValue("RedirectOnFirstLogin", values, RedirectOnFirstLogin);
                if (!string.IsNullOrEmpty(redirectTabName))
                {
                    var redirectTabInfo = new TabController().GetTabByName(redirectTabName, PortalID);
                    if (redirectTabInfo == null)
                    {
                        status = InvitationSubmissionStatus.InvalidRedirectOnLogin;
                        invitation.RedirectOnFirstLogin = -1;
                    }
                    else
                    {
                        invitation.RedirectOnFirstLogin = redirectTabInfo.TabID;
                    }
                }

                return Utilities.CleanAndVerifyData(invitation, false);

            }
            catch (MissingFieldException exc)
            {
                return InvitationSubmissionStatus.RequiredFieldDataMissing;
            }
        }

        public InvitationInfo CreateInvitation(string[] values, out InvitationSubmissionStatus status)
        {
            var currentInvitation = new InvitationInfo(PortalID, ModuleID, TabID);
            status = LoadData(currentInvitation, values);

            if (status == InvitationSubmissionStatus.Success)
            {
                var currentInvitationID = InvitationController.UpdateInvitation(currentInvitation, ImportedByUserID);

                if (currentInvitationID > 0)
                {
                    var assignedRoles = EscapedString.Seperate(GetImportedValue("AssignedRoles", values, ""), ';');

                    // assigned roles will be stored in csv field as semi-colon separated string of RoleNames (not RoleIDs). With CSV import it will be assumed that role is
                    // effective on date invitation is accepted. Optionally the RoleName may be followed with pipe character and the expiration date in the form of yyyy-MM-dd hh:mm:ss
                    // with the expiration date time zone assumed to be same as the portal time zone.

                    if (assignedRoles.Count() > 0 && ModuleSecurity.AssignableRoles.Count > 0)
                    {

                        foreach (var ar in assignedRoles)
                        {
                            var roleValidityStatus = RoleValidityStatus.ValidRole;
                            var parts = EscapedString.Seperate(ar, '|').ToArray();
                            var effectiveDate = Utilities.NullDateTime;
                            var expiryDate = Utilities.NullDateTime;
                            var loweredRoleName = parts[0].Trim().ToLower();

                            var roleInfo = ModuleSecurity.AssignableRoles.Where(ri => ri.RoleName.ToLower() == loweredRoleName).SingleOrDefault();

                            if (roleInfo == null)
                            {
                                roleValidityStatus = RoleValidityStatus.InvalidRoleName;
                            }
                            else
                            {
                                if (parts.Length == 2)
                                {
                                    if (!Utilities.NullableDateTimeTryParse(parts[1], out expiryDate))
                                    {
                                        roleValidityStatus = RoleValidityStatus.InvalidRoleExpiryDate;
                                    }
                                }
                                else if (parts.Length == 3)
                                {
                                    if (!Utilities.NullableDateTimeTryParse(parts[1], out effectiveDate))
                                    {
                                        roleValidityStatus = RoleValidityStatus.InvalidRoleEffectiveDate;
                                    }

                                    if (!Utilities.NullableDateTimeTryParse(parts[2], out expiryDate))
                                    {
                                        roleValidityStatus |= RoleValidityStatus.InvalidRoleExpiryDate;
                                    }
                                }
                            }

                            if (roleValidityStatus == RoleValidityStatus.ValidRole)
                            {
                                currentInvitation.AssignedRoles.Add(new AssignableRoleInfo(roleInfo.RoleID, roleInfo.RoleName, roleInfo.Description) { EffectiveDate = effectiveDate, ExpiryDate = expiryDate });
                            }
                            else
                            {
                                status = InvitationSubmissionStatus.InvalidRole;
                            }
                        }

                        foreach (var ar in currentInvitation.AssignedRoles)
                        {
                            ar.InvitationID = currentInvitationID;
                            InvitationController.UpdateAssignableRole(ar, ImportedByUserID);
                        }

                        status = status == InvitationSubmissionStatus.Undefined ? InvitationSubmissionStatus.Success : status;
                    }
                }
                else
                {
                    status = InvitationSubmissionStatus.FailureSavingInvitation;
                    currentInvitation = null;
                }
            }

            return currentInvitation;
        }


        public T GetImportedValue<T>(string invitationFieldName, string[] values, T defaultValue)
        {
            if (!InvitationFields.ContainsKey(invitationFieldName))
            {
                throw new ArgumentException(string.Format("Field named '{0}' is not a valid invitation field.", invitationFieldName), "invitationFieldName");
            }

            InvitationField fieldInfo = InvitationFields[invitationFieldName];
            var idx = fieldInfo.MappedImportFieldIndex;
            if (idx >= 0 && idx < values.Length)
            {
                var value = values.ElementAt(idx).Trim();
                if (string.IsNullOrEmpty(value))
                {
                    if (fieldInfo.IsRequired)
                    {
                        throw new MissingFieldException("InvitationField", fieldInfo.FieldName);
                    }
                    else return defaultValue;
                }
                else
                {
                    try
                    {
                        return (T)Convert.ChangeType(value, typeof(T));
                    }
                    catch (InvalidCastException)
                    {
                        return defaultValue;
                    }
                }

            }
            return defaultValue;
        }

        public string SendInvitations(out int invitationsSent, out int notificationsSent, out int errorCount)
        {
            var messageSB = new StringBuilder();

            string action = "appendlogentry";

            invitationsSent = 0;
            notificationsSent = 0;
            errorCount = 0;
            var totalInvitationsToSend = 0;

            if (!IsDoneSending)
            {
                var invitationsToSend = InvitationController.GetBulkImportedInvitationsNotSent(BulkImportID, SendBatchSize, ref totalInvitationsToSend);

                if (invitationsToSend.Count() > 0)
                {
                    if (InvitationsSent == 0) InvitationController.UpdateBulkImportStatus(this, "sendstarted", 0, 0, 0, 0, 0, new LogEntry(BatchNumber).ToXml());

                    foreach (var invitation in invitationsToSend)
                    {
                        var mailer = new MailManager(invitation);
                        var errMsg = mailer.SendBulkMail("send");
                        if (string.IsNullOrEmpty(errMsg))
                        {
                            messageSB.AppendLine(string.Format("Invitation sent to {0} at {1}", invitation.RecipientFullName, invitation.RecipientEmail));
                            invitationsSent++;
                            var currentInvitation = InvitationController.UpdateInvitationStatus(invitation.InvitationID, "sent", invitation.InvitedByUserID);
                            notificationsSent += NotificationsHelper.SendNotifications(currentInvitation, Notifications.Sent);
                        }
                        else
                        {
                            messageSB.AppendLine("Send failure: " + errMsg);
                            errorCount++;
                            notificationsSent += NotificationsHelper.SendNotifications(invitation, Notifications.Errored, errMsg);
                        }

                    }
                    action = (IsDoneReading && (invitationsToSend.Count() == totalInvitationsToSend)) ? "sendfinished" : "sendbatchfinished";
                    var logEntry = new LogEntry(messageSB.ToString(), BatchNumber);
                    InvitationController.UpdateBulkImportStatus(this, action, 0, 0, 0, invitationsSent, errorCount, logEntry.ToXml());
                }

                if (IsDoneReading && (totalInvitationsToSend == 0 || action == "sendfinished"))
                {
                    InvitationController.UpdateBulkImportStatus(this, "finished", 0, 0, 0, 0, 0, new LogEntry("Bulk Import Completed", BatchNumber).ToXml());
                    var currentBulkImport = InvitationController.GetBulkImport(BulkImportID);
                    notificationsSent += NotificationsHelper.SendNotifications(currentBulkImport, Notifications.Finished);
                }

            }

            return messageSB.ToString();
        }

        private LocalizedConfiguration GetLocalizedConfiguration(string recipientCultureCode)
        {
            return MyConfiguration.GetLocalizedConfiguration(recipientCultureCode, FallbackCultureCode);
        }

        private string LocalizeSharedResourceString(string resourceKey)
        {
            return Localization.GetString(resourceKey, Configuration.LocalSharedResourceFile);
        }

        public void Fill(System.Data.IDataReader dr)
        {
            BulkImportID = Null.SetNullInteger(dr["BulkImportID"]);
            PortalID = Null.SetNullInteger(dr["PortalID"]);
            TabID = Null.SetNullInteger(dr["TabID"]);
            ModuleID = Null.SetNullInteger(dr["ModuleID"]);
            ImportedByUserID = Null.SetNullInteger(dr["ImportedByUserID"]);
            ImportedByUserIPAddr = Null.SetNullString(dr["ImportedByUserIPAddr"]);
            ImportedOnDate = Null.SetNullDateTime(dr["ImportedOnDate"]);
            WizardStepLastCompleted = Null.SetNullInteger(dr["WizardStepLastCompleted"]);
            ImportSourceType = (ImportSourceTypes)(Convert.ToInt32(dr["ImportSourceType"]));
            ImportFilename = Null.SetNullString(dr["ImportFilename"]);
            ConnectionStringKey = Null.SetNullString(dr["ConnectionStringKey"]);

            ClearDatabaseConnectionString();
            DatabaseConnectionString = Null.SetNullString(dr["DatabaseConnectionString"]);

            DatabaseTableName = Null.SetNullString(dr["DatabaseTableName"]);
            OrderBy = Null.SetNullString(dr["OrderBy"]);
            CSVDelimiter = Null.SetNullString(dr["CSVDelimiter"])[0];
            FirstRowIsHeader = Null.SetNullBoolean(dr["FirstRowIsHeader"]);
            ItemNodeName = Null.SetNullString(dr["ItemNodeName"]);
            InvitationFields = Json.Deserialize<Dictionary<string, InvitationField>>(Null.SetNullString(dr["InvitationFields"]));
            ImportFields = Json.Deserialize<List<ImportField>>(Null.SetNullString(dr["ImportFields"]));
            SenderEmail = Null.SetNullString(dr["SenderEmail"]);
            FallbackCultureCode = Null.SetNullString(dr["FallbackCultureCode"]);
            PersonalNote = Null.SetNullString(dr["PersonalNote"]);
            RedirectOnFirstLogin = Null.SetNullString(dr["RedirectOnFirstLogin"]);
            SendMethod = (SendMethods)(Convert.ToInt32(dr["SendMethod"]));
            SendBatchSize = Null.SetNullInteger(dr["SendBatchSize"]);
            SendBatchInteval = Null.SetNullInteger(dr["SendBatchInteval"]);
            RecordsRead = Utilities.SetNullAsZeroInteger(dr["RecordsRead"]);
            RecordsSkipped = Utilities.SetNullAsZeroInteger(dr["RecordsSkipped"]);
            InvitationsCreated = Utilities.SetNullAsZeroInteger(dr["InvitationsCreated"]);
            InvitationsSent = Utilities.SetNullAsZeroInteger(dr["InvitationsSent"]);
            BatchNumber = Utilities.SetNullAsZeroInteger(dr["BatchNumber"]);
            ScheduledJobStartsAt = Null.SetNullDateTime(dr["ScheduledJobStartsAt"]);
            ReadStartedOnDate = Null.SetNullDateTime(dr["ReadStartedOnDate"]);
            ReadBatchFinishedOnDate = Null.SetNullDateTime(dr["ReadBatchFinishedOnDate"]);
            ReadFinishedOnDate = Null.SetNullDateTime(dr["ReadFinishedOnDate"]);
            SendStartedOnDate = Null.SetNullDateTime(dr["SendStartedOnDate"]);
            SendBatchFinishedOnDate = Null.SetNullDateTime(dr["SendBatchFinishedOnDate"]);
            SendFinishedOnDate = Null.SetNullDateTime(dr["SendFinishedOnDate"]);
            AbortedOnDate = Null.SetNullDateTime(dr["AbortedOnDate"]);
            CanceledOnDate = Null.SetNullDateTime(dr["CanceledOnDate"]);
            FinishedOnDate = Null.SetNullDateTime(dr["FinishedOnDate"]);
            ErrorCount = Utilities.SetNullAsZeroInteger(dr["ErrorCount"]);
            BulkImportLog = SetNullXElement(dr["BulkImportLog"]);
        }

        private XElement SetNullXElement(object obj)
        {
            if (DBNull.Value == obj)
            {
                return XElement.Parse(emptyBulkImportLog, LoadOptions.PreserveWhitespace);
            }
            else
            {
                return XElement.Parse((string)obj);
            }
        }

        public int KeyID
        {
            get
            {
                return BulkImportID;
            }
            set
            {
                BulkImportID = value;
            }
        }

        public CacheLevel Cacheability
        {
            get { return CacheLevel.notCacheable; }
        }

        public string GetProperty(string propertyName, string format, System.Globalization.CultureInfo formatProvider, UserInfo accessingUser, Scope accessLevel, ref bool propertyNotFound)
        {
            if (string.IsNullOrEmpty(format)) format = "g";
            if (formatProvider == null) formatProvider = System.Threading.Thread.CurrentThread.CurrentCulture;

            propertyNotFound = false;
            string result = string.Empty;

            switch (propertyName.ToLowerInvariant())
            {
                case "portalid":
                    result = PortalID.ToString(format, formatProvider);
                    break;
                case "portalname":
                    result = PortalController.Instance.GetPortal(PortalID, formatProvider.Name).PortalName;
                    break;
                case "portallogosrc":
                    result = PortalController.Instance.GetPortal(PortalID, formatProvider.Name).LogoFile;
                    break;
                case "importedbyuserid":
                    result = ImportedByUserID.ToString(format, formatProvider);
                    break;
                case "importedbyusername":
                    result = ImportedByUsername;
                    break;
                case "importedbyuserdisplayname":
                    result = ImportedByUserDisplayName;
                    break;
                case "importedondate":
                    result = Utilities.FormattedUTCDate(ImportedOnDate, accessingUser, format, formatProvider);
                    break;
                case "readstartedondate":
                    result = Utilities.FormattedUTCDate(ReadStartedOnDate, accessingUser, format, formatProvider);
                    break;
                case "readfinishedondate":
                    result = Utilities.FormattedUTCDate(ReadFinishedOnDate, accessingUser, format, formatProvider);
                    break;
                case "sendstartedondate":
                    result = Utilities.FormattedUTCDate(SendStartedOnDate, accessingUser, format, formatProvider);
                    break;
                case "sendfinishedondate":
                    result = Utilities.FormattedUTCDate(SendFinishedOnDate, accessingUser, format, formatProvider);
                    break;
                case "abortedondate":
                    result = Utilities.FormattedUTCDate(AbortedOnDate, accessingUser, format, formatProvider);
                    break;
                case "canceledondate":
                    result = Utilities.FormattedUTCDate(CanceledOnDate, accessingUser, format, formatProvider);
                    break;
                case "finishedondate":
                    result = Utilities.FormattedUTCDate(FinishedOnDate, accessingUser, format, formatProvider);
                    break;
                case "recordsread":
                    result = RecordsRead.ToString(format, formatProvider);
                    break;
                case "recordsskipped":
                    result = RecordsSkipped.ToString(format, formatProvider);
                    break;
                case "invitationscreated":
                    result = InvitationsCreated.ToString(format, formatProvider);
                    break;
                case "invitationssent":
                    result = InvitationsSent.ToString(format, formatProvider);
                    break;
                case "errorcount":
                    result = ErrorCount.ToString(format, formatProvider);
                    break;
                case "bulkimportlog":
                    result = BulkImportLog.ToString().Replace(Environment.NewLine, "<br />");
                    break;                
            }

            return result;

        }
    }
}