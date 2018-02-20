using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SQLite;
using System.Data.SqlClient;
using System.Data;

namespace GestiónSeguidoresTwitter
{
    public class BDSQLite
    {
        public SQLiteConnection conexionSQLite;

        //crear la BD SQLite y las tablas para guardar datos de Twitter
        public void crearBDSQLite (string fichero, string contrasena, ref string resultado)
        {     
            try
            {

                //si existe el fichero de BD creamos las tablas
                if (!System.IO.File.Exists(fichero))
                {
                    //creamos la BD SQLite
                    SQLiteConnection.CreateFile(fichero);
                    if (contrasena != "")
                    {
                        //establecemos la contraseña
                        conexionSQLite = new SQLiteConnection("Data Source=" + fichero + ";Version=3;Password=" + contrasena + ";");
                    }

                    //conectamos con la BD SQLite para crear las tablas
                    string resultadoConexion = "";
                    if (conectarBDSQLite(fichero, contrasena, ref resultadoConexion))
                    {
                        //crear las tablas de SQLite para guardar datos de Twitter
                        //tabla usuarios
                        string consultaSQL = "create table cuentas (" +
                            "id long not null primary key, usuario text, password text, fecha datetime, state integer)";
                        SQLiteCommand comandoSQL =
                            new SQLiteCommand(consultaSQL, conexionSQLite);
                        comandoSQL.CommandType = CommandType.Text;
                        comandoSQL.ExecuteNonQuery();
                        //tabla seguidores
                        consultaSQL = "create table seguidores (" +
                            "id long not null primary key, fecha datetime)";
                        comandoSQL =
                            new SQLiteCommand(consultaSQL, conexionSQLite);
                        comandoSQL.CommandType = CommandType.Text;
                        comandoSQL.ExecuteNonQuery();
                        //tabla amigos
                        consultaSQL = "create table amigos (" +
                            "id long not null primary key, fecha datetime)";
                        comandoSQL =
                            new SQLiteCommand(consultaSQL, conexionSQLite);
                        comandoSQL.CommandType = CommandType.Text;
                        comandoSQL.ExecuteNonQuery();
                        //tabla mensajes directos
                        consultaSQL = "create table mensaje (" +
                            "id not null primary key, idusuario long)";
                        comandoSQL =
                            new SQLiteCommand(consultaSQL, conexionSQLite);
                        comandoSQL.CommandType = CommandType.Text;
                        comandoSQL.ExecuteNonQuery();
                        //tabla amigos
                        consultaSQL = "create table tweets (" +
                            "id long not null primary key, tweet text, status integer, fecha datetime, state integer, idCuenta integer)";
                        comandoSQL =
                            new SQLiteCommand(consultaSQL, conexionSQLite);
                        comandoSQL.CommandType = CommandType.Text;
                        comandoSQL.ExecuteNonQuery();

                        resultado = System.DateTime.Now + " " +
                            "BD SQLite creada correctamente.";
                    }
                    else
                    {
                        if (contrasena != "")
                        {
                            //establecemos la contraseña
                            conexionSQLite = new SQLiteConnection("Data Source=" + fichero + ";Version=3;Password=" + contrasena + ";");
                            resultado = System.DateTime.Now + " " +
                                        "Conectado a la BD SQLite correctamente";
                        }
                    }
                }
            }
            catch (Exception error)
            {
                resultado = System.DateTime.Now + " " +
                    "Error al abrir la BD SQLite: " + error.Message;
            }
        }



        //conectar con la base de datos SQLite
        public bool conectarBDSQLite (string fichero, 
            string contrasena, ref string resultado)
        {
            //si existe la BD SQLite
            if (System.IO.File.Exists(fichero))
            {
                string stringConexionSQLite =
                    String.Format("Data Source={0}", fichero);
                try
                {
                    conexionSQLite = new SQLiteConnection(stringConexionSQLite);
                    if (contrasena != "")
                        conexionSQLite.SetPassword(contrasena);
                    conexionSQLite.Open();
                    resultado = System.DateTime.Now + " " +
                        "Conectado a la BD SQLite correctamente";
                    return true;
                }
                catch (Exception error)
                {
                    resultado = System.DateTime.Now + " " +
                        "Error al abrir BD SQLite: " + error.Message;
                    return false;
                }
            }
            else
            {
                resultado = System.DateTime.Now + " " +
                    "Error al abrir BD SQLite: no existe el fichero " +
                    fichero;
                return false;
            }
        }


