using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Text;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

public class Result
{

    /*
     * Complete the 'highestInternationalStudents' function below.
     *
     * The function is expected to return a STRING.
     * The function accepts following parameters:
     *  1. STRING firstCity
     *  2. STRING secondCity
     * Base URL: https://jsonmock.hackerrank.com/api/universities?page=
     */

    // Função para encontrar a universidade com mais estudantes internacionais entre duas cidades.
    // Recebe duas cidades (firstCity e secondCity) como parâmetros.

    public static async Task<string> highestInternationalStudents(string firstCity, string secondCity)
    {
        string url = "https://jsonmock.hackerrank.com/api/universities?page=";

        using (var httpClient = new HttpClient())
        {
            List<UniversityDTO> universities = await GetUniversities(httpClient, url);
            string universityMostStudents = UniversityMostStudents(universities, firstCity, secondCity);
            return universityMostStudents;
        }


    }

    // Função para obter todas as universidades a partir da API paginada.
    // Recebe o HttpClient e a URL base como parâmetros e retorna uma lista de UniversityDTO.
    private static async Task<List<UniversityDTO>> GetUniversities(HttpClient httpClient, string url)
    {
        List<UniversityDTO> allUniversities = new List<UniversityDTO>();
        int pageNumber = 1;
        string apiUrl = url + pageNumber;

        while (true)
        {
            var response = await httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = System.Text.Json.JsonSerializer.Deserialize<ResponseDTO>(content);
            allUniversities.AddRange(result.Data);

            if (result.Page == result.TotalPages)
                break;

            pageNumber++;
            apiUrl = url + pageNumber;
        }

        return allUniversities;
    }
    public class UniversityDTO
    {
        [JsonPropertyName("university")]
        public string UniversityName { get; set; }

        [JsonPropertyName("rank_display")]
        public string RankDisplay { get; set; }

        [JsonPropertyName("score")]
        public double Score { get; set; }

        [JsonPropertyName("student_faculty_ratio")]
        public int StudentFacultyRatio { get; set; }

        [JsonPropertyName("international_students")]
        public string InternationalStudentsString { get; set; }

        public int InternationalStudents
        {
            get
            {
                if (int.TryParse(InternationalStudentsString?.Replace(",", ""), out int result))
                {
                    return result;
                }
                return 0;
            }
        }

        [JsonPropertyName("faculty_count")]
        public string FacultyCount { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("location")]
        public LocationDTO Location { get; set; }
    }

    public class LocationDTO
    {
        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("region")]
        public string Region { get; set; }
    }

    public class ResponseDTO
    {
        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("per_page")]
        public int PerPage { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("data")]
        public UniversityDTO[] Data { get; set; }
    }


    private static string UniversityMostStudents(List<UniversityDTO> universities, string firstCity, string secondCity)
    {
        // Inicializa variáveis para armazenar a universidade com mais estudantes internacionais e a quantidade máxima de estudantes encontrada.
        string universityMostStudents = "";
        int maxStudents = -1;

        // Loop para encontrar a universidade com mais estudantes internacionais na primeira cidade (firstCity).
        foreach (var university in universities)
        {
            // Verifica se a universidade está localizada na primeira cidade e se tem mais estudantes internacionais do que a quantidade máxima já encontrada.
            if (university.Location.City.Equals(firstCity) && (university.InternationalStudents > maxStudents))
            {
                // Se sim, atualiza os dados da universidade com mais estudantes internacionais.
                universityMostStudents = university.UniversityName;
                maxStudents = university.InternationalStudents;
            }
        }

        // Se nenhuma universidade com mais estudantes internacionais foi encontrada na primeira cidade, procura na segunda cidade (secondCity).
        if (universityMostStudents == "")
        {
            // Loop para encontrar a universidade com mais estudantes internacionais na segunda cidade (secondCity).
            foreach (var university in universities)
            {
                // Verifica se a universidade está localizada na segunda cidade e se tem mais estudantes internacionais do que a quantidade máxima já encontrada.
                if (university.Location.City.Equals(secondCity) && (university.InternationalStudents > maxStudents))
                {
                    // Se sim, atualiza os dados da universidade com mais estudantes internacionais.
                    universityMostStudents = university.UniversityName;
                    maxStudents = university.InternationalStudents;
                }
            }
        }

        // Retorna o nome da universidade com mais estudantes internacionais entre as duas cidades (firstCity e secondCity).
        return universityMostStudents;
    }
}

class Solution
{
    public static void Main(string[] args)
    {

        string firstCity = Console.ReadLine();
        string secondCity = Console.ReadLine();
        string result = Result.highestInternationalStudents(firstCity, secondCity).Result;
        Console.WriteLine(result);
    }
}