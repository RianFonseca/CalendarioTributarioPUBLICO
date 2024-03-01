namespace CalendarioTributario.Model
{
    public class Obrigacoes
    {
        public int Id { get; set; }
        public string? MesObrigacoes { get; set; }
        public string Estado { get; set; }
        public int Ano { get; set; }
        public int Mes { get; set; }
        //public DateTime? Data { get; set; }
        public bool Ativo { get; set; }
    }
}
