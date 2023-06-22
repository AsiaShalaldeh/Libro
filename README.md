<h1>Libro: Library Management System</h1>

<h3>Project Description:</h3>
Libro is a comprehensive Book Management System designed to facilitate the easy management and discovery of books in a library setting. The primary focus of this project is to design and implement the web APIs that will support the functionality of this application. These APIs will handle user registration and authentication, book transactions, patron profile management, book and author management, and more.

<h3>Main Features:</h3>

<ul>
    <li><b>User Registration and Authentication:</b> Users can register and log in to the system with different access levels (patrons, librarians, administrators).</li>

<li><b>Searching and Browsing Books:</b> Users can search for books by title, author, genre, and other criteria, and browse all available books. Book information includes details like title, author, publication date, genre, and availability status.</li>

<li><b>Book Transactions:</b> Patrons can reserve available books, librarians can check out books to patrons, and accept returned books. The system tracks due dates for borrowed books and identifies overdue books.</li>

<li><b>Patron Profiles:</b> Patrons can view their own profile, including borrowing history and current or overdue loans. Librarians and administrators can view and edit patron profiles.</li>

<li><b>Book and Author Management:</b> Librarians and administrators can add, edit, or remove books and authors in the system. Administrators can manage librarian accounts.</li>
</ul>

<h3>Additional Features:</h3>

<ul>
<li><b>Reading Lists:</b> Patrons can create and manage custom reading lists.</li>

<li><b>Book Reviews and Ratings:</b> Patrons can rate and review books, and view reviews and ratings by other patrons.</li>

<li><b>Notifications:</b> The system can send notifications to patrons about due dates, reserved books, or other important events.</li>

<li><b>Book Recommendations:</b> The system can provide personalized book recommendations to patrons based on their borrowing history or favorite genres.</li>
</ul>

<h3>Architecture</h4>
<p>This project follows the Onion Architecture with Domain-Driven Design (DDD) principles to build a scalable and maintainable software system. The Onion Architecture provides a layered architectural approach that promotes separation of concerns and allows for easy testing and evolution of the system.</p>
<p>
The inner layer is the Domain Layer and the outer ones are the Infrastructure and WebApi layers. Code is always coupled towards the center which is the Domain Model and, since the Domain Model is the center, it can be only coupled to itself.
</p>
<p>By adopting the Onion Architecture with DDD, this project aims to achieve a clear separation of concerns, maintainable codebase, and flexibility to adapt to changing business requirements. The layering structure ensures that each layer has well-defined responsibilities, making the system more modular, testable, and easier to maintain in the long run.</p>
<img src="C:\Users\Done\Desktop\Programming\CSharp\Project\images/onion.png">

<h3>Project Structure</h3>
The project is organized into the following four layers:
<ul>
<li><b>Libro.Domain:</b> This layer represents the core domain of the application and contains the domain models, Enums, and interfaces that define the behavior of the system. It encapsulates the essential business logic and is independent of any specific technology or framework.</li>

<li><b>Libro.Application:</b> The application layer implements the application services that orchestrate the domain entities. It coordinates the flow of data and actions within the system and enforces the business rules defined in the domain layer. This layer depends on domain layer.</li>

<li><b>Libro.Infrastructure:</b> It includes data access components,implementations for repositories, and other infrastructure-specific implementations. The infrastructure layer handles persistence and data retrieval and takes care of communicating with the external world.</li>

<li><b>Libro.WebApi:</b> Presentation Layer that is the entry point of the application and handles the communication with clients. It exposes the necessary endpoints for clients to interact with the system. This layer is responsible for handling HTTP requests, authentication, authorization, and input/output mapping.</li>
</ul>


<h3>Technologies Used:</h3>
<ul>
    <li>Programming Language: C#</li>
    <li>Framework: ASP.NET Core</li>
    <li>Database: SQL Server</li>
    <li>Authentication: JWT (JSON Web Tokens)</li>
    <li>ORM: Entity Framework Core</li>
    <li>API Documentation: Postman</li>
    <li>Validations: FluentValidation</li>
</ul>

<h3>Installation and Setup:</h3>
<ul>
    <li>Clone the repository.</li>
    <li>Install the required dependencies.</li>
    <li>Configure the database connection.</li>
    <li>Run the database migrations.</li>
    <li>Start the application.</li>
</ul>

<h3>API Documentation:</h3>
The API documentation for Libro is available using Postman, a popular API development and testing tool. With Postman, you can explore and interact with the Libro APIs directly from your browser. The documentation provides details about each API endpoint, the required request parameters, sample request/response payloads, and more.

<h3>Contributing:</h3>

Contributions to the Libro project are welcome! If you find any issues or have suggestions for improvements, please create a new issue in the project
