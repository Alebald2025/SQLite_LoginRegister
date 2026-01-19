using System;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    private string dbPath; // Ruta al archivo de base de datos

    void Start()
    {
        // Define la ruta al archivo .db (en persistentDataPath para que persista entre ejecuciones)
        dbPath = "URI=file:" + Application.persistentDataPath + "/users.db";
        CreateDatabaseIfNotExists(); // Crea la DB y tabla si no existe
    }

    // Método para crear la DB y la tabla de usuarios
    private void CreateDatabaseIfNotExists()
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                // Crea tabla Users con username (único) y password (simple, sin hash por ahora)
                command.CommandText = "CREATE TABLE IF NOT EXISTS Users (Username TEXT PRIMARY KEY, Password TEXT)";
                command.ExecuteNonQuery();
            }
        }
        Debug.Log("Base de datos creada en: " + dbPath);
    }

    // Método para registrar un nuevo usuario
    public bool RegisterUser(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("Username o password vacíos");
            return false;
        }

        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                // Verifica si el usuario ya existe
                command.CommandText = "SELECT COUNT(*) FROM Users WHERE Username = @username";
                command.Parameters.AddWithValue("@username", username);
                int count = Convert.ToInt32(command.ExecuteScalar());

                if (count > 0)
                {
                    Debug.Log("Usuario ya existe");
                    return false;
                }

                // Inserta el nuevo usuario
                command.CommandText = "INSERT INTO Users (Username, Password) VALUES (@username, @password)";
                command.Parameters.AddWithValue("@password", password); // En producción, hashea la password (ej. con SHA256)
                command.ExecuteNonQuery();
                Debug.Log("Usuario registrado: " + username);
                return true;
            }
        }
    }

    // Método para login
    public bool LoginUser(string username, string password)
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT COUNT(*) FROM Users WHERE Username = @username AND Password = @password";
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);
                int count = Convert.ToInt32(command.ExecuteScalar());

                if (count > 0)
                {
                    Debug.Log("Login exitoso: " + username);
                    return true;
                }
                else
                {
                    Debug.Log("Login fallido");
                    return false;
                }
            }
        }
    }
}