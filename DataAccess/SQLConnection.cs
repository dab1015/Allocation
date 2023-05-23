using SNRWMSPortal.Common;
using SNRWMSPortal.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace SNRWMSPortal.DataAccess
{
    public class SQLConnection
    {
        SqlConnection sqlConnect;

       
        SqlCommand sqlCommand;
        SqlDataReader sqlDataReader;

        public string sql = "";
        public string saltiness = ConfigurationManager.AppSettings["Asin"];
        public string nitertaions = ConfigurationManager.AppSettings["NIterations"];

        public SQLConnection()
        {
            string SqlConnectionWMS = ConfigurationManager.ConnectionStrings["SQLConnect"].ConnectionString;

            sqlConnect = new SqlConnection(SqlConnectionWMS);

        }

        public bool VerifyUser(string Username, string Password)
        {
            int numberofSalt = Convert.ToInt32(saltiness);
            int numberofiterations = Convert.ToInt32(nitertaions);

            sql = $"SELECT Salt,Hash from [People] where UserID =@Username COLLATE SQL_Latin1_General_CP1_CS_AS";
            sqlCommand = new SqlCommand(sql, sqlConnect);
            sqlCommand.Parameters.Add(new SqlParameter()
            {
                Value = Username,
                ParameterName = "@Username",
                DbType = DbType.String
            });


            sqlConnect.Open();

            sqlDataReader = sqlCommand.ExecuteReader();

            if (sqlDataReader.Read())
            {
                string salt = sqlDataReader.GetString(0);
                string passwordFromDB = sqlDataReader.GetString(1);
                string pwdHashed = SecurityHelper.HashPassword(Password, salt, numberofiterations, numberofSalt);
                sqlConnect.Close();
                return passwordFromDB == pwdHashed;
            }
            else
            {

                sqlConnect.Close();
                return false;
            }
        }

        public int VerifyUserIfReset(string Username)
        {

            sql = $"SELECT IsPasswordReset from [People] where UserID =@Username COLLATE SQL_Latin1_General_CP1_CS_AS";
            sqlCommand = new SqlCommand(sql, sqlConnect);
            sqlCommand.Parameters.Add(new SqlParameter()
            {
                Value = Username,
                ParameterName = "@Username",
                DbType = DbType.String
            });


            sqlConnect.Open();

            sqlDataReader = sqlCommand.ExecuteReader();

            if (sqlDataReader.Read())
            {

                bool IsPasswordReset = sqlDataReader.GetBoolean(0);

                if (IsPasswordReset == true)
                {
                    sqlConnect.Close();
                    return 1;
                }
                else
                {
                    sqlConnect.Close();
                    return 0;
                }


            }
            else
            {
                sqlConnect.Dispose();
                sqlConnect.Close();
                return 3;
            }
        }

        public bool VerifyUserRegister(string Username)
        {
            sql = $"SELECT UserID from [People] where UserID =@Username COLLATE SQL_Latin1_General_CP1_CS_AS";
            sqlCommand = new SqlCommand(sql, sqlConnect);
            sqlCommand.Parameters.Add(new SqlParameter()
            {
                Value = Username,
                ParameterName = "@Username",
                DbType = DbType.String
            });
            sqlCommand.CommandText = sql;
            sqlConnect.Open();

            sqlDataReader = sqlCommand.ExecuteReader();

            if (sqlDataReader.HasRows)
            {
                sqlDataReader.Close();
                sqlConnect.Close();

                return true;
            }
            else
            {
                sqlDataReader.Close();
                sqlConnect.Close();
                return false;
            }
        }


        public void RegisterUser(string Username, string Salt, string Hash)
        {
            //sqlConnect = new SqlConnection();
            sql = $"INSERT INTO [People](UserID,Salt,Hash,Profile) VALUES (@Username,@Salt,@Hash,1 )";
            sqlCommand = new SqlCommand(sql, sqlConnect);
            sqlCommand.Parameters.Add(new SqlParameter()
            {
                Value = Username,
                ParameterName = "@Username",
                DbType = DbType.String
            });
            sqlCommand.Parameters.Add(new SqlParameter()
            {
                Value = Salt,
                ParameterName = "@Salt",
                DbType = DbType.String
            });

            sqlCommand.Parameters.Add(new SqlParameter()
            {
                Value = Hash,
                ParameterName = "@Hash",
                DbType = DbType.String
            });
            sqlCommand.CommandText = sql;
            sqlConnect.Open();

            sqlCommand.ExecuteNonQuery();
            sqlConnect.Dispose();
            sqlConnect.Close();
        }


        public bool VerifyUserTempPassword(string Username, string temppassword)
        {
            int numberofSalt = Convert.ToInt32(saltiness);
            int numberofiterations = Convert.ToInt32(nitertaions);

            sql = $"SELECT UserID,Salt,Hash,FirstName,IsPasswordReset from [People] where UserID =@Username COLLATE SQL_Latin1_General_CP1_CS_AS";
            sqlCommand = new SqlCommand(sql, sqlConnect);
            sqlCommand.Parameters.Add(new SqlParameter()
            {
                Value = Username,
                ParameterName = "@Username",
                DbType = DbType.String
            });



            sqlConnect.Open();

            sqlDataReader = sqlCommand.ExecuteReader();

            if (sqlDataReader.Read())
            {
                string usernameFromDB = sqlDataReader.GetString(0);
                string salt = sqlDataReader.GetString(1);
                string passwordFromDB = sqlDataReader.GetString(2);
                string userFirstName = sqlDataReader.GetString(3);
                string pwdHashed = SecurityHelper.HashPassword(temppassword, salt, numberofiterations, numberofSalt);
                sqlConnect.Close();
                return passwordFromDB == pwdHashed;
            }
            else
            {
                sqlConnect.Dispose();
                sqlConnect.Close();
                return false;
            }
        }


        public void ResetUserPassword(string Username, string newsalt, string newpwdHashed)
        {
            sql = "UPDATE [People] SET Salt = @Salt, Hash =@Hash, IsPasswordReset=0 where UserID=@Username COLLATE SQL_Latin1_General_CP1_CS_AS";
            sqlConnect.Open();
            sqlCommand = new SqlCommand(sql, sqlConnect);
            sqlCommand.Parameters.Add(new SqlParameter()
            {
                Value = Username,
                ParameterName = "@Username",
                DbType = DbType.String
            });
            sqlCommand.Parameters.Add(new SqlParameter()
            {
                Value = newsalt,
                ParameterName = "@Salt",
                DbType = DbType.String
            });
            sqlCommand.Parameters.Add(new SqlParameter()
            {
                Value = newpwdHashed,
                ParameterName = "@Hash",
                DbType = DbType.String
            });
            sqlCommand.ExecuteNonQuery();
            sqlConnect.Close();


        }

        public List<UserInfosDetail> getUserInfos(string Username)
        {
            List<UserInfosDetail> profileLists = new List<UserInfosDetail>();
            List<string> userProfilesID = new List<string>();
            DataAccess.SQLQueryPeople getProfilesPeople = new DataAccess.SQLQueryPeople();
            var profileModels = getProfilesPeople.GetProfile();


            sql = $"SELECT * FROM People where UserID =@Username COLLATE SQL_Latin1_General_CP1_CS_AS";
            sqlCommand = new SqlCommand(sql, sqlConnect);
            sqlCommand.Parameters.Add(new SqlParameter()
            {
                Value = Username,
                ParameterName = "@Username",
                DbType = DbType.String
            });
            sqlCommand.CommandText = sql;

            sqlConnect.Open();

            sqlDataReader = sqlCommand.ExecuteReader();
            while (sqlDataReader.Read())
            {
                UserInfosDetail userInfos = new UserInfosDetail()
                {
                    Username = sqlDataReader["UserID"].ToString(),
                    userFirstName = sqlDataReader["FirstName"].ToString(),
                    userLastName = sqlDataReader["LastName"].ToString(),


                };
                string[] profileSplit = sqlDataReader["Profile"].ToString().Split(',');

                List<string> profileIDS = new List<string>();

                foreach (string profile in profileSplit)
                {
                    if (profile == "8")
                    {
                        userInfos.isProfileAdmin = "8";
                    }
                    profileIDS.Add(profileModels.Where(w => w.Code == profile).FirstOrDefault().Profile);
                }

                userInfos.userProfile = string.Join(",", profileIDS);
                profileLists.Add(userInfos);
            }
            sqlDataReader.Close();
            sqlConnect.Close();
            return profileLists;
        }



        //public List<Models.StagingModel> GetStaggingCodes()
        //{
        //    sql = "select distinct StagingCode from PickList where Status IN (4,5,99)";
        //    sqlCommand = new SqlCommand(sql, sqlConnect);
        //    sqlCommand.CommandText = sql;
        //    sqlConnect.Open();

        //    var listofStagingCodes = new List<Models.StagingModel>();

        //    sqlDataReader = sqlCommand.ExecuteReader();

        //    while (sqlDataReader.Read())
        //    {
        //        var stagingCode = new StagingModel();
        //        stagingCode.StagingCode = sqlDataReader["StagingCode"].ToString();
        //        //stagingCode.StagingCodeStatus = sqlDataReader["StagingCodeStatus"].ToString();
        //        listofStagingCodes.Add(stagingCode);

        //    }
        //    sqlConnect.Dispose();
        //    sqlConnect.Close();


        //    return listofStagingCodes;
        //}

        //public List<Models.PickList> ShowStaggingData(string StagingCode)
        //{

        //    sql = "SELECT PickListId,Username,SKU,ItemDesc,SlotLoc,STDPK,DistributionConfig,Remarks,QtyPcs,QtyPcsEdited,QtyToPick,QtyPicked,Status from PickList where StagingCode='" + StagingCode + "' and Status=4  order by Username,PickSeq";
        //    sqlCommand = new SqlCommand(sql, sqlConnect);
        //    sqlCommand.CommandText = sql;
        //    sqlConnect.Open();

        //    var listofPicklist = new List<Models.PickList>();

        //    sqlDataReader = sqlCommand.ExecuteReader();

        //    while (sqlDataReader.Read())
        //    {
        //        var pickList = new PickList();
        //        pickList.PickListId = (int)sqlDataReader["PickListId"];
        //        pickList.Username = sqlDataReader["Username"].ToString();
        //        pickList.SKU = sqlDataReader["SKU"].ToString();
        //        pickList.ItemDesc = sqlDataReader["ItemDesc"].ToString();
        //        pickList.SlotLoc = sqlDataReader["SlotLoc"].ToString();
        //        pickList.STDPK = sqlDataReader["STDPK"].ToString();
        //        pickList.DistributionConfig = sqlDataReader["DistributionConfig"].ToString();
        //        pickList.Remarks = sqlDataReader["Remarks"].ToString();
        //        pickList.QtyPcs = (decimal)sqlDataReader["QtyPcs"];
        //        pickList.QtyPcsEdited = (decimal)sqlDataReader["QtyPcsEdited"];
        //        pickList.QtyToPick = (int)sqlDataReader["QtyToPick"];
        //        pickList.QtyPicked = (int)sqlDataReader["QtyPicked"];
        //        pickList.Status = (int)sqlDataReader["Status"];
        //        //stagingCode.StagingCodeStatus = sqlDataReader["StagingCodeStatus"].ToString();
        //        listofPicklist.Add(pickList);

        //    }
        //    sqlConnect.Dispose();
        //    sqlConnect.Close();


        //    return listofPicklist;
        //}



    }
}