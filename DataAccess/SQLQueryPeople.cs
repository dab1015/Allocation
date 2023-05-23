using Microsoft.SqlServer.Server;
using SNRWMSPortal.Common;
using SNRWMSPortal.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Profile;
using static SNRWMSPortal.Controllers.AccountController;

namespace SNRWMSPortal.DataAccess
{
    public class SQLQueryPeople
    {
        public string saltiness = ConfigurationManager.AppSettings["Asin"];
        public string nitertaions = ConfigurationManager.AppSettings["NIterations"];
        SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SQLConnect"].ToString());
        SqlCommand sqlCommand = new SqlCommand();
        SqlDataReader sqlDataReader;

        public void UpdatePeople(RegisterPeople registerpeopleModel)
        {
            string profiles = "";
            int isResetPass = 0;
            profiles = string.Join<string>(",", registerpeopleModel.Profile);

            if (registerpeopleModel.Hash == null)
            {
                sqlCommand = new SqlCommand("UPDATE People SET UserID = @UserID, EmployeeNo = @EmployeeNo, FirstName = @FirstName, LastName = @LastName, Profile = @Profile, Designation = @Designation, EmploymentStatus = @Status, WithActiveDirectoryAccount = @isActiveDirectory WHERE Id = @Id", sqlConnection);
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Parameters.AddWithValue("@Id", registerpeopleModel.Id);
                sqlCommand.Parameters.AddWithValue("@UserID", registerpeopleModel.UserID);
                sqlCommand.Parameters.AddWithValue("@EmployeeNo", registerpeopleModel.EmployeeNo);
                sqlCommand.Parameters.AddWithValue("@FirstName", registerpeopleModel.FirstName);
                sqlCommand.Parameters.AddWithValue("@LastName", registerpeopleModel.LastName);
                sqlCommand.Parameters.AddWithValue("@Designation", registerpeopleModel.Designation);
                sqlCommand.Parameters.AddWithValue("@Profile", profiles);
                sqlCommand.Parameters.AddWithValue("@Status", registerpeopleModel.Status);
                sqlCommand.Parameters.AddWithValue("@isActiveDirectory", registerpeopleModel.isActiveDirectory);
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
                sqlConnection.Close();
            }
            else
            {
                isResetPass = 1;
                sqlCommand = new SqlCommand("UPDATE People SET UserID = @UserID, Hash = @Hash, Salt = @Salt, EmployeeNo = @EmployeeNo, FirstName = @FirstName, LastName = @LastName, Profile = @Profile, Designation = @Designation, EmploymentStatus = @Status, IsPasswordReset = @Reset,WithActiveDirectoryAccount = @isActiveDirectory WHERE Id = @Id", sqlConnection);
                string salt = SecurityHelper.GenerateSalt(70);
                string pwdHashed = SecurityHelper.HashPassword(registerpeopleModel.Hash, salt, 10101, 70);
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Parameters.AddWithValue("@Id", registerpeopleModel.Id);
                sqlCommand.Parameters.AddWithValue("@UserID", registerpeopleModel.UserID);
                sqlCommand.Parameters.AddWithValue("@Hash", pwdHashed);
                sqlCommand.Parameters.AddWithValue("@Salt", salt);
                sqlCommand.Parameters.AddWithValue("@EmployeeNo", registerpeopleModel.EmployeeNo);
                sqlCommand.Parameters.AddWithValue("@FirstName", registerpeopleModel.FirstName);
                sqlCommand.Parameters.AddWithValue("@LastName", registerpeopleModel.LastName);
                sqlCommand.Parameters.AddWithValue("@Designation", registerpeopleModel.Designation);
                sqlCommand.Parameters.AddWithValue("@Profile", profiles);
                sqlCommand.Parameters.AddWithValue("@Status", registerpeopleModel.Status);
                sqlCommand.Parameters.AddWithValue("@Reset", isResetPass);
                sqlCommand.Parameters.AddWithValue("@isActiveDirectory", registerpeopleModel.isActiveDirectory);
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
                sqlConnection.Close();

            }
        }



