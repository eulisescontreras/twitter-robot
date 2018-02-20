using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GestiónSeguidoresTwitter
{
    public partial class formTwitter : Form
    {
        Dictionary<int,TareasTwitter> tareasTwitter = null;
        public static int i = 0;
        public BDSQLite bdSQLite = new BDSQLite();
        Int32 numMensajesDirectosSesion = 0;
        int id;
        int idCuenta;

        private void cerrarAplicacion()
        {
            Application.Exit();
        }

        public formTwitter()
        {
            InitializeComponent();
        }

        private void btEliminarMensajeDirecto_Click(object sender, EventArgs e)
        {
            try
            {
                long idMensaje;
                idMensaje = Convert.ToInt64(lsMensajes.Items[lsMensajes.FocusedItem.Index].Text);

                if (MessageBox.Show("¿Está seguro que desea eliminar el mensaje directo [" +
                    Convert.ToString(idMensaje) + "]?", "Eliminar mensaje directo",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                    System.Windows.Forms.DialogResult.Yes)
                {
                    string resultado = "";
                    tareasTwitter[0].eliminarMensajeDirecto(idMensaje, ref resultado);
                    txtLog.Text = resultado + Environment.NewLine + txtLog.Text;
                }
            }
            catch (Exception error)
            {
                txtLog.Text = System.DateTime.Now + " " +
                    "Error al eliminar mensaje directo: " + error.Message +
                    Environment.NewLine + txtLog.Text;
            }
        }

        private void btResponderMensajeDirecto_Click(object sender, EventArgs e)
        {
            try
            {
                long idUsuario;
                idUsuario =
                    Convert.ToInt64(lsMensajes.Items[lsMensajes.FocusedItem.Index].SubItems[1].Text);

                string resultado = "";
                string mensaje = "";
                InputBox.solicitarTexto("Mensaje",
                    "Introduzca mensaje directo a enviar:", ref mensaje);
                tareasTwitter[i].enviarMensajeDirectoTwitter(mensaje, idUsuario, ref resultado);
                txtLog.Text = resultado + Environment.NewLine + txtLog.Text;
            }
            catch (Exception error)
            {
                txtLog.Text = System.DateTime.Now + " " +
                    "Error al responder mensaje directo: " + error.Message +
                    Environment.NewLine + txtLog.Text;
            }
        }

        private void btConectarTwitter_Click(object sender, EventArgs e)
        {
            string resultado = "";
            if (tareasTwitter[i].obtenerTokenTwitter(ref resultado))
            {
                btObtenerDatosPerfil.Enabled = true;
                btPerfilVisitarURL.Enabled = true;
                tabListadeTweetsAutomatizados.Enabled = true;
                btObtenerDatosPerfil.Text = "Obtener datos del perfil @" +
                    txtUsuarioAccesoTwitter.Text;

            }
            else
            {
                btObtenerDatosPerfil.Enabled = false;
                btPerfilVisitarURL.Enabled = false;
                tabListadeTweetsAutomatizados.Enabled = false;
            }
            txtLog.Text = resultado;                
        }

        private void btObtenerSeguidores_Click(object sender, EventArgs e)
        {
            //preparamos el ListView
            lsSeguidores.Clear();
            lsSeguidores.View = View.Details;
            lsSeguidores.GridLines = true;
            lsSeguidores.FullRowSelect = true;

            lsSeguidores.Columns.Add("ID", 70);
            lsSeguidores.Columns.Add("Nick", 80);
            lsSeguidores.Columns.Add("Nombre", 95);
            lsSeguidores.Columns.Add("Lo sigo", 40);
            lsSeguidores.Columns.Add("Seguidores", 60);
            lsSeguidores.Columns.Add("Siguiendo", 60);
            lsSeguidores.Columns.Add("Fecha alta", 60);
            lsSeguidores.Columns.Add("Descripción", 100);
            lsSeguidores.Columns.Add("URL Perfil", 60);
            lsSeguidores.Columns.Add("Ubicación", 40);
            lsSeguidores.Columns.Add("Idioma", 40);

            //Añadimos los elementos (filas) al ListView
            string[] elementosFila = new string[11];
            ListViewItem elementoListView;
            
            string resultado = "";
            List<Seguidor> seguidores = new List<Seguidor>();
            seguidores = tareasTwitter[i].obtenerUltimos20Seguidores(ref resultado);

            //Añadimos los elementos (filas) al ListView 
            //con los seguidores twitter obtenidos            
            foreach (Seguidor seguidor in seguidores)
            {
                elementosFila[0] = Convert.ToString(seguidor.perfilUsuario.id);
                elementosFila[1] = Convert.ToString(seguidor.perfilUsuario.nick);
                elementosFila[2] = seguidor.perfilUsuario.nombre;
                elementosFila[3] = seguidor.meSigue;
                elementosFila[4] = Convert.ToString(seguidor.perfilUsuario.seguidores);
                elementosFila[5] = Convert.ToString(seguidor.perfilUsuario.siguiendo);
                elementosFila[6] = Convert.ToString(seguidor.perfilUsuario.fechaAlta);
                elementosFila[7] = seguidor.perfilUsuario.descripcion;
                elementosFila[8] = seguidor.perfilUsuario.urlPerfil;
                elementosFila[9] = seguidor.perfilUsuario.ubicacion;
                elementosFila[10] = seguidor.perfilUsuario.idioma;
                elementoListView = new ListViewItem(elementosFila);
                lsSeguidores.Items.Add(elementoListView);
            }
            txtLog.Text = resultado + Environment.NewLine + txtLog.Text;
        }


        private void btVisitarPerfilSeguidor_Click(object sender, EventArgs e)
        {
            string urlPerfilTwitter = "";

            try
            {
            urlPerfilTwitter =
                lsSeguidores.Items[lsSeguidores.FocusedItem.Index].SubItems[8].Text;
            System.Diagnostics.Process.Start(urlPerfilTwitter);
            txtLog.Text = System.DateTime.Now + " " +
                "Visitado perfil [" + urlPerfilTwitter + "]" +
                Environment.NewLine + txtLog.Text;
            }
            catch (Exception error)
            {
                txtLog.Text = System.DateTime.Now + " " +
                    "Error al visitar perfil Twitter [" +
                    urlPerfilTwitter + "]: " + error.Message +
                    Environment.NewLine + txtLog.Text;
            }
        }


        private void btDejardeSeguir_Click(object sender, EventArgs e)
        {
            try
            {
                string seguidorNick;
                seguidorNick =
                    lsSeguidores.Items[lsSeguidores.FocusedItem.Index].SubItems[1].Text;
                string resultado = "";
                tareasTwitter[i].dejarDeSeguirAmigo(seguidorNick, ref resultado);
                txtLog.Text = resultado + Environment.NewLine + txtLog.Text;
            }
            catch (Exception error)
            {
                txtLog.Text = System.DateTime.Now + " " +
                    "Error al dejar de seguir a seguidor: " + error.Message +
                    Environment.NewLine + txtLog.Text;
            }
        }

        private void btSeguirSeguidor_Click(object sender, EventArgs e)
        {
            try
            {
                string seguidorNick;
                seguidorNick =
                    lsSeguidores.Items[lsSeguidores.FocusedItem.Index].SubItems[1].Text;
                string resultado = "";
                tareasTwitter[i].seguirUsuario(seguidorNick, ref resultado);
                txtLog.Text = resultado + Environment.NewLine + txtLog.Text;
            }
            catch (Exception error)
            {
                txtLog.Text = System.DateTime.Now + " " +
                    "Error al seguir a seguidor: " + error.Message +
                    Environment.NewLine + txtLog.Text;
            }
        }

        private void btObtenerUltimosAmigos_Click(object sender, EventArgs e)
        {
            //preparamos el ListView
            lsAmigos.Clear();
            lsAmigos.View = View.Details;
            lsAmigos.GridLines = true;
            lsAmigos.FullRowSelect = true;

            lsAmigos.Columns.Add("ID", 70);
            lsAmigos.Columns.Add("Nick", 80);
            lsAmigos.Columns.Add("Nombre", 95);
            lsAmigos.Columns.Add("Seguidores", 60);
            lsAmigos.Columns.Add("Siguiendo", 60);
            lsAmigos.Columns.Add("Fecha alta", 60);
            lsAmigos.Columns.Add("Descripción", 100);
            lsAmigos.Columns.Add("URL Perfil", 60);
            lsAmigos.Columns.Add("Ubicación", 40);
            lsAmigos.Columns.Add("Idioma", 40);

            //Añadimos los elementos (filas) al ListView
            string[] elementosFila = new string[10];
            ListViewItem elementoListView;

            string resultado = "";
            List<Amigo> amigos = new List<Amigo>();
            amigos = tareasTwitter[i].obtenerUltimos20Amigos(ref resultado);

            //Añadimos los elementos (filas) al ListView 
            //con los seguidores twitter obtenidos            
            foreach (Amigo amigo in amigos)
            {
                elementosFila[0] = Convert.ToString(amigo.perfilUsuario.id);
                elementosFila[1] = Convert.ToString(amigo.perfilUsuario.nick);
                elementosFila[2] = amigo.perfilUsuario.nombre;
                elementosFila[3] = Convert.ToString(amigo.perfilUsuario.seguidores);
                elementosFila[4] = Convert.ToString(amigo.perfilUsuario.siguiendo);
                elementosFila[5] = Convert.ToString(amigo.perfilUsuario.fechaAlta);
                elementosFila[6] = amigo.perfilUsuario.descripcion;
                elementosFila[7] = amigo.perfilUsuario.urlPerfil;
                elementosFila[8] = amigo.perfilUsuario.ubicacion;
                elementosFila[9] = amigo.perfilUsuario.idioma;
                elementoListView = new ListViewItem(elementosFila);
                lsAmigos.Items.Add(elementoListView);
            }
            txtLog.Text = resultado + Environment.NewLine + txtLog.Text;
        }

        private void btDejarSeguirAmigo_Click(object sender, EventArgs e)
        {
            try
            {
                string seguidorNick;
                seguidorNick =
                    lsAmigos.Items[lsAmigos.FocusedItem.Index].SubItems[1].Text;
                string resultado = "";
                tareasTwitter[i].dejarDeSeguirAmigo(seguidorNick, ref resultado);
                txtLog.Text = resultado + Environment.NewLine + txtLog.Text;
            }
            catch (Exception error)
            {
                txtLog.Text = System.DateTime.Now + " " +
                    "Error al dejar de seguir a amigo: " + error.Message +
                    Environment.NewLine + txtLog.Text;
            }
        }

        private void btSeguirAmigo_Click(object sender, EventArgs e)
        {
            try
            {
                string seguidorNick;
                seguidorNick =
                    lsAmigos.Items[lsAmigos.FocusedItem.Index].SubItems[1].Text;
                string resultado = "";
                tareasTwitter[i].seguirUsuario(seguidorNick, ref resultado);
                txtLog.Text = resultado + Environment.NewLine + txtLog.Text;
            }
            catch (Exception error)
            {
                txtLog.Text = System.DateTime.Now + " " +
                    "Error al seguir a amigo: " + error.Message +
                    Environment.NewLine + txtLog.Text;
            }
        }

        private void btEnviarTweet_Click(object sender, EventArgs e)
        {
            string resultado = "";
            try
            {

                tareasTwitter[i].enviarTweet(txtTweet.Text, this);
                resultado = System.DateTime.Now + " " +
                    "Tweet enviado correctamente [" + txtTweet.Text + "]";
            }
            catch (Exception error)
            {
                resultado = System.DateTime.Now + " " +
                    "Error al enviar tweet: " + error.Message;
            }
            txtLog.Text = resultado + Environment.NewLine + txtLog.Text;
        }


        private void btObtenerUltimosMensajesDirectos_Click(object sender, EventArgs e)
        {
            //establecemos las propiedades del ListView
            lsMensajes.Clear();
            lsMensajes.View = View.Details;
            lsMensajes.GridLines = true;
            lsMensajes.FullRowSelect = true;
            
            //añadimos las columnas al ListView
            lsMensajes.Columns.Add("ID", 90);
            lsMensajes.Columns.Add("ID Usuario", 70);
            lsMensajes.Columns.Add("Nick", 70);
            lsMensajes.Columns.Add("Usuario", 90);
            lsMensajes.Columns.Add("Mensaje", 600);
            lsMensajes.Columns.Add("Fecha", 90);
            string[] elementosFila = new string[6];
            ListViewItem elementoListView;

            string resultado = "";
            PerfilUsuario datosPerfilUsuario = new PerfilUsuario();
            tareasTwitter[i].obtenerDatosPerfilPIN(txtUsuarioAccesoTwitter.Text,
                ref datosPerfilUsuario, ref resultado);
            List<MensajeDirecto> mensajes = new List<MensajeDirecto>();
            mensajes = tareasTwitter[i].obtenerUltimos20MensajesDirectos(ref resultado, datosPerfilUsuario.id);

            //Añadimos los elementos (filas) al ListView con los mensajes twitter obtenidos            
            if (mensajes != null)
            { 
                foreach (MensajeDirecto mensaje in mensajes)
                {
                    elementosFila[0] = Convert.ToString(mensaje.id);
                    elementosFila[1] = Convert.ToString(mensaje.idUsuario);
                    elementosFila[2] = mensaje.nick;
                    elementosFila[3] = mensaje.usuario;
                    elementosFila[4] = mensaje.mensaje;
                    elementosFila[5] = Convert.ToString(mensaje.fecha);
                    elementoListView = new ListViewItem(elementosFila);
                    lsMensajes.Items.Add(elementoListView);
                }
            }
            txtLog.Text = resultado +
                Environment.NewLine + txtLog.Text;
        }

        private void btObtenerUltimosTweetsTimeLine_Click(object sender, EventArgs e)
        {
            //Establecemos las propiedades del ListView
            lsTweets.Clear();
            lsTweets.View = View.Details;
            lsTweets.GridLines = true;
            lsTweets.FullRowSelect = true;
            //Añadimos las columnas al ListView
            lsTweets.Columns.Add("ID", 90);
            lsTweets.Columns.Add("ID Usuario", 70);
            lsTweets.Columns.Add("Usuario", 70);
            lsTweets.Columns.Add("Fecha", 90);
            lsTweets.Columns.Add("Tweet", 500);
            lsTweets.Columns.Add("Nº retweets", 90);
            //Añadimos los elementos (filas) al ListView
            string[] elementosFila = new string[6];
            ListViewItem elementoListView;

            string resultado = "";
            List<DatosTweet> tweets = new List<DatosTweet>();
            tweets = tareasTwitter[i].obtener20UltimosTweet("HomeTimeLine", ref resultado);
            foreach (DatosTweet tweet in tweets)
            {
                //Añadimos una primera fila al ListView
                elementosFila[0] = Convert.ToString(tweet.id);
                elementosFila[1] = Convert.ToString(tweet.idUsuario);
                elementosFila[2] = tweet.usuario;
                elementosFila[3] = Convert.ToString(tweet.fecha);
                elementosFila[4] = tweet.tweet;
                elementosFila[5] = Convert.ToString(tweet.NumeroRetweet);
                elementoListView = new ListViewItem(elementosFila);
                lsTweets.Items.Add(elementoListView);
            }
            txtLog.Text = resultado + Environment.NewLine + txtLog.Text;
        }

        private void btObtenerUltimosTweetsMios_Click(object sender, EventArgs e)
        {
            //Establecemos las propiedades del ListView
            lsTweetsMios.Clear();
            lsTweetsMios.View = View.Details;
            lsTweetsMios.GridLines = true;
            lsTweetsMios.FullRowSelect = true;
            //Añadimos las columnas al ListView
            lsTweetsMios.Columns.Add("ID", 90);
            lsTweetsMios.Columns.Add("ID Usuario", 70);
            lsTweetsMios.Columns.Add("Usuario", 70);
            lsTweetsMios.Columns.Add("Fecha", 90);
            lsTweetsMios.Columns.Add("Tweet", 500);
            lsTweetsMios.Columns.Add("Nº retweets", 90);
            //Añadimos los elementos (filas) al ListView
            string[] elementosFila = new string[6];
            ListViewItem elementoListView;

            string resultado = "";
            List<DatosTweet> tweets = new List<DatosTweet>();
            tweets = tareasTwitter[i].obtener20UltimosTweet("UserTimeLine", ref resultado);
            foreach (DatosTweet tweet in tweets)
            {
                //Añadimos una primera fila al ListView
                elementosFila[0] = Convert.ToString(tweet.id);
                elementosFila[1] = Convert.ToString(tweet.idUsuario);
                elementosFila[2] = tweet.usuario;
                elementosFila[3] = Convert.ToString(tweet.fecha);
                elementosFila[4] = tweet.tweet;
                elementosFila[5] = Convert.ToString(tweet.NumeroRetweet);
                elementoListView = new ListViewItem(elementosFila);
                lsTweetsMios.Items.Add(elementoListView);
            }
            txtLog.Text = resultado + Environment.NewLine + txtLog.Text;
        }

        private void btObtenerTweetsMenciones_Click(object sender, EventArgs e)
        {
            //Establecemos las propiedades del ListView
            lsTweetMenciones.Clear();
            lsTweetMenciones.View = View.Details;
            lsTweetMenciones.GridLines = true;
            lsTweetMenciones.FullRowSelect = true;
            //Añadimos las columnas al ListView
            lsTweetMenciones.Columns.Add("ID", 90);
            lsTweetMenciones.Columns.Add("ID Usuario", 70);
            lsTweetMenciones.Columns.Add("Usuario", 70);
            lsTweetMenciones.Columns.Add("Fecha", 90);
            lsTweetMenciones.Columns.Add("Tweet", 500);
            lsTweetMenciones.Columns.Add("Nº retweets", 90);
            //Añadimos los elementos (filas) al ListView
            string[] elementosFila = new string[6];
            ListViewItem elementoListView;

            string resultado = "";
            List<DatosTweet> tweets = new List<DatosTweet>();
            tweets = tareasTwitter[i].obtener20UltimosTweet("Menciones", ref resultado);
            foreach (DatosTweet tweet in tweets)
            {
                //Añadimos una primera fila al ListView
                elementosFila[0] = Convert.ToString(tweet.id);
                elementosFila[1] = Convert.ToString(tweet.idUsuario);
                elementosFila[2] = tweet.usuario;
                elementosFila[3] = Convert.ToString(tweet.fecha);
                elementosFila[4] = tweet.tweet;
                elementosFila[5] = Convert.ToString(tweet.NumeroRetweet);
                elementoListView = new ListViewItem(elementosFila);
                lsTweetMenciones.Items.Add(elementoListView);
            }
            txtLog.Text = resultado + Environment.NewLine + txtLog.Text;
        }

        private void btObtenerTweetsFavoritos_Click(object sender, EventArgs e)
        {
            //Establecemos las propiedades del ListView
            lsTweetsFavoritos.Clear();
            lsTweetsFavoritos.View = View.Details;
            lsTweetsFavoritos.GridLines = true;
            lsTweetsFavoritos.FullRowSelect = true;
            //Añadimos las columnas al ListView
            lsTweetsFavoritos.Columns.Add("ID", 90);
            lsTweetsFavoritos.Columns.Add("ID Usuario", 70);
            lsTweetsFavoritos.Columns.Add("Usuario", 70);
            lsTweetsFavoritos.Columns.Add("Fecha", 90);
            lsTweetsFavoritos.Columns.Add("Tweet", 500);
            lsTweetsFavoritos.Columns.Add("Nº retweets", 90);
            //Añadimos los elementos (filas) al ListView
            string[] elementosFila = new string[6];
            ListViewItem elementoListView;

            string resultado = "";
            List<DatosTweet> tweets = new List<DatosTweet>();
            tweets = tareasTwitter[i].obtener20UltimosTweet("Favoritos", ref resultado);
            foreach (DatosTweet tweet in tweets)
            {
                //Añadimos una primera fila al ListView
                elementosFila[0] = Convert.ToString(tweet.id);
                elementosFila[1] = Convert.ToString(tweet.idUsuario);
                elementosFila[2] = tweet.usuario;
                elementosFila[3] = Convert.ToString(tweet.fecha);
                elementosFila[4] = tweet.tweet;
                elementosFila[5] = Convert.ToString(tweet.NumeroRetweet);
                elementoListView = new ListViewItem(elementosFila);
                lsTweetsFavoritos.Items.Add(elementoListView);
            }
            txtLog.Text = resultado + Environment.NewLine + txtLog.Text;
        }



        private void btObtenerTweetsMisRetwees_Click(object sender, EventArgs e)
        {
            //Establecemos las propiedades del ListView
            lsTweetsMisRetweets.Clear();
            lsTweetsMisRetweets.View = View.Details;
            lsTweetsMisRetweets.GridLines = true;
            lsTweetsMisRetweets.FullRowSelect = true;
            //Añadimos las columnas al ListView
            lsTweetsMisRetweets.Columns.Add("ID", 90);
            lsTweetsMisRetweets.Columns.Add("ID Usuario", 70);
            lsTweetsMisRetweets.Columns.Add("Usuario", 70);
            lsTweetsMisRetweets.Columns.Add("Fecha", 90);
            lsTweetsMisRetweets.Columns.Add("Tweet", 500);
            lsTweetsMisRetweets.Columns.Add("Nº retweets", 90);
            //Añadimos los elementos (filas) al ListView
            string[] elementosFila = new string[6];
            ListViewItem elementoListView;

            string resultado = "";
            List<DatosTweet> tweets = new List<DatosTweet>();
            tweets = tareasTwitter[i].obtener20UltimosTweet("Retweets", ref resultado);
            foreach (DatosTweet tweet in tweets)
            {
                //Añadimos una primera fila al ListView
                elementosFila[0] = Convert.ToString(tweet.id);
                elementosFila[1] = Convert.ToString(tweet.idUsuario);
                elementosFila[2] = tweet.usuario;
                elementosFila[3] = Convert.ToString(tweet.fecha);
                elementosFila[4] = tweet.tweet;
                elementosFila[5] = Convert.ToString(tweet.NumeroRetweet);
                elementoListView = new ListViewItem(elementosFila);
                lsTweetsMisRetweets.Items.Add(elementoListView);
            }
            txtLog.Text = resultado + Environment.NewLine + txtLog.Text;
        }


        private void temporizador_Tick(object sender, EventArgs e)
        {
            string resultado = "";

            try
            {
                //obtenemos últimos 20 seguidores
                List<Seguidor> seguidores = new List<Seguidor>();
                seguidores = tareasTwitter[i].obtenerUltimos20Seguidores(ref resultado);
                txtLog.Text = resultado +
                    Environment.NewLine + txtLog.Text;

                //recorremos seguidor a seguidor para tareas automáticas
                foreach (Seguidor seguidor in seguidores)
                {
                    //si el seguidor es nuevo lo añadimos a la BD SQLite
                    if (!bdSQLite.existeSeguidor(seguidor.perfilUsuario.id, ref resultado))
                    {
                        txtLog.Text = resultado +
                            Environment.NewLine + txtLog.Text;
                        bdSQLite.insertarSeguidor(seguidor.perfilUsuario.id, ref resultado);
                        txtLog.Text = resultado +
                            Environment.NewLine + txtLog.Text;
                       
                        //Es un nuevo seguidor, le enviamos automáticamente un mensaje directo
                        if (txtTareaAutoMensajeDirectoSeguidores.Text != "" &
                            opTareaAutoEnviarMensajeDirectoSeguidores.Checked)
                        {
                            tareasTwitter[i].enviarMensajeDirectoTwitter(
                                txtTareaAutoMensajeDirectoSeguidores.Text,
                                seguidor.perfilUsuario.id, ref resultado);
                            txtLog.Text = resultado +
                                Environment.NewLine + txtLog.Text;
                            numMensajesDirectosSesion = numMensajesDirectosSesion + 1;
                            bePNumMensajesDirectos.Text = "Nº mensajes directos: " +
                                Convert.ToString(numMensajesDirectosSesion);
                            if (iconizarApp.Visible)
                                iconizarApp.Text = Application.ProductName + " @" +
                                    txtUsuarioAccesoTwitter.Text + " [" +
                                    Convert.ToString(numMensajesDirectosSesion) + "]";
                        }

                        //añadi mención nuevo seguidor
                        if (opTareaAutoMencion.Checked)
                        {
                            if (seguidor.perfilUsuario.nick != "" & txtTareaAutoMencion.Text != "")
                            {
                                string textoMencion = "";
                                textoMencion = txtTareaAutoMencion.Text;
                                textoMencion = textoMencion.Replace("##nick##",
                                    "@" + seguidor.perfilUsuario.nick);
                                try { 
                                    tareasTwitter[i].enviarTweet(textoMencion, this);
                                    txtLog.Text = textoMencion +
                                        Environment.NewLine + txtLog.Text;
                                }
                                catch (Exception error)
                                {
                                    resultado = System.DateTime.Now + " " +
                                        "Error al enviar tweet: " + error.Message;
                                }
                            }
                        }

                        //Es un nuevo seguidor, lo seguimos
                        if (opTareaAutoSeguirSeguidor.Checked)
                        {
                            tareasTwitter[i].seguirUsuario(seguidor.perfilUsuario.nick, ref resultado);
                            txtLog.Text = resultado +
                                Environment.NewLine + txtLog.Text;
                        }
                    }
                    else
                        txtLog.Text = System.DateTime.Now + " " +
                            "El seguidor [" + Convert.ToString(seguidor.perfilUsuario.id) +
                            "] ya existe en la BD" +
                            Environment.NewLine + txtLog.Text;
                }
                
                //Automatizar tweets aleatorios
                if (opTareaAutomatizarTweets.Checked)
                {
                    List<AutomaticMessageModel> tweets = new List<AutomaticMessageModel>();
                    bdSQLite.obtenerTweetsAutomatizados(ref resultado, ref tweets, txtUsuarioAccesoTwitter.Text);
                    tareasTwitter[i].generarTweets(tweets, this);
                    txtLog.Text = resultado +
                            Environment.NewLine + txtLog.Text;
                }
            }
            catch (Exception error)
            {
                txtLog.Text = System.DateTime.Now + " " +
                    "Error al iniciar temporizador: " + error.Message +
                    Environment.NewLine + txtLog.Text;
            }
        }



        private void btCrearBDSQLite_Click(object sender, EventArgs e)
        {
                string resultado = "";
                bdSQLite.crearBDSQLite(txtBDSQLite.Text, 
                    txtBDSQLiteContrasena.Text, ref resultado);
                txtLog.Text = resultado +
                    Environment.NewLine + txtLog.Text;
        }

        private void bSelFicBDSQLite_Click(object sender, EventArgs e)
        {
            dlAbrirFichero.CheckFileExists = false;
            dlAbrirFichero.CheckPathExists = true;
            dlAbrirFichero.Multiselect = false;
            dlAbrirFichero.DefaultExt = "sqlite";
            dlAbrirFichero.FileName = "";
            dlAbrirFichero.Filter = "Archivos SQLite (*.sqlite)|*.sqlite|" +
                "Todos los archivos (*.*)|*.*";
            dlAbrirFichero.Title = "Seleccionar fichero BD SQLite";
            if (dlAbrirFichero.ShowDialog() == DialogResult.OK)
            {
                txtBDSQLite.Text = dlAbrirFichero.FileName;
            }         
        }


        private void btActivarTareasAutomaticas_Click(object sender, EventArgs e)
        {
            try
            {
                temporizador.Enabled = false;
                if (txtTareaAutomaticaIntervalo.Text != "")
                    temporizador.Interval =
                        Convert.ToInt32(txtTareaAutomaticaIntervalo.Text) * 1000;
                else
                    temporizador.Interval = 300000; //si no se especifica intervalo: 5 minutos

                if (System.IO.File.Exists(txtBDSQLite.Text))
                {
                    string resultado = "";
                    if (bdSQLite.conectarBDSQLite(txtBDSQLite.Text,
                        txtBDSQLiteContrasena.Text, ref resultado))
                    {
                        txtLog.Text = resultado + Environment.NewLine + txtLog.Text;
                        temporizador.Enabled = true;
                        btActivarTareasAutomaticas.Enabled = false;
                        btDetenerTareasAutomaticas.Enabled = true;
                        mnuDesactivarTareasAutomaticas.Enabled = true;
                        mnuActivarTareasAutomaticas.Enabled = false;
                    }
                    else
                        txtLog.Text = resultado + Environment.NewLine + txtLog.Text;
                }
                else
                {
                    tabListadeTweetsAutomatizados.SelectedTab = tabConfiguracion;
                    bSelFicBDSQLite.Focus();
                    MessageBox.Show("Para activar las tareas automáticas debe " +
                        "crear la BD SQLite previamente.",
                        "Tareas automáticas Twitter",
                    MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            catch (Exception error)
            {
                txtLog.Text = System.DateTime.Now + " " +
                    "Error al iniciar tareas automáticas: " + error.Message +
                    Environment.NewLine + txtLog.Text;
            }
        }


        private void btObtenerDatosPerfil_Click(object sender, EventArgs e)
        {
            string resultado = "";
            PerfilUsuario datosPerfilUsuario = new PerfilUsuario();

            tareasTwitter[i].obtenerDatosPerfilPIN(txtUsuarioAccesoTwitter.Text,
                ref datosPerfilUsuario, ref resultado);
            txtPerfilNick.Text = datosPerfilUsuario.nick;
            txtPerfilNombre.Text = datosPerfilUsuario.nombre;
            txtPerfilBiografia.Text = datosPerfilUsuario.descripcion;
            txtPerfilSiguiendo.Text = 
                Convert.ToString(datosPerfilUsuario.siguiendo);
            txtPerfilSeguidores.Text =
                Convert.ToString(datosPerfilUsuario.seguidores);
            txtPerfilFechaAlta.Text =
                Convert.ToString(datosPerfilUsuario.fechaAlta);
            txtPerfilUbicacion.Text = datosPerfilUsuario.ubicacion;
                txtPerfilID.Text =
                    Convert.ToString(datosPerfilUsuario.id);
            txtPerfilIdioma.Text = datosPerfilUsuario.idioma;
            txtPerfilURL.Text = datosPerfilUsuario.urlPerfil;
            opPerfilRestringido.Checked = datosPerfilUsuario.protegido;
            opPerfilTraductor.Checked = datosPerfilUsuario.traductor;

            txtLog.Text = resultado + Environment.NewLine + txtLog.Text;
        }

        private void btVisitarPerfilTweetTimeLine_Click(object sender, EventArgs e)
        {
            string urlPerfilTwitter = "";
            string usuario = "";

            try
            {            
                usuario = lsTweets.Items[lsTweets.FocusedItem.Index].SubItems[2].Text;
                string resultado = "";
                PerfilUsuario perfilUsuario = new PerfilUsuario();
                tareasTwitter[i].obtenerDatosPerfilPIN(usuario, ref perfilUsuario, ref resultado);
                txtLog.Text = resultado + Environment.NewLine + txtLog.Text;
                urlPerfilTwitter = perfilUsuario.urlPerfil;
                if (urlPerfilTwitter != null)
                {
                    System.Diagnostics.Process.Start(urlPerfilTwitter);
                    txtLog.Text = System.DateTime.Now + " " +
                        "Visitado perfil [" + urlPerfilTwitter + "]" +
                        Environment.NewLine + txtLog.Text;
                }
                else
                    txtLog.Text = System.DateTime.Now + 
                        " No se ha podido obtener la URL del perfil del usuario";
            }
            catch (Exception error)
            {
                txtLog.Text = System.DateTime.Now + " " +
                    "Error al visitar perfil Twitter [" +
                    urlPerfilTwitter + "]: " + error.Message +
                    Environment.NewLine + txtLog.Text;
            }
        }

        private void btVisitarPefilAmigo_Click(object sender, EventArgs e)
        {
            string urlPerfilTwitter = "";

            try
            {
                
                urlPerfilTwitter =
                    lsAmigos.Items[lsAmigos.FocusedItem.Index].SubItems[7].Text;
                System.Diagnostics.Process.Start(urlPerfilTwitter);
                txtLog.Text = System.DateTime.Now + " " +
                    "Visitado perfil [" + urlPerfilTwitter + "]" +
                    Environment.NewLine + txtLog.Text;
            }
            catch (Exception error)
            {
                txtLog.Text = System.DateTime.Now + " " +
                    "Error al visitar perfil Twitter [" + 
                    urlPerfilTwitter + "]: " + error.Message +
                    Environment.NewLine + txtLog.Text;
            }
        }

        private void btDejarSeguirTimeLine_Click(object sender, EventArgs e)
        {
            try
            {
                string seguidorNick;
                seguidorNick =
                    lsTweets.Items[lsTweets.FocusedItem.Index].SubItems[2].Text;
                string resultado = "";
                tareasTwitter[i].dejarDeSeguirAmigo(seguidorNick, ref resultado);
                txtLog.Text = resultado + Environment.NewLine + txtLog.Text;
            }
            catch (Exception error)
            {
                txtLog.Text = System.DateTime.Now + " " +
                    "Error al dejar de seguir a usuario: " + error.Message +
                    Environment.NewLine + txtLog.Text;
            }
        }

        private void btSeguirTimeLine_Click(object sender, EventArgs e)
        {
            try
            {
                string seguidorNick;
                seguidorNick =
                    lsTweets.Items[lsTweets.FocusedItem.Index].SubItems[2].Text;
                string resultado = "";
                tareasTwitter[i].seguirUsuario(seguidorNick, ref resultado);
                txtLog.Text = resultado + Environment.NewLine + txtLog.Text;
            }
            catch (Exception error)
            {
                txtLog.Text = System.DateTime.Now + " " +
                    "Error al seguir a usuario: " + error.Message +
                    Environment.NewLine + txtLog.Text;
            }
        }

        private void btVisitarPerfilMisTweets_Click(object sender, EventArgs e)
        {
            string urlPerfilTwitter = ""; 
            string usuario = "";

            try
            {
                usuario = lsTweetsMios.Items[lsTweetsMios.FocusedItem.Index].SubItems[2].Text;
                string resultado = "";
                PerfilUsuario perfilUsuario = new PerfilUsuario();
                tareasTwitter[i].obtenerDatosPerfilPIN(usuario, ref perfilUsuario, ref resultado);
                txtLog.Text = resultado + Environment.NewLine + txtLog.Text;

                urlPerfilTwitter = perfilUsuario.urlPerfil;
                if (urlPerfilTwitter != null)
                {
                    System.Diagnostics.Process.Start(urlPerfilTwitter);
                    txtLog.Text = System.DateTime.Now + " " +
                        "Visitado perfil [" + urlPerfilTwitter + "]" +
                        Environment.NewLine + txtLog.Text;
                }
                else
                    txtLog.Text = System.DateTime.Now +
                        " No se ha podido obtener la URL del perfil del usuario";
            }
            catch (Exception error)
            {
                txtLog.Text = System.DateTime.Now + " " +
                    "Error al visitar perfil Twitter [" +
                    urlPerfilTwitter + "]: " + error.Message +
                    Environment.NewLine + txtLog.Text;
            }            
        }

        private void btDejarSeguirMenciones_Click(object sender, EventArgs e)
        {
            try
            {
                string seguidorNick;
                seguidorNick =
                    lsTweetMenciones.Items[lsTweetMenciones.FocusedItem.Index].SubItems[2].Text;
                string resultado = "";
                tareasTwitter[i].dejarDeSeguirAmigo(seguidorNick, ref resultado);
                txtLog.Text = resultado + Environment.NewLine + txtLog.Text;
            }
            catch (Exception error)
            {
                txtLog.Text = System.DateTime.Now + " " +
                    "Error al dejar de seguir a usuario: " + error.Message +
                    Environment.NewLine + txtLog.Text;
            }
        }

        private void btSeguirMenciones_Click(object sender, EventArgs e)
        {
            try
            {
                string seguidorNick;
                seguidorNick =
                    lsTweetMenciones.Items[lsTweetMenciones.FocusedItem.Index].SubItems[2].Text;
                string resultado = "";
                tareasTwitter[i].seguirUsuario(seguidorNick, ref resultado);
                txtLog.Text = resultado + Environment.NewLine + txtLog.Text;
            }
            catch (Exception error)
            {
                txtLog.Text = System.DateTime.Now + " " +
                    "Error al seguir a usuario: " + error.Message +
                    Environment.NewLine + txtLog.Text;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {            
            string urlPerfilTwitter = "";
            string usuario = "";

            try
            {
                usuario = lsTweetMenciones.Items[lsTweetMenciones.FocusedItem.Index].SubItems[2].Text;
                string resultado = "";
                PerfilUsuario perfilUsuario = new PerfilUsuario();
                tareasTwitter[i].obtenerDatosPerfilPIN(usuario, ref perfilUsuario, ref resultado);
                txtLog.Text = resultado + Environment.NewLine + txtLog.Text;
                
                urlPerfilTwitter = perfilUsuario.urlPerfil;
                if (urlPerfilTwitter != null)
                {
                    System.Diagnostics.Process.Start(urlPerfilTwitter);
                    txtLog.Text = System.DateTime.Now + " " +
                        "Visitado perfil [" + urlPerfilTwitter + "]" +
                        Environment.NewLine + txtLog.Text;
                }
                else
                    txtLog.Text = System.DateTime.Now +
                        " No se ha podido obtener la URL del perfil del usuario";
            }
            catch (Exception error)
            {
                txtLog.Text = System.DateTime.Now + " " +
                    "Error al visitar perfil Twitter [" +
                    urlPerfilTwitter + "]: " + error.Message +
                    Environment.NewLine + txtLog.Text;
            }
        }

        private void btDetenerTareasAutomaticas_Click(object sender, EventArgs e)
        {
            temporizador.Enabled = false;
            btActivarTareasAutomaticas.Enabled = true;
            btDetenerTareasAutomaticas.Enabled = false;
            mnuDesactivarTareasAutomaticas.Enabled = false;
            mnuActivarTareasAutomaticas.Enabled = true;
        }

        private void lbEnlaceURL_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.ajpdsoft.com");
        }

        private void formTwitter_FormClosed(object sender, FormClosedEventArgs e)
        {
            GuardarConfiguracion guardarConfig = new GuardarConfiguracion();
            guardarConfig.guardarValorConfiguracion("TA.Intervalo",
                Convert.ToString(txtTareaAutomaticaIntervalo.Value));    
           if (opTareaAutoEnviarMensajeDirectoSeguidores.Checked)
                guardarConfig.guardarValorConfiguracion("TA.Mensaje.Activar", "Sí");
           else
               guardarConfig.guardarValorConfiguracion("TA.Mensaje.Activar", "No");
           guardarConfig.guardarValorConfiguracion("TA.Mensaje.Mensaje",
               txtTareaAutoMensajeDirectoSeguidores.Text);
           if (opTareaAutoMencion.Checked)
               guardarConfig.guardarValorConfiguracion("TA.Mencion.Activar", "Sí");
           else
               guardarConfig.guardarValorConfiguracion("TA.Mencion.Activar", "No");
           guardarConfig.guardarValorConfiguracion("TA.Mencion.Mensaje",
               txtTareaAutoMencion.Text);
           if (opTareaAutoSeguirSeguidor.Checked)
               guardarConfig.guardarValorConfiguracion("TA.Seguir.Activar", "Sí");
           else
               guardarConfig.guardarValorConfiguracion("TA.Seguir.Activar", "No");
           
           guardarConfig.guardarValorConfiguracion("BD.Fichero",
               txtBDSQLite.Text);
           Encriptar encriptar = new Encriptar();
            guardarConfig.guardarValorConfiguracion("BD.Contraseña",
                          encriptar.cifrarTextoAES(txtBDSQLite.Text,
                          "AjpdSoft_Frase_Encriptado", "AjpdSoft_Frase_Encriptado",
                          "MD5", 22, "1234567891234567", 128));
            guardarConfig.guardarValorConfiguracion("Tweet.Tweet",
               txtTweet.Text);
            guardarConfig.guardarValorConfiguracion("Twitter.Usuario",
               txtUsuarioAccesoTwitter.Text);            
        }

        private void formTwitter_Load(object sender, EventArgs e)
        {
            tareasTwitter = new Dictionary<int, TareasTwitter>();
            tareasTwitter.Add(0, new TareasTwitter()); 
            if (System.IO.File.Exists(
                System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                AppDomain.CurrentDomain.FriendlyName + ".config")))
            {

                GuardarConfiguracion guardarConfig = new GuardarConfiguracion();
                string intervalo = "";
                txtUsuarioAccesoTwitter.Text =
                    guardarConfig.leerValorConfiguracion("Twitter.Usuario");
                intervalo = guardarConfig.leerValorConfiguracion("TA.Intervalo");
                if (intervalo != "")
                    txtTareaAutomaticaIntervalo.Value = Convert.ToDecimal(intervalo);
                string op = "";
                op = guardarConfig.leerValorConfiguracion("TA.Mensaje.Activar");
                if (op == "Sí")
                    opTareaAutoEnviarMensajeDirectoSeguidores.Checked = true;
                else
                    opTareaAutoEnviarMensajeDirectoSeguidores.Checked = false;
                txtTareaAutoMensajeDirectoSeguidores.Text =
                    guardarConfig.leerValorConfiguracion("TA.Mensaje.Mensaje");
                op = guardarConfig.leerValorConfiguracion("TA.Mencion.Activar");
                if (op == "Sí")
                    opTareaAutoMencion.Checked = true;
                else
                    opTareaAutoMencion.Checked = false;
                txtTareaAutoMencion.Text =
                    guardarConfig.leerValorConfiguracion("TA.Mencion.Mensaje");
                op = guardarConfig.leerValorConfiguracion("TA.Seguir.Activar");
                if (op == "Sí")
                    opTareaAutoSeguirSeguidor.Checked = true;
                else
                    opTareaAutoSeguirSeguidor.Checked = false;

                txtBDSQLite.Text = guardarConfig.leerValorConfiguracion("BD.Fichero");
                Encriptar encriptar = new Encriptar();
                txtBDSQLite.Text =
                    encriptar.descifrarTextoAES(
                      guardarConfig.leerValorConfiguracion("BD.Contraseña"),
                      "AjpdSoft_Frase_Encriptado", "AjpdSoft_Frase_Encriptado",
                      "MD5", 22, "1234567891234567", 128);
                txtTweet.Text =
                    guardarConfig.leerValorConfiguracion("Tweet.Tweet");
            }
        }

        
        //Buscar un elemento en un ListView
        private bool existeElementoListView(ListView lista, string elementoBuscar)
        {            
            foreach (ListViewItem elementoLista in lista.Items)
            {
                if (elementoLista.Text == elementoBuscar)
                    return true;
            }            
            return false;
        }



        private void btTest_Click(object sender, EventArgs e)
        {
            string resultado = "";
            
            //últimos 5000 amigos
            List<long> listaAmigos = new List<long>();
            listaAmigos = tareasTwitter[0].obtenerUltimos5000Amigos(-1, ref resultado);
            txtLog.Text = resultado + Environment.NewLine + txtLog.Text;
            lsAmigosTodos.Clear();
            lsAmigosTodos.View = View.Details;
            lsAmigosTodos.GridLines = true;
            lsAmigosTodos.FullRowSelect = true;
            //Añadimos los elementos (filas) al ListView
            string[] elementosFila = new string[1];
            ListViewItem elementoListView;
            lsAmigosTodos.Columns.Add("ID", 70);
            foreach (long amigo in listaAmigos)
            {
                elementosFila[0] = Convert.ToString(amigo);
                elementoListView = new ListViewItem(elementosFila);
                lsAmigosTodos.Items.Add(elementoListView);
            }
            lInfoAmigos.Text = "Amigos (siguiendo) [" +
                Convert.ToString(lsAmigosTodos.Items.Count) + "]";

            //últimos 5000 seguidores
            List<long> listaSeguidores = new List<long>();
            listaSeguidores = tareasTwitter[0].obtenerUltimos5000Seguidores(-1, ref resultado);
            txtLog.Text = resultado + Environment.NewLine + txtLog.Text;
            lsSeguidoresTodos.Clear();
            lsSeguidoresTodos.View = View.Details;
            lsSeguidoresTodos.GridLines = true;
            lsSeguidoresTodos.FullRowSelect = true;
            //Añadimos los elementos (filas) al ListView
            string[] elementosFilaSeguidores = new string[1];
            ListViewItem elementoListViewSeguidores;
            lsSeguidoresTodos.Columns.Add("ID", 70);
            foreach (long seguidor in listaSeguidores)
            {
                elementosFilaSeguidores[0] = Convert.ToString(seguidor);
                elementoListViewSeguidores = new ListViewItem(elementosFilaSeguidores);
                lsSeguidoresTodos.Items.Add(elementoListViewSeguidores);
            }
            lInfoSeguidores.Text = "Seguidores [" +
                Convert.ToString(lsSeguidoresTodos.Items.Count) + "]";

            //comparamos cada elemento del listview de amigos con los del listview de seguidores 
            //para mostrar los amigos que no nos siguen
            lsAmigosNoTeSiguen.Clear();
            lsAmigosNoTeSiguen.View = View.Details;
            lsAmigosNoTeSiguen.GridLines = true;
            lsAmigosNoTeSiguen.FullRowSelect = true;
            //Añadimos los elementos (filas) al ListView
            string[] elementosFilaAmigosNoteSiguen = new string[1];
            ListViewItem elementoListViewAmigosNoteSiguen;
            lsAmigosNoTeSiguen.Columns.Add("ID", 70);
            foreach (ListViewItem elementoLista in lsAmigosTodos.Items)
            {
                if (!existeElementoListView(lsSeguidoresTodos, elementoLista.Text))
                {
                    elementosFilaAmigosNoteSiguen[0] = elementoLista.Text;
                    elementoListViewAmigosNoteSiguen = new ListViewItem(elementosFilaAmigosNoteSiguen);
                    lsAmigosNoTeSiguen.Items.Add(elementoListViewAmigosNoteSiguen);                    
                }
            }
            lInfoAmigosNoTeSiguen.Text = "Amigos que no te siguen [" +
                Convert.ToString(lsAmigosNoTeSiguen.Items.Count) + "]";        
        }

        private void btMinimizar_Click(object sender, EventArgs e)
        {
            iconizarApp.Icon = this.Icon;
            iconizarApp.ContextMenuStrip = this.mnuContextual;
            iconizarApp.Text = Application.ProductName + " @" +
                    txtUsuarioAccesoTwitter.Text;
            iconizarApp.Visible = true;
            this.Visible = false;
        }

        private void mnuMostrarAplicacion_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            Activate();
            iconizarApp.Visible = false;
        }

        private void mnuActivarTareasAutomaticas_Click(object sender, EventArgs e)
        {
            btActivarTareasAutomaticas_Click(sender, e);
        }

        private void mnuDesactivarTareasAutomaticas_Click(object sender, EventArgs e)
        {
            btDetenerTareasAutomaticas_Click(sender, e);
        }

        private void formTwitter_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                btMinimizar_Click(sender, e);
                //Tipo de icono a mostrar el el globo informativo (Info, Error, None, Warning)
                iconizarApp.BalloonTipIcon = ToolTipIcon.Info;
                //Título del balón informativo (el nombre de la aplicación)
                iconizarApp.BalloonTipTitle = Application.ProductName + " @" + 
                    txtUsuarioAccesoTwitter.Text;
                //Texto del balón informativo
                iconizarApp.BalloonTipText = "La aplicación ha quedado ocultada " +
                    "en el área de notificación. Para mostrarla haga " +
                    "doble clic sobre el icono";
                //Tiempo que aparecerá hasta ocultarse automáticamente
                iconizarApp.ShowBalloonTip(8);
            }   
        }

        private void iconizarApp_BalloonTipClicked(object sender, EventArgs e)
        {
            mnuMostrarAplicacion_Click(sender, e);
        }

        private void mnuCerrar_Click(object sender, EventArgs e)
        {
            cerrarAplicacion();
        }

        private void txtUsuarioAccesoTwitter_TextChanged(object sender, EventArgs e)
        {
            this.Text = Application.ProductName + " @" + txtUsuarioAccesoTwitter.Text;
        }

        private void txtTweet_TextChanged(object sender, EventArgs e)
        {
            try
            {
                lInfoTweet.Text = "Caracteres restantes " +
                    Convert.ToString(140 - txtTweet.Text.Length);
            }
            catch
            {
            }
        }

        private void txtTareaAutoMencion_TextChanged(object sender, EventArgs e)
        {
            string textoMencion = "";
            textoMencion = txtTareaAutoMencion.Text;
            textoMencion = textoMencion.Replace("##nick##",
                "@" + txtUsuarioAccesoTwitter.Text);
            lMencionResultado.Text = textoMencion;
        }

        private void btDejarSeguirTodos_Click(object sender, EventArgs e)
        {
            if (lsAmigosTodos.Items.Count == 5000 || lsSeguidoresTodos.Items.Count == 5000)
            {
                MessageBox.Show("Este procedimiento sólo funcionará si tiene menos de 5.000 " +
                    "amigos o seguidores. En futuras versiones ampliaremos este límite.",
                    "El proceso no puede realizarse", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (MessageBox.Show("¿Está seguro que desea dejar de seguir a " + 
                    Convert.ToString(lsAmigosNoTeSiguen.Items.Count) + " amigos? Recuerde " +
                    "que Twitter puede penalizar el uso abusivo de esta técnica." + 
                    Environment.NewLine + Environment.NewLine + 
                    "El proceso puede tardar varios minutos en función del número " +
                    "de amigos a dejar de seguir.",  "Dejar de seguir a amigos",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == 
                    System.Windows.Forms.DialogResult.Yes)
                {
                    foreach (ListViewItem elemento in lsAmigosNoTeSiguen.Items)
                    {
                        string resultado = "";
                        long idUsuario = 0;
                        try
                        {
                            if (elemento.Text != "")
                            {
                                idUsuario = Convert.ToInt64(elemento.Text);
                                tareasTwitter[0].dejarDeSeguirAmigo(idUsuario, ref resultado);
                                txtLog.Text = resultado + Environment.NewLine + txtLog.Text;
                            }
                        }
                        catch (Exception error)
                        {
                            txtLog.Text = System.DateTime.Now + " " +
                               "Error al dejar de seguir a [" + Convert.ToString(idUsuario) + "]: " +
                               error.Message + Environment.NewLine + txtLog.Text;
                        }
                    }
                }
            }
        }

        private void btPerfilVisitarURL_Click(object sender, EventArgs e)
        {
            string urlPerfilTwitter = "";

            try
            {
                urlPerfilTwitter =
                    txtPerfilURL.Text;
                System.Diagnostics.Process.Start(urlPerfilTwitter);
                txtLog.Text = System.DateTime.Now + " " +
                    "Visitado perfil [" + urlPerfilTwitter + "]" +
                    Environment.NewLine + txtLog.Text;
            }
            catch (Exception error)
            {
                txtLog.Text = System.DateTime.Now + " " +
                    "Error al visitar perfil Twitter [" +
                    urlPerfilTwitter + "]: " + error.Message +
                    Environment.NewLine + txtLog.Text;
            }
        }

        private void btVisitarPerfilAmigosNoSiguen_Click(object sender, EventArgs e)
        {
            string urlPerfilTwitter = "";
            long usuario = 0;

            try
            {
                usuario = Convert.ToInt64(lsAmigosNoTeSiguen.Items[lsAmigosNoTeSiguen.FocusedItem.Index].SubItems[0].Text);
                
                string resultado = "";
                PerfilUsuario perfilUsuario = new PerfilUsuario();
                tareasTwitter[0].obtenerPerfilUsuario(usuario, ref perfilUsuario, ref resultado);
                txtLog.Text = resultado + Environment.NewLine + txtLog.Text;

                urlPerfilTwitter = perfilUsuario.urlPerfil;
                if (urlPerfilTwitter != null)
                {
                    System.Diagnostics.Process.Start(urlPerfilTwitter);
                    txtLog.Text = System.DateTime.Now + " " +
                        "Visitado perfil [" + urlPerfilTwitter + "]" +
                        Environment.NewLine + txtLog.Text;
                }
                else
                    txtLog.Text = System.DateTime.Now +
                        " No se ha podido obtener la URL del perfil del usuario";
            }
            catch (Exception error)
            {
                txtLog.Text = System.DateTime.Now + " " +
                    "Error al visitar perfil Twitter [" +
                    urlPerfilTwitter + "]: " + error.Message +
                    Environment.NewLine + txtLog.Text;
            }
        }

        private void btProbarInsercionRegistroBD_Click(object sender, EventArgs e)
        {
            string resultado = "";
            string valorID = "";
            InputBox.solicitarTexto("ID Seguidor (número)", "11111111", ref valorID);
            try
            {
                bdSQLite.insertarSeguidor(Convert.ToInt64(valorID), ref resultado);
                MessageBox.Show("Resultado ejecución SQL inserción: " +
                        Environment.NewLine + Environment.NewLine + resultado, 
                        "Insertar seguidor en BD",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Insertar seguidor en BD", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btComprobarExisteSeguidorBD_Click(object sender, EventArgs e)
        {
            string resultado = "";
            string valorID = "";
            InputBox.solicitarTexto("ID Seguidor (número)", "11111111", ref valorID);
            try
            {
                if (bdSQLite.existeSeguidor(Convert.ToInt64(valorID), ref resultado))
                    MessageBox.Show("El ID de seguidor ya existe en la BD." + 
                        Environment.NewLine + Environment.NewLine + resultado,
                        "Existe seguidor",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("El ID de seguidor no existe en la BD." +
                        Environment.NewLine + Environment.NewLine + resultado,
                        "Existe seguidor",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btConectarBD_Click(object sender, EventArgs e)
        {
            string resultado = "";
            if (bdSQLite.conectarBDSQLite(txtBDSQLite.Text, txtBDSQLiteContrasena.Text, ref resultado))
            { 
                MessageBox.Show("Conexión establecida con la BD SQLite." +
                    Environment.NewLine + Environment.NewLine + resultado,
                    "Conexión BD",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                List<Cuentas> cuentas = new List<Cuentas>();
                bdSQLite.obtenerCuentas(ref resultado, ref cuentas);
                foreach (Cuentas cuenta in cuentas)
                {
                    //Añadimos una primera fila al ListView
                    tareasTwitter.Add(cuenta.Id + 1, new TareasTwitter());
                }
            }
            else
                MessageBox.Show("Error al conectar con la BD SQLite." +
                    Environment.NewLine + Environment.NewLine + resultado,
                    "Conexión BD",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string resultado = "";
            construirTabla(ref resultado);
            txtLog.Text = resultado + Environment.NewLine + txtLog.Text;
        }

        public void construirTablaCuentas(ref string resultado)
        {
            //Establecemos las propiedades del ListView
            listViewCuentas.Clear();
            listViewCuentas.View = View.Details;
            listViewCuentas.GridLines = true;
            listViewCuentas.FullRowSelect = true;
            //Añadimos las columnas al ListView
            listViewCuentas.Columns.Add("ID", 200);
            listViewCuentas.Columns.Add("Username", 200);
            listViewCuentas.Columns.Add("Password", 350);
            listViewCuentas.Columns.Add("Fecha", 200);
            //Añadimos los elementos (filas) al ListView
            string[] elementosFila = new string[4];
            ListViewItem elementoListView;

            List<Cuentas> cuentas = new List<Cuentas>();
            bdSQLite.obtenerCuentas(ref resultado, ref cuentas);
            foreach (Cuentas cuenta in cuentas)
            {
                //Añadimos una primera fila al ListView
                elementosFila[0] = Convert.ToString(cuenta.Id);
                elementosFila[1] = Convert.ToString(cuenta.Username);
                elementosFila[2] = Convert.ToString(cuenta.Password);
                elementosFila[3] = Convert.ToString(cuenta.Fecha);
                elementoListView = new ListViewItem(elementosFila);
                listViewCuentas.Items.Add(elementoListView);
            }
        }
        public void construirTabla(ref string resultado)
        {
            //Establecemos las propiedades del ListView
            listViewTweetsAutomatizados.Clear();
            listViewTweetsAutomatizados.View = View.Details;
            listViewTweetsAutomatizados.GridLines = true;
            listViewTweetsAutomatizados.FullRowSelect = true;
            //Añadimos las columnas al ListView
            listViewTweetsAutomatizados.Columns.Add("ID", 100);
            listViewTweetsAutomatizados.Columns.Add("Tweet", 350);
            listViewTweetsAutomatizados.Columns.Add("Estatus", 200);
            listViewTweetsAutomatizados.Columns.Add("Fecha", 200);
            //Añadimos los elementos (filas) al ListView
            string[] elementosFila = new string[4];
            ListViewItem elementoListView;

            List<AutomaticMessageModel> tweets = new List<AutomaticMessageModel>();
            bdSQLite.obtenerTweetsAutomatizados(ref resultado, ref tweets, txtUsuarioAccesoTwitter.Text);
            foreach (AutomaticMessageModel tweet in tweets)
            {
                //Añadimos una primera fila al ListView
                elementosFila[0] = Convert.ToString(tweet.Id);
                elementosFila[1] = Convert.ToString(tweet.Tweet);
                switch(tweet.Status)
                {
                    case 0:
                        elementosFila[2] = Convert.ToString("Por eliminar");
                        break;
                    case 1:
                        elementosFila[2] = Convert.ToString("Por publicar");
                        break;
                }
                elementosFila[3] = Convert.ToString(tweet.Fecha);
                elementoListView = new ListViewItem(elementosFila);
                listViewTweetsAutomatizados.Items.Add(elementoListView);
            }
        }

        private void ButtonAgregar_Click(object sender, EventArgs e)
        {
            string resultado = "";
            List<AutomaticMessageModel> tweets = new List<AutomaticMessageModel>();
            bdSQLite.obtenerTweetsAutomatizados(ref resultado, ref tweets, txtUsuarioAccesoTwitter.Text);
            var valor = tweets.Find(x => x.Tweet == Convert.ToString(textBoxTweet.Text));

            if (valor == null)
            {
                bdSQLite.insertarTweet(Convert.ToString(textBoxTweet.Text), ref resultado, txtUsuarioAccesoTwitter.Text);
                construirTabla(ref resultado);
            }
            else
            {
                resultado = System.DateTime.Now + " " + "El tweet ya se encuentra registrado.";
                MessageBox.Show("El tweet ya se encuentra registrado." +
                    Environment.NewLine + Environment.NewLine + resultado,
                    "Tweet",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            txtLog.Text = resultado + Environment.NewLine + txtLog.Text;
        }

        private void listViewTweetsAutomatizados_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBoxTweet.Text = Convert.ToString(listViewTweetsAutomatizados.Items[listViewTweetsAutomatizados.FocusedItem.Index].SubItems[1].Text);
        }

        private void ButtonEliminar_Click(object sender, EventArgs e)
        {
            string resultado = "";
            bdSQLite.deleteTweet(Convert.ToInt32(listViewTweetsAutomatizados.Items[listViewTweetsAutomatizados.FocusedItem.Index].SubItems[0].Text), ref resultado);
            construirTabla(ref resultado);
            txtLog.Text = resultado + Environment.NewLine + txtLog.Text;
        }

        private void ButtonEditar_Click(object sender, EventArgs e)
        {
            AutomaticMessageModel message = new AutomaticMessageModel();
            try
            {

                message.Id = Convert.ToInt32(listViewTweetsAutomatizados.Items[listViewTweetsAutomatizados.FocusedItem.Index].SubItems[0].Text);
                id = message.Id;
            }
            catch (Exception ex)
            {
                message.Id = id;
            }
            message.Tweet = Convert.ToString(textBoxTweet.Text);
            string resultado = "";
            List<AutomaticMessageModel> tweets = new List<AutomaticMessageModel>();
            bdSQLite.obtenerTweetsAutomatizados(ref resultado, ref tweets, txtUsuarioAccesoTwitter.Text);
            var valor = tweets.Find(x => x.Tweet == Convert.ToString(textBoxTweet.Text));

            if (valor == null)
            {
                bdSQLite.updateTweet(message, ref resultado);
                construirTabla(ref resultado);
            }
            else
            {
                resultado = System.DateTime.Now + " " + "El tweet ya se encuentra registrado.";
                MessageBox.Show("El tweet ya se encuentra registrado." +
                    Environment.NewLine + Environment.NewLine + resultado,
                    "Tweet",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            txtLog.Text = resultado + Environment.NewLine + txtLog.Text;
        }

        private void MostrarCuentas_Click(object sender, EventArgs e)
        {
            string resultado = "";
            construirTablaCuentas(ref resultado);
            txtLog.Text = resultado + Environment.NewLine + txtLog.Text;
        }

        private void AgregarCuenta_Click(object sender, EventArgs e)
        {
            string resultado = "";
            List<Cuentas> cuentas = new List<Cuentas>();
            bdSQLite.obtenerCuentas(ref resultado, ref cuentas);
            var valor = cuentas.Find(x => x.Username == Convert.ToString(textBoxUsernameCuentas.Text));

            if (valor == null)
            {
                Cuentas cuenta = new Cuentas();
                cuenta.Username = textBoxUsernameCuentas.Text; cuenta.Password = textBoxPasswordCuentas.Text; 
                bdSQLite.insertarCuenta(ref tareasTwitter,cuenta, ref resultado);
                construirTablaCuentas(ref resultado);
            }
            else
            {
                resultado = System.DateTime.Now + " " + "La cuenta ya se encuentra registrado.";
                MessageBox.Show("El cuenta ya se encuentra registrado." +
                    Environment.NewLine + Environment.NewLine + resultado,
                    "Cuentas",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            txtLog.Text = resultado + Environment.NewLine + txtLog.Text;
        }

        private void EliminarCuentas_Click(object sender, EventArgs e)
        {
            string resultado = "";
            bdSQLite.deleteCuenta(Convert.ToInt32(listViewCuentas.Items[listViewCuentas.FocusedItem.Index].SubItems[0].Text), ref resultado);
            construirTablaCuentas(ref resultado);
            txtLog.Text = resultado + Environment.NewLine + txtLog.Text;
        }

        private void EditarCuentas_Click(object sender, EventArgs e)
        {
            Cuentas cuenta = new Cuentas();
            try
            {

                cuenta.Id = Convert.ToInt32(listViewCuentas.Items[listViewCuentas.FocusedItem.Index].SubItems[0].Text);
                idCuenta = cuenta.Id;
            }
            catch (Exception ex)
            {
                cuenta.Id = idCuenta;
            }
            cuenta.Username = Convert.ToString(textBoxUsernameCuentas.Text);
            cuenta.Password = Convert.ToString(textBoxPasswordCuentas.Text);
            string resultado = "";
            List<Cuentas> cuentas = new List<Cuentas>();
            bdSQLite.obtenerCuentas(ref resultado, ref cuentas);
            var valor = cuentas.Find(x => x.Username == Convert.ToString(textBoxUsernameCuentas.Text));

            if (valor == null)
            {
                bdSQLite.updateCuenta(cuenta, ref resultado);
                construirTablaCuentas(ref resultado);
            }
            else
            {
                resultado = System.DateTime.Now + " " + "La cuenta ya se encuentra registrado.";
                MessageBox.Show("La cuenta ya se encuentra registrado." +
                    Environment.NewLine + Environment.NewLine + resultado,
                    "Cuentas",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            txtLog.Text = resultado + Environment.NewLine + txtLog.Text;
        }

        private void conectarCuentaButton_Click(object sender, EventArgs e)
        {
            string resultado = "";
            int id = Convert.ToInt32(listViewCuentas.Items[listViewCuentas.FocusedItem.Index].SubItems[0].Text);
            txtUsuarioAccesoTwitter.Text = Convert.ToString(listViewCuentas.Items[listViewCuentas.FocusedItem.Index].SubItems[1].Text);
            string consumerKey = Convert.ToString(listViewCuentas.Items[listViewCuentas.FocusedItem.Index].SubItems[2].Text);
            string consumerSecret = Convert.ToString(listViewCuentas.Items[listViewCuentas.FocusedItem.Index].SubItems[3].Text);
            i = id + 1;
            if (tareasTwitter[id+1].obtenerTokenTwitter(ref resultado, consumerKey, consumerSecret))
            {
                btObtenerDatosPerfil.Enabled = true;
                btPerfilVisitarURL.Enabled = true;
                tabListadeTweetsAutomatizados.Enabled = true;
                btObtenerDatosPerfil.Text = "Obtener datos del perfil @" +
                    txtUsuarioAccesoTwitter.Text;
            }
            else
            {
                btObtenerDatosPerfil.Enabled = false;
                btPerfilVisitarURL.Enabled = false;
                tabListadeTweetsAutomatizados.Enabled = false;
            }
            txtLog.Text = resultado;

        }

        private void listViewCuentas_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            textBoxUsernameCuentas.Text = Convert.ToString(listViewCuentas.Items[listViewCuentas.FocusedItem.Index].SubItems[1].Text);
            textBoxPasswordCuentas.Text = Convert.ToString(listViewCuentas.Items[listViewCuentas.FocusedItem.Index].SubItems[2].Text);
        }
    }
}
