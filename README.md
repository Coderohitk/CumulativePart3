# Cumulative Part 3: Teacher, Student, and Course Management System

## Overview

This project is the third part of the Cumulative Assessment for Back-End Web Development. The goal is to build a web-based Teacher, Student, and Course Management System using ASP.NET Core and MySQL. This system now includes functionality to update teacher, student, and course information, along with error handling for invalid input cases.

## Features

### Core Functionality
- **View Teachers**: Displays a list of teachers and their details.
- **View Students**: Displays a list of students and their details.
- **View Courses**: Displays a list of courses and their details.
- **Insert Teachers**: Allows adding new teachers to the system.
- **Insert Students**: Allows adding new students to the system.
- **Insert Courses**: Allows adding new courses to the system.
- **Delete Teachers**: Allows deleting teachers from the system.
- **Delete Students**: Allows deleting students from the system.
- **Delete Courses**: Allows deleting courses from the system.
- **Update Teachers**: Allows updating existing teacher information.
- **Update Students**: Allows updating existing student information.
- **Update Courses**: Allows updating existing course information.

### Enhanced Error Handling
- **Empty Fields**:
  - Teacher Name, Student Name, and Course Name cannot be empty.
- **Invalid Dates**:
  - Start Date and Finish Date cannot be in the future.


## Technologies Used

- **ASP.NET Core**: For building the web application and API.
- **MySQL**: For database management and storage.
- **HTML/CSS**: For building the front-end of the application.
- **Razor Views**: For dynamically generating HTML pages based on model data.

## Setup

To run this project locally, follow the steps below:

### Prerequisites

- .NET 6.0 SDK or higher
- MySQL Server
- A suitable IDE (Visual Studio or VS Code)

### Clone the Repository

```bash
git clone https://github.com/Coderohitk/CumulativePart3.git
```

### Configure the Database

1. Create a MySQL database for the project.
2. Update the `appsettings.json` file with your database connection details.
3. Use the provided SQL script to create the required tables: Teachers, Students, and Courses.

### Run the Application

1. Open the project in Visual Studio.
2. Restore NuGet packages.
3. Run the application using IIS Express or Kestrel.



## Testing

### API Testing

- Verify the HTTP PUT methods for updating teachers, students, and courses using tools like Postman or cURL.
- Include the following test cases:
  - Update with valid data.
  - Attempt to update with empty names.
  - Attempt to update with future dates.

### UI Testing

- Testing the web pages for updating teachers, students, and courses by manually entering valid and invalid data.
- Ensuring appropriate error messages are displayed for invalid input.

## Error Handling Examples

### Server-Side Validation
- Reject updates for:
  - Empty Teacher Name, Student Name, or Course Name.
  - Future Start Date or Finish Date.
  - Negative Salary for teachers.

### Client-Side Validation
- Display error messages when:
  - Required fields are left empty.
  - Dates are in the future.

