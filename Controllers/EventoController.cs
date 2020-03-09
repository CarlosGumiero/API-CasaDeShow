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
    public class EventoController : ControllerBase
    {
        private readonly Data.ApplicationDbContext database;
        private Hateoas.Hateoas Hateoas;

        public EventoController(ApplicationDbContext database)
        {
            this.database = database;
            Hateoas = new Hateoas.Hateoas("localhost:5001/casadeshow/v1/CasaDeShow");
            Hateoas.AddAction("GET_INFO", "GET");
            Hateoas.AddAction("DELETE_PRODUCT", "DELETE");
            Hateoas.AddAction("EDIT_PRODUCT", "PATCH");

        }

        ///<summary>
        /// Listar todas os eventos.
        ///</summary>
        [HttpGet]
        public IActionResult Get()
        {
            if (database.Evento.Count() == 0)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new { msg = "Não há eventos cadastrados!" });
            }

            database.CasaDeShow.ToList();
            database.Genero.ToList();
            var eventos = database.Evento.ToList();

            List<EventoContainer> eventosHateoas = new List<EventoContainer>();

            foreach (var eve in eventos)
            {
                EventoContainer eventoHateoas = new EventoContainer();
                eventoHateoas.evento = eve;
                eventoHateoas.links = Hateoas.GetActions(eve.EventoId.ToString());
                eventosHateoas.Add(eventoHateoas);
            }
            return Ok(eventosHateoas);
        }

        ///<summary>
        /// Listar em evento passando um ID.
        ///</summary>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                Evento evento = database.Evento.First(x => x.EventoId == id);
                database.CasaDeShow.ToList();
                database.Genero.ToList();
                EventoContainer eventoHatoeas = new EventoContainer();
                eventoHatoeas.evento = evento;
                eventoHatoeas.links = Hateoas.GetActions(evento.EventoId.ToString());
                return Ok(eventoHatoeas);
            }
            catch (Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("Evento não encontrado!");
            }
        }

        ///<summary>
        /// Criar um evento.
        ///</summary>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Post([FromBody] EventoTemp eTemp)
        {
            database.CasaDeShow.ToList();
            database.Genero.ToList();
            try
            {
                // validation
                if (database.Genero.Count() == 0)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new { msg = "Não há gêneros cadastrados!" });
                }

                if (database.CasaDeShow.Count() == 0)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new { msg = "Não há casas de show cadastradas!" });
                }

                if (eTemp.PrecoIngresso <= 0)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new { msg = "Preço não pode ser negativo nem 0." });
                }

                if (eTemp.Nome.Length <= 1)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new { msg = "Nome precisa de mais de 1 caracter." });
                }

                if (eTemp.Capacidade <= 0)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new { msg = "Capacidade nao pode ser 0 nem negativa." });
                }

                if (eTemp.PrecoIngresso <= 0)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new { msg = "Preço nao pode ser 0 nem negativo." });
                }

                if (eTemp.CasaDeShow == null)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new { msg = "Casa de show inválida!" });
                }

                if (eTemp.Genero == null)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new { msg = "Gênero inválido!" });
                }

                Evento e = new Evento();

                e.Nome = eTemp.Nome;
                e.Capacidade = eTemp.Capacidade;
                e.PrecoIngresso = eTemp.PrecoIngresso;
                e.Data = eTemp.Data;
                e.QtdIngresso = eTemp.QtdIngresso;
                e.Genero = database.Genero.First(x => x.GeneroId == eTemp.Genero.GeneroId);
                e.CasaDeShow = database.CasaDeShow.First(x => x.CasaDeShowId == eTemp.CasaDeShow.CasaDeShowId);
                e.Foto = eTemp.Foto;
                database.Evento.Add(e);
                database.SaveChanges();

                Response.StatusCode = 201;
                return new ObjectResult("Evento criado com sucesso!");
            }
            catch (Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("Casa de show / Genero inválido!");
            }
        }

        ///<summary>
        /// Excluir um evento passando um ID.
        ///</summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult Delete(int id)
        {
            try
            {
                Evento evento = database.Evento.First(x => x.EventoId == id);
                database.Evento.Remove(evento);
                database.SaveChanges();
                return Ok();
            }
            catch (Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("Evento não encontrado!");
            }
        }

        ///<summary>
        /// Editar um evento passando um ID.
        ///</summary>
        [HttpPatch]
        [Authorize(Roles = "admin")]
        public IActionResult Patch([FromBody] Evento evento)
        {
            if (evento.EventoId > 0)
            {
                try
                {
                    var e = database.Evento.First(x => x.EventoId == evento.EventoId);
                    database.CasaDeShow.ToList();
                    database.Genero.ToList();

                    if (e != null)
                    {
                        if (e.PrecoIngresso <= 0)
                        {
                            Response.StatusCode = 400;
                            return new ObjectResult(new { msg = "Preço não pode ser negativo nem 0." });
                        }

                        if (e.Nome.Length <= 1)
                        {
                            Response.StatusCode = 400;
                            return new ObjectResult(new { msg = "Nome precisa de mais de 1 caracter." });
                        }

                        if (e.Capacidade <= 0)
                        {
                            Response.StatusCode = 400;
                            return new ObjectResult(new { msg = "Capacidade nao pode ser 0 nem negativa." });
                        }

                        if (e.PrecoIngresso <= 0)
                        {
                            Response.StatusCode = 400;
                            return new ObjectResult(new { msg = "Preço nao pode ser 0 nem negativo." });
                        }

                        if (e.CasaDeShow == null)
                        {
                            Response.StatusCode = 400;
                            return new ObjectResult(new { msg = "Casa de show inválida!" });
                        }

                        if (e.Genero == null)
                        {
                            Response.StatusCode = 400;
                            return new ObjectResult(new { msg = "Gênero inválido!" });
                        }
                        //Editar
                        e.Nome = evento.Nome;
                        e.Capacidade = evento.Capacidade;
                        e.PrecoIngresso = evento.PrecoIngresso;
                        e.Data = evento.Data;
                        e.QtdIngresso = evento.QtdIngresso;
                        e.Genero = database.Genero.First(x => x.GeneroId == evento.Genero.GeneroId);
                        e.CasaDeShow = database.CasaDeShow.First(x => x.CasaDeShowId == evento.CasaDeShow.CasaDeShowId);
                        e.Foto = evento.Foto;

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
        /// Listar todos os eventos por capacidade em ordem crescente.
        ///</summary>
        [HttpGet("capacidade/" + "asc")]
        public IActionResult CapacidadeAsc()
        {
            var evento = database.Evento.ToList();
            return Ok(evento.OrderBy(x => x.Capacidade));
        }

        ///<summary>
        /// Listar todos os eventos por capacidade em ordem decrescente.
        ///</summary>
        [HttpGet("capacidade/" + "desc")]
        public IActionResult CapacidadeDesc()
        {
            var evento = database.Evento.ToList();
            return Ok(evento.OrderByDescending(x => x.Capacidade));
        }

        ///<summary>
        /// Listar todos os eventos por data em ordem crescente.
        ///</summary>
        [HttpGet("data/" + "asc")]
        public IActionResult DataAsc()
        {
            var evento = database.Evento.ToList();
            return Ok(evento.OrderBy(x => x.Data));
        }

        ///<summary>
        /// Listar todos os eventos por data em ordem decrescente.
        ///</summary>
        [HttpGet("data/" + "desc")]
        public IActionResult DataDesc()
        {
            var evento = database.Evento.ToList();
            return Ok(evento.OrderByDescending(x => x.Data));
        }

        ///<summary>
        /// Listar todos os eventos por nome em ordem crescente.
        ///</summary>
        [HttpGet("nome/" + "asc")]
        public IActionResult NomeAsc()
        {
            var evento = database.Evento.ToList();
            return Ok(evento.OrderBy(x => x.Nome));
        }

        ///<summary>
        /// Listar todos os eventos por nome em ordem decrescente.
        ///</summary>
        [HttpGet("nome/" + "desc")]
        public IActionResult NomeDesc()
        {
            var evento = database.Evento.ToList();
            return Ok(evento.OrderByDescending(x => x.Nome));
        }

        ///<summary>
        /// Listar todos os eventos por preço em ordem crescente.
        ///</summary>
        [HttpGet("preco/" + "asc")]
        public IActionResult PrecoAsc()
        {
            var evento = database.Evento.ToList();
            return Ok(evento.OrderBy(x => x.PrecoIngresso));
        }

        ///<summary>
        /// Listar todos os eventos por preço em ordem decrescente.
        ///</summary>
        [HttpGet("preco/" + "desc")]
        public IActionResult PrecoDesc()
        {
            var evento = database.Evento.ToList();
            return Ok(evento.OrderByDescending(x => x.PrecoIngresso));
        }
    }

    public class EventoTemp
    {
        public int EventoId { get; set; }
        public string Nome { get; set; }
        public int Capacidade { get; set; }
        public float PrecoIngresso { get; set; }
        public CasaDeShow CasaDeShow { get; set; }
        public DateTime Data { get; set; }
        public int QtdIngresso { get; set; }
        public Genero Genero { get; set; }
        public byte[] Foto { get; set; }

    }

    public class EventoContainer
    {
        public Evento evento { get; set; }
        public Link[] links { get; set; }
    }
}