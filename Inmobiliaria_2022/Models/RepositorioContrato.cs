using System.Data;
using System.Data.SqlClient;

namespace Inmobiliaria_2022.Models
{
	public class RepositorioContrato
	{
		private readonly string connectionString;
		private readonly IConfiguration configuration;
		public RepositorioContrato(IConfiguration configuration)
		{
			this.configuration = configuration;
			connectionString = configuration["ConnectionStrings:DefaultConnection"];
		}
		public int Alta(Contrato c)
		{
			int res = -1;
			try
			{
				using (var connection = new SqlConnection(connectionString))
				{
					string sql = $"INSERT INTO Contratos (Descripcion, FechaAlta, FechaBaja, Monto, InmuebleId, InquilinoId,EstaHabilitado) " +
						$"VALUES (@descripcion, @fechaAlta, @fechaBaja, @monto, @inmuebleId, @inquilinoId,0);" +
						$"SELECT SCOPE_IDENTITY();";//devuelve el id insertado
					using (var command = new SqlCommand(sql, connection))
					{
						command.CommandType = CommandType.Text;
						command.Parameters.AddWithValue("@descripcion", c.Descripcion);
						command.Parameters.AddWithValue("@fechaAlta", c.FechaAlta);
						command.Parameters.AddWithValue("@fechaBaja", c.FechaBaja);
						command.Parameters.AddWithValue("@monto", c.Monto);
						command.Parameters.AddWithValue("@inmuebleId", c.InmuebleId);
						command.Parameters.AddWithValue("@inquilinoId", c.InquilinoId);
						connection.Open();
						res = Convert.ToInt32(command.ExecuteScalar());
						c.Id = res;
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
				string sql = $"DELETE FROM Contratos WHERE Id = @id";
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
		public int Modificacion(Contrato c)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"UPDATE Contratos SET " +
					"Descripcion=@descripcion, FechaAlta=@fechaAlta, FechaBaja=@fechaBaja, Monto=@monto, InmuebleId=@inmuebleId, InquilinoId=@inquilinoId " +
					"WHERE Id = @id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@descripcion", c.Descripcion);
					command.Parameters.AddWithValue("@fechaAlta", c.FechaAlta);
					command.Parameters.AddWithValue("@fechaBaja", c.FechaBaja);
					command.Parameters.AddWithValue("@monto", c.Monto);
					command.Parameters.AddWithValue("@inmuebleId", c.InmuebleId);
					command.Parameters.AddWithValue("@inquilinoId", c.InquilinoId);
					command.Parameters.AddWithValue("@id", c.Id);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}
		
		public IList<Contrato> ObtenerTodos()
		{
			IList<Contrato> res = new List<Contrato>();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $" SELECT c.Id, Descripcion, FechaAlta, FechaBaja, Monto, c.InmuebleId, c.InquilinoId," +
					$" iq.Nombre, iq.Apellido," +
					$" i.Direccion, i.Costo," +
					$" p.Nombre, p.Apellido" +
					$" FROM Contratos c join Inmuebles i ON c.InmuebleId = i.Id " +
					$"				    join Inquilinos iq ON c.InquilinoId = iq.Id " +
					$"				    join Propietarios p ON p.Id = i.PropietarioId ";

				using (var command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Contrato a = new Contrato
						{
							Id = reader.GetInt32(0),
							Descripcion = reader.GetString(1),
							FechaAlta = reader.GetDateTime(2),
							FechaBaja = reader.GetDateTime(3),
							Monto = reader.GetDecimal(4),
							InmuebleId = reader.GetInt32(5),
							InquilinoId = reader.GetInt32(6),
							inquilino = new Inquilino
							{
								Nombre = reader.GetString(7),
								Apellido = reader.GetString(8),
							},
							inmueble = new Inmueble
							{
								Direccion = reader.GetString(9),
								Costo = reader.GetDecimal(10),
								Propietario = new Propietario
								{
									Nombre = reader.GetString(11),
									Apellido = reader.GetString(12),
								},
							},
						};
						res.Add(a);
					}
					connection.Close();
				}
			}
			return res;
		}


