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

using DotNetNuke.Common;
using DnnUtils = DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins.Controls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Xml.Linq;
using DotNetNuke.Data;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using Excel;

namespace WESNet.DNN.Modules.ByInvitation
{
    public partial class BulkImportInvitations : ByInvitationModuleBase
    {

        const int maxPreviewRows = 3;

        private static char[] delimiters = new [] { ',', ';', '\t', ' ', '|' };

        private BulkImportInfo _BulkImport = null;

        public int BulkImportID
        {
            get
            {
                return ViewState["BulkImportID"] == null ? -1 : (int)ViewState["BulkImportID"];
            }
            set
            {
                ViewState["BulkImportID"] = value;
            }
        }


        public BulkImportInfo BulkImport
        {
            get
            {
                if (_BulkImport == null)
                {
                    if (BulkImportID == -1)
                    {
                        _BulkImport = new BulkImportInfo(PortalId, TabId, ModuleId, UserId, Security.GetUserIPAddress(false));
                        _BulkImport.SenderEmail = UserInfo.IsInRole("Administrators") ? UserInfo.Email : PortalSettings.Email;
                        _BulkImport.FallbackCultureCode = Thread.CurrentThread.CurrentUICulture.Name;
                    }
                    else
                    {
                        _BulkImport = InvitationController.GetBulkImport(BulkImportID);
                        if (_BulkImport == null)
                        {
                            throw new NullReferenceException("Unable to reload BulkImport (" + BulkImportID.ToString() + ") setup data.");
                        }
                    }
                }
                return _BulkImport;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            JavaScript.RequestRegistration(CommonJs.DnnPlugins);

            cmdResume.Click += cmdResume_Click;
            cmdStartOver.Click += cmdStartOver_Click;
            cmdCancel.Click += cmdCancel_Click;

            rblCSVDelimiter.SelectedIndexChanged += new EventHandler(rblCSVDelimiter_SelectedIndexChanged);
            cbCSVFirstRow.CheckedChanged += new EventHandler(cbFirstRow_CheckedChanged);
            cbExcelFirstRow.CheckedChanged += new EventHandler(cbFirstRow_CheckedChanged);
            ddlItemNodeName.SelectedIndexChanged += new EventHandler(ddlItemNodeName_SelectedIndexChanged);
            ddlConnectionStringKey.SelectedIndexChanged += new EventHandler(ddlConnectionStringKey_SelectedIndexChanged);
            ddlDatabaseTableName.SelectedIndexChanged += new EventHandler(ddlDatabaseTableName_SelectedIndexChanged);

            grdPreviewData.RowDataBound +=  new GridViewRowEventHandler(grdPreviewData_RowDataBound);

            wizBulkImport.ActiveStepChanged += new EventHandler(wizBulkImport_ActiveStepChanged);
            wizBulkImport.CancelButtonClick += new EventHandler(wizBulkImport_CancelButtonClick);
            wizBulkImport.NextButtonClick += new WizardNavigationEventHandler(wizBulkImport_NextButtonClick);
            wizBulkImport.FinishButtonClick += new WizardNavigationEventHandler(wizBulkImport_FinishButtonClick);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!ModuleSecurity.CanBulkImportInvitations)
            {
                Response.Redirect(Globals.AccessDeniedURL(), true);
            }

            ctlCultureCode.PortalId = PortalId;

            valSenderEmail.ValidationExpression = MyLocalizedConfiguration.EmailRegex;

            if (!Page.IsPostBack)
            {
                var resumableBulkImport = InvitationController.GetBulkImports(PortalId, UserId, true).FirstOrDefault(bi => !bi.IsWizardFinished);
                if (resumableBulkImport != null && resumableBulkImport.BulkImportID != -1)
                {
                    divResumeWizard.Visible = true;
                    divStep0Form.Visible = false;
                    BulkImportID = resumableBulkImport.BulkImportID;
                }
                else
                {
                    BulkImportID = -1;
                    divResumeWizard.Visible = false;
                    divStep0Form.Visible = true;
                    SetDefaultsForImportSourceType(ImportSourceTypes.CSV);
                    BindFormFields();
                } 
            }
        }
 
        protected string GetActiveStepText(string type)
        {
            string text = string.Empty;
            if (type == "Title")
            {
                text = LocalizeString(wizBulkImport.ActiveStep.Title + ".Title");
            }
            else if (type == "Help")
            {
                text = LocalizeString(wizBulkImport.ActiveStep.Title + ".Help");
            }
            return text;
        }

#region Private Methods

        private void BindFormFields()
        {
            ddlImportSourceType.SelectedValue = BulkImport.ImportSourceType.ToString("d");

            ConfigureFieldVisibilityForImportSourceType();

            if (BulkImport.ImportSourceType != ImportSourceTypes.DatabaseConnection)
            {
                BindImportFilenames(BulkImport.ImportFilename, BulkImport.ImportSourceType);
            }

            if (BulkImport.ImportSourceType == ImportSourceTypes.DatabaseFile || BulkImport.ImportSourceType == ImportSourceTypes.DatabaseConnection)
            {
                BindDatabaseConnectionStrings(BulkImport.ConnectionStringKey);
                tbDataSource.Text = BulkImport.DBDataSource;
                tbDatabase.Text = BulkImport.DBInitialCatalog;
                cbIntegratedSecurity.Checked = BulkImport.DBIntegratedSecurity;
                tbDbUser.Text = BulkImport.DBUser;
                tbDbPassword.Text = BulkImport.DBPassword;
                BindDatabaseTableNames(BulkImport.DatabaseTableName);
            }
            
            if (BulkImport.ImportSourceType == ImportSourceTypes.CSV)
            {
                SetCSVDelimiterToDDL(BulkImport.CSVDelimiter);
                cbCSVFirstRow.Checked = BulkImport.FirstRowIsHeader;
            }

            if (BulkImport.ImportSourceType == ImportSourceTypes.Excel)
            {
                cbExcelFirstRow.Checked = BulkImport.FirstRowIsHeader;
            }

            if (BulkImport.ImportSourceType == ImportSourceTypes.XML)
            {
                BindItemNodeNames(BulkImport.ItemNodeName);
            }

            grdDataFieldMap.ImportFields = BulkImport.ImportFields;
            grdDataFieldMap.InvitationFields = BulkImport.InvitationFields;

            tbSenderEmail.Text = BulkImport.SenderEmail;

            if (LocaleController.Instance.GetLocales(PortalId).Count > 1)
            {
                divCultureCode.Visible = true;
                ctlCultureCode.DataBind();
                ctlCultureCode.SetLanguage(BulkImport.FallbackCultureCode);
            }
            else
            {
                divCultureCode.Visible = false;
            }

            tbPersonalNote.Text = BulkImport.PersonalNote;

            BindRedirectOnFirstLoginTabs(BulkImport.RedirectOnFirstLogin);

            rblSendMethod.SelectedValue = BulkImport.SendMethod.ToString("d");

            if (BulkImport.ScheduledJobStartsAt != DateTime.MinValue)
            {
                ctlScheduledJobStartsAt.SelectedDate = UserInfo.LocalTime(BulkImport.ScheduledJobStartsAt);
            }

            ctlSendBatchSize.Value = BulkImport.SendBatchSize;
            ctlSendBatchInteval.Value = BulkImport.SendBatchInteval;
        }

