using System;
using System.Web;
using System.IO;
using SageFrame.Framework;
using AspxCommerce.Core;

public partial class Modules_AspxCommerce_GoogleCheckOut_APICallbackUrl : PageBase
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                                             
                                                                           
               
               
                string serialNumber = string.Empty; ;
                               Stream requestInputStream = Request.InputStream;
                string requestStreamAsString = string.Empty; ;
                using (System.IO.StreamReader streamReader = new StreamReader(requestInputStream))
                {
                    requestStreamAsString = streamReader.ReadToEnd();
                }
                               string[] requestStreamAsParts = requestStreamAsString.Split(new char[] { '=' });
                if (requestStreamAsParts.Length >= 2)
                {
                    serialNumber = requestStreamAsParts[1];
                }

               
                if (!string.IsNullOrEmpty(serialNumber))
                {
                    bool isSerialNumber = GoogleCheckOutHandler.SerialNumberExist(serialNumber.Substring(0, 15));
                    if (!isSerialNumber)
                    { 
                                               GoogleCheckoutHelper.ProcessNotification(serialNumber);
                       
                                               var response = new GCheckout.AutoGen.NotificationAcknowledgment();
                        response.serialnumber = serialNumber;
                        HttpContext.Current.Response.Clear();
                        HttpContext.Current.Response.BinaryWrite(GCheckout.Util.EncodeHelper.Serialize(response));
                        HttpContext.Current.Response.StatusCode = 200;
                    }                    

                }
            }
            catch (Exception ex)
            { ProcessException(ex); }
        }


    }








}

