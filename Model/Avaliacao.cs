using API_ANIME.Controllers;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace API_ANIME.Model
{
    public class Avaliacao
    {
        public MySqlConnection Conexao { get; set; }
        public MySqlCommand Cmd { get; set; }
        public string Sql { get; set; }

        public Avaliacao(MySqlConnection conexao)
        {
            Conexao= conexao;
        }

        public string BuscaAvalDropado(AvaliacaoVO avaliacao)
        {
            Conexao.Open();
            Sql = $"SELECT nome_anime_id, temporada_aval, dropado, animes_nome_anime.nome as nome_anime FROM animes_avaliacao " +
                  $"INNER JOIN animes_nome_anime ON animes_avaliacao.nome_anime_id = animes_nome_anime.id " +
                  $"WHERE nome_anime_id = @nome_anime_id AND dropado = true";
            Cmd = new MySqlCommand(Sql, Conexao);
            Cmd.Parameters.AddWithValue("@nome_anime_id", avaliacao.nome_anime_id);

            MySqlDataReader registros = Cmd.ExecuteReader();
            string nomeAnime = registros.Read() ? registros["nome_anime"].ToString() : null;
            Conexao.Close();

            return nomeAnime;
        }
        public AvaliacaoResponseObject BuscarAvalPorMaiorTempAval(AvaliacaoVO avaliacao)
        {
            Conexao.Open();
            Sql = $"SELECT nome_anime_id, MAX(temporada_aval) as max_temporada_aval, animes_nome_anime.nome as nome_anime FROM animes_avaliacao " +
                  $"INNER JOIN animes_nome_anime ON animes_avaliacao.nome_anime_id = animes_nome_anime.id " +
                  $"WHERE nome_anime_id = @nome_anime_id "+
                  $"GROUP BY nome_anime_id, animes_nome_anime.nome "+
                  $"ORDER BY max_temporada_aval DESC "+
                  $"LIMIT 1";
            Cmd = new MySqlCommand(Sql, Conexao);
            Cmd.Parameters.AddWithValue("@nome_anime_id", avaliacao.nome_anime_id);

            MySqlDataReader registros = Cmd.ExecuteReader();
            AvaliacaoResponseObject responseAval = new AvaliacaoResponseObject();
            if (registros.Read())
            {
                responseAval.nome_anime = registros["nome_anime"]?.ToString();
                responseAval.temporada_aval = Convert.ToInt32(registros["max_temporada_aval"]);
                if(responseAval.temporada_aval <= avaliacao.temporada_aval)
                {
                    responseAval = null;
                }
            }
            else
            {
                responseAval = null;
            }

            Conexao.Close();

            return responseAval;
        }
      


    }
}
