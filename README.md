Senswise Case – Kullanıcı CRUD (CQRS, .NET 6, PostgreSQL)

Kullanıcı ekleme / güncelleme / silme / listeleme API’si.
Clean Architecture + CQRS, MediatR, EF Core (PostgreSQL), FluentValidation, Mapster.

İçerik

Mimari

Teknolojiler

Dizin Yapısı

Kurulum & Çalıştırma

Migration (EF Core)

API Uçları

Doğrulama Kuralları

Hata Yönetimi

Veritabanı Notları

Geliştirilebilir Alanlar

Sık Sorunlar / İpuçları

Mimari

Clean Architecture ilkesiyle bağımlılıklar içe akar:

API  →  Application  →  Domain
  └──→  Infrastructure (Application’daki portları/arayüzleri uygular)


Domain: Entity & Value Object’ler (ör. User, Email), domain kuralları.

Application: Use-case’ler (MediatR Command/Query + Handler), DTO’lar, doğrulama (FluentValidation), mapping (Mapster), pipeline behaviors.

Infrastructure: EF Core + Npgsql, AppDbContext, repository implementasyonları, UnitOfWork, DI kablolaması.

API: ASP.NET Core WebAPI, Controller’lar, exception middleware, Swagger.

Teknolojiler

.NET 6, ASP.NET Core WebAPI

CQRS / MediatR

FluentValidation (DTO seviyesinde doğrulama)

Mapster (mapping + server-side projection)

EF Core + Npgsql (PostgreSQL)

EFCore.NamingConventions (snake_case tablo/sütun)

ProblemDetails (RFC7807) tabanlı hata cevapları

Dizin Yapısı
src/
├─ Api/
│  ├─ Program.cs
│  ├─ Controllers/
│  │  └─ UsersController.cs
│  └─ Middleware/
│     └─ ExceptionHandlingMiddleware.cs
├─ Application/
│  ├─ Commands/Queries/Handlers (Users)
│  ├─ Contracts/Users (DTO’lar)
│  ├─ Repositories (IUserRepository, IUnitOfWork, IRepository)
│  ├─ Validators (FluentValidation)
│  ├─ Behaviors (Logging, Validation)
│  ├─ Mapping (MapsterConfig)
│  └─ DependencyInjection.cs
├─ Domain/
│  ├─ Entities (User)
│  ├─ ValueObjects (Email)
│  └─ Abstractions (IHasTimestamps vs.)
└─ Infrastructure/
   ├─ Db (AppDbContext, Configurations, Interceptors, DesignTimeFactory)
   ├─ Repositories (EfRepository, UserRepository, UnitOfWork)
   ├─ Db/Migrations (EF Core migrations)
   └─ DependencyInjection.cs

Kurulum & Çalıştırma
1) PostgreSQL

Docker ile:

docker run --name senswise-pg -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres:16

2) Connection String

src/Api/appsettings.Development.json

{
  "ConnectionStrings": {
    "Postgres": "Host=localhost;Port=5432;Database=senswise_case;Username=postgres;Password=postgres"
  }
}


Parolan farklıysa güncelle.

3) Bağımlılıklar ve Build
dotnet clean
dotnet restore
dotnet build

4) Migration & DB

Aşağıdaki bölümdeki adımları uygula: Migration (EF Core)

5) Çalıştır
dotnet run --project src/Api/Api.csproj


Swagger: https://localhost:<port>/swagger

Migration (EF Core)
Visual Studio – Package Manager Console (en kolay)

Tools → NuGet Package Manager → Package Manager Console

# Gerekirse
Install-Package Microsoft.EntityFrameworkCore.Tools -Project Infrastructure
Install-Package Microsoft.EntityFrameworkCore.Design -Project Api

# Migration oluştur
Add-Migration Init -Project Infrastructure -StartupProject Api -Context AppDbContext -OutputDir Db\Migrations


citext eklentisi: Oluşan migration’ın Up() metodunun en başına ekleyin:

migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS citext;");

# Veritabanına uygula
Update-Database -Project Infrastructure -StartupProject Api -Context AppDbContext

Uygulama açılışında otomatik migrate (opsiyonel)

Program.cs:

using Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}


Prod’da dikkatli kullanın; CI/CD script veya bundle daha güvenli.

API Uçları
POST /api/users

Body

{
  "firstName": "Ada",
  "lastName": "Lovelace",
  "email": "ada@senswise.io",
  "address": "London"
}


200 OK

{
  "id": "GUID",
  "firstName": "Ada",
  "lastName": "Lovelace",
  "email": "ada@senswise.io",
  "address": "London"
}

PUT /api/users/{id}

Body: CreateUserRequest ile aynı (Id route’tan gelir).

DELETE /api/users/{id}

204 No Content

GET /api/users/{id}

200 OK / 404 Not Found

GET /api/users?page=1&pageSize=20&search=ada

200 OK

{
  "items": [ { "id":"...", "firstName":"...", "lastName":"...", "email":"...", "address":"..." } ],
  "page": 1,
  "pageSize": 20,
  "totalCount": 1
}

Doğrulama Kuralları

Zorunlu: FirstName, LastName, Email

Opsiyonel: Address

Email format kontrolü (FluentValidation), benzersiz email (unique index + handler kontrolü)

Uzunluklar: FirstName/LastName ≤ 100, Email ≤ 200, Address ≤ 500

Hata Yönetimi

ValidationException → 400 (ValidationProblemDetails)

KeyNotFoundException → 404

Eşsiz email çakışması → 409

Eşzamanlılık (xmin) → 409

Diğerleri → 500 (ProblemDetails)

Veritabanı Notları

citext: email sütunu case-insensitive (PostgreSQL citext).

Unique Index: email üzerinde tekillik.

Timestamps: CreatedAt/UpdatedAt SaveChangesInterceptor ile otomatik.

Snake case: EFCore.NamingConventions ile tablo/sütunlar snake_case.

EF Çeviri İpucu (Önemli):
Sorgularda Email.Value kullanma; EF bunu SQL’e çeviremeyebilir.
Karşılaştırma yaparken VO eşitliği kullan:

await _set.AnyAsync(u => u.Email == Email.Create(email), ct); // ✔️


Arama yaparken Email.Value.ToLower() yerine EF.Functions.ILike + EF.Property<string>(x, "email") tercih edin (Infrastructure repo içinde).

Geliştirilebilir Alanlar

Testler:

Unit (Domain kuralları, Handlers için fake repos)

Integration (WebApplicationFactory + Testcontainer PostgreSQL)

Transaction Behavior (MediatR pipeline): Komutlar için otomatik transaction.

Outbox Pattern (domain events + mesajlaşma garantisi).

Serilog + OpenTelemetry (log, tracing, metrics).

Health Checks (db bağlantısı, readiness/liveness).

Caching (ListUsers → redis, response caching).

AuthN/AuthZ (JWT + policy bazlı yetki).

Pagination/Sorting (dinamik alanlara göre sort).

Globalization (TR/EN hata mesajları).

Repository’de spesifik metotlar (ör. SearchAsync) ile Application’da EF bağımlılığını sıfırlama.
