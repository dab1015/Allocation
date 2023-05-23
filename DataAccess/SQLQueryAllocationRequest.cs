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
    public class SQLQueryAllocationRequest

    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["SQLConnect"].ToString());

        OleDbConnection conmms = new OleDbConnection(ConfigurationManager.ConnectionStrings["MMSConnect"].ToString());


        public string MSsql;
        SqlCommand cmd;
        SqlDataReader reader;

        OleDbCommand olecmd;
        OleDbDataReader olereader;

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

                MSsql = $"Select SKU,Reason,PIC from  AllocationRequest WHERE SKU = {_skuId}";
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
                MSsql = $"Select Distinct B.SKU, B.DConfig,C.DConfig as DistributionConfig  from AllocationSKU B inner join DistributionConfig C on B.DConfig = C.Code WHERE B.SKU = {_skuId}";
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

        //public List<AllocationMerchandiseModel> GetAllocation()
        //{

        //    List<AllocationMerchandiseModel> alloclub = new List<AllocationMerchandiseModel>();

        //    MSsql = "select * from CreateAllocation order by PrioritizationNo asc";

        //    cmd = new SqlCommand(MSsql, con);
        //    con.Open();
        //    reader = cmd.ExecuteReader();
        //    while (reader.Read())
        //    {
        //        var clublist = new AllocationMerchandiseModel();

        //        clublist.ClubCode = Convert.ToInt32(reader["ClubCode"]);
        //        clublist.SKU = Convert.ToInt64(reader["SKU"]);
        //        clublist.RequestedQty = Convert.ToInt32(reader["RequestedQty"]);
        //        clublist.Prioritization = Convert.ToString(reader["Prioritization"]);
        //        clublist.Status = Convert.ToString(reader["Status"]);
        //        var MSsql2 = $"select INUMBR,IDESCR,ISTDPK from mmjdalib.invmst  WHERE INUMBR = {Convert.ToInt64(reader["SKU"])}";
        //        olecmd = new OleDbCommand(MSsql2, conmms);
        //        conmms.Open();
        //        olereader = olecmd.ExecuteReader();
        //        while (olereader.Read())
        //        {
        //            clublist.INUMBR = Convert.ToInt64(olereader["INUMBR"]);
        //            clublist.IDESCR = Convert.ToString(olereader["IDESCR"]);
        //            clublist.ISTDPK = Convert.ToDouble(olereader["ISTDPK"]);


        //        }
        //        conmms.Close();

        //        alloclub.Add(clublist);
        //    }

        //    con.Close();
        //    return alloclub;


        //}

        

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
           
            MSsql = $"select C.clubcode,concat(C.ClubCode,'-',C.ClubName) as ClubNames,(case when D.sku is null then {_skuId} else D.sku end) as sku, (case when D.Pieces is null then 0 else D.Pieces end) as Pieces, (case when D.DistributionConfig is null then '' else D.DistributionConfig end) as DistributionConfig,(case when D.Reason is null then 0 else D.Reason end) as Reason from AllocationClub C left outer join(select A.sku, A.ClubCode,A.Pieces,A.DistributionConfig,A.Reason from AllocationRequest A left join (select ClubCode from AllocationClub ) B on A.ClubCode = b.ClubCode  where A.sku = {_skuId})  D on C.ClubCode = D.ClubCode where C.Status = 1 group by C.ClubCode,D.SKU,D.Pieces,D.DistributionConfig,D.Reason,C.ClubName order by C.ClubCode asc";
             
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

            MSsql = "UPDATE [AllocationRequest] SET Pieces = @Pieces, Reason = @Reason, PIC = @PIC, UserID = @UserID, DateModified = @Todays,DistributionConfig = @DConfig,Description = @Description where SKU = @SKU and ClubCode = @ClubCode";
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

        public void UpdateMerchanAllocation(int SKU, int ClubCode, int Pieces,string ReasonName,  int Prioritization,double DCMOH, string Todays)
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

        public bool InsertMerchan(int SKU,int ClubCode, int Pieces, int Reason, string PIC,string UserID,string Todays,string DConfig,string Description)
        {

            try
            {
                DateTime formattedday = DateTime.Parse(Todays);

                SqlCommand cmd = new SqlCommand("INSERT AllocationRequest(SKU,ClubCode,Pieces,Reason,PIC,UserID,DateModified,DistributionConfig,Description) VALUES(@SKU,@ClubCode,@Pieces,@Reason,@PIC,@UserID,@Todays,@DConfig,@Description)", con);
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

        public bool InsertMerchanAllocation(int SKU, int ClubCode, int Pieces,string ReasonName, int Prioritization,double DCMOH, string Todays,string DConfig)
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
            MSsql = $"SELECT SKU,Clubcode from AllocationRequest where SKU = @skucode and ClubCode = @clubcode";
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


        public int CurrentQtyAllocation(int skucode, int clubcode,int prio)
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