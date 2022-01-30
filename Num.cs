using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ADO_NET
{
    // ORM - класс со структурой, как у таблицы БД
    public class Num
    {
        public override string ToString()
        {
            return Id + " " + Val;
        }
        public String Id { get; set; }
        public int Val { get; set; }
    }

    // Context - окружение, отражение всей БД - 
    //  набора таблиц
    public class ContextDb
    {
        public List<Num> Nums;
        public ContextDb(SqlConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(
                    "Connection is NULL");
            }
            using (SqlCommand cmd =  // 0  1 
                new SqlCommand("SELECT id, num FROM nums", connection))
            {
                try
                {
                    SqlDataReader result = cmd.ExecuteReader();
                    Nums = new List<Num>();
                    while (result.Read())
                    {
                        Nums.Add(
                            new Num
                            {
                                Id = result.GetGuid(0).ToString(),
                                Val = result.GetInt32(1)
                            } );
                    }
                    result.Close();
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}
