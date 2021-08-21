using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagnosticoImpresoras.Interfaces
{
    public interface IServicioImpresion
    {
        Task<bool> DiagnosticarConexion();
        Task<bool> DiagnosticarLenguaje();
        Task<bool> DiagnosticarEstado();
        Task<bool> DiagnosticarCaret();
        void ProbarImpresion();
    }
}