        //comprobar si existe seguidor en la bd SQLite (por el ID de Twitter)
        public bool existeSeguidor(long idSeguidor, ref string resultado)
        {
            string consultaSQL =
                "select count(*) from seguidores where id = " +
                Convert.ToString(idSeguidor);
            SQLiteCommand comandoSQL =
                new SQLiteCommand(consultaSQL, conexionSQLite);
            comandoSQL.CommandType = CommandType.Text;
            try
            {
                long numRegistros = (long)comandoSQL.ExecuteScalar();
                comandoSQL.Dispose();
                resultado = System.DateTime.Now + " " +
                    "Ejecutada consulta SQL en SQLite para existe seguidor: " +
                        Convert.ToString(numRegistros);
                return (numRegistros > 0);
            }
            catch (Exception error)
            {
                resultado = System.DateTime.Now + " " +
                    "Error SQLite existe seguidor: " + error.Message;
                return false;
            }
        }

        //insertar nuevo seguidor en BD SQLite
        public void insertarTweet(string tweet, ref string resultado, string idCuenta)
        {
            string consultaSQL =
                "insert into tweets (id,tweet,status,fecha,state,idCuenta) values ((select count(*) from tweets),@tweet,@status,@fecha,@state,@idCuenta)";
            try
            {

                SQLiteParameter parametroTweet = new SQLiteParameter();
                parametroTweet.ParameterName = "@tweet";
                parametroTweet.DbType = DbType.String;
                parametroTweet.Value = Convert.ToString(tweet);

                SQLiteParameter parametroStatus = new SQLiteParameter();
                parametroStatus.ParameterName = "@status";
                parametroStatus.DbType = DbType.Int32;
                parametroStatus.Value = Convert.ToInt32("1");

                SQLiteParameter parametroFecha = new SQLiteParameter();
                parametroFecha.ParameterName = "@fecha";
                parametroFecha.DbType = DbType.DateTime;
                parametroFecha.Value = DateTime.Now;

                SQLiteParameter parametroState = new SQLiteParameter();
                parametroState.ParameterName = "@state";
                parametroState.DbType = DbType.Int32;
                parametroState.Value = Convert.ToInt32("1");

                SQLiteParameter parametroidCuenta = new SQLiteParameter();
                parametroidCuenta.ParameterName = "@idCuenta";
                parametroidCuenta.DbType = DbType.String;
                parametroidCuenta.Value = Convert.ToString(idCuenta);


                SQLiteCommand comandoSQL =
                     new SQLiteCommand(consultaSQL, conexionSQLite);
                comandoSQL.CommandType = CommandType.Text;
                comandoSQL.Parameters.Add(parametroTweet);
                comandoSQL.Parameters.Add(parametroStatus);
                comandoSQL.Parameters.Add(parametroFecha);
                comandoSQL.Parameters.Add(parametroState);
                comandoSQL.Parameters.Add(parametroidCuenta);
                comandoSQL.ExecuteNonQuery();
                resultado = System.DateTime.Now + " " +
                    "Insertado nuevo tweet en BD SQLite [" +
                    Convert.ToString(tweet) + "]";
            }
            catch (Exception error)
            {
                resultado = System.DateTime.Now + " " +
                    "Error SQLite insertar tweet: " +
                    error.Message;
            }
        }

