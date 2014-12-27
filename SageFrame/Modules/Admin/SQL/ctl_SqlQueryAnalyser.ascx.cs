#region "Copyright"
/*
FOR FURTHER DETAILS ABOUT LICENSING, PLEASE VISIT "LICENSE.txt" INSIDE THE SAGEFRAME FOLDER
*/
#endregion

#region "References"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SageFrame.Web;
using SageFrame.Web.Utilities;
using System.IO;
#endregion

namespace SageFrame.Modules.Admin.SQL
{
    public partial class ctl_SqlQueryAnalyser : BaseAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        #region "Control Events"

        protected void imbUploadSqlScript_Click(object sender, EventArgs e)
        {
            LoadSqlScript();
        }

        protected void imbExecuteSql_Click(object sender, EventArgs e)
        {
            ExecuteSql();
        }

        #endregion

        #region "private methods"

        private void LoadSqlScript()
        {
            try
            {
                if (IsPostBack)
                {
                    if (fluSqlScript.HasFile && fluSqlScript.PostedFile.FileName != string.Empty)
                    {
                        string ext = Path.GetExtension(fluSqlScript.PostedFile.FileName);
                        if (Path.GetExtension(fluSqlScript.PostedFile.FileName) != ".sql" && Path.GetExtension(fluSqlScript.PostedFile.FileName) != ".txt")
                        {
                            ShowMessage(SageMessageTitle.Information.ToString(), "Invalid file format", "", SageMessageType.Alert);

                        }
                        else
                        {
                            var file = fluSqlScript.PostedFile.InputStream;
                            //GetBytesFromStream(System.IO.Stream)
                            file.Seek(0, SeekOrigin.Begin);
                            System.IO.StreamReader scriptFile = new System.IO.StreamReader(file);
                            txtSqlQuery.Text = scriptFile.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ProcessException(ex);
            }
        }

        //public static byte[] ConvertImageToByteArray(Stream afuInputStream, int contentLength)
        //{
        //    byte[] sqlBinaryData = new byte[contentLength];
        //    afuInputStream.Read(sqlBinaryData, 0, contentLength);
        //    return sqlBinaryData;
        //}

        private void ExecuteSql()
        {
            try
            {
                SQLHandler objSqlh = new SQLHandler();
                if (chkRunAsScript.Checked == true)
                {
                    string strError = objSqlh.ExecuteScript(txtSqlQuery.Text);
                    if (string.IsNullOrEmpty(strError))
                    {


                        ShowMessage(SageMessageTitle.Information.ToString(), GetSageMessage("SQL", "TheQueryCompletedSuccessfully"), "", SageMessageType.Success);

                    }
                    else
                    {
                        ShowMessage(SageMessageTitle.Notification.ToString(), strError, "", SageMessageType.Alert);

                    }
                }
                else
                {
                    System.Data.DataTable dt = objSqlh.ExecuteSQL(txtSqlQuery.Text);
                    if (dt != null)
                    {
                        gdvResults.DataSource = dt;
                        gdvResults.DataBind();
                    }
                    else
                    {

                        ShowMessage(SageMessageTitle.Notification.ToString(), GetSageMessage("SQL", "ThereIsAnErrorInYourQuery"), "", SageMessageType.Alert);

                    }
                }
                string txt = txtSqlQuery.Text;
            }
            catch (Exception ex)
            {
                ProcessException(ex);
            }
        }

        #endregion


    }
}