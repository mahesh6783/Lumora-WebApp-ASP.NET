# 🎬 Lumora

Lumora is a movie browsing web application built with **ASP.NET Core MVC** and **MySQL**. It uses the **TMDB (The Movie Database) API** to display movie information, search movies, and manage user favorites.

---

## ✨ Features

- 🔐 User Registration & Login
- 🎥 Browse Popular Movies
- 🎬 Browse Action Movies
- 🔎 Search Movies
- 📄 Movie Details Page
- ❤️ Add Movies to Favorites
- 🗑 Remove Movies from Favorites
- 👤 User Account Page
- 🔒 Session-based Authentication
- 📱 Responsive UI using Bootstrap

---

## 🛠 Technologies Used

- ASP.NET Core MVC (.NET 10)
- C#
- MySQL
- TMDB API
- Bootstrap 5
- HTML
- CSS
- JavaScript

---

## 📂 Project Structure

```
Lumora
│
├── Controllers
│   └── HomeController.cs
│
├── Models
│
├── Views
│   ├── Home
│   ├── Shared
│
├── wwwroot
│   ├── css
│   ├── js
│   └── lib
│
└── Program.cs
```

---

## 🗄 Database

Create a MySQL database named:

```sql
movie
```

### User Table

```sql
CREATE TABLE user (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100),
    email VARCHAR(100),
    password VARCHAR(100)
);
```

### Favorite Table

```sql
CREATE TABLE favorite (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT,
    movie_id INT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(user_id, movie_id)
);
```

---

## 🎬 TMDB API

Create a TMDB account and generate an API Read Access Token.

Replace the Bearer Token in `HomeController.cs`.

```
Authorization: Bearer YOUR_TMDB_TOKEN
```

---

## 🚀 Installation

Clone the repository

```bash
git clone https://github.com/YOUR_USERNAME/Lumora.git
```

Go to the project folder

```bash
cd Lumora
```

Restore packages

```bash
dotnet restore
```

Run the project

```bash
dotnet run
```

---

## 📸 Screenshots

You can add screenshots here.

- Login Page
- Home Page
- Search Page
- Movie Details
- Favorites Page

---

## 🔮 Future Improvements

- Movie Trailer Support
- Watchlist 
- Ratings 
- Genre Filtering  

---

## 👨‍💻 Author

**Mahesh H**

Portfolio: https://maheshh.vercel.app

---

## 📜 License

This project is created for learning and educational purposes.