        private void SetDefaultsForImportSourceType(ImportSourceTypes importSourceType)
        {
            BulkImport.ImportSourceType = importSourceType;
            switch (importSourceType)
            {
                case ImportSourceTypes.CSV:
                    BulkImport.CSVDelimiter = ',';
                    BulkImport.FirstRowIsHeader = false;
                    BulkImport.ItemNodeName = string.Empty;
                    break;
                case ImportSourceTypes.Excel:
                    BulkImport.CSVDelimiter = (char)0;
                    BulkImport.FirstRowIsHeader = false;
                    BulkImport.ItemNodeName = string.Empty;
                    break;
                case ImportSourceTypes.XML:
                    BulkImport.CSVDelimiter = (char)0;
                    BulkImport.FirstRowIsHeader = false;
                    break;
                case ImportSourceTypes.DatabaseFile:
                    BulkImport.CSVDelimiter = (char)0;
                    BulkImport.FirstRowIsHeader = false;
                    BulkImport.ItemNodeName = string.Empty;
                    BulkImport.ClearDatabaseConnectionString();
                    BulkImport.ConnectionStringKey = null;
                    BulkImport.DatabaseTableName = null;
                    break;
                case ImportSourceTypes.DatabaseConnection:
                    BulkImport.CSVDelimiter = (char)0;
                    BulkImport.FirstRowIsHeader = false;
                    BulkImport.ItemNodeName = string.Empty;
                    BulkImport.ClearDatabaseConnectionString();
                    BulkImport.ConnectionStringKey = null;
                    BulkImport.DatabaseTableName = null;
                    break;
            }
        }

        private void ConfigureFieldVisibilityForImportSourceType()
        {
            switch (BulkImport.ImportSourceType)
            {
                case ImportSourceTypes.CSV:
                    divFileUpload.Visible = true;
                    divConnectionStringKey.Visible = false;
                    divDatabaseConnection.Visible = false;
                    divCSVOptions.Visible = true;
                    divExcelOptions.Visible = false;
                    divXMLOptions.Visible = false;
                    divDatabaseOptions.Visible = false;
                    break;
                case ImportSourceTypes.Excel:
                    divFileUpload.Visible = true;
                    divConnectionStringKey.Visible = false;
                    divDatabaseConnection.Visible = false;
                    divCSVOptions.Visible = false;
                    divExcelOptions.Visible = true;
                    divXMLOptions.Visible = false;
                    divDatabaseOptions.Visible = false;
                    break;
                case ImportSourceTypes.XML:
                    divFileUpload.Visible = true;
                    divConnectionStringKey.Visible = false;
                    divDatabaseConnection.Visible = false;
                    divCSVOptions.Visible = false;
                    divExcelOptions.Visible = false;
                    divXMLOptions.Visible = true;
                    divDatabaseOptions.Visible = false;
                    break;
                case ImportSourceTypes.DatabaseFile:
                    divFileUpload.Visible = true;
                    divConnectionStringKey.Visible = false;
                    divDatabaseConnection.Visible = true;
                    divSQLConnection.Visible = false;
                    divDBCredentials.Visible = true;
                    divCSVOptions.Visible = false;
                    divExcelOptions.Visible = false;
                    divXMLOptions.Visible = false;
                    divDatabaseOptions.Visible = true;
                    break;
                case ImportSourceTypes.DatabaseConnection:
                    divFileUpload.Visible = false;
                    divConnectionStringKey.Visible = true;
                    divDatabaseConnection.Visible = true;
                    divSQLConnection.Visible = true;
                    divCSVOptions.Visible = false;
                    divExcelOptions.Visible = false;
                    divXMLOptions.Visible = false;
                    divDatabaseOptions.Visible = true;
                    break;
            }
            ctlImportFile.Attributes.Add("accept", DnnUtils.EscapedString.Combine(Consts.ValidFileExtensions[(int)BulkImport.ImportSourceType],','));
        }

        private char GetCSVDelimiterFromDDL()
        {
            return rblCSVDelimiter.SelectedIndex == -1 ? ',' : delimiters[rblCSVDelimiter.SelectedIndex];
        }

        private void SetCSVDelimiterToDDL(char csvDelimiter)
        {
            rblCSVDelimiter.SelectedIndex = Array.IndexOf(delimiters, csvDelimiter == (char)0 ? ',' : csvDelimiter);
        }

        private List<string> GetItemNodeNames()
        {
            if (BulkImport.ImportSourceType == ImportSourceTypes.XML && BulkImport.ImportFilename != string.Empty)
            {
                var importFilePhysicalPath = Utilities.GetImportFilePhysicalPath(BulkImport.ImportFilename);
                if (importFilePhysicalPath != string.Empty)
                {
                    using (var fs = new FileStream(importFilePhysicalPath, FileMode.Open))
                    {
                        XDocument xDoc = null;
                        xDoc = XDocument.Load(fs);
                        var rootNode = xDoc.Root;
                        if (rootNode != null)
                        {
                            return rootNode.Elements().Distinct(new NodeNameComparer()).Select(iN => iN.Name.LocalName).OrderBy(s => s).ToList();
                        }                      
                    }
                }
            }
            return new List<string>();
        }