        public int DetailsPeople(int registerpeopleModel)
        {
            SqlCommand cmd = new SqlCommand("Select FROM People WHERE Id = @Id", sqlConnection);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@Id", registerpeopleModel);
            cmd.Parameters.AddWithValue("@UserID", registerpeopleModel);
            cmd.Parameters.AddWithValue("@EmployeeNo", registerpeopleModel);
            cmd.Parameters.AddWithValue("@FirstName", registerpeopleModel);
            cmd.Parameters.AddWithValue("@LastName", registerpeopleModel);
            cmd.Parameters.AddWithValue("@Designation", registerpeopleModel);
            cmd.Parameters.AddWithValue("@Profile", registerpeopleModel);
            cmd.Parameters.AddWithValue("@Status", registerpeopleModel);
            sqlConnection.Open();
            int i = cmd.ExecuteNonQuery();
            sqlConnection.Close();
            return i;
        }

        public List<DisplayRegisteredPeople> GetPeople()
        {
            string sql = "";
            List<RegisterPeople> peoples = new List<RegisterPeople>();
            List<DisplayRegisteredPeople> profileLists = new List<DisplayRegisteredPeople>();
            var profileModels = GetProfile();

            sql = $"SELECT * FROM People ORDER BY id desc";

            sqlCommand = new SqlCommand(sql, sqlConnection);


            sqlCommand.CommandText = sql;
            sqlConnection.Open();
            sqlDataReader = sqlCommand.ExecuteReader();
            while (sqlDataReader.Read())
            {
                DisplayRegisteredPeople profileModelList = new DisplayRegisteredPeople()
                {
                    Id = Convert.ToInt32(sqlDataReader["id"]),
                    UserID = sqlDataReader["UserID"].ToString(),
                    FirstName = sqlDataReader["FirstName"].ToString(),
                    LastName = sqlDataReader["LastName"].ToString(),
                    EmployeeNo = sqlDataReader["EmployeeNo"].ToString(),
                    Designation = sqlDataReader["Designation"].ToString(),
                    Status = Convert.ToString(sqlDataReader["EmploymentStatus"]),
                    isActiveDirectory = Convert.ToBoolean(sqlDataReader["WithActiveDirectoryAccount"])


                };

                string[] profileSplit = sqlDataReader["Profile"].ToString().Split(',');
                List<string> profileIDS = new List<string>();
                foreach (string profile in profileSplit)
                {
                    profileIDS.Add(profileModels.Where(w => w.Code == profile).FirstOrDefault().Profile);
                }

                profileModelList.Profile = string.Join(",", profileIDS);

                profileLists.Add(profileModelList);

            }

            sqlDataReader.Close();
            sqlConnection.Close();


            //for (int i = 0; i < idList.Count; i++)
            //{

            //    sql = $"SELECT B.Profile as ProfileRole FROM People A,[Profile] B where A.id =@{}  and B.Code=@profileSplit";
            //    //sql = $"SELECT A.[id],A.UserID,A.EmployeeNo,A.FirstName,A.LastName,A.Designation,B.Profile as Profile,A.Status from People A JOIN Profile B ON A.Profile ={profileCode}";

            //}



            return profileLists;


            //sql = $"SELECT A.[id],A.UserID,A.EmployeeNo,A.FirstName,A.LastName,A.Designation,B.Profile as Profile,A.Status from People A JOIN Profile B ON A.Profile ={profileCode}";
        }

        public List<EmployeeDetails> EmployeeDetails(string empnum)
        {
            List<EmployeeDetails> employeeDetailsList = new List<EmployeeDetails>();

            SqlCommand cmd = new SqlCommand($"SELECT *  From Jeonsoft_Employee_Details where EmployeeCode='{empnum}'", sqlConnection);
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sqlConnection.Open();
            sd.Fill(dt);
            sqlConnection.Close();
            foreach (DataRow dr in dt.Rows)
            {
                employeeDetailsList.Add(
                    new EmployeeDetails
                    {
                        EmployeeCode = Convert.ToInt32(dr["EmployeeCode"]),
                        FirstName = Convert.ToString(dr["FirstName"]),
                        LastName = Convert.ToString(dr["LastName"]),
                        MiddleName = Convert.ToString(dr["MiddleName"]),
                        Position = Convert.ToString(dr["Position"])

                    });
            }
            return employeeDetailsList;
        }

        public List<ProfileModel> GetProfile()
        {
            List<ProfileModel> profiles = new List<ProfileModel>();
            SqlCommand cmd = new SqlCommand("SELECT *  From Profile", sqlConnection);
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sqlConnection.Open();
            sd.Fill(dt);
            sqlConnection.Close();
            foreach (DataRow dr in dt.Rows)
            {
                profiles.Add(
                    new ProfileModel
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        Code = Convert.ToString(dr["Code"]),
                        Profile = Convert.ToString(dr["Profile"])

                    });
            }
            return profiles;
        }




