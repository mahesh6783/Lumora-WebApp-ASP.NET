using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Lumora.Models;
using MySqlConnector;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
 
namespace Lumora.Controllers;


public class HomeController : Controller
{
    public string conn = "Server=127.0.0.1;Port=3307;Database=movie;Uid=root;Pwd=;"; 
    
    
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
// login
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Login(string email, string password )
    {
        var con = new MySqlConnection(conn);
        con.Open();
        var cmd = new MySqlCommand("SELECT id FROM user WHERE email=@e AND password=@p", con);
       
        cmd.Parameters.AddWithValue("@e", email);
        cmd.Parameters.AddWithValue("@p", password);

        var result = cmd.ExecuteScalar();

        if (result != null) // At least one row found
        {
            // Login success
            HttpContext.Session.SetInt32("UserId", Convert.ToInt32(result));
            return RedirectToAction("Movies");
            
        }
        
        else
        {
            ViewBag.Message = "Invalid Email or Password";
            return View();
           
        }
         
    }
// login end 

// register

    public IActionResult Register()
    {
         
        return View();
    }
    [HttpPost]
    public IActionResult Register(string name, string email, string password)
    {
        var con = new MySqlConnection(conn);
        con.Open();

        var cmd = new MySqlCommand("INSERT INTO user(name,email,password) VALUES(@n,@e,@p)", con);

        cmd.Parameters.AddWithValue("@n", name);
        cmd.Parameters.AddWithValue("@e", email);
        cmd.Parameters.AddWithValue("@p", password);

        cmd.ExecuteNonQuery();

        con.Close();

        TempData["Success"] = "Registration Successful!";

        // Redirect to Login page
        return RedirectToAction("Login");
    }

    // register end 

    // movies 
  
