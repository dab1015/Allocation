using SNRWMSPortal.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SNRWMSPortal.DataAccess
{
    public class SQLQueryEquipment
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["SQLConnect"].ToString());

        public int InsertEquipment(RegisterEquipment equipmentModel)
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO Equipment(EquipmentDesc,EquipmentType,PrimaryFunc,SecondaryFunc,TertiaryFunc,Status) VALUES(@EquipmentDesc,@EquipmentType,@PrimaryFunc,@SecondaryFunc,@TertiaryFunc,@Status)", con);
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.AddWithValue("@EquipmentDesc", equipmentModel.EquipmentDesc);
            cmd.Parameters.AddWithValue("@EquipmentType", equipmentModel.EquipmentType);
            cmd.Parameters.AddWithValue("@PrimaryFunc", equipmentModel.PrimaryFunc);
            cmd.Parameters.AddWithValue("@SecondaryFunc", equipmentModel.SecondaryFunc);
            cmd.Parameters.AddWithValue("@TertiaryFunc", equipmentModel.TertiaryFunc);
            cmd.Parameters.AddWithValue("@Status", equipmentModel.Status);
            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }

        public int UpdateEquipment(RegisterEquipment equipmentModel)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("UPDATE Equipment SET EquipmentDesc = @EquipmentDesc, EquipmentType = @EquipmentType, PrimaryFunc = @PrimaryFunc, SecondaryFunc = @SecondaryFunc, TertiaryFunc = @TertiaryFunc, Status = @Status WHERE EquipmentId = @EquipmentId", con);

            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@EquipmentId", equipmentModel.EquipmentId);
            cmd.Parameters.AddWithValue("@EquipmentDesc", equipmentModel.EquipmentDesc);
            cmd.Parameters.AddWithValue("@EquipmentType", equipmentModel.EquipmentType);
            cmd.Parameters.AddWithValue("@PrimaryFunc", equipmentModel.PrimaryFunc);
            cmd.Parameters.AddWithValue("@SecondaryFunc", equipmentModel.SecondaryFunc);
            cmd.Parameters.AddWithValue("@TertiaryFunc", equipmentModel.TertiaryFunc);
            cmd.Parameters.AddWithValue("@Status", equipmentModel.Status);

            try
            {
                cmd.ExecuteNonQuery();

            }
            catch (Exception)
            {

                return 2;
            }
            con.Close();
            return 1;
        }



        public int DetailsEquipment(int equipmentModel)
        {
            SqlCommand cmd = new SqlCommand("Select FROM Equipment WHERE EquipmentId = @EquipmentId", con);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@EquipmentId", equipmentModel);
            cmd.Parameters.AddWithValue("@EquipmentDesc", equipmentModel);
            cmd.Parameters.AddWithValue("@EquipmentType", equipmentModel);
            cmd.Parameters.AddWithValue("@PrimaryFunc", equipmentModel);
            cmd.Parameters.AddWithValue("@TertiaryFunc", equipmentModel);
            cmd.Parameters.AddWithValue("@SecondaryFunc", equipmentModel);
            cmd.Parameters.AddWithValue("@Status", equipmentModel);
            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }

        public List<RegisterEquipment> GetEquipment()
        {
            List<RegisterEquipment> equipments = new List<RegisterEquipment>();
            SqlCommand cmd = new SqlCommand("SELECT A.[EquipmentID],A.EquipmentDesc,B.Description as EquipmentType,A.PrimaryFunc,A.SecondaryFunc,A.TertiaryFunc,A.Status from Equipment A JOIN EquipmentType B ON A.EquipmentType=B.Code", con);
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            con.Open();
            sd.Fill(dt);
            con.Close();
            foreach (DataRow dr in dt.Rows)
            {
                equipments.Add(
                    new RegisterEquipment
                    {
                        EquipmentId = Convert.ToInt32(dr["EquipmentId"]),
                        EquipmentDesc = Convert.ToString(dr["EquipmentDesc"]),
                        EquipmentType = Convert.ToString(dr["EquipmentType"]),
                        PrimaryFunc = Convert.ToInt32(dr["PrimaryFunc"]),
                        SecondaryFunc = Convert.ToInt32(dr["SecondaryFunc"]),
                        TertiaryFunc = Convert.ToInt32(dr["TertiaryFunc"]),
                        Status = Convert.ToString(dr["Status"])

                    });
            }
            return equipments;
        }

        public List<FunctionModel> GetFunction()
        {
            List<FunctionModel> functions = new List<FunctionModel>();
            SqlCommand cmd = new SqlCommand("SELECT *  From Function", con);
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            con.Open();
            sd.Fill(dt);
            con.Close();
            foreach (DataRow dr in dt.Rows)
            {
                functions.Add(
                    new FunctionModel
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        Code = Convert.ToString(dr["Code"]),
                        FunctionName = Convert.ToString(dr["FunctionName"]),
                        Description = Convert.ToString(dr["Description"])

                    });
            }
            return functions;
        }


        public List<EquipmentTypeModel> GetEquipmentType()
        {
            List<EquipmentTypeModel> equipmenttypes = new List<EquipmentTypeModel>();
            SqlCommand cmd = new SqlCommand("SELECT *  From EquipmentType", con);
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            con.Open();
            sd.Fill(dt);
            con.Close();
            foreach (DataRow dr in dt.Rows)
            {
                equipmenttypes.Add(
                    new EquipmentTypeModel
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        Code = Convert.ToString(dr["Code"]),
                        Description = Convert.ToString(dr["Description"]),
                        Level = Convert.ToString(dr["Level"])

                    });
            }
            return equipmenttypes;
        }
    }
}