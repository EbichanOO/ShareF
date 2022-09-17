namespace PretiaArCloud
{
    public interface IJwtDecoder
    {
        /// <summary>
        /// Returns the payload of a JWT 
        /// </summary>
        string Decode(string token);
    }
}