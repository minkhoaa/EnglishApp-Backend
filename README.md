# EnglishApp Backend

![.NET](https://img.shields.io/badge/.NET_8-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-4169E1?style=for-the-badge&logo=postgresql&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)
![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)
![JWT](https://img.shields.io/badge/JWT-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white)
![Cloudinary](https://img.shields.io/badge/Cloudinary-3448C5?style=for-the-badge&logo=cloudinary&logoColor=white)

RESTful API backend for an English learning platform. Built with ASP.NET Core 8, it provides structured lesson content, vocabulary flashcards, practice exercises, IELTS mock exams, and AI-powered writing evaluation — all secured with JWT and OAuth 2.0.

---

## Features

- **Authentication & Authorization** — JWT Bearer + Cookie dual-scheme authentication, Google and Facebook OAuth 2.0, email-based OTP for registration and password reset, role-based access control via ASP.NET Core Identity
- **Lessons & Curriculum** — Hierarchical content model: Categories → Lessons → Lesson Contents with support for text, audio, and video material
- **Exercises & Quizzes** — Multiple-choice exercises with per-option answer tracking and automatic grading; user progress and result history persisted per lesson
- **Flashcards & Decks** — Personal vocabulary decks with individual flashcard CRUD, favourite deck management
- **Exams** — Full IELTS-style mock exam structure: Exam Categories → Exams → Sections → Questions → Options with scored user result records and analytics
- **AI Writing Evaluation** — IELTS Writing Task 2 scoring via OpenRouter (DeepSeek model); evaluates Task Response, Coherence & Cohesion, Lexical Resource, and Grammatical Range; stores writing history per user
- **Media Uploads** — Cloudinary integration for profile avatar storage
- **Email Notifications** — SMTP-based transactional email (OTP delivery, account notifications) via FluentEmail

---

## Tech Stack

| Layer | Technology |
|---|---|
| Runtime | .NET 8 / ASP.NET Core 8 |
| Language | C# 12 |
| ORM | Entity Framework Core 8 (Npgsql provider) |
| Database | PostgreSQL |
| Identity | ASP.NET Core Identity |
| Authentication | JWT Bearer, Cookie, Google OAuth 2.0, Facebook OAuth 2.0 |
| AI Integration | OpenRouter API (DeepSeek model) |
| Media Storage | Cloudinary |
| Email | FluentEmail + SMTP |
| API Docs | Swagger / Swashbuckle |
| Containerisation | Docker (multi-stage build) |
| Config | DotNetEnv (`.env` file loading) |

---

## Architecture

The project follows a layered architecture within a single ASP.NET Core application:

```
EnglishApp/
├── Controllers/        # HTTP request handling, route definitions
├── Service/            # Business logic layer (interfaces + implementations)
├── Repository/         # Data access layer (interfaces + implementations)
├── Data/               # EF Core DbContext and entity classes
├── Dto/
│   ├── Request/        # Inbound data transfer objects
│   └── Response/       # Outbound data transfer objects
├── Model/              # Configuration models (JWT, Email, Cloudinary, OpenRouter)
├── Migrations/         # EF Core database migrations
├── Program.cs          # Application entry point, DI registration, middleware pipeline
└── Dockerfile          # Multi-stage production container build
```

**Request flow:** `Controller` → `Service` → `Repository` → `DbContext` → PostgreSQL

Authentication is handled by a policy scheme that automatically selects JWT Bearer or Cookie authentication based on the presence of an `Authorization: Bearer` header.

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/) 14+
- A Cloudinary account
- An OpenRouter API key
- SMTP credentials (Gmail or any SMTP provider)
- (Optional) [Docker](https://www.docker.com/) for containerised deployment

### Environment Variables

Create a `.env` file in the `EnglishApp/` directory with the following variables:

```env
# Database
CONNECTIONSTRINGS__MYDB=Host=localhost;Port=5432;Database=englishapp;Username=postgres;Password=yourpassword

# JWT
JWT__VALIDAUDIENCE=https://localhost:5001
JWT__VALIDISSUER=https://localhost:5001
JWT__SECRETKEY=your-256-bit-secret-key

# Email (SMTP)
EMAILSETTINGS__SMTPSERVER=smtp.gmail.com
EMAILSETTINGS__SMTPPORT=587
EMAILSETTINGS__SENDEREMAIL=your@email.com
EMAILSETTINGS__SENDERPASSWORD=your-app-password
EMAILSETTINGS__ENABLESSL=true

# Cloudinary
CLOUDINARYSETTINGS__CLOUDNAME=your-cloud-name
CLOUDINARYSETTINGS__APIKEY=your-api-key
CLOUDINARYSETTINGS__APISECRET=your-api-secret

# AI (OpenRouter)
AI_API_KEY=your-openrouter-api-key

# OAuth
GOOGLE__CLIENT__ID=your-google-client-id
GOOGLE__CLIENT__SECRET=your-google-client-secret
FACEBOOK_APP_ID=your-facebook-app-id
FACEBOOK_APP_SECRET=your-facebook-app-secret
```

### Running Locally

```bash
# 1. Clone the repository
git clone https://github.com/minkhoaa/EnglishApp-Backend.git
cd EnglishApp-Backend

# 2. Apply database migrations
cd EnglishApp
dotnet ef database update

# 3. Start the application
dotnet run
```

The API will be available at `https://localhost:5001`. The Swagger UI is served at `/swagger`.

### Running with Docker

```bash
# Build the image
docker build -t englishapp-backend ./EnglishApp

# Run the container (pass environment variables via --env-file)
docker run -p 8080:8080 --env-file ./EnglishApp/.env englishapp-backend
```

---

## API Overview

| Module | Prefix | Description |
|---|---|---|
| Authentication | `/api/auth` | Register, login, OTP, password reset, OAuth |
| Lessons | `/api/lessons` | Lesson catalogue and lesson content |
| Categories | `/api/categories` | Content categories |
| Exercises | `/api/exercises` | Practice questions and answer options |
| Exams | `/api/exam` | IELTS-style mock exams |
| Flashcards | `/api/flashcard` | Vocabulary cards |
| Decks | `/api/deck` | Flashcard deck management |
| Writing / AI | `/api/writing` | AI writing evaluation via OpenRouter |
| User Results | `/api/userresult` | Learning progress and exam history |

Full interactive documentation is available via Swagger UI at `/swagger` when the application is running.

---

## License

This project is for educational and portfolio purposes.
