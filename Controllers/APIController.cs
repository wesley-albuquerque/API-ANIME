using API_ANIME.Model;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Data;

namespace API_ANIME.Controllers
{
    [ApiController]
    [Route("api-anime")]
    public class APIController : ControllerBase
    {
        private readonly string StrinConnection = @"Server = localhost;
                                                Database=personal_site;
                                                User=wesley;
                                                Password=waa123";
        public MySqlConnection Conexao { get; set; }
        public string Sql { get; set; }
        public MySqlCommand Cmd { get; set; }
        public Avaliacao Avaliacao { get; set; }
        public APIController()
        {
            Conexao = new MySqlConnection(StrinConnection);
        }

        [HttpPost("insere-{tabela}")]
        public IActionResult insereTabelasAuxiliares(string tabela, [FromBody] TabelaAuxiliares tabelaAuxiliares)
        {
            try
            {
                Conexao.Open();
                Sql = $"SELECT * FROM {tabela} WHERE nome = '{tabelaAuxiliares.Nome}'";
                Cmd = new MySqlCommand(Sql, Conexao);
                MySqlDataReader objeto = Cmd.ExecuteReader();
                if (objeto.Read())
                {
                    return BadRequest($"{tabelaAuxiliares.Nome} já cadastrado no id {objeto["id"]}");
                }
                else
                {
                    Conexao.Close();
                    Conexao.Open();
                    Sql = $"INSERT INTO {tabela} (nome) VALUES (@nome)";

                    Cmd = new MySqlCommand(Sql, Conexao);
                    Cmd.Parameters.AddWithValue("@nome", tabelaAuxiliares.Nome);

                    Cmd.ExecuteNonQuery();
                    Conexao.Close();

                    return Ok($"{tabela} cadastrado com sucesso");
                }

            }
            catch (Exception ex)
            {
                Conexao.Close();
                return BadRequest($"Erro ao inserir dados: {ex.Message}");
            }
        }

        [HttpGet("consulta-{tabela}/{id?}")]
        public IActionResult consultaTabelaAuxiliares(string tabela, int? id)
        {
            try
            {
                Conexao.Open();

                if (id.HasValue)
                {
                    Sql = $"SELECT * from {tabela} WHERE id = @id";
                    Cmd = new MySqlCommand(Sql, Conexao);
                    Cmd.Parameters.AddWithValue("@id", id.Value);
                }
                else
                {
                    Sql = $"SELECT * from {tabela}";
                    Cmd = new MySqlCommand(Sql, Conexao);
                }

                MySqlDataReader objeto = Cmd.ExecuteReader();

                List<TabelaAuxiliares> registros = new List<TabelaAuxiliares>();

                while (objeto.Read())
                {
                    TabelaAuxiliares registro = new TabelaAuxiliares
                    {
                        Id = int.Parse(objeto["id"].ToString()),
                        Nome = objeto["nome"].ToString()
                    };
                    registros.Add(registro);
                }
                if (registros.Count > 0)
                {
                    Conexao.Close();
                    string jsonRegistros = JsonConvert.SerializeObject(registros);
                    return Ok(jsonRegistros);
                }
                else
                {
                    Conexao.Close();
                    return BadRequest($"Nenhum {tabela} cadastrado");
                }

            }
            catch (Exception ex)
            {
                Conexao.Close();
                return BadRequest($"Erro ao inserir dados: {ex.Message}");
            }
        }

        [HttpPost("atualiza-{tabela}")]
        public IActionResult atualizaTabelaAuxiliares(string tabela, [FromBody] TabelaAuxiliares tabelaAuxiliares)
        {
            try
            {
                Conexao.Open();
                Sql = $"SELECT * FROM {tabela} WHERE nome = '{tabelaAuxiliares.Nome}'";
                Cmd = new MySqlCommand(Sql, Conexao);
                MySqlDataReader objeto = Cmd.ExecuteReader();
                if (objeto.Read())
                {
                    return BadRequest($"{tabelaAuxiliares.Nome} já cadastrado no id {objeto["id"]}");
                }
                else
                {
                    Conexao.Close();
                    Conexao.Open();
                    Sql = $"UPDATE {tabela} set nome = @nome WHERE id =" + tabelaAuxiliares.Id;
                    Cmd = new MySqlCommand(Sql, Conexao);
                    Cmd.Parameters.AddWithValue("@nome", tabelaAuxiliares.Nome);
                    Cmd.ExecuteNonQuery();
                    Conexao.Close();
                    return Ok($"{tabela} atualizado com sucesso");
                }



            }
            catch (Exception ex)
            {
                return BadRequest("Erro ao atualizar gênero" + ex);
            }
        }

