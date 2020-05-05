using PickleAndHope.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PickleAndHope.Models;
using Microsoft.Data.SqlClient;

namespace PickleAndHope.DataAccess
{
    public class PickleRepository
    {

        static List<Pickle> _pickles = new List<Pickle> { new Pickle { Type = "Bread and Butter", NumberInStock = 5, Id = 1 } };

        const string ConnectionString = "Server = localhost; Database = PickleAndHope; Trusted_Connection = True;";

        public Pickle Add(Pickle pickle)
        {
            //pickle.Id = _pickles.Max(x => x.Id) + 1;
            //_pickles.Add(pickle);

            var sql = @"insert into Pickle(NumberInStock, Price, Size, Type)
                            output inserted.*
                                    values(@NumberInStock, @Price, @Size, @Type)";

            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = sql;

                cmd.Parameters.AddWithValue("NumberInStock", pickle.NumberInStock);
                cmd.Parameters.AddWithValue("Price", pickle.Price);
                cmd.Parameters.AddWithValue("Size", pickle.Size);
                cmd.Parameters.AddWithValue("Type", pickle.Type);

                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    var newPickle = MapReaderToPickle(reader);
                    return newPickle;
                }

                return null;
            }

        }

        public void Remove(string type)
        {
            throw new NotImplementedException();
        }

        public Pickle Update(Pickle pickle)
        {
            //var pickleToUpdate = _pickles.First(p => p.Type == pickle.Type);

            //pickleToUpdate.NumberInStock += pickle.NumberInStock;

            //return pickleToUpdate;

            var sql = @"update Pickle
	                        set NumberInStock = NumberInStock + @NewStock
                                output inserted.*		                        
                                    where Id = @Id";

            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = sql;

                cmd.Parameters.AddWithValue("NewStock", pickle.NumberInStock);
                cmd.Parameters.AddWithValue("Id", pickle.Id);

                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    var updatedPickle = MapReaderToPickle(reader);
                    return updatedPickle;
                }

                return null;
            }
        }

        public Pickle GetByType(string type)
        {
            //return _pickles.FirstOrDefault(p => p.Type == type);

            //GLOBAL VALUE NOW //var connectionString = "Server = localhost; Database = PickleAndHope; Trusted_Connection = True;";
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                var query = @"Select *
                               from Pickle
                                   where Type = @type";

                var cmd = connection.CreateCommand();
                cmd.CommandText = query;
                // parameters.AddWithValue protects against SQLInjection.
                // first variable "Type" has to match with the query value, second value must match passed in value.
                cmd.Parameters.AddWithValue("Type", type);

                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return MapReaderToPickle(reader);
                }

                return null;
            }
                                
        }

        public List<Pickle> GetAll()
        {
            //return _pickles; C# version

            // select * from Pickle T-SQL Version

            // Connection String - connection command
            //var connectionString = "Server = localhost; Database = PickleAndHope; Trusted_Connection = True;";

            // SQL Connection

            var connection = new SqlConnection(ConnectionString);
            connection.Open();

            // SQL command

            var cmd = connection.CreateCommand();
            cmd.CommandText = "Select * from Pickle";
            var reader = cmd.ExecuteReader();

            var pickles = new List<Pickle>();

            while (reader.Read())
            {
                // using the (int) keyword is called direct casting or explicit cast.
                //var id = (int)reader["Id"];
                //var type = (string)reader["Type"];
                //var numberInStock = (int)reader["NumberInStock"];
                //var price = (decimal)reader["Price"];
                //var size = (string)reader["Size"];

                // this way is better though
                var pickle = new Pickle
                {
                    Id = (int)reader["Id"],
                    Type = (string)reader["Type"],
                    NumberInStock = (int)reader["NumberInStock"],
                    Price = (decimal)reader["Price"],
                    Size = (string)reader["Size"]
                };


                pickles.Add(pickle);
            }

            // Closing the connection is VERY IMPORTANT
            // SQL only allows so many connections at once. If you have too many you will run into blocks.
            connection.Close();
            return pickles;
        }

        public Pickle GetById(int id)
        {
           //return _pickles.FirstOrDefault(p => p.Id == id);

            using(var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                var query = @"select * from Pickle where Id = @id";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("Id", id);

                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return MapReaderToPickle(reader);
                }

                return null;
                
            }
        }

        Pickle MapReaderToPickle(SqlDataReader reader)
        {
            var pickle = new Pickle
            {
                Id = (int)reader["Id"],
                Type = (string)reader["Type"],
                NumberInStock = (int)reader["NumberInStock"],
                Price = (decimal)reader["Price"],
                Size = (string)reader["Size"]
            };

            return pickle;
        }
    }
}
