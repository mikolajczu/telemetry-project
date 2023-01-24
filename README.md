# Projekt na bazy danych 2 - Telemetria

## Mateusz Różański, Mikołaj Czurłowski

### Opis projektu.

Jest to projekt aplikacji internetowej, której celem jest zbieranie informacji o sesji użytkownika, tj. czas spędzony na przeglądaniu poszczególnych stron aplikacji,
ogólny czas trwania sesji, data logowania, status zalogowania.

### Wykorzystana technologia.

- ASP.NET Core 6.0
- Dokumentowa baza danych MongoDB
- Docker

### Struktura projektu.

Solucja podzielona jest na trzy projekty:

- Telemetry - aplikacja internetowa MVC
- Telemetry.Entities - wykorzystywane modele
- Mongo.AspNetCore.Identity - napisany na własne potrzeby provider do MongoDB dla ASP.NET Identity Framework (uwierzytelnianie i autoryzacja)

### Jak uruchomić?

- <code>git clone https://github.com/mikolajczu/telemetry-project.git</code> lub <code> git clone https://github.com/matthewrosse/telemetry-project.git</code>
- <code>cd telemetry-project/</code>
- <code>docker compose -f docker-compose.yml -f docker-compose.override.yml up --build -d</code>

Następnie można udać się pod adres http://localhost:5196 w przeglądarce i się zarejestrować.

Aby pozbyć się utworzonych kontenerów: <code>docker compose -f docker-compose.yml -f docker-compose.override.yml down</code>
