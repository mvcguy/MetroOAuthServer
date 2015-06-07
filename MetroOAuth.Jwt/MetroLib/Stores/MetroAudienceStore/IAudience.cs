namespace MetroOAuth.Jwt.MetroLib.Stores.MetroAudienceStore
{
    public interface IAudience<out TKey>
    {
        TKey Id { get; }

        string Base64Key { get; set; }
    }
}