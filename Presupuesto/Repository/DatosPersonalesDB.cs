﻿using Domain.Dtos.Cliente;
using Presupuesto.DataBase;
using System.Data;
using System.Data.SqlClient;
using System.Net.NetworkInformation;

namespace Presupuesto.Repository
{
    public class DatosPersonalesDB
    {
        //public async Task<DatosPersonales> DatosPersonalesByDni(int dni)
        //{
        //    DatosPersonales clienteData = new DatosPersonales();
        //    using (SqlConnection connection = Connection.ObtenerConexion())
        //    {
        //        SqlCommand command = new SqlCommand(
        //            string.Format("Select Dni, Nombre, Apellido, Domicilio, Fecha_Nacimiento from DatosPersonales where Dni like {0}", dni),
        //              connection
        //        );

        //        SqlDataReader reader = command.ExecuteReader();

        //        while (reader.Read())
        //        {
        //            clienteData.Dni = reader.GetInt32(0);
        //            clienteData.Nombre = reader.GetString(1);
        //            clienteData.Apellido = reader.GetString(2);
        //            clienteData.Domicilio = reader.GetString(3);
        //            clienteData.Fecha_Nacimiento = reader.GetDateTime(4);
        //        }

        //        connection.Close();
        //        return clienteData;
        //    }
        //}

        //public async Task<List<DatosPersonales>> Clientes()
        //{
        //    using (SqlConnection connection = Connection.ObtenerConexion())
        //    {
        //        SqlCommand command = new SqlCommand(
        //            string.Format("Select Dni, Nombre, Apellido, Domicilio, Fecha_Nacimiento from DatosPersonales"),
        //              connection
        //        );

        //        SqlDataReader reader = command.ExecuteReader();

        //        var clientes = new List<DatosPersonales>();

        //        while (reader.Read())
        //        {
        //            var cliente = new DatosPersonales()
        //            {
        //                Dni = reader.GetInt32(0),
        //                Nombre = reader.GetString(1),
        //                Apellido = reader.GetString(2),
        //                Domicilio = reader.GetString(3),
        //                Fecha_Nacimiento = reader.GetDateTime(4)

        //            };
        //            clientes.Add(cliente);
        //        }

        //        connection.Close();
        //        return clientes;
        //    }
        //}

        // public async Task<string> EliminarUsuario(int dni)
        //{
        //    using (SqlConnection conn = Connection.ObtenerConexion())
        //    {
        //        using (SqlCommand cmd = new SqlCommand())
        //        {
        //            cmd.Connection = conn;
        //            cmd.CommandType = CommandType.Text;
        //            cmd.CommandText = @"DELETE FROM DatosPersonales WHERE Dni = @Dni";

        //            cmd.Parameters.AddWithValue("@Dni", dni);

        //            //conn.Open();
        //            cmd.ExecuteNonQuery();

        //            conn.Close();
        //            return "El Cliente se eliminó correctamente.";
        //        }
        //    }
        //}

        //public async Task<DatosPersonales> CrearUsuario(DatosPersonales nuevoCliente)
        //{

        //}
    }
}
  