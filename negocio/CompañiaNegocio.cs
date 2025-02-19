﻿using dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace negocio
{
     public class CompañiaNegocio
    {
        public List<Marca> listar()
        {

			List<Marca> lista = new List<Marca>();
			AccesoDatos datos = new AccesoDatos();
			try
			{
				datos.setearConsulta("select id, Descripcion from MARCAS");
				datos.ejecutarLectura();

				while(datos.Lector.Read())
				{
					Marca aux = new Marca();
					aux.Id = (int)datos.Lector["Id"];
					aux.Descripcion = (string)datos.Lector["Descripcion"];

					lista.Add(aux);

				}


				return lista;
			}
			catch (Exception)
			{

				throw;
			}
			finally
			{

				datos.cerrarConexion();

			}
        }
    }
}
