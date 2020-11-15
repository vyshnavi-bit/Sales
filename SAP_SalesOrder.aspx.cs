﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.SqlClient;
public partial class SAP_SalesOrder : System.Web.UI.Page
{
    MySqlCommand cmd;
    string UserName = "";
    VehicleDBMgr vdm;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["salestype"] == null)
        {
            Response.Redirect("Login.aspx");
        }
        //UserName = Session["field1"].ToString();
        //vdm = new VehicleDBMgr();
        if (!this.IsPostBack)
        {
            if (!Page.IsCallback)
            {
                txtFromdate.Text = DateTime.Now.ToString("dd-MM-yyyy HH:mm");
                lblTitle.Text = Session["TitleName"].ToString();
                FillRouteName();
            }
        }


    }
    protected void ddlType_SelectedIndexChanged(object sender, EventArgs e)
    {
        vdm = new VehicleDBMgr();
        FillRouteName();
    }
    protected void btnGenerate_Click(object sender, EventArgs e)
    {
        GetReport();
    }
    void FillRouteName()
    {
        vdm = new VehicleDBMgr();
        if (Session["salestype"].ToString() == "Plant")
        {
            PBranch.Visible = true;
            cmd = new MySqlCommand("SELECT branchdata.BranchName, branchdata.sno FROM branchdata INNER JOIN branchmappingtable ON branchdata.sno = branchmappingtable.SubBranch WHERE (branchmappingtable.SuperBranch = @SuperBranch) and (branchdata.SalesType=@SalesType)  ");
            cmd.Parameters.AddWithValue("@SuperBranch", Session["branch"]);
            cmd.Parameters.AddWithValue("@SalesType", "21");
            cmd.Parameters.AddWithValue("@SalesType1", "26");
            DataTable dtRoutedata = vdm.SelectQuery(cmd).Tables[0];
            ddlSalesOffice.DataSource = dtRoutedata;
            ddlSalesOffice.DataTextField = "BranchName";
            ddlSalesOffice.DataValueField = "sno";
            ddlSalesOffice.DataBind();
        }
        else
        {
            PBranch.Visible = true;
            cmd = new MySqlCommand("SELECT BranchName, sno FROM branchdata WHERE (sno = @BranchID)");
            cmd.Parameters.AddWithValue("@SOID", Session["branch"]);
            cmd.Parameters.AddWithValue("@BranchID", Session["branch"]);
            DataTable dtRoutedata = vdm.SelectQuery(cmd).Tables[0];
            ddlSalesOffice.DataSource = dtRoutedata;
            ddlSalesOffice.DataTextField = "BranchName";
            ddlSalesOffice.DataValueField = "sno";
            ddlSalesOffice.DataBind();
        }
    }
    private DateTime GetLowDate(DateTime dt)
    {
        double Hour, Min, Sec;
        DateTime DT = DateTime.Now;
        DT = dt;
        Hour = -dt.Hour;
        Min = -dt.Minute;
        Sec = -dt.Second;
        DT = DT.AddHours(Hour);
        DT = DT.AddMinutes(Min);
        DT = DT.AddSeconds(Sec);
        return DT;
    }
    private DateTime GetHighDate(DateTime dt)
    {
        double Hour, Min, Sec;
        DateTime DT = DateTime.Now;
        Hour = 23 - dt.Hour;
        Min = 59 - dt.Minute;
        Sec = 59 - dt.Second;
        DT = dt;
        DT = DT.AddHours(Hour);
        DT = DT.AddMinutes(Min);
        DT = DT.AddSeconds(Sec);
        return DT;
    }
    DataTable Report = new DataTable();
    void GetReport()
    {
        try
        {
            lblmsg.Text = "";
            pnlHide.Visible = true;
            Report = new DataTable();
            Session["RouteName"] = ddlSalesOffice.SelectedItem.Text;
            Session["xporttype"] = "SapSales";
            Session["IDate"] = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy");
            vdm = new VehicleDBMgr();
            DateTime fromdate = DateTime.Now;
            string[] dateFromstrig = txtFromdate.Text.Split(' ');
            if (dateFromstrig.Length > 1)
            {
                if (dateFromstrig[0].Split('-').Length > 0)
                {
                    string[] dates = dateFromstrig[0].Split('-');
                    string[] times = dateFromstrig[1].Split(':');
                    fromdate = new DateTime(int.Parse(dates[2]), int.Parse(dates[1]), int.Parse(dates[0]), int.Parse(times[0]), int.Parse(times[1]), 0);
                }
            }
            lbl_selfromdate.Text = fromdate.ToString("dd/MM/yyyy");
            lblRoutName.Text = ddlSalesOffice.SelectedItem.Text;
            Session["filename"] = ddlSalesOffice.SelectedItem.Text + " Sap Sales " + fromdate.ToString("dd/MM/yyyy");

            //Old 02/02/2017
            //cmd = new MySqlCommand("SELECT branchdata_1.whcode, branchdata.tbranchname,branchdata.customercode, branchdata_1.sno, branchdata.BranchName, branchdata.sno AS BSno,indent.IndentNo, indent.IndentType, ROUND(SUM(indents_subtable.unitQty), 2) AS unitQty, indents_subtable.UnitCost, productsdata.tproduct, productsdata.ProductName,productsdata.Itemcode,productsdata.Units, productsdata.sno AS Expr1, branchdata_1.SalesOfficeID, products_category.tcategory, branchproducts.VatPercent FROM  (SELECT IndentNo, Branch_id, I_date, Status, IndentType FROM indents WHERE (I_date BETWEEN @starttime AND @endtime) AND (Status <> 'D')) indent INNER JOIN branchdata ON indent.Branch_id = branchdata.sno INNER JOIN indents_subtable ON indent.IndentNo = indents_subtable.IndentNo INNER JOIN productsdata ON indents_subtable.Product_sno = productsdata.sno INNER JOIN branchmappingtable ON branchdata.sno = branchmappingtable.SubBranch INNER JOIN branchdata branchdata_1 ON branchmappingtable.SuperBranch = branchdata_1.sno INNER JOIN products_subcategory ON productsdata.SubCat_sno = products_subcategory.sno INNER JOIN products_category ON products_subcategory.category_sno = products_category.sno INNER JOIN branchproducts ON branchmappingtable.SuperBranch = branchproducts.branch_sno AND productsdata.sno = branchproducts.product_sno WHERE (branchmappingtable.SuperBranch = @BranchID) OR (branchdata_1.SalesOfficeID = @SOID) GROUP BY productsdata.sno, BSno, branchmappingtable.SuperBranch ORDER BY branchdata.BranchName");
            cmd = new MySqlCommand("SELECT     branchdata_1.whcode, branchdata.tbranchname, branchdata.customercode, branchdata_1.sno, branchdata.BranchName, branchdata.sno AS BSno, indent.IndentNo,indent.IndentType, ROUND(SUM(indents_subtable.unitQty), 2) AS unitQty, indents_subtable.UnitCost, productsdata.tproduct, productsdata.ProductName, productsdata.Itemcode,productsdata.hsncode, productsdata.Units, productsdata.sno AS Expr1, branchdata_1.SalesOfficeID, products_category.tcategory, branchproducts.VatPercent,productsdata.igst, productsdata.cgst, productsdata.sgst, branchdata.BranchCode, branchdata_1.stateid FROM  (SELECT IndentNo, Branch_id, I_date, Status, IndentType FROM  indents WHERE (I_date BETWEEN @starttime AND @endtime) AND (Status <> 'D')) indent INNER JOIN  branchdata ON indent.Branch_id = branchdata.sno INNER JOIN indents_subtable ON indent.IndentNo = indents_subtable.IndentNo INNER JOIN productsdata ON indents_subtable.Product_sno = productsdata.sno INNER JOIN branchmappingtable ON branchdata.sno = branchmappingtable.SubBranch INNER JOIN branchdata branchdata_1 ON branchmappingtable.SuperBranch = branchdata_1.sno INNER JOIN products_subcategory ON productsdata.SubCat_sno = products_subcategory.sno INNER JOIN products_category ON products_subcategory.category_sno = products_category.sno INNER JOIN branchproducts ON branchmappingtable.SuperBranch = branchproducts.branch_sno AND productsdata.sno = branchproducts.product_sno WHERE (branchmappingtable.SuperBranch = @BranchID) AND (indents_subtable.unitQty <>0) OR (branchdata_1.SalesOfficeID = @SOID) AND (indents_subtable.unitQty <>0) GROUP BY productsdata.sno, BSno, branchmappingtable.SuperBranch ORDER BY branchdata.BranchName");

            if (Session["salestype"].ToString() == "Plant")
            {
                string BranchID = ddlSalesOffice.SelectedValue;
                if (BranchID == "572")
                {
                    BranchID = "158";
                }
                cmd.Parameters.AddWithValue("@BranchID", BranchID);
                cmd.Parameters.AddWithValue("@SOID", BranchID);
            }
            else
            {
                cmd.Parameters.AddWithValue("@BranchID", Session["branch"]);
                cmd.Parameters.AddWithValue("@SOID", Session["branch"]);
            }
            cmd.Parameters.AddWithValue("@starttime", GetLowDate(fromdate.AddDays(-1)));
            cmd.Parameters.AddWithValue("@endtime", GetHighDate(fromdate.AddDays(-1)));
            DataTable dtble = vdm.SelectQuery(cmd).Tables[0];
            DateTime ReportDate = VehicleDBMgr.GetTime(vdm.conn);
            DateTime dtapril = new DateTime();
            DateTime dtmarch = new DateTime();
            int currentyear = ReportDate.Year;
            int nextyear = ReportDate.Year + 1;
            if (ReportDate.Month > 3)
            {
                string apr = "4/1/" + currentyear;
                dtapril = DateTime.Parse(apr);
                string march = "3/31/" + nextyear;
                dtmarch = DateTime.Parse(march);
            }
            if (ReportDate.Month <= 3)
            {
                string apr = "4/1/" + (currentyear - 1);
                dtapril = DateTime.Parse(apr);
                string march = "3/31/" + (nextyear - 1);
                dtmarch = DateTime.Parse(march);
            }
            if (dtble.Rows.Count > 0)
            {
                DataView view = new DataView(dtble);
                Report.Columns.Add("Ledger Type");
                Report.Columns.Add("Customer Code");
                Report.Columns.Add("Customer Name");
                Report.Columns.Add("Invoice Date");
                Report.Columns.Add("Invoce No");
                Report.Columns.Add("HSN CODE");
                Report.Columns.Add("Item Code");
                Report.Columns.Add("Item Name");
                Report.Columns.Add("Qty");
                Report.Columns.Add("Rate");
                Report.Columns.Add("Tax Code");
                Report.Columns.Add("Sales Type");
                Report.Columns.Add("TAX%");
                Report.Columns.Add("Taxable Value");
                Report.Columns.Add("Rounding Off");
                Report.Columns.Add("WH Code");
                Report.Columns.Add("Inv Value");
                Report.Columns.Add("Net Value");
                Report.Columns.Add("Narration");
                int i = 1;
                cmd = new MySqlCommand("SELECT branchdata.whcode,branchdata.sno,branchdata.Branchcode,branchdata.companycode,branchdata.tax,branchdata.ntax,  branchdata.BranchName,branchdata.stateid, statemastar.statename, statemastar.statecode , statemastar.gststatecode FROM branchdata INNER JOIN statemastar ON branchdata.stateid = statemastar.sno WHERE (branchdata.sno = @BranchID)");
                if (Session["salestype"].ToString() == "Plant")
                {
                    cmd.Parameters.AddWithValue("@BranchID", ddlSalesOffice.SelectedValue);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@BranchID", Session["branch"]);
                }
                DataTable dtstatename = vdm.SelectQuery(cmd).Tables[0];
                string statename = "";
                string statecode = "";
                string fromstateid = "";
                string Branchcode = "";
                string gststatecode = "";
                string companycode = "";
                string whcode = "";
                string ntax = "";
                string tax = "";
                if (dtstatename.Rows.Count > 0)
                {
                    Branchcode = dtstatename.Rows[0]["Branchcode"].ToString();
                    statename = dtstatename.Rows[0]["statename"].ToString();
                    statecode = dtstatename.Rows[0]["statecode"].ToString();
                    fromstateid = dtstatename.Rows[0]["stateid"].ToString();
                    gststatecode = dtstatename.Rows[0]["gststatecode"].ToString();
                    companycode = dtstatename.Rows[0]["companycode"].ToString();
                    whcode = dtstatename.Rows[0]["whcode"].ToString();
                    ntax = dtstatename.Rows[0]["ntax"].ToString();
                    tax = dtstatename.Rows[0]["tax"].ToString();
                }
                foreach (DataRow branch in dtble.Rows)
                {

                    if (branch["igst"].ToString() != "0")
                    {
                        DataRow newrow = Report.NewRow();
                        string DCNO = branch["IndentNo"].ToString();
                        whcode = branch["whcode"].ToString();

                        DCNO = Branchcode + "/" + dtapril.ToString("yy") + "-" + dtmarch.ToString("yy") + "T/" + DCNO;


                        double igst = 0;
                        double.TryParse(branch["igst"].ToString(), out igst);
                        string tcategory = "";
                        string TaxCode = "GSTEXEMP";
                        double vatpercent = 0;

                        //NEW
                        newrow["Customer Name"] = branch["tBranchName"].ToString();
                        newrow["Customer Code"] = branch["customercode"].ToString();
                        // newrow["Customer Code"] = branch["customercode"].ToString();
                        newrow["WH Code"] = whcode;
                        newrow["Invoce No"] = DCNO;
                        if (ddlSalesOffice.SelectedValue == "306")
                        {
                            newrow["Invoice Date"] = fromdate.AddDays(1).ToString("dd-MMM-yyyy");
                        }
                        else
                        {
                            newrow["Invoice Date"] = fromdate.ToString("dd-MMM-yyyy");
                        }
                        newrow["HSN CODE"] = branch["hsncode"].ToString();
                        newrow["Item Name"] = branch["tProduct"].ToString();
                        newrow["Item Code"] = branch["Itemcode"].ToString();
                        //newrow["Category Code"] = branch["categorycode"].ToString();
                        double percent = 0;
                        newrow["Qty"] = branch["unitQty"].ToString();
                        double UnitCost = 0;
                        double Unitprice = 0;
                        double.TryParse(branch["UnitCost"].ToString(), out UnitCost);
                        Unitprice = UnitCost;
                        double.TryParse(branch["igst"].ToString(), out igst);
                        float rate = 0;
                        double invval = 0;
                        double qty = 0;
                        double.TryParse(branch["unitQty"].ToString(), out qty);
                        double taxval = 0;
                        float.TryParse(branch["UnitCost"].ToString(), out rate);
                        double tot_vatamount = 0;
                        double PAmount = 0;
                        string tostateid = branch["stateid"].ToString();
                        //NEW CLOSING
                        tcategory = branch["tcategory"].ToString();
                        if (fromstateid == tostateid)
                        {
                            double sgst = 0;
                            double sgstamount = 0;
                            double cgst = 0;
                            double cgstamount = 0;
                            double Igst = 0;
                            double Igstamount = 0;
                            double totRate = 0;
                            double.TryParse(branch["Igst"].ToString(), out Igst);
                            double Igstcon = 100 + Igst;
                            Igstamount = (rate / Igstcon) * Igst;
                            Igstamount = Math.Round(Igstamount, 2);
                            totRate = Igstamount;
                            if (igst == null || igst == 0.0)
                            {
                                tcategory = branch["tcategory"].ToString();
                            }
                            else
                            {
                                tcategory = branch["tcategory"].ToString() + "@" + branch["cgst"].ToString() + "-CGST/SGST-" + Branchcode;
                            }
                            newrow["Ledger Type"] = tcategory.ToString();
                            double Vatrate = rate - totRate;
                            Vatrate = Math.Round(Vatrate, 2);
                            newrow["Rate"] = Vatrate.ToString();
                            PAmount = qty * Vatrate;
                            newrow["Taxable Value"] = Math.Round(PAmount, 2);
                            tot_vatamount = (PAmount * Igst) / 100;
                            sgstamount = (tot_vatamount / 2);
                            sgstamount = Math.Round(sgstamount, 2);
                            if (branch["cgst"].ToString() != "0")
                            {
                                double cgsttax = Convert.ToDouble(branch["cgst"].ToString());
                                cgsttax = cgsttax + cgsttax;
                                TaxCode = "CGST" + cgsttax + "";
                                newrow["TAX%"] =Convert.ToDouble(branch["cgst"].ToString()) + Convert.ToDouble(branch["cgst"].ToString());
                            }
                            newrow["TAX CODE"] = TaxCode;
                            newrow["Sales Type"] = "8";
                            tcategory = branch["tcategory"].ToString() + " " + "@" + " " + "-CGST/SGST-" + Branchcode; ;
                        }
                        else
                        {
                            double Igst = 0;
                            double Igstamount = 0;
                            double totRate = 0;
                            double.TryParse(branch["Igst"].ToString(), out Igst);
                            double Igstcon = 100 + Igst;
                            Igstamount = (rate / Igstcon) * Igst;
                            Igstamount = Math.Round(Igstamount, 2);
                            totRate = Igstamount;
                            if (igst == null || igst == 0.0)
                            {
                                tcategory = branch["tcategory"].ToString();
                            }
                            else
                            {
                                tcategory = branch["tcategory"].ToString() + "@" + branch["Igst"].ToString() + "-IGST-" + Branchcode;
                            }
                            newrow["Ledger Type"] = tcategory.ToString();
                            double Vatrate = rate - totRate;
                            Vatrate = Math.Round(Vatrate, 2);
                            newrow["Rate"] = Vatrate.ToString();
                            PAmount = qty * Vatrate;
                            newrow["Taxable Value"] = Math.Round(PAmount, 2);
                            tot_vatamount = (PAmount * Igst) / 100;
                            newrow["Sales Type"] = "208";
                            if (branch["Igst"].ToString() != "0")
                            {
                                double igsttax = Convert.ToDouble(branch["Igst"].ToString());
                                TaxCode = "IGST" + branch["Igst"].ToString() + "";
                                newrow["TAX%"] = branch["Igst"].ToString();
                            }
                            newrow["TAX CODE"] = TaxCode;
                        }
                        newrow["Tax Code"] = TaxCode.ToString();
                        invval = Math.Round(invval, 2);
                        double netvalue = 0;
                        netvalue = invval + taxval;
                        netvalue = Math.Round(netvalue, 2);
                        double tot_amount = PAmount + tot_vatamount;
                        tot_amount = Math.Round(tot_amount, 2);
                        newrow["Net Value"] = tot_amount;
                        newrow["Narration"] = "Being the sale of milk to  " + branch["tBranchName"].ToString() + " vide DC No " + DCNO + ",DC Date " + fromdate.ToString("dd/MM/yyyy") + ",Emp Name " + Session["EmpName"].ToString();
                        Report.Rows.Add(newrow);
                        i++;
                    }
                    else
                    {
                        string TaxCode = "";
                        DataRow newrow = Report.NewRow();
                        string DCNO = branch["IndentNo"].ToString();
                        whcode = branch["whcode"].ToString();

                        DCNO = Branchcode + "/" + dtapril.ToString("yy") + "-" + dtmarch.ToString("yy") + "N/" + DCNO;
                        newrow["Customer Name"] = branch["tBranchName"].ToString();
                        newrow["Customer Code"] = branch["customercode"].ToString();
                        newrow["WH Code"] = whcode;
                        newrow["Invoce No"] = DCNO;
                        if (ddlSalesOffice.SelectedValue == "306")
                        {
                            newrow["Invoice Date"] = fromdate.AddDays(1).ToString("dd-MMM-yyyy");
                        }
                        else
                        {
                            newrow["Invoice Date"] = fromdate.ToString("dd-MMM-yyyy");
                        }
                        newrow["HSN CODE"] = branch["hsncode"].ToString();
                        newrow["Item Name"] = branch["tProduct"].ToString();
                        newrow["Item Code"] = branch["Itemcode"].ToString();
                        double igst = 0;
                        double.TryParse(branch["igst"].ToString(), out igst);
                        double delqty = 0;
                        double.TryParse(branch["unitQty"].ToString(), out delqty);
                        string tcategory = "";
                        double percent = 0;
                        newrow["Qty"] = branch["unitQty"].ToString();
                        double UnitCost = 0;
                        double Unitprice = 0;
                        double.TryParse(branch["unitQty"].ToString(), out UnitCost);
                        Unitprice = UnitCost;
                        double.TryParse(branch["igst"].ToString(), out igst);
                        float rate = 0;
                        double invval = 0;
                        double qty = 0;
                        double.TryParse(branch["unitQty"].ToString(), out qty);
                        double taxval = 0;
                        float.TryParse(branch["UnitCost"].ToString(), out rate);
                        double tot_vatamount = 0;
                        double PAmount = 0;
                        string tostateid = branch["stateid"].ToString();
                        double Igst = 0;
                        double totRate = 0;
                        double.TryParse(branch["Igst"].ToString(), out Igst);
                        if (igst == null || igst == 0.0)
                        {
                            tcategory = branch["tcategory"].ToString();
                        }
                        newrow["TAX%"] = "0";
                        newrow["Sales Type"] = 208;
                        newrow["Ledger Type"] = tcategory.ToString();
                        newrow["Rate"] = rate.ToString();
                        PAmount = qty * rate;
                        newrow["Taxable Value"] = Math.Round(PAmount, 2);
                        TaxCode = "GSTEXEMP";
                        newrow["TAX CODE"] = TaxCode;
                        newrow["Net Value"] = Math.Round(PAmount,2);
                        newrow["Narration"] = "Being the sale of milk to  " + branch["tBranchName"].ToString() + " vide DC No " + DCNO + ",DC Date " + fromdate.ToString("dd/MM/yyyy") + ",Emp Name " + Session["EmpName"].ToString();
                        Report.Rows.Add(newrow);
                    }
                }
                grdReports.DataSource = Report;
                grdReports.DataBind();
                Session["xportdata"] = Report;

            }
            else
            {
                pnlHide.Visible = false;
                lblmsg.Text = "No Indent Found";
                grdReports.DataSource = Report;
                grdReports.DataBind();
            }
        }
        catch (Exception ex)
        {
            lblmsg.Text = ex.Message;
            grdReports.DataSource = Report;
            grdReports.DataBind();
        }
    }
    private string GetSpace(string p)
    {
        int i = 0;
        for (; i < p.Length; i++)
        {
            if (char.IsNumber(p[i]))
            {
                break;
            }
        }
        return p.Substring(0, i) + " " + p.Substring(i, p.Length - i);
    }
    SqlCommand sqlcmd;
    protected void BtnSave_Click(object sender, EventArgs e)
    {
        try
        {
            vdm = new VehicleDBMgr();
            DateTime CreateDate = VehicleDBMgr.GetTime(vdm.conn);
            SAPdbmanger SAPvdm = new SAPdbmanger();
            DateTime fromdate = DateTime.Now;
            DataTable dt = (DataTable)Session["xportdata"];
            string[] dateFromstrig = txtFromdate.Text.Split(' ');
            if (dateFromstrig.Length > 1)
            {
                if (dateFromstrig[0].Split('-').Length > 0)
                {
                    string[] dates = dateFromstrig[0].Split('-');
                    string[] times = dateFromstrig[1].Split(':');
                    fromdate = new DateTime(int.Parse(dates[2]), int.Parse(dates[1]), int.Parse(dates[0]), int.Parse(times[0]), int.Parse(times[1]), 0);
                }
            }
            DataTable CustomerCodes = new DataTable();
            CustomerCodes.Columns.Add("Ledger Type");
            CustomerCodes.Columns.Add("Customer Code");
            CustomerCodes.Columns.Add("Customer Name");
            CustomerCodes.Columns.Add("Invoice Date");
            CustomerCodes.Columns.Add("Invoce No");
            CustomerCodes.Columns.Add("HSN CODE");
            CustomerCodes.Columns.Add("Item Code");
            CustomerCodes.Columns.Add("Item Name");
            CustomerCodes.Columns.Add("Qty");
            CustomerCodes.Columns.Add("Rate");
            CustomerCodes.Columns.Add("Tax Code");
            CustomerCodes.Columns.Add("Sales Type");
            CustomerCodes.Columns.Add("TAX%");
            CustomerCodes.Columns.Add("Taxable Value");
            CustomerCodes.Columns.Add("Rounding Off");
            CustomerCodes.Columns.Add("WH Code");
            CustomerCodes.Columns.Add("Inv Value");
            CustomerCodes.Columns.Add("Net Value");
            CustomerCodes.Columns.Add("Narration");


            cmd = new MySqlCommand("SELECT sno, BranchName, whcode, ladger_dr_code, tax, ntax, ledger_jv_code FROM branchdata WHERE (sno = @BranchID)");
            cmd.Parameters.AddWithValue("@BranchID", ddlSalesOffice.SelectedValue);
            DataTable dtwhscode = vdm.SelectQuery(cmd).Tables[0];
            ////sqlcmd = new SqlCommand("SELECT CreateDate, CardCode, CardName, TaxDate, DocDate, DocDueDate, DiscPercent, ReferenceNo FROM  EMRORDR WHERE (TaxDate BETWEEN @d1 AND @d2)  AND (WhsCode = @WhsCode) AND CardCode=@CardCode");
            ////sqlcmd.Parameters.Add("@d1", GetLowDate(fromdate));
            ////sqlcmd.Parameters.Add("@d2", GetHighDate(fromdate));
            ////sqlcmd.Parameters.Add("@WhsCode", dtwhscode.Rows[0]["whcode"].ToString());
            ////sqlcmd.Parameters.Add("@CardCode", dtwhscode.Rows[0]["whcode"].ToString());
            DataTable dtOrder = SAPvdm.SelectQuery(sqlcmd).Tables[0];
            if (dtOrder.Rows.Count > 0)
            {
                lblmsg.Text = "This Transaction already saved";
            }
            else
            {
                foreach (DataRow dr in dt.Rows)
                {

                    sqlcmd = new SqlCommand("SELECT CreateDate, CardCode, CardName, TaxDate, DocDate, DocDueDate, DiscPercent, ReferenceNo FROM  EMRORDR WHERE (TaxDate BETWEEN @d1 AND @d2)  AND (WhsCode = @WhsCode) AND CardCode=@CardCode");
                    sqlcmd.Parameters.Add("@d1", GetLowDate(fromdate));
                    sqlcmd.Parameters.Add("@d2", GetHighDate(fromdate));
                    sqlcmd.Parameters.Add("@WhsCode", dtwhscode.Rows[0]["whcode"].ToString());
                    sqlcmd.Parameters.Add("@CardCode", dtwhscode.Rows[0]["whcode"].ToString());


                    string Customercode = dr["Customer Code"].ToString();
                    string whccode = dr["WH Code"].ToString();
                    if (Customercode == "CHN01")
                    {
                        
                    }
                    if (Customercode == "")
                    {
                        DataRow newrow = CustomerCodes.NewRow();
                        newrow["Ledger Type"] = dr["Ledger Type"].ToString();
                        newrow["Customer Name"] = dr["Customer Name"].ToString();
                        newrow["Customer Code"] = dr["Customer Code"].ToString();
                        newrow["Invoice Date"] = dr["Invoice Date"].ToString();
                        newrow["Invoce No"] = dr["Invoce No"].ToString();
                        newrow["HSN CODE"] = dr["HSN CODE"].ToString();
                        newrow["Item Code"] = dr["Item Code"].ToString();
                        newrow["Item Name"] = dr["Item Name"].ToString();
                        newrow["Qty"] = dr["Qty"].ToString();
                        newrow["Rate"] = dr["Rate"].ToString();
                        newrow["Tax Code"] = dr["Tax Code"].ToString();
                        newrow["Sales Type"] = dr["Sales Type"].ToString();
                        newrow["TAX%"] = dr["TAX%"].ToString();
                        newrow["Taxable Value"] = dr["Taxable Value"].ToString();
                        newrow["WH Code"] = dr["WH Code"].ToString();
                        newrow["Net Value"] = dr["Net Value"].ToString();
                        newrow["Narration"] = dr["Narration"].ToString();
                        CustomerCodes.Rows.Add(newrow);
                    }
                    else
                    {
                        if (Customercode.Length >= 8)
                        {

                            string Itemcode = dr["Item Code"].ToString();
                            if (Itemcode == "")
                            {
                            }
                            else
                            {
                                //sqlcmd = new SqlCommand("SELECT CreateDate, CardCode, CardName, TaxDate, DocDate, DocDueDate, DiscPercent, ReferenceNo FROM  EMRORDR WHERE (TaxDate BETWEEN @d1 AND @d2) AND (ReferenceNo = @ReferenceNo) AND (itemcode=@itemcode) AND (WhsCode = @WhsCode)");
                                //sqlcmd.Parameters.Add("@d1", GetLowDate(fromdate));
                                //sqlcmd.Parameters.Add("@d2", GetHighDate(fromdate));
                                //sqlcmd.Parameters.Add("@ReferenceNo", dr["Invoce No"].ToString());
                                //sqlcmd.Parameters.Add("@WhsCode", whccode);
                                //sqlcmd.Parameters.Add("@itemcode", dr["Item Code"].ToString());
                                //DataTable dtSalesOrder = SAPvdm.SelectQuery(sqlcmd).Tables[0];
                                //if (dtSalesOrder.Rows.Count > 0)
                                //{
                                //}
                                //else
                                //{
                                sqlcmd = new SqlCommand("Insert into EMRORDR (cardcode,cardname,TaxDate, DocDate, DocDueDate,dscription,itemcode,quantity,price,whscode,vat_percent,taxamount,ReferenceNo,TaxCode,B1Upload,Processed,CreateDate,REMARKS,SALETYPE) values(@cardcode,@cardname,@TaxDate,@DocDate,@DocDueDate,@dscription,@itemcode,@quantity,@price,@whscode,@vat_percent,@taxamount,@ReferenceNo,@TaxCode,@B1Upload,@Processed,@CreateDate,@REMARKS,@SALETYPE)");
                                sqlcmd.Parameters.Add("@cardcode", dr["Customer Code"].ToString());
                                sqlcmd.Parameters.Add("@cardname", dr["Customer Name"].ToString());
                                sqlcmd.Parameters.Add("@TaxDate", GetLowDate(fromdate));
                                sqlcmd.Parameters.Add("@docdate", GetLowDate(fromdate));
                                sqlcmd.Parameters.Add("@DocDueDate", GetLowDate(fromdate));
                                sqlcmd.Parameters.Add("@dscription", dr["Item Name"].ToString());
                                sqlcmd.Parameters.Add("@itemcode", dr["Item Code"].ToString());
                                sqlcmd.Parameters.Add("@quantity", dr["Qty"].ToString());
                                sqlcmd.Parameters.Add("@price", dr["Rate"].ToString());
                                sqlcmd.Parameters.Add("@whscode", whccode);
                                sqlcmd.Parameters.Add("@vat_percent", dr["TAX%"].ToString());
                                sqlcmd.Parameters.Add("@taxamount", dr["Taxable Value"].ToString());
                                sqlcmd.Parameters.Add("@ReferenceNo", dr["Invoce No"].ToString());
                                string TaxCode = dr["Tax Code"].ToString();
                                string B1Upload = "N";
                                string Processed = "N";
                                sqlcmd.Parameters.Add("@TaxCode", TaxCode);
                                sqlcmd.Parameters.Add("@B1Upload", B1Upload);
                                sqlcmd.Parameters.Add("@Processed", Processed);
                                sqlcmd.Parameters.Add("@CreateDate", CreateDate);
                                sqlcmd.Parameters.Add("@REMARKS", dr["Narration"].ToString());
                                string salestype = dr["Sales Type"].ToString();
                                sqlcmd.Parameters.Add("@SALETYPE", salestype);
                                //SAPvdm.insert(sqlcmd);
                            }
                            //}
                        }
                        else
                        {
                            DataRow newrow1 = CustomerCodes.NewRow();
                            newrow1["Ledger Type"] = dr["Ledger Type"].ToString();
                            newrow1["Customer Name"] = dr["Customer Name"].ToString();
                            newrow1["Customer Code"] = dr["Customer Code"].ToString();
                            newrow1["Invoice Date"] = dr["Invoice Date"].ToString();
                            newrow1["Invoce No"] = dr["Invoce No"].ToString();
                            newrow1["HSN CODE"] = dr["HSN CODE"].ToString();
                            newrow1["Item Code"] = dr["Item Code"].ToString();
                            newrow1["Item Name"] = dr["Item Name"].ToString();
                            newrow1["Qty"] = dr["Qty"].ToString();
                            newrow1["Rate"] = dr["Rate"].ToString();
                            newrow1["Tax Code"] = dr["Tax Code"].ToString();
                            newrow1["Sales Type"] = dr["Sales Type"].ToString();
                            newrow1["TAX%"] = dr["TAX%"].ToString();
                            newrow1["Taxable Value"] = dr["Taxable Value"].ToString();
                            newrow1["WH Code"] = dr["WH Code"].ToString();
                            newrow1["Net Value"] = dr["Net Value"].ToString();
                            newrow1["Narration"] = dr["Narration"].ToString();
                            CustomerCodes.Rows.Add(newrow1);
                        }
                    }
                }
                //pnlHide.Visible = false;
                DataTable dtempty = new DataTable();
                grdReports.DataSource = dtempty;
                grdReports.DataBind();
                grdReports1.DataSource = CustomerCodes;
                grdReports1.DataBind();
                lblmsg.Text = "Successfully Saved";
            }
        }
        catch (Exception ex)
        {
            lblmsg.Text = ex.ToString();
        }
    }
}