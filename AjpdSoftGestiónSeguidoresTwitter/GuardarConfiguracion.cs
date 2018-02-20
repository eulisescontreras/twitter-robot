using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Windows.Forms;

namespace GestiónSeguidoresTwitter
{
    class GuardarConfiguracion
    {
        //leer valor de configuración del fichero app.config
        public string leerValorConfiguracion (string clave)
        {
            try
            {
                string resultado =
                    ConfigurationManager.AppSettings[clave].ToString();
                return resultado;
            }
            catch
            {
                return "";
            }
        }

        //escribir valor de configuración en fichero app.config
        public void guardarValorConfiguracion(string clave, string valor)
        {
            try
            {
                Configuration ficheroConfXML =
                    ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);

                //eliminamos la clave actual (si existe), si no la eliminamos
                //los valores se irán acumulando separados por coma
                ficheroConfXML.AppSettings.Settings.Remove(clave);

                //asignamos el valor en la clave indicada
                ficheroConfXML.AppSettings.Settings.Add(clave, valor);

                //guardamos los cambios definitivamente en el fichero de configuración
                ficheroConfXML.Save(ConfigurationSaveMode.Modified);
            }
            catch
            {
               /* MessageBox.Show("Error al guardar valor de configuración: " +
                    System.Environment.NewLine + System.Environment.NewLine +
                    ex.GetType().ToString() + System.Environment.NewLine +
                    ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);*/
            }
        }
    }
}
