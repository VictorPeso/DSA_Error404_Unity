using System;

/// <summary>
/// Clases para deserializar el JSON del endpoint /profile/{username}
/// Estructura del JSON del backend:
/// {
///   "user": { "username": "...", "actFrag": 0, "bestScore": 0, "monedas": 100, ... },
///   "objects": [ { "id": "obj01", "nombre": "...", "tipo": "ESPADA", "precio": 100, "cantidad": 1 }, ... ]
/// }
/// </summary>

[Serializable]
public class UnityProfileResponse
{
    public UserData user;
    public GameObjectData[] objects;
}

[Serializable]
public class UserData
{
    public string username;
    public string password;      // No lo usamos pero viene del backend
    public string email;          // No lo usamos pero viene del backend
    public int actFrag;           // ← NIVEL MÁXIMO ALCANZADO (0-5)
    public int bestScore;         // ← MEJOR PUNTUACIÓN
    public int monedas;           // ← BYTES/MONEDAS ACTUALES
    public int vidaInicial;       // No lo usamos
}

[Serializable]
public class GameObjectData
{
    public string id;             // ej: "obj01"
    public string nombre;         // ej: "Antivirus Militar"
    public string descripcion;    // ej: "Software militar..."
    public string tipo;           // ej: "ESPADA", "ESCUDO", "ARMADURA", "CASCO", "POCION"
    public int precio;            // Precio en la tienda
    public int cantidad;          // Cantidad que posee el usuario
}
