namespace MetroOAuth.Jwt.MetroLib.Stores.MetroAppStore
{
    public interface IApp<out TKey>
    {
        TKey Id { get; }

        string Name { get; set; }
    }
}