        [HttpDelete("delete-{tabela}/{id}")]
        public IActionResult deleteTabelaAuxiliares(string tabela, int id)
        {
            try
            {
                Conexao.Open();
                Sql = $"DELETE FROM {tabela} WHERE id = {id}";
                Cmd = new MySqlCommand(Sql, Conexao);
                if (Cmd.ExecuteNonQuery() > 0)
                {
                    Conexao.Close();
                    return Ok($"{tabela} excluído com sucesso");
                }
                else
                {
                    Conexao.Close();
                    return BadRequest($"Id({id}) não encontrado no banco de dados");
                }
            }
            catch (Exception ex)
            {
                Conexao.Close();
                return BadRequest($"Erro ao excluir {tabela}: " + ex);
            }
        }

        [HttpPost("insere-animes")]
        public IActionResult InsereNovaAvaliacao([FromBody] AvaliacaoVO avaliacao)
        {
            try
            {
                Avaliacao = new Avaliacao(Conexao);
                var registrosDropado = Avaliacao.BuscaAvalDropado(avaliacao);
                if (registrosDropado != null)
                {
                    return Conflict($"Cadastro não permitido. Anime {registrosDropado} é marcado como dropado!");
                }
                var maiorRegistroPorAval = Avaliacao.BuscarAvalPorMaiorTempAval(avaliacao);
                if (maiorRegistroPorAval != null)
                {
                    return Conflict($"Cadastro não permitido. A última temporada do avaliada do anime {maiorRegistroPorAval.nome_anime} foi a {maiorRegistroPorAval.temporada_aval}°");
                }

                Conexao.Open();
                Sql = $"INSERT INTO animes_avaliacao ({string.Join(", ", typeof(AvaliacaoVO).GetProperties().Select(propriedade => propriedade.Name))}) " +
                      $"VALUES ({string.Join(", ", typeof(AvaliacaoVO).GetProperties().Select(propriedade => $"@{propriedade.Name}"))})";

                Cmd = new MySqlCommand(Sql, Conexao);
                foreach (var propriedade in typeof(AvaliacaoVO).GetProperties())
                {
                    var value = propriedade.GetValue(avaliacao);
                    Cmd.Parameters.AddWithValue($"@{propriedade.Name}", value ?? null);
                }
                Cmd.ExecuteNonQuery();
                Conexao.Close();

                return Ok($"Avaliação cadastrada com sucesso");
            }
            catch (Exception ex)
            {
                Conexao.Close();
                return BadRequest($"Erro ao inserir dados: {ex.Message}");
            }
        }
        [HttpGet("consulta-animes-avaliacao/{id?}")]
        public IActionResult consultaAnimesAvaliacao(int? id)
        {
            try
            {
                Conexao.Open();

                if (id.HasValue)
                {
                    Sql = $"SELECT animes_avaliacao.*, animes_nome_anime.nome as nome_anime, animes_autor.nome as autor, animes_genero.nome as genero, animes_origem.nome as origem " +
                        $"FROM animes_avaliacao " +
                        $"INNER JOIN animes_nome_anime ON animes_avaliacao.nome_anime_id = animes_nome_anime.id " +
                        $"INNER JOIN animes_autor ON animes_avaliacao.autor_id = animes_autor.id " +
                        $"INNER JOIN animes_genero ON animes_avaliacao.genero_id = animes_genero.id " +
                        $"INNER JOIN animes_origem ON animes_avaliacao.origem_id = animes_origem.id " +
                        $"WHERE animes_avaliacao.id = @id";
                    Cmd = new MySqlCommand(Sql, Conexao);
                    Cmd.Parameters.AddWithValue("@id", id.Value);
                }
                else
                {
                    Sql = $"SELECT animes_avaliacao.id, animes_nome_anime.nome as nome_anime, animes_autor.nome as autor, animes_genero.nome as genero, animes_avaliacao.lancamento_anime, animes_avaliacao.temporada_aval, animes_avaliacao.nota_final, animes_avaliacao.data_criacao, animes_avaliacao.data_mod, animes_avaliacao.dropado " +
                          $"FROM animes_avaliacao " +
                          $"INNER JOIN animes_nome_anime ON animes_avaliacao.nome_anime_id = animes_nome_anime.id " +
                          $"LEFT JOIN animes_autor ON animes_avaliacao.autor_id = animes_autor.id " +
                          $"LEFT JOIN animes_genero ON animes_avaliacao.genero_id = animes_genero.id";
                    Cmd = new MySqlCommand(Sql, Conexao);
                }

                MySqlDataReader objeto = Cmd.ExecuteReader();

                List<AvaliacaoResponseConsulta> registros = new List<AvaliacaoResponseConsulta>();

                while (objeto.Read())
                {
                    AvaliacaoResponseConsulta registro = new AvaliacaoResponseConsulta();
                    foreach (var propriedade in typeof(AvaliacaoResponseConsulta).GetProperties())
                    {
                        string propriedadeNome = propriedade.Name;
                        if (objeto.HasColumn(propriedadeNome) && objeto[propriedadeNome] != DBNull.Value)
                        {
                            Type propriedadeTipo = propriedade.PropertyType;

                            // Trate tipos Nullable
                            if (propriedadeTipo.IsGenericType && propriedadeTipo.GetGenericTypeDefinition() == typeof(Nullable<>))
                            {
                                propriedadeTipo = Nullable.GetUnderlyingType(propriedadeTipo);
                            }

                            // Converte o valor do banco de dados para o tipo apropriado
                            object value = Convert.ChangeType(objeto[propriedadeNome], propriedadeTipo);

                            // Define o valor na propriedade do objeto AvaliacaoVO
                            propriedade.SetValue(registro, value);
                        }
                    }
                    registros.Add(registro);
                }
                if (registros.Count > 0)
                {
                    Conexao.Close();
                    string jsonRegistros = JsonConvert.SerializeObject(registros, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    return Ok(jsonRegistros);
                }
                else
                {
                    Conexao.Close();
                    return BadRequest($"Nenhuma avaliação cadastrada");
                }

            }
            catch (Exception ex)
            {
                Conexao.Close();
                return BadRequest($"Erro ao consultar dados: {ex.Message}");
            }
        }
        [HttpPost("atualiza-animes-avaliacao")]
        public IActionResult atualizaAvaliacao(string tabela, [FromBody] TabelaAuxiliares tabelaAuxiliares)
        {
            try
            {
                Conexao.Open();
                Sql = $"SELECT * FROM {tabela} WHERE nome = '{tabelaAuxiliares.Nome}'";
                Cmd = new MySqlCommand(Sql, Conexao);
                MySqlDataReader objeto = Cmd.ExecuteReader();
                if (objeto.Read())
                {
                    return BadRequest($"{tabelaAuxiliares.Nome} já cadastrado no id {objeto["id"]}");
                }
                else
                {
                    Conexao.Close();
                    Conexao.Open();
                    Sql = $"UPDATE {tabela} set nome = @nome WHERE id =" + tabelaAuxiliares.Id;
                    Cmd = new MySqlCommand(Sql, Conexao);
                    Cmd.Parameters.AddWithValue("@nome", tabelaAuxiliares.Nome);
                    Cmd.ExecuteNonQuery();
                    Conexao.Close();
                    return Ok($"{tabela} atualizado com sucesso");
                }



            }
            catch (Exception ex)
            {
                return BadRequest("Erro ao atualizar gênero" + ex);
            }
        }
    }

}