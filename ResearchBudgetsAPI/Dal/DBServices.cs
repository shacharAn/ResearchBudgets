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
    }
}