        //insertar nuevo seguidor en BD SQLite
        public void insertarSeguidor(long idSeguidor, ref string resultado)
        {
            string consultaSQL =
                "insert into seguidores (id, fecha) values (@idSeguidor,  @fecha)";
            try
            {
                SQLiteParameter parametroFecha = new SQLiteParameter();
                parametroFecha.ParameterName = "@fecha";
                parametroFecha.DbType = DbType.DateTime;
                parametroFecha.Value = System.DateTime.Now;
                SQLiteParameter parametroidSeguidor = new SQLiteParameter();
                parametroidSeguidor.ParameterName = "@idSeguidor";
                parametroidSeguidor.DbType = DbType.String;
                parametroidSeguidor.Value = Convert.ToString(idSeguidor);
                SQLiteCommand comandoSQL =
                    new SQLiteCommand(consultaSQL, conexionSQLite);
                comandoSQL.CommandType = CommandType.Text;
                comandoSQL.Parameters.Add(parametroFecha);
                comandoSQL.Parameters.Add(parametroidSeguidor);
                comandoSQL.ExecuteNonQuery();
                resultado = System.DateTime.Now + " " +
                    "Insertado nuevo seguidor en BD SQLite [" +
                    Convert.ToString(idSeguidor) + "]";
            }
            catch (Exception error)
            {
                resultado = System.DateTime.Now + " " +
                    "Error SQLite insertar seguidor: " +
                    error.Message;
            }
        }

        public void deleteTweet(int id, ref string resultado)
        {
            string consultaSQL =
                "update tweets set state = 2 where id = @idTweet";
            try
            {
                SQLiteParameter parametroidTweet = new SQLiteParameter();
                parametroidTweet.ParameterName = "@idTweet";
                parametroidTweet.DbType = DbType.Int32;
                parametroidTweet.Value = Convert.ToInt32(id);

                SQLiteCommand comandoSQL =
                            new SQLiteCommand(consultaSQL, conexionSQLite);
                comandoSQL.CommandType = CommandType.Text;
                comandoSQL.Parameters.Add(parametroidTweet);
                comandoSQL.ExecuteNonQuery();
                resultado = System.DateTime.Now + " " +
                                    "Eliminado tweet automatico en BD SQLite [" +
                                    Convert.ToString(id) + "]";
            }
            catch (Exception error)
            {
                resultado = System.DateTime.Now + " " +
                    "Error SQLite eliminar tweet automatico: " +
                    error.Message;
            }
        }

        public void updateTweet(AutomaticMessageModel automaticMessage, ref string resultado)
        {
            string consultaSQL =
                "update tweets set tweet = @tweet where id = @idTweet";
            try
            {
                SQLiteParameter parametroidTweet = new SQLiteParameter();
                parametroidTweet.ParameterName = "@idTweet";
                parametroidTweet.DbType = DbType.Int32;
                parametroidTweet.Value = Convert.ToInt32(automaticMessage.Id);

                SQLiteParameter parametroTweet = new SQLiteParameter();
                parametroTweet.ParameterName = "@tweet";
                parametroTweet.DbType = DbType.String;
                parametroTweet.Value = Convert.ToString(automaticMessage.Tweet);

                SQLiteCommand comandoSQL =
                            new SQLiteCommand(consultaSQL, conexionSQLite);
                comandoSQL.CommandType = CommandType.Text;
                comandoSQL.Parameters.Add(parametroidTweet);
                comandoSQL.Parameters.Add(parametroTweet);
                comandoSQL.ExecuteNonQuery();
                resultado = System.DateTime.Now + " " +
                                    "Actualizado tweet automatico en BD SQLite [" +
                                    Convert.ToString(automaticMessage.Id) + "]";
            }
            catch (Exception error)
            {
                resultado = System.DateTime.Now + " " +
                    "Error SQLite eliminar tweet automatico: " +
                    error.Message;
            }
        }

        public void updateStatus(AutomaticMessageModel automaticMessage, ref string resultado)
        {
            string consultaSQL =
                "update tweets set status = @status where id = @idTweet";
            try
            {
                SQLiteParameter parametroidTweet = new SQLiteParameter();
                parametroidTweet.ParameterName = "@idTweet";
                parametroidTweet.DbType = DbType.Int32;
                parametroidTweet.Value = Convert.ToInt32(automaticMessage.Id);

                SQLiteParameter parametroTweet = new SQLiteParameter();
                parametroTweet.ParameterName = "@status";
                parametroTweet.DbType = DbType.Int32;
                parametroTweet.Value = Convert.ToInt32(automaticMessage.Status);

                SQLiteCommand comandoSQL =
                            new SQLiteCommand(consultaSQL, conexionSQLite);
                comandoSQL.CommandType = CommandType.Text;
                comandoSQL.Parameters.Add(parametroidTweet);
                comandoSQL.Parameters.Add(parametroTweet);
                comandoSQL.ExecuteNonQuery();
                resultado = System.DateTime.Now + " " +
                                    "Actualizado tweet automatico en BD SQLite [" +
                                    Convert.ToString(automaticMessage.Id) + "]";
            }
            catch (Exception error)
            {
                resultado = System.DateTime.Now + " " +
                    "Error SQLite eliminar tweet automatico: " +
                    error.Message;
            }
        }

