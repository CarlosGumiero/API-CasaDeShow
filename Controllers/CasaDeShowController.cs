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
    public class CasaDeShowController : ControllerBase
    {
        private readonly Data.ApplicationDbContext database;
        private Hateoas.Hateoas Hateoas;

        public CasaDeShowController(ApplicationDbContext database)
        {
            this.database = database;
            Hateoas = new Hateoas.Hateoas("localhost:5001/APIcasadeshow/v1/CasaDeShow");
            Hateoas.AddAction("GET_INFO", "GET");
            Hateoas.AddAction("DELETE_PRODUCT", "DELETE");
            Hateoas.AddAction("EDIT_PRODUCT", "PATCH");
        }

        ///<summary>
        /// Listar todas as casas.
        ///</summary>
        [HttpGet]
        public IActionResult Get()
        {
            var casadeshows = database.CasaDeShow.ToList();
            List<CasaDeShowContainer> casadeshowsHateoas = new List<CasaDeShowContainer>();
            foreach (var casa in casadeshows)
            {
                CasaDeShowContainer casaHateoas = new CasaDeShowContainer();
                casaHateoas.casadeshow = casa;
                casaHateoas.links = Hateoas.GetActions(casa.CasaDeShowId.ToString());
                casadeshowsHateoas.Add(casaHateoas);
            }
            return Ok(casadeshowsHateoas);
        }

        ///<summary>
        /// Listar casa de show por ID.
        ///</summary>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                CasaDeShow casadeshow = database.CasaDeShow.First(x => x.CasaDeShowId == id);
                CasaDeShowContainer casadeshowHateoas = new CasaDeShowContainer();
                casadeshowHateoas.casadeshow = casadeshow;
                casadeshowHateoas.links = Hateoas.GetActions(casadeshow.CasaDeShowId.ToString());
                return Ok(casadeshowHateoas);
            }
            catch (Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("");
            }
        }

        ///<summary>
        /// Criar casa de show.
        ///</summary>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Post([FromBody] CasaTemp ctemp)
        {
            try
            {
                // validation
                if (ctemp.Nome.Length <= 3)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new { msg = "Nome precisa de mais de 3 caracteres." });
                }

                if (ctemp.Endereco.Length <= 3)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new { msg = "Endereço precisa de mais de 3 caracteres." });
                }

                CasaDeShow c = new CasaDeShow();

                c.Nome = ctemp.Nome;
                c.Endereco = ctemp.Endereco;
                database.CasaDeShow.Add(c);
                database.SaveChanges();

                Response.StatusCode = 201;
                return new ObjectResult("Casa de show criada!");
            }
            catch (Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("Casa de show inválida!");
            }
        }


        /// <summary>
        /// Remove uma casa de show por ID.
        /// </summary> 
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult Delete(int id)
        {
            try
            {
                CasaDeShow casadeshow = database.CasaDeShow.First(x => x.CasaDeShowId == id);
                database.CasaDeShow.Remove(casadeshow);
                database.SaveChanges();

                return Ok();
            }
            catch (Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("Casa de show inválida");
            }
        }

        ///<summary>
        /// Editar uma casa de show por ID.
        ///</summary>
        [HttpPatch]
        [Authorize(Roles = "admin")]
        public IActionResult Patch([FromBody] CasaDeShow casadeshow)
        {
            if (casadeshow.CasaDeShowId > 0)
            {
                try
                {
                    var c = database.CasaDeShow.First(x => x.CasaDeShowId == casadeshow.CasaDeShowId);

                    if (c != null)
                    {
                        if (casadeshow.Nome.Length <= 3)
                        {
                            Response.StatusCode = 400;
                            return new ObjectResult(new { msg = "Nome precisa de mais de 3 caracteres." });
                        }

                        if (casadeshow.Endereco.Length <= 3)
                        {
                            Response.StatusCode = 400;
                            return new ObjectResult(new { msg = "Endereço precisa de mais de 3 caracteres." });
                        }
                        //Editar
                        c.Nome = casadeshow.Nome != null ? casadeshow.Nome : c.Nome;
                        c.Endereco = casadeshow.Endereco != null ? casadeshow.Endereco : c.Endereco;

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

        ///<summary>
        /// Listar todas as casas em ordem crescente.
        ///</summary>
        [HttpGet("asc")]
        public IActionResult Asc()
        {
            var casadeshow = database.CasaDeShow.ToList();
            return Ok(casadeshow.OrderBy(x => x.Nome));
        }

        ///<summary>
        /// Listar todas as casas em ordem decrescente.
        ///</summary>
        [HttpGet("desc")]
        public IActionResult Desc()
        {
            var casadeshow = database.CasaDeShow.ToList();
            return Ok(casadeshow.OrderByDescending(x => x.Nome));
        }

        ///<summary>
        /// Listar uma casa passando um nome.
        ///</summary>
        [HttpGet("nome/" + "{nome}")]
        public IActionResult Nome(string nomecasa)
        {
            try
            {
                CasaDeShow casadeshow = database.CasaDeShow.First(x => x.Nome == nomecasa);
            }
            catch (Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("Compra não encontrada!");
            }

            return Ok(nomecasa);
        }
    }
    public class CasaTemp
    {
        public int CasaDeShowId { get; set; }
        public string Nome { get; set; }
        public string Endereco { get; set; }
    }

    public class CasaDeShowContainer
    {
        public CasaDeShow casadeshow { get; set; }
        public Link[] links { get; set; }
    }
}