		public Contrato ObtenerPorId(int id)
		{
			Contrato entidad = null;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT c.Id, Descripcion, FechaAlta, FechaBaja, Monto, c.InmuebleId, c.InquilinoId, " +
					$" inm.Direccion, inm.Costo, " +
					$" i.Nombre, i.Apellido " +
					$" FROM Contratos c, Inmuebles inm, Inquilinos i " +
					$" WHERE c.InmuebleId = inm.Id and " +
					$"       c.InquilinoId = i.Id and " +
					$"       c.Id=@id";

				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@id", SqlDbType.Int).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						entidad = new Contrato
						{
							Id = reader.GetInt32(0),
							Descripcion = reader.GetString(1),
							FechaAlta = reader.GetDateTime(2),
							FechaBaja = reader.GetDateTime(3),
							Monto = reader.GetDecimal(4),
							InmuebleId = reader.GetInt32(5),
							InquilinoId = reader.GetInt32(6),
							inmueble = new Inmueble {
								Direccion = reader.GetString(7),
								Costo = reader.GetDecimal(8),
							},
							inquilino = new Inquilino {
								Nombre = reader.GetString(9),
								Apellido = reader.GetString(10),
							},
						};
					} connection.Close();
				}
			} return entidad;
		}
		//..........................................VIGENTES.............................................
		public IList<Contrato> ObtenerVigentes()
		{
			IList<Contrato> res = new List<Contrato>();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $" SELECT c.Id, Descripcion, FechaAlta, FechaBaja, Monto, c.InmuebleId, c.InquilinoId," +
				$" iq.Nombre, iq.Apellido," +
				$" i.Direccion, i.Costo," +
				$" p.Nombre, p.Apellido" +
				$" FROM Contratos c join Inmuebles i ON c.InmuebleId = i.Id " +
				$"				    join Inquilinos iq ON c.InquilinoId = iq.Id " +
				$"				    join Propietarios p ON p.Id = i.PropietarioId " +
				$"                          WHERE c.FechaBaja >= GETDATE()";

				using (var command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Contrato a = new Contrato
						{
							Id = reader.GetInt32(0),
							Descripcion = reader.GetString(1),
							FechaAlta = reader.GetDateTime(2),
							FechaBaja = reader.GetDateTime(3),
							Monto = reader.GetDecimal(4),
							InmuebleId = reader.GetInt32(5),
							InquilinoId = reader.GetInt32(6),
							inquilino = new Inquilino
							{
								Nombre = reader.GetString(7),
								Apellido = reader.GetString(8),
							},
							inmueble = new Inmueble
							{
								Direccion = reader.GetString(9),
								Costo = reader.GetDecimal(10),
								Propietario = new Propietario
								{
									Nombre = reader.GetString(11),
									Apellido = reader.GetString(12),
								},

							},
						};
						res.Add(a);
					}
					connection.Close();
				}
			}
			return res;
		}
		//..........................................NO VIGENTES.............................................

		public IList<Contrato> ObtenerNoVigentes()
		{
			IList<Contrato> res = new List<Contrato>();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $" SELECT c.Id, Descripcion, FechaAlta, FechaBaja, Monto, c.InmuebleId, c.InquilinoId," +
				$" iq.Nombre, iq.Apellido," +
				$" i.Direccion, i.Costo," +
				$" p.Nombre, p.Apellido" +
				$" FROM Contratos c join Inmuebles i ON c.InmuebleId = i.Id " +
				$"				    join Inquilinos iq ON c.InquilinoId = iq.Id " +
				$"				    join Propietarios p ON p.Id = i.PropietarioId " +
				$"                          WHERE c.FechaBaja < GETDATE()";

				using (var command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Contrato a = new Contrato
						{
							Id = reader.GetInt32(0),
							Descripcion = reader.GetString(1),
							FechaAlta = reader.GetDateTime(2),
							FechaBaja = reader.GetDateTime(3),
							Monto = reader.GetDecimal(4),
							InmuebleId = reader.GetInt32(5),
							InquilinoId = reader.GetInt32(6),
							inquilino = new Inquilino
							{
								Nombre = reader.GetString(7),
								Apellido = reader.GetString(8),
							},
							inmueble = new Inmueble
							{
								Direccion = reader.GetString(9),
								Costo = reader.GetDecimal(10),
								Propietario = new Propietario
								{
									Nombre = reader.GetString(11),
									Apellido = reader.GetString(12),
								},

							},
						};
						res.Add(a);
					}
					connection.Close();
				}
			}
			return res;
		}

	public IList<Contrato> ObtenerPorDireccion(string direccion)
	{
		IList<Contrato> res = new List<Contrato>();
		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			string sql = $" SELECT c.Id, Descripcion, FechaAlta, FechaBaja, Monto, c.InmuebleId, c.InquilinoId," +
						 $" iq.Nombre, iq.Apellido," +
					     $" i.Direccion, i.Costo," +
					     $" p.Nombre, p.Apellido" +
					     $" FROM Contratos c join Inmuebles i ON c.InmuebleId = i.Id " +
					     $"				     join Inquilinos iq ON c.InquilinoId = iq.Id " +
					     $"				     join Propietarios p ON p.Id = i.PropietarioId " +
					     $" WHERE i.Direccion like @direccion";

				using (SqlCommand command = new SqlCommand(sql, connection))
			{
				command.Parameters.Add("@direccion", SqlDbType.NVarChar).Value = direccion;
				command.CommandType = CommandType.Text;
				connection.Open();
				var reader = command.ExecuteReader();
				while (reader.Read())
				{
						Contrato c = new Contrato
						{
							Id = reader.GetInt32(0),
							Descripcion = reader.GetString(1),
							FechaAlta = reader.GetDateTime(2),
							FechaBaja = reader.GetDateTime(3),
							Monto = reader.GetDecimal(4),
							InmuebleId = reader.GetInt32(5),
							InquilinoId = reader.GetInt32(6),
							inquilino = new Inquilino
							{
								Nombre = reader.GetString(7),
								Apellido = reader.GetString(8),
							},
							inmueble = new Inmueble
							{
								Direccion = reader.GetString(9),
								Costo = reader.GetDecimal(10),
								Propietario = new Propietario
								{
									Nombre = reader.GetString(11),
									Apellido = reader.GetString(12),
								},

							},
						};
						res.Add(c);
				}
					connection.Close();
			}
		}
			return res;
	}

	}
}



