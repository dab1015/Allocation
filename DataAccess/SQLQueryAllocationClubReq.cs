using Microsoft.AspNet.Identity;
using Microsoft.SqlServer.Server;
using SNRWMSPortal.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Web;

namespace SNRWMSPortal.DataAccess
{
    public class SQLQueryAllocationClubReq
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["SQLConnect"].ToString());

        OleDbConnection conmms = new OleDbConnection(ConfigurationManager.ConnectionStrings["MMSConnect"].ToString());


        public string MSsql;
        SqlCommand cmd;
        SqlDataReader reader;

        OleDbCommand olecmd;
        OleDbDataReader olereader;



        public List<AllocationClub> GetClubRequest(int clubcode,string status)
        {

            List<AllocationClub> alloclub = new List<AllocationClub>();

            MSsql = $"SELECT  A.SKU,A.Clubcode,A.Description,A.Reason,B.Reason as Reasons,A.Status,A.Quantity,A.PQty,A.RequestedDate,A.Remarks,A.PConfig FROM AllocationClubRequest A inner join AllocationReason B on A.Reason = B.Code where A.Clubcode = {clubcode} and A.Status = '{status}' ";

            cmd = new SqlCommand(MSsql, con);
            con.Open();
            reader = cmd.ExecuteReader();



            while (reader.Read())
            {
                var clublist = new AllocationClub();

                string dtRequest = reader["RequestedDate"].ToString();
              //  string dtApproved = reader["ApprovedDate"].ToString();
                // string dtTo = reader["DateTo"].ToString();
                string formattedDateRequest = DateTime.Parse(dtRequest).ToString("MM-dd-yyyy");
              //  string formattedDateApproved = DateTime.Parse(dtApproved).ToString("MM-dd-yyyy");
                //  string formattedDateTo = DateTime.Parse(dtTo).ToString("MM-dd-yyyy");
                clublist.CLubCode = Convert.ToInt32(reader["ClubCode"]);
                clublist.SKU = Convert.ToInt64(reader["SKU"]);
                clublist.Description = Convert.ToString(reader["Description"]);
                clublist.Reason = Convert.ToInt32(reader["Reason"]);
                clublist.Reasons = Convert.ToString(reader["Reasons"]);
                clublist.Status = Convert.ToString(reader["Status"]);
                clublist.Quantity = Convert.ToInt32(reader["Quantity"]);
                clublist.PQty = Convert.ToInt32(reader["PQty"]);
                clublist.RequestedDate = formattedDateRequest;
               // clublist.ApprovedDate = formattedDateApproved;
                clublist.Remarks = Convert.ToString(reader["Remarks"]);
                clublist.PConfig = Convert.ToString(reader["PConfig"]);
                // clublist.DateFrom = formattedDateFrom;
                // clublist.DateTo = formattedDateTo;

                alloclub.Add(clublist);
            }

            con.Close();
            return alloclub;


        }




        public AllocationClub SKUDes(long _skuId, int clubcode)
        {

            var skuslist = new AllocationClub();



            MSsql = $"select B.INUMBR,B.IDESCR,A.IBHAND,B.ISTDPK,(B.IVPLTI * B.IVPLHI * B.ISTDPK) as Pallet,(B.IVPLTI * B.ISTDPK) as Layer,B.ISTDPK,B.IVPLTI,B.IVPLHI from mmjdalib.INVBAL A inner join  mmjdalib.invmst B on A.INUMBR = B.INUMBR WHERE  B.INUMBR = {_skuId} and A.ISTORE = {clubcode}";
            olecmd = new OleDbCommand(MSsql, conmms);
            conmms.Open();
            olereader = olecmd.ExecuteReader();
            if (olereader.Read())
            {
                

                skuslist.SKU = Convert.ToInt32(olereader["INUMBR"]);
                skuslist.Description = olereader["IDESCR"].ToString();
                skuslist.Pallet = Convert.ToInt32(olereader["Pallet"]);
                skuslist.Layer = Convert.ToInt32(olereader["Layer"]);
                skuslist.IVPLTI = Convert.ToInt32(olereader["IVPLTI"]);
                skuslist.IVPLHI = Convert.ToInt32(olereader["IVPLHI"]);
                skuslist.ISTDPK = Convert.ToDouble(olereader["ISTDPK"]);
                skuslist.StoreOH = Convert.ToDouble(olereader["IBHAND"]);
                skuslist.PConfig = Convert.ToString(olereader["Pallet"] + "_" + olereader["Layer"] + "_" + olereader["ISTDPK"]);

                //double weeklysales = (skuslist.StoreOH + skuslist.InTransit) / skuslist.AveSales;
                //skuslist.StoreWS = weeklysales;

            }
            conmms.Close();

            return skuslist;


        }


        public AllocationClub SKUOnHandDCM(long _skuId)
        {

            var skuslist = new AllocationClub();



            MSsql = $"select INUMBR,IBHAND from mmjdalib.invbal where INUMBR = {_skuId} and ISTORE = 880";
            olecmd = new OleDbCommand(MSsql, conmms);
            conmms.Open();
            olereader = olecmd.ExecuteReader();
            if (olereader.Read())
            {
                skuslist.SKU = Convert.ToInt32(olereader["INUMBR"]);
                skuslist.DCMOH = Convert.ToDouble(olereader["IBHAND"]);
            }
            conmms.Close();

            return skuslist;
        }


        public AllocationClub SKUWS(long _skuId, int clubcode)
        {

            var skuslist = new AllocationClub();



            MSsql = $"select INUMBR,IBINTQ from  mmjdalib.INVBAL where INUMBR  = {_skuId} and ISTORE = {clubcode}";
            olecmd = new OleDbCommand(MSsql, conmms);
            conmms.Open();
            olereader = olecmd.ExecuteReader();
            if (olereader.Read())
            {


                skuslist.SKU = Convert.ToInt32(olereader["INUMBR"]);
               
                
                skuslist.InTransit = Convert.ToDouble(olereader["IBINTQ"]);
               


                //double weeklysales = (skuslist.StoreOH + skuslist.InTransit) / skuslist.AveSales;
                //skuslist.StoreWS = weeklysales;

            }
            conmms.Close();

            return skuslist;


        }


        public AllocationClub SKUWS_Ave(long _skuId, int clubcode)
        {

            var skuslist = new AllocationClub();



            MSsql = $"select SKU,AverageSales from Allocation_AverageSales where SKU = {_skuId} and Clubcode = {clubcode}";
            cmd = new SqlCommand(MSsql, con);
            con.Open();
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {


                skuslist.SKU = Convert.ToInt32(reader["SKU"]);
                skuslist.AveSales = Convert.ToDouble(reader["AverageSales"]);


            }
            con.Close();

            return skuslist;


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

        public List<AllocationClub> GetReason()
        {
            List<AllocationClub> configlist = new List<AllocationClub>();
            SqlCommand cmd = new SqlCommand("SELECT *  From AllocationReason", con);
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            con.Open();
            sd.Fill(dt);
            con.Close();
            foreach (DataRow dr in dt.Rows)
            {
                configlist.Add(
                    new AllocationClub
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        Reason = Convert.ToInt32(dr["Code"]),
                        Reasons = Convert.ToString(dr["Reason"])


                    });
            }
            return configlist;
        }

        public AllocationClub SKUConfig(long _skuId, int clubcode)
        {

            var skuslist = new AllocationClub();


            MSsql = $"SELECT A.DConfig,B.DConfig as DConfigName FROM AllocationSKU A inner join DistributionConfig B on A.DConfig = B.Code WHERE A.SKU  = {_skuId} and A.ClubCode = {clubcode} ";
            cmd = new SqlCommand(MSsql, con);
            con.Open();
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {

                skuslist.DConfig = (int)reader["DConfig"];
                skuslist.DConfigName = (string)reader["DConfigName"];



            }
            con.Close();
            return skuslist;


        }

        public AllocationClub SKUClubQunatity(long _skuId, int clubcode)
        {

            var skuslist = new AllocationClub();


            MSsql = $"SELECT (case when A.RequestedDate is null then CAST(GETDATE() as DATE) else A.RequestedDate end) as RequestedDate, " +
                $"(case when A.Reason is null then 0 else A.Reason end) as Reason,A.Status,A.Quantity,A.Remarks,B.Reason as ReasonName FROM AllocationClubRequest A inner join AllocationReason B on A.Reason = B.Code WHERE A.SKU  = {_skuId} and A.ClubCode = {clubcode} and A.Status = 'PENDING' ";
            cmd = new SqlCommand(MSsql, con);
            con.Open();
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {

                string dtRequest = reader["RequestedDate"].ToString();
                string formattedDateRequest = DateTime.Parse(dtRequest).ToString("MM-dd-yyyy");

                skuslist.Reason = (int)reader["Reason"];
                skuslist.Status = reader["Status"].ToString();
                skuslist.Quantity = (int)reader["Quantity"];
                
                skuslist.RequestedDate = formattedDateRequest;
                skuslist.Remarks = reader["Remarks"].ToString();
                skuslist.ReasonName = reader["ReasonName"].ToString();



            }
            con.Close();
            return skuslist;


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

        public List<AllocationClub> GetSKU(int id)
        {
            List<AllocationClub> equipments = new List<AllocationClub>();
            SqlCommand cmd = new SqlCommand($"SELECT * from AllocationClubRequest where SKU = {id}", con);
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            con.Open();
            sd.Fill(dt);
            con.Close();
            foreach (DataRow dr in dt.Rows)
            {
                equipments.Add(
                    new AllocationClub
                    {
                        SKU = Convert.ToInt32(dr["SKU"]),




                    });
            }
            return equipments;
        }


        public bool VerifyInsert(int skuid, int clubcode)
        {
            MSsql = $"SELECT SKU,Clubcode from AllocationClubRequest where SKU = @skuid and ClubCode = @clubcode and Status = 'PENDING' ";
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


        public bool VerifyInsertAllocation(int skuid, int clubcode)
        {
            MSsql = $"SELECT SKU,Clubcode from CreateAllocation where SKU = @skuid and ClubCode = @clubcode and Prioritization = 5 and Status is null ";
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




        public void UpdateRequest(int SKU, int ClubCode, string Description, int Reason,  int Quantity,int PQty, string RequestedDate, string Remarks,string PConfig,string UserID,string DConfig)
        {
            DateTime formattedDateRequest = DateTime.Parse(RequestedDate);


            MSsql = "UPDATE [AllocationClubRequest] SET Description = @Description, Reason = @Reason, Quantity = @Quantity,PQty = @PQty, RequestedDate = @RequestedDate, Remarks = @Remarks,PConfig = @PConfig,UserID = @UserID,DistributionConfig = @DConfig where SKU = @SKU and ClubCode = @ClubCode and Status = 'PENDING' ";
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
                Value = Description,
                ParameterName = "@Description",
                DbType = DbType.String
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = Reason,
                ParameterName = "@Reason",
                DbType = DbType.Int32
            });
            
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = Quantity,
                ParameterName = "@Quantity",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = PQty,
                ParameterName = "@PQty",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = formattedDateRequest,
                ParameterName = "@RequestedDate",
                DbType = DbType.DateTime,
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = Remarks,
                ParameterName = "@Remarks",
                DbType = DbType.String
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = PConfig,
                ParameterName = "@PConfig",
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
                Value = DConfig,
                ParameterName = "@DConfig",
                DbType = DbType.String
            });
            



            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void UpdateApprovedRequest(int SKU, int ClubCode, string Status,  string ApprovedDate)
        {
            DateTime formattedDateApproved = DateTime.Parse(ApprovedDate);


            MSsql = "UPDATE [AllocationClubRequest] SET Status = @Status, ApprovedDate = @ApprovedDate where SKU = @SKU and ClubCode = @ClubCode";
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
                Value = Status,
                ParameterName = "@Status",
                DbType = DbType.String
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = formattedDateApproved,
                ParameterName = "@ApprovedDate",
                DbType = DbType.DateTime,
            });
            
           

            cmd.ExecuteNonQuery();
            con.Close();
        }



        public void UpdateApprovedAllocation(int SKU, int ClubCode,int Qty,string Reason, string Todays)
        {
            DateTime formattedday = DateTime.Parse(Todays);


            MSsql = "UPDATE [CreateAllocation] SET RequestedQty = @Qty, Remarks = @Reason,DateCreated = @Todays where SKU = @SKU and ClubCode = @ClubCode and Prioritization = 5 and BatchCode is null";
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
                Value = Qty,
                ParameterName = "@Qty",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = Reason,
                ParameterName = "@Reason",
                DbType = DbType.String
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = formattedday,
                ParameterName = "@Todays",
                DbType = DbType.DateTime,
            });



            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void UpdateDisApprovedRequest(int SKU, int ClubCode, string Status)
        {
           


            MSsql = "UPDATE [AllocationClubRequest] SET Status = @Status  where SKU = @SKU and ClubCode = @ClubCode";
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
                Value = Status,
                ParameterName = "@Status",
                DbType = DbType.String
            });
            

            cmd.ExecuteNonQuery();
            con.Close();
        }


        public bool InsertAllocation(int SKU, int ClubCode, int Quantity, int Prioritization,string DConfig,string Remarks,double DCMOH,string Todays)
        {

            try
            {
                DateTime formattedday = DateTime.Parse(Todays);

                SqlCommand cmd = new SqlCommand("INSERT CreateAllocation   (SKU,ClubCode,RequestedQty,Prioritization,DistributionConfig,Remarks,OnHand,DateCreated) VALUES(@SKU,@ClubCode,@Quantity, @Prioritization,@DConfig,@Remarks,@DCMOH,@Todays)", con);
                cmd.CommandType = CommandType.Text;
                con.Open();

                cmd.Parameters.AddWithValue("@SKU", SKU);
                cmd.Parameters.AddWithValue("@ClubCode", ClubCode);
                cmd.Parameters.AddWithValue("@Quantity", Quantity);
                
                
                cmd.Parameters.AddWithValue("@Prioritization", Prioritization);
                cmd.Parameters.AddWithValue("@DCMOH", DCMOH);
                cmd.Parameters.AddWithValue("@DConfig", DConfig);
                cmd.Parameters.AddWithValue("@Remarks", Remarks);
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

        public bool InsertRequest(int SKU, int ClubCode, string Description, int Reason, string Status, int Quantity,int PQty, string RequestedDate, string Remarks,string PConfig,string UserID, string DConfig)
        {

            try
            {
                DateTime formattedDateRequest = DateTime.Parse(RequestedDate);

                SqlCommand cmd = new SqlCommand("INSERT AllocationClubRequest(SKU,ClubCode,Description,Reason,Status,Quantity,PQty,RequestedDate,Remarks,PConfig,UserID,DistributionConfig) VALUES(@SKU,@ClubCode,@Description,@Reason,@Status,@Quantity,@PQty,@RequestedDate,@Remarks,@PConfig,@UserID,@DConfig)", con);
                cmd.CommandType = CommandType.Text;
                con.Open();

                cmd.Parameters.AddWithValue("@SKU", SKU);
                cmd.Parameters.AddWithValue("@ClubCode", ClubCode);
                cmd.Parameters.AddWithValue("@Description", Description);
                cmd.Parameters.AddWithValue("@Reason", Reason);
                cmd.Parameters.AddWithValue("@Status", Status);
                cmd.Parameters.AddWithValue("@Quantity", Quantity);
                cmd.Parameters.AddWithValue("@PQty", PQty);
                cmd.Parameters.AddWithValue("@RequestedDate", formattedDateRequest);
                cmd.Parameters.AddWithValue("@Remarks", Remarks);
                cmd.Parameters.AddWithValue("@PConfig", PConfig);
                cmd.Parameters.AddWithValue("@UserID", UserID);
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