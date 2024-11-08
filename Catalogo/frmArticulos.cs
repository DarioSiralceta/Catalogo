using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dominio;
using negocio;

namespace Catalogo
{
    public partial class frmCatalogo : Form

    {

        private List<Articulo> listaArticulo;
        public frmCatalogo()
        {
            InitializeComponent();
        }

        public frmCatalogo(Articulo articulo)
        {
            InitializeComponent();
            
        }
        private void frmCatalogo_Load(object sender, EventArgs e)
        {
            cargar();
            cboCampo.Items.Add("Precio");
            cboCampo.Items.Add("Categoria");
            cboCampo.Items.Add("Marca");
        }

        private void dgvArticulos_SelectionChanged(object sender, EventArgs e)
        {

            if (dgvArticulos.CurrentRow != null)
            {
                Articulo seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;
                cargarImagen(seleccionado.ImagenUrl);
            }
        }

        private void cargar()
        {
            CatalogoNegocio negocio = new CatalogoNegocio();
            try
            {
                listaArticulo = negocio.listar();
                dgvArticulos.DataSource = negocio.listar();
                ocultarColumnas();
                cargarImagen(listaArticulo[0].ImagenUrl);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }


        }

        private void ocultarColumnas()
        {

            dgvArticulos.Columns["ImagenUrl"].Visible = false;
            dgvArticulos.Columns["Id"].Visible = false;

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


        private void btnAgregar_Click(object sender, EventArgs e)
        {
            frmAltaArticulo alta = new frmAltaArticulo();
            alta.ShowDialog();
            cargar();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            Articulo seleccionado;
            seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;


            frmAltaArticulo modificar  = new frmAltaArticulo(seleccionado);
            modificar .ShowDialog();
            cargar();
        }

        private void btnEliminarFisico_Click(object sender, EventArgs e)
        {
            CatalogoNegocio negocio = new CatalogoNegocio();
            Articulo seleccionado;
            try
            {
                DialogResult respuesta = MessageBox.Show("De verdad desea eliminarlo?", "Eliminando", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (respuesta == DialogResult.Yes)
                {
                    seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;
                    negocio.eliminar(seleccionado.Id);
                    cargar();


                }
            }

            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }



        private bool validarFiltro()
        {
            if(cboCampo.SelectedIndex < 0)
            {

                MessageBox.Show("Por favor, seleccione el campo para filtrar");
                return true;

            }
            if (cboCriterio.SelectedIndex < 0)
            {

                MessageBox.Show("Por favor, seleccione el criterio para filtrar");
                return true;
            }
            if(cboCampo.SelectedItem.ToString() == "Precio")
            {   
                if(string.IsNullOrEmpty(txtFiltroAvanzado.Text))
                {
                    MessageBox.Show("Por favor cargue con un valor numerico el filtro");
                    return true;

                }
                if (! (soloNumeros(txtFiltroAvanzado.Text)))
                {
                    MessageBox.Show("Por favor ingrese solo numeros");
                    return true;


                }



            }
            

            return false;   

        }

        private bool soloNumeros(string cadena)
        {
            foreach  (char caracter in cadena)
            {
                if(! char.IsNumber(caracter))
                    return false;
            }
            return true;
            
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
           
            CatalogoNegocio negocio = new CatalogoNegocio();
            try
            {

                if (validarFiltro())
                    return;
                string campo = cboCampo.SelectedItem.ToString();
                string criterio = cboCriterio.SelectedItem.ToString();
                string filtro = txtFiltroAvanzado.Text;
                dgvArticulos.DataSource = negocio.filtrar(campo, criterio,filtro);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }


            
        }
   

        private void txtFiltroRapido_TextChanged(object sender, EventArgs e)
        {
            List<Articulo> listaFiltrada;
            string filtro = txtFiltroRapido.Text;

            if (filtro.Length >= 2)
            {


                listaFiltrada = listaArticulo.FindAll(x => x.Codigo.ToUpper().Contains(filtro.ToUpper()) || x.Compañia.Descripcion.ToUpper().Contains(filtro.ToUpper()));

            }
            else
            {

                listaFiltrada = listaArticulo;

            }

            dgvArticulos.DataSource = null;
            dgvArticulos.DataSource = listaFiltrada;
            ocultarColumnas();
        }

        private void cboCampo_SelectedIndexChanged(object sender, EventArgs e)
        {

                if (cboCampo.SelectedItem == null)
                {
                    return;
                }
                string opcion = cboCampo.SelectedItem.ToString();
                if (opcion == "Precio")
                {
                    cboCriterio.Items.Clear();
                    cboCriterio.Items.Add("Mayor a");
                    cboCriterio.Items.Add("Menor a");
                    cboCriterio.Items.Add("Igual a");

                }
                else
                {
                    cboCriterio.Items.Clear();
                    cboCriterio.Items.Add("Comienza con");
                    cboCriterio.Items.Add("Termina con");
                    cboCriterio.Items.Add("Contiene");



                }
            

        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            CatalogoNegocio negocio = new CatalogoNegocio();

            try
            {
                txtFiltroAvanzado?.Clear(); 
                txtFiltroRapido?.Clear();

                listaArticulo = negocio.Restablecer(); 
                if (listaArticulo == null || listaArticulo.Count == 0) 
                { MessageBox.Show("No se encontraron artículos para restablecer."); 
                    return;
                }

                dgvArticulos.DataSource = null; 
                dgvArticulos.DataSource = listaArticulo;
                ocultarColumnas();

                cboCampo.SelectedIndex = -1; 
                cboCriterio.SelectedIndex = -1;
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error al restablecer los filtros: " + ex.Message);
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }




}
