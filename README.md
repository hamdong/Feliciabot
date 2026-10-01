# Feliciabot 🍵

![C#](https://img.shields.io/badge/C%23-.NET%2010-%23239120?style=flat-square&logo=c-sharp&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Supported-%230db7ed?style=flat-square&logo=docker&logoColor=white)
![Discord](https://img.shields.io/badge/Discord-Bot-%235865F2?style=flat-square&logo=discord&logoColor=white)

Feliciabot is a custom Discord bot with a quirky personality, themed after a clumsy maid character from a tactical roleplaying game. She brings interactive fun and helpful utilities to your server — all powered by .NET 10 runtime.

---

## ✨ Features

- 👋 **Greetings**: Welcomes new users and announces member joins/leaves.
- 🎲 **Randomized Games**: Roll dice, flip coins, and even… roll tea.
- 😂 **Reaction GIFs**: Sends gifs for various emotes or roleplay scenarios.
- 🔍 **Media Search**:
  - Booru-style image searching
  - Video lookup and quick-post

- 💬 **Text Filters**: Includes fun text manipulation like `uwu`-speak and more.

---

## 🛠️ Tech Stack

- [C# (.NET 10)](https://dotnet.microsoft.com/en-us/)
- [Discord.NET](https://github.com/discord-net/Discord.Net)
- Docker for containerization

---

## 🚀 Getting Started

### Clone the Repository

```bash
git clone https://github.com/hamdong/Feliciabot.git
cd Feliciabot
```

### Setup Environment Variables

Create a `.env.dev` file (for development) or `.env.prod` (for production) in the root directory:

```ini
DISCORD_TOKEN=your-discord-token-here
```

> 🛡️ Do **not** commit your token files or `.env` to version control.

---

## 🐳 Running with Docker

### Development

```bash
docker-compose --env-file .env.dev up --build
```

### Production

```bash
docker-compose --env-file .env.prod up --build -d
```

> 🛠️ The bot reads its token from an environment variable and will fail to start if one is not provided.

### Stopping the Container

```bash
docker-compose down
```

### Metrics

For metrics gathering, run once:

```bash
docker network create feliciabot-metrics
```

---

## 🧪 Local Development (without Docker)

For local development:

1. Copy `Feliciabot/Properties/launchSettings.example.json` to
   `Feliciabot/Properties/launchSettings.json`, then replace the placeholder
   `DISCORD_TOKEN` with your bot token. Do not commit the copied file.

2. Run from the project directory so the app can find `appsettings.json`:

   ```bash
   cd Feliciabot
   dotnet run --launch-profile Feliciabot
   ```

---

## 📁 Project Structure

```bash
Feliciabot/
├── Feliciabot/               # Main project source code
│   ├── Program.cs
│   ├── DiscordClientHost.cs
│   ├── ...
│   ├── appsettings.json
│   └── Properties/
│       └── launchSettings.example.json
├── FeliciabotTests/          # Automated tests
├── .env.example
├── docker-compose.yml
└── Dockerfile
```

---

## 🤝 Related Projects

Check out [Florabot ❄️](https://github.com/hamdong/Florabot) — a Node.js-based sister bot with a different personality and feature set!
