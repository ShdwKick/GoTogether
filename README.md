# GraphQL Server Application

## Description (English)

This is a server-side application written in C#. It leverages the HotChocolate library to implement a GraphQL API and uses Entity Framework Core for interacting with a PostgreSQL database. The repository does not include the database itself; instead, it focuses on the backend logic and database interaction layer.

### Features

- **GraphQL API**: Built with HotChocolate to provide a flexible and efficient way to query and mutate data.
- **PostgreSQL Integration**: Entity Framework Core is used for seamless database interaction.
- **Modular Design**: Easy to extend and maintain.

### Prerequisites

- .NET 9.0 or higher
- PostgreSQL database
- Tools for managing PostgreSQL (e.g., pgAdmin, DBeaver)

### Setup and Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/ShdwKick/GoTogether.git
   ```
2. Navigate to the project directory:
   ```bash
   cd GoTogether
   ```
3. Install dependencies:
   ```bash
   dotnet restore
   ```
4. Update the connection string in `appsettings.json` to point to your PostgreSQL database.
5. Apply migrations:
   ```bash
   dotnet ef database update
   ```
6. Run the application:
   ```bash
   dotnet run
   ```

### Usage

Once the application is running, you can access the GraphQL endpoint at:

```
http://localhost:5000/graphql
```

Use tools like [GraphiQL](https://www.npmjs.com/package/graphiql) or [Altair GraphQL Client](https://altair.sirmuel.design/) to interact with the API.

### Contribution

Feel free to fork the repository, create a new branch, and submit a pull request for any improvements or features.


---

## Описание (Russian)

Это серверное приложение, написанное на C#. Оно использует библиотеку HotChocolate для реализации GraphQL API и Entity Framework Core для взаимодействия с базой данных PostgreSQL. В репозитории отсутствует сама база данных; акцент сделан на серверной логике и слое взаимодействия с базой данных.

### Особенности

- **GraphQL API**: Реализовано с помощью HotChocolate для гибкого и эффективного запроса и изменения данных.
- **Интеграция с PostgreSQL**: Используется Entity Framework Core для удобной работы с базой данных.
- **Модульная архитектура**: Легко расширять и поддерживать.

### Требования

- .NET 9.0 или выше
- PostgreSQL база данных
- Инструменты для работы с PostgreSQL (например, pgAdmin, DBeaver)

### Установка и настройка

1. Склонируйте репозиторий:
   ```bash
   git clone https://github.com/ShdwKick/GoTogether.git
   ```
2. Перейдите в директорию проекта:
   ```bash
   cd GoTogether
   ```
3. Установите зависимости:
   ```bash
   dotnet restore
   ```
4. Обновите строку подключения в файле `appsettings.json`, указав вашу базу данных PostgreSQL.
5. Примените миграции:
   ```bash
   dotnet ef database update
   ```
6. Запустите приложение:
   ```bash
   dotnet run
   ```

### Использование

После запуска приложения GraphQL endpoint будет доступен по адресу:

```
http://localhost:5000/graphql
```

Используйте инструменты, такие как [GraphiQL](https://www.npmjs.com/package/graphiql) или [Altair GraphQL Client](https://altair.sirmuel.design/), для взаимодействия с API.

### Вклад

Вы можете форкнуть репозиторий, создать новую ветку и отправить pull request с улучшениями или новыми функциями.

