using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace RuppinResearchBudget.DAL
{
    public class DBServices
    {
        protected SqlConnection connect(string conStringName)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            string cStr = configuration.GetConnectionString(conStringName);
            SqlConnection con = new SqlConnection(cStr);
            con.Open();
            return con;
        }

        protected SqlCommand CreateCommandWithStoredProcedureGeneral(
            string spName,
            SqlConnection con,
            Dictionary<string, object> paramDic)
        {
            SqlCommand cmd = new SqlCommand
            {
                Connection = con,
                CommandText = spName,
                CommandTimeout = 10,
                CommandType = CommandType.StoredProcedure
            };

            if (paramDic != null)
            {
                foreach (KeyValuePair<string, object> param in paramDic)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value);
                }
            }

            return cmd;
        }
    }
}
