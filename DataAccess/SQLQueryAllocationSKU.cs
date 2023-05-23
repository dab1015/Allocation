using Microsoft.AspNet.Identity;
using Microsoft.SqlServer.Server;
using SNRWMSPortal.Models;
using SNRWMSPortal.Reports;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Web;

namespace SNRWMSPortal.DataAccess
{
    public class SQLQueryAllocationSKU
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["SQLConnect"].ToString());

        OleDbConnection conmms = new OleDbConnection(ConfigurationManager.ConnectionStrings["MMSConnect"].ToString());


        public string MSsql;
        SqlCommand cmd;
        SqlDataReader reader;

        OleDbCommand olecmd;
        OleDbDataReader olereader;

        public AllocationSKUModel SelectSKU(long _skuId)
        {


            var skuslist = new AllocationSKUModel();

            var keyid = CheckSKUId(_skuId);
            if (keyid == 0)
            {
                return skuslist;
            }
            else
            {

                MSsql = $"SELECT INUMBR, IDESCR,IVPLTI,IVPLHI,ISTDPK,IDSCCD,IDEPT,ISDEPT,ICLAS,ISCLAS,ASNUM FROM MMJDALIB.INVMST WHERE INUMBR  = {_skuId}";
                olecmd = new OleDbCommand(MSsql, conmms);
                conmms.Open();
                olereader = olecmd.ExecuteReader();
                if (olereader.Read())
                {
                    skuslist.SKU = Convert.ToInt32(olereader["INUMBR"]);
                    skuslist.Description = olereader["IDESCR"].ToString();
                    skuslist.IVPLTI = Convert.ToInt32(olereader["IVPLTI"]);
                    skuslist.IVPLHI = Convert.ToInt32(olereader["IVPLHI"]);
                    skuslist.ISTDPK = Convert.ToDouble(olereader["ISTDPK"]);
                    skuslist.Status = olereader["IDSCCD"].ToString();
                    skuslist.Department = Convert.ToInt32(olereader["IDEPT"]);
                    skuslist.SubDepartment = Convert.ToInt32(olereader["ISDEPT"]);
                    skuslist.ClassName = Convert.ToInt32(olereader["ICLAS"]);
                    skuslist.SubClass = Convert.ToInt32(olereader["ISCLAS"]);
                    skuslist.Vendor = Convert.ToInt32(olereader["ASNUM"]);
                }
                conmms.Close();

                MSsql = $"Select SKU,DConfig,Category,BuildToDF from  AllocationSKU WHERE SKU = {_skuId}";
                cmd = new SqlCommand(MSsql, con);
                con.Open();
                reader = cmd.ExecuteReader();
                if (reader.Read())
                { 
                    skuslist.SKU = (int)reader["SKU"];
                    skuslist.DConfig = Convert.ToInt32(reader["DConfig"]);
                    skuslist.Category = Convert.ToInt32(reader["Category"]);
                    skuslist.BuildToDF = Convert.ToString(reader["BuildToDF"]);
                }

                con.Close();

            }
            
            return skuslist;
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



        public List<AllocationSKUModel> GetClubSKU(long _skuId)
        {

            List<AllocationSKUModel> alloclub = new List<AllocationSKUModel>();

            DateTime date = DateTime.Now;
            string todaysDate = date.ToString("MM-dd-yyyy");



            //MSsql = "select ";
            MSsql = $"select Concat(A.ClubCode, '-', A.ClubName) as ClubNames, A.ClubCode,  (case when B.SKU is null then {_skuId} else B.SKU end) as SKU, (case when B.ClubCode is null then 0 else B.Clubcode end) as ClubCode, " +
                //$"(case when B.AverageSales is null then 0 else B.AverageSales end) as AverageSales, " +
                //$"(case when B.NeededOrder is null then 0 else B.NeededOrder end) as NeededOrder, " +
                //$"(case when B.DConfigQty is null then 0 else B.DConfigQty end) as DConfigQty, " +
                $"(case when C.AverageSales is null then 0 else C.AverageSales end) as AverageSales," +
                $"(case when B.Minimum is null then 0 else B.Minimum end) as Minimum, " +
                
                $"(case when B.Multiplier is null then 0 else B.Multiplier end) as Multiplier," +
                $"(case when B.CurrentMultiplier is null then 0 else B.CurrentMultiplier end) as CurrentMultiplier," +
                
                $"(case when B.LeadTime is null then 0 else B.LeadTime end) as LeadTime," +
                $"(case when B.Triggers is null then 0 else B.Triggers end) as Triggers," +
                $"(case when B.BuildTo is null then 0 else B.BuildTo end) as BuildTo," +
                $"(case when B.BuildToDF is null then 'DYNAMIC' else B.BuildToDF end) as BuildToDF," +
                $"(case when B.DConfig is null then 0 else B.DConfig end) as DConfig," +
                $"(case when B.ClubNeeds is null then 0 else B.ClubNeeds end) as ClubNeeds," +
                $"(case when B.Category is null then 0 else B.Category end) as Category," +
                $"(case when B.RoundedClubNeeds is null then 0 else B.RoundedClubNeeds end) as RoundedClubNeeds" +
                $" from AllocationClub A left outer join(select SKU, ClubCode,Minimum,Multiplier,CurrentMultiplier,LeadTime,Triggers,BuildTo,BuildToDF,DConfig,ClubNeeds,Category, RoundedClubNeeds from AllocationSKU where SKU = {_skuId}) B on A.ClubCode = B.ClubCode" +
                $" left join  (select SKU, ClubCode,AverageSales from Allocation_AverageSales where SKU = {_skuId}) C on A.ClubCode = C.ClubCode " +
                $" where A.Status = 1 order by ClubNames asc";
              // MSsql = $"Select * from AllocationSKU where SKU = {_skuId}";
            cmd = new SqlCommand(MSsql, con);
            con.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var clublist = new AllocationSKUModel();
             
                clublist.ClubName = Convert.ToString(reader["ClubNames"]);
                clublist.ClubCode = Convert.ToInt32(reader["ClubCode"]);
                clublist.SKU = Convert.ToInt64(reader["SKU"]);
                clublist.ClubCode = Convert.ToInt32(reader["ClubCode"]);
                //clublist.AveSalesPerDay = Convert.ToDouble(reader["AverageSales"]);
                //clublist.NeededOrder = Convert.ToDouble(reader["NeededOrder"]);
                //clublist.DConfigQty = Convert.ToInt32(reader["DConfigQty"]);
                clublist.Minimum = (int)reader["Minimum"];
                clublist.Multiplier = (int)reader["Multiplier"];
                clublist.CurrentMultiplier = (int)reader["CurrentMultiplier"];
                clublist.LeadTime = (int)reader["LeadTime"];
                clublist.BuildTo = Convert.ToDouble(reader["BuildTo"]);
                clublist.BuildToDF = Convert.ToString(reader["BuildToDF"]);
                clublist.Triggers = Convert.ToDouble(reader["Triggers"]);
                clublist.DConfig = Convert.ToInt32(reader["DConfig"]);
                clublist.ClubNeeds = Convert.ToDouble(reader["ClubNeeds"]);
                clublist.Category = Convert.ToInt32(reader["Category"]);
                clublist.RoundedClubNeeds = (int)reader["RoundedClubNeeds"];
                clublist.AveSalesPerDay = Convert.ToDouble(reader["AverageSales"]);

                var MSsql2 = $"SELECT A.STRNUM as ISTORE, " +
                $"(CASE WHEN B.INUMBR is null THEN {_skuId} ELSE B.INUMBR END)" +
                $" as INUMBR,(CASE WHEN B.IBHAND is null THEN 0 ELSE B.IBHAND END) as IBHAND," +
                $"(CASE WHEN B.IBINTQ is null THEN 0 ELSE B.IBINTQ END) as IBINTQ," +
                $"(CASE WHEN B.OHITsum is null THEN 0 ELSE B.OHITsum END) as OHITsum," +
                $"(CASE WHEN B.IDEPT is null THEN 0 ELSE B.IDEPT END) as IDEPT, " +
                $"(CASE WHEN B.ISDEPT is null THEN 0 ELSE B.ISDEPT END) as ISDEPT, " +
                $"(CASE WHEN B.ICLAS is null THEN 0 ELSE B.ICLAS END) as ICLAS, " +
                $"(CASE WHEN B.ISCLAS is null THEN 0 ELSE B.ISCLAS END) as ISCLAS, " +
                $"(CASE WHEN B.ASNUM is null THEN 0 ELSE B.ASNUM END) as ASNUM " +
               

                $"FROM mmjdalib.TBLSTR A LEFT JOIN(SELECT B.INUMBR, B.ISTORE, B.IBHAND, B.IBINTQ, " +
                $"(SUM(B.IBHAND) +SUM(B.IBINTQ)) as OHITsum, C.IDEPT,C.ISDEPT,C.ICLAS,C.ISCLAS,C.ASNUM FROM mmjdalib.INVBAL B " +

               

                $"left join" +
                $"(select INUMBR,IDEPT,ISDEPT,ICLAS,ISCLAS,ASNUM from mmjdalib.INVMST where INUMBR = {_skuId}) C on B.INUMBR = C.INUMBR WHERE B.INUMBR = {_skuId} AND B.ISTORE " +
                $"IN(SELECT STRNUM FROM mmjdalib.TBLSTR WHERE STRNUM BETWEEN 201 AND 299 AND STSDAT<>0) GROUP BY B.INUMBR,B.ISTORE,B.IBHAND,B.IBINTQ,C.IDEPT,C.ISDEPT,C.ICLAS,C.ISCLAS,C.ASNUM) B" +
                $" ON A.STRNUM = B.ISTORE  WHERE A.STRNUM = {Convert.ToInt32(reader["ClubCode"])} AND A.STSDAT <> 0 ORDER BY A.STRNUM ";
                olecmd = new OleDbCommand(MSsql2, conmms);
                conmms.Open();
                olereader = olecmd.ExecuteReader();
                while (olereader.Read())
                {
                    clublist.INUMBR = Convert.ToInt64(olereader["INUMBR"]);
                    
                    clublist.ISTORE = Convert.ToInt32(olereader["ISTORE"]);
                    clublist.IBHAND = Convert.ToDouble(olereader["IBHAND"]);
                    clublist.IBINTQ = Convert.ToDouble(olereader["IBINTQ"]);
                    clublist.OHITsum = Convert.ToDouble(olereader["OHITsum"]);
                    clublist.Department = Convert.ToInt32(olereader["IDEPT"]);
                    clublist.SubDepartment = Convert.ToInt32(olereader["ISDEPT"]);
                    clublist.ClassName = Convert.ToInt32(olereader["ICLAS"]);
                    clublist.SubClass = Convert.ToInt32(olereader["ISCLAS"]);
                    clublist.Vendor = Convert.ToInt32(olereader["ASNUM"]);



                }
                conmms.Close();


                alloclub.Add(clublist);
            }

            con.Close();
            return alloclub; 


        }



        //public List<AllocationSKUModel> GetClubInval(long _skuId)
        //{
        //    List<AllocationSKUModel> alloclub = new List<AllocationSKUModel>();

        //MSsql = $"SELECT A.STRNUM as ISTORE, " +
        //        $"(CASE WHEN B.INUMBR is null THEN {_skuId} ELSE B.INUMBR END)" +
        //        $" as INUMBR,(CASE WHEN B.IBHAND is null THEN 0 ELSE B.IBHAND END) as IBHAND," +
        //        $"(CASE WHEN B.IBINTQ is null THEN 0 ELSE B.IBINTQ END) as IBINTQ," +
        //        $"(CASE WHEN B.OHITsum is null THEN 0 ELSE B.OHITsum END) as OHITsum," +
        //        $"(CASE WHEN B.IDEPT is null THEN 0 ELSE B.IDEPT END) as IDEPT, " +
        //        $"(CASE WHEN B.ISDEPT is null THEN 0 ELSE B.ISDEPT END) as ISDEPT, " +
        //        $"(CASE WHEN B.ICLAS is null THEN 0 ELSE B.ICLAS END) as ICLAS, " +
        //        $"(CASE WHEN B.ISCLAS is null THEN 0 ELSE B.ISCLAS END) as ISCLAS, " +
        //        $"(CASE WHEN B.ASNUM is null THEN 0 ELSE B.ASNUM END) as ASNUM, " +
        //        $"(CASE WHEN B.O3AVED is null THEN 0 ELSE B.O3AVED END) as O3AVED " +
        //        $"FROM mmjdalib.TBLSTR A LEFT JOIN(SELECT B.INUMBR, B.ISTORE, B.IBHAND, B.IBINTQ, " +
        //        $"(SUM(B.IBHAND) +SUM(B.IBINTQ)) as OHITsum, C.IDEPT,C.ISDEPT,C.ICLAS,C.ISCLAS,C.ASNUM,D.O3AVED, D.O3STR FROM mmjdalib.INVBAL B " +
        //        $"LEFT JOIN(select O3AVED, O3STR, O3SKU from mmjdalib.slsqtys where O3SKU = {_skuId})D on B.INUMBR = D.O3SKU " +
        //        $"left join" +
        //        $"(select INUMBR,IDEPT,ISDEPT,ICLAS,ISCLAS,ASNUM from mmjdalib.INVMST where INUMBR = {_skuId}) C on B.INUMBR = C.INUMBR WHERE B.INUMBR = {_skuId} AND B.ISTORE " +
        //        $"IN(SELECT STRNUM FROM mmjdalib.TBLSTR WHERE STRNUM BETWEEN 201 AND 299 AND STSDAT<>0) GROUP BY B.INUMBR,B.ISTORE,B.IBHAND,B.IBINTQ,C.IDEPT,C.ISDEPT,C.ICLAS,C.ISCLAS,C.ASNUM,D.O3AVED, D.O3STR) B" +
        //        $" ON A.STRNUM = B.ISTORE AND A.STRNUM = B.O3STR WHERE A.STRNUM BETWEEN 201 AND 299 AND A.STSDAT <> 0 ORDER BY A.STRNUM ";
        //olecmd = new OleDbCommand(MSsql, conmms);
        //conmms.Open();
        //olereader = olecmd.ExecuteReader();
        //while (olereader.Read())
        //{
        //    var clublist = new AllocationSKUModel();
        //// clublist.INUMBR = Convert.ToInt32(olereader["INUMBR"]);
        //// skuslist.Description = olereader["IDESCR"].ToString();
        //     clublist.INUMBR = Convert.ToInt64(olereader["INUMBR"]);
        //     clublist.AveSalesPerDay = Convert.ToDouble(olereader["O3AVED"]);
        //     clublist.ISTORE = Convert.ToInt32(olereader["ISTORE"]);
        //     clublist.IBHAND = Convert.ToDouble(olereader["IBHAND"]);
        //     clublist.IBINTQ = Convert.ToDouble(olereader["IBINTQ"]);
        //     clublist.OHITsum = Convert.ToDouble(olereader["OHITsum"]);
        //     clublist.Department = Convert.ToInt32(olereader["IDEPT"]);
        //     clublist.SubDepartment = Convert.ToInt32(olereader["ISDEPT"]);
        //     clublist.ClassName = Convert.ToInt32(olereader["ICLAS"]);
        //     clublist.SubClass = Convert.ToInt32(olereader["ISCLAS"]);
        //     clublist.Vendor = Convert.ToInt32(olereader["ASNUM"]);

        //     alloclub.Add(clublist);

        //}
        //     conmms.Close();
        //    return alloclub;

        //}

        public List<AllocationSKUModel> GetClubMMS()
        {
            List<AllocationSKUModel> clublist = new List<AllocationSKUModel>();

            OleDbCommand cmd = new OleDbCommand("select STRNUM, replace(strnam,'KAREILA - ','')as strnam FROM mmjdalib.TBLSTR WHERE STCOMP = 200 AND STRNAM LIKE 'KAREILA%'", conmms);

            cmd.CommandType = CommandType.Text;
            OleDbDataAdapter sd = new OleDbDataAdapter(cmd);
            DataTable dt = new DataTable();
            conmms.Open();
            sd.Fill(dt);
            conmms.Close();
            foreach (DataRow dr in dt.Rows)
            {
                clublist.Add(
                    new AllocationSKUModel
                    {
                        STRNUM = Convert.ToInt32(dr["STRNUM"]),
                        STRNAM = Convert.ToString(dr["STRNAM"])
                    });
            }
            return clublist;
        }

        public List<AllocationSKUModel> GetClubProv(int id)
        {
            List<AllocationSKUModel> equipments = new List<AllocationSKUModel>();
            SqlCommand cmd = new SqlCommand($"SELECT * from AllocationClub where ClubCode = {id}", con);
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            con.Open();
            sd.Fill(dt);
            con.Close();
            foreach (DataRow dr in dt.Rows)
            {
                equipments.Add(
                    new AllocationSKUModel
                    {
                        STRNUM = Convert.ToInt32(dr["ClubCode"]),
                        STRNAM = Convert.ToString(dr["ClubName"]),
                        ClubStatus = Convert.ToInt32(dr["Status"]),
                        IsProvince = Convert.ToInt32(dr["IsProvince"]),




                    });
            }
            return equipments;
        }

        public void UpdateClubProvince(int ClubCode, int Status, int IsProvince)
        {



            MSsql = "UPDATE [AllocationClub] SET Status = @Status, IsProvince = @IsProvince  where ClubCode = @ClubCode";
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
                Value = IsProvince,
                ParameterName = "@IsProvince",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = Status,
                ParameterName = "@Status",
                DbType = DbType.String
            });


            cmd.ExecuteNonQuery();
            con.Close();
        }


        //public List<AllocationSKUModel> GetClubMMS()
        //{

        //    List<AllocationSKUModel> alloclub = new List<AllocationSKUModel>();

        //    MSsql = $"select STRNUM, replace(strnam,'KAREILA - ','')as strnam FROM mmjdalib.TBLSTR WHERE STCOMP = 200 AND STRNAM LIKE 'KAREILA%'";

        //    olecmd = new OleDbCommand(MSsql, conmms);
        //    conmms.Open();
        //    olereader = olecmd.ExecuteReader();
        //    while (olereader.Read())
        //    {
        //        var clublist = new AllocationSKUModel();
        //        clublist.STRNUM = Convert.ToInt32(olereader["STRNUM"]);
        //        clublist.STRNAM = Convert.ToString(olereader["STRNAM"]);



        //        var MSsql2 = $"Select Status,IsProvince from AllocationClub where ClubCode = {Convert.ToInt32(olereader["STRNUM"])}";
        //        cmd = new SqlCommand(MSsql2, con);
        //        con.Open();
        //        reader = cmd.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            clublist.ClubStatus = Convert.ToInt32(reader["Status"]);
        //            clublist.IsProvince = Convert.ToInt32(reader["IsProvince"]);



        //        }
        //        con.Close();

        //        alloclub.Add(clublist);
        //    }

        //    con.Close();
        //    return alloclub;


        //}


        public List<AllocationSKUModel> GetClubSQL()
        {
            List<AllocationSKUModel> clublist = new List<AllocationSKUModel>();
            cmd = new SqlCommand("select concat(ClubCode, '-',ClubName) as ClubCodes FROM AllocationClub", con);
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            con.Open();
            sd.Fill(dt);
            con.Close();
            foreach (DataRow dr in dt.Rows)
            {
                clublist.Add(
                    new AllocationSKUModel
                    {
                        ClubName = dr["ClubCodes"].ToString()
                        //STRNAM = Convert.ToString(dr["STRNAM"]),
                        // STSDAT = Convert.ToInt32(dr["STSDAT"])


                    });
            }
            return clublist;
        }

        public List<TruckLoadCategory> GetTruckcategory()
        {
            List<TruckLoadCategory> truckloads = new List<TruckLoadCategory>();
            SqlCommand cmd = new SqlCommand("SELECT *  From TruckLoadCategory", con);
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            con.Open();
            sd.Fill(dt);
            con.Close();
            foreach (DataRow dr in dt.Rows)
            {
                truckloads.Add(
                    new TruckLoadCategory
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        Code = Convert.ToInt32(dr["Code"]),
                        CategoryName = Convert.ToString(dr["CategoryName"])
                        

                    });
            }
            return truckloads;
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

       

        public bool InsertSKU(int SKU, int ClubCode,double Ave, int Minimum, int Multiplier, int LeadTime, double BuildTo,double Triggers,double OHIT, double ClubNeeds, int DConfig, int Category,int RoundedClubNeeds, int Department, int SubDepartment, int ClassName, int SubClass,int Vendor,string BuildToDF,double NeededOrder,int DConfigQty,string UserID,string Todays)
        {

            try
            {
                DateTime formattedday = DateTime.Parse(Todays);
                DateTime date = DateTime.Now;
                string todaysDate = date.ToString("MM-dd-yyyy");

                SqlCommand cmd = new SqlCommand("INSERT AllocationSKU(SKU,ClubCode,AverageSales,Minimum,Multiplier,CurrentMultiplier,LeadTime,BuildTo,Triggers,OHIT,ClubNeeds,DConfig,Category,RoundedClubNeeds,Department,SubDepartment,ClassName,SubClass,Vendor,BuildToDF,NeededOrder,DConfigQty,UserID,DateModified,DateCreated) VALUES(@SKU,@ClubCode,@Ave,@Minimum,@Multiplier,@CurrentMultiplier,@LeadTime,@BuildTo,@Triggers,@OHIT,@ClubNeeds,@DConfig,@Category,@RoundedClubNeeds,@Department,@SubDepartment,@ClassName,@SubClass,@Vendor,@BuilToDF,@NeededOrder,@DConfigQty,@UserID,@Todays,@DateCreated)", con);
                cmd.CommandType = CommandType.Text;
                con.Open();
                cmd.Parameters.AddWithValue("@SKU", SKU);
                cmd.Parameters.AddWithValue("@ClubCode", ClubCode);
                cmd.Parameters.AddWithValue("@Ave", Ave);
                cmd.Parameters.AddWithValue("@Minimum", Minimum);
                cmd.Parameters.AddWithValue("@Multiplier", Multiplier);
                cmd.Parameters.AddWithValue("@CurrentMultiplier", Multiplier);
                cmd.Parameters.AddWithValue("@LeadTime", LeadTime);
                cmd.Parameters.AddWithValue("@BuildTo", BuildTo);
                cmd.Parameters.AddWithValue("@Triggers", Triggers);
                cmd.Parameters.AddWithValue("@OHIT", OHIT);
                cmd.Parameters.AddWithValue("@ClubNeeds", ClubNeeds);
                cmd.Parameters.AddWithValue("@DConfig", DConfig);
                cmd.Parameters.AddWithValue("@Category", Category);
                cmd.Parameters.AddWithValue("@RoundedClubNeeds", RoundedClubNeeds);
                cmd.Parameters.AddWithValue("@Department", Department);
                cmd.Parameters.AddWithValue("@SubDepartment", SubDepartment);
                cmd.Parameters.AddWithValue("@ClassName", ClassName);
                cmd.Parameters.AddWithValue("@SubClass", SubClass);
                cmd.Parameters.AddWithValue("@Vendor", Vendor);
                cmd.Parameters.AddWithValue("@BuilToDF", BuildToDF);
                cmd.Parameters.AddWithValue("@NeededOrder", NeededOrder);
                cmd.Parameters.AddWithValue("@DConfigQty",DConfigQty);
                cmd.Parameters.AddWithValue("@UserID", UserID);
                cmd.Parameters.AddWithValue("@Todays", formattedday);
                cmd.Parameters.AddWithValue("@DateCreated", todaysDate);



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

        


       

        public bool VerifyClubInsert(int skuid, int clubcode)
        {
            MSsql = $"SELECT SKU,Clubcode from AllocationSKU where SKU = @skuid and ClubCode = @clubcode" ;
            cmd = new SqlCommand(MSsql, con);
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = skuid,
                ParameterName = "@skuid",
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

        //public bool VerifyInvalInsert(int skuid, int clubcode)
        //{
        //    MSsql = $"SELECT SKUNO,Clubcode from AllocationOHIT where SKUNO = {skuid} and ClubCode = {clubcode}";
        //    cmd = new SqlCommand(MSsql, con);
        //    cmd.Parameters.Add(new SqlParameter()
        //    {
        //        Value = skuid,
        //        ParameterName = $"{skuid}",
        //        DbType = DbType.Int32
        //    });
        //    cmd.Parameters.Add(new SqlParameter()
        //    {
        //        Value = clubcode,
        //        ParameterName = $"{clubcode}",
        //        DbType = DbType.Int32
        //    });
        //    cmd.CommandText = MSsql;
        //    con.Open();

        //    reader = cmd.ExecuteReader();

        //    if (reader.HasRows)
        //    {
        //        reader.Close();
        //        con.Close();

        //        return true;
        //    }
        //    else
        //    {
        //        reader.Close();
        //        con.Close();
        //        return false;
        //    }
        //}


        public void UpdateSKU(int SKU, int ClubCode, double Ave,int Minimum, int Multiplier, int LeadTime,  double BuildTo, double Triggers,double OHIT,  double ClubNeeds, int DConfig, int Category,int RoundedClubNeeds, int Department, int SubDepartment, int ClassName, int SubClass,int Vendor,string BuildToDF,double NeededOrder,int DConfigQty,string UserID, string Todays)
        {
            DateTime date = DateTime.Now;
            string todaysDate = date.ToString("MM-dd-yyyy");
            DateTime formattedday = DateTime.Parse(Todays);

            
           
           


            MSsql = "UPDATE [AllocationSKU] SET  AverageSales = @Ave,Minimum =@Minimum, Multiplier = CASE WHEN DateTo >= @todaysDate THEN Multiplier ELSE @Multiplier END,CurrentMultiplier = @CurrentMultiplier, LeadTime =@LeadTime,  BuildTo =@BuildTo, Triggers =@Triggers,OHIT = @OHIT, ClubNeeds = @ClubNeeds,DConfig = @DConfig, Category= @Category,RoundedClubNeeds = @RoundedClubNeeds , Department = @Department, SubDepartment = @SubDepartment, ClassName = @ClassName, SubClass = @SubClass, Vendor = @Vendor,BuildToDF = @BuildToDF,NeededOrder = @NeededOrder ,DConfigQty = @DConfigQty,UserID = @UserID,DateModified = @Todays,DateCreated = @todaysDate where SKU = @SKU and CLubCode = @ClubCode";
            con.Open();
            cmd = new SqlCommand(MSsql, con);
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = SKU,
                ParameterName = "@SKU",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = ClubCode,
                ParameterName = "@ClubCode",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = Ave,
                ParameterName = "@Ave",
                DbType = DbType.Double
            });


            cmd.Parameters.Add(new SqlParameter()
            {
                Value = Minimum,
                ParameterName = "@Minimum",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = Multiplier,
                ParameterName = "@Multiplier",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = Multiplier,
                ParameterName = "@CurrentMultiplier",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = LeadTime,
                ParameterName = "@LeadTime",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = BuildTo,
                ParameterName = "@BuildTo",
                DbType = DbType.Double
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = Triggers,
                ParameterName = "@Triggers",
                DbType = DbType.Double
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = OHIT,
                ParameterName = "@OHIT",
                DbType = DbType.Double
            });



            cmd.Parameters.Add(new SqlParameter()
            {
                Value = ClubNeeds,
                ParameterName = "@ClubNeeds",
                DbType = DbType.Double
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = DConfig,
                ParameterName = "@DConfig",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = Category,
                ParameterName = "@Category",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = RoundedClubNeeds,
                ParameterName = "@RoundedClubNeeds",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = Department,
                ParameterName = "@Department",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = SubDepartment,
                ParameterName = "@SubDepartment",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = ClassName,
                ParameterName = "@ClassName",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = SubClass,
                ParameterName = "@SubClass",
                DbType = DbType.Int32
            });

            cmd.Parameters.Add(new SqlParameter()
            {
                Value = Vendor,
                ParameterName = "@Vendor",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = BuildToDF,
                ParameterName = "@BuildToDF",
                DbType = DbType.String
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = NeededOrder,
                ParameterName = "@NeededOrder",
                DbType = DbType.Double
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = DConfigQty,
                ParameterName = "@DConfigQty",
                DbType = DbType.Int32
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
                Value = todaysDate,
                ParameterName = "@todaysDate",
                DbType = DbType.DateTime
            });
            


            cmd.ExecuteNonQuery();
            con.Close();


        }


        public bool InsertClub(int ClubCode, string ClubName)
        {

            try
            {
                SqlCommand cmd = new SqlCommand("INSERT AllocationClub(ClubCode,ClubName) VALUES(@ClubCode,@ClubName)", con);
                cmd.CommandType = CommandType.Text;
                con.Open();
               
                cmd.Parameters.AddWithValue("@ClubCode", ClubCode);
                cmd.Parameters.AddWithValue("@ClubName", ClubName);


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

       

        //public void UpdateInval(int SKU, int ClubCode, double ClubNeeds,int RoundedClubNeeds)
        //{
        //    MSsql = "UPDATE [AllocationSKU] SET  ClubNeeds = @ClubNeeds, RoundedClubNeeds = @RoundedClubNeeds  where SKU = @SKU and CLubCode = @ClubCode";
        //    con.Open();
        //    cmd = new SqlCommand(MSsql, con);
        //    cmd.Parameters.Add(new SqlParameter()
        //    {
        //        Value = SKU,
        //        ParameterName = "@SKU",
        //        DbType = DbType.Int32
        //    });
        //    cmd.Parameters.Add(new SqlParameter()
        //    {
        //        Value = ClubCode,
        //        ParameterName = "@ClubCode",
        //        DbType = DbType.Int32
        //    });
           
        //    cmd.Parameters.Add(new SqlParameter()
        //    {
        //        Value = ClubNeeds,
        //        ParameterName = "@ClubNeeds",
        //        DbType = DbType.Double
        //    });
        //    cmd.Parameters.Add(new SqlParameter()
        //    {
        //        Value = RoundedClubNeeds,
        //        ParameterName = "@RoundedClubNeeds",
        //        DbType = DbType.Int32
        //    });


        //    cmd.ExecuteNonQuery();
        //    con.Close();


        //}

        public void UpdateClub(int ClubCode, string ClubName)
        {
            MSsql = "UPDATE [AllocationClub] SET ClubCode = @ClubCode, ClubName =@ClubName where ClubCode = @ClubCode";
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
                Value = ClubName,
                ParameterName = "@ClubName",
                DbType = DbType.String
            });
            
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public bool VerifyInsertClub(int clubcode)
        {
            MSsql = $"SELECT Clubcode from AllocationClub where ClubCode = {clubcode}";
            cmd = new SqlCommand(MSsql, con);
            
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = clubcode,
                ParameterName = $"{clubcode}",
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

        //public bool InsertBatch(double AverageSales,int Minimum, int Multiplier, int LeadTime, int SKU, int ClubCode, double BuildTo, double Triggers,int DConfig, int Category, double ClubNeeds, int RoundedClubNeeds, int Department, int SubDepartment, int ClassName, int SubClass,int Vendor,string BuildToDF,double NeededOrder,double OHITSum,double DConfigQty,string UserID, string Todays)

             public bool InsertBatch(int Minimum, int Multiplier, int LeadTime, int SKU, int ClubCode,  int DConfig, int Category, /*int Department, int SubDepartment, int ClassName, int SubClass, int Vendor,*/ string BuildToDF,  string UserID, string Todays)
        {
            //Department,SubDepartment, ClassName,SubClass,Vendor,
            //@Department, @SubDepartment, @ClassName, @SubClass, @Vendor,
            //,CurrentMultiplier,
            //@CurrentMultiplier
            try
            {
                DateTime formattedday = DateTime.Parse(Todays);
                DateTime date = DateTime.Now;
                string todaysDate = date.ToString("MM-dd-yyyy");

                SqlCommand cmd = new SqlCommand("INSERT AllocationSKU(Minimum,Multiplier,CurrentMultiplier,LeadTime,SKU,ClubCode,DConfig,Category, BuildToDF,UserID,DateModified,DateCreated) VALUES(@Minimum,@Multiplier,@CurrentMultiplier,@LeadTime,@SKU,@ClubCode,@DConfig,@Category, @BuildToDF, @UserID,@Todays,@todaysDate)", con);
                cmd.CommandType = CommandType.Text;
                con.Open();
                //cmd.Parameters.AddWithValue("@AverageSales", AverageSales);
                cmd.Parameters.AddWithValue("@Minimum", Minimum);
                cmd.Parameters.AddWithValue("@Multiplier", Multiplier);
                cmd.Parameters.AddWithValue("@CurrentMultiplier", Multiplier);
                cmd.Parameters.AddWithValue("@LeadTime", LeadTime);
                

                cmd.Parameters.AddWithValue("@SKU", SKU);
                cmd.Parameters.AddWithValue("@ClubCode", ClubCode);
                
                //cmd.Parameters.AddWithValue("@BuildTo", BuildTo);
                //cmd.Parameters.AddWithValue("@Triggers", Triggers);
                cmd.Parameters.AddWithValue("@DConfig", DConfig);
               
                cmd.Parameters.AddWithValue("@Category", Category);
                //cmd.Parameters.AddWithValue("@ClubNeeds", ClubNeeds);
                //cmd.Parameters.AddWithValue("@RoundedClubNeeds", RoundedClubNeeds);
                //cmd.Parameters.AddWithValue("@Department", Department);
                //cmd.Parameters.AddWithValue("@SubDepartment", SubDepartment);
                //cmd.Parameters.AddWithValue("@ClassName", ClassName);
                //cmd.Parameters.AddWithValue("@SubClass", SubClass);
                //cmd.Parameters.AddWithValue("@Vendor", Vendor);
                cmd.Parameters.AddWithValue("@BuildToDF", BuildToDF);
                //cmd.Parameters.AddWithValue("@NeededOrder", NeededOrder);
                //cmd.Parameters.AddWithValue("@OHITSum", OHITSum);
                //cmd.Parameters.AddWithValue("@DConfigQty", DConfigQty);
                cmd.Parameters.AddWithValue("@UserID", UserID);
                cmd.Parameters.AddWithValue("@Todays", formattedday);
                cmd.Parameters.AddWithValue("@todaysDate", todaysDate);


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

        //public void UpdateBatch(double AverageSales,int Minimum, int Multiplier, int LeadTime,  int SKU, int ClubCode,double BuildTo, double Triggers,int DConfig, int Category,double ClubNeeds,int RoundedClubNeeds, int Department, int SubDepartment, int ClassName, int SubClass,int Vendor,string BuildToDF,double NeededOrder,double OHITSum, double DConfigQty,string UserID, string Todays)
            public void UpdateBatch(int Minimum, int Multiplier, int LeadTime, int SKU, int ClubCode, int DConfig, int Category, string BuildToDF,string UserID, string Todays)
        {
            DateTime formattedday = DateTime.Parse(Todays);
            DateTime date = DateTime.Now;
            string todaysDate = date.ToString("MM-dd-yyyy");

            //MSsql = "UPDATE [AllocationSKU] SET AverageSales = @AverageSales, Minimum = @Minimum, Multiplier = CASE WHEN DateTo >= @todaysDate THEN Multiplier ELSE @Multiplier END,CurrentMultiplier = @CurrentMultiplier, LeadTime = @LeadTime,BuildTo= @BuildTo, Triggers= @Triggers, DConfig= @DConfig, Category = @Category,ClubNeeds=@ClubNeeds,RoundedClubNeeds = @RoundedClubNeeds, Department = @Department, SubDepartment = @SubDepartment, ClassName = @ClassName, SubClass = @SubClass, Vendor = @Vendor,BuildToDF = @BuildToDF,NeededOrder = @NeededOrder, OHIT = @OHITSum, DConfigQty = @DConfigQty,UserID = @UserID,DateModified = @Todays where SKU = @SKU and CLubCode = @ClubCode";

            MSsql = "UPDATE [AllocationSKU] SET Minimum = @Minimum, Multiplier = CASE WHEN DateTo >= @todaysDate THEN Multiplier ELSE @Multiplier END,CurrentMultiplier = @CurrentMultiplier, LeadTime = @LeadTime, DConfig= @DConfig, Category = @Category, BuildToDF = @BuildToDF,UserID = @UserID,DateModified = @Todays, DateCreated = @todaysDate where SKU = @SKU and CLubCode = @ClubCode";
            con.Open();
            cmd = new SqlCommand(MSsql, con);
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = SKU,
                ParameterName = "@SKU",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = ClubCode,
                ParameterName = "@ClubCode",
                DbType = DbType.Int32
            });

            

            cmd.Parameters.Add(new SqlParameter()
            {
                Value = Minimum,
                ParameterName = "@Minimum",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = Multiplier,
                ParameterName = "@Multiplier",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = Multiplier,
                ParameterName = "@CurrentMultiplier",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = LeadTime,
                ParameterName = "@LeadTime",
                DbType = DbType.Int32
            });

            cmd.Parameters.Add(new SqlParameter()
            {
                Value = DConfig,
                ParameterName = "@DConfig",
                DbType = DbType.Int32
            });


            cmd.Parameters.Add(new SqlParameter()
            {
                Value = Category,
                ParameterName = "@Category",
                DbType = DbType.Int32
            });

            cmd.Parameters.Add(new SqlParameter()
            {
                Value = BuildToDF,
                ParameterName = "@BuildToDF",
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
                Value = todaysDate,
                ParameterName = "@todaysDate",
                DbType = DbType.DateTime
            });


            cmd.ExecuteNonQuery();
            con.Close();


        }


        public bool VerifyBatchInsert(int skuid, int clubcode)
        {
            MSsql = $"SELECT SKU,Clubcode from AllocationSKU where SKU = @skuid and ClubCode = @clubcode";
            cmd = new SqlCommand(MSsql, con);
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = skuid,
                ParameterName = "@skuid",
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


        public double AverageSales(int SKUId, int ClubCode)
        {
            MSsql = $"SELECT O3AVED FROM mmjdalib.slsqtys where O3SKU = {SKUId} and O3STR = {ClubCode} ";
            olecmd = new OleDbCommand(MSsql, conmms);
            conmms.Open();
            olereader = olecmd.ExecuteReader();
            if (olereader.Read())
            {
                double ave = Convert.ToDouble(olereader["O3AVED"]);
                conmms.Close();
                return (ave);
                
            }
            else
            {
                conmms.Close();
                return 0;
            }
        }

        public double SelectOHITMMS(int _skuId,int _storecode )
        {

            MSsql = $"select INUMBR,ISTORE, IBHAND,IBINTQ, (SUM(IBHAND) + SUM(IBINTQ)) as OHITsum FROM mmjdalib.INVBAL WHERE INUMBR = {_skuId} AND ISTORE = {_storecode} GROUP BY INUMBR,ISTORE, IBHAND,IBINTQ";
            olecmd = new OleDbCommand(MSsql, conmms);
            conmms.Open();
            olereader = olecmd.ExecuteReader();
            if (olereader.Read())
            {
                double ohit = Convert.ToDouble(olereader["OHITsum"]);
                conmms.Close();
                return (ohit);

            }
            else
            {
                conmms.Close();
                return 0;
            }
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



       

    }
}