using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using dominio;
using System.Runtime.InteropServices;

namespace negocio
{
    public class CatalogoNegocio
    {
        public List<Articulo> listar()

        {
            List<Articulo> lista = new List<Articulo>();
            SqlConnection conexion = new SqlConnection();
            SqlCommand comando = new SqlCommand();
            SqlDataReader lector;


            try
            {
                conexion.ConnectionString = "server=.\\SQLEXPRESS; database=CATALOGO_DB; integrated security= true";
                comando.CommandType = System.Data.CommandType.Text;
                comando.CommandText = "select Codigo, Nombre, A.Descripcion, ImagenUrl, Precio, C.Descripcion Categoria, M.Descripcion Marca, A.IdCategoria, A.IdMarca, A.Id from ARTICULOS A, CATEGORIAS C, MARCAS M Where C.Id = A.IdCategoria And M.Id = A.IdMarca";
                comando.Connection = conexion;

                conexion.Open();
                lector = comando.ExecuteReader();

                while (lector.Read())
                {
                    Articulo aux = new Articulo();
                    aux.Id = (int)lector["Id"];
                    aux.Codigo = (string)lector["Codigo"];
                    aux.Nombre = (string)lector["Nombre"];
                    aux.Descripcion = (string)lector["Descripcion"];
                    if (!(lector["ImagenUrl"] is DBNull))
                        aux.ImagenUrl = (string)lector["ImagenUrl"];
                    if(!(lector["Precio"] is DBNull))
                        aux.Precio = lector.GetDecimal(4);
                    aux.Tipo = new Categoria();
                    aux.Tipo.Id = (int)lector["IdCategoria"];
                    aux.Tipo.Descripcion = (string)lector["Categoria"];
                    aux.Compañia = new Marca();
                    aux.Compañia.Id = (int)lector["IdMarca"];
                    aux.Compañia.Descripcion = (string)lector["Marca"];

                    lista.Add(aux);


                }
                conexion.Close();
                return lista;
            }
            catch (Exception ex)
            {

                throw ex;
            }



        }



        public void agregar(Articulo nuevo)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("INSERT INTO ARTICULOS (Codigo, Nombre, Descripcion, Precio, ImagenUrl, IdMarca, IdCategoria) VALUES (@Codigo, @Nombre, @Descripcion, @Precio,@ImagenUrl, @IdMarca, @IdCategoria)");
                datos.setearParametro("@Codigo", nuevo.Codigo);
                datos.setearParametro("@Nombre", nuevo.Nombre);
                datos.setearParametro("@Descripcion", nuevo.Descripcion);
                datos.setearParametro("@Precio", nuevo.Precio);
                datos.setearParametro("@ImagenUrl", nuevo.ImagenUrl);
                datos.setearParametro("@IdMarca", nuevo.Compañia.Id);
                datos.setearParametro("@IdCategoria", nuevo.Tipo.Id);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void modificar(Articulo articulo)
        {

            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("update ARTICULOS set Codigo = @codigo, Nombre = @nombre, Descripcion = @descripcion, IdMarca = @idMarca, IdCategoria = @idCategoria, ImagenUrl = @imagen, Precio = @precio where Id = @id");
                datos.setearParametro("@codigo", articulo.Codigo);
                datos.setearParametro("@nombre", articulo.Nombre);
                datos.setearParametro("@descripcion", articulo.Descripcion);
                datos.setearParametro("@idMarca", articulo.Compañia.Id);
                datos.setearParametro("@idCategoria", articulo.Tipo.Id);
                datos.setearParametro("@imagen", articulo.ImagenUrl);
                datos.setearParametro("@precio", articulo.Precio);
                datos.setearParametro("@id", articulo.Id);

                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {

                datos.cerrarConexion();
            }

            
        }

