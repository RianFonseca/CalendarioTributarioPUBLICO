
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CalendarioTributario.AutorizacaoEAutentificacao
{
    public class GerarToken
    {
        private readonly IConfiguration _configuration;
        public GerarToken(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            var chaveSecreta = _configuration["JWT:JwtToken"];
            if (string.IsNullOrEmpty(chaveSecreta))
            {
                throw new InvalidOperationException("A configuração 'JWT:JwtToken' não foi encontrada ou está vazia.");
            }
        }
        public string GenerateToken(int id, string login)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JWT:JwtToken"]);
            var audience = _configuration["JWT:Audience"];
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim("id", id.ToString()),
            new Claim("login", login),
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = audience,
                Issuer = _configuration["JWT:Issuer"]
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public string GetExistingToken(string token)
        {
            return token;
        }
    }
}
