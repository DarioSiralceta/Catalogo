using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dominio;
using negocio;
using System.Configuration;


namespace Catalogo
{
    public partial class frmAltaArticulo : Form
    {       
        private Articulo articulo = null;
        private OpenFileDialog archivo = null;
        public frmAltaArticulo()
        {
            InitializeComponent();
            
        }
        public frmAltaArticulo(Articulo articulo)
        {
            InitializeComponent();
            this.articulo = articulo;
            Text = "Modificar Articulo";
        }
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {

            string errores = ValidarCampos(); 
            if (!string.IsNullOrEmpty(errores)) 
            { MessageBox.Show("Por favor, complete los siguientes campos: " + errores);
                return;
            }

            CatalogoNegocio negocio = new CatalogoNegocio();

            try
            {
                if(articulo == null)
                    articulo = new Articulo();

                articulo.Codigo = txtCodigo.Text;
                articulo.Nombre = txtNombre.Text;
                articulo.Descripcion = txtDescripcion.Text;
                articulo.Precio = Convert.ToDecimal(txtPrecio.Text);
                articulo.ImagenUrl = txtImagenUrl.Text;
                articulo.Tipo = (Categoria)cboTipo.SelectedItem;
                articulo.Compañia = (Marca)cboCompañia.SelectedItem;


                if(articulo.Id != 0)
                {
                    negocio.modificar(articulo);
                    MessageBox.Show("Modificado exitosamente");

                }
                else
                {
                   
                    negocio.agregar(articulo);
                    MessageBox.Show("Agregado exitosamente");
                }
                if(archivo != null && !(txtImagenUrl.Text.ToUpper().Contains("HTTP")))
                    File.Copy(archivo.FileName, ConfigurationManager.AppSettings["images-folder"] + archivo.SafeFileName);


                Close();


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private string ValidarCampos()
        {
            List<string> errores = new List<string>();

            if (string.IsNullOrEmpty(txtCodigo.Text))
                errores.Add("Código");

            if (string.IsNullOrEmpty(txtNombre.Text))
                errores.Add("Nombre");

            if (string.IsNullOrEmpty(txtPrecio.Text) || !decimal.TryParse(txtPrecio.Text, out _))
                errores.Add("Precio (debe ser numérico)");

            return string.Join(", ", errores);
        }




        private void frmAltaArticulo_Load(object sender, EventArgs e)
        {
            TipoNegocio tipoNegocio = new TipoNegocio();
            CompañiaNegocio compañiaNegocio = new CompañiaNegocio();

            try
            {
                cboTipo.DataSource = tipoNegocio.listar();
                cboTipo.ValueMember = "Id";
                cboTipo.DisplayMember = "Descripcion";
                cboCompañia.DataSource = compañiaNegocio.listar();
                cboCompañia.ValueMember = "Id";
                cboCompañia.DisplayMember = "Descripcion";

                if(articulo != null)
                {
                    txtCodigo.Text = articulo.Codigo;
                    txtNombre.Text = articulo.Nombre;
                    txtDescripcion.Text = articulo.Descripcion;
                    txtImagenUrl.Text = articulo.ImagenUrl;
                    cargarImagen(articulo.ImagenUrl);
                    txtPrecio.Text = articulo.Precio.ToString();
                    cboTipo.SelectedValue = articulo.Tipo.Id;
                    cboCompañia.SelectedValue = articulo.Compañia.Id;






                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void txtImagenUrl_Leave(object sender, EventArgs e)
        {
            cargarImagen(txtImagenUrl.Text);
        }

        private void cargarImagen(string imagen)
        {
            try
            {


                pbxArticulos.Load(imagen);

            }
            catch
            {
                pbxArticulos.Load("https://img.freepik.com/vector-gratis/ilustracion-icono-galeria_53876-27002.jpg");
            }
        }

        private void lblPrecio_Click(object sender, EventArgs e)
        {

        }

        private void btnAgregarImg_Click(object sender, EventArgs e)
        {
            archivo = new OpenFileDialog();
            archivo.Filter = "jpg|*.jpg|png|*.png";
            if(archivo.ShowDialog() == DialogResult.OK)
            {
                txtImagenUrl.Text = archivo.FileName;
                cargarImagen(archivo.FileName);

               

            }
        }
    }
}
