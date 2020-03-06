using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using APICasadeshow.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace APICasadeshow.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        public DbSet<CasaDeShow> CasaDeShow { get; set; }
        public DbSet<Genero> Genero { get; set; }
        public DbSet<Evento> Evento { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Compra> Compra { get; set; }
    }
}