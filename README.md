# Zenvofin's back-end

## Requirements

- .NET SDK 10.0 or newer

## Installation process

1. Navigate to the devops repository and follow the instructions to set up needed component.
2. Clone this repository to your local machine.
3. Open your IDE or Code Editor and navigate to the project folder.
4. Create .env file in the root folder and add environment variables:

```
POSTGRESQL_CONNECTION=Host=localhost;Database=zenvofin;Username=postgres;Password=postgres
JWT_SECRET=generate_secret_key
SEQ_API_KEY=generate_key_in_seq
```

5. Run the application.
