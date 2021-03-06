using System;
using System.Linq;
using APICasadeshow.Data;
using APICasadeshow.Models;
using Microsoft.AspNetCore.Mvc;
using APICasadeshow.Hateoas;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace APICasadeshow.Controllers
{
    [Route("apicasadeshow/v1/[controller]")]
    [ApiController]
    public class GeneroController : ControllerBase
    {
        private readonly Data.ApplicationDbContext database;
        private Hateoas.Hateoas Hateoas;

        public GeneroController(ApplicationDbContext database)
        {
            this.database = database;
            Hateoas = new Hateoas.Hateoas("localhost:5001/casadeshow/v1/Genero");
            Hateoas.AddAction("GET_INFO", "GET");
            Hateoas.AddAction("DELETE_PRODUCT", "DELETE");
            Hateoas.AddAction("EDIT_PRODUCT", "PATCH");

        }

        ///<summary>
        /// Listar todos os gêneros.
        ///</summary>
        [HttpGet]
        public IActionResult Get()
        {
            var genero = database.Genero.ToList();
            List<GeneroContainer> generosHateoas = new List<GeneroContainer>();
            foreach (var gen in genero)
            {
                GeneroContainer generoHateoas = new GeneroContainer();
                generoHateoas.genero = gen;
                generoHateoas.links = Hateoas.GetActions(gen.GeneroId.ToString());
                generosHateoas.Add(generoHateoas);
            }
            return Ok(generosHateoas);
        }

        ///<summary>
        /// Lista gênero por ID.
        ///</summary>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                Genero genero = database.Genero.First(x => x.GeneroId == id);
                GeneroContainer generoHateoas = new GeneroContainer();
                generoHateoas.genero = genero;
                generoHateoas.links = Hateoas.GetActions(genero.GeneroId.ToString());
                return Ok(generoHateoas);
            }
            catch (Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("");
            }
        }

        ///<summary>
        /// Criar gênero.
        ///</summary>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Post([FromBody] GeneroTemp gtemp)
        {
            // validation
            if (gtemp.Nome.Length <= 3)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new { msg = "Nome precisa de mais de 3 caracteres." });
            }

            Genero g = new Genero();

            g.Nome = gtemp.Nome;
            database.Genero.Add(g);
            database.SaveChanges();

            Response.StatusCode = 201;
            return new ObjectResult("Gênero criado com sucesso!");
        }

        ///<summary>
        /// Excluir gênero por ID.
        ///</summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult Delete(int id)
        {
            try
            {
                Genero genero = database.Genero.First(x => x.GeneroId == id);
                database.Genero.Remove(genero);
                database.SaveChanges();
                return Ok();
            }
            catch (Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("Gênero não encontrado!");
            }
        }

        ///<summary>
        /// Editar gênero por ID.
        ///</summary>
        [HttpPatch]
        [Authorize(Roles = "admin")]
        public IActionResult Patch([FromBody] Genero genero)
        {
            if (genero.GeneroId > 0)
            {
                try
                {
                    var g = database.Genero.First(x => x.GeneroId == genero.GeneroId);

                    if (g != null)
                    {
                        if (genero.Nome.Length <= 3)
                        {
                            Response.StatusCode = 400;
                            return new ObjectResult(new { msg = "Nome precisa de mais de 3 caracteres." });
                        }

                        //Editar
                        g.Nome = genero.Nome != null ? genero.Nome : g.Nome;

                        database.SaveChanges();
                        return Ok();
                    }
                    else
                    {
                        Response.StatusCode = 400;
                        return new ObjectResult(new { msg = "Produto não encontrado" });
                    }
                }
                catch
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new { msg = "Produto não encontrado" });
                }
            }
            else
            {
                Response.StatusCode = 400;
                return new ObjectResult(new { msg = "Id do produto é inválido" });
            }
        }
    }

    public class GeneroTemp
    {
        public int GeneroId { get; set; }
        public string Nome { get; set; }
    }

    public class GeneroContainer
    {
        public Genero genero { get; set; }
        public Link[] links { get; set; }
    }
}