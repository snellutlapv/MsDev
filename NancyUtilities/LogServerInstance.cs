using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace NancyUtilities
{
    public class LogServerInstance
    {
        public static async Task UpdateDbWithServerInfo(string connString, string spName, string hostUri, string port, string serviceName)
        {
            using (var conn = new SqlConnection(connString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    using (var cmd = new SqlCommand(spName, conn, tran))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@ServiceName", serviceName));
                        cmd.Parameters.Add(new SqlParameter("@ServiceUri", hostUri));
                        cmd.Parameters.Add(new SqlParameter("@Port", port));
                        try
                        {
                            await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                        }
                        catch (Exception)
                        {
                            tran.Rollback();
                        }
                        tran.Commit();
                    }
                }
            }
        }

        public static async Task UpdateDbWithServerLog(string connString, string serviceUrl, string serviceName,
            string methodName, string actionName, string referrer, string query, string responseTime)
        {
            var sqlSpName = "shms.dbo.[CreateServiceLog]";

            using (var conn = new SqlConnection(connString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    using (var cmd = new SqlCommand(sqlSpName, conn, tran))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@ServiceUrl", serviceUrl));
                        cmd.Parameters.Add(new SqlParameter("@ResponseTime", responseTime));
                        cmd.Parameters.Add(new SqlParameter("@ServiceName", serviceName));
                        cmd.Parameters.Add(new SqlParameter("@MethodName", methodName));
                        cmd.Parameters.Add(new SqlParameter("@Action", actionName));
                        cmd.Parameters.Add(new SqlParameter("@Referrer", referrer));
                        cmd.Parameters.Add(new SqlParameter("@Query", query));
                        try
                        {
                            await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                        }
                        catch (Exception)
                        {
                            tran.Rollback();
                        }
                        tran.Commit();
                    }
                }
            }
        }
    }
}