    public class Movie
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }

        [JsonPropertyName("backdrop_path")]
        public string? BackdropPath { get; set; }

        [JsonPropertyName("overview")]
        public string? Overview { get; set; }

        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }

        [JsonPropertyName("release_date")]
        public string? ReleaseDate { get; set; }
    }

  
    public class MovieResponse
    {
        [JsonPropertyName("results")]
        public List<Movie>? Results { get; set; }
    }

    public async Task<IActionResult> Movies()
    {
        if (HttpContext.Session.GetInt32("UserId") == null)
        {
            return RedirectToAction("Login");
        }
        var client = new HttpClient();

        client.DefaultRequestHeaders.Add("Authorization","Bearer eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiIxMTM2Y2JjYjQ3MzIzNzAzOGFkYzk4Y2QzNTAwM2VlOSIsIm5iZiI6MTc2ODIyOTE2NS4yODksInN1YiI6IjY5NjUwOTJkYTQ2OWQ1ZDk5YzI4NDg3MSIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.9Ksn4uD8FkJccNVVACe9XIXOybQseFJBp-kQcjU15Zk");

        var p_movies = await client.GetFromJsonAsync<MovieResponse>("https://api.themoviedb.org/3/movie/popular");
        var a_movies = await client.GetFromJsonAsync<MovieResponse>("https://api.themoviedb.org/3/discover/movie?with_genres=28");

        
        ViewBag.BannerMovie = p_movies?.Results?.FirstOrDefault();
        ViewBag.p_Movies = p_movies?.Results;
        ViewBag.a_Movies = a_movies?.Results;
        

        return View();
    }
    // movies end 

    // details 

    public async Task<IActionResult> Details(int id)
    {
        if (HttpContext.Session.GetInt32("UserId") == null)
        {
            return RedirectToAction("Login");
        }
        var client = new HttpClient();

        client.DefaultRequestHeaders.Add("Authorization","Bearer eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiIxMTM2Y2JjYjQ3MzIzNzAzOGFkYzk4Y2QzNTAwM2VlOSIsIm5iZiI6MTc2ODIyOTE2NS4yODksInN1YiI6IjY5NjUwOTJkYTQ2OWQ1ZDk5YzI4NDg3MSIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.9Ksn4uD8FkJccNVVACe9XIXOybQseFJBp-kQcjU15Zk");

        var movie_details = await client.GetFromJsonAsync<Movie>($"https://api.themoviedb.org/3/movie/{id}");

        ViewBag.movie_details = movie_details;

        int? userId = HttpContext.Session.GetInt32("UserId");

        using var con = new MySqlConnection(conn);
        con.Open();

        var cmd = new MySqlCommand(
            "SELECT COUNT(*) FROM favorite WHERE user_id=@u AND movie_id=@m", con);

        cmd.Parameters.AddWithValue("@u", userId);
        cmd.Parameters.AddWithValue("@m", id);

        ViewBag.IsFavorite = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
       

        return View();
    }
    // details end 

    // account
    [HttpGet]
    public async Task<IActionResult> Search(string movieName)
    {
        if (HttpContext.Session.GetInt32("UserId") == null)
        {
            return RedirectToAction("Login");
        }
        if (string.IsNullOrWhiteSpace(movieName))
        {
            ViewBag.Movies = new List<Movie>();
            return View();
        }

        var client = new HttpClient();

        client.DefaultRequestHeaders.Add("Authorization","Bearer eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiIxMTM2Y2JjYjQ3MzIzNzAzOGFkYzk4Y2QzNTAwM2VlOSIsIm5iZiI6MTc2ODIyOTE2NS4yODksInN1YiI6IjY5NjUwOTJkYTQ2OWQ1ZDk5YzI4NDg3MSIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.9Ksn4uD8FkJccNVVACe9XIXOybQseFJBp-kQcjU15Zk");


        var result = await client.GetFromJsonAsync<MovieResponse>(
            $"https://api.themoviedb.org/3/search/movie?query={Uri.EscapeDataString(movieName)}"
        );

        ViewBag.Movies = result?.Results;

        return View();
    }
    
    [HttpPost]
    public IActionResult AddFavorite(int movieId)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
            return RedirectToAction("Login");

        using var con = new MySqlConnection(conn);
        con.Open();

        var cmd = new MySqlCommand(
            "INSERT IGNORE INTO favorite(user_id,movie_id) VALUES(@u,@m)", con);

        cmd.Parameters.AddWithValue("@u", userId);
        cmd.Parameters.AddWithValue("@m", movieId);

        cmd.ExecuteNonQuery();

        return RedirectToAction("Details", new { id = movieId });
    }
    [HttpPost]
    public IActionResult RemoveFavorite(int movieId)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");

        using var con = new MySqlConnection(conn);
        con.Open();

        var cmd = new MySqlCommand(
            "DELETE FROM favorite WHERE user_id=@u AND movie_id=@m", con);

        cmd.Parameters.AddWithValue("@u", userId);
        cmd.Parameters.AddWithValue("@m", movieId);

        cmd.ExecuteNonQuery();

        return RedirectToAction("Details", new { id = movieId });
    }

    public async Task<IActionResult> Account()
    {
        int? userId = HttpContext.Session.GetInt32("UserId");

        var ids = new List<int>();

        using var con = new MySqlConnection(conn);
        con.Open();

        var cmd = new MySqlCommand(
            "SELECT movie_id FROM favorite WHERE user_id=@u", con);

        cmd.Parameters.AddWithValue("@u", userId);

        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            ids.Add(reader.GetInt32("movie_id"));
        }

        reader.Close();

        var client = new HttpClient();

        client.DefaultRequestHeaders.Add("Authorization","Bearer eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiIxMTM2Y2JjYjQ3MzIzNzAzOGFkYzk4Y2QzNTAwM2VlOSIsIm5iZiI6MTc2ODIyOTE2NS4yODksInN1YiI6IjY5NjUwOTJkYTQ2OWQ1ZDk5YzI4NDg3MSIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.9Ksn4uD8FkJccNVVACe9XIXOybQseFJBp-kQcjU15Zk");


        var movies = new List<Movie>();

        foreach (int id in ids)
        {
            var movie = await client.GetFromJsonAsync<Movie>(
                $"https://api.themoviedb.org/3/movie/{id}"
            );

            if (movie != null)
                movies.Add(movie);
        }

        ViewBag.Favorites = movies;

        return View();
    }



    // account end

    // logout
    public IActionResult Logout()
        {   
            HttpContext.Session.Clear(); 
            return RedirectToAction("Index");
        }

    // logout end

}



 