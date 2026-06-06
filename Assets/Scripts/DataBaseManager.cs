using System;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;

public class DatabaseManager : MonoBehaviour
{
    private string dbPath;

    private void Awake()
    {
        dbPath = "URI=file:" + Application.persistentDataPath + "/usuaris.db";
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        try
        {
            using (var conn = new SqliteConnection(dbPath))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Usuaris (
                            UserID      INTEGER PRIMARY KEY AUTOINCREMENT,
                            Username    TEXT    UNIQUE NOT NULL,
                            Password    TEXT    NOT NULL
                        )";
                    cmd.ExecuteNonQuery();
                }
            }
            Debug.Log("Base de dades inicialitzada correctament");
        }
        catch (Exception e)
        {
            Debug.LogError("Error creant base de dades: " + e.Message);
        }
    }

    public string RegisterUser(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username))
            return "El nombre de usuario no puede estar vacio";

        if (password.Length < 8)
            return "La contraseña debe tener minimo 8 caracteres";

        try
        {
            using (var conn = new SqliteConnection(dbPath))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    // Comprobar si existe
                    cmd.CommandText = "SELECT COUNT(*) FROM Usuaris WHERE Username = @user";
                    cmd.Parameters.AddWithValue("@user", username);
                    long count = (long)cmd.ExecuteScalar();

                    if (count > 0)
                        return "Este usuario ya existe";

                    // Registrarse
                    cmd.CommandText = "INSERT INTO Usuaris (Username, Password) VALUES (@user, @pass)";
                    cmd.Parameters.AddWithValue("@user", username);
                    cmd.Parameters.AddWithValue("@pass", password);
                    cmd.ExecuteNonQuery();

                    return "OK";
                }
            }
        }
        catch (SqliteException ex)
        {
            if (ex.Message.Contains("UNIQUE constraint failed"))
                return "Este usuario ya existe";
            return "Error de base de dades: " + ex.Message;
        }
        catch (Exception ex)
        {
            return "Error inesperat: " + ex.Message;
        }
    }

    public (bool success, string message, int userId) LoginUser(string username, string password)
    {
        try
        {
            using (var conn = new SqliteConnection(dbPath))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT UserID FROM Usuaris WHERE Username = @user AND Password = @pass";
                    cmd.Parameters.AddWithValue("@user", username);
                    cmd.Parameters.AddWithValue("@pass", password);

                    var result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        int userId = Convert.ToInt32(result);
                        return (true, "Login correcto", userId);
                    }
                    else
                    {
                        return (false, "Usuario o contraseña incorrectos", -1);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return (false, "Error de connexión: " + ex.Message, -1);
        }
    }
}