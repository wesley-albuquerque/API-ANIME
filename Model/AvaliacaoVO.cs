 using MySql.Data.MySqlClient;
using System.Data;

namespace API_ANIME.Model
{
    public class AvaliacaoVO
    {
        public int? nome_anime_id { get; set; }
        public int? autor_id { get; set; }
        public int? genero_id { get; set; }
        public int? temporada_aval { get; set; }
        public int? episodios { get; set; }
        public int? temporadas { get; set; }
        public DateTime? lancamento_anime { get; set; }
        public int? status_anime_id { get; set; }
        public int? origem_id { get; set; }
        public int? volumes { get; set; }
        public int? lancamento_origem { get; set; }
        public int? status_origem_id { get; set; }
        public int? ovas { get; set; }
        public int? filmes { get; set; }
        public string? sinopse { get; set; }
        public string? resumo { get; set; }
        public int? enredo { get; set; }
        public int? enrolacao { get; set; }
        public int? animacao { get; set; }
        public int? desenvolvimento { get; set; }
        public string? critica { get; set; }
        public bool dropado { get; set; }
        public decimal? nota_final { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_mod { get; set; }

        public AvaliacaoVO()
        {
            data_criacao = DateTime.Now;
            data_mod = DateTime.Now;
        }
    }

    public partial class AvaliacaoResponseObject
    {
        public int nome_anime_id { get; set; }
        public int? temporada_aval { get; set; }
        public bool dropado { get; set; }
        public string? nome_anime { get; set; }
    }

    public partial class AvaliacaoResponseConsulta
    {
        public int? id { get; set; }
        public int? nome_anime_id { get; set; }
        public string? nome_anime { get; set; }
        public int? autor_id { get; set; }
        public string? autor { get; set; }
        public int? genero_id { get; set; }
        public string? genero { get; set; }
        public string? lancamento_anime { get; set; }
        public int? temporada_aval { get; set; }
        public bool dropado { get; set; }
        public int? episodios { get; set; }
        public int? temporadas { get; set; }
        public int? status_anime_id { get; set; }
        public int? origem_id { get; set; }
        public string origem { get; set; }
        public int? volumes { get; set; }
        public int? lancamento_origem { get; set; }
        public int? status_origem_id { get; set; }
        public int? ovas { get; set; }
        public int? filmes { get; set; }
        public string? sinopse { get; set; }
        public string? resumo { get; set; }
        public int? enredo { get; set; }
        public int? enrolacao { get; set; }
        public int? animacao { get; set; }
        public int? desenvolvimento { get; set; }
        public decimal? nota_final { get; set; }
        public string? critica { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_mod { get; set; }

    }
    public static class DataReaderExtensions
    {
        public static bool HasColumn(this IDataRecord dr, string columnName)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (dr.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}
