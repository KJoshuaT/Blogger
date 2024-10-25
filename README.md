# ChitTalk

ChitTalk is a full-stack blogging application that allows users to create, manage, and share blog posts. The application is designed to be user-friendly and provides a seamless experience for both authors and readers.

## Features

- User authentication (registration, login, logout)
- Create, edit, and delete blog posts
- Comment on posts
- Image uploads for blog content
- Responsive design for mobile and desktop

## Technologies Used

- **Frontend:** HTML, CSS, JavaScript
- **Backend:** ASP.NET Core
- **Database:** SQLite / SQL Server
- **Hosting:** AWS

## Getting Started

### Prerequisites

- .NET SDK 6.0 or higher
- [Docker](https://www.docker.com/get-started) (is optional)

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/KJoshuaT/ChitTalk.git
   cd ChitTalk

2. Restore Dependencies:
    ```dotnet restore

3. Run the application locally:
    dotnet run

The application will be available at http://localhost:5090

### Docker Setup

1. To build the Docker image:
    docker build -t chit-talk .

2. Run the  Docker container:
    docker run -p 5090:80 chit-talk

The application will be accessible at http://localhost:5090.

### Usage 

1. Open your web browser and navigate to the application URL.

2. Register a new account or log in with an existing account.

3. Start creating and sharing blog posts!

## Contributing
Contributions are welcome! If you have suggestions or improvements, please fork the repository and submit a pull request.

## Acknowledgements

ASP.NET Core Documentation
    
Docker Documentation

