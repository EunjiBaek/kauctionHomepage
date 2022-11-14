using Dapper;
using KA.Entities.Models.Live;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;

namespace KA.Repositories
{
    public interface ILiveRepository 
    {
        DataSet GetDataSet(string _spName, object _param);

        DataSet GetLiveData(int auc_kind, int auc_num);

        void InertUserLogData(LiveUser liveUser);
    }


    public class LiveRepository : BaseRepository, ILiveRepository
    {   
        public LiveRepository(IConfiguration configuration) {
            _kauctionConnectionString = GetConnectionString(configuration, "kauctionConnectionString");
            _logConnectionString = GetConnectionString(configuration, "logConnectionString");
        }

        public DataSet GetDataSet(string _spName, object _param)
        {
            DataSet _ds = new DataSet();
            using (SqlConnection cn = new SqlConnection(GetSqlConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = cn;

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = _spName;

                    // 파라매터 세팅
                    cmd.Parameters.Clear();

                    foreach (var _pair in _param.GetType().GetProperties())
                    {
                        cmd.Parameters.AddWithValue(_pair.Name, _param.GetType().GetProperty(_pair.Name).GetValue(_param, null));
                    }

                    cmd.Connection.Open();
                    SqlDataAdapter ada = new SqlDataAdapter(cmd);
                    ada.Fill(_ds);

                    cmd.Connection.Close();
                }

                cn.Close();
            }

            return _ds;
        }


        /// <summary>
        /// 현재 진행중인 경매 및 랏 조회
        /// </summary>
        /// <param name="auc_kind"></param>
        /// <param name="auc_num"></param>
        /// <returns></returns>
        public DataSet GetLiveData(int auc_kind, int auc_num) 
        {
            
            var _param = new
            {
                auc_kind = auc_kind
                ,
                auc_num = auc_num                
            };

            return this.GetDataSet("usp_Live_Auc_LotStat_Info_SelectForWAITORING", _param);
        }

        /// <summary>
        /// 사용자로그 등록
        /// </summary>
        /// <param name="_param"></param>
 
        public void InertUserLogData(LiveUser liveUser) 
        {
            this.GetDataSet("usp_Live_Auc_UserLog_Hst_Insert", liveUser);
        }


        private string GetSqlConnectionString() {

            var sqlConnectionSB = new SqlConnectionStringBuilder();

            sqlConnectionSB.ConnectionString = base._kauctionConnectionString;
            sqlConnectionSB.Pooling = true;
            //sqlConnectionSB.MaxPoolSize = 100;
            sqlConnectionSB.ConnectTimeout = 10;

            // Change these values to your values.  
            //sqlConnectionSB.DataSource = "tcp:myazuresqldbserver.database.windows.net,1433"; //["Server"]  
            //sqlConnectionSB.InitialCatalog = "MyDatabase"; //["Database"]  
            //sqlConnectionSB.UserID = "MyLogin";  // "@yourservername"  as suffix sometimes.  
            //sqlConnectionSB.Password = "MyPassword";
            //sqlConnectionSB.IntegratedSecurity = false;

            // Adjust these values if you like. (ADO.NET 4.5.1 or later.)  
            //sqlConnectionSB.ConnectRetryCount  = 3;
            //sqlConnectionSB.ConnectRetryInterval = 10;  // Seconds.  

            // Leave these values as they are.  
            //sqlConnectionSB.IntegratedSecurity = false;
            //sqlConnectionSB.Encrypt = true;

            return sqlConnectionSB.ToString();

        }
    }
}
