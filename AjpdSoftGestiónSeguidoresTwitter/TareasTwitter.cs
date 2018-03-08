using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Spring.Social.OAuth1;
using Spring.Social.Twitter.Api;
using Spring.Social.Twitter.Connect;
using LinqToTwitter;
using System.Net.Http;
using System.IO;
using System.Net;
using TweetSharp;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GestiónSeguidoresTwitter
{
    //datos perfil usuario Twitter
    public struct PerfilUsuario
    {
        public string nombre;
        public string nick;
        public long id;
        public string descripcion;
        public int seguidores;
        public int siguiendo;
        public DateTime? fechaAlta;
        public string ubicacion;
        public string idioma;
        public string urlPerfil;
        public bool notificaciones;
        public bool traductor;
        public bool verificado;
        public bool protegido;
        public bool loSigo;
    }

    //datos mensaje directo Twitter
    public struct MensajeDirecto
    {
        public long id;
        public long idUsuario;
        public string nick; 
        public string usuario;
        public string mensaje;
        public DateTime? fecha;
    }

    //datos seguidor Twitter
    public struct Seguidor
    {
        public PerfilUsuario perfilUsuario;
        public string meSigue;
    }

    //Datos amigo Twitter
    public struct Amigo
    {
        public PerfilUsuario perfilUsuario;
        public string meSigue;
    }

    //Tweet Twitter (TimeLine, Retweet, menciones)
    public struct DatosTweet
    {
        public long id;
        public long idUsuario;
        public string usuario;
        public DateTime? fecha;
        public string tweet;
        public long NumeroRetweet;
    }

    public class TareasTwitter
    {
        public static string TwitterConsumerKeyClass = "";
        public static string TwitterConsumerSecretClass = "";
        ITwitter twitter;
        TwitterContext context;

        //Conectar con API de Twitter mediante PIN para obtener token de acceso
        //devuelve true si la conexión se ha realizado correctamente
        public bool obtenerTokenTwitter(ref string resultado, string username, formTwitter form, string TwitterConsumerKey = "8T90BKQKn6m3onMZqRMSkTliF", string TwitterConsumerSecret = "20JCkeUiJlAze7rUmoHgwdGUwXItgLess93bRI8DRpgpDdGPML")
        {
            try
            {
                
                string text = System.IO.File.ReadAllText(form.txtRutaCuenta.Text+"\\acces.txt");
                var accounts = Convert.ToString(text).Split(new string[] { "</tweet>" }, StringSplitOptions.None);
                int i = 0;
                int j = 0;
                foreach (var account in accounts)
                {
                    if ((account.Split(new string[] { "\r\n" }, StringSplitOptions.None)[0] != "" ? account.Split(new string[] { "\r\n" }, StringSplitOptions.None)[0] : account.Split(new string[] { "\r\n" }, StringSplitOptions.None)[1]).Split(new string[] { "username:" }, StringSplitOptions.None)[1] == username)
                    {
                        j = i;
                    }
                    i++;
                }

                var auth = new SingleUserAuthorizer
                {
                    CredentialStore = new SingleUserInMemoryCredentialStore
                    {
                        ConsumerKey = (accounts[j].Split(new string[] { "\r\n" }, StringSplitOptions.None)[0] != "" ? accounts[j].Split(new string[] { "\r\n" }, StringSplitOptions.None)[1] : accounts[j].Split(new string[] { "\r\n" }, StringSplitOptions.None)[2]).Split(new string[] { "Consumer Key:" }, StringSplitOptions.None)[1],
                        ConsumerSecret = (accounts[j].Split(new string[] { "\r\n" }, StringSplitOptions.None)[0] != "" ? accounts[j].Split(new string[] { "\r\n" }, StringSplitOptions.None)[2] : accounts[j].Split(new string[] { "\r\n" }, StringSplitOptions.None)[3]).Split(new string[] { "Consumer Secret:" }, StringSplitOptions.None)[1],
                        AccessToken = (accounts[j].Split(new string[] { "\r\n" }, StringSplitOptions.None)[0] != "" ? accounts[j].Split(new string[] { "\r\n" }, StringSplitOptions.None)[3] : accounts[j].Split(new string[] { "\r\n" }, StringSplitOptions.None)[4]).Split(new string[] { "Access Token:" }, StringSplitOptions.None)[1],
                        AccessTokenSecret = (accounts[j].Split(new string[] { "\r\n" }, StringSplitOptions.None)[0] != "" ? accounts[j].Split(new string[] { "\r\n" }, StringSplitOptions.None)[4] : accounts[j].Split(new string[] { "\r\n" }, StringSplitOptions.None)[5]).Split(new string[] { "Access Token Secret:" }, StringSplitOptions.None)[1]
                    }
                };

                context = new TwitterContext(auth);

                TwitterServiceProvider twitterServiceProvider =
                    new TwitterServiceProvider(TwitterConsumerKey, TwitterConsumerSecret);
                // Autenticación en Twitter usando código PIN
                OAuthToken oauthToken =
                     twitterServiceProvider.OAuthOperations.FetchRequestTokenAsync("oob", null).Result;
                string authenticateUrl =
                    twitterServiceProvider.OAuthOperations.BuildAuthorizeUrl(oauthToken.Value, null);
                System.Diagnostics.Process.Start(authenticateUrl);

                string pinCode = "";
                InputBox.solicitarTexto("Código PIN",
                    "Introduzca el código PIN obtenido de Twitter:", ref pinCode);
                // Step 3 - Exchange the Request Token for an Access Token
                string verifier = pinCode; // <-- This is input into your application by your user

                AuthorizedRequestToken requestToken =
                    new AuthorizedRequestToken(oauthToken, pinCode);
                OAuthToken oauthAccessToken =
                    twitterServiceProvider.OAuthOperations.ExchangeForAccessTokenAsync(requestToken, null).Result;
                twitter = twitterServiceProvider.GetApi(oauthAccessToken.Value, oauthAccessToken.Secret);
                resultado = System.DateTime.Now + " " +
                    "Conectado a API de twitter correctamente con PIN";
                return true;
            }
            catch (Exception error)
            {
                resultado = System.DateTime.Now + " " +
                    "Error al obtener token acceso Twitter: " + error.Message;
                return false;
            }
        }
        //Obtener los datos del perfil del usuario de Twitter
        //que ha iniciado sesión de autenticación con PIN
        public void obtenerDatosPerfilPIN(string usuario,
            ref PerfilUsuario datosPerfilUsuario, ref string resultado)
        {
            try
            {
                datosPerfilUsuario.nick =
                    twitter.UserOperations.GetUserProfileAsync(usuario).Result.ScreenName;
                datosPerfilUsuario.nombre =
                        twitter.UserOperations.GetUserProfileAsync(usuario).Result.Name;
                datosPerfilUsuario.descripcion =
                        twitter.UserOperations.GetUserProfileAsync(usuario).Result.Description;
                datosPerfilUsuario.siguiendo =
                        twitter.UserOperations.GetUserProfileAsync(usuario).Result.FriendsCount;
                datosPerfilUsuario.seguidores =
                        twitter.UserOperations.GetUserProfileAsync(usuario).Result.FollowersCount;
                datosPerfilUsuario.fechaAlta =
                        twitter.UserOperations.GetUserProfileAsync(usuario).Result.CreatedDate;
                datosPerfilUsuario.ubicacion =
                        twitter.UserOperations.GetUserProfileAsync(usuario).Result.Location;
                datosPerfilUsuario.id =
                        twitter.UserOperations.GetUserProfileAsync(usuario).Result.ID;
                datosPerfilUsuario.idioma =
                        twitter.UserOperations.GetUserProfileAsync(usuario).Result.Language;
                datosPerfilUsuario.urlPerfil =
                        twitter.UserOperations.GetUserProfileAsync(usuario).Result.ProfileUrl;
                datosPerfilUsuario.verificado =
                    twitter.UserOperations.GetUserProfileAsync(usuario).Result.IsVerified;
                datosPerfilUsuario.traductor =
                    twitter.UserOperations.GetUserProfileAsync(usuario).Result.IsTranslator;
                datosPerfilUsuario.protegido =
                    twitter.UserOperations.GetUserProfileAsync(usuario).Result.IsProtected;
                resultado = System.DateTime.Now + " " +
                        "Datos del perfil [" + usuario + "] obtenidos correctamente";
            }
            catch (Exception error)
            {
                resultado = System.DateTime.Now + " " +
                            "Error al obtener datos del perfil [" + usuario +
                            "]: " + error.Message;
            }
        }


        //obtener datos perfil de usuario por ID
        public void obtenerPerfilUsuario(long id,
            ref PerfilUsuario perfilUsuario, ref string resultado)
        {
            try
            {
                TwitterProfile perfilUsuarioTwitter =
                    twitter.UserOperations.GetUserProfileAsync(id).Result;
                PerfilUsuario perfil = new PerfilUsuario();
                perfil.descripcion = perfilUsuarioTwitter.Description;
                perfil.fechaAlta = perfilUsuarioTwitter.CreatedDate;
                perfil.id = perfilUsuarioTwitter.ID;
                perfil.idioma = perfilUsuarioTwitter.Language;
                perfil.nick = perfilUsuarioTwitter.ScreenName;
                perfil.nombre = perfilUsuarioTwitter.Name;
                perfil.verificado = perfilUsuarioTwitter.IsVerified;
                perfil.traductor = perfilUsuarioTwitter.IsTranslator;
                perfil.protegido = perfilUsuarioTwitter.IsProtected;
                perfil.loSigo = perfilUsuarioTwitter.IsFollowing;
                perfil.seguidores = perfilUsuarioTwitter.FriendsCount;
                perfil.siguiendo = perfilUsuarioTwitter.FriendsCount;
                perfil.ubicacion = perfilUsuarioTwitter.Location;
                perfil.urlPerfil = perfilUsuarioTwitter.ProfileUrl;
                perfilUsuario = perfil;
                resultado = "Obtenido perfil usuario " + Convert.ToString(id);
            }
            catch (Exception error)
            {
                resultado = System.DateTime.Now + " " +
                            "Error al obtener datos del perfil [" + Convert.ToString(id) +
                            "]: " + error.Message;
            }
        }


        //Enviar mensaje directo a usuario Twitter 
        //debe ser seguido por el usuario que ha iniciado sesión
        public void enviarMensajeDirectoTwitter(string mensaje,
            long idUsuario, ref string resultado)
        {
            if (mensaje != "")
            {
                try
                {
                    Spring.Social.Twitter.Api.DirectMessage mensajeDirectoTwiiter =
                        twitter.DirectMessageOperations.SendDirectMessageAsync(idUsuario, mensaje).Result;
                    resultado = System.DateTime.Now + " " +
                        "Mensaje directo [" + mensaje + "] enviado a [" +
                        Convert.ToString(idUsuario) + "]";
                }
                catch (Exception error)
                {
                    resultado = System.DateTime.Now + " " +
                        "Error al enviar mensaje directo: " + error.Message;
                }
            }
        }

        //Obtener últimos 20 mensajes directos Twitter
        public List<MensajeDirecto> obtenerUltimos20MensajesDirectos(ref string resultado, long userId)
        {
            try
            {

                List<MensajeDirecto> mensajes = new List<MensajeDirecto>();

                IList<Spring.Social.Twitter.Api.DirectMessage> mensajesTwiiter =
                    twitter.DirectMessageOperations.GetDirectMessagesReceivedAsync().Result;

                foreach (Spring.Social.Twitter.Api.DirectMessage mensajeTwitter in mensajesTwiiter)
                {
                    MensajeDirecto mensajeDirecto = new MensajeDirecto();
                    mensajeDirecto.id = mensajeTwitter.ID;
                    mensajeDirecto.idUsuario = mensajeTwitter.Sender.ID;
                    mensajeDirecto.nick = mensajeTwitter.Sender.ScreenName;
                    mensajeDirecto.usuario = mensajeTwitter.Sender.Name;
                    mensajeDirecto.mensaje = mensajeTwitter.Text;
                    mensajeDirecto.fecha = mensajeTwitter.CreatedAt;
                    mensajes.Add(mensajeDirecto);
                    resultado = System.DateTime.Now + " " +
                        "Obteniendo mensaje [" + Convert.ToString(mensajeTwitter.ID) + "]" +
                        Environment.NewLine + resultado;
                }
                resultado = System.DateTime.Now + " " +
                    "Últimos 20 mensajes directos obtenidos";
                return mensajes;
            }
            catch (Exception error)
            {
                resultado = System.DateTime.Now + " " +
                    "Error al obtener mensajes directos: " + error.Message;
                return null;
            }
        }

        //Elimina un mensaje directo de Twitter que nos hayan enviado
        public void eliminarMensajeDirecto(long idMensaje, ref string resultado)
        {
            try
            {
                Spring.Social.Twitter.Api.DirectMessage mensaje;
                mensaje =
                    twitter.DirectMessageOperations.DeleteDirectMessageAsync(idMensaje).Result;
                resultado = System.DateTime.Now + " " +
                    "Mensaje directo [" + idMensaje + "] eliminado correctamente";
            }
            catch (Exception error)
            {
                resultado = System.DateTime.Now + " " +
                    "Error al eliminar mensaje directo [" +
                    Convert.ToString(idMensaje) + "]: " + error.Message;
            }
        }

        //Obtener últimos 20 usuarios que nos han seguido en Twitter
        public List<Seguidor> obtenerUltimos20Seguidores(ref string resultado)
        {
            try
            {
                List<Seguidor> seguidores = new List<Seguidor>();

                CursoredList<TwitterProfile> seguidoresTwitter =
                    twitter.FriendOperations.GetFollowersAsync().Result;
                foreach (TwitterProfile seguidorTwitter in seguidoresTwitter)
                {
                    Seguidor seguidor = new Seguidor();
                    seguidor.perfilUsuario.id = seguidorTwitter.ID;
                    seguidor.perfilUsuario.nick = seguidorTwitter.ScreenName;
                    seguidor.perfilUsuario.nombre = seguidorTwitter.Name;
                    if (seguidorTwitter.IsFollowing)
                        seguidor.meSigue = "Sí";
                    else
                        seguidor.meSigue = "No";
                    seguidor.perfilUsuario.seguidores = seguidorTwitter.FollowersCount;
                    seguidor.perfilUsuario.siguiendo = seguidorTwitter.FriendsCount;
                    seguidor.perfilUsuario.fechaAlta = seguidorTwitter.CreatedDate;
                    seguidor.perfilUsuario.descripcion = seguidorTwitter.Description;
                    seguidor.perfilUsuario.urlPerfil = seguidorTwitter.ProfileUrl;
                    seguidor.perfilUsuario.ubicacion = seguidorTwitter.Location;
                    seguidor.perfilUsuario.idioma = seguidorTwitter.Language;
                    seguidores.Add(seguidor);
                    resultado = "Obteniendo seguidor [" + Convert.ToString(seguidorTwitter.ID) + "]" +
                        Environment.NewLine + resultado;
                }
                resultado = System.DateTime.Now + " " +
                    "Últimos 20 seguidores obtenidos" +
                    Environment.NewLine + resultado;
                return seguidores;
            }
            catch (Exception error)
            {
                resultado = System.DateTime.Now + " " +
                    "Error al obtener seguidores: " + error.Message;
                return null;
            }
        }

        //Obtener últimos 20 usuarios a los que seguimos (amigos) en Twitter
        public List<Amigo> obtenerUltimos20Amigos(ref string resultado)
        {
            try
            {
                List<Amigo> amigos = new List<Amigo>();

                CursoredList<TwitterProfile> amigosTwitter =
                    twitter.FriendOperations.GetFriendsAsync().Result;
                foreach (TwitterProfile amigoTwitter in amigosTwitter)
                {
                    Amigo amigo = new Amigo();
                    amigo.perfilUsuario.id = amigoTwitter.ID;
                    amigo.perfilUsuario.nick = amigoTwitter.ScreenName;
                    amigo.perfilUsuario.nombre = amigoTwitter.Name;
                    if (amigoTwitter.IsFollowing)
                        amigo.meSigue = "Sí";
                    else
                        amigo.meSigue = "No";
                    amigo.perfilUsuario.seguidores = amigoTwitter.FollowersCount;
                    amigo.perfilUsuario.siguiendo = amigoTwitter.FriendsCount;
                    amigo.perfilUsuario.fechaAlta = amigoTwitter.CreatedDate;
                    amigo.perfilUsuario.descripcion = amigoTwitter.Description;
                    amigo.perfilUsuario.urlPerfil = amigoTwitter.ProfileUrl;
                    amigo.perfilUsuario.ubicacion = amigoTwitter.Location;
                    amigo.perfilUsuario.idioma = amigoTwitter.Language;
                    amigos.Add(amigo);
                    resultado = "Obteniendo amigo [" + Convert.ToString(amigoTwitter.ID) + "]" +
                        Environment.NewLine + resultado;
                }
                resultado = System.DateTime.Now + " " +
                    "Últimos 20 amigos obtenidos" +
                    Environment.NewLine + resultado;
                return amigos;
            }
            catch (Exception error)
            {
                resultado = System.DateTime.Now + " " +
                    "Error al obtener amigos: " + error.Message;
                return null;
            }
        }

        //Dejar de seguir a usuario que seguimos (amigo) por nick
        public void dejarDeSeguirAmigo(string usuario, ref string resultado)
        {
            try
            {
                TwitterProfile amigo =
                    twitter.FriendOperations.UnfollowAsync(usuario).Result;
                resultado = System.DateTime.Now + " " +
                    "Dejado de seguir a [" + usuario + "]";
            }
            catch (Exception error)
            {
                resultado = System.DateTime.Now + " " +
                    "Error al dejar de seguir a [" + usuario + "]: " +
                    error.Message;
            }
        }

        //Dejar de seguir a usuario que seguimos (amigo) por ID
        public void dejarDeSeguirAmigo(long usuario, ref string resultado)
        {
            try
            {
                TwitterProfile amigo =
                    twitter.FriendOperations.UnfollowAsync(usuario).Result;
                resultado = System.DateTime.Now + " " +
                    "Dejado de seguir a [" + usuario + "]";
            }
            catch (Exception error)
            {
                resultado = System.DateTime.Now + " " +
                    "Error al dejar de seguir a [" + Convert.ToString(usuario) + "]: " +
                    error.Message;
            }
        }


        public void generarTweets(List<AutomaticMessageModel> tweetsAutomatics, formTwitter form)
        {
            try
            { 
                while (true)
                {
                    string resultado = "";
                    Random rnd = new Random();
                    int i = rnd.Next(tweetsAutomatics.Count);

                    if (tweetsAutomatics[i].Status == 1 && tweetsAutomatics[i].State == 1)
                    {
                        this.enviarTweet(tweetsAutomatics[i].Tweet, form, Convert.ToString(tweetsAutomatics[i].Id));
                        tweetsAutomatics[i].Status = 0;
                        form.bdSQLite.updateStatus(tweetsAutomatics[i], ref resultado);
                        form.construirTabla(ref resultado);
                        break;
                    }
                    else if (tweetsAutomatics[i].Status == 0 && tweetsAutomatics[i].State == 1)
                    {
                        this.eliminarTweet(tweetsAutomatics[i].Tweet, form);
                        tweetsAutomatics[i].Status = 1;
                        form.bdSQLite.updateStatus(tweetsAutomatics[i], ref resultado);
                        form.construirTabla(ref resultado);
                        break;
                    }
                }
            }            
            catch (Exception ex)
            {

            }
}

        public void generarSeguidores(List<Cuentas> cuentas, formTwitter form)
        {
            try
            { 
                while (true && cuentas.Find(x => x.Follow == true) != null)
                {
                    string resultado = "";
                    Random rnd = new Random();
                    int i = rnd.Next(cuentas.Count);

                    if (cuentas[i].Follow)
                    {
                        if (cuentas[i].Status == 1 && cuentas[i].State == 1)
                        {
                            this.FollowToFollows(cuentas[i].Username, form);
                            cuentas[i].Status = 0;
                            form.bdSQLite.updateStatusSeguidores(cuentas[i], ref resultado);
                            form.construirTablaCuentas(ref resultado);
                            break;
                        }
                        else if (cuentas[i].Status == 0 && cuentas[i].State == 1)
                        {
                            this.UnFollow(cuentas[i].Username, form);
                            cuentas[i].Status = 1;
                            form.bdSQLite.updateStatusSeguidores(cuentas[i], ref resultado);
                            this.UnFollowTwenttyNoFollow(form);
                            form.construirTablaCuentas(ref resultado);
                            break;
                        }
                    }

                }
            }            
            catch (Exception ex)
            {

            }
}

        public void seguirUsuario(string usuario, ref string resultado)
        {
            try
            {
                TwitterProfile amigo =
                    twitter.FriendOperations.FollowAsync(usuario).Result;
                resultado = System.DateTime.Now + " " +
                    "Siguiendo a [" + usuario + "]";
            }
            catch (Exception error)
            {
                resultado = System.DateTime.Now + " " +
                    "Error al seguir a [" + usuario + "]: " + error.Message;
            }
        }



        //Obtener los 20 últimos tweets de mi línea de tiempo (HomeTimeLine)
        //últimos de mis tweets, menciones y mis tweets retwitteados
        public List<DatosTweet> obtener20UltimosTweet(string tipo, ref string resultado)
        {
            try
            {
                List<DatosTweet> tweets = new List<DatosTweet>();

                IList<Tweet> lineaTiempo;
                lineaTiempo =
                    twitter.TimelineOperations.GetHomeTimelineAsync().Result;

                if (tipo == "UserTimeLine")
                {
                    lineaTiempo =
                        twitter.TimelineOperations.GetUserTimelineAsync().Result;
                }
                if (tipo == "Menciones")
                {
                    lineaTiempo =
                        twitter.TimelineOperations.GetMentionsAsync().Result;
                }
                if (tipo == "Favoritos")
                {
                    lineaTiempo =
                        twitter.TimelineOperations.GetFavoritesAsync().Result;
                }
                if (tipo == "Retweets")
                {
                    lineaTiempo =
                        twitter.TimelineOperations.GetRetweetsOfMeAsync().Result;
                }

                foreach (Tweet tweetTwitter in lineaTiempo)
                {
                    DatosTweet datoTweet = new DatosTweet();
                    datoTweet.id = tweetTwitter.ID;
                    datoTweet.idUsuario = tweetTwitter.User.ID;
                    datoTweet.usuario = tweetTwitter.User.ScreenName;
                    datoTweet.fecha = tweetTwitter.CreatedAt;
                    datoTweet.tweet = tweetTwitter.Text;
                    datoTweet.NumeroRetweet = tweetTwitter.RetweetCount;
                    tweets.Add(datoTweet);
                    resultado = System.DateTime.Now + " " +
                        "Obteniendo tweet [" + Convert.ToString(tweetTwitter.ID) + "]" +
                        Environment.NewLine + resultado;
                }
                resultado = System.DateTime.Now + " " +
                    "Tweets TimeLine obtenidos" + Environment.NewLine + resultado;

                return tweets;
            }
            catch (Exception error)
            {
                resultado = System.DateTime.Now + " " +
                    "Error al obtener tweets de TimeLine: " + error.Message;
                return null;
            }
        }


        //Enviar tweet
        public async void enviarTweet(string texto, formTwitter form, string tweetid)
        {
            try
            {
                string resultado = "";
                Random rnd = new Random();
                Tweet tweet;
                List<string> listImagen = new List<string>();
                form.bdSQLite.obtenerImagen(ref resultado,ref listImagen, tweetid);
                List<Task<Media>> imageUploadTasks = new List<Task<Media>>();

                List<string> listVideo = new List<string>();
                form.bdSQLite.obtenerVideos(ref resultado, ref listVideo, tweetid);
                List<Task<Media>> videoUploadTasks = new List<Task<Media>>();

                if (listImagen.Count > 0 || listVideo.Count > 0)
                {
                    int i = rnd.Next(10000);
                    if (i % 2 == 0)
                    {
                        foreach (var image in listImagen)
                        {
                            imageUploadTasks.Add(context.UploadMediaAsync(File.ReadAllBytes(image), "image/jpg", "tweet_image"));
                        }
                        await Task.WhenAll(imageUploadTasks);

                        var mediaIds =
                           (from tsk in imageUploadTasks
                            select tsk.Result.MediaID)
                           .ToList();
                        await context.TweetAsync(
                            texto,
                            mediaIds
                        );
                    }
                    else
                    {
                        Media media = null;
                        foreach (var video in listVideo)
                        {
                            media = await context.UploadMediaAsync(File.ReadAllBytes(video), "video/mp4", "tweet_video");
                        }

                        Media mediaStatusResponse = null;
                        do
                        {
                            if (mediaStatusResponse != null)
                            {
                                int checkAfterSeconds = mediaStatusResponse?.ProcessingInfo?.CheckAfterSeconds ?? 0;
                                Console.WriteLine($"Twitter video testing in progress - waiting {checkAfterSeconds} seconds.");
                                await Task.Delay(checkAfterSeconds * 1000);
                            }

                            mediaStatusResponse =
                                await
                                (from stat in context.Media
                                 where stat.Type == MediaType.Status &&
                                       stat.MediaID == media.MediaID
                                 select stat)
                                .SingleOrDefaultAsync();
                        } while (mediaStatusResponse?.ProcessingInfo?.State == MediaProcessingInfo.InProgress);

                        if (mediaStatusResponse?.ProcessingInfo?.State == MediaProcessingInfo.Succeeded)
                        {
                            Status tweett = await context.TweetAsync(texto, new ulong[] { media.MediaID });

                            if (tweett != null)
                                Console.WriteLine($"Tweet sent: {tweett.Text}");
                        }
                        else
                        {
                            MediaError error = mediaStatusResponse?.ProcessingInfo?.Error;

                            if (error != null)
                            {
                                form.txtLog.Text = System.DateTime.Now + " " +
                                    "Error al enviar tweet: " + "Request failed - Code: {error.Code}, Name: {error.Name}, Message: {error.Message}" + Environment.NewLine + form.txtLog.Text;
                            }
                        }
                    }
                     
                }else
                    tweet = await twitter.TimelineOperations.UpdateStatusAsync(texto);
            }
            catch (Exception error)
            {
                form.txtLog.Text = System.DateTime.Now + " " +
                    "Error al enviar tweet: " + error.Message + Environment.NewLine + form.txtLog.Text;
            }
        }

        public async void enviarTweet(string texto, formTwitter form)
        {
            try
            {
                Tweet tweet = await twitter.TimelineOperations.UpdateStatusAsync(texto);
            }
            catch (Exception error)
            {
                form.txtLog.Text = System.DateTime.Now + " " +
                    "Error al enviar tweet: " + error.Message + Environment.NewLine + form.txtLog.Text;
            }
        }
        //Eliminar tweet
        public async void eliminarTweet(string texto, formTwitter form)
        {
            try
            {
                IList<Tweet> lineaTiempo;
                Tweet tweet;
                long id = 0;
                lineaTiempo =
                    twitter.TimelineOperations.GetUserTimelineAsync().Result;
                foreach (Tweet tweetTwitter in lineaTiempo)
                {
                    if (Convert.ToString(tweetTwitter.Text).Split(new string[] { " https" }, StringSplitOptions.None)[0] == texto.Replace("\r",""))
                        id = tweetTwitter.ID;
                }
                if (lineaTiempo.First(x => x.ID == id) != null)
                    tweet = await twitter.TimelineOperations.DeleteStatusAsync(id);
            }
            catch (Exception error)
            {
                form.txtLog.Text = System.DateTime.Now + " " +
                    "Error al enviar tweet: " + error.Message + Environment.NewLine + form.txtLog.Text;
            }
        }

        //Obtener últimos 5000 usuarios a los que seguimos (amigos) en Twitter
        //cursor = -1 obtendrá los 5000 amigos del usuario con el que hemos iniciado sesión
        public List<long> obtenerUltimos5000Amigos(long cursor, ref string resultado)
        {
            try
            {
                CursoredList<long> amigosTwitter =
                    twitter.FriendOperations.GetFriendIdsInCursorAsync(cursor).Result;
                resultado = System.DateTime.Now + " " +
                    "Últimos 5000 amigos obtenidos" +
                    Environment.NewLine + resultado;
                return amigosTwitter;
            }
            catch (Exception error)
            {
                resultado = System.DateTime.Now + " " +
                    "Error al obtener 5000 últimos amigos: " + error.Message;
                return null;
            }
        }

        public async void FollowToFollows(string username, formTwitter form)
        {
            try
            {
                Random rnd4 = new Random();
                CursoredList<TwitterProfile> twitterp = await twitter.FriendOperations.GetFollowersAsync(username);
                string resultado = "";
                foreach (TwitterProfile tp in twitterp)
                {
                    try
                    {

                        if (form.txtUsuarioAccesoTwitter.Text != tp.ScreenName)
                        {
                            await twitter.FriendOperations.FollowAsync(tp.ID);
                            List<Cuentas> cuentas = new List<Cuentas>();
                            form.bdSQLite.obtenerCuentas(ref resultado, ref cuentas);
                            var valor = cuentas.Find(x => x.Username == Convert.ToString(tp.ScreenName));

                            if (valor == null)
                            {
                                Cuentas cuenta = new Cuentas();
                                cuenta.Username = tp.ScreenName;
                                cuenta.Status = 1;

                                int i4 = rnd4.Next(10000);
                                if (i4 % 2 == 0)
                                    cuenta.Follow = true;
                                else
                                    cuenta.Follow = false;
                                form.bdSQLite.insertarCuenta(ref form.tareasTwitter, cuenta, ref resultado);
                            }
                            else
                            {
                                resultado = System.DateTime.Now + " " + "La cuenta ya se encuentra registrado.";
                                form.txtLog.Text = System.DateTime.Now + " " +
                                    resultado +
                                    Environment.NewLine + form.txtLog.Text;
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
                form.construirTablaCuentas(ref resultado);
            }
            catch (Exception ex)
            {

            }
        }

        public async void UnFollowTwenttyNoFollow(formTwitter form)
        {
            string resultado = "";

            try
            {
                form.btTest_Aux();
                form.UnFollow_Aux();
               
            }
            catch (Exception ex)
            {

            }

        }

        public async void UnFollow(string username, formTwitter form)
        {
            try
            {
                await twitter.FriendOperations.UnfollowAsync(username);
            }            
            catch (Exception ex)
            {

            }

        }

        public async void AddToFavoriteAndRetweet(string username, formTwitter form) {
            try
            {
                IList<Tweet> tweets = await twitter.TimelineOperations.GetUserTimelineAsync(username);
                foreach (Tweet aux in tweets)
                {
                    try
                    {
                        await twitter.TimelineOperations.AddToFavoritesAsync(aux.ID);
                        await twitter.TimelineOperations.RetweetAsync(aux.ID);
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
            }            
            catch (Exception ex)
            {

            }
}

        public void generarRetweetFavoritos(List<Cuentas> cuentas, formTwitter form)
        {
            try
            { 
                while (true && (cuentas.Find(x => x.Autorizar == true) != null || cuentas.Find(x => x.Favorites == true) != null))
                {
                    string resultado = "";
             
                    Random rnd = new Random();
                    int i = rnd.Next(cuentas.Count);

                    if (cuentas[i].Autorizar || cuentas[i].Favorites)
                    {
                        if (cuentas[i].Status == 1 && cuentas[i].State == 1)
                        {
                            this.AddToFavoriteAndRetweet(cuentas[i].Username, form);
                            cuentas[i].Status = 0;
                            form.bdSQLite.updateStatusSeguidores(cuentas[i], ref resultado);
                            form.construirTablaCuentas(ref resultado);
                            break;
                        }
                        else if (cuentas[i].Status == 0 && cuentas[i].State == 1)
                        {
                            //this.UnFollow(cuentas[i].Username, form);
                            cuentas[i].Status = 1;
                            form.bdSQLite.updateStatusSeguidores(cuentas[i], ref resultado);
                            form.construirTablaCuentas(ref resultado);
                            break;
                        }
                    }

                }
            }            
            catch (Exception ex)
            {

            }
        }

        //Obtener últimos 5000 usuarios que nos siguen (seguidores) en Twitter
        //cursor = -1 obtendrá los 5000 seguidores del usuario con el que hemos iniciado sesión
        public List<long> obtenerUltimos5000Seguidores(long cursor, ref string resultado)
        {
            try
            {
                CursoredList <long> seguidoresTwitter =
                    twitter.FriendOperations.GetFollowerIdsInCursorAsync(cursor).Result;

                resultado = System.DateTime.Now + " " +
                    "Últimos 5000 seguidores obtenidos" +
                    Environment.NewLine + resultado;
                return seguidoresTwitter;
            }
            catch (Exception error)
            {
                resultado = System.DateTime.Now + " " +
                    "Error al obtener 5000 últimos seguidores: " + error.Message;
                return null;
            }
        }
        
    }
}
