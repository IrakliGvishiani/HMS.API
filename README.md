# HMS: Hotel Management System API



ASP.NET Core Web API for managing hotels, rooms and reservations, with role-based access for Admins, Managers and Guests.



The Angular client lives in a separate repository (`hms-client`).



# Tech stack



- .NET 10, ASP.NET Core, ASP.NET Identity, EF Core

- SQL Server 2022

- Redis (email confirmation codes, refresh tokens, rate limiting)

- JWT access tokens + refresh tokens

- Cloudinary (hotel and room images)

- Brevo SMTP relay (transactional email)

- Mapster, Swagger (Swashbuckle)

- Docker Compose, GitHub Actions



# Solution structure



| Project | Responsibility |

|---|---|

| `HMS.API` | Controllers, middleware, background jobs, `Program.cs` |

| `HMS.Application` | Services, DTOs, mapping, contracts |

| `HMS.Domain` | Entities and enums |

| `HMS.Infrastructure` | `DbContext`, configurations, repositories, migrations |



# Roles



| Role | Can do |

|---|---|

| \*\*Admin\*\* | Manage hotels, managers and other admins; view and manage all reservations; global analytics |

| \*\*Manager\*\* | Edit own hotel, manage own hotel's rooms and reservations, view own hotel analytics |

| \*\*Guest\*\* | Browse hotels and rooms, create / edit / cancel own reservations |



# Features



- Registration with 6-digit email confirmation code (stored in Redis for 5 minutes)

- Login, refresh tokens, token revocation

- Forgot / reset password by email

- Hotel and room CRUD with multiple images (primary image selection)

- Reservations with availability checks and automatic status updates

 (Reserved → Active → Completed, handled by a background service)

- Rervation confirmation emails

- Analytics (rooms, reservations, guests, revenue)



# Getting started



# Prerequisites



- Docker and Docker Compose

- A Cloudinary account

- A Brevo account (SMTP key and a verified sender)



# 1. Configure environment



Create a `.env` file next to `docker-compose.yml` (see `.env.example`):



| Variable | Description |

|---|---|

| `MSSQL_SA_PASSWORD` | SQL Server `sa` password (must satisfy SQL Server complexity rules) |

| `JWT_SECRET` | Long random string, at least 32 characters |

| `SMTP_USERNAME` | Brevo SMTP login |

| `SMTP_PASSWORD` | Brevo SMTP key |

| `CLOUDINARY_CLOUD_NAME` | Cloudinary cloud name |

| `CLOUDINARY_API_KEY` | Cloudinary API key |

| `CLOUDINARY_API_SECRET` | Cloudinary API secret |



# 2. Run



```bash

docker compose up -d --build

```



Migrations are applied automatically on startup.



| Service | URL |

|---|---|

| API | `http://127.0.0.1:8080` |

| Swagger | `http://127.0.0.1:8080/swagger` |

| Health check | `http://127.0.0.1:8080/health` |

| Client (if `hms.client` is enabled) | `http://127.0.0.1:8081` |



# API overview



| Route | Purpose |

|---|---|

| `/api/auth` | Register, confirm email, login, refresh / revoke token, password reset |

| `/api/hotel` | Hotel list, details, create / update / delete |

| `/api/room` | Rooms by hotel, details, search, create / update / delete |

| `/api/reservations` | Create, update, cancel, search, get by id |

| `/api/manager` | Manager list, own profile, analytics |

| `/api/admin` | Admin management |

| `/api/guest` | Guest list, update, delete |



# Deployment notes



- Production runs on a DigitalOcean droplet behind nginx (reverse proxy).

- DigitalOcean blocks outbound SMTP on ports 25, 465 and 587, so the API uses the Brevo relay on port \*\*2525\*\* with STARTTLS (`EmailSettings__UseSsl=false`). Gmail SMTP does not work from the droplet.

- nginx needs `client_max_body_size 20M` for image uploads.

- Pushing to `master` triggers GitHub Actions, which SSHes into the server, runs

 `git fetch` + `git reset --hard origin/master`, then `docker compose up -d --build`.

 Never edit tracked files on the server by hand; they are overwritten on the next deploy.



# First admin



The first Admin account is created automatically during application startup by the data seeder. If an Admin account already exists, the seeder does not create another one.



The Admin registration endpoint is restricted to authenticated Admin users.



The initial Admin's credentials and personal information are configured through environment variables in the .env file. Never commit the .env file or expose its contents in the repository.



