using DiagnosticoImpresoras.Interfaces;
using DiagnosticoImpresoras.Vistas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DiagnosticoImpresoras.Controladores
{
    public class ctrlServicioImpresion
    {
        internal vistaServicioImpresion view { get; set; }

        IServicioImpresion _servicio;

        internal ctrlServicioImpresion(vistaServicioImpresion pview)
        {
            view = pview;
            _servicio = DependencyService.Get<IServicioImpresion>();
        }

        internal async void btnDiagnosticarConexion_Clicked()
        {
            bool Respuesta = false;
            Respuesta = await _servicio.DiagnosticarConexion();

            if (Respuesta)
            {
                view.FindByName<Button>("btnDiagnosticarConexion").BackgroundColor = Color.Green;
            }
            else
            {
                view.FindByName<Button>("btnDiagnosticarConexion").BackgroundColor = Color.Red;
            }            
        }

        internal async void btnDiagnosticarLenguaje_Clicked()
        {
            bool Respuesta = false;
            Respuesta = await _servicio.DiagnosticarLenguaje();

            if (Respuesta)
            {
                view.FindByName<Button>("btnDiagnosticarLenguaje").BackgroundColor = Color.Green;
            }
            else
            {
                view.FindByName<Button>("btnDiagnosticarLenguaje").BackgroundColor = Color.Red;
            }
        }

        internal async void btnDiagnosticarEstado_Clicked()
        {
            bool Respuesta = false;
            Respuesta = await _servicio.DiagnosticarEstado();

            if (Respuesta)
            {
                view.FindByName<Button>("btnDiagnosticarEstado").BackgroundColor = Color.Green;
            }
            else
            {
                view.FindByName<Button>("btnDiagnosticarEstado").BackgroundColor = Color.Red;
            }
        }

        internal async void btnDiagnosticarCaret_Clicked()
        {
            bool Respuesta = false;
            Respuesta = await _servicio.DiagnosticarCaret();

            if (Respuesta)
            {
                view.FindByName<Button>("btnDiagnosticarCaret").BackgroundColor = Color.Green;
            }
            else
            {
                view.FindByName<Button>("btnDiagnosticarCaret").BackgroundColor = Color.Red;
            }
        }

        internal void btnPruebaImpresion_Clicked()
        {
            _servicio.ProbarImpresion();
        }
    }
}
