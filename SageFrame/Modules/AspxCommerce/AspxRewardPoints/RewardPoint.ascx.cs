using SageFrame.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Modules_AspxCommerce_AspxRewardPoints_RewardPoint : BaseAdministrationUserControl
{
    public string servicePath = "";
    public double rewardpoint = 0;
    protected void Page_Load(object sender, EventArgs e)
    {

        servicePath = ResolveUrl(this.TemplateSourceDirectory);

        rewardpoint = CheckOutSessions.Get<double>("RewardPoints", 0);

    }
}