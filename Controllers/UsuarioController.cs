using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using APICasadeshow.Data;
using APICasadeshow.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace APICasadeshow.Controllers
{
    [Route("apicasadeshow/v1/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly Data.ApplicationDbContext database;

        public UsuarioController(ApplicationDbContext database)
        {
            this.database = database;
        }

        //apicasadeshow/v1/usuario/registro
        [HttpPost("registro")]
        public IActionResult Registro([FromBody] Usuario usuario)
        {
            //Verificar se as credenciais são válidas ok
            //Verificar se o e-mail já está cadastrado no banco ok
            //Encriptar a senha not ok

            Regex rg = new Regex(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$");

            if (database.Usuario.Any(x => x.Email.Equals(usuario.Email)))
            {
                Response.StatusCode = 400;
                return new ObjectResult("Email já cadastrado!");
            }

            if (usuario.Senha.Length <= 5)
            {
                Response.StatusCode = 400;
                return new ObjectResult("A senha deve ser maior que 6 caracteres!");
            }

            if (rg.IsMatch(usuario.Email))
            {
                database.Add(usuario);
                database.SaveChanges();
                return Ok(new { msg = "Usuário cadastrado com sucesso!" });
            }
            else
            {
                Response.StatusCode = 400;
                return new ObjectResult("Email inválido!");
            }
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] Usuario credenciais)
        {
            //Buscar um usuario por email
            //Verificar se a senha está correta
            //Gerar um token JWT e retornar esse token para o usuário
            try
            {
                Usuario usuario = database.Usuario.First(x => x.Email.Equals(credenciais.Email));

                if (usuario != null)
                {
                    //Achou um usuário com cadastro válido
                    if (usuario.Senha.Equals(credenciais.Senha))
                    {
                        //Usuário acertou a senha : logar
                        string chaveDeSeguranca = "A_barata_da_vizinha_ta_na_minha_cama.";  //Chave de segurança
                        var chaveSimetrica = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveDeSeguranca));
                        var credenciaisDeAcesso = new SigningCredentials(chaveSimetrica, SecurityAlgorithms.HmacSha256Signature);

                        var claims = new List<Claim>();
                        claims.Add(new Claim("usuarioid", usuario.UsuarioId.ToString()));
                        claims.Add(new Claim("email", usuario.Email));

                        if (usuario.Role.ToString().Equals("admin", StringComparison.InvariantCultureIgnoreCase))
                        {
                            claims.Add(new Claim(ClaimTypes.Role, "admin"));
                        }
                        else if (usuario.Role.ToString().Equals("user", StringComparison.InvariantCultureIgnoreCase))
                        {
                            claims.Add(new Claim(ClaimTypes.Role, "user"));
                        }

                        var JWT = new JwtSecurityToken(
                            issuer: "APICasaDeShow.com", // Quem está fornecendo o JWT para o usuário
                            expires: DateTime.Now.AddHours(1),
                            audience: "usuario",
                            signingCredentials: credenciaisDeAcesso,
                            claims: claims
                        );


                        return Ok(new JwtSecurityTokenHandler().WriteToken(JWT));
                    }
                    else
                    {
                        // Não existe nenhum usuário com este email
                        Response.StatusCode = 401; //Não autorizado
                        return new ObjectResult("Usuário/senha inválidos!");
                    }
                }
                else
                {
                    Response.StatusCode = 401;
                    return new ObjectResult("Usuário inválido!");
                }
            }
            catch (Exception)
            {
                Response.StatusCode = 401;
                return new ObjectResult("Usuário inválido!");
            }

        }
    }
}