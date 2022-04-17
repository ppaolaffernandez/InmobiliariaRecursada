using System.Data;
using System.Data.SqlClient;

namespace Inmobiliaria_2022.Models
{
	public class RepositorioPago
	{
		private readonly string connectionString;
		private readonly IConfiguration configuration;
		public RepositorioPago(IConfiguration configuration)
		{
			this.configuration = configuration;
			connectionString = configuration["ConnectionStrings:DefaultConnection"];
		}
		public int Alta(Pago p)
		{
			int res = -1;
			try
			{
				using (var connection = new SqlConnection(connectionString))
				{
					string sql = $"INSERT INTO Pagos (Numero, Fecha, Importe, ContratoId) " +
						$"VALUES (@numero, @fecha, @importe, @contratoId);" +
						$"SELECT SCOPE_IDENTITY();";//devuelve el id insertado
					using (var command = new SqlCommand(sql, connection))
					{
						command.CommandType = CommandType.Text;
						command.Parameters.AddWithValue("@numero", p.Numero);
						command.Parameters.AddWithValue("@fecha", p.Fecha);
						command.Parameters.AddWithValue("@importe", p.Importe);
						command.Parameters.AddWithValue("@ContratoId", p.ContratoId);
						connection.Open();
						res = Convert.ToInt32(command.ExecuteScalar());
						p.Id = res;
						connection.Close();
					}
				}

			}
			catch (Exception ex)
			{

			}
			return res;
		}
		public int Baja(int id)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"DELETE FROM Pagos WHERE Id = @id";
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

		public int Modificacion(Pago p)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"UPDATE Pagos SET " +
					"Numero=@numero, Fecha=@fecha, Importe=@importe, ContratoId=@contratoId " +
					"WHERE Id = @id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@numero", p.Numero);
					command.Parameters.AddWithValue("@fecha", p.Fecha);
					command.Parameters.AddWithValue("@importe", p.Importe);
					command.Parameters.AddWithValue("@alquilerId", p.ContratoId);
					command.Parameters.AddWithValue("@id", p.Id);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public IList<Pago> ObtenerTodos()
		{
			IList<Pago> res = new List<Pago>();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $" SELECT p.Id, Numero, Fecha, Importe, p.ContratoId " +
					$" FROM Pagos p, Contratos c, Inquilinos i " +
					$" WHERE p.ContratoId = c.Id and " +
					$"       c.InquilinoId = i.Id ";
				using (var command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Pago p = new Pago
						{
							Id = reader.GetInt32(0),
							Numero = reader.GetString(1),
							Fecha = reader.GetDateTime(2),
							Importe = reader.GetDecimal(3),
							ContratoId = reader.GetInt32(4),
							Contrato = new Contrato
							{
								Monto = reader.GetDecimal(5),
								Descripcion = reader.GetString(6),
							},
						};
						res.Add(p);
					}
					connection.Close();
				}
			}
			return res;
		}

		public Pago ObtenerPorId(int id)
		{
			Pago p = null;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT p.Id, Numero, Fecha, Importe, p.ContratoId, " +
					"c.Descripcion,c.Monto " +
					"FROM Pagos p INNER JOIN Contrato c ON c.Id = p.ContratoId " +
					" WHERE p.Id=@id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@id", SqlDbType.Int).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						p = new Pago
						{
							Id = reader.GetInt32(0),
							Numero = reader.GetString(1),
							Fecha = reader.GetDateTime(2),
							Importe = reader.GetDecimal(3),
							ContratoId = reader.GetInt32(4),
							Contrato = new Contrato
							{
								Descripcion = reader.GetString(5),
								Monto = reader.GetDecimal(6),
							}
						};
					}
					connection.Close();
				}
			}
			return p;
		}
		public IList<Pago> ObtenerPorContrato(string contratoId)
		{
			IList<Pago> res = new List<Pago>();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $" SELECT p.Id, p.Numero, p.Fecha, p.Importe, p.ContratoId, i.Direccion, iq.Nombre, iq.Apellido, " +
					$" c.Descripcion, c.Monto " +
					$" FROM Pagos p JOIN Contratos c ON p.ContratoId = c.Id " +
                    $"              JOIN Inquilinos iq ON c.InquilinoId = iq.Id " +
					$"	            JOIN Inmuebles i ON c.InmuebleId = i.Id " +
					$" WHERE p.ContratoId=@contratoId";

				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					connection.Open();
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@ContratoId", contratoId);
					var reader = command.ExecuteReader();//byaca realiz x v

					while (reader.Read())
					{

						Pago p = new Pago
						{
							Id = reader.GetInt32(0),
							Numero = reader.GetString(1),
							Fecha = reader.GetDateTime(2),
							Importe = reader.GetDecimal(3),
							ContratoId = reader.GetInt32(4),
							Contrato = new Contrato
							{
								inmueble = new Inmueble
								{
									Direccion = reader.GetString(5),
								},
								inquilino = new Inquilino
								{
									Nombre = reader.GetString(6),
									Apellido = reader.GetString(7),
								},
							}

						};
						res.Add(p);

					}
					connection.Close();
				}
			}
			return res;
		}
	public Pago ObtenerNumeroDePagoPorIdContrato(int id)
	{
		Pago p = null;
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			string sql = $"SELECT Numero FROM Pagos where ContratoId = @id order by Numero desc";//camb Id por contatoId 16/7/2022
				using (SqlCommand command = new SqlCommand(sql, connection))
			{
				command.Parameters.Add("@id", SqlDbType.Int).Value = id;
				command.CommandType = CommandType.Text;
				connection.Open();
				var reader = command.ExecuteReader();
				if (reader.Read())
				{
					p = new Pago
					{
						Numero = reader.GetString(0),
					};
				}
				connection.Close();
			}
		}
		return p;
	   }
	}
}

