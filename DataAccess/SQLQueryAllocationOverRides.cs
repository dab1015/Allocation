using Microsoft.AspNet.Identity;
using Microsoft.SqlServer.Server;
using SNRWMSPortal.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Web;
using System.Web.UI.WebControls;

namespace SNRWMSPortal.DataAccess
{
    public class SQLQueryAllocationOverRides
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["SQLConnect"].ToString());

        OleDbConnection conmms = new OleDbConnection(ConfigurationManager.ConnectionStrings["MMSConnect"].ToString());


        public string MSsql;
        SqlCommand cmd;
        SqlDataReader reader;

        OleDbCommand olecmd;
        OleDbDataReader olereader;


        public AllocationOverRides SelectSKU(long _skuId)
        {


            var skuslist = new AllocationOverRides();

            
                MSsql = $"SELECT A.INUMBR, A.IDESCR,A.ASNUM,B.ASNAME FROM MMJDALIB.INVMST A inner join MMJDALIB.apsupp B on A.ASNUM = B.ASNUM where A.INUMBR = {_skuId}";
                olecmd = new OleDbCommand(MSsql, conmms);
                conmms.Open();
                olereader = olecmd.ExecuteReader();
                if (olereader.Read())
                {
                    skuslist.SKU = Convert.ToInt64(olereader["INUMBR"]);
                    skuslist.Description = olereader["IDESCR"].ToString();
                    skuslist.VendorCode = Convert.ToInt32(olereader["ASNUM"]);
                    skuslist.VendorName = Convert.ToString(olereader["ASNAME"]);
            }
            return skuslist;
        }

        public AllocationOverRides SelectVendorName(int _vendorId)
        {


            var skuslist = new AllocationOverRides();


            MSsql = $"SELECT ASNUM,ASNAME FROM MMJDALIB.apsupp where ASNUM = {_vendorId}";
            olecmd = new OleDbCommand(MSsql, conmms);
            conmms.Open();
            olereader = olecmd.ExecuteReader();
            if (olereader.Read())
            {
                
                skuslist.VendorCode = Convert.ToInt32(olereader["ASNUM"]);
                skuslist.VendorName = Convert.ToString(olereader["ASNAME"]);
            }
            return skuslist;
        }

        public List<AllocationOverRides> GetDepartment()
        {
            List<AllocationOverRides> departments = new List<AllocationOverRides>();
            SqlCommand cmd = new SqlCommand("SELECT concat(Code,'-',DeptName) as DepartmentName,Code,DeptName  From Department", con);
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            con.Open();
            sd.Fill(dt);
            con.Close();
            foreach (DataRow dr in dt.Rows)
            {
                departments.Add(
                    new AllocationOverRides
                    {
                        
                        Code = Convert.ToInt32(dr["Code"]),
                        DeptName = Convert.ToString(dr["DepartmentName"]),
                       // SubDepartmentCode = Convert.ToInt32(dr["SubDepartmentCode"])


                    });
            }
            return departments;
        }

        public List<AllocationOverRides> GetDepartmentSub(int deptcode)
        {
            List<AllocationOverRides> departments = new List<AllocationOverRides>();
            OleDbCommand cmd = new OleDbCommand($"select (isdept || '-' || dptnam) as SubDepartment,isdept from mmjdalib.invdpt where idept = {deptcode}  and isdept <> 0 and iclas = 0 and isclas = 0", conmms);
            cmd.CommandType = CommandType.Text;
            OleDbDataAdapter sd = new OleDbDataAdapter(cmd);
            DataTable dt = new DataTable();
            conmms.Open();
            sd.Fill(dt);
            conmms.Close();
            foreach (DataRow dr in dt.Rows)
            {
                departments.Add(
                    new AllocationOverRides
                    {
                     
                        SubCode = Convert.ToInt32(dr["isdept"]),
                        SubDeptName = Convert.ToString(dr["SubDepartment"])
                      
                    });
            }
            return departments;
        }

        public List<AllocationOverRides> GetClass(int deptcode, int subcode)
        {
            List<AllocationOverRides> departments = new List<AllocationOverRides>();
            OleDbCommand cmd = new OleDbCommand($"select (iclas || '-' || dptnam) as ClassName,iclas from mmjdalib.invdpt where idept = {deptcode}  and isdept = {subcode} and iclas <> 0 and isclas = 0", conmms);
            cmd.CommandType = CommandType.Text;
            OleDbDataAdapter sd = new OleDbDataAdapter(cmd);
            DataTable dt = new DataTable();
            conmms.Open();
            sd.Fill(dt);
            conmms.Close();
            foreach (DataRow dr in dt.Rows)
            {
                departments.Add(
                    new AllocationOverRides
                    {

                        ClassCode = Convert.ToInt32(dr["iclas"]),
                        ClassName = Convert.ToString(dr["ClassName"])

                    });
            }
            return departments;
        }

        public List<AllocationOverRides> GetSubClass(int deptcode, int subcode, int classcode)
        {
            List<AllocationOverRides> departments = new List<AllocationOverRides>();
            OleDbCommand olebcmd = new OleDbCommand($"select (isclas || '-' || dptnam) as SubClassName,isclas from mmjdalib.invdpt where idept ={deptcode}  and isdept = {subcode} and iclas = {classcode} and isclas <> 0", conmms);
            olebcmd.CommandType = CommandType.Text;
            OleDbDataAdapter sd = new OleDbDataAdapter(olebcmd);
            DataTable dt = new DataTable();
            conmms.Open();
            sd.Fill(dt);
            conmms.Close();
            foreach (DataRow dr in dt.Rows)
            {
                departments.Add(
                    new AllocationOverRides
                    {

                        SubClassCode = Convert.ToInt32(dr["isclas"]),
                        SubClassName = Convert.ToString(dr["SubClassName"])

                    });
            }
            return departments;
        }


       


        public List<AllocationOverRides> GetClubSKU(long _skuId)
        {

            List<AllocationOverRides> alloclub = new List<AllocationOverRides>();

            MSsql = $"select C.clubcode,concat(C.ClubCode,'-',C.ClubName) as ClubNames,(case when D.sku is null then {_skuId} else D.sku end) as sku,(case when D.ClubCode is null then 0 else D.ClubCode end) as ClubCode,concat(C.ClubCode,'-',C.ClubName)as ClubNames," +
                $"(case when D.Multiplier is null then 0 else D.Multiplier end) as Multiplier," +
                $"(case when D.MultiplierAdjustment is null then 0 else D.MultiplierAdjustment end) as MultiplierAdjustment," +
                $"(case when D.CurrentMultiplier is null then 0 else D.CurrentMultiplier end) as CurrentMultiplier, " +
                $"(case when D.DateFrom is null then CAST(GETDATE() as Date) else D.DateFrom end) as DateFrom, " +
                $"(case when D.DateTo is null then CAST(GETDATE() as DATE) else D.DateTo end) as DateTo " +
                $"from AllocationClub C left outer join(select A.sku, A.ClubCode, A.Multiplier, A.CurrentMultiplier,A.MultiplierAdjustment,A.DateFrom,A.DateTo from AllocationSKU A left join (select ClubCode from AllocationOverRides) B on A.ClubCode = b.ClubCode where A.sku = {_skuId}) D on C.ClubCode = D.ClubCode where C.Status = 1 group by C.ClubCode,D.ClubCode,D.SKU,D.Multiplier,D.CurrentMultiplier,D.MultiplierAdjustment,C.ClubName,D.DateFrom,D.DateTo order by C.ClubCode asc";
            
            cmd = new SqlCommand(MSsql, con);
            con.Open();
            reader = cmd.ExecuteReader();
            


                while (reader.Read())
                {
                    var clublist = new AllocationOverRides();

                string dtFrom = reader["DateFrom"].ToString();
                string dtTo = reader["DateTo"].ToString();


                string formattedDateFrom = DateTime.Parse(dtFrom).ToString("MM-dd-yyyy");
                string formattedDateTo = DateTime.Parse(dtTo).ToString("MM-dd-yyyy");

                clublist.ClubCode = Convert.ToInt32(reader["ClubCode"]);
                clublist.ClubNames = Convert.ToString(reader["ClubNames"]);
                clublist.SKU = Convert.ToInt64(reader["sku"]);
                clublist.MultiplierSetup = Convert.ToInt32(reader["Multiplier"]);
                clublist.CurrentMultiplier = Convert.ToInt32(reader["CurrentMultiplier"]);
                clublist.MultiplierAdjustment = Convert.ToInt32(reader["MultiplierAdjustment"]);
                clublist.DateFrom = formattedDateFrom;
                clublist.DateTo = formattedDateTo;

                alloclub.Add(clublist);
                }
            
                con.Close();
                return alloclub;
            

        }



        public List<AllocationOverRides> SearchAll(int searchcode)
        {


            List<AllocationOverRides> alloclub = new List<AllocationOverRides>();

            MSsql = $"select distinct(Clubcodes),ClubNames,Department,MultiplierAdjustment,SubDepartment,ClassName,SubClass,Vendor,AllSearch,DateFrom,DateTo from(" +
                $" select  (case when A.ClubCode is null then 0 else A.Clubcode end) as ClubCodes,concat(A.ClubCode,'-',A.ClubName)as ClubNames," +
                $"(case when B.Department is null then 0 else B.Department end) as Department," +
                $"(case when B.SubDepartment is null then 0 else B.SubDepartment end) as SubDepartment," +
                $"(case when B.ClassName is null then 0 else B.ClassName end) as ClassName," +
                $"(case when B.SubClass is null then 0 else B.SubClass end) as SubClass," +
                $"(case when B.Vendor is null then 0 else B.Vendor end) as Vendor," +
                $"(case when B.AllSearch is null then {searchcode} else B.AllSearch end) as AllSearch," +
                
                $"(case when B.MultiplierAdjustment is null then 0 else B.MultiplierAdjustment end) as MultiplierAdjustment," +
                $"(case when B.DateFrom is null then CAST(GETDATE() as Date) else B.DateFrom end) as DateFrom, " +
                $"(case when B.DateTo is null then CAST(GETDATE() as DATE) else B.DateTo end) as DateTo " +
                $" from AllocationClub A left join(select ClubCode,MultiplierAdjustment,Department,SubDepartment,ClassName,SubClass,Vendor,AllSearch,DateFrom,DateTo from AllocationOverRides B where AllSearch = {searchcode} and Vendor = 0 and Department = 0 and SubDepartment = 0 and ClassName = 0 and SubClass = 0) B on A.ClubCode = B.ClubCode  where A.Status = 1) A order by ClubCodes asc";
            //MSsql = $"select IDEPT,INUMBR,IDESCR from mmjdalib.INVmst  WHERE IDEPT = {deptcode}";
            cmd = new SqlCommand(MSsql, con);
            con.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var clublist = new AllocationOverRides();
                string dtFrom = reader["DateFrom"].ToString();
                string dtTo = reader["DateTo"].ToString();


                string formattedDateFrom = DateTime.Parse(dtFrom).ToString("MM-dd-yyyy");
                string formattedDateTo = DateTime.Parse(dtTo).ToString("MM-dd-yyyy");

                clublist.ClubCode = Convert.ToInt32(reader["ClubCodes"]);
                clublist.ClubNames = Convert.ToString(reader["ClubNames"]);
                clublist.VendorCode = Convert.ToInt32(reader["Vendor"]);
                
                clublist.MultiplierAdjustment = Convert.ToInt32(reader["MultiplierAdjustment"]);
                clublist.Department = Convert.ToInt32(reader["Department"]);
                clublist.SubDepartment = Convert.ToInt32(reader["SubDepartment"]);
                clublist.ClassCode = Convert.ToInt32(reader["ClassName"]);
                clublist.SubClassCode = Convert.ToInt32(reader["SubClass"]);
                clublist.AllSearch = Convert.ToInt32(reader["AllSearch"]);
                clublist.DateFrom = formattedDateFrom;
                clublist.DateTo = formattedDateTo;
                
                
                alloclub.Add(clublist);
            }

            con.Close();



            return alloclub;



        }


        public List<AllocationOverRides> SearchVCode(int vcode)
        {

            string todaysDate = DateTime.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            List<AllocationOverRides> alloclub = new List<AllocationOverRides>();
            
            MSsql = $"select distinct(Clubcodes),ClubNames,Department,MultiplierAdjustment,SubDepartment,ClassName,SubClass,Vendor,AllSearch,DateFrom,DateTo from(" +
                $" select  (case when A.ClubCode is null then 0 else A.Clubcode end) as ClubCodes,concat(A.ClubCode,'-',A.ClubName)as ClubNames," +
                $"(case when B.Department is null then 0 else B.Department end) as Department," +
                $"(case when B.SubDepartment is null then 0 else B.SubDepartment end) as SubDepartment," +
                $"(case when B.ClassName is null then 0 else B.ClassName end) as ClassName," +
                $"(case when B.SubClass is null then 0 else B.SubClass end) as SubClass," +
                $"(case when B.Vendor is null then {vcode} else B.Vendor end) as Vendor," +
                $"(case when B.AllSearch is null then 0 else B.AllSearch end) as AllSearch," +
                
                $"(case when B.MultiplierAdjustment is null then 0 else B.MultiplierAdjustment end) as MultiplierAdjustment, " +
                $"(case when B.DateFrom is null then CAST(GETDATE() as Date) else B.DateFrom end) as DateFrom, " +
                $"(case when B.DateTo is null then CAST(GETDATE() as DATE) else B.DateTo end) as DateTo " +
                $" from AllocationClub A left join(select ClubCode,MultiplierAdjustment,Department,SubDepartment,ClassName,SubClass,Vendor,AllSearch,DateFrom,DateTo from AllocationOverRides B where Vendor = {vcode} and Department = 0 and SubDepartment = 0 and ClassName = 0 and SubClass = 0) B on A.ClubCode = B.ClubCode  where A.Status = 1) A order by ClubCodes asc";
            //MSsql = $"select IDEPT,INUMBR,IDESCR from mmjdalib.INVmst  WHERE IDEPT = {deptcode}";
            cmd = new SqlCommand(MSsql, con);
            con.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var clublist = new AllocationOverRides();

                string dtFrom = reader["DateFrom"].ToString();
                string dtTo = reader["DateTo"].ToString();

                
                string formattedDateFrom = DateTime.Parse(dtFrom).ToString("MM-dd-yyyy");
                string formattedDateTo = DateTime.Parse(dtTo).ToString("MM-dd-yyyy");




                clublist.ClubCode = Convert.ToInt32(reader["ClubCodes"]);
                clublist.ClubNames = Convert.ToString(reader["ClubNames"]);
                clublist.VendorCode = Convert.ToInt32(reader["Vendor"]);
               
                clublist.MultiplierAdjustment = Convert.ToInt32(reader["MultiplierAdjustment"]);
                clublist.Department = Convert.ToInt32(reader["Department"]);
                clublist.SubDepartment = Convert.ToInt32(reader["SubDepartment"]);
                clublist.ClassCode = Convert.ToInt32(reader["ClassName"]);
                clublist.SubClassCode = Convert.ToInt32(reader["SubClass"]);
                clublist.AllSearch = Convert.ToInt32(reader["AllSearch"]);
                clublist.DateFrom = formattedDateFrom;
                clublist.DateTo = formattedDateTo;
                alloclub.Add(clublist);
            }

            con.Close();



            return alloclub;



        }

        public List<AllocationOverRides> GetClubDeptCode(int deptcode)
        {

            List<AllocationOverRides> alloclub = new List<AllocationOverRides>();

            MSsql = $"select distinct(Clubcodes),ClubNames,Department,MultiplierAdjustment,SubDepartment,ClassName,SubClass,Vendor,AllSearch,DateFrom,DateTo from(" +
                $" select  (case when A.ClubCode is null then 0 else A.Clubcode end) as ClubCodes,concat(A.ClubCode,'-',A.ClubName)as ClubNames," +
                $"(case when B.Department is null then {deptcode} else B.Department end) as Department," +
                $"(case when B.SubDepartment is null then 0 else B.SubDepartment end) as SubDepartment," +
                $"(case when B.ClassName is null then 0 else B.ClassName end) as ClassName," +
                $"(case when B.SubClass is null then 0 else B.SubClass end) as SubClass," +
                $"(case when B.Vendor is null then 0 else B.Vendor end) as Vendor," +
                $"(case when B.AllSearch is null then 0 else B.AllSearch end) as AllSearch," +
                
                $"(case when B.MultiplierAdjustment is null then 0 else B.MultiplierAdjustment end) as MultiplierAdjustment," +
                $"(case when B.DateFrom is null then CAST(GETDATE() as Date) else B.DateFrom end) as DateFrom, " +
                $"(case when B.DateTo is null then CAST(GETDATE() as DATE) else B.DateTo end) as DateTo " +
                $" from AllocationClub A left join(select ClubCode,MultiplierAdjustment,Department,SubDepartment,ClassName,SubClass,Vendor,AllSearch,DateFrom,DateTo from AllocationOverRides B where Department = {deptcode} and SubDepartment = 0 and ClassName = 0 and SubClass = 0 and Vendor = 0) B on A.ClubCode = B.ClubCode where A.Status = 1) A order by ClubCodes asc";

            cmd = new SqlCommand(MSsql, con);
            con.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var clublist = new AllocationOverRides();

                string dtFrom = reader["DateFrom"].ToString();
                string dtTo = reader["DateTo"].ToString();


                string formattedDateFrom = DateTime.Parse(dtFrom).ToString("MM-dd-yyyy");
                string formattedDateTo = DateTime.Parse(dtTo).ToString("MM-dd-yyyy");

                clublist.ClubCode = Convert.ToInt32(reader["ClubCodes"]);
                clublist.ClubNames = Convert.ToString(reader["ClubNames"]);
                
                clublist.MultiplierAdjustment = Convert.ToInt32(reader["MultiplierAdjustment"]);
                clublist.Department = Convert.ToInt32(reader["Department"]);
                clublist.SubDepartment = Convert.ToInt32(reader["SubDepartment"]);
                clublist.ClassCode = Convert.ToInt32(reader["ClassName"]);
                clublist.SubClassCode = Convert.ToInt32(reader["SubClass"]);
                clublist.VendorCode = Convert.ToInt32(reader["Vendor"]);
                clublist.AllSearch = Convert.ToInt32(reader["AllSearch"]);
                clublist.DateFrom = formattedDateFrom;
                clublist.DateTo = formattedDateTo;
                alloclub.Add(clublist);
            }

            con.Close();
            return alloclub;


        }

        public List<AllocationOverRides> GetClubSubDeptCode(int deptcode,int subdeptcode)
        {

            List<AllocationOverRides> alloclub = new List<AllocationOverRides>();

            MSsql = $"select distinct(Clubcodes),ClubNames,Department,SubDepartment,ClassName,SubClass,MultiplierAdjustment,Vendor,AllSearch,DateFrom,DateTo from(" +
                $"select (case when A.ClubCode is null then 0 else A.Clubcode end) as ClubCodes,concat(A.ClubCode,'-',A.ClubName)as ClubNames," +
                $"(case when B.Department is null then {deptcode} else B.Department end) as Department," +
                $"(case when B.SubDepartment is null then {subdeptcode} else B.SubDepartment end) as SubDepartment," +
                $"(case when B.ClassName is null then 0 else B.ClassName end) as ClassName," +
                $"(case when B.SubClass is null then 0 else B.SubClass end) as SubClass," +
                $"(case when B.Vendor is null then 0 else B.Vendor end) as Vendor," +
                $"(case when B.AllSearch is null then 0 else B.AllSearch end) as AllSearch," +
                
                $"(case when B.MultiplierAdjustment is null then 0 else B.MultiplierAdjustment end) as MultiplierAdjustment," +
                $"(case when B.DateFrom is null then CAST(GETDATE() as Date) else B.DateFrom end) as DateFrom, " +
                $"(case when B.DateTo is null then CAST(GETDATE() as DATE) else B.DateTo end) as DateTo " +
                $" from AllocationClub A left join(select Department,SubDepartment,ClassName,SubClass, ClubCode,MultiplierAdjustment,Vendor,AllSearch,DateFrom,DateTo from AllocationOverRides B where Department = {deptcode} and SubDepartment = {subdeptcode} and ClassName = 0 and SubClass = 0 and Vendor = 0) B on A.ClubCode = B.ClubCode where A.Status = 1) A order by ClubCodes asc";

            cmd = new SqlCommand(MSsql, con);
            con.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var clublist = new AllocationOverRides();

                string dtFrom = reader["DateFrom"].ToString();
                string dtTo = reader["DateTo"].ToString();


                string formattedDateFrom = DateTime.Parse(dtFrom).ToString("MM-dd-yyyy");
                string formattedDateTo = DateTime.Parse(dtTo).ToString("MM-dd-yyyy");

                clublist.ClubCode = Convert.ToInt32(reader["ClubCodes"]);
                clublist.ClubNames = Convert.ToString(reader["ClubNames"]);
                
                clublist.MultiplierAdjustment = Convert.ToInt32(reader["MultiplierAdjustment"]);
                clublist.Department = Convert.ToInt32(reader["Department"]);
                clublist.SubDepartment = Convert.ToInt32(reader["SubDepartment"]);
                clublist.ClassCode = Convert.ToInt32(reader["ClassName"]);
                clublist.SubClassCode = Convert.ToInt32(reader["SubClass"]);
                clublist.VendorCode = Convert.ToInt32(reader["Vendor"]);
                clublist.AllSearch = Convert.ToInt32(reader["AllSearch"]);
                clublist.DateFrom = formattedDateFrom;
                clublist.DateTo = formattedDateTo;
                alloclub.Add(clublist);
            }

            con.Close();
            return alloclub;


        }

        public List<AllocationOverRides> GetClubClassCode(int deptcode, int subdeptcode,int classcode)
        {

            List<AllocationOverRides> alloclub = new List<AllocationOverRides>();

            MSsql = $"select distinct(Clubcodes),ClubNames,Department,SubDepartment,ClassName,SubClass,MultiplierAdjustment,Vendor,AllSearch,DateFrom,DateTo from(" +
                $"select (case when A.ClubCode is null then 0 else A.Clubcode end) as ClubCodes,concat(A.ClubCode,'-',A.ClubName)as ClubNames," +
                $"(case when B.Department is null then {deptcode} else B.Department end) as Department," +
                $"(case when B.SubDepartment is null then {subdeptcode} else B.SubDepartment end) as SubDepartment," +
                $"(case when B.ClassName is null then {classcode} else B.ClassName end) as ClassName," +
                $"(case when B.SubClass is null then 0 else B.SubClass end) as SubClass," +
                $"(case when B.Vendor is null then 0 else B.Vendor end) as Vendor," +
                $"(case when B.AllSearch is null then 0 else B.AllSearch end) as AllSearch," +
                
                 $"(case when B.MultiplierAdjustment is null then 0 else B.MultiplierAdjustment end) as MultiplierAdjustment," +
                 $"(case when B.DateFrom is null then CAST(GETDATE() as Date) else B.DateFrom end) as DateFrom, " +
                $"(case when B.DateTo is null then CAST(GETDATE() as DATE) else B.DateTo end) as DateTo " +
                $" from AllocationClub A left join(select Department,SubDepartment,ClassName,SubClass, ClubCode,MultiplierAdjustment,Vendor,AllSearch,DateFrom,DateTo from AllocationOverRides B where Department = {deptcode} and SubDepartment = {subdeptcode} and ClassName = {classcode} and SubClass = 0 and Vendor = 0) B on A.ClubCode = B.ClubCode where A.Status = 1) A order by ClubCodes asc";

            cmd = new SqlCommand(MSsql, con);
            con.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var clublist = new AllocationOverRides();

                string dtFrom = reader["DateFrom"].ToString();
                string dtTo = reader["DateTo"].ToString();


                string formattedDateFrom = DateTime.Parse(dtFrom).ToString("MM-dd-yyyy");
                string formattedDateTo = DateTime.Parse(dtTo).ToString("MM-dd-yyyy");

                clublist.ClubCode = Convert.ToInt32(reader["ClubCodes"]);
                clublist.ClubNames = Convert.ToString(reader["ClubNames"]);
                
                clublist.MultiplierAdjustment = Convert.ToInt32(reader["MultiplierAdjustment"]);
                clublist.Department = Convert.ToInt32(reader["Department"]);
                clublist.SubDepartment = Convert.ToInt32(reader["SubDepartment"]);
                clublist.ClassCode = Convert.ToInt32(reader["ClassName"]);
                clublist.SubClassCode = Convert.ToInt32(reader["SubClass"]);
                clublist.VendorCode = Convert.ToInt32(reader["Vendor"]);
                clublist.AllSearch = Convert.ToInt32(reader["AllSearch"]);
                clublist.DateFrom = formattedDateFrom;
                clublist.DateTo = formattedDateTo;
                alloclub.Add(clublist);
            }

            con.Close();
            return alloclub;


        }

        public List<AllocationOverRides> GetClubSubClassCode(int deptcode, int subdeptcode, int classcode,int subclasscode)
        {

            List<AllocationOverRides> alloclub = new List<AllocationOverRides>();

            MSsql = $"select distinct(Clubcodes),ClubNames,Department,SubDepartment,ClassName,SubClass,MultiplierAdjustment,Vendor,AllSearch,DateFrom,DateTo from(" +
                $"select (case when A.ClubCode is null then 0 else A.Clubcode end) as ClubCodes,concat(A.ClubCode,'-',A.ClubName)as ClubNames," +
                $"(case when B.Department is null then {deptcode} else B.Department end) as Department," +
                $"(case when B.SubDepartment is null then {subdeptcode} else B.SubDepartment end) as SubDepartment," +
                $"(case when B.ClassName is null then {classcode} else B.ClassName end) as ClassName," +
                $"(case when B.SubClass is null then {subclasscode} else B.SubClass end) as SubClass," +
                $"(case when B.Vendor is null then 0 else B.Vendor end) as Vendor," +
                $"(case when B.AllSearch is null then 0 else B.AllSearch end) as AllSearch," +
               
                $"(case when B.MultiplierAdjustment is null then 0 else B.MultiplierAdjustment end) as MultiplierAdjustment," +
                $"(case when B.DateFrom is null then CAST(GETDATE() as Date) else B.DateFrom end) as DateFrom, " +
                $"(case when B.DateTo is null then CAST(GETDATE() as DATE) else B.DateTo end) as DateTo " +
                $" from AllocationClub A left join(select Department,SubDepartment,ClassName,SubClass, ClubCode,MultiplierAdjustment,Vendor,AllSearch,DateFrom,DateTo from AllocationOverRides B where Department = {deptcode} and SubDepartment = {subdeptcode} and ClassName = {classcode} and SubClass = {subclasscode} and Vendor = 0) B on A.ClubCode = B.ClubCode where A.Status = 1) A order by ClubCodes asc";

            cmd = new SqlCommand(MSsql, con);
            con.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var clublist = new AllocationOverRides();

                string dtFrom = reader["DateFrom"].ToString();
                string dtTo = reader["DateTo"].ToString();


                string formattedDateFrom = DateTime.Parse(dtFrom).ToString("MM-dd-yyyy");
                string formattedDateTo = DateTime.Parse(dtTo).ToString("MM-dd-yyyy");

                clublist.ClubCode = Convert.ToInt32(reader["ClubCodes"]);
                clublist.ClubNames = Convert.ToString(reader["ClubNames"]);
               
                clublist.MultiplierAdjustment = Convert.ToInt32(reader["MultiplierAdjustment"]);
                clublist.Department = Convert.ToInt32(reader["Department"]);
                clublist.SubDepartment = Convert.ToInt32(reader["SubDepartment"]);
                clublist.ClassCode = Convert.ToInt32(reader["ClassName"]);
                clublist.SubClassCode = Convert.ToInt32(reader["SubClass"]);
                clublist.VendorCode = Convert.ToInt32(reader["Vendor"]);
                clublist.AllSearch = Convert.ToInt32(reader["AllSearch"]);
                clublist.DateFrom = formattedDateFrom;
                clublist.DateTo = formattedDateTo;
                alloclub.Add(clublist);
            }

            con.Close();
            return alloclub;


        }

        public void UpdateOverSKU(int ClubCode,  int MultiplierAdjustment,int SKU,string DateFrom,string DateTo)
        {
            DateTime formattedDateFrom = DateTime.Parse(DateFrom);
            DateTime formattedDateTo = DateTime.Parse(DateTo);


            MSsql = "UPDATE [AllocationSKU] SET MultiplierAdjustment = @MultiplierAdjustment,DateFrom = @DateFrom, DateTo = @DateTo where SKU = @SKU and ClubCode = @ClubCode";
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
                Value = MultiplierAdjustment,
                ParameterName = "@MultiplierAdjustment",
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
                Value = formattedDateFrom,
                ParameterName = "@DateFrom",
                DbType = DbType.DateTime,
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = formattedDateTo,
                ParameterName = "@DateTo",
                DbType = DbType.DateTime,
            });


            cmd.ExecuteNonQuery();
            con.Close();


        }




        public bool VerifyOverInsert(int skuid, int clubcode)
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


        public void UpdateSKUVendor(int ClubCode,  int MultiplierAdjustment, int Vendor, string DateFrom, string DateTo)
        {
            DateTime formattedDateFrom = DateTime.Parse(DateFrom);
            DateTime formattedDateTo = DateTime.Parse(DateTo);


            MSsql = "UPDATE [AllocationSKU] SET MultiplierAdjustment = @MultiplierAdjustment, DateFrom = @DateFrom, DateTo = @DateTo where Vendor = @Vendor and ClubCode = @ClubCode";
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
                Value = MultiplierAdjustment,
                ParameterName = "@MultiplierAdjustment",
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
                Value = formattedDateFrom,
                ParameterName = "@DateFrom",
                DbType = DbType.DateTime,
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = formattedDateTo,
                ParameterName = "@DateTo",
                DbType = DbType.DateTime,
            });


            cmd.ExecuteNonQuery();
            con.Close();
        }


        public void UpdateSKUDept(int ClubCode, int MultiplierAdjustment, int Department, string DateFrom, string DateTo)
        {

            DateTime formattedDateFrom = DateTime.Parse(DateFrom);
            DateTime formattedDateTo = DateTime.Parse(DateTo);


            MSsql = "UPDATE [AllocationSKU] SET MultiplierAdjustment = @MultiplierAdjustment, DateFrom = @DateFrom, DateTo = @DateTo where Department = @Department and ClubCode = @ClubCode";
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
                Value = MultiplierAdjustment,
                ParameterName = "@MultiplierAdjustment",
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
                Value = formattedDateFrom,
                ParameterName = "@DateFrom",
                DbType = DbType.DateTime,
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = formattedDateTo,
                ParameterName = "@DateTo",
                DbType = DbType.DateTime,
            });


            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void UpdateSKUSubDept(int ClubCode,  int MultiplierAdjustment, int Department, int SubDepartment, string DateFrom, string DateTo)

        {
            DateTime formattedDateFrom = DateTime.Parse(DateFrom);
            DateTime formattedDateTo = DateTime.Parse(DateTo);


            MSsql = "UPDATE [AllocationSKU] SET MultiplierAdjustment = @MultiplierAdjustment, DateFrom = @DateFrom, DateTo = @DateTo where Department = @Department and ClubCode = @ClubCode and SubDepartment = @SubDepartment";
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
                Value = MultiplierAdjustment,
                ParameterName = "@MultiplierAdjustment",
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
                Value = formattedDateFrom,
                ParameterName = "@DateFrom",
                DbType = DbType.DateTime,
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = formattedDateTo,
                ParameterName = "@DateTo",
                DbType = DbType.DateTime,
            });


            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void UpdateSKUClass(int ClubCode,  int MultiplierAdjustment, int Department, int SubDepartment, int ClassName, string DateFrom, string DateTo)
        {
            DateTime formattedDateFrom = DateTime.Parse(DateFrom);
            DateTime formattedDateTo = DateTime.Parse(DateTo);

            MSsql = "UPDATE [AllocationSKU] SET MultiplierAdjustment = @MultiplierAdjustment, DateFrom = @DateFrom, DateTo = @DateTo where Department = @Department and ClubCode = @ClubCode and SubDepartment = @SubDepartment and ClassName = @ClassName";
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
                Value = MultiplierAdjustment,
                ParameterName = "@MultiplierAdjustment",
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
                Value = formattedDateFrom,
                ParameterName = "@DateFrom",
                DbType = DbType.DateTime,
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = formattedDateTo,
                ParameterName = "@DateTo",
                DbType = DbType.DateTime,
            });


            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void UpdateSKUSubClass(int ClubCode, int MultiplierAdjustment, int Department, int SubDepartment, int ClassName,int SubClass, string DateFrom, string DateTo)
        {
            DateTime formattedDateFrom = DateTime.Parse(DateFrom);
            DateTime formattedDateTo = DateTime.Parse(DateTo);

            MSsql = "UPDATE [AllocationSKU] SET MultiplierAdjustment = @MultiplierAdjustment, DateFrom = @DateFrom, DateTo = @DateTo where Department = @Department and ClubCode = @ClubCode and SubDepartment = @SubDepartment and ClassName = @ClassName and SubClass = @SubClass";
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
                Value = MultiplierAdjustment,
                ParameterName = "@MultiplierAdjustment",
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
                Value = formattedDateFrom,
                ParameterName = "@DateFrom",
                DbType = DbType.DateTime,
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = formattedDateTo,
                ParameterName = "@DateTo",
                DbType = DbType.DateTime,
            });

            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void UpdateOverVendor(int ClubCode, int MultiplierAdjustment, int Vendor,int Department,int SubDepartment,int ClassName, int SubClass,string DateFrom, string DateTo, string UserID, string Todays)
        {

           


            DateTime formattedDateFrom = DateTime.Parse(DateFrom);
            DateTime formattedDateTo = DateTime.Parse(DateTo);
            DateTime formattedday = DateTime.Parse(Todays);

            MSsql = "UPDATE [AllocationOverRides] SET MultiplierAdjustment = @MultiplierAdjustment, DateFrom = @DateFrom, DateTo = @DateTo,UserID = @UserID, DateModified = @Todays where Vendor = @Vendor and ClubCode = @ClubCode and Department = @Department and SubDepartment = @SubDepartment and ClassName = ClassName and SubClass = @SubClass ";
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
                Value = MultiplierAdjustment,
                ParameterName = "@MultiplierAdjustment",
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
                Value = formattedDateFrom,
                ParameterName = "@DateFrom",
                DbType = DbType.DateTime,
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = formattedDateTo,
                ParameterName = "@DateTo",
                DbType = DbType.DateTime,
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


            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void UpdateOverSearchAll(int ClubCode,int MultiplierAdjustment, int AllSearch,string DateFrom,string DateTo,string UserID,string Todays)
        {
            DateTime formattedDateFrom = DateTime.Parse(DateFrom);
            DateTime formattedDateTo = DateTime.Parse(DateTo);
            DateTime formattedday = DateTime.Parse(Todays);

            MSsql = "UPDATE [AllocationOverRides] SET MultiplierAdjustment = @MultiplierAdjustment, DateFrom = @DateFrom,DateTo = @DateTo,UserID = @UserID, DateModified = @Todays where ClubCode = @ClubCode and AllSearch = @AllSearch";
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
                Value = AllSearch,
                ParameterName = "@AllSearch",
                DbType = DbType.Int32
            });
            
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = MultiplierAdjustment,
                ParameterName = "@MultiplierAdjustment",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = formattedDateFrom,
                ParameterName = "@DateFrom",
                DbType = DbType.DateTime,
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = formattedDateTo,
                ParameterName = "@DateTo",
                DbType = DbType.DateTime,
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



            cmd.ExecuteNonQuery();
            con.Close();
        }


        public void UpdateOverAll(int ClubCode,  int MultiplierAdjustment,string DateFrom,string DateTo, string UserID, string Todays)
        {
            DateTime formattedDateFrom = DateTime.Parse(DateFrom);
            DateTime formattedDateTo = DateTime.Parse(DateTo);
            DateTime formattedday = DateTime.Parse(Todays);

            MSsql = "UPDATE [AllocationOverRides] SET MultiplierAdjustment = @MultiplierAdjustment,DateFrom = @DateFrom,DateTo = @DateTo,UserID = @UserID, DateModified = @Todays where ClubCode = @ClubCode";
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
                Value = MultiplierAdjustment,
                ParameterName = "@MultiplierAdjustment",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = formattedDateFrom,
                ParameterName = "@DateFrom",
                DbType = DbType.DateTime,
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = formattedDateTo,
                ParameterName = "@DateTo",
                DbType = DbType.DateTime,
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

            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void UpdateSKUAll(int ClubCode,  int MultiplierAdjustment, string DateFrom, string DateTo)
        {
            DateTime formattedDateFrom = DateTime.Parse(DateFrom);
            DateTime formattedDateTo = DateTime.Parse(DateTo);

            MSsql = "UPDATE [AllocationSKU] SET MultiplierAdjustment = @MultiplierAdjustment,DateFrom = @DateFrom,DateTo = @DateTo where ClubCode = @ClubCode";
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
                Value = MultiplierAdjustment,
                ParameterName = "@MultiplierAdjustment",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = formattedDateFrom,
                ParameterName = "@DateFrom",
                DbType = DbType.DateTime,
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = formattedDateTo,
                ParameterName = "@DateTo",
                DbType = DbType.DateTime,
            });


            cmd.ExecuteNonQuery();
            con.Close();
        }

        public bool InsertOverVendor(int ClubCode, int MultiplierAdjustment, int Vendor,int Department,int SubDepartment, int ClassName, int SubClass,int AllSearch,string DateFrom, string DateTo, string UserID, string Todays)
        {

            try
            {
                DateTime formattedDateFrom = DateTime.Parse(DateFrom);
                DateTime formattedDateTo = DateTime.Parse(DateTo);
                DateTime formattedday = DateTime.Parse(Todays);

                SqlCommand cmd = new SqlCommand("INSERT AllocationOverRides(Vendor,ClubCode,MultiplierAdjustment,Department,SubDepartment,ClassName,SubClass,AllSearch,DateFrom,DateTo,UserID, DateModified) VALUES(@Vendor,@ClubCode,@MultiplierAdjustment,@Department,@SubDepartment,@ClassName,@SubClass,@AllSearch,@DateFrom,@DateTo, @UserID,@Todays)", con);
                cmd.CommandType = CommandType.Text;
                con.Open();
                
                cmd.Parameters.AddWithValue("@ClubCode", ClubCode);
               
                cmd.Parameters.AddWithValue("@MultiplierAdjustment", MultiplierAdjustment);
                cmd.Parameters.AddWithValue("@Vendor", Vendor);
                cmd.Parameters.AddWithValue("@Department", Department);
                cmd.Parameters.AddWithValue("@SubDepartment", SubDepartment);
                cmd.Parameters.AddWithValue("@ClassName", ClassName);
                cmd.Parameters.AddWithValue("@SubClass", SubClass);
                cmd.Parameters.AddWithValue("@AllSearch", AllSearch);
                cmd.Parameters.AddWithValue("@DateFrom", formattedDateFrom);
                cmd.Parameters.AddWithValue("@DateTo", formattedDateTo);
                cmd.Parameters.AddWithValue("@UserID", UserID);
                cmd.Parameters.AddWithValue("@Todays", formattedday);
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

        public bool InsertOverSearchAll(int ClubCode,  int MultiplierAdjustment, int AllSearch, int Department, int SubDepartment, int ClassName, int SubClass,int Vendor,string DateFrom,string DateTo, string UserID, string Todays)
        {

            try
            {
                DateTime formattedDateFrom = DateTime.Parse(DateFrom);
                DateTime formattedDateTo = DateTime.Parse(DateTo);
                DateTime formattedday = DateTime.Parse(Todays);

                SqlCommand cmd = new SqlCommand("INSERT AllocationOverRides(ClubCode,MultiplierAdjustment,AllSearch,Department,SubDepartment,ClassName,SubClass,Vendor,DateFrom,DateTo,UserID, DateModified) VALUES(@ClubCode,@MultiplierAdjustment,@AllSearch,@Department,@SubDepartment,@ClassName,@SubClass,@Vendor,@DateFrom,@DateTo, @UserID,@Todays)", con);
                cmd.CommandType = CommandType.Text;
                con.Open();

                cmd.Parameters.AddWithValue("@ClubCode", ClubCode);
                
                cmd.Parameters.AddWithValue("@MultiplierAdjustment", MultiplierAdjustment);
                cmd.Parameters.AddWithValue("@AllSearch", AllSearch);
               
                cmd.Parameters.AddWithValue("@Department", Department);
                cmd.Parameters.AddWithValue("@SubDepartment", SubDepartment);
                cmd.Parameters.AddWithValue("@ClassName", ClassName);
                cmd.Parameters.AddWithValue("@SubClass", SubClass);
                cmd.Parameters.AddWithValue("@Vendor", Vendor);
                cmd.Parameters.AddWithValue("@DateFrom", formattedDateFrom);
                cmd.Parameters.AddWithValue("@DateTo", formattedDateTo);
                cmd.Parameters.AddWithValue("@UserID", UserID);
                cmd.Parameters.AddWithValue("@Todays", formattedday);

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


        public bool VerifySKUInsertVendor(int vendorid, int clubcode)
        {
            MSsql = $"SELECT Vendor,Clubcode from AllocationSKU where Vendor = @vendorid and ClubCode = @clubcode";
            cmd = new SqlCommand(MSsql, con);
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = vendorid,
                ParameterName = "@vendorid",
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


        public bool VerifySKUInsertDept(int clubcode, int deptid)
        {
            MSsql = $"SELECT Department,Clubcode from AllocationSKU where Department = @deptid and ClubCode = @clubcode";
            cmd = new SqlCommand(MSsql, con);
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = deptid,
                ParameterName = "@deptid",
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


        public bool VerifySKUInsertSubDept(int clubcode, int deptid, int subdeptid)
        {
            MSsql = $"SELECT Department,Clubcode from AllocationSKU where Department = @deptid and ClubCode = @clubcode and SubDepartment = @subdeptid";
            cmd = new SqlCommand(MSsql, con);
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = deptid,
                ParameterName = "@deptid",
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
                Value = subdeptid,
                ParameterName = "@subdeptid",
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


        public bool VerifySKUInsertClass(int clubcode, int deptid, int subdeptid, int classid)
        {
            MSsql = $"SELECT Department,Clubcode from AllocationSKU where Department = @deptid and ClubCode = @clubcode and SubDepartment = @subdeptid and ClassName = @classid";
            cmd = new SqlCommand(MSsql, con);
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = deptid,
                ParameterName = "@deptid",
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
                Value = subdeptid,
                ParameterName = "@subdeptid",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = classid,
                ParameterName = "@classid",
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


        public bool VerifySKUInsertSubClass(int clubcode, int deptid, int subdeptid, int classid, int subclassid)
        {
            MSsql = $"SELECT Department,Clubcode from AllocationSKU where Department = @deptid and ClubCode = @clubcode and SubDepartment = @subdeptid and ClassName = @classid and SubClass = @subclassid";
            cmd = new SqlCommand(MSsql, con);
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = deptid,
                ParameterName = "@deptid",
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
                Value = subdeptid,
                ParameterName = "@subdeptid",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = classid,
                ParameterName = "@classid",
                DbType = DbType.Int32
            });

            cmd.Parameters.Add(new SqlParameter()
            {
                Value = subclassid,
                ParameterName = "@subclassid",
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

        public bool VerifyOverInsertAllSearch(int clubcode,int searchid)
        {
            MSsql = $"SELECT ClubCode,AllSearch from AllocationOverRides where ClubCode = @clubcode and AllSearch = @searchid ";
            cmd = new SqlCommand(MSsql, con);
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = searchid,
                ParameterName = "@searchid",
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

        public bool VerifyOverInsertVendor(int vendorid, int clubcode)
        {
            MSsql = $"SELECT Vendor,Clubcode from AllocationOverRides where Vendor = @vendorid and ClubCode = @clubcode";
            cmd = new SqlCommand(MSsql, con);
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = vendorid,
                ParameterName = "@vendorid",
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

        public bool VerifyOverInsertDept(int clubcode, int deptid)
        {
            MSsql = $"SELECT Clubcode,Department from AllocationOverRides where Department = @deptid and ClubCode = @clubcode ";
            cmd = new SqlCommand(MSsql, con);

            cmd.Parameters.Add(new SqlParameter()
            {
                Value = clubcode,
                ParameterName = "@clubcode",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = deptid,
                ParameterName = "@deptid",
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

        public bool VerifyOverInsertSubDept(int clubcode, int deptid, int subdepartment)
        {
            MSsql = $"SELECT Clubcode,Department,SubDepartment from AllocationOverRides where Department = @deptid and SubDepartment = @subdepartment and ClubCode = @clubcode";
            cmd = new SqlCommand(MSsql, con);

            cmd.Parameters.Add(new SqlParameter()
            {
                Value = clubcode,
                ParameterName = "@clubcode",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = deptid,
                ParameterName = "@deptid",
                DbType = DbType.Int32
            });

            cmd.Parameters.Add(new SqlParameter()
            {
                Value = subdepartment,
                ParameterName = "@subdepartment",
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

        public bool VerifyOverInsertClass(int clubcode, int deptid, int subdepartment, int classcode)
        {
            MSsql = $"SELECT Clubcode,Department,SubDepartment,ClassName from AllocationOverRides where Department = @deptid and  SubDepartment = @subdepartment and ClassName = @classcode  and ClubCode = @clubcode ";
            cmd = new SqlCommand(MSsql, con);

            cmd.Parameters.Add(new SqlParameter()
            {
                Value = clubcode,
                ParameterName = "@clubcode",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = deptid,
                ParameterName = "@deptid",
                DbType = DbType.Int32
            });
            
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = subdepartment,
                ParameterName = "@subdepartment",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = classcode,
                ParameterName = "@classcode",
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

        public bool VerifyOverInsertSubClass(int clubcode, int deptid, int subdepartment, int classcode,int subclasscode)
        {
            MSsql = $"SELECT Clubcode,Department,SubDepartment,ClassName from AllocationOverRides where Department = @deptid and  SubDepartment = @subdepartment and ClassName = @classcode and SubClass = @subclasscode and ClubCode = @clubcode ";
            cmd = new SqlCommand(MSsql, con);

            cmd.Parameters.Add(new SqlParameter()
            {
                Value = clubcode,
                ParameterName = "@clubcode",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = deptid,
                ParameterName = "@deptid",
                DbType = DbType.Int32
            });

            cmd.Parameters.Add(new SqlParameter()
            {
                Value = subdepartment,
                ParameterName = "@subdepartment",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = classcode,
                ParameterName = "@classcode",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = subclasscode,
                ParameterName = "@subclasscode",
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

        public void UpdateOverDept(int ClubCode, int MultiplierAdjustment, int Department,string DateFrom, string DateTo, string UserID, string Todays)
        {
            DateTime formattedDateFrom = DateTime.Parse(DateFrom);
            DateTime formattedDateTo = DateTime.Parse(DateTo);
            DateTime formattedday = DateTime.Parse(Todays);

            MSsql = "UPDATE [AllocationOverRides] SET MultiplierAdjustment = @MultiplierAdjustment,DateFrom = @DateFrom, DateTo = @DateTo,UserID = @UserID, DateModified = @Todays  where ClubCode = @ClubCode and Department = @Department ";
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
                Value = MultiplierAdjustment,
                ParameterName = "@MultiplierAdjustment",
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
                Value = formattedDateFrom,
                ParameterName = "@DateFrom",
                DbType = DbType.DateTime,
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = formattedDateTo,
                ParameterName = "@DateTo",
                DbType = DbType.DateTime,
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



            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void UpdateOverSubDept(int ClubCode,  int MultiplierAdjustment, int Department, int SubDepartment,string DateFrom,string DateTo, string UserID, string Todays)
        {

            DateTime formattedDateFrom = DateTime.Parse(DateFrom);
            DateTime formattedDateTo = DateTime.Parse(DateTo);
            DateTime formattedday = DateTime.Parse(Todays);

            MSsql = "UPDATE [AllocationOverRides] SET MultiplierAdjustment = @MultiplierAdjustment,DateFrom = @DateFrom,DateTo = @DateTo,UserID = @UserID, DateModified = @Todays where Department = @Department and SubDepartment = @SubDepartment and ClubCode = @ClubCode";
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
                Value = MultiplierAdjustment,
                ParameterName = "@MultiplierAdjustment",
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
                Value = formattedDateFrom,
                ParameterName = "@DateFrom",
                DbType = DbType.DateTime,
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = formattedDateTo,
                ParameterName = "@DateTo",
                DbType = DbType.DateTime,
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

            cmd.ExecuteNonQuery();
            con.Close();
        }



        public void UpdateOverClass(int ClubCode, int MultiplierAdjustment, int Department, int SubDepartment, int ClassName,string DateFrom,string DateTo, string UserID, string Todays)
        {
            DateTime formattedDateFrom = DateTime.Parse(DateFrom);
            DateTime formattedDateTo = DateTime.Parse(DateTo);
            DateTime formattedday = DateTime.Parse(Todays);

            MSsql = "UPDATE [AllocationOverRides] SET MultiplierAdjustment = @MultiplierAdjustment,DateFrom = @DateFrom, DateTo = @DateTo,UserID = @UserID, DateModified = @Todays where Department = @Department and ClubCode = @ClubCode and SubDepartment = @SubDepartment and ClassName = @ClassName";
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
                Value = MultiplierAdjustment,
                ParameterName = "@MultiplierAdjustment",
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
                Value = formattedDateFrom,
                ParameterName = "@DateFrom",
                DbType = DbType.DateTime,
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = formattedDateTo,
                ParameterName = "@DateTo",
                DbType = DbType.DateTime,
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


            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void UpdateOverSubClass(int ClubCode,  int MultiplierAdjustment, int Department, int SubDepartment, int ClassName, int SubClass,string DateFrom,string DateTo, string UserID, string Todays)
        {
            DateTime formattedDateFrom = DateTime.Parse(DateFrom);
            DateTime formattedDateTo = DateTime.Parse(DateTo);
            DateTime formattedday = DateTime.Parse(Todays);

            MSsql = "UPDATE [AllocationOverRides] SET MultiplierAdjustment = @MultiplierAdjustment, DateFrom = @DateFrom,DateTo = @DateTo,UserID = @UserID, DateModified = @Todays where Department = @Department and SubDepartment = @Subdepartment and ClassName = @ClassName and Subclass = @SubClass and ClubCode = @ClubCode";
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
                Value = MultiplierAdjustment,
                ParameterName = "@MultiplierAdjustment",
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
                Value = formattedDateFrom,
                ParameterName = "@DateFrom",
                DbType = DbType.DateTime,
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = formattedDateTo,
                ParameterName = "@DateTo",
                DbType = DbType.DateTime,
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


            cmd.ExecuteNonQuery();
            con.Close();
        }

        public bool InsertOverDept(int ClubCode, int MultiplierAdjustment, int Department, int SubDepartment, int ClassName, int SubClass,int Vendor,int AllSearch,string DateFrom,string DateTo, string UserID, string Todays)
        {

            try
            {
                DateTime formattedDateFrom = DateTime.Parse(DateFrom);
                DateTime formattedDateTo = DateTime.Parse(DateTo);
                DateTime formattedday = DateTime.Parse(Todays);

                SqlCommand cmd = new SqlCommand("INSERT AllocationOverRides(ClubCode,MultiplierAdjustment,Department,SubDepartment,ClassName,SubClass,Vendor,AllSearch,DateFrom,DateTo,UserID, DateModified) VALUES(@ClubCode,@MultiplierAdjustment,@Department,@SubDepartment,@ClassName,@SubClass,@Vendor,@AllSearch,@DateFrom,@DateTo, @UserID, @Todays)", con);
                cmd.CommandType = CommandType.Text;
                con.Open();

                cmd.Parameters.AddWithValue("@ClubCode", ClubCode);
                
                cmd.Parameters.AddWithValue("@MultiplierAdjustment", MultiplierAdjustment);
                cmd.Parameters.AddWithValue("@Department", Department);
                cmd.Parameters.AddWithValue("@SubDepartment", SubDepartment);
                cmd.Parameters.AddWithValue("@ClassName", ClassName);
                cmd.Parameters.AddWithValue("@SubClass", SubClass);
                cmd.Parameters.AddWithValue("@Vendor", Vendor);
                cmd.Parameters.AddWithValue("@AllSearch", AllSearch);
                cmd.Parameters.AddWithValue("@DateFrom", formattedDateFrom);
                cmd.Parameters.AddWithValue("@DateTo", formattedDateTo);
                cmd.Parameters.AddWithValue("@UserID", UserID);
                cmd.Parameters.AddWithValue("@Todays", formattedday);

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

        public bool InsertOverSubDept(int ClubCode, int MultiplierAdjustment, int Department, int SubDepartment,int ClassName, int SubClass,int Vendor,int AllSearch,string DateFrom,string DateTo, string UserID, string Todays)
        {

            try
            {
                DateTime formattedDateFrom = DateTime.Parse(DateFrom);
                DateTime formattedDateTo = DateTime.Parse(DateTo);
                DateTime formattedday = DateTime.Parse(Todays);

                SqlCommand cmd = new SqlCommand("INSERT AllocationOverRides(ClubCode,MultiplierAdjustment,Department,SubDepartment,ClassName,SubClass,Vendor,AllSearch,DateFrom,DateTo,UserID, DateModified) VALUES(@ClubCode,@MultiplierAdjustment,@Department,@SubDepartment,@ClassName,@SubClass,@Vendor,@AllSearch,@DateFrom,@DateTo, @UserID,@Todays)", con);
                cmd.CommandType = CommandType.Text;
                con.Open();

                cmd.Parameters.AddWithValue("@ClubCode", ClubCode);
                
                cmd.Parameters.AddWithValue("@MultiplierAdjustment", MultiplierAdjustment);
                cmd.Parameters.AddWithValue("@Department", Department);
                cmd.Parameters.AddWithValue("@SubDepartment", SubDepartment);
                cmd.Parameters.AddWithValue("@ClassName", ClassName);
                cmd.Parameters.AddWithValue("@SubClass", SubClass);
                cmd.Parameters.AddWithValue("@Vendor", Vendor);
                cmd.Parameters.AddWithValue("@AllSearch", AllSearch);
                cmd.Parameters.AddWithValue("@DateFrom", formattedDateFrom);
                cmd.Parameters.AddWithValue("@DateTo", formattedDateTo);
                cmd.Parameters.AddWithValue("@UserID", UserID);
                cmd.Parameters.AddWithValue("@Todays", formattedday);


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

        public bool InsertOverClass(int ClubCode, int MultiplierAdjustment, int Department, int SubDepartment, int ClassName,int SubClass,int Vendor,int AllSearch,string DateFrom,string DateTo, string UserID, string Todays)
        {

            try
            {
                DateTime formattedDateFrom = DateTime.Parse(DateFrom);
                DateTime formattedDateTo = DateTime.Parse(DateTo);
                DateTime formattedday = DateTime.Parse(Todays);

                SqlCommand cmd = new SqlCommand("INSERT AllocationOverRides(ClubCode,MultiplierAdjustment,Department,SubDepartment,ClassName,SubClass,Vendor,AllSearch,DateFrom,DateTo,UserID," +
                    "DateModified) VALUES(@ClubCode,@MultiplierAdjustment,@Department,@SubDepartment,@ClassName,@SubClass,@Vendor,@AllSearch,@DateFrom,@DateTo, @UserID,@Todays)", con);
                cmd.CommandType = CommandType.Text;
                con.Open();

                cmd.Parameters.AddWithValue("@ClubCode", ClubCode);
                
                cmd.Parameters.AddWithValue("@MultiplierAdjustment", MultiplierAdjustment);
                cmd.Parameters.AddWithValue("@Department", Department);
                cmd.Parameters.AddWithValue("@SubDepartment", SubDepartment);
                cmd.Parameters.AddWithValue("@ClassName", ClassName);
                cmd.Parameters.AddWithValue("@SubClass", SubClass);
                cmd.Parameters.AddWithValue("@Vendor", Vendor);
                cmd.Parameters.AddWithValue("@AllSearch", AllSearch);
                cmd.Parameters.AddWithValue("@DateFrom", formattedDateFrom);
                cmd.Parameters.AddWithValue("@DateTo", formattedDateTo);
                cmd.Parameters.AddWithValue("@UserID", UserID);
                cmd.Parameters.AddWithValue("@Todays", formattedday);
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

        public bool InsertOverSubClass(int ClubCode, int MultiplierAdjustment, int Department, int SubDepartment, int ClassName, int SubClass,int Vendor,int AllSearch,string DateFrom,string DateTo, string UserID, string Todays)
        {

            try
            {
                DateTime formattedDateFrom = DateTime.Parse(DateFrom);
                DateTime formattedDateTo = DateTime.Parse(DateTo);
                DateTime formattedday = DateTime.Parse(Todays);

                SqlCommand cmd = new SqlCommand("INSERT AllocationOverRides(ClubCode,MultiplierAdjustment,Department,SubDepartment,ClassName,SubClass,Vendor,AllSearch,DateFrom,DateTo,UserID, DateModified) VALUES(@ClubCode,@MultiplierAdjustment,@Department,@SubDepartment,@ClassName,@SubClass,@Vendor,@AllSearch,@DateFrom,@DateTo, @UserID,@Todays)", con);
                cmd.CommandType = CommandType.Text;
                con.Open();

                cmd.Parameters.AddWithValue("@ClubCode", ClubCode);
                
                cmd.Parameters.AddWithValue("@MultiplierAdjustment", MultiplierAdjustment);
                cmd.Parameters.AddWithValue("@Department", Department);
                cmd.Parameters.AddWithValue("@SubDepartment", SubDepartment);
                cmd.Parameters.AddWithValue("@ClassName", ClassName);
                cmd.Parameters.AddWithValue("@SubClass", SubClass);
                cmd.Parameters.AddWithValue("@Vendor", Vendor);
                cmd.Parameters.AddWithValue("@AllSearch", AllSearch);
                cmd.Parameters.AddWithValue("@DateFrom", formattedDateFrom);
                cmd.Parameters.AddWithValue("@DateTo", formattedDateTo);
                cmd.Parameters.AddWithValue("@UserID", UserID);
                cmd.Parameters.AddWithValue("@Todays", formattedday);
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

        public int SKUAvailable(long SKUId)
        {
            MSsql = $"SELECT SKU FROM AllocationSKU WHERE SKU = {SKUId}";
            cmd = new SqlCommand(MSsql, con);
            con.Open();
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                
                con.Close();
                return 1;

            }
            else
            {
                con.Close();
                return 0;
            }
        }

        public int VendorAvailable(int VendorId)
        {
            MSsql = $"SELECT SKU FROM AllocationSKU WHERE Vendor = {VendorId}";
            cmd = new SqlCommand(MSsql, con);
            con.Open();
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {

                con.Close();
                return 1;

            }
            else
            {
                con.Close();
                return 0;
            }
        }

        public int DeptAvailable(int Deptcode)
        {
            MSsql = $"SELECT SKU FROM AllocationSKU WHERE Department = {Deptcode}";
            cmd = new SqlCommand(MSsql, con);
            con.Open();
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {

                con.Close();
                return 1;

            }
            else
            {
                con.Close();
                return 0;
            }
        }

        public int SubDeptAvailable(int Deptcode,int SubDeptcode)
        {
            MSsql = $"SELECT SKU FROM AllocationSKU WHERE Department = {Deptcode} and SubDepartment = {SubDeptcode}";
            cmd = new SqlCommand(MSsql, con);
            con.Open();
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {

                con.Close();
                return 1;

            }
            else
            {
                con.Close();
                return 0;
            }
        }

        public int ClassAvailable(int Deptcode, int SubDeptcode,int Classcode)
        {
            MSsql = $"SELECT SKU FROM AllocationSKU WHERE Department = {Deptcode} and SubDepartment = {SubDeptcode} and ClassName = {Classcode}";
            cmd = new SqlCommand(MSsql, con);
            con.Open();
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {

                con.Close();
                return 1;

            }
            else
            {
                con.Close();
                return 0;
            }
        }

        public int SubClassAvailable(int Deptcode, int SubDeptcode, int Classcode,int SubClasscode)
        {
            MSsql = $"SELECT SKU FROM AllocationSKU WHERE Department = {Deptcode} and SubDepartment = {SubDeptcode} and ClassName = {Classcode} and SubClass = {SubClasscode}";
            cmd = new SqlCommand(MSsql, con);
            con.Open();
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {

                con.Close();
                return 1;

            }
            else
            {
                con.Close();
                return 0;
            }
        }

       


    }
}