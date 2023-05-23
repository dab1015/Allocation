using DocumentFormat.OpenXml.Drawing.Diagrams;
using Microsoft.AspNet.Identity;
using Microsoft.SqlServer.Server;
using Oracle.ManagedDataAccess.Client;
using SNRWMSPortal.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Web;

namespace SNRWMSPortal.DataAccess
{
    public class SQLQueryAllocationMerchandise
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["SQLConnect"].ToString());
        SqlConnection con_UAT = new SqlConnection(ConfigurationManager.ConnectionStrings["SQLConnect_UAT"].ToString());


        OleDbConnection conmms = new OleDbConnection(ConfigurationManager.ConnectionStrings["MMSConnect"].ToString());


        public string MSsql;
        SqlCommand cmd;
        SqlDataReader reader;

        OleDbCommand olecmd;
        OleDbDataReader olereader;

        OracleConnection wms_con = new OracleConnection();
        OracleConnectionStringBuilder ocsb = new OracleConnectionStringBuilder();



       

        public AllocationMerchandiseModel SelectSKU(long _skuId)
        {


            var skuslist = new AllocationMerchandiseModel();

            var keyid = CheckSKUId(_skuId);
            if (keyid == 0)
            {
                return skuslist;
            }
            else
            {

                MSsql = $"SELECT INUMBR, IDESCR FROM MMJDALIB.INVMST WHERE INUMBR  = {_skuId}";
                olecmd = new OleDbCommand(MSsql, conmms);
                conmms.Open();
                olereader = olecmd.ExecuteReader();
                if (olereader.Read())
                {
                    skuslist.SKU = Convert.ToInt32(olereader["INUMBR"]);
                    skuslist.Description = olereader["IDESCR"].ToString();
                    
                }
                conmms.Close();

                MSsql = $"Select SKU,Reason,PIC from  AllocationMerchandise WHERE SKU = {_skuId}";
                cmd = new SqlCommand(MSsql, con);
                con.Open();
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {


                    skuslist.SKU = (int)reader["SKU"];
                    skuslist.Reason = Convert.ToInt32(reader["Reason"]);
                    skuslist.PIC = Convert.ToString(reader["PIC"]);
                    

                }

                con.Close();

                MSsql = $"Select Distinct B.SKU, B.DConfig,C.DConfig as DistributionConfig  from AllocationSKU B left join DistributionConfig C on B.DConfig = C.Code WHERE B.SKU = {_skuId}";
                cmd = new SqlCommand(MSsql, con);
                con.Open();
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {


                    skuslist.SKU = (int)reader["SKU"];
                    skuslist.DConfig = (int)reader["DConfig"];
                    skuslist.DConfigName = Convert.ToString(reader["DistributionConfig"]);

                }

                con.Close();

            }
            
            return skuslist;
        }




        public List<AllocationMerchandiseModel> GetAllocationSKU(int clubcode)
        {

            List<AllocationMerchandiseModel> alloclub = new List<AllocationMerchandiseModel>();
            
            var lpnlist = GetAllocationLPN();
           // var dconfiglist = GetAllocationDConfig();
            
            var lpnval = ""; 
            if(lpnlist.Count == 0)
            {
                lpnval = "0";
            }
           else
            {
                lpnval = string.Join("','", lpnlist.Select(s => s.LPN).ToArray());
            }

            

            MSsql = $"select SKU,Count(SKU) as TopCount from CreateAllocation where Clubcode = {clubcode} and DistributionConfig = 'PALLET' and Status = 3 and SlotLoc is null group by SKU order by sku";
            
            cmd = new SqlCommand(MSsql, con);
           
            con.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
               long sku = Convert.ToInt64(reader["SKU"]);
                int topcount = Convert.ToInt32(reader["TopCount"]);
                //string dconfigname = Convert.ToString(reader["DistributionConfig"]);
                //int dconfival = 0;
                //int dconfival1 = 0;


                
                //=========================================================================//

                ocsb.Password = ConfigurationManager.ConnectionStrings["Password"].ConnectionString;
                ocsb.UserID = ConfigurationManager.ConnectionStrings["UserID"].ConnectionString;
                ocsb.DataSource = ConfigurationManager.ConnectionStrings["WMSConnection1"].ConnectionString;
                wms_con.ConnectionString = ocsb.ConnectionString;
                wms_con.Open();

                OracleCommand command = wms_con.CreateCommand();
                
                string sql = $"select d.prtnum as SKU,b.lodnum as LPN,e.lngdsc as DESCRIPTION,d.untqty as QTY,to_char(d.expire_dte, 'MM/DD/YYYY') as EXPIRY,to_char(d.fifdte, 'MM/DD/YYYY') as RECEIVEDDATE,b.stoloc as LOCATION,a.lvl as LVL,to_date(d.expire_dte) as Orders from locmst a left join invlod b on a.stoloc = b.stoloc left join invsub c on b.lodnum = c.lodnum left join invdtl d on c.subnum = d.subnum left join (select distinct substr(colval, 0, instr(colval, '|KMC|') -1) as prtnum,lngdsc from prtdsc) e on d.prtnum = e.prtnum where a.wh_id = '880' and a.loc_typ_id in ('10022') and d.prtnum in ({sku}) and b.lodnum not in ('{lpnval}') and a.lvl > 1 and (to_date(d.expire_dte) - to_date(d.fifdte) > 60 or d.expire_dte is null)  order by Orders asc fetch first {topcount} rows only "; //Query
                command.CommandText = sql;
                OracleDataReader readers = command.ExecuteReader();
                while (readers.Read())
                {
                    var clublist = new AllocationMerchandiseModel();
                    clublist.INUMBR = Convert.ToInt64(readers["SKU"]);
                    clublist.LPN = Convert.ToString(readers["LPN"]);
                    clublist.SlotLoc = Convert.ToString(readers["LOCATION"]);



                    alloclub.Add(clublist);
                }

                readers.Close();
                wms_con.Close();


                //=========================================================================//


            }

            con.Close();
           
            return alloclub;


        }

        public List<AllocationMerchandiseModel> GetAllocationSKUCase(int clubcode)
        {

            List<AllocationMerchandiseModel> alloclub = new List<AllocationMerchandiseModel>();

            //var lpnlist = GetAllocationLPN();
            //// var dconfiglist = GetAllocationDConfig();

            //var lpnval = "";
            //if (lpnlist.Count == 0)
            //{
            //    lpnval = "0";
            //}
            //else
            //{
            //    lpnval = string.Join("','", lpnlist.Select(s => s.LPN).ToArray());
            //}



            MSsql = $"select SKU,1 as TopCount from CreateAllocation where Clubcode = {clubcode} and" +
                $" (DistributionConfig = 'CASE' or DistributionConfig = 'LAYER') and Status = 3 and SlotLoc is null order by Prioritization";

            cmd = new SqlCommand(MSsql, con);

            con.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                long sku = Convert.ToInt64(reader["SKU"]);
                int topcount = Convert.ToInt32(reader["TopCount"]);
                //string dconfigname = Convert.ToString(reader["DistributionConfig"]);
                //int dconfival = 0;
                //int dconfival1 = 0;



                //=========================================================================//

                ocsb.Password = ConfigurationManager.ConnectionStrings["Password"].ConnectionString;
                ocsb.UserID = ConfigurationManager.ConnectionStrings["UserID"].ConnectionString;
                ocsb.DataSource = ConfigurationManager.ConnectionStrings["WMSConnection1"].ConnectionString;
                wms_con.ConnectionString = ocsb.ConnectionString;
                wms_con.Open();

                OracleCommand command = wms_con.CreateCommand();

                string sql = $"select d.prtnum as SKU,b.lodnum as LPN,e.lngdsc as DESCRIPTION,d.untqty as QTY,to_char(d.expire_dte, 'MM/DD/YYYY') as EXPIRY,to_char(d.fifdte, 'MM/DD/YYYY') as RECEIVEDDATE,b.stoloc as LOCATION,a.lvl as LVL,to_date(d.expire_dte) as Orders from locmst a left join invlod b on a.stoloc = b.stoloc left join invsub c on b.lodnum = c.lodnum left join invdtl d on c.subnum = d.subnum left join (select distinct substr(colval, 0, instr(colval, '|KMC|') -1) as prtnum,lngdsc from prtdsc) e on d.prtnum = e.prtnum where a.wh_id = '880' and a.loc_typ_id in ('10022') and d.prtnum in ({sku}) and a.lvl = 1 and (to_date(d.expire_dte) - to_date(d.fifdte) > 60 or d.expire_dte is null) order by Orders asc fetch first {topcount} rows only "; //Query
                command.CommandText = sql;
                OracleDataReader readers = command.ExecuteReader();
                while (readers.Read())
                {
                    var clublist = new AllocationMerchandiseModel();
                    clublist.INUMBR = Convert.ToInt64(readers["SKU"]);
                    clublist.LPN = Convert.ToString(readers["LPN"]);
                    clublist.SlotLoc = Convert.ToString(readers["LOCATION"]);



                    alloclub.Add(clublist);
                }

                readers.Close();
                wms_con.Close();


                //=========================================================================//


            }

            con.Close();

            return alloclub;


        }


        public List<AllocationMerchandiseModel> GetAllocationClubCode(int clubcode)
        {
            
            DateTime date = DateTime.Now;
            string todaysDate = date.ToString("yyMMdd");

            string batchlist = GetAllocationBatchCode(clubcode);
            

            string batchval = "";
            if(batchlist == "")
            {
                batchval = "A";
            }
            else if(batchlist == "A")
            {
                batchval = "B";
            }
            else if (batchlist == "B")
            {
                batchval = "C";
            }
            else if (batchlist == "C")
            {
                batchval = "D";
            }
            else if (batchlist == "D")
            {
                batchval = "E";
            }
            else if (batchlist == "E")
            {
                batchval = "F";
            }
            else if (batchlist == "F")
            {
                batchval = "G";
            }
            else if (batchlist == "G")
            {
                batchval = "H";
            }
            else if (batchlist == "H")
            {
                batchval = "I";
            }

            int count = 1;
            

            List<AllocationMerchandiseModel> alloclub = new List<AllocationMerchandiseModel>();

            

           MSsql = $"select left(D.ClubName,3) ClubName,A.DistributionConfig,A.ClubCode,A.SKU,A.RequestedQty,B.Description as Prio,A.LPN,A.SlotLoc,A.Prioritization,(case when A.Remarks is null then 'None' else A.Remarks end) as Remarks from CreateAllocation A left join AllocationClub D on D.ClubCode = A.ClubCode left join Prioritization B on A.Prioritization = B.Code where A.ClubCode = {clubcode} and A.OnHand <> 0 and A.Status is null order by A.Prioritization asc";

            cmd = new SqlCommand(MSsql, con);
            con.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var clublist = new AllocationMerchandiseModel();

               
                clublist.ClubName = Convert.ToString(reader["ClubName"] + todaysDate + batchval);
                clublist.ClubCode = Convert.ToInt32(reader["ClubCode"]);
                clublist.SKU = Convert.ToInt64(reader["SKU"]);
                clublist.RequestedQty = Convert.ToInt32(reader["RequestedQty"]);
                clublist.PrioritizationName = Convert.ToString(reader["Prio"]);
               
               clublist.Remarks = Convert.ToString(reader["Remarks"]);
                clublist.LPN = Convert.ToString(reader["LPN"]);
                clublist.SlotLoc = Convert.ToString(reader["SlotLoc"]);
                clublist.Prioritization = Convert.ToInt32(reader["Prioritization"]);

                clublist.DConfigName = Convert.ToString(reader["DistributionConfig"]);
                clublist.Checked = count;

                var MSsql2 = $"select INUMBR,IDESCR,ISTDPK,IVPLTI,IVPLHI,IWGHT from mmjdalib.invmst WHERE INUMBR = {Convert.ToInt64(reader["SKU"])}";
                olecmd = new OleDbCommand(MSsql2, conmms);
                conmms.Open();
                olereader = olecmd.ExecuteReader();
                while (olereader.Read())
                {
                    clublist.INUMBR = Convert.ToInt64(olereader["INUMBR"]);
                    clublist.IDESCR = Convert.ToString(olereader["IDESCR"]);
                    clublist.ISTDPK = Convert.ToDouble(olereader["ISTDPK"]);
                    clublist.IVPLTI = Convert.ToInt32(olereader["IVPLTI"]);
                    clublist.IVPLHI = Convert.ToInt32(olereader["IVPLHI"]);
                    clublist.IWGHT = Convert.ToDouble(olereader["IWGHT"]);

                }
                conmms.Close();

                alloclub.Add(clublist);
            }

            con.Close();
            return alloclub;


        }


        public List<AllocationMerchandiseModel> GetAllocationClubCodeToPick(int clubcode)
        {

            DateTime date = DateTime.Now;
            string todaysDate = date.ToString("yyMMdd");
            string batchlist = GetAllocationBatchCode(clubcode);
            string batchval = "";
            if (batchlist == "")
            {
                batchval = "A";
            }
            else if (batchlist == "A")
            {
                batchval = "B";
            }
            else if (batchlist == "B")
            {
                batchval = "C";
            }
            else if (batchlist == "C")
            {
                batchval = "D";
            }
            else if (batchlist == "D")
            {
                batchval = "E";
            }
            else if (batchlist == "E")
            {
                batchval = "F";
            }
            else if (batchlist == "F")
            {
                batchval = "G";
            }
            
            else if (batchlist == "G")
            {
                batchval = "H";
            }
            else if (batchlist == "H")
            {
                batchval = "I";
            }

            List<AllocationMerchandiseModel> alloclub = new List<AllocationMerchandiseModel>();

            MSsql = $"select left(D.ClubName,3) ClubName,A.DistributionConfig,A.ClubCode,A.SKU,A.RequestedQty,B.Description as Prio,A.Status,C.Description as Stat,A.LPN,A.SlotLoc,Prioritization from CreateAllocation A left join AllocationClub D on D.ClubCode = A.ClubCode left join Prioritization B on A.Prioritization = B.Code left join PickListStatus C on A.Status = C.Code where A.ClubCode = {clubcode} and A.Status = 3 and A.SlotLoc is null order by Prioritization asc";

            cmd = new SqlCommand(MSsql, con);
            con.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var clublist = new AllocationMerchandiseModel();
                clublist.ClubName = Convert.ToString(reader["ClubName"] + todaysDate + batchval);
                clublist.ClubCode = Convert.ToInt32(reader["ClubCode"]);
                clublist.SKU = Convert.ToInt64(reader["SKU"]);
                clublist.RequestedQty = Convert.ToInt32(reader["RequestedQty"]);
                clublist.PrioritizationName = Convert.ToString(reader["Prio"]);
                clublist.StatusName = Convert.ToString(reader["Stat"]);
                clublist.Status = Convert.ToInt32(reader["Status"]);
                clublist.LPN = Convert.ToString(reader["LPN"]);
                clublist.SlotLoc = Convert.ToString(reader["SlotLoc"]);
                clublist.Prioritization = Convert.ToInt32(reader["Prioritization"]);

                clublist.DConfigName = Convert.ToString(reader["DistributionConfig"]);
                var MSsql2 = $"select INUMBR,IDESCR,ISTDPK,IVPLTI,IVPLHI,IWGHT from mmjdalib.invmst  WHERE INUMBR = {Convert.ToInt64(reader["SKU"])}";
                olecmd = new OleDbCommand(MSsql2, conmms);
                conmms.Open();
                olereader = olecmd.ExecuteReader();
                while (olereader.Read())
                {
                    clublist.INUMBR = Convert.ToInt64(olereader["INUMBR"]);
                    clublist.IDESCR = Convert.ToString(olereader["IDESCR"]);
                    clublist.ISTDPK = Convert.ToDouble(olereader["ISTDPK"]);
                    clublist.IVPLTI = Convert.ToInt32(olereader["IVPLTI"]);
                    clublist.IVPLHI = Convert.ToInt32(olereader["IVPLHI"]);
                    clublist.IWGHT = Convert.ToDouble(olereader["IWGHT"]);



                }
                conmms.Close();

                alloclub.Add(clublist);
            }

            con.Close();
            return alloclub;


        }


        public List<AllocationMerchandiseModel> GetAllocationClubCodeToPickReady(int clubcode)
        {

            DateTime date = DateTime.Now;
            string todaysDate = date.ToString("yyMMdd");
            string batchlist = GetAllocationBatchCode(clubcode);
            string batchval = "";
            if (batchlist == "")
            {
                batchval = "A";
            }
            else if (batchlist == "A")
            {
                batchval = "B";
            }
            else if (batchlist == "B")
            {
                batchval = "C";
            }
            else if (batchlist == "C")
            {
                batchval = "D";
            }
            else if (batchlist == "D")
            {
                batchval = "E";
            }
            else if (batchlist == "E")
            {
                batchval = "F";
            }
            else if (batchlist == "F")
            {
                batchval = "G";
            }
            else if (batchlist == "G")
            {
                batchval = "H";
            }



            List<AllocationMerchandiseModel> alloclub = new List<AllocationMerchandiseModel>();

            MSsql = $"select left(D.ClubName,3) ClubName,A.DistributionConfig,A.ClubCode,A.SKU,A.RequestedQty,B.Description as Prio,A.Status,C.Description as Stat,A.LPN,A.SlotLoc,Prioritization,(case when E.Sequence is null then '' else E.Sequence end) as Sequence from CreateAllocation A left join AllocationClub D on D.ClubCode = A.ClubCode left join Prioritization B on A.Prioritization = B.Code left join PickListStatus C on A.Status = C.Code left join PickingSequence E on E.SlotLocation = A.SlotLoc where A.ClubCode = {clubcode} and A.Status = 3 and A.SlotLoc is not null and BatchCode is null order by Prioritization asc";

            cmd = new SqlCommand(MSsql, con);
            con.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var clublist = new AllocationMerchandiseModel();
                clublist.ClubName = Convert.ToString(reader["ClubName"] + todaysDate + batchval);
                clublist.ClubCode = Convert.ToInt32(reader["ClubCode"]);
                clublist.SKU = Convert.ToInt64(reader["SKU"]);
                clublist.RequestedQty = Convert.ToInt32(reader["RequestedQty"]);
                clublist.PrioritizationName = Convert.ToString(reader["Prio"]);
                clublist.StatusName = Convert.ToString(reader["Stat"]);
                clublist.Status = Convert.ToInt32(reader["Status"]);
                clublist.LPN = Convert.ToString(reader["LPN"]);
                clublist.SlotLoc = Convert.ToString(reader["SlotLoc"]);
                clublist.Prioritization = Convert.ToInt32(reader["Prioritization"]);
                clublist.Sequence = Convert.ToString(reader["Sequence"]);
                clublist.DConfigName = Convert.ToString(reader["DistributionConfig"]);
                var MSsql2 = $"select INUMBR,IDESCR,ISTDPK,IVPLTI,IVPLHI from mmjdalib.invmst  WHERE INUMBR = {Convert.ToInt64(reader["SKU"])}";
                olecmd = new OleDbCommand(MSsql2, conmms);
                conmms.Open();
                olereader = olecmd.ExecuteReader();
                while (olereader.Read())
                {
                    clublist.INUMBR = Convert.ToInt64(olereader["INUMBR"]);
                    clublist.IDESCR = Convert.ToString(olereader["IDESCR"]);
                    clublist.ISTDPK = Convert.ToDouble(olereader["ISTDPK"]);
                    clublist.IVPLTI = Convert.ToInt32(olereader["IVPLTI"]);
                    clublist.IVPLHI = Convert.ToInt32(olereader["IVPLHI"]);



                }
                conmms.Close();

                alloclub.Add(clublist);
            }

            con.Close();
            return alloclub;


        }



        public List<AllocationMerchandiseModel> GetAllocationLPN()
        {

            

            List<AllocationMerchandiseModel> alloclub = new List<AllocationMerchandiseModel>();

            MSsql = $"select LPN from CreateAllocation where Status = 3 and lpn is not null";

            cmd = new SqlCommand(MSsql, con);
            con.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var clublist = new AllocationMerchandiseModel();
                clublist.LPN = Convert.ToString(reader["LPN"]);
                

                
                alloclub.Add(clublist);
            }

            con.Close();
            return alloclub;


        }

        public string GetAllocationBatchCode(int clubCode)
        {

            MSsql = $"select top 1 right(BatchCode,1) BatchCode from CreateAllocation where ClubCode = {clubCode} and BatchCode is not null order by id desc";
            cmd = new SqlCommand(MSsql, con);
            con.Open();
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string batch = Convert.ToString(reader["BatchCode"]);
                con.Close();
                return (batch);

            }
            else
            {
                con.Close();
                return "";
            }

            


        }


        public List<DistributionConfig> GetDConfig()
        {
            List<DistributionConfig> configlist = new List<DistributionConfig>();
            SqlCommand cmd = new SqlCommand("SELECT *  From DistributionConfig", con);
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            con.Open();
            sd.Fill(dt);
            con.Close();
            foreach (DataRow dr in dt.Rows)
            {
                configlist.Add(
                    new DistributionConfig
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        Code = Convert.ToInt32(dr["Code"]),
                        Dconfig = Convert.ToString(dr["DConfig"])


                    });
            }
            return configlist;
        }

        public List<AllocationMerchandiseModel> GetReason()
        {
            List<AllocationMerchandiseModel> configlist = new List<AllocationMerchandiseModel>();
            SqlCommand cmd = new SqlCommand("SELECT *  From AllocationReason where Code <> 1 and Code <> 3 and Code <> 4", con);
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            con.Open();
            sd.Fill(dt);
            con.Close();
            foreach (DataRow dr in dt.Rows)
            {
                configlist.Add(
                    new AllocationMerchandiseModel
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        Reason = Convert.ToInt32(dr["Code"]),
                        Reasons = Convert.ToString(dr["Reason"])


                    });
            }
            return configlist;
        }

        public AllocationSKUModel SelectSKUDistribution(int _skuId)
        {


            var skuslist = new AllocationSKUModel();


            MSsql = $"SELECT INUMBR,IVPLTI,IVPLHI,ISTDPK FROM MMJDALIB.INVMST WHERE INUMBR  = {_skuId}";
            olecmd = new OleDbCommand(MSsql, conmms);
            conmms.Open();
            olereader = olecmd.ExecuteReader();
            if (olereader.Read())
            {
               
                skuslist.INUMBR = (int)olereader["INUMBR"];
                skuslist.IVPLTI = (int)olereader["IVPLTI"];
                skuslist.IVPLHI = (int)olereader["IVPLHI"];
                skuslist.ISTDPK = (double)olereader["ISTDPK"];

            }
            conmms.Close();
            return skuslist;
        }



        public List<AllocationMerchandiseModel> GetClubSKU(long _skuId)
        {

            List<AllocationMerchandiseModel> alloclub = new List<AllocationMerchandiseModel>();
           
            MSsql = $"select C.clubcode,concat(C.ClubCode,'-',C.ClubName) as ClubNames,(case when D.sku is null then {_skuId} else D.sku end) as sku, (case when D.Pieces is null then 0 else D.Pieces end) as Pieces, (case when D.DistributionConfig is null then '' else D.DistributionConfig end) as DistributionConfig,(case when D.Reason is null then 0 else D.Reason end) as Reason from AllocationClub C left outer join(select A.sku, A.ClubCode,A.Pieces,A.DistributionConfig,A.Reason from AllocationMerchandise A left join (select ClubCode from AllocationClub ) B on A.ClubCode = b.ClubCode  where A.sku = {_skuId})  D on C.ClubCode = D.ClubCode where C.Status = 1 group by C.ClubCode,D.SKU,D.Pieces,D.DistributionConfig,D.Reason,C.ClubName order by C.ClubCode asc";
             
            cmd = new SqlCommand(MSsql, con);
            con.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var clublist = new AllocationMerchandiseModel();
              
                clublist.ClubName = Convert.ToString(reader["ClubNames"]);
                clublist.ClubCode = Convert.ToInt32(reader["ClubCode"]);
                clublist.SKU = Convert.ToInt64(reader["SKU"]);
                clublist.ClubCode = Convert.ToInt32(reader["ClubCode"]);
                clublist.Pieces = Convert.ToInt32(reader["Pieces"]);
                clublist.Reason = Convert.ToInt32(reader["Reason"]);
                clublist.DConfigName = Convert.ToString(reader["DistributionConfig"]);
                



                alloclub.Add(clublist);
            }

            con.Close();
            return alloclub;


        }


        public AllocationMerchandiseModel SKUOnHandDCM(long _skuId)
        {

            var skuslist = new AllocationMerchandiseModel();



            MSsql = $"select INUMBR,IBHAND from mmjdalib.invbal where INUMBR = {_skuId} and ISTORE = 880";
            olecmd = new OleDbCommand(MSsql, conmms);
            conmms.Open();
            olereader = olecmd.ExecuteReader();
            if (olereader.Read())
            {
                skuslist.SKU = Convert.ToInt32(olereader["INUMBR"]);
                skuslist.DCMOH= Convert.ToInt32(olereader["IBHAND"]);
            }
            conmms.Close();

            return skuslist;
        }
        public AllocationMerchandiseModel SKUOnHandDCL(long _skuId)
        {

            var skuslist = new AllocationMerchandiseModel();



            MSsql = $"select INUMBR,IBHAND from mmjdalib.invbal where INUMBR = {_skuId} and ISTORE = 881";
            olecmd = new OleDbCommand(MSsql, conmms);
            conmms.Open();
            olereader = olecmd.ExecuteReader();
            if (olereader.Read())
            {
                skuslist.SKU = Convert.ToInt32(olereader["INUMBR"]);
                skuslist.DCLOH = Convert.ToInt32(olereader["IBHAND"]);
            }
            conmms.Close();

            return skuslist;
        }
        public AllocationMerchandiseModel SKUOnHandDCP(long _skuId)
        {

            var skuslist = new AllocationMerchandiseModel();



            MSsql = $"select INUMBR,IBHAND from mmjdalib.invbal where INUMBR = {_skuId} and ISTORE = 882";
            olecmd = new OleDbCommand(MSsql, conmms);
            conmms.Open();
            olereader = olecmd.ExecuteReader();
            if (olereader.Read())
            {
                skuslist.SKU = Convert.ToInt32(olereader["INUMBR"]);
                skuslist.DCPOH = Convert.ToInt32(olereader["IBHAND"]);
            }
            conmms.Close();

            return skuslist;
        }
        public AllocationMerchandiseModel SKUOnHandDCB(long _skuId)
        {

            var skuslist = new AllocationMerchandiseModel();



            MSsql = $"select INUMBR,IBHAND from mmjdalib.invbal where INUMBR = {_skuId} and ISTORE = 889";
            olecmd = new OleDbCommand(MSsql, conmms);
            conmms.Open();
            olereader = olecmd.ExecuteReader();
            if (olereader.Read())
            {
                skuslist.SKU = Convert.ToInt32(olereader["INUMBR"]);
                skuslist.DCBOH = Convert.ToInt32(olereader["IBHAND"]);
            }
            conmms.Close();

            return skuslist;
        }




        public long SearchKeyID(long SKUId)
        {
            MSsql = $"SELECT Id FROM AllocationSKUDes WHERE SKU = {SKUId}";
            cmd = new SqlCommand(MSsql, con);
            con.Open();
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                long skuId = Convert.ToInt64(reader["Id"]);
                con.Close();
                return (skuId);
            }
            else
            {
                con.Close();
                return 0;
            }
        }

        public List<AllocationClub> GetClubSQL()
        {
            List<AllocationClub> clublist = new List<AllocationClub>();
            cmd = new SqlCommand("select concat(ClubCode, '-',ClubName) as ClubCodes,ClubCode FROM AllocationClub where Status = 1", con);
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            con.Open();
            sd.Fill(dt);
            con.Close();
            foreach (DataRow dr in dt.Rows)
            {
                clublist.Add(
                    new AllocationClub
                    {
                        ClubName = dr["ClubCodes"].ToString(),
                        CLubCode = Convert.ToInt32(dr["ClubCode"])
                        //STRNAM = Convert.ToString(dr["STRNAM"]),
                        // STSDAT = Convert.ToInt32(dr["STSDAT"])


                    });
            }
            return clublist;
        }

        public int CheckSKUId(long skuid)
        {
            string SqlConnectionMMS = ConfigurationManager.ConnectionStrings["MMSConnect"].ConnectionString;
            OleDbConnection MMSsqlConnect = new OleDbConnection(SqlConnectionMMS);

            string sql = $"SELECT INUMBR FROM MMJDALIB.INVMST WHERE INUMBR = {skuid}";
            OleDbCommand sqlCommand = new OleDbCommand(sql, MMSsqlConnect);
            MMSsqlConnect.Open();
            OleDbDataReader sqlDataReader = sqlCommand.ExecuteReader();
            if (sqlDataReader.Read())
            {
                int skuId = Convert.ToInt32(sqlDataReader["INUMBR"]);
                MMSsqlConnect.Close();
                return (skuId);
            }
            else
            {
                MMSsqlConnect.Close();
                return 0;
            }
        }

        public void UpdateMerchan(int SKU, int ClubCode, int Pieces,int Reason, string PIC,string UserID,string Todays, string DConfig,string Description)
        {
            DateTime formattedday = DateTime.Parse(Todays);

            MSsql = "UPDATE [AllocationMerchandise] SET Pieces = @Pieces, Reason = @Reason, PIC = @PIC, UserID = @UserID, DateModified = @Todays,DistributionConfig = @DConfig,Description = @Description where SKU = @SKU and ClubCode = @ClubCode";
            con.Open();
            cmd = new SqlCommand(MSsql, con);

            cmd.Parameters.Add(new SqlParameter()
            {
                Value = ClubCode,
                ParameterName = "@ClubCode",
                DbType = DbType.Int32
            });

            cmd.Parameters.Add(new SqlParameter()
            {
                Value = SKU,
                ParameterName = "@SKU",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = Pieces,
                ParameterName = "@Pieces",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = Reason,
                ParameterName = "@Reason",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = PIC,
                ParameterName = "@PIC",
                DbType = DbType.String
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = UserID,
                ParameterName = "@UserID",
                DbType = DbType.String
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = formattedday,
                ParameterName = "@Todays",
                DbType = DbType.DateTime
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = DConfig,
                ParameterName = "@DConfig",
                DbType = DbType.String
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = Description,
                ParameterName = "@Description",
                DbType = DbType.String
            });



            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void UpdateMerchanAllocation(int SKU, int ClubCode, int Pieces,string ReasonName, int Prioritization,double DCMOH, string Todays)
        {

            DateTime formattedday = DateTime.Parse(Todays);

            MSsql = "UPDATE [CreateAllocation] SET RequestedQty = @Pieces,Remarks = @ReasonName,OnHand = @DCMOH, DateCreated = @Todays  where SKU = @SKU and ClubCode = @ClubCode and Prioritization = @Prioritization";
            con.Open();
            cmd = new SqlCommand(MSsql, con);

            cmd.Parameters.Add(new SqlParameter()
            {
                Value = ClubCode,
                ParameterName = "@ClubCode",
                DbType = DbType.Int32
            });

            cmd.Parameters.Add(new SqlParameter()
            {
                Value = SKU,
                ParameterName = "@SKU",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = Pieces,
                ParameterName = "@Pieces",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = ReasonName,
                ParameterName = "@ReasonName",
                DbType = DbType.String
            });


            cmd.Parameters.Add(new SqlParameter()
            {
                Value = Prioritization,
                ParameterName = "@Prioritization",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = DCMOH,
                ParameterName = "@DCMOH",
                DbType = DbType.Double
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = formattedday,
                ParameterName = "@Todays",
                DbType = DbType.DateTime
            });




            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void DeleteMerchanAllocation(int SKU, int ClubCode, int Prioritization)
        {

           

            MSsql = "Delete from CreateAllocation  where SKU = @SKU and ClubCode = @ClubCode and Prioritization = @Prioritization and  RequestedQty = 0";
            con.Open();
            cmd = new SqlCommand(MSsql, con);

            cmd.Parameters.Add(new SqlParameter()
            {
                Value = ClubCode,
                ParameterName = "@ClubCode",
                DbType = DbType.Int32
            });

            cmd.Parameters.Add(new SqlParameter()
            {
                Value = SKU,
                ParameterName = "@SKU",
                DbType = DbType.Int32
            });
            

            cmd.Parameters.Add(new SqlParameter()
            {
                Value = Prioritization,
                ParameterName = "@Prioritization",
                DbType = DbType.Int32
            });
           




            cmd.ExecuteNonQuery();
            con.Close();
        }


        public void DeleteRemainingZero()
        {



            MSsql = "Delete from CreateAllocation where RequestedQty <= 0 and Status is null";
            con.Open();
            cmd = new SqlCommand(MSsql, con);

            cmd.ExecuteNonQuery();
            con.Close();
        }

        public bool InsertMerchan(int SKU,int ClubCode, int Pieces, int Reason, string PIC,string UserID,string Todays,string DConfig,string Description)
        {

            try
            {
                DateTime formattedday = DateTime.Parse(Todays);

                SqlCommand cmd = new SqlCommand("INSERT AllocationMerchandise(SKU,ClubCode,Pieces,Reason,PIC,UserID,DateModified,DistributionConfig,Description) VALUES(@SKU,@ClubCode,@Pieces,@Reason,@PIC,@UserID,@Todays,@DConfig,@Description)", con);
                cmd.CommandType = CommandType.Text;
                con.Open();

                cmd.Parameters.AddWithValue("@SKU", SKU);
                cmd.Parameters.AddWithValue("@ClubCode", ClubCode);
                cmd.Parameters.AddWithValue("@Pieces", Pieces);
                cmd.Parameters.AddWithValue("@Reason", Reason);
                cmd.Parameters.AddWithValue("@PIC", PIC);
                cmd.Parameters.AddWithValue("@UserID", UserID);
                cmd.Parameters.AddWithValue("@Todays", formattedday);
                cmd.Parameters.AddWithValue("@DConfig", DConfig);
                cmd.Parameters.AddWithValue("@Description", Description);
                cmd.ExecuteNonQuery();

                con.Close();
                return true;


            }
            catch (Exception)
            {
                con.Close();
                return false;

            }

        }

        public bool InsertMerchanAllocation(int SKU, int ClubCode, int Pieces, string ReasonName, int Prioritization,double DCMOH, string Todays, string DConfig)
        {

            try
            {
                DateTime formattedday = DateTime.Parse(Todays);

                SqlCommand cmd = new SqlCommand("INSERT CreateAllocation(SKU,ClubCode,RequestedQty,Remarks,Prioritization,OnHand,DateCreated,DistributionConfig) VALUES(@SKU,@ClubCode,@Pieces,@ReasonName,@Prioritization,@DCMOH,@Todays,@DConfig)", con);
                cmd.CommandType = CommandType.Text;
                con.Open();

                cmd.Parameters.AddWithValue("@SKU", SKU);
                cmd.Parameters.AddWithValue("@ClubCode", ClubCode);
                cmd.Parameters.AddWithValue("@Pieces", Pieces);
                cmd.Parameters.AddWithValue("@ReasonName", ReasonName);
               
                cmd.Parameters.AddWithValue("@Prioritization", Prioritization);
                cmd.Parameters.AddWithValue("@DCMOH", DCMOH);
                cmd.Parameters.AddWithValue("@Todays", formattedday);
                cmd.Parameters.AddWithValue("@DConfig", DConfig);
                cmd.ExecuteNonQuery();

                con.Close();
                return true;


            }
            catch (Exception)
            {
                con.Close();
                return false;

            }

        }

        public bool VerifyMerchan(int skucode, int clubcode)
        {
            MSsql = $"SELECT SKU,Clubcode from AllocationMerchandise where SKU = @skucode and ClubCode = @clubcode";
            cmd = new SqlCommand(MSsql, con);
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = skucode,
                ParameterName = "@skucode",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = clubcode,
                ParameterName = "@clubcode",
                DbType = DbType.Int32
            });
            cmd.CommandText = MSsql;
            con.Open();

            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Close();
                con.Close();

                return true;
            }
            else
            {
                reader.Close();
                con.Close();
                return false;
            }
        }

        public bool VerifyMerchanAllocation(int skucode, int clubcode, int prio)
        {
            MSsql = $"SELECT SKU,Clubcode,Prioritization from CreateAllocation where SKU = @skucode and ClubCode = @clubcode and Prioritization = @prio ";
            cmd = new SqlCommand(MSsql, con);
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = skucode,
                ParameterName = "@skucode",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = clubcode,
                ParameterName = "@clubcode",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = prio,
                ParameterName = "@prio",
                DbType = DbType.Int32
            });
            cmd.CommandText = MSsql;
            con.Open();

            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Close();
                con.Close();

                return true;
            }
            else
            {
                reader.Close();
                con.Close();
                return false;
            }
        }


        public bool VerifyInsertPiclist(int skucode, int clubcode, int prio)
        {
            MSsql = $"SELECT SKU,Clubcode from CreateAllocation where SKU = @skucode and ClubCode = @clubcode and Prioritization = @Prioritization and Status = 3 and Slotloc is not null";
            cmd = new SqlCommand(MSsql, con);
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = skucode,
                ParameterName = "@skucode",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = clubcode,
                ParameterName = "@clubcode",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = prio,
                ParameterName = "@Prioritization",
                DbType = DbType.Int32
            });
            
            cmd.CommandText = MSsql;
            con.Open();

            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Close();
                con.Close();

                return true;
            }
            else
            {
                reader.Close();
                con.Close();
                return false;
            }
        }

        public void UpdateAllocatedStatus(string BatchCode,int ClubCode,int SKU,int Prio)
        {
            

            MSsql = "UPDATE [CreateAllocation] SET BatchCode = @BatchCode,Status = 3 where SKU = @SKU and ClubCode = @ClubCode and Prioritization = @Prio and Status = 3 and Slotloc is not null";
            con.Open();
            cmd = new SqlCommand(MSsql, con);

            cmd.Parameters.Add(new SqlParameter()
            {
                Value = ClubCode,
                ParameterName = "@ClubCode",
                DbType = DbType.Int32
            });

            cmd.Parameters.Add(new SqlParameter()
            {
                Value = SKU,
                ParameterName = "@SKU",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = Prio,
                ParameterName = "@Prio",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = BatchCode,
                ParameterName = "@BatchCode",
                DbType = DbType.String
            });






            cmd.ExecuteNonQuery();
            con.Close();
        }


        public void UpdateAllocatedBatchCode(string BatchCode,int ClubCode,int SKU)
        {


            MSsql = "UPDATE [CreateAllocation] SET BatchCode = @BatchCode where ClubCode = @ClubCode and SKU = @SKU and Priritization = @Prio and Slotloc is not null";
            con.Open();
            cmd = new SqlCommand(MSsql, con);

            cmd.Parameters.Add(new SqlParameter()
            {
                Value = ClubCode,
                ParameterName = "@ClubCode",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = SKU,
                ParameterName = "@SKU",
                DbType = DbType.Int32
            });

            cmd.Parameters.Add(new SqlParameter()
            {
                Value = BatchCode,
                ParameterName = "@BatchCode",
                DbType = DbType.String
            });
           
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void UpdateAllocatedReady(int ClubCode, int SKU, double RequestedQty,int Prio)
        {


            MSsql = "UPDATE [CreateAllocation] SET RequestedQty = @RequestedQty where ClubCode = @ClubCode and SKU = @SKU and Prioritization = @Prio and Status is null";
            con.Open();
            cmd = new SqlCommand(MSsql, con);

            cmd.Parameters.Add(new SqlParameter()
            {
                Value = ClubCode,
                ParameterName = "@ClubCode",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = SKU,
                ParameterName = "@SKU",
                DbType = DbType.Int32
            });

            cmd.Parameters.Add(new SqlParameter()
            {
                Value = Prio,
                ParameterName = "@Prio",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = RequestedQty,
                ParameterName = "@RequestedQty",
                DbType = DbType.Double
            });

            cmd.ExecuteNonQuery();
            con.Close();
        }

        //  batchcode,clubcode, skucode,description, reqty,prioname, slotloc, lpn, qtytopick, qtypicked, dconfig, stdpk, status,userName, //todaysDate
        public bool InsertPicklist(string BatchCode,int ClubCode, int SKU, string Description, int ReqQty,string Remarks, string SlotLoc, string LPN, int QtyToPick, int QtyPicked, string DConfig, double STDPK,int Status,string Sequence,string UserName, string Todays)
        {

            try
            {
                DateTime formattedday = DateTime.Parse(Todays);

                SqlCommand cmd = new SqlCommand("INSERT Picklist(BatchCode,StoreCode,SKU,ItemDesc,QtyPcs,QtyPcsEdited,Remarks,Status,DistributionConfig,STDPK,QtyToPick,QtyPicked,LPN,SlotLoc, PickSeq, UserName,DateCreated) VALUES(@BatchCode,@ClubCode,@SKU,@Description,@ReqQty,@QtyPiecesEdited,@Remarks,@Status,@DistributionConfig,@STDPK,@QtyToPick,@QtyPicked,@LPN,@SlotLoc,@Sequence,@UserName,@DateCreated)", con_UAT);
                cmd.CommandType = CommandType.Text;
                con_UAT.Open();
                cmd.Parameters.AddWithValue("@BatchCode", BatchCode);
                cmd.Parameters.AddWithValue("@ClubCode", ClubCode);
                cmd.Parameters.AddWithValue("@SKU", SKU);
                
                cmd.Parameters.AddWithValue("@Description", Description);
                cmd.Parameters.AddWithValue("@ReqQty", ReqQty);
                cmd.Parameters.AddWithValue("@QtyPiecesEdited", ReqQty);
                cmd.Parameters.AddWithValue("@Remarks", Remarks);
                cmd.Parameters.AddWithValue("@Status", Status);
                cmd.Parameters.AddWithValue("@DistributionConfig", DConfig);
                cmd.Parameters.AddWithValue("@STDPK", STDPK);
               
                cmd.Parameters.AddWithValue("@QtyToPick", QtyToPick);
                cmd.Parameters.AddWithValue("@QtyPicked", QtyPicked);
                cmd.Parameters.AddWithValue("@LPN", LPN);
                cmd.Parameters.AddWithValue("@SlotLoc", SlotLoc);
                cmd.Parameters.AddWithValue("@Sequence", Sequence);
                cmd.Parameters.AddWithValue("@UserName", UserName);
                cmd.Parameters.AddWithValue("@DateCreated", formattedday);
                cmd.ExecuteNonQuery();

                con_UAT.Close();
                return true;


            }
            catch (Exception)
            {
                con_UAT.Close();
                return false;

            }

        }


        public bool InsertCreateAllocation(int ClubCode, int SKU, double ReqQty, int Status, string DConfig, int Prio,string Remarks, string Todays)
        {

            try
            {
                DateTime formattedday = DateTime.Parse(Todays);

                SqlCommand cmd = new SqlCommand("INSERT CreateAllocation(clubCode,SKU,RequestedQty,Status,DistributionConfig,Prioritization,Remarks,DateCreated) VALUES(@ClubCode,@SKU,@ReqQty,@Status,@DistributionConfig,@Prio,@Remarks,@DateCreated)", con);
                cmd.CommandType = CommandType.Text;
                con.Open();
               
                cmd.Parameters.AddWithValue("@ClubCode", ClubCode);
                cmd.Parameters.AddWithValue("@SKU", SKU);

                
                cmd.Parameters.AddWithValue("@ReqQty", ReqQty);
                cmd.Parameters.AddWithValue("@Status", Status);
                
                
                cmd.Parameters.AddWithValue("@DistributionConfig", DConfig);
                cmd.Parameters.AddWithValue("@Prio", Prio);
                cmd.Parameters.AddWithValue("@Remarks", Remarks);


                cmd.Parameters.AddWithValue("@DateCreated", formattedday);
                cmd.ExecuteNonQuery();

                con.Close();
                return true;


            }
            catch (Exception)
            {
                con.Close();
                return false;

            }

        }


        public bool VerifyUpdateSlotLoc(int skucode, int clubcode)
        {
            MSsql = $"select * FROM CreateAllocation where SKU = {skucode} and Clubcode = {clubcode} and SLotLoc is null order by Prioritization";
            cmd = new SqlCommand(MSsql, con);
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = skucode,
                ParameterName = "@skucode",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = clubcode,
                ParameterName = "@clubcode",
                DbType = DbType.Int32
            });
            
            cmd.CommandText = MSsql;
            con.Open();

            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Close();
                con.Close();

                return true;
            }
            else
            {
                reader.Close();
                con.Close();
                return false;
            }
        }

        public bool VerifyUpdateSlotLocStop(int skucode, int clubcode,int prio)
        {
            MSsql = $"select * FROM CreateAllocation where SKU = {skucode} and Clubcode = {clubcode} and Prioritization = @Prio and Status IS NULL order by Prioritization";
            cmd = new SqlCommand(MSsql, con);
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = skucode,
                ParameterName = "@skucode",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = clubcode,
                ParameterName = "@clubcode",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = prio,
                ParameterName = "@prio",
                DbType = DbType.Int32
            });

            cmd.CommandText = MSsql;
            con.Open();

            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Close();
                con.Close();

                return true;
            }
            else
            {
                reader.Close();
                con.Close();
                return false;
            }
        }

        public void UpdateSlotLoc(int ClubCode, int SKU, string LPN, string SlotLoc)
        {
            

            MSsql = "UPDATE [CreateAllocation] SET LPN = @LPN, SlotLoc = @SlotLoc where id = (select top 1 id FROM [CreateAllocation] where SKU = @SKU and Clubcode = @ClubCode and SLotLoc is null and Status = 3 order by Prioritization asc)";
            con.Open();
            cmd = new SqlCommand(MSsql, con);

            cmd.Parameters.Add(new SqlParameter()
            {
                Value = ClubCode,
                ParameterName = "@ClubCode",
                DbType = DbType.Int32
            });

            cmd.Parameters.Add(new SqlParameter()
            {
                Value = SKU,
                ParameterName = "@SKU",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = LPN,
                ParameterName = "@LPN",
                DbType = DbType.String
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = SlotLoc,
                ParameterName = "@SlotLoc",
                DbType = DbType.String
            });
            




            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void UpdateSlotLocStop(int ClubCode, int SKU,int ReqQty,int Prio)
        {


            MSsql = "UPDATE [CreateAllocation] SET RequestedQty = @ReqQty where SKU = @SKU and Clubcode = @ClubCode and Prioritization = @Prio and Status IS NULL";
            con.Open();
            cmd = new SqlCommand(MSsql, con);

            cmd.Parameters.Add(new SqlParameter()
            {
                Value = ClubCode,
                ParameterName = "@ClubCode",
                DbType = DbType.Int32
            });

            cmd.Parameters.Add(new SqlParameter()
            {
                Value = SKU,
                ParameterName = "@SKU",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = Prio,
                ParameterName = "@Prio",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = ReqQty,
                ParameterName = "@ReqQty",
                DbType = DbType.String
            });
            




            cmd.ExecuteNonQuery();
            con.Close();
        }


        public void DeleteSlotLocStop(int ClubCode, int SKU, int Prio)
        {


            MSsql = "Delete from [CreateAllocation]  where SKU = @SKU and Clubcode = @ClubCode and Prioritization = @Prio and Status = 3 and BatchCode is null";
            con.Open();
            cmd = new SqlCommand(MSsql, con);

            cmd.Parameters.Add(new SqlParameter()
            {
                Value = ClubCode,
                ParameterName = "@ClubCode",
                DbType = DbType.Int32
            });

            cmd.Parameters.Add(new SqlParameter()
            {
                Value = SKU,
                ParameterName = "@SKU",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = Prio,
                ParameterName = "@Prio",
                DbType = DbType.Int32
            });
            





            cmd.ExecuteNonQuery();
            con.Close();
        }



        public void DeleteStatusNull(int ClubCode,int SKUID)
        {


            MSsql = "Delete from [CreateAllocation]  where  Clubcode = @ClubCode and SKU = @SKUID and Status is null";
            con.Open();
            cmd = new SqlCommand(MSsql, con);

            cmd.Parameters.Add(new SqlParameter()
            {
                Value = ClubCode,
                ParameterName = "@ClubCode",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = SKUID,
                ParameterName = "@SKUID",
                DbType = DbType.Int32
            });


            cmd.ExecuteNonQuery();
            con.Close();
        }


        public void DeleteStatus3andNullSlotLoc(int ClubCode)
        {


            MSsql = "Delete from [CreateAllocation]  where  Clubcode = @ClubCode and Status = 3 and BatchCode is null";
            con.Open();
            cmd = new SqlCommand(MSsql, con);

            cmd.Parameters.Add(new SqlParameter()
            {
                Value = ClubCode,
                ParameterName = "@ClubCode",
                DbType = DbType.Int32
            });
           


            cmd.ExecuteNonQuery();
            con.Close();
        }





        public void UpdateStatus3(int ClubCode)
        {


            MSsql = "Update [CreateAllocation] set Status = NULL where  ClubCode = @ClubCode and Status = 3 and SlotLoc is null";
            con.Open();
            cmd = new SqlCommand(MSsql, con);

            cmd.Parameters.Add(new SqlParameter()
            {
                Value = ClubCode,
                ParameterName = "@ClubCode",
                DbType = DbType.Int32
            });
            

            cmd.ExecuteNonQuery();
            con.Close();
        }

       


        public int CheckQty(int clubcode,int sku,int prio)
        {
            
            string sql = $"SELECT REQUESTEDQTY FROM CREATEALLOCATION WHERE CLUBCODE = {clubcode} AND SKU = {sku} AND PRIORITIZATION = {prio} AND STATUS IS NULL";
            SqlCommand sqlCommand = new SqlCommand(sql, con);
            con.Open();
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            if (sqlDataReader.Read())
            {
                int REQ = Convert.ToInt32(sqlDataReader["REQUESTEDQTY"]);
                con.Close();
                return (REQ);
            }
            else
            {
                con.Close();
                return 0;
            }
        }

        public bool VerifyTruckLoad(int clubcode)
        {
            MSsql = $"select ClubCode FROM AllocationClub where  Clubcode = {clubcode} and IsVismin = 1";
            cmd = new SqlCommand(MSsql, con);
            
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = clubcode,
                ParameterName = "@clubcode",
                DbType = DbType.Int32
            });
            

            cmd.CommandText = MSsql;
            con.Open();

            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Close();
                con.Close();

                return true;
            }
            else
            {
                reader.Close();
                con.Close();
                return false;
            }
        }

        public int CurrentQtyAllocation(int skucode, int clubcode, int prio)
        {
            MSsql = $"SELECT RequestedQty FROM CreateAllocation where SKU = {skucode} and ClubCode = {clubcode} and Prioritization = {prio} and Status is null";
            cmd = new SqlCommand(MSsql, con);
            con.Open();
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                int cure = Convert.ToInt32(reader["RequestedQty"]);
                con.Close();
                return (cure);

            }
            else
            {
                con.Close();
                return 0;
            }
        }

    }
}