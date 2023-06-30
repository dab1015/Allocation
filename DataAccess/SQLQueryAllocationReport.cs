using Microsoft.AspNet.Identity;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json.Linq;
using SNRWMSPortal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc.Routing.Constraints;

namespace SNRWMSPortal.DataAccess
{
    public class SQLQueryAllocationReport
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["SQLConnect"].ToString());

        OleDbConnection conmms = new OleDbConnection(ConfigurationManager.ConnectionStrings["MMSConnect"].ToString());


        public string MSsql;
       // SqlCommand cmd;

        OleDbCommand olecmd;
        OleDbDataReader olereader;

        SqlCommand cmd;
        SqlDataReader reader;



        public List<AllocationSKUModel> NewSKU(string from, string to)
        {
            List<AllocationSKUModel> zerooh = new List<AllocationSKUModel>();
            OleDbCommand cmd = new OleDbCommand($"select A.INUMBR,A.IDESCR,A.IDSCCD,A.IATRB1,(A.isdept || ''-'' || B.dptnam) as Department,A.ISDEPT,A.IMCRDT,A.IVPLTI, A.IVPLHI, A.ISTDPK from mmjdalib.invmst A left join mmjdalib.invdpt B on on A.IDEPT = B.IDEPT WHERE A.IMCRDT BETWEEN {from} and {to}", conmms);
            cmd.CommandType = CommandType.Text;
            OleDbDataAdapter sd = new OleDbDataAdapter(cmd);
            DataTable dt = new DataTable();
            conmms.Open();
            sd.Fill(dt);
            conmms.Close();

            
            foreach (DataRow dr in dt.Rows)
            {
                
                string drRow = dr["IMDATE"].ToString();
                DateTime dateTime = DateTime.ParseExact(drRow, "yyMMdd", CultureInfo.InvariantCulture);
                string formattedDateTime = dateTime.ToString("MM-dd-yyyy", CultureInfo.InvariantCulture);
        
                zerooh.Add(

                new AllocationSKUModel
                {



                    INUMBR = Convert.ToInt32(dr["INUMBR"]),
                    Description = Convert.ToString(dr["IDESCR"]),
                    Status = Convert.ToString(dr["IDSCCD"]),
                    DepartementMMS = Convert.ToString(dr["Department"]),
                    ISDEPT = Convert.ToInt32(dr["ISDEPT"]),
                    IATRB1 = Convert.ToString(dr["IATRB1"]),
                    //IMDATE = Convert.ToString(dr["IMDATE"]),
                    IMDATE = formattedDateTime,
                    Case = Convert.ToDouble(dr["ISTDPK"]),
                    Layer = Convert.ToDouble(dr["IVPLHI"]),
                    Pallet = Convert.ToDouble(dr["IVPLTI"])

                });
            }
            return zerooh;
        }


        public List<AllocationSKUModel> ZeroOnHand()
        {
            List<AllocationSKUModel> zerooh = new List<AllocationSKUModel>();





            OleDbCommand cmd = new OleDbCommand("select C.strname,A.INUMBR,A.IDESCR,A.IDSCCD,A.IDEPT,A.ISDEPT,A.IATRB1,SUM(B.IBHAND + B.IBINTQ) AS OHIT,B.ISTORE from mmjdalib.invmst A" +
                " INNER JOIN mmjdalib.INVBAL B on A.INUMBR = B.INUMBR INNER JOIN (select STRNUM,concat(a.STRNUM1, a.strnam1) as strname from (select STRNUM,concat(strnum, '-') as STRNUM1, replace(strnam,'KAREILA -','') as strnam1 FROM mmjdalib.TBLSTR " +
                "WHERE STRNUM = 201) a)C ON B.ISTORE = C.STRNUM WHERE B.ISTORE = 201 AND B.INUMBR = 77275 GROUP BY C.strname,A.INUMBR,A.IDESCR,A.IDSCCD,A.IDEPT,A.ISDEPT,A.IATRB1,B.IBHAND,B.ISTORE", conmms);
            cmd.CommandType = CommandType.Text;
            OleDbDataAdapter sd = new OleDbDataAdapter(cmd);
            DataTable dt = new DataTable();
            conmms.Open();
            sd.Fill(dt);
            conmms.Close();

            SqlCommand cmd1 = new SqlCommand("select Triggers from AllocationSKU where SKU = 77275 and ClubCode = 201", con);
            cmd1.CommandType = CommandType.Text;
            SqlDataAdapter sd1 = new SqlDataAdapter(cmd1);
            DataTable dt1 = new DataTable();
            con.Open();
            sd1.Fill(dt1);
            con.Close();



            foreach (DataRow dr in dt.Rows)
            {
                foreach (DataRow dr1 in dt1.Rows)
                {
                    zerooh.Add(
                        new AllocationSKUModel
                        {


                            STRNAM = Convert.ToString(dr["strname"]),
                            INUMBR = Convert.ToInt32(dr["INUMBR"]),
                            Description = Convert.ToString(dr["IDESCR"]),
                            Status = Convert.ToString(dr["IDSCCD"]),
                            IDEPT = Convert.ToInt32(dr["IDEPT"]),
                            ISDEPT = Convert.ToInt32(dr["ISDEPT"]),
                            IATRB1 = Convert.ToString(dr["IATRB1"]),
                            Triggers = Convert.ToDouble(dr1["Triggers"]),
                            OHITsum = Convert.ToDouble(dr["OHIT"]),
                            ISTORE = Convert.ToInt32(dr["ISTORE"])



                        });
                }
            }

            return zerooh;


        }


        public List<AllocationSKUModel> GetNewSKUDate(string from,string to)
        {
            try
            {
                List<AllocationSKUModel> datemodel = new List<AllocationSKUModel>();

                MSsql = $"SELECT INUMBR,IDESCR,Department,SubDepartment,IDSCCD,IATRB1,IBALD2,ISTDPK,LAYER,PALLET,IBHAND FROM OPENQUERY(SNR, 'SELECT A.INUMBR,A.IDESCR,Concat(Concat(A.IDEPT,''-''), F.DPTNAM) AS Department,Concat(Concat(A.ISDEPT,''-''), H.DPTNAM) AS SubDepartment,B.IBALD2,A.IDSCCD,A.IATRB1,A.ISTDPK,(A.ISTDPK * A.IVPLTI) AS LAYER,(A.ISTDPK * A.IVPLTI) * A.IVPLHI AS PALLET,B.IBHAND from mmjdalib.invmst A left join MMJDALIB.INVBAL B on A.INUMBR = B.INUMBR LEFT JOIN MMJDALIB.INVDPT F ON A.IDEPT=F.IDEPT AND F.ISDEPT=0 AND F.ICLAS=0 AND F.ISCLAS=0 LEFT JOIN MMJDALIB.INVDPT H ON A.IDEPT=H.IDEPT AND A.ISDEPT=H.ISDEPT AND H.ICLAS=0 AND H.ISCLAS=0 where B.IBHAND > 0 and B.ISTORE BETWEEN 880 AND 893 and  B.IBALD2 between {from} and {to} ') WHERE INUMBR NOT IN (SELECT SKU FROM AllocationSKU)";
                cmd = new SqlCommand(MSsql, con);
                con.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var datelist = new AllocationSKUModel();

                    // string drRow = olereader["DateReceipt"].ToString();
                    // DateTime dateTime = DateTime.ParseExact(drRow, "yyMMdd", CultureInfo.InvariantCulture);
                    //  string formattedDateTime = dateTime.ToString("MM-dd-yyyy", CultureInfo.InvariantCulture);
                    // clublist.INUMBR = Convert.ToInt32(olereader["INUMBR"]);
                    // skuslist.Description = olereader["IDESCR"].ToString();


                    datelist.SKU = Convert.ToInt32(reader["INUMBR"]);
                    datelist.Description = Convert.ToString(reader["IDESCR"]);

                    datelist.DepartementMMS = Convert.ToString(reader["Department"]);
                    //datelist.DepartementMMS = Convert.ToString(olereader["dptnam"]);
                    datelist.SubDepartementMMS = Convert.ToString(reader["SubDepartment"]);
                    datelist.Status = Convert.ToString(reader["IDSCCD"]);
                    datelist.IATRB1 = Convert.ToString(reader["IATRB1"]);
                    datelist.DCInv = Convert.ToDouble(reader["IBHAND"]);
                    //IMDATE = Convert.ToString(dr["IMDATE"]),
                    //datelist.DateModified = formattedDateTime;

                    datelist.DateReceipt = Convert.ToInt32(reader["IBALD2"]);
                    if (datelist.DateReceipt == 0)
                    {
                        datelist.IMDATE = "";
                    }
                    else
                    {
                        string drRow = datelist.DateReceipt.ToString();
                        DateTime dateTime = DateTime.ParseExact(drRow, "yyMMdd", CultureInfo.InvariantCulture);
                        string formattedDateTime = dateTime.ToString("MM/dd/yy", CultureInfo.InvariantCulture);
                        datelist.IMDATE = formattedDateTime;
                    }

                    datelist.Case = Convert.ToDouble(reader["ISTDPK"]);
                    datelist.Layer = Convert.ToDouble(reader["Layer"]);
                    datelist.Pallet = Convert.ToDouble(reader["Pallet"]);
                    datemodel.Add(datelist);

                }
                con.Close();
                return datemodel;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            

        }


        public List<AllocationSKUModel> Daily_Service_Level(string from, string to)
        {
            List<AllocationSKUModel> datemodel = new List<AllocationSKUModel>();

            MSsql = $"select b.ClubCode, Allocated, Served, (case when ((NULLIF(Allocated,0) / NULLIF(Served,0) * 100)) is null then 0 else (NULLIF(Allocated,0) / NULLIF(Served,0) * 100) end) as SKUPercentage, Unserved, AllocatedQty, ServedQty,(case when ((NULLIF(AllocatedQty,0) / NULLIF(ServedQty,0) * 100)) is null then 0 else (NULLIF(AllocatedQty,0) / NULLIF(ServedQty,0) * 100) end) as QtyPercentage from (select clubcode,count(sku) as Allocated, sum(case when status is null then 1 else 0 end) as Unserved, sum(case when status is not null then 1 else 0 end) as Served, sum(RequestedQty) as AllocatedQty, sum(case when status is not null then RequestedQty else 0 end) as ServedQty, DateCreated from CreateAllocationHistory group by clubcode,DateCreated) b left join AllocationClub C on b.ClubCode = C.ClubCode where C.Status = 1 and b.DateCreated BETWEEN '{from}' and '{to}' order by b.ClubCode";
            cmd = new SqlCommand(MSsql, con);
            con.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var datelist = new AllocationSKUModel();

                //string drRow = reader["DateCreated"].ToString();
                //DateTime dateTime = DateTime.ParseExact(drRow, "yyMMdd", CultureInfo.InvariantCulture);
                //string formattedDateTime = dateTime.ToString("MM-dd-yyyy", CultureInfo.InvariantCulture);
                datelist.ClubCode = Convert.ToInt32(reader["ClubCode"]);
                datelist.Allocated = Convert.ToInt32(reader["Allocated"]);
                datelist.Served = Convert.ToInt32(reader["Served"]);
                datelist.SKUPercentage = Convert.ToInt32(reader["SKUPercentage"]);
                datelist.Unserved = Convert.ToInt32(reader["Unserved"]);
                datelist.AllocatedQty = Convert.ToInt32(reader["AllocatedQty"]);
                datelist.ServedQty = Convert.ToInt32(reader["ServedQty"]);
                datelist.QtyPercentage = Convert.ToInt32(reader["QtyPercentage"]);
                //datelist.DateCreated = formattedDateTime;
                
                datemodel.Add(datelist);

            }
            con.Close();
            return datemodel;

        }


        public List<AllocationSKUModel> Summary_Service_Level(string from, string to)
        {
            List<AllocationSKUModel> datemodel = new List<AllocationSKUModel>();

            MSsql = $" select  ALLSKU, ALLQTY,B.DateCreated,(case when ((NULLIF(ALLSKU,0) / NULLIF(NoofSKU,0) * 100)) is null then 0 else (NULLIF(ALLSKU,0) / NULLIF(NoofSKU,0) * 100) end) as SKUPercentage,(case when ((NULLIF(ALLQTY,0) / NULLIF(NoOfQTy,0) * 100)) is null then 0 else (NULLIF(ALLQTY,0) / NULLIF(NoOfQTy,0) * 100) end) as QtyPercentage from (select count(sku) as ALLSKU,sum(requestedqty) as ALLQTY, sum(case when status is not null then 1 else 0 end) as NoofSKU, sum(case when status is not null then RequestedQty else 0 end) as NoOfQTy, DateCreated from CreateAllocationHistory where Status is not null group by DateCreated) b where b.DateCreated BETWEEN '{from}' and '{to}' ";
            cmd = new SqlCommand(MSsql, con);
            con.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var datelist = new AllocationSKUModel();

                //string drRow = reader["DateCreated"].ToString();
                //DateTime dateTime = DateTime.ParseExact(drRow, "yyMMdd", CultureInfo.InvariantCulture);
                //string formattedDateTime = dateTime.ToString("MM-dd-yyyy", CultureInfo.InvariantCulture);
                
                datelist.SKUPercentage = Convert.ToInt32(reader["SKUPercentage"]);
                datelist.DateCreated = Convert.ToString(reader["DateCreated"]);
                datelist.QtyPercentage = Convert.ToInt32(reader["QtyPercentage"]);
                //datelist.DateCreated = formattedDateTime;

                datemodel.Add(datelist);

            }
            con.Close();
            return datemodel;

        }



        public List<AllocationSKUModel> Unserved_SKU(string from, string to)
        {
            List<AllocationSKUModel> datemodel = new List<AllocationSKUModel>();

            MSsql = $"select ClubCode,SKU,DateCreated,Description,SkuStatus,DepartmentName,SubDepartmentName,LI,RequestedQty as AllocatedQty, RequestedQty as UnservedQty, (case when Remarks is null then '' else Remarks end) as Remarks from CreateAllocationHistory where Status is null and DateCreated BETWEEN '{from}' and '{to}' order by ClubCode ";
            cmd = new SqlCommand(MSsql, con);
            con.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var datelist = new AllocationSKUModel();

                //string drRow = reader["DateCreated"].ToString();
                //DateTime dateTime = DateTime.ParseExact(drRow, "yyMMdd", CultureInfo.InvariantCulture);
                //string formattedDateTime = dateTime.ToString("MM-dd-yyyy", CultureInfo.InvariantCulture);
                datelist.ClubCode = Convert.ToInt32(reader["ClubCode"]);
                datelist.SKU = Convert.ToInt32(reader["SKU"]);
                datelist.UnservedDAte = Convert.ToString(reader["DateCreated"]);
                datelist.AllocatedQty = Convert.ToInt32(reader["AllocatedQty"]);
                datelist.UnservedQty = Convert.ToInt32(reader["UnservedQty"]);
                datelist.Remarks = Convert.ToString(reader["Remarks"]);
                datelist.Description = Convert.ToString(reader["Description"]);
                datelist.Status = Convert.ToString(reader["SkuStatus"]);

                datelist.DepartementMMS = Convert.ToString(reader["DepartmentName"]);
                datelist.SubDepartementMMS = Convert.ToString(reader["SubDepartmentName"]);
                datelist.IATRB1 = Convert.ToString(reader["LI"]);
                //datelist.DateCreated = formattedDateTime;
                //var MSsql2 = $"select IDESCR, IDSCCD,IDEPT, IATRB1,ISDEPT from mmjdalib.invmst where INUMBR = {Convert.ToInt32(reader["SKU"])}";
                //olecmd = new OleDbCommand(MSsql2, conmms);
                //conmms.Open();
                //olereader = olecmd.ExecuteReader();
                //while (olereader.Read())
                //{


                //    datelist.Description = Convert.ToString(olereader["IDESCR"]);
                //    datelist.Status = Convert.ToString(olereader["IDSCCD"]);

                //    datelist.IDEPT = Convert.ToInt32(olereader["IDEPT"]);
                //    datelist.ISDEPT = Convert.ToInt32(olereader["ISDEPT"]);
                //    datelist.IATRB1 = Convert.ToString(olereader["IATRB1"]);
                //    //var MSsql3 = $"select (a.isdept || '-' || b.dptnam) as SubDepartment from mmjdalib.invmst a left join mmjdalib.invdpt b on a.isdept = b.isdept  WHERE b.idept = {Convert.ToInt32(olereader["IDEPT"])} and b.iclas = 0 and b.isclas = 0 and a.INUMBR = {Convert.ToInt32(reader["SKU"])}";
                //    //olecmd = new OleDbCommand(MSsql3, conmms);

                //    //olereader = olecmd.ExecuteReader();
                //    //while (olereader.Read())
                //    //{

                //    //    //var MSsql2 = $"select a.IDESCR, a.IDSCCD,a.IDEPT, a.IATRB1, (a.idept || '-' || b.dptnam) as Department from mmjdalib.invmst a left join mmjdalib.invdpt b on a.idept = b.idept  WHERE b.isdept = 0 and b.iclas = 0 and b.isclas = 0 and a.INUMBR = {Convert.ToInt32(reader["SKU"])}";

                //    //    datelist.SubDepartementMMS = Convert.ToString(olereader["SubDepartment"]);



                //    //}

                //}
                //conmms.Close();
                datemodel.Add(datelist);

            }
            con.Close();
            return datemodel;

        }


        public List<AllocationSKUModel> SKU_ClubSetup(string from, string to)
        {
            List<AllocationSKUModel> datemodel = new List<AllocationSKUModel>();

            MSsql = $"select A.ClubCode,A.SKU,(case when A.AverageSales is null then 0 else A.AverageSales end) as AverageSales, B.DConfig, (case when A.NeededOrder is null then 0 else A.NeededOrder end) as NeededOrder,   A.Minimum, (case when A. CurrentMultiplier is null then 0 else A.CurrentMultiplier end) as CurrentMultiplier, (case when A. Multiplier is null then 0 else A.Multiplier end) as Multiplier, A.LeadTime, (case when A.Triggers is null then 0 else A.Triggers end) as Triggers  ,(case when A.BuildTo is null then 0 else A.BuildTo end) as BuildTo, (case when A.Description is null then '' else A.Description end) as Description,(case when A.Status is null then '' else A.Status end) as Status, A.BuildToDF ,A.DateCreated,C.CategoryName,A.UserID from AllocationSKU A left join DistributionConfig B on A.DConfig = B.Code left join TruckLoadCategory C on A.Category = C.Code  where A.DateCreated BETWEEN '{from}' and '{to}' order by A.Clubcode";
            cmd = new SqlCommand(MSsql, con);
            con.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var datelist = new AllocationSKUModel();

                //string drRow = reader["DateCreated"].ToString();
                //DateTime dateTime = DateTime.ParseExact(drRow, "yyMMdd", CultureInfo.InvariantCulture);
                //string formattedDateTime = dateTime.ToString("MM-dd-yyyy", CultureInfo.InvariantCulture);
                datelist.ClubCode = Convert.ToInt32(reader["ClubCode"]);
                datelist.SKU = Convert.ToInt32(reader["SKU"]);
                datelist.AveSalesPerDay = Convert.ToDouble(reader["AverageSales"]);
                datelist.DConfigName = Convert.ToString(reader["DConfig"]);
                datelist.NeededOrder = Convert.ToDouble(reader["NeededOrder"]);
                datelist.Minimum = Convert.ToInt32(reader["Minimum"]);
                datelist.Multiplier = Convert.ToInt32(reader["Multiplier"]);
                datelist.BuildToDF = Convert.ToString(reader["BuildToDF"]);
                datelist.CurrentMultiplier = Convert.ToInt32(reader["CurrentMultiplier"]);
                datelist.LeadTime = Convert.ToInt32(reader["LeadTime"]);
                datelist.Triggers = Convert.ToDouble(reader["Triggers"]);
                datelist.BuildTo = Convert.ToDouble(reader["BuildTo"]);
                datelist.DateCreated = Convert.ToString(reader["DateCreated"]);
                datelist.CategoryName = Convert.ToString(reader["CategoryName"]);
                datelist.Username = Convert.ToString(reader["UserID"]);
                datelist.Description = Convert.ToString(reader["Description"]);
                datelist.Status = Convert.ToString(reader["Status"]);

                //var MSsql2 = $"select IDESCR, IDSCCD from mmjdalib.invmst where INUMBR = {Convert.ToInt32(reader["SKU"])}";
                //olecmd = new OleDbCommand(MSsql2, conmms);
                //conmms.Open();
                //olereader = olecmd.ExecuteReader();
                //while (olereader.Read())
                //{


                //    datelist.Description = Convert.ToString(olereader["IDESCR"]);
                //    datelist.Status = Convert.ToString(olereader["IDSCCD"]);

                    

                //}
                //conmms.Close();
                datemodel.Add(datelist);

            }
            con.Close();
            return datemodel;

        }


        public List<AllocationSKUModel> Club_Request(string from, string to)
        {
            List<AllocationSKUModel> datemodel = new List<AllocationSKUModel>();

            MSsql = $"Select (case when A.ApprovedDate is null then '' else A.ApprovedDate end) as Date ,A.CLubCode,A.SKU,Description,A.Quantity,B.Reason,(case when A.Remarks is null then '' else A.Remarks end) as Remarks,A.UserID from AllocationClubRequest A left join AllocationReason B on A.Reason = B.Code where A.Status = 'APPROVED' and A.ApprovedDate BETWEEN '{from}' and '{to}' order by A.ApprovedDate";
            cmd = new SqlCommand(MSsql, con);
            con.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var datelist = new AllocationSKUModel();

                //string drRow = reader["DateCreated"].ToString();
                //DateTime dateTime = DateTime.ParseExact(drRow, "yyMMdd", CultureInfo.InvariantCulture);
                //string formattedDateTime = dateTime.ToString("MM-dd-yyyy", CultureInfo.InvariantCulture);
                datelist.ClubCode = Convert.ToInt32(reader["ClubCode"]);
                datelist.SKU = Convert.ToInt32(reader["SKU"]);
                datelist.DateCreated = Convert.ToString(reader["Date"]);
                datelist.Description = Convert.ToString(reader["Description"]);
                datelist.Quantity = Convert.ToInt32(reader["Quantity"]);
                datelist.Reason = Convert.ToString(reader["Reason"]);
                datelist.Remarks = Convert.ToString(reader["Remarks"]);
                datelist.Username = Convert.ToString(reader["UserID"]);
                

                
                datemodel.Add(datelist);

            }
            con.Close();
            return datemodel;

        }


        public List<AllocationSKUModel> Allocation_Request(string from, string to)
        {
            List<AllocationSKUModel> datemodel = new List<AllocationSKUModel>();

            MSsql = $"Select  A.DateModified,A.CLubCode,A.SKU,A.Pieces,B.Reason,A.UserID,A.Description from AllocationRequest A left join AllocationReason B on A.Reason = B.Code where A.DateModified BETWEEN '{from}' and '{to}' order by A.DateModified";
            cmd = new SqlCommand(MSsql, con);
            con.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var datelist = new AllocationSKUModel();

                //string drRow = reader["DateCreated"].ToString();
                //DateTime dateTime = DateTime.ParseExact(drRow, "yyMMdd", CultureInfo.InvariantCulture);
                //string formattedDateTime = dateTime.ToString("MM-dd-yyyy", CultureInfo.InvariantCulture);
                datelist.ClubCode = Convert.ToInt32(reader["ClubCode"]);
                datelist.SKU = Convert.ToInt32(reader["SKU"]);
                datelist.DateCreated = Convert.ToString(reader["DateModified"]);
                
                datelist.Quantity = Convert.ToInt32(reader["Pieces"]);
                datelist.Reason = Convert.ToString(reader["Reason"]);
               
                datelist.Username = Convert.ToString(reader["UserID"]);
                datelist.Description = Convert.ToString(reader["Description"]);
                //var MSsql2 = $"select IDESCR from mmjdalib.invmst where INUMBR = {Convert.ToInt32(reader["SKU"])}";
                //olecmd = new OleDbCommand(MSsql2, conmms);
                //conmms.Open();
                //olereader = olecmd.ExecuteReader();
                //while (olereader.Read())
                //{

                //    datelist.Description = Convert.ToString(olereader["IDESCR"]);
                  
                //}
                //conmms.Close();


                datemodel.Add(datelist);

            }
            con.Close();
            return datemodel;

        }

        public List<AllocationSKUModel> Merchandise_Request(string from, string to)
        {
            List<AllocationSKUModel> datemodel = new List<AllocationSKUModel>();

            MSsql = $"Select  DateModified,A.CLubCode,A.SKU,A.Pieces,B.Reason,A.UserID,A.Description from AllocationMerchandise A left join AllocationReason B on A.Reason = B.Code where DateModified BETWEEN '{from}' and '{to}'";
            cmd = new SqlCommand(MSsql, con);
            con.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var datelist = new AllocationSKUModel();

                //string drRow = reader["DateCreated"].ToString();
                //DateTime dateTime = DateTime.ParseExact(drRow, "yyMMdd", CultureInfo.InvariantCulture);
                //string formattedDateTime = dateTime.ToString("MM-dd-yyyy", CultureInfo.InvariantCulture);
                datelist.ClubCode = Convert.ToInt32(reader["ClubCode"]);
                datelist.SKU = Convert.ToInt32(reader["SKU"]);
                datelist.DateCreated = Convert.ToString(reader["DateModified"]);

                datelist.Quantity = Convert.ToInt32(reader["Pieces"]);
                datelist.Reason = Convert.ToString(reader["Reason"]);

                datelist.Username = Convert.ToString(reader["UserID"]);
                datelist.Description = Convert.ToString(reader["Description"]);
                //var MSsql2 = $"select IDESCR from mmjdalib.invmst where INUMBR = {Convert.ToInt32(reader["SKU"])}";
                //olecmd = new OleDbCommand(MSsql2, conmms);
                //conmms.Open();
                //olereader = olecmd.ExecuteReader();
                //while (olereader.Read())
                //{

                //    datelist.Description = Convert.ToString(olereader["IDESCR"]);

                //}
                //conmms.Close();


                datemodel.Add(datelist);

            }
            con.Close();
            return datemodel;

        }


    }
}