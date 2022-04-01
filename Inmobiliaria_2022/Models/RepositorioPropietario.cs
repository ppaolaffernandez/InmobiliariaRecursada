using System.Data;
using System.Data.SqlClient;

namespace Inmobiliaria_2022.Models
{
	public class RepositorioPropietario
	{
		private readonly string connectionString;
		private readonly IConfiguration configuration;

		public RepositorioPropietario(IConfiguration configuration)
		{
			this.configuration = configuration;
			connectionString = configuration["ConnectionStrings:DefaultConnection"];
		}
		public int Alta(Propietario p)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"INSERT INTO Propietarios (Nombre, Apellido, Dni, Telefono, Email, Clave) " +
					$"VALUES (@nombre, @apellido, @dni, @telefono, @email, @clave);" +
					"SELECT SCOPE_IDENTITY();";//devuelve el id insertado (LAST_INSERT_ID para mysql)
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@nombre", p.Nombre);
					command.Parameters.AddWithValue("@apellido", p.Apellido);
					command.Parameters.AddWithValue("@dni", p.Dni);
					command.Parameters.AddWithValue("@telefono", p.Telefono);
					command.Parameters.AddWithValue("@email", p.Email);
					command.Parameters.AddWithValue("@clave", p.Clave);
					connection.Open();
					res = Convert.ToInt32(command.ExecuteScalar());
					p.Id = res;
					connection.Close();
				}
			}
			return res;
		}
		public int Baja(int id)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"DELETE FROM Propietarios WHERE Id = @id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@id", id);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}
		public int Modificacion(Propietario p)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"UPDATE Propietarios SET Nombre=@nombre, Apellido=@apellido, Dni=@dni, Telefono=@telefono, Email=@email, Clave=@clave " +
					$"WHERE Id = @id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@nombre", p.Nombre);
					command.Parameters.AddWithValue("@apellido", p.Apellido);
					command.Parameters.AddWithValue("@dni", p.Dni);
					command.Parameters.AddWithValue("@telefono", p.Telefono);
					command.Parameters.AddWithValue("@email", p.Email);
					command.Parameters.AddWithValue("@clave", p.Clave);
					command.Parameters.AddWithValue("@id", p.Id);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}
		public IList<Propietario> ObtenerTodos()
		{
			IList<Propietario> res = new List<Propietario>();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT Id, Dni, Nombre, Apellido,  Email, Telefono, Clave" +
					$" FROM Propietarios";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Propietario p = new Propietario
						{
							Id = reader.GetInt32(0),
							Dni = reader.GetString(1),
							Nombre = reader.GetString(2),
							Apellido = reader.GetString(3),
							Email = reader.GetString(4),
							Telefono = reader["Telefono"].ToString(),
							Clave = reader.GetString(6),

						};
						res.Add(p);
					}
					connection.Close();
				}
			}
			return res;
		}
		public Propietario ObtenerPorId(int id)
		{
			Propietario p = null;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT Id, Dni, Nombre, Apellido, Email, Telefono, Clave FROM Propietarios" +
					$" WHERE Id=@id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@id", SqlDbType.Int).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						p = new Propietario
						{
							Id = reader.GetInt32(0),
							Dni = reader.GetString(1),
							Nombre = reader.GetString(2),
							Apellido = reader.GetString(3),
							Email = reader.GetString(4),
							Telefono = reader.GetString(5),
							Clave=(String)reader["Clave"]

						};
					}
					connection.Close();
				}
			}
			return p;
		}
	}
}