        public void obtenerCuentas(ref string resultado, ref List<Cuentas> list)
        {
            List<Cuentas> cuentas = new List<Cuentas>();
            string consultaSQL =
                "select * from cuentas where state = 1";
            try
            {
                SQLiteCommand comandoSQL =
                    new SQLiteCommand(consultaSQL, conexionSQLite);
                comandoSQL.CommandType = CommandType.Text;
                SQLiteDataReader reader = comandoSQL.ExecuteReader();
                while (reader.Read())
                {
                    Cuentas cuenta = new Cuentas();
                    cuenta.Id = Convert.ToInt32(reader["id"]); 
                    cuenta.Username = Convert.ToString(reader["usuario"]);
                    cuenta.Password = Convert.ToString(reader["password"]);
                    cuenta.Fecha = Convert.ToDateTime(reader["fecha"]);
                    cuenta.State = Convert.ToInt32(reader["state"]);
                    cuentas.Add(cuenta);
                }
                resultado = System.DateTime.Now + " " +
                    "Cuentas automatizados extraidos exitosamente";
                list = cuentas;
            }
            catch (Exception error)
            {
                resultado = System.DateTime.Now + " " +
                    "Error SQLite extraer cuentas automatizados: " +
                    error.Message;
            }

        }

        //insertar nuevo seguidor en BD SQLite
        public void obtenerTweetsAutomatizados(ref string resultado, ref List<AutomaticMessageModel> list, string idCuenta)
        {
            List<AutomaticMessageModel> messages = new List<AutomaticMessageModel>();
            string consultaSQL =
                "select * from tweets where state = 1 and idCuenta = @idCuenta";
            try
            {
                SQLiteParameter parametroidCuenta = new SQLiteParameter();
                parametroidCuenta.ParameterName = "@idCuenta";
                parametroidCuenta.DbType = DbType.String;
                parametroidCuenta.Value = Convert.ToString(idCuenta);

                SQLiteCommand comandoSQL =
                    new SQLiteCommand(consultaSQL, conexionSQLite);
                comandoSQL.CommandType = CommandType.Text;
                comandoSQL.Parameters.Add(parametroidCuenta);
                SQLiteDataReader reader = comandoSQL.ExecuteReader();
                while (reader.Read())
                {
                    AutomaticMessageModel message = new AutomaticMessageModel();
                    message.Id = Convert.ToInt32(reader["id"]);
                    message.Tweet = Convert.ToString(reader["tweet"]);
                    message.Status = Convert.ToInt32(reader["status"]);
                    message.Fecha = Convert.ToDateTime(reader["fecha"]);
                    message.State = Convert.ToInt32(reader["state"]);
                    messages.Add(message);
                }
                resultado = System.DateTime.Now + " " +
                    "Mensajes automatizados extraidos exitosamente";
                list = messages;
            }
            catch (Exception error)
            {
                resultado = System.DateTime.Now + " " +
                    "Error SQLite extraer mensajes automatizados: " +
                    error.Message;
            }
        }

