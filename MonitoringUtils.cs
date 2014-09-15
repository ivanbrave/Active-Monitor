using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace CSMMon.ConsoleTool
{
    public class MonitoringUtils
    {
        public static AvailabilityEntity CheckAMAvailability(string componentName, DateTime date)
        {
            return CheckAvailabilityInternal(
                Config.AMAvailabilityDbConnectionString,
                string.Format(Config.AMAvailabilityCheckSqlTemplate, string.Format("{0}_AM", componentName), string.Format("DATE = '{0}'", date.ToSqlDateString()))).FirstOrDefault();
        }

        public static AvailabilityEntity CheckPMAvailability(string componentName, DateTime date)
        {
            return CheckAvailabilityInternal(
                Config.PMAvailabilityDbConnectionString,
                string.Format(Config.PMAvailabilityCheckSqlTemplate, string.Format("{0}_PM", componentName), string.Format("DATE = '{0}'", date.ToSqlDateString()))).FirstOrDefault();
        }

        public static AvailabilityEntity[] CheckAMAvailabilityRange(string componentName, DateTime startDate, DateTime endDate)
        {
            return CheckAvailabilityInternal(
                "Data Source=.;Initial Catalog=CSM_PROD;Persist Security Info=True;User Id=sa;Password=DAD2install!",
                string.Format("SELECT * FROM {0} WHERE {1} order by DATE", string.Format("{0}_AM", componentName), string.Format("{0} >= '{1}' AND {0} <= '{2}'", "DATE", startDate.ToSqlDateString(), endDate.ToSqlDateString())));
        }

        public static AvailabilityEntity[] CheckPMAvailabilityRange(DateTime startDate, DateTime endDate)
        {
            return CheckAvailabilityInternal(
                Config.PMAvailabilityDbConnectionString,
                string.Format(Config.PMAvailabilityCheckSqlTemplate, string.Format("{0} >= '{1}' AND {0} <= '{2}'", Config.PMAvailabilitySqlDateField, startDate.ToSqlDateString(), endDate.ToSqlDateString())));
        }

        public static AvailabilityEntity[] CheckAvailabilityInternal(string connectionString, string sql)
        {
            using (SqlConnection conn = CommonUtils.CreateDbConnection(connectionString))
            using (SqlDataReader reader = conn.ExecuteReader(sql))
            {
                List<AvailabilityEntity> results = new List<AvailabilityEntity>();
                while (reader.Read())
                {
                    results.Add(AvailabilityEntity.FromSqlResult(reader));
                }

                return results.ToArray();
            }
        }

        
    }
}
