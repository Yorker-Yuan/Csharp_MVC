using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace smallWorld.Models
{

    public class db
    {
        SqlConnection con = new SqlConnection("Data Source=LAPTOP-49GQ7CD1\\SQLEXPRESS;Initial Catalog=dbCustomer;Integrated Security=True");
        public int LoginCheck(CLogin c)
        {
            SqlCommand cmd = new SqlCommand("SP_Login",con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@fAccount",c.fAccount);
            cmd.Parameters.AddWithValue("@fPassword", c.fPassword);
            SqlParameter objlogin = new SqlParameter();
            objlogin.ParameterName = "@Isvalid";
            objlogin.SqlDbType = SqlDbType.Bit;
            objlogin.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(objlogin);
            con.Open();
            cmd.ExecuteNonQuery();
            int res = Convert.ToInt32(objlogin.Value);
            con.Close();
            return res;
        }
    }
}
