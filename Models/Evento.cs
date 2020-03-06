using System;

namespace APICasadeshow.Models
{
    public class Evento
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
}