        //insertar nuevo seguidor en BD SQLite
        public void insertarCuenta(ref Dictionary<int, TareasTwitter> tareasTwitter, Cuentas cuenta, ref string resultado)
        {
            string consultaSQL =
                "insert into cuentas (id,usuario,password,fecha,state) values ((select count(*) from cuentas),@usuario,@password,@fecha,@state)";
            try
            {

                SQLiteParameter parametroUsuario = new SQLiteParameter();
                parametroUsuario.ParameterName = "@usuario";
                parametroUsuario.DbType = DbType.String;
                parametroUsuario.Value = Convert.ToString(cuenta.Username);

                SQLiteParameter parametroPassword = new SQLiteParameter();
                parametroPassword.ParameterName = "@password";
                parametroPassword.DbType = DbType.String;
                parametroPassword.Value = Convert.ToString(cuenta.Password);

                SQLiteParameter parameterFecha = new SQLiteParameter();
                parameterFecha.ParameterName = "@fecha";
                parameterFecha.DbType = DbType.DateTime;
                parameterFecha.Value = DateTime.Now;

                SQLiteParameter parametroState = new SQLiteParameter();
                parametroState.ParameterName = "@state";
                parametroState.DbType = DbType.Int32;
                parametroState.Value = Convert.ToInt32("1");

                SQLiteCommand comandoSQL =
                     new SQLiteCommand(consultaSQL, conexionSQLite);
                comandoSQL.CommandType = CommandType.Text;
                comandoSQL.Parameters.Add(parametroUsuario);
                comandoSQL.Parameters.Add(parametroPassword);
                comandoSQL.Parameters.Add(parameterFecha);
                comandoSQL.Parameters.Add(parametroState);
                comandoSQL.ExecuteNonQuery();

                comandoSQL.CommandText = "select last_insert_rowid()";
                Int64 LastRowID64 = (Int64)comandoSQL.ExecuteScalar();
                int LastRowID = (int)LastRowID64;
                tareasTwitter.Add(LastRowID, new TareasTwitter());
                resultado = System.DateTime.Now + " " +
                    "Insertado nuevo tweet en BD SQLite [" +
                    cuenta.Username + "]";
            }
            catch (Exception error)
            {
                resultado = System.DateTime.Now + " " +
                    "Error SQLite insertar tweet: " +
                    error.Message;
            }
        }

        public void deleteCuenta(int id, ref string resultado)
        {
            string consultaSQL =
                "update cuentas set state = 2 where id = @idCuenta";
            try
            {
                SQLiteParameter parameteridCuenta = new SQLiteParameter();
                parameteridCuenta.ParameterName = "@idCuenta";
                parameteridCuenta.DbType = DbType.Int32;
                parameteridCuenta.Value = Convert.ToInt32(id);

                SQLiteCommand comandoSQL =
                            new SQLiteCommand(consultaSQL, conexionSQLite);
                comandoSQL.CommandType = CommandType.Text;
                comandoSQL.Parameters.Add(parameteridCuenta);
                comandoSQL.ExecuteNonQuery();
                resultado = System.DateTime.Now + " " +
                                    "Eliminado tweet automatico en BD SQLite [" +
                                    Convert.ToString(id) + "]";
            }
            catch (Exception error)
            {
                resultado = System.DateTime.Now + " " +
                    "Error SQLite eliminar tweet automatico: " +
                    error.Message;
            }
        }

        public void updateCuenta(Cuentas cuenta, ref string resultado)
        {
            string consultaSQL =
                "update cuentas set usuario = @usuario, password = @password where id = @idCuenta";
            try
            {
                SQLiteParameter parametroidCuenta = new SQLiteParameter();
                parametroidCuenta.ParameterName = "@idCuenta";
                parametroidCuenta.DbType = DbType.Int32;
                parametroidCuenta.Value = Convert.ToInt32(cuenta.Id);

                SQLiteParameter parametroUsuario = new SQLiteParameter();
                parametroUsuario.ParameterName = "@usuario";
                parametroUsuario.DbType = DbType.String;
                parametroUsuario.Value = Convert.ToString(cuenta.Username);

                SQLiteParameter parametroPassword = new SQLiteParameter();
                parametroPassword.ParameterName = "@password";
                parametroPassword.DbType = DbType.String;
                parametroPassword.Value = Convert.ToString(cuenta.Password);
                
                SQLiteCommand comandoSQL =
                            new SQLiteCommand(consultaSQL, conexionSQLite);
                comandoSQL.CommandType = CommandType.Text;
                comandoSQL.Parameters.Add(parametroidCuenta);
                comandoSQL.Parameters.Add(parametroUsuario);
                comandoSQL.Parameters.Add(parametroPassword);
                comandoSQL.ExecuteNonQuery();
                resultado = System.DateTime.Now + " " +
                                    "Actualizado cuenta automatico en BD SQLite [" +
                                    Convert.ToString(cuenta.Id) + "]";
            }
            catch (Exception error)
            {
                resultado = System.DateTime.Now + " " +
                    "Error SQLite actualizar cuenta automatico: " +
                    error.Message;
            }
        }


    }
}
