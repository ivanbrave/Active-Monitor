using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSMMon.ConsoleTool
{
    using System.Data;
    using System.Data.SqlClient;

    using System.Configuration;

   
    /// <summary>
    /// Single-instance Pattern. It is used to access database.
    /// </summary>
    public class DBAccess
    {
        private SqlConnection sqlConn;

        /// <summary>
        /// Initialize a new CodeReviewDBAccess
        /// </summary>
        public DBAccess()
        {
            //AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            try
            {
                sqlConn = new SqlConnection(Config.CSMNoteDbConnectionString);
                sqlConn.Open();
            }
            catch (Exception ex)
            {
                throw new ApplicationException();
            }
        }


        public void Close()
        {
            if (sqlConn != null)
            {
                sqlConn.Close();
            }
        }

        /// <summary>
        /// Query for insert, detele and update
        /// </summary>
        /// <param name="nonQuery"></param>
        /// <returns>the affected rows. If these is error or exception, the affected rows will be zero</returns>
        public int SqlNonQuery(string nonQuery)
        {
            int affectRows = 0;
            try
            {
                SqlCommand cmd = sqlConn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = nonQuery;
                affectRows = cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                affectRows = 0;
                Console.WriteLine(ex.Message);
            }
            return affectRows;
        }

        /// <summary>
        /// Query for select.
        /// </summary>
        /// <param name="query"></param>
        /// <returns>the data set</returns>
        public SqlDataReader SqlQuery(string query)
        {
            SqlDataReader dataReader = null;
            bool success = false;

            try
            {
                SqlCommand cmd = sqlConn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = query;
                dataReader = cmd.ExecuteReader();
                success = true;
            }
            catch (SqlException ex)
            {
                success = false;
                Console.WriteLine(ex.Message);
            }

            if (success == false)
                dataReader = null;

            return dataReader;
        }

        /// <summary>
        /// Use to check the record
        /// </summary>
        /// <param name="query"></param>
        /// <returns>true if record existed</returns>
        public bool SqlCheckRecord(string query)
        {
            SqlCommand cmd = new SqlCommand(query, sqlConn);
            bool success = false;
            try
            {
                int res = (int)cmd.ExecuteScalar();

                if (res > 0)
                {
                    success = true;
                }
            }
            catch (Exception)
            {
                success = false;
            }
            return success;
        }

        /// <summary>
        /// Initialize the single instance
        /// </summary>
        static DBAccess()
        {
            dbAccess = new DBAccess();
        }

        /// <summary>
        /// Single instance for DBAccess
        /// </summary>
        private static DBAccess dbAccess;

        public static DBAccess getDBAccess() 
        {
            return dbAccess; 
        }
    }
}
