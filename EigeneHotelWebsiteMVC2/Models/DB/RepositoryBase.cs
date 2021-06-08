using MySql.Data.MySqlClient;

namespace EigeneHotelWebsiteMVC2.Models.DB
{
    public class RepositoryBase : IRepositoryBase
    {
        protected MySqlConnection _connection;
        protected string _connectionString = "server=localhost;database=db_hotel_1;uid=root;pwd=root";

        public void Open()
        {
            if (this._connection == null)
            {
                this._connection = new MySqlConnection(this._connectionString);
            }
            if (this._connection.State != System.Data.ConnectionState.Open)
            {
                this._connection.Open();
            }
        }
        public void Close()
        {
            if (this._connection != null && this._connection.State == System.Data.ConnectionState.Open)
            {
                this._connection.Close();
            }
        }
    }
}