        private IDataReader ExecuteReader(string query, bool isSchemaQuery, out string errorMsg)
        {
            IDataReader dr = null;
            System.Data.Common.DbConnection connection = null;
            System.Data.Common.DbCommand command = null;

            var connectionString = BulkImport.DatabaseConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                errorMsg = LocalizeString("NoDatabaseConnectionConfigured");
                return null;
            }

            try
            {
                if (BulkImport.ImportSourceType == ImportSourceTypes.DatabaseFile)
                {
                    connection = new System.Data.OleDb.OleDbConnection(connectionString);
                    command = new System.Data.OleDb.OleDbCommand(query, (System.Data.OleDb.OleDbConnection)connection);
                }
                else
                {
                    connection = new System.Data.SqlClient.SqlConnection(BulkImport.DatabaseConnectionString);
                    command = new System.Data.SqlClient.SqlCommand(query, (System.Data.SqlClient.SqlConnection)connection);
                }

                var commandBehavior = isSchemaQuery ? CommandBehavior.CloseConnection | CommandBehavior.KeyInfo : CommandBehavior.CloseConnection;
                connection.Open();
                dr = command.ExecuteReader(commandBehavior);
                errorMsg = string.Empty;
            }
            catch (Exception exc)
            {
                errorMsg = exc.ToString();
                if (dr != null) dr.Dispose();
            }
            return dr;
        }

