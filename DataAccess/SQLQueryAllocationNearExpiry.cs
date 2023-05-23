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
    public class SQLQueryAllocationNearExpiry
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


            }
            
            return skuslist;
        }

       

        public List<AllocationMerchandiseModel> GetAllocationClubCode(int clubcode)
        {

            List<AllocationMerchandiseModel> alloclub = new List<AllocationMerchandiseModel>();

            MSsql = $"select A.ClubCode,A.SKU,A.RequestedQty,B.Description as Prio,C.Description as Stat from CreateAllocation A inner join Prioritization B on A.Prioritization = B.Code inner join PickListStatus C on A.Status = C.Code where A.ClubCode = {clubcode} order by Prioritization asc";

            cmd = new SqlCommand(MSsql, con);
            con.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var clublist = new AllocationMerchandiseModel();

                clublist.ClubCode = Convert.ToInt32(reader["ClubCode"]);
                clublist.SKU = Convert.ToInt64(reader["SKU"]);
                clublist.RequestedQty = Convert.ToInt32(reader["RequestedQty"]);
                clublist.PrioritizationName = Convert.ToString(reader["Prio"]);
                clublist.StatusName = Convert.ToString(reader["Stat"]);
                var MSsql2 = $"select INUMBR,IDESCR,ISTDPK from mmjdalib.invmst  WHERE INUMBR = {Convert.ToInt64(reader["SKU"])}";
                olecmd = new OleDbCommand(MSsql2, conmms);
                conmms.Open();
                olereader = olecmd.ExecuteReader();
                while (olereader.Read())
                {
                    clublist.INUMBR = Convert.ToInt64(olereader["INUMBR"]);
                    clublist.IDESCR = Convert.ToString(olereader["IDESCR"]);
                    clublist.ISTDPK = Convert.ToDouble(olereader["ISTDPK"]);


                }
                conmms.Close();

                alloclub.Add(clublist);
            }

            con.Close();
            return alloclub;


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



        public List<AllocationNearExpiryModel> GetClubSKU(long _skuId)
        {

            List<AllocationNearExpiryModel> alloclub = new List<AllocationNearExpiryModel>();
           
            MSsql = $"select C.clubcode,concat(C.ClubCode,'-',C.ClubName) as ClubNames,(case when D.sku is null then {_skuId} else D.sku end) as sku, D.OneToThirty,D.ThirtyOneToSixty,D.SixtyOneAbove from AllocationClub C left outer join(select A.sku, A.ClubCode,A.OneToThirty,A.ThirtyOneToSixty,A.SixtyOneAbove from AllocationNearExpiry A left join (select ClubCode from AllocationClub ) B on A.ClubCode = b.ClubCode  where A.sku = {_skuId})  D on C.ClubCode = D.ClubCode where C.Status = 1 group by C.ClubCode,D.SKU,D.OneToThirty,D.ThirtyOneToSixty,D.SixtyOneAbove,C.ClubName order by C.ClubCode asc";
             
            cmd = new SqlCommand(MSsql, con);
            con.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var clublist = new AllocationNearExpiryModel();
              
                clublist.ClubName = Convert.ToString(reader["ClubNames"]);
                clublist.ClubCode = Convert.ToInt32(reader["ClubCode"]);
                clublist.SKU = Convert.ToInt64(reader["SKU"]);
                clublist.ClubCode = Convert.ToInt32(reader["ClubCode"]);
                clublist.OneToThirty = Convert.ToInt32(reader["OneToThirty"]);
                clublist.ThirtyoneToSixty = Convert.ToInt32(reader["ThirtyOneToSixty"]);
                clublist.SixtyoneAbove = Convert.ToInt32(reader["SixtyOneAbove"]);




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

        public void UpdateNearExpiry(int ClubCode, int One,int Two, int Three,int SKU)
        {
           // DateTime formattedday = DateTime.Parse(Todays);

            MSsql = "UPDATE [AllocationNearExpiry] SET OneToThirty = @One, ThirtyOneToSixty = @Two, SixtyOneAbove = @Three where SKU = @SKU and ClubCode = @ClubCode";
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
                Value = One,
                ParameterName = "@One",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = Two,
                ParameterName = "@Two",
                DbType = DbType.Int32
            });
            cmd.Parameters.Add(new SqlParameter()
            {
                Value = Three,
                ParameterName = "@Three",
                DbType = DbType.Int32
            });
           
            cmd.ExecuteNonQuery();
            con.Close();
        }

        

        

        public bool InsertNearExpiry(int ClubCode, int One, int Two, int Three, int SKU)
        {

            try
            {
             //   DateTime formattedday = DateTime.Parse(Todays);

                SqlCommand cmd = new SqlCommand("INSERT AllocationNearExpiry(ClubCode,OneToThirty,ThirtyOneToSixty,SixtyOneAbove,SKU) VALUES(@ClubCode,@One,@Two,@Three,@SKU,)", con);
                cmd.CommandType = CommandType.Text;
                con.Open();

                cmd.Parameters.AddWithValue("@SKU", SKU);
                cmd.Parameters.AddWithValue("@One", One);
                cmd.Parameters.AddWithValue("@Two", Two);
                cmd.Parameters.AddWithValue("@Three", Three);
                cmd.Parameters.AddWithValue("@ClubCode", ClubCode);
                
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

        public bool InsertMerchanAllocation(int SKU, int ClubCode, int Pieces,   int Status, int Prioritization, string Todays, int DConfig)
        {

            try
            {
                DateTime formattedday = DateTime.Parse(Todays);

                SqlCommand cmd = new SqlCommand("INSERT CreateAllocation(SKU,ClubCode,RequestedQty,Status,Prioritization,DateCreated,DistributionConfig) VALUES(@SKU,@ClubCode,@Pieces,@Status,@Prioritization,@Todays,@DConfig)", con);
                cmd.CommandType = CommandType.Text;
                con.Open();

                cmd.Parameters.AddWithValue("@SKU", SKU);
                cmd.Parameters.AddWithValue("@ClubCode", ClubCode);
                cmd.Parameters.AddWithValue("@Pieces", Pieces);
               
                cmd.Parameters.AddWithValue("@Status", Status);
                cmd.Parameters.AddWithValue("@Prioritization", Prioritization);
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

        public bool VerifyNearExpiry(int skucode, int clubcode)
        {
            MSsql = $"SELECT SKU,Clubcode from AllocationNearExpiry where SKU = @skucode and ClubCode = @clubcode";
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

    }
}