        public string sql = "";

        public bool VerifyUserRegister(string Username)
        {
            sql = $"SELECT UserID from People where UserID =@UserID";
            sqlCommand = new SqlCommand(sql, sqlConnection);
            sqlCommand.Parameters.Add(new SqlParameter()
            {
                Value = Username,
                ParameterName = "@UserID",
                DbType = DbType.String
            });
            sqlCommand.CommandText = sql;
            sqlConnection.Open();

            sqlDataReader = sqlCommand.ExecuteReader();

            if (sqlDataReader.HasRows)
            {
                sqlDataReader.Close();
                sqlConnection.Close();

                return true;
            }
            else
            {
                sqlDataReader.Close();
                sqlConnection.Close();
                return false;
            }
        }


        public bool VerifyUserEmployeeNo(string EmployeeNo)
        {
            sql = $"SELECT EmployeeNo from People where EmployeeNo =@EmployeeNo";
            sqlCommand = new SqlCommand(sql, sqlConnection);
            sqlCommand.Parameters.Add(new SqlParameter()
            {
                Value = EmployeeNo,
                ParameterName = "@EmployeeNo",
                DbType = DbType.String
            });
            sqlCommand.CommandText = sql;
            sqlConnection.Open();

            sqlDataReader = sqlCommand.ExecuteReader();

            if (sqlDataReader.HasRows)
            {
                sqlDataReader.Close();
                sqlConnection.Close();

                return true;
            }
            else
            {
                sqlDataReader.Close();
                sqlConnection.Close();
                return false;
            }
        }




        public void RegisterUserFull(string Username, string Hash, string Salt, string EmployeeNo, string FirstName, string LastName, string Designation, string Profile, int Status, bool isActiveDirectory)
        {
            int isReset = 1;


            //sqlConnect = new SqlConnection();
            sql = $"INSERT INTO People(UserID,Hash,Salt,EmployeeNo,FirstName,LastName,Designation,Profile,EmploymentStatus,IsPasswordReset,WithActiveDirectoryAccount) VALUES (@UserID,@Hash,@Salt,@EmployeeNo,@FirstName,@LastName,@Designation,@Profile,@Status,@Reset,@isActiveDirectory)";
            sqlCommand = new SqlCommand(sql, sqlConnection);
            sqlCommand.Parameters.Add(new SqlParameter()
            {
                Value = Username,
                ParameterName = "@UserID",
                DbType = DbType.String
            });

            sqlCommand.Parameters.Add(new SqlParameter()
            {
                Value = Hash,
                ParameterName = "@Hash",
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
                Value = EmployeeNo,
                ParameterName = "@EmployeeNo",
                DbType = DbType.String
            });

            sqlCommand.Parameters.Add(new SqlParameter()
            {
                Value = FirstName,
                ParameterName = "@FirstName",
                DbType = DbType.String
            });

            sqlCommand.Parameters.Add(new SqlParameter()
            {
                Value = LastName,
                ParameterName = "@LastName",
                DbType = DbType.String
            });



            sqlCommand.Parameters.Add(new SqlParameter()
            {
                Value = Designation,
                ParameterName = "@Designation",
                DbType = DbType.String
            });

            sqlCommand.Parameters.Add(new SqlParameter()
            {
                Value = Profile,
                ParameterName = "@Profile",
                DbType = DbType.String
            });

            sqlCommand.Parameters.Add(new SqlParameter()
            {
                Value = Status,
                ParameterName = "@Status",
                DbType = DbType.Int32
            });

            sqlCommand.Parameters.Add(new SqlParameter()
            {
                Value = isReset,
                ParameterName = "@Reset",
                DbType = DbType.Int32
            });

            sqlCommand.Parameters.Add(new SqlParameter()
            {
                Value = isActiveDirectory,
                ParameterName = "@isActiveDirectory",
                DbType = DbType.Boolean
            });

            sqlCommand.CommandText = sql;
            sqlConnection.Open();

            sqlCommand.ExecuteNonQuery();
            // sqlConnect.Dispose();
            sqlConnection.Close();
        }
    }
}