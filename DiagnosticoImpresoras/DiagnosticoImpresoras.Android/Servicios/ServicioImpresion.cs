using Acr.UserDialogs;
using Android.Bluetooth;
using DiagnosticoImpresoras.Droid.Servicios;
using DiagnosticoImpresoras.Interfaces;
using LinkOS.Plugin;
using LinkOS.Plugin.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(ServicioImpresion))]
namespace DiagnosticoImpresoras.Droid.Servicios
{
    public class ServicioImpresion: IServicioImpresion
    {
        private IConnection connection = null;

        public async Task<bool> DiagnosticarConexion()
        {
            bool Conectado = false;

            //Revisar si ya esta connectado
            if ((connection == null) || (!connection.IsConnected))
            {
                var listaAddress = await ObtenerBluetoothAddress();

                int contador = listaAddress.Count;

                foreach (var addres in listaAddress)
                {
                    try
                    {
                        connection = ConnectionBuilder.Current.Build("BT:" + addres);
                        connection.Open();
                        if (connection.IsConnected)
                        {
                            Conectado = true;
                            await UserDialogs.Instance.AlertAsync("Correcto, Conexion exitosa con la BluetoothAddress: " + addres, "Alerta", "Aceptar");
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        contador = contador - 1;
                        if (contador == 0)
                        {
                            Conectado = false;
                            await UserDialogs.Instance.AlertAsync("Error, Se revisaron " + contador.ToString() + " dispositivos y no se pudo conectar", "Alerta", "Aceptar");
                        }
                    }
                }
            }
            else
            {
                if (connection.IsConnected)
                {
                    Conectado = true;
                }
                else
                {
                    Conectado = false;
                }
            }

            return Conectado;
        }

        public async Task<bool> DiagnosticarLenguaje()
        {
            bool Respuesta = false;

            string GetLanguage = "! U1 getvar \"device.languages\"\r\n\r\n";
            byte[] ResponseGetLanguage = connection.SendAndWaitForResponse(Encoding.UTF8.GetBytes(GetLanguage), 500, 500);
            string RespuestaLenguaje = Encoding.UTF8.GetString(ResponseGetLanguage);
            if (RespuestaLenguaje.Contains("zpl"))
            {
                Respuesta = true;
                await UserDialogs.Instance.AlertAsync("Correcto, El lenguaje de la impresora es: " + RespuestaLenguaje, "Información", "Aceptar");
            }
            else
            {
                Respuesta = false;

                if (RespuestaLenguaje.Contains("?"))
                {                    
                    await UserDialogs.Instance.AlertAsync("Error, La impresora no soporta el comando para cambio de lenguaje, dado que devuelve el valor de: " + RespuestaLenguaje, "Información", "Aceptar");
                }
                else
                {
                    await UserDialogs.Instance.AlertAsync("Error, El lenguaje de la impresora es: " + RespuestaLenguaje + ", Se intentara configurar al lenguaje adecuado", "Alerta", "Aceptar");

                    string SetLanguage = "! U1 setvar \"device.languages\" \"zpl\"\r\n\r\n! U1 getvar \"device.languages\"\r\n\r\n";

                    byte[] ResponseSetLanguage = connection.SendAndWaitForResponse(Encoding.UTF8.GetBytes(SetLanguage), 500, 500);

                    string RespuestaSetLanguaje = Encoding.UTF8.GetString(ResponseSetLanguage);

                    if (RespuestaSetLanguaje.Contains("zpl"))
                    {
                        Respuesta = true;
                        await UserDialogs.Instance.AlertAsync("Correcto, se configuro la impresora  al lenguaje: " + RespuestaLenguaje, "Información", "Aceptar");
                    }
                    else
                    {
                        Respuesta = false;
                        await UserDialogs.Instance.AlertAsync("Error, no se pudo configurar el lenguaje, dado que devuelve el valor de: " + RespuestaLenguaje, "Información", "Aceptar");
                    }
                }
                
            }

            return Respuesta;
        }

        public async Task<bool> DiagnosticarEstado()
        {
            bool Respuesta = false;
            IZebraPrinter printer = ZebraPrinterFactory.Current.GetInstance(PrinterLanguage.ZPL, connection);
            IPrinterStatus status = printer.CurrentStatus;

            if (status.IsReadyToPrint)
            {
                Respuesta = true;
                await UserDialogs.Instance.AlertAsync("Correcto, el estado de la impresora es: Listo para Imprimir", "Información", "Aceptar");
            }
            else
            {
                Respuesta = false;
                await UserDialogs.Instance.AlertAsync("Error, el estado de la impresora no es: Listo para Imprimir", "Información", "Aceptar");
            }

            return Respuesta;
        }

        public async Task<bool> DiagnosticarCaret()
        {
            bool Respuesta = false;

            string GetCaret = "! U1 getvar \"zpl.caret\"\r\n\r\n";
            byte[] ResponseGetCaret = connection.SendAndWaitForResponse(Encoding.UTF8.GetBytes(GetCaret), 500, 500);
            string RespuestaCaret = Encoding.UTF8.GetString(ResponseGetCaret);
            if (RespuestaCaret.Contains("^"))
            {
                Respuesta = true;
                await UserDialogs.Instance.AlertAsync("Correcto, el identificador de codigos es: " + RespuestaCaret, "Información", "Aceptar");
            }
            else
            {
                Respuesta = false;

                if (RespuestaCaret.Contains("?"))
                {
                    await UserDialogs.Instance.AlertAsync("Error, La impresora no soporta el comando para cambio de identificador de codigos, dado que devuelve el valor de: " + RespuestaCaret, "Información", "Aceptar");
                }
                else
                {
                    await UserDialogs.Instance.AlertAsync("Error, el identificador de codigos es: " + RespuestaCaret + " Se intentara configurar el identificador de codigos adecuado", "Alerta", "Aceptar");

                    string SetCaret = "! U1 setvar \"zpl.caret\" \"^\"\r\n\r\n! U1 getvar \"zpl.caret\"\r\n\r\n";

                    byte[] ResponseSetCaret = connection.SendAndWaitForResponse(Encoding.UTF8.GetBytes(SetCaret), 500, 500);

                    string RespuestaSetCaret = Encoding.UTF8.GetString(ResponseSetCaret);

                    if (RespuestaSetCaret.Contains("^"))
                    {
                        Respuesta = true;
                        await UserDialogs.Instance.AlertAsync("Correcto, se configuro el identificador de codigos a: " + RespuestaCaret, "Información", "Aceptar");
                    }
                    else
                    {
                        Respuesta = false;
                        await UserDialogs.Instance.AlertAsync("Error, no se pudo configurar el identificador de codigos, dado que devuelve el valor de: " + RespuestaCaret, "Información", "Aceptar");
                    }
                }

            }

            return Respuesta;
        }

        public async void ProbarImpresion()
        {
            if (await DiagnosticarConexion())
            {
                if(await DiagnosticarLenguaje())
                {
                    if(await DiagnosticarEstado())
                    {
                        List<string> ListaLineas = TextoDePrueba();

                        try
                        {
                            foreach (var item in ListaLineas)
                            {
                                string texto = Conversion(item);
                                string zpl = "^XA^LL48^POI^FO0,0^ACN,18,10^FH_^FD" + texto + "^FS^XZ";
                                connection.Write(Encoding.UTF8.GetBytes(zpl));
                            }
                        }                                                
                        catch (Exception e)
                        {
                            await UserDialogs.Instance.AlertAsync("Se dio la siguiente excepción: " + e.Message + ".", "Excepción", "Aceptar");
                        }
                    }                    
                }                
            }
        }

        private List<string> TextoDePrueba()
        {
            List<string> Lineas = new List<string>();

            Lineas.Add(Environment.NewLine);
            Lineas.Add(Environment.NewLine);
            Lineas.Add("SISTEMA CHARGING ON LINE - PRUEBA IMPRESIÓN");
            Lineas.Add(Environment.NewLine);
            Lineas.Add(Environment.NewLine);
            Lineas.Add("000000000000000000000000000000000000000000000000");
            Lineas.Add("111111111111111111111111111111111111111111111111");
            Lineas.Add("222222222222222222222222222222222222222222222222");
            Lineas.Add("333333333333333333333333333333333333333333333333");
            Lineas.Add("444444444444444444444444444444444444444444444444");
            Lineas.Add("555555555555555555555555555555555555555555555555");
            Lineas.Add("666666666666666666666666666666666666666666666666");
            Lineas.Add("777777777777777777777777777777777777777777777777");
            Lineas.Add("888888888888888888888888888888888888888888888888");
            Lineas.Add("999999999999999999999999999999999999999999999999");
            Lineas.Add("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            Lineas.Add("BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB");
            Lineas.Add("CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC");
            Lineas.Add("DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD");
            Lineas.Add("EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE");
            Lineas.Add("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF");
            Lineas.Add("GGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGG");
            Lineas.Add("HHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH");
            Lineas.Add("IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII");
            Lineas.Add("JJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJ");
            Lineas.Add("KKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKK");
            Lineas.Add("LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL");
            Lineas.Add("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM");
            Lineas.Add("NNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNN");
            Lineas.Add("ÑÑÑÑÑÑÑÑÑÑÑÑÑÑÑÑÑÑÑÑÑÑÑÑÑÑÑÑÑÑÑÑÑÑÑÑÑÑÑÑÑÑÑÑÑÑÑÑ");
            Lineas.Add("OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO");
            Lineas.Add("PPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPP");
            Lineas.Add("QQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQ");
            Lineas.Add("RRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRR");
            Lineas.Add("SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS");
            Lineas.Add("TTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTT");
            Lineas.Add("UUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUU");
            Lineas.Add("VVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV");
            Lineas.Add("WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW");
            Lineas.Add("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
            Lineas.Add("YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY");
            Lineas.Add("ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ");
            Lineas.Add(Environment.NewLine);
            Lineas.Add(Environment.NewLine);

            return Lineas;
        }

        private string Conversion(string Texto)
        {
            string newText = "";
            char[] charArray = Texto.ToCharArray();
            foreach (char c in charArray)
            {
                switch (c)
                {
                    case 'Ñ':
                        newText += "_a5";
                        break;
                    case 'ñ':
                        newText += "_a4";
                        break;
                    case 'Á':
                        newText += "_b5";
                        break;
                    case 'á':
                        newText += "_a0";
                        break;
                    case 'É':
                        newText += "_90";
                        break;
                    case 'é':
                        newText += "_82";
                        break;
                    case 'Í':
                        newText += "_d6";
                        break;
                    case 'í':
                        newText += "_a1";
                        break;
                    case 'Ó':
                        newText += "_e0";
                        break;
                    case 'ó':
                        newText += "_a2";
                        break;
                    case 'Ú':
                        newText += "_e9";
                        break;
                    case 'ú':
                        newText += "_a3";
                        break;
                    default:
                        newText += c;
                        break;
                }
            }
            return newText;
        }

        private async Task<List<string>> ObtenerBluetoothAddress()
        {
            List<string> Addresses = new List<string>();

            //1) Obtener el bluethoot adapter
            BluetoothAdapter mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            if (mBluetoothAdapter == null)
            {
                // Device does not support Bluetooth
                await UserDialogs.Instance.AlertAsync("El dispositivo no soporta Bluetooth", "Alerta", "Aceptar");
            }
            //2)Habilita Bluetooth
            if (!mBluetoothAdapter.IsEnabled)
            {
                //no esta habilitado asi que hay que habilitarlo
                await UserDialogs.Instance.AlertAsync("El dispositivo tiene deshabilitado el Bluetooth", "Alerta", "Aceptar");
            }
            //3) COnsultar dispositivos sincronizados
            ICollection<BluetoothDevice> pairedDevices = mBluetoothAdapter.BondedDevices;
            // If there are paired devices
            if (pairedDevices.Count > 0)
            {
                // Loop through paired devices
                foreach (BluetoothDevice device in pairedDevices)
                {
                    string parcial = device.Address;
                    parcial = parcial.Replace(":", "");
                    Addresses.Add(parcial);
                }
            }
            else
            {
                await UserDialogs.Instance.AlertAsync("No hay impresora conectada", "Alerta", "Aceptar");
            }

            return Addresses;
        }
    }
}