using System;

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
    public string password;
    public string email;
    public int actFrag;
    public int bestScore;
    public int monedas;
    public int vidaInicial;
}

[Serializable]
public class GameObjectData
{
    public string id;
    public string nombre;
    public string descripcion;
    public string tipo;
    public int precio;
    public int cantidad;
}