        private void DisplayPreview()
        {
            var errorMessage = string.Empty;

            var dataTable = new System.Data.DataTable();

            if (BulkImport.ImportSourceType == ImportSourceTypes.DatabaseConnection || BulkImport.ImportSourceType == ImportSourceTypes.DatabaseFile)
            {
                if (!string.IsNullOrEmpty(BulkImport.DatabaseTableName))
                {
                    var getColumnsQuery = string.Format("select top 1 * from [{0}]", BulkImport.DatabaseTableName);
                    var getPreviewDataQuery = string.Format("select top {0} * from [{1}]", maxPreviewRows, BulkImport.DatabaseTableName); ;

                    var errorMsg = string.Empty;
                    IDataReader dr = null;

                    using (dr = ExecuteReader(getColumnsQuery, true, out errorMsg))
                    {
                        if (dr != null && dr.Read())
                        {
                            var importFields = new List<ImportField>();
                            var schemaTable = dr.GetSchemaTable();
                            for (var i = 0; i < schemaTable.Rows.Count; i++)
                            {
                                var columnName = (string)schemaTable.Rows[i]["ColumnName"];
                                var columnType = ((System.Type)schemaTable.Rows[i]["DataType"]).Name;
                                var columnSize = (int)schemaTable.Rows[i]["ColumnSize"];

                                var importField = new ImportField(columnName, i);
                                importField.DataType = columnType;
                                importField.MaxLength = columnSize;
                                importFields.Add(importField);
                            }
                            BulkImport.ImportFields = importFields;
                        }
                        else
                        {
                            ShowMessage(errorMsg, ModuleMessage.ModuleMessageType.RedError);
                            divPreviewData.Visible = false;
                            grdDataFieldMap.Visible = false;
                            return;
                        }
                    }

                    using (dr = ExecuteReader(getPreviewDataQuery, false, out errorMessage))
                    {
                        dataTable.Clear();
                        if (dr == null || dr.IsClosed)
                        {
                            ShowMessage(LocalizeString("UnableToOpenDatabaseTable") + errorMessage, ModuleMessage.ModuleMessageType.RedError);
                            divPreviewData.Visible = false;
                            grdDataFieldMap.Visible = false;
                            return;
                        }
                        else
                        {
                            dataTable.Load(dr); // dr should get closed and disposed of at end of DataTable Load
                        }
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(BulkImport.ImportFilename))
                {
                    ShowMessage(LocalizeString("NoImportFileUploadedOrSelected"), ModuleMessage.ModuleMessageType.RedError);
                    return;
                }

                var importFilePhysicalPath = Utilities.GetImportFilePhysicalPath(BulkImport.ImportFilename);
                var fileExtension = Path.GetExtension(BulkImport.ImportFilename).TrimStart('.');

                if (importFilePhysicalPath == string.Empty)
                {
                    ShowMessage(string.Format(LocalizeString("ImportFileNotFound"), BulkImport.ImportFilename), ModuleMessage.ModuleMessageType.RedError);
                    return;
                }

                var fieldIndex = 0;
                var rowIndex = -1; 
                switch (BulkImport.ImportSourceType)
                {
                    case ImportSourceTypes.CSV:
                        using (var fs = new FileStream(importFilePhysicalPath, FileMode.Open))
                        {
                            var reader = new System.IO.StreamReader(fs, Encoding.UTF8);

                            while (!reader.EndOfStream && dataTable.Rows.Count < maxPreviewRows)
                            {
                                rowIndex++;
                                var line = reader.ReadLine();

                                var values = DnnUtils.EscapedString.Seperate(line, GetCSVDelimiterFromDDL()).ToArray();
                                if (rowIndex == 0)
                                {
                                    BulkImport.ImportFields.Clear();
                                    string fieldName;
                                    bool skippedField;
                                    foreach (var value in values)
                                    {
                                        if (value == string.Empty || !cbCSVFirstRow.Checked)
                                        {
                                            fieldName = "Column" + fieldIndex.ToString("00");
                                        }
                                        else
                                        {
                                            fieldName = value.Trim();
                                        }
                                        skippedField = value == string.Empty;

                                        var field = new ImportField(fieldName, fieldIndex) { SkippedField = skippedField };
                                        BulkImport.ImportFields.Add(field);
                                        dataTable.Columns.Add(fieldName, typeof(string));
                                        fieldIndex++;
                                    }
                                    if (!BulkImport.FirstRowIsHeader) dataTable.Rows.Add(values);
                                }
                                else
                                {
                                    if (values.Length > BulkImport.ImportFields.Count)
                                    {
                                        values = values.Take(BulkImport.ImportFields.Count).ToArray();
                                    }   
                                    dataTable.Rows.Add(values);
                                }
                            }
                        }
                        break;
                    case ImportSourceTypes.Excel:
                        using (var fs = new FileStream(importFilePhysicalPath, FileMode.Open))
                        {
                            IExcelDataReader excelReader = null;
                            
                            if (fileExtension == "xls")
                            {
                                excelReader = Excel.ExcelReaderFactory.CreateBinaryReader(fs);
                            }
                            else if (fileExtension == "xlsx")
                            {
                                excelReader = Excel.ExcelReaderFactory.CreateOpenXmlReader(fs);
                            }
                            else
                            {
                                break;
                            }
                            excelReader.IsFirstRowAsColumnNames = cbExcelFirstRow.Checked;

                            var ds = excelReader.AsDataSet(false);
                            if (ds != null)
                            {
                                var rowCount = ds.Tables[0].Rows.Count;
                                var columnCount = ds.Tables[0].Columns.Count;
                                var maxRows = Math.Min(rowCount, maxPreviewRows);

                                BulkImport.ImportFields.Clear();
                                string fieldName;
                                for (fieldIndex = 0; fieldIndex < columnCount; fieldIndex++)
                                {
                                    fieldName = ds.Tables[0].Columns[fieldIndex].Caption;
                                    var field = new ImportField(fieldName, fieldIndex);
                                    BulkImport.ImportFields.Add(field);
                                    dataTable.Columns.Add(fieldName, typeof(string));
                                }

                                while (excelReader.Read() && rowIndex < maxRows)
                                {
                                    rowIndex++;
                                    string[] values = new string[columnCount];
                                    for (fieldIndex = 0; fieldIndex < columnCount; fieldIndex++)
                                    {
                                        values[fieldIndex] = excelReader.GetString(fieldIndex);
                                    }
                                    if (!(rowIndex == 0 && BulkImport.FirstRowIsHeader))
                                    {
                                        dataTable.Rows.Add(values);
                                    }
                                }
                            }
                        }
                            
                        break;
                    case ImportSourceTypes.XML:
                        using (var fs = new FileStream(importFilePhysicalPath, FileMode.Open))
                        {
                            var xDoc = XDocument.Load(fs);
                            var rootNode = xDoc.Root;
                            if (rootNode != null)
                            {
                                var rootNodeName = rootNode.Name.LocalName;
                                var items = rootNode.Elements(ddlItemNodeName.SelectedValue).Take(maxPreviewRows);
                                foreach (XElement item in items)
                                {
                                    rowIndex++;
                                    var kvps = item.Elements().Select(el => new KeyValuePair<string, string>(el.Name.LocalName, el.Value)).OrderBy(k=>k.Key);
                                            
                                    if (rowIndex == 0)
                                    {
                                        BulkImport.ImportFields.Clear();
                                        var i = 0;
                                        var importFields = kvps.Select(k => new ImportField(k.Key, i++));
                                        BulkImport.ImportFields.AddRange(importFields);
                                        dataTable.Columns.AddRange(kvps.Select(k => new DataColumn(k.Key, typeof(string))).ToArray());
                                        dataTable.Rows.Add(kvps.Select(k => k.Value).ToArray());
                                    }
                                    else
                                    {
                                        var importFieldsCount = BulkImport.ImportFields.Count;
                                        if (kvps.Count() == importFieldsCount)
                                        {
                                            dataTable.Rows.Add(kvps.Select(k => k.Value).ToArray());
                                        }
                                        else
                                        {
                                            string[] values = new string[importFieldsCount];
                                            var kvpDictionary = kvps.ToDictionary(k => k.Key);
                                            foreach (ImportField importField in BulkImport.ImportFields)
                                            {
                                                values[importField.ImportFieldIndex] = kvpDictionary[importField.ImportFieldName].Value;
                                            }
                                            dataTable.Rows.Add(values);
                                        }
                                    }                                     
                                }
                            }                               
                        }
                        break;
                }
            }

            if (dataTable.Rows.Count > 0)
            {
                divPreviewData.Visible = true;
                grdPreviewData.DataSource = dataTable;
                grdPreviewData.DataBind();
            }
            else
            {
                ShowMessage(LocalizeString("UnableToOpenDataSourceOrPreview") + errorMessage, ModuleMessage.ModuleMessageType.RedError);
                divPreviewData.Visible = false;
                grdDataFieldMap.Visible = false;
                return;
            }

            if (BulkImport.ImportFields.Count == 0)
            {
                ShowMessage(LocalizeString("DataSourceFormatUnrecognizable") + errorMessage, ModuleMessage.ModuleMessageType.RedError);
                grdDataFieldMap.Visible = false;
            }
            else
            {
                grdDataFieldMap.Visible = true;
                grdDataFieldMap.ImportFields = BulkImport.ImportFields;
                grdDataFieldMap.InvitationFields = BulkImport.InvitationFields;
            }      
        }

        private void BindRedirectOnFirstLoginTabs(string selectedTabName)
        {
            List<TabInfo> tabs = TabController.GetPortalTabs(PortalId, TabId, true, true);
            ddlRedirectOnFirstLogin.DataSource = tabs;
            ddlRedirectOnFirstLogin.DataBind();
            ddlRedirectOnFirstLogin.Items.Insert(0, new ListItem(Localization.GetString("NoneSelected"), ""));

            if (!string.IsNullOrEmpty(selectedTabName) && ddlRedirectOnFirstLogin.Items.FindByValue(selectedTabName) != null)
            {
                ddlRedirectOnFirstLogin.SelectedValue = selectedTabName;
            }
            else
            {
                ddlRedirectOnFirstLogin.SelectedIndex = 0;
            }
        }

        private void BindDatabaseConnectionStrings (string initialValue)
        {
            ddlConnectionStringKey.Items.Clear();
            var siteConnectionStringName = DnnUtils.Config.GetConnectionString();
            
            foreach (ConnectionStringSettings connection in ConfigurationManager.ConnectionStrings)
            {
                var name = connection.Name.ToLower();
                if (name != "localmysqlserver" && name != "localsqlserver" || (connection.Name == siteConnectionStringName && UserInfo.IsSuperUser))
                {
                    ddlConnectionStringKey.Items.Add(new ListItem(connection.Name, connection.Name));
                }
            }

            ddlConnectionStringKey.Items.Insert(0, new ListItem(base.LocalizeString("Custom"), "Custom"));

            if (string.IsNullOrEmpty(initialValue) || ddlConnectionStringKey.Items.FindByValue(initialValue) == null)
            {
                ddlConnectionStringKey.SelectedIndex = 0;
            }
            else
            {
                ddlConnectionStringKey.SelectedValue = initialValue;
                divDatabaseConnection.Visible = initialValue == "Custom";
            }
        }

        private void TestAndChooseOleDbProvider(string preferredProvider, out string errorMsg)
        {
            errorMsg = string.Empty;
            var attempt = 0;
            var failed = false;
            var alternateProvider = preferredProvider == Consts.MSJet4OleDbProvider ? Consts.MSJet12OleDbProvider : Consts.MSJet4OleDbProvider;
            var oleDbProviders = new []{preferredProvider, alternateProvider};
            do
            {
                BulkImport.DBProvider = oleDbProviders[attempt];
                var connection = new System.Data.OleDb.OleDbConnection(BulkImport.DatabaseConnectionString);
                try
                {
                    connection.Open();
                    failed = false;
                }
                catch (Exception exc)
                {
                    errorMsg += string.Format(LocalizeString("OleDbProviderFailure") + Environment.NewLine,
                                 BulkImport.ImportFilename, BulkImport.DBProvider, exc.Message);
                    failed = true;
                    attempt++;
                }
                finally
                {
                    if (connection != null) connection.Dispose();
                }
            } while (failed && attempt < oleDbProviders.Length);
            if (failed)
            {
                BulkImport.DBProvider = string.Empty;
            }
            else
            {
                errorMsg = string.Empty;
            }
        }

        private string BindDatabaseTableNames(string initialTableName)
        {
            var errorMsg = string.Empty;

            var connectionString = BulkImport.DatabaseConnectionString;
            if (!string.IsNullOrEmpty(connectionString))
            {
                ddlDatabaseTableName.DataTextField = "Table_Name";
                ddlDatabaseTableName.DataValueField = "Table_Name";

                System.Data.Common.DbConnection connection = null;             
 
                try
                {
                    if (BulkImport.ImportSourceType == ImportSourceTypes.DatabaseFile)
                    {
                        connection = new System.Data.OleDb.OleDbConnection(connectionString);
                    }
                    else
                    {
                        connection = new System.Data.SqlClient.SqlConnection(BulkImport.DatabaseConnectionString);
                    }
                    connection.Open();
                    var dbSchema = connection.GetSchema("Tables");
                    dbSchema.DefaultView.Sort="Table_Name";
                    ddlDatabaseTableName.DataSource = dbSchema.DefaultView;
                    ddlDatabaseTableName.DataBind();

                    if (!string.IsNullOrEmpty(initialTableName))
                    {
                        if (ddlDatabaseTableName.Items.FindByValue(initialTableName) != null)
                        {
                            ddlItemNodeName.SelectedValue = initialTableName;
                        }
                        else
                        {
                            ddlItemNodeName.SelectedIndex = -1;
                        }
                    }
                    else
                    {
                        ddlDatabaseTableName.SelectedIndex = 0;
                    }
                }
                catch (Exception exc)
                {
                    ddlDatabaseTableName.Items.Clear();
                    errorMsg = exc.Message;
                }
                finally
                {
                    connection.Dispose();
                }
            }

            return errorMsg;
        }

        private string GetPrimaryKeyOrClusteredIndex()
        {
            string keyOrIndex;

            if (BulkImport.ImportSourceType == ImportSourceTypes.DatabaseConnection)
            {
                keyOrIndex = GetSQLClusteredIndex();
            }
            else
            {
                keyOrIndex = GetOleDbPrimaryKey();
            }

            if (keyOrIndex == string.Empty)  // no clustered index or primary key exists on this table so order by recipient email field
            {
                var mappedEmailFieldIndex = BulkImport.InvitationFields["RecipientEmail"].MappedImportFieldIndex;
                if (mappedEmailFieldIndex != -1 && BulkImport.ImportFields.Count > mappedEmailFieldIndex)
                {
                    keyOrIndex = BulkImport.ImportFields[mappedEmailFieldIndex].ImportFieldName;
                }
            }
            return keyOrIndex;
        }

        private string GetSQLClusteredIndex()
        {
            var connectionString = BulkImport.DatabaseConnectionString;
            var tableName = BulkImport.DatabaseTableName;
            var clusteredIndex = string.Empty;

            if (!string.IsNullOrEmpty(connectionString) && !string.IsNullOrEmpty(tableName))
            {
                var clusteredIndexQuery = string.Format("select " +
                                                            "   sc.name, sic.is_descending_key " +
                                                            "from sys.objects t " +
                                                            "left outer join sys.indexes si on si.object_id = t.object_id " +
                                                            "left outer join sys.index_columns sic on sic.object_id = t.object_id and sic.index_id = si.index_id and sic.is_included_column = 0 " +
                                                            "left outer join sys.columns sc on sc.object_id = t.object_id and sc.column_id = sic.column_id " +
                                                            "where (si.type = 1 or si.is_primary_key = 1) and t.name = '{0}' " +
                                                            "order by sic.index_column_id", tableName);


                var errorMsg = string.Empty;

                using (var dr = DotNetNuke.Data.DataProvider.Instance().ExecuteSQLTemp(BulkImport.DatabaseConnectionString, clusteredIndexQuery, out errorMsg))
                {
                    if (dr != null && !dr.IsClosed)
                    {
                        while (dr.Read())
                        {
                            clusteredIndex += dr.GetString(0).WrapWithBrackets() + (dr.GetBoolean(1) ? " DESC" : "") + ", ";
                        }
                    }
                }
            }
            return clusteredIndex.Substring(0, clusteredIndex.EndsWith(", ") ? clusteredIndex.Length - 2 : clusteredIndex.Length);
        }

        private string GetOleDbPrimaryKey()
        {
            var connectionString = BulkImport.DatabaseConnectionString;
            var tableName = BulkImport.DatabaseTableName;
            var primaryKey = string.Empty;

            if (!string.IsNullOrEmpty(connectionString) && !string.IsNullOrEmpty(tableName))
            {
                var connection = new System.Data.OleDb.OleDbConnection(connectionString);
           
                try
                {
                    connection.Open();
                    var restrictions = connection.GetSchema("Restrictions");
                    var tableNameRestrictionNumber = (int)restrictions.Select("CollectionName = 'Indexes' and RestrictionName = 'Table_Name'")[0]["RestrictionNumber"];
                    var restriction = new string[5];
                    restriction[tableNameRestrictionNumber - 1] = tableName;
                    var indexes = connection.GetSchema("Indexes", restriction);
                    indexes.DefaultView.Sort = "Ordinal_Position";
                    foreach (DataRow row in indexes.Rows)
                    {
                        if ((bool)row["Primary_Key"] || (bool)row["Clustered"])
                        {
                            var column_Name = (string)row["Column_Name"];
                            var collation = (short)row["Collation"] == 2 ? " DESC" : "";
                            primaryKey += column_Name.WrapWithBrackets() + collation + ", ";
                        }                     
                    }
                    primaryKey.TrimEnd(' ');    
                }
                finally
                {
                    connection.Dispose();
                }
            }
            return primaryKey.Substring(0, primaryKey.EndsWith(", ") ? primaryKey.Length - 2 : primaryKey.Length);
        }

        private void BindImportFilenames(string importFilename, ImportSourceTypes type)
        {
            var extensions = Consts.ValidFileExtensions[(int)type];
            var importFilenames = Utilities.GetImportFolderFileNames(extensions);
            
            ddlImportFilename.DataSource = importFilenames;
            ddlImportFilename.DataBind();
            ddlImportFilename.Items.Insert(0, new ListItem(LocalizeString("NoneSelected"), ""));
            if (importFilenames.Count > 0)
            {
                if (string.IsNullOrEmpty(importFilename))
                {
                    ddlImportFilename.SelectedIndex = 0;
                }
                else
                {
                    var li = ddlImportFilename.Items.FindByValue(importFilename);
                    if (li != null)
                    {
                        ddlImportFilename.SelectedValue = importFilename;
                    }
                    else
                    {
                        ddlImportFilename.SelectedIndex = 0;
                    }
                }
            }
            else
            {
                ddlImportFilename.Attributes.Add("display", "none");
            }
        }

        private void BindItemNodeNames(string initialValue)
        {
            var itemNodeNames = GetItemNodeNames();
            if (itemNodeNames.Count > 0)
            {
                ddlItemNodeName.DataSource = itemNodeNames;
                ddlItemNodeName.DataBind();
                if (string.IsNullOrEmpty(initialValue))
                {
                    ddlItemNodeName.SelectedIndex = 0;
                }
                else
                {
                    var li = ddlItemNodeName.Items.FindByValue(initialValue);
                    if (li != null)
                    {
                        ddlItemNodeName.SelectedValue = initialValue;
                    }
                    else
                    {
                        ddlItemNodeName.SelectedIndex = -1;
                    }
                }
            }
        }

        private void ShowMessage(string msg, ModuleMessage.ModuleMessageType messageType )
        {
            var phMessage = wizBulkImport.FindControl("HeaderContainer").FindControl("phMessage") as PlaceHolder;
            if (phMessage != null)
            {
                phMessage.Visible = true;
                ModuleMessage moduleMessage = DotNetNuke.UI.Skins.Skin.GetModuleMessageControl(null, msg, messageType, null);
                phMessage.Controls.Add(moduleMessage);
            }
        }

#endregion

 #region Event Handlers

        protected void rblCSVDelimiter_SelectedIndexChanged(object sender, EventArgs e)
        {
            BulkImport.ImportFields.Clear();
            BulkImport.CSVDelimiter = GetCSVDelimiterFromDDL();
            InvitationController.SaveBulkImport(BulkImport);
            DisplayPreview();
        }

        protected void cbFirstRow_CheckedChanged(object sender, EventArgs e)
        {
            var cbFirstRow = (CheckBox)sender;
            BulkImport.FirstRowIsHeader = cbFirstRow.Checked;
            InvitationController.SaveBulkImport(BulkImport);
            DisplayPreview();
        }

        protected void ddlItemNodeName_SelectedIndexChanged(object sender, EventArgs e)
        {
            BulkImport.ImportFields.Clear();
            BulkImport.ItemNodeName = ddlItemNodeName.SelectedValue;
            InvitationController.SaveBulkImport(BulkImport);
            DisplayPreview();
        }

        protected void ddlConnectionStringKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            var connectionStringKey = ddlConnectionStringKey.SelectedValue;
            if (connectionStringKey == "Custom")
            {
                divDatabaseConnection.Visible = true;
                tbDataSource.Text = string.Empty;
                tbDatabase.Text = string.Empty;
                cbIntegratedSecurity.Checked = false;
                tbDbUser.Text = string.Empty;
                tbDbPassword.Text = string.Empty;
            }
            else
            {
                ConnectionStringSettings connection = ConfigurationManager.ConnectionStrings[connectionStringKey];
                if (connection != null)
                {
                    BulkImport.DatabaseConnectionString = connection.ConnectionString;
                    divDatabaseConnection.Visible = false;        
                }
            }
        }

