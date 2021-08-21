using Acr.UserDialogs;
using DiagnosticoImpresoras.Controladores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DiagnosticoImpresoras.Vistas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class vistaServicioImpresion : ContentPage
    {
        private ctrlServicioImpresion controlador = null;

        public vistaServicioImpresion()
        {
            try
            {
                InitializeComponent();
                controlador = new ctrlServicioImpresion(this);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        private void btnDiagnosticarConexion_Clicked(object sender, EventArgs e)
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Procesando...", MaskType.Black);
                controlador.btnDiagnosticarConexion_Clicked();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        private void btnDiagnosticarLenguaje_Clicked(object sender, EventArgs e)
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Procesando...", MaskType.Black);
                controlador.btnDiagnosticarLenguaje_Clicked();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        private void btnDiagnosticarEstado_Clicked(object sender, EventArgs e)
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Procesando...", MaskType.Black);
                controlador.btnDiagnosticarEstado_Clicked();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        private void btnDiagnosticarCaret_Clicked(object sender, EventArgs e)
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Procesando...", MaskType.Black);
                controlador.btnDiagnosticarCaret_Clicked();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        private void btnPruebaImpresion_Clicked(object sender, EventArgs e)
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Procesando...", MaskType.Black);
                controlador.btnPruebaImpresion_Clicked();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }
    }
}