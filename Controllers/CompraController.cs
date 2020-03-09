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
    [Authorize]
    public class CompraController : ControllerBase
    {
        private readonly Data.ApplicationDbContext database;
        private Hateoas.Hateoas Hateoas;

        public CompraController(ApplicationDbContext database)
        {
            this.database = database;
            Hateoas = new Hateoas.Hateoas("localhost:5001/casadeshow/v1/Compra");
            Hateoas.AddAction("GET_INFO", "GET");
            Hateoas.AddAction("DELETE_PRODUCT", "DELETE");
            Hateoas.AddAction("EDIT_PRODUCT", "PATCH");
        }

        ///<summary>
        /// Listar todas as compras.
        ///</summary>
        [HttpGet]
        public IActionResult Get()
        {
            if (database.Compra.Count() == 0)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new { msg = "Não há compras efetuadas!" });
            }

            database.Evento.ToList();
            database.CasaDeShow.ToList();
            database.Genero.ToList();
            var compras = database.Compra.ToList();

            List<CompraContainer> comprasHateoas = new List<CompraContainer>();

            foreach (var comp in compras)
            {
                CompraContainer compraHateoas = new CompraContainer();
                compraHateoas.compra = comp;
                compraHateoas.links = Hateoas.GetActions(comp.CompraId.ToString());
                comprasHateoas.Add(compraHateoas);
            }
            return Ok(comprasHateoas);
        }

        ///<summary>
        /// Procura uma compra por ID.
        ///</summary>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                Compra compra = database.Compra.First(x => x.CompraId == id);
                database.Evento.ToList();
                database.CasaDeShow.ToList();
                database.Genero.ToList();
                CompraContainer compraHateoas = new CompraContainer();
                compraHateoas.compra = compra;
                compraHateoas.links = Hateoas.GetActions(compra.CompraId.ToString());
                return Ok(compraHateoas);
            }
            catch (Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("Compra não encontrada!");
            }
        }

        ///<summary>
        /// Efetua uma compra.
        ///</summary>
        [HttpPost]
        public IActionResult Post([FromBody] CompraTemp compraTemp)
        {
            if (database.Evento.Count() == 0)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new { msg = "Não há eventos cadastrados!" });
            }

            database.Evento.ToList();
            database.CasaDeShow.ToList();
            database.Genero.ToList();

            try
            {
                // validation

                Compra c = new Compra();
                c.Data = DateTime.Now;
                c.QtdIngressos = compraTemp.QtdIngressos;
                c.Evento = database.Evento.First(x => x.EventoId == compraTemp.Evento.EventoId);
                c.Total = (compraTemp.QtdIngressos * c.Evento.PrecoIngresso);

                var userMoment = HttpContext.User.Claims.First(claim => claim.Type.ToString().Equals("Usuarioid", StringComparison.InvariantCultureIgnoreCase)).Value;
                c.Usuario = database.Usuario.First(x => x.UsuarioId == int.Parse(userMoment));

                var zea = database.Evento.First(c => c.EventoId == compraTemp.Evento.EventoId);
                zea.QtdIngresso -= compraTemp.QtdIngressos;
                database.Compra.Add(c);
                database.SaveChanges();

                Response.StatusCode = 201;
                return new ObjectResult("Compra realizada com sucesso!");
            }
            catch (Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("Compra inválida!");
            }
        }
    }

    public class CompraTemp
    {
        public int QtdIngressos { get; set; }
        public Evento Evento { get; set; }
    }

    public class CompraContainer
    {
        public Compra compra { get; set; }
        public Link[] links { get; set; }
    }
}