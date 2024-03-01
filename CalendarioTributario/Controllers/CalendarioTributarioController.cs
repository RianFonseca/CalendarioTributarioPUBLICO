using CalendarioTributario.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Data.SqlClient;

namespace CalendarioTributario.Controllers
{
    public class CalendarioController : ControllerBase
    {
        private readonly string _connString;
        private readonly IMemoryCache _cache;

        public CalendarioController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
            _connString = Environment.GetEnvironmentVariable("SQLServer");
        }

        [HttpGet]
        [Authorize]
        [EnableRateLimiting("Limite")]
        [Route("Calendario")]
        public IActionResult GetEstado(string estado, int ano = 0, int mes = 0)
        {
            if (string.IsNullOrEmpty(estado) || ano == 0 || mes == 0 )
            {
                return BadRequest("Estado, ano e mes são campos obrigatórios.");
            }

            string cacheKey = $"Obrigacoes_{estado}_{ano}_{mes}";

            List<Obrigacoes> result = _cache.Get<List<Obrigacoes>>(cacheKey);

            if (result == null)
            {
                result = QueryDatabase(estado, ano, mes);
                _cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));
            }

            if (result.Any())
            {
                return Ok(result);
            }

            return NotFound();
        }

        private List<Obrigacoes> QueryDatabase(string estado, int ano, int mes)
        {
            if (ano == 0 && mes == 0)
            {
                return (List<Obrigacoes>)GetEstado(estado);
            }
            else if (ano != 0 && mes == 0)
            {
                return GetEstadoAno(estado, ano);
            }
            else
            {
                return GetEstadoMes(estado, ano, mes);
            }
        }

        private List<Obrigacoes> GetEstadoAno(string estado, int ano)
        {
            return GetObrigacoes("SELECT * FROM Calendario.Obrigacoes WHERE estado = @estado AND ano = @ano",
                new SqlParameter("@estado", estado),
                new SqlParameter("@ano", ano));
        }

        private List<Obrigacoes> GetEstadoMes(string estado, int ano, int mes)
        {
            return GetObrigacoes("SELECT * FROM Calendario.Obrigacoes WHERE estado = @estado AND ano = @ano AND mes = @mes",
                new SqlParameter("@estado", estado),
                new SqlParameter("@ano", ano),
                new SqlParameter("@mes", mes));
        }

        private List<Obrigacoes> GetObrigacoes(string sql, params SqlParameter[] parameters)
        {
            List<Obrigacoes> obrigacoesList = new List<Obrigacoes>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddRange(parameters);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Obrigacoes calendario = new Obrigacoes
                                {
                                    Id = (int)reader["id"],
                                    MesObrigacoes = reader["mes_obrigacoes"] as string,
                                    Estado = reader["estado"] as string,
                                    Ano = (int)reader["ano"],
                                    Mes = (int)reader["mes"],
                                    //Data = (DateTime)reader["data"],
                                    Ativo = (bool)reader["ativo"]
                                };

                                obrigacoesList.Add(calendario);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
            }

            return obrigacoesList;
        }
    }
}