        public void eliminar(int id)
        {
            try
            {
                AccesoDatos datos = new AccesoDatos();
                datos.setearConsulta("delete from ARTICULOS where id = @id");
                datos.setearParametro("@id", id);
                datos.ejecutarAccion();
            }
            catch (Exception ex )
            {

                throw ex;
            }




        }

        public List<Articulo> filtrar(string campo, string criterio, string filtro)
        {

            List<Articulo> lista = new List<Articulo>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
               
                string consulta = ("select Codigo, Nombre, A.Descripcion, ImagenUrl, Precio, C.Descripcion Categoria, M.Descripcion Marca, A.IdCategoria, A.IdMarca, A.Id from ARTICULOS A, CATEGORIAS C, MARCAS M Where C.Id = A.IdCategoria And M.Id = A.IdMarca And ");
                if(campo == "Precio")
                {
                    switch (criterio)
                    {
                        case "Mayor a":
                            consulta += "Precio > " + filtro;
                            break;
                        case "Menor a":
                            consulta += "Precio < " + filtro;       
                            break;
                        default:
                            consulta += "Precio = " + filtro;
                            break;
                    }

                }
                else if(campo == "Categoria")
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += "C.Descripcion like '" + filtro + "%' ";
                            break;
                        case "Termina con":
                            consulta += "C.Descripcion like '%" + filtro + "'";
                            break;
                        default:
                            consulta += "C.Descripcion like '%" + filtro + "%'";
                            break;
                    }

                }
                else 
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += "M.Descripcion like '" + filtro + "%' ";
                            break;
                        case "Termina con":
                            consulta += "M.Descripcion like '%" + filtro + "'";
                            break;
                        default:
                            consulta += "M.Descripcion like '%" + filtro + "%'";
                            break;

                    }




                }

                datos.setearConsulta(consulta);
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    Articulo aux = new Articulo();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Codigo = (string)datos.Lector["Codigo"];
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Descripcion = (string)datos.Lector["Descripcion"];
                    if (!(datos.Lector["ImagenUrl"] is DBNull))
                        aux.ImagenUrl = (string)datos.Lector["ImagenUrl"];
                    if (!(datos.Lector["Precio"] is DBNull))
                        aux.Precio = datos.Lector.GetDecimal(4);
                    aux.Tipo = new Categoria();
                    aux.Tipo.Id = (int)datos.Lector["IdCategoria"];
                    aux.Tipo.Descripcion = (string)datos.Lector["Categoria"];
                    aux.Compañia = new Marca();
                    aux.Compañia.Id = (int)datos.Lector["IdMarca"];
                    aux.Compañia.Descripcion = (string)datos.Lector["Marca"];

                    lista.Add(aux);


                }
                return lista;

            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

        public List<Articulo> Restablecer()
        {
            List<Articulo> lista = new List<Articulo>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                string consulta = "select Codigo, Nombre, A.Descripcion, ImagenUrl, Precio, C.Descripcion Categoria, M.Descripcion Marca, A.IdCategoria, A.IdMarca, A.Id from ARTICULOS A, CATEGORIAS C, MARCAS M Where C.Id = A.IdCategoria And M.Id = A.IdMarca";
                datos.setearConsulta(consulta);
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Articulo aux = new Articulo();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Codigo = (string)datos.Lector["Codigo"];
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Descripcion = (string)datos.Lector["Descripcion"];
                    aux.Precio = (decimal)datos.Lector["Precio"];
                    if (!(datos.Lector["ImagenUrl"] is DBNull))
                        aux.ImagenUrl = (string)datos.Lector["ImagenUrl"];

                    aux.Tipo = new Categoria();
                    aux.Tipo.Id = (int)datos.Lector["IdCategoria"];
                    aux.Tipo.Descripcion = (string)datos.Lector["Categoria"];
                    aux.Compañia = new Marca();
                    aux.Compañia.Id = (int)datos.Lector["IdMarca"];
                    aux.Compañia.Descripcion = (string)datos.Lector["Marca"];

                    lista.Add(aux);
                }
                return lista;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

    }







}
