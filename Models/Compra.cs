using System;
using Microsoft.AspNetCore.Identity;

namespace APICasadeshow.Models
{
    public class Compra
    {
        public int CompraId {get; set;}
        public DateTime Data {get; set;}
        public int QtdIngressos {get; set;}
        public float Total {get; set;}
        public Evento Evento {get; set;}
        public IdentityUser IdentityUser {get; set;}
    }
}