        protected void ddlDatabaseTableName_SelectedIndexChanged(object sender, EventArgs e)
        {
            BulkImport.ImportFields.Clear();
            BulkImport.DatabaseTableName = ddlDatabaseTableName.SelectedValue;
            BulkImport.OrderBy = GetPrimaryKeyOrClusteredIndex();
            InvitationController.SaveBulkImport(BulkImport);
            DisplayPreview();
        }

        protected void grdPreviewData_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var lblRowNumber = e.Row.FindControl("lblRowNumber") as Label;
                if (lblRowNumber != null)
                {
                    lblRowNumber.Text = (e.Row.DataItemIndex + 1).ToString("000");
                }
            }
        }

        protected void wizBulkImport_NextButtonClick(object sender, WizardNavigationEventArgs e)
        {
            string errMessage = string.Empty;

            switch (e.CurrentStepIndex)
            {
                case 0:
                    SetDefaultsForImportSourceType((ImportSourceTypes)Convert.ToInt32(ddlImportSourceType.SelectedValue));
                    ConfigureFieldVisibilityForImportSourceType();
                    if (BulkImport.ImportSourceType != ImportSourceTypes.DatabaseConnection)
                    {
                        BindImportFilenames(BulkImport.ImportFilename, BulkImport.ImportSourceType);
                    }
                    else
                    {
                        BindDatabaseConnectionStrings(BulkImport.ConnectionStringKey);
                    }
                    break;
                case 1:

                    if (BulkImport.ImportSourceType == ImportSourceTypes.DatabaseConnection)
                    {
                        BulkImport.ImportFilename = string.Empty;
                        BulkImport.ClearDatabaseConnectionString();

                        if (ddlConnectionStringKey.SelectedValue == "Custom")
                        {
                            BulkImport.DBDataSource = tbDataSource.Text.Trim();
                            BulkImport.DBInitialCatalog = tbDatabase.Text.Trim();
                            BulkImport.DBIntegratedSecurity = cbIntegratedSecurity.Checked;
                            if (cbIntegratedSecurity.Checked)
                            {
                                tbDbUser.Text = string.Empty;
                                tbDbPassword.Text = string.Empty;
                            }
                            BulkImport.DBUser = tbDbUser.Text.Trim();
                            BulkImport.DBPassword = tbDbPassword.Text.Trim();
                        }
                        else if (ddlConnectionStringKey.SelectedValue != string.Empty)
                        {
                            ConnectionStringSettings connection = ConfigurationManager.ConnectionStrings[ddlConnectionStringKey.SelectedValue];
                            if (connection != null)
                            {
                                BulkImport.DatabaseConnectionString = connection.ConnectionString;
                            }
                        }

                        var connectionString = BulkImport.DatabaseConnectionString;

                        var testOpenDatabaseQuery = string.Format("select name from sys.databases where name = '{0}'", BulkImport.DBInitialCatalog);

                        using (var dr = DataProvider.Instance().ExecuteSQLTemp(connectionString, testOpenDatabaseQuery, out errMessage))
                        {
                            if (dr == null || !dr.Read())
                            {
                                ShowMessage(LocalizeString("DatabaseConnectionConfigurationError") + errMessage, ModuleMessage.ModuleMessageType.RedError);
                                e.Cancel = true;
                                return;
                            }
                        }
                    }
                    else
                    {
                        var filename = string.Empty;
                        var fileExtension = string.Empty;

                        if (ctlImportFile.PostedFile != null && ctlImportFile.PostedFile.ContentLength > 0 && !string.IsNullOrEmpty(ctlImportFile.PostedFile.FileName))
                        {
                            filename = ctlImportFile.PostedFile.FileName;
                            fileExtension = Path.GetExtension(filename).ToLower();

                            if (Utilities.IsValidExtension(fileExtension, BulkImport.ImportSourceType))
                            {
                                errMessage = Utilities.SaveImportFile(ctlImportFile.PostedFile.InputStream, filename);
                                BulkImport.ImportFilename = filename;
                                BindImportFilenames(filename, BulkImport.ImportSourceType);
                            }
                            else
                            {
                                errMessage = string.Format(LocalizeString("InvalidFilenameExtension"), fileExtension);
                            }
                        }
                        else if (ddlImportFilename.SelectedIndex > 0)
                        {
                            filename = BulkImport.ImportFilename = ddlImportFilename.SelectedValue;
                            fileExtension = Path.GetExtension(BulkImport.ImportFilename).ToLower();
                        }
                        else
                        {
                            errMessage = LocalizeString("NoImportFileUploadedOrSelected");
                        }

                        if (!string.IsNullOrEmpty(errMessage))
                        {
                            ShowMessage(errMessage, ModuleMessage.ModuleMessageType.RedError);
                            e.Cancel = true;
                            return;
                        }

                        if (BulkImport.ImportSourceType == ImportSourceTypes.DatabaseFile)
                        {
                            BulkImport.DBDataSource = Utilities.GetImportFilePhysicalPath(filename);
                            BulkImport.DBUser = tbDbUser.Text.Trim();
                            BulkImport.DBPassword = tbDbPassword.Text.Trim();

                            switch (fileExtension)
                            {
                                case ".mdb":
                                    string oleDBProviderError;
                                    TestAndChooseOleDbProvider(Consts.MSJet4OleDbProvider, out oleDBProviderError);
                                    if (BulkImport.DBProvider == string.Empty)
                                    {
                                        ShowMessage(oleDBProviderError, ModuleMessage.ModuleMessageType.RedError);
                                        e.Cancel = true;
                                        return;
                                    }
                                    break;
                                case ".accdb":
                                    BulkImport.DBProvider = Consts.MSJet12OleDbProvider;
                                    break;
                                default:
                                    BulkImport.DBProvider = Consts.DefaultOleDbProvider;
                                    break;
                            }
                        }
                    }

                    if (BulkImport.ImportSourceType == ImportSourceTypes.DatabaseConnection || BulkImport.ImportSourceType == ImportSourceTypes.DatabaseFile)
                    {
                        errMessage = BindDatabaseTableNames(BulkImport.DatabaseTableName);

                        if (string.IsNullOrEmpty(errMessage))
                        {
                            ShowMessage(LocalizeString("DatabaseOpenedSuccessfully"), ModuleMessage.ModuleMessageType.BlueInfo);
                            BulkImport.DatabaseTableName = ddlDatabaseTableName.SelectedValue;
                            BulkImport.OrderBy = GetPrimaryKeyOrClusteredIndex();
                                
                        }
                        else
                        {
                            ShowMessage(LocalizeString("DatabaseConnectionConfigurationError") + errMessage, ModuleMessage.ModuleMessageType.RedError);
                            e.Cancel = true;
                            return;
                        }
                    }
                    else if (BulkImport.ImportSourceType == ImportSourceTypes.XML)
                    {
                        BindItemNodeNames(BulkImport.ItemNodeName);
                    }
                    DisplayPreview();
                    break;
                case 2:
                    grdDataFieldMap.UpdateFieldMapping();

                    foreach (InvitationField invitationField in grdDataFieldMap.InvitationFields.Values)
                    {
                        if (invitationField.IsRequired && invitationField.MappedImportFieldIndex == -1)
                        {
                            ShowMessage(string.Format(LocalizeString("RequiredFieldNotMapped"), invitationField.FieldName), ModuleMessage.ModuleMessageType.RedError);
                            e.Cancel = true;
                            return;
                        }
                    }
 
                    switch (BulkImport.ImportSourceType)
                    {
                        case ImportSourceTypes.CSV:
                            BulkImport.CSVDelimiter = GetCSVDelimiterFromDDL();
                            BulkImport.FirstRowIsHeader = cbCSVFirstRow.Checked;
                            break;
                        case ImportSourceTypes.Excel:
                            BulkImport.FirstRowIsHeader = cbExcelFirstRow.Checked;
                            break;
                        case ImportSourceTypes.XML:
                            BulkImport.ItemNodeName = ddlItemNodeName.SelectedValue;
                            break;
                    }

                    if (grdDataFieldMap.ImportFields != null) BulkImport.ImportFields = grdDataFieldMap.ImportFields;
                    if (grdDataFieldMap.InvitationFields != null) BulkImport.InvitationFields = grdDataFieldMap.InvitationFields;

                    break;
                case 3:
                    BulkImport.SenderEmail = tbSenderEmail.Text != string.Empty ? tbSenderEmail.Text : (UserInfo.IsInRole("Administrators") ? UserInfo.Email : PortalSettings.Email);
                    BulkImport.PersonalNote = tbPersonalNote.Text;
                    BulkImport.RedirectOnFirstLogin = ddlRedirectOnFirstLogin.SelectedValue;
                    BulkImport.SendMethod = (SendMethods)(Convert.ToInt32(rblSendMethod.SelectedValue));
                    if (BulkImport.SendMethod == SendMethods.Scheduled)
                    {
                        var scheduledJobStartsAt = DnnUtils.DateUtils.GetDatabaseTime();
                        if (ctlScheduledJobStartsAt.SelectedDate != null)
                        {
                            scheduledJobStartsAt = Utilities.UserToUTCTime((DateTime)(ctlScheduledJobStartsAt.SelectedDate), UserInfo);
                        }
                        BulkImport.ScheduledJobStartsAt = scheduledJobStartsAt;
                        BulkImport.SendBatchSize = (int)(ctlSendBatchSize.Value);
                        BulkImport.SendBatchInteval = (int)(ctlSendBatchInteval.Value);
                    }
                    else
                    {
                        BulkImport.ScheduledJobStartsAt = DateTime.MinValue;
                        BulkImport.SendBatchSize = 0;
                        BulkImport.SendBatchInteval = 0;
                    }

                    break;
                case 4:
                    switch (BulkImport.SendMethod)
                    {
                        case SendMethods.Sync:
                            int recordsRead = 0;
                            int recordsSkipped = 0;
                            int invitationsCreated = 0;
                            int invitationsSent = 0;
                            int notificationsSent = 0;
                            int errorCount = 0;
                            bool aborted = false;

                            BulkImport.BatchNumber = 1;
                            
                            var msg = BulkImport.CreateInvitations(out recordsRead, out recordsSkipped, out invitationsCreated, out notificationsSent, out errorCount, out aborted);
                            if (aborted)
                            {
                                ShowMessage(string.Format(LocalizeString("FatalErrorWhileCreatingInvitations"), msg), ModuleMessage.ModuleMessageType.RedError);
                                e.Cancel = true;
                                return;
                            }
                                
                            if (invitationsCreated > 0)
                            {
                                msg += Environment.NewLine + BulkImport.SendInvitations(out invitationsSent, out notificationsSent, out errorCount);
                                var finishNavigationControl = wizBulkImport.FindControl("FinishNavigationTemplateContainerID");
                                if (finishNavigationControl != null)
                                {
                                    var previousButtonFinish = finishNavigationControl.FindControl("previousButtonFinish") as LinkButton;
                                    if (previousButtonFinish != null)
                                    {
                                        previousButtonFinish.Visible = false;
                                    }
                                }
                            }
                            else
                            {
                                ShowMessage(LocalizeString("NoInvitationsCreated"), ModuleMessage.ModuleMessageType.RedError);
                            }
                            divCompletionSummary.InnerHtml = msg.Replace(Environment.NewLine, "<br />");

                            break;

                        case SendMethods.ASync:
                            ShowMessage(LocalizeString("AsyncNotImplemented"), ModuleMessage.ModuleMessageType.RedError);
                            e.Cancel = true;
                            return;
                        case SendMethods.Scheduled:
                            // Force scheduled job to start immediately
                            var scheduler = DotNetNuke.Services.Scheduling.DNNScheduler.Instance();
                            var scheduleItem = scheduler.GetSchedule("WESNet.DNN.Modules.ByInvitation.InvitationScheduler,WESNet.DNN.Modules.ByInvitation", "");
                            scheduler.RunScheduleItemNow(scheduleItem);

                            break;
                    }
                    break;
            }
            
            BulkImport.WizardStepLastCompleted = e.CurrentStepIndex;
            BulkImportID = InvitationController.SaveBulkImport(BulkImport);

        }
        
        protected void wizBulkImport_ActiveStepChanged(object sender, EventArgs e)
        {
            string errorMsg = string.Empty;

            switch (wizBulkImport.ActiveStepIndex)
            {
                case 0: //Introduction and select import source type              
                    break;
                case 1: //Upload source file or connect to database                              
                    break;
                case 2: //Preview Data and Map Import to Invitation Fields       
                    //DisplayPreview();
                    break;
                case 3:         
                    break;
                case 4: //Summary of settings and SendMethod warning
                    lblSelectedSendMethodHelp.Text = LocalizeString(BulkImport.SendMethod.ToString() + ".Help");
                    break;
                case 5:                    
                    break;
            }
        }

        protected void cmdResume_Click(object sender, EventArgs e)
        {
            divResumeWizard.Visible = false;
            divStep0Form.Visible = true;
            BindFormFields();
            wizBulkImport.ActiveStepIndex = BulkImport.WizardStepLastCompleted + 1;
        }

        protected void cmdStartOver_Click(object sender, EventArgs e)
        {
            InvitationController.DeleteBulkImport(PortalId, UserId, BulkImportID);
            BulkImportID = -1;
            divResumeWizard.Visible = false;
            divStep0Form.Visible = true;
            SetDefaultsForImportSourceType(ImportSourceTypes.CSV);
            BindFormFields();
        }

        protected void cmdCancel_Click(object sender, EventArgs e)
        {
            //InvitationController.DeleteBulkImport(PortalId, UserId, BulkImportID);
            Response.Redirect(Globals.NavigateURL(TabId), true);
        }

        protected void wizBulkImport_CancelButtonClick(object sender, EventArgs e)
        {
            //Redirect to module view control
            Response.Redirect(Globals.NavigateURL(), true);
        }

        protected void wizBulkImport_FinishButtonClick(object sender, WizardNavigationEventArgs e)
        {
            BulkImport.WizardStepLastCompleted = 5;
            BulkImportID = InvitationController.SaveBulkImport(BulkImport);
            Response.Redirect(Globals.NavigateURL(), true);
        }

#endregion
    }
}