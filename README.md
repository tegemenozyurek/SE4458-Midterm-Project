# Mini AirBnb Management API

[Youtube Link](https://www.youtube.com/watch?v=4k15DfdxaeM)  
[GitHub Repository](https://github.com/tegemenozyurek/SE4458-Midterm-Project.git)
---

This API is designed for managing short-term accommodations. It provides functionality for **hosts**, **guests**, and **administrators** to manage listings, book stays, leave reviews, and generate reports. Built using ASP.NET Core, the API supports versioned endpoints, making it ideal for mobile and web-based vacation rental applications.

---

## Features

### Host
- **Insert Listing:** Hosts can create new listings with details such as country, city, price, and the number of people it accommodates.

### Guest
- **Query Listings:** Guests can search for listings by filters like dates, number of people, and location. Listings already booked are excluded.
- **Book a Stay:** Guests can book available listings for specified dates.
- **Review a Stay:** Guests who booked a stay can leave a review with a rating and a comment.

### Admin
- **Report Listings:** Administrators can generate reports filtered by country, city, and rating.

---

## Design and Assumptions

### Design
1. **Authentication:** OAuth 2.0 authentication was planned but is not yet fully implemented due to time constraints.
2. **API Versioning:** The API supports versioned endpoints for compatibility and flexibility.
3. **Database:** MySQL is used for data storage, ensuring scalability and reliability.
4. **Paging:** All endpoints returning lists support paging for efficient data handling.
5. **Swagger Documentation:** The API includes integrated Swagger for easy testing and documentation.

### Assumptions:
- Users are categorized as **Host**, **Guest**, or **Admin** with appropriate roles and permissions.
- Booking conflicts are handled at the database query level, ensuring no double bookings.

---

## Issues Encountered

- **Authentication Challenges:** Integrating OAuth 2.0 required configuring Azure Active Directory and fine-tuning tokens, but it remains incomplete.

---

## Database Schema

The API uses a **MySQL database**. Below is the schema:

```sql
CREATE DATABASE dbSE4458;
USE dbSE4458;

CREATE TABLE Listings (
    ListingID INT AUTO_INCREMENT PRIMARY KEY,
    NoOfPeople INT NOT NULL,
    Country VARCHAR(50) NOT NULL,
    City VARCHAR(50) NOT NULL,
    Price DECIMAL(10, 2) NOT NULL,
    Rating FLOAT DEFAULT NULL
);

CREATE TABLE Bookings (
    BookingID INT AUTO_INCREMENT PRIMARY KEY,
    ListingID INT NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    NoOfPeople INT NOT NULL,
    FOREIGN KEY (ListingID) REFERENCES Listings(ListingID)
);

CREATE TABLE Reviews (
    ReviewID INT AUTO_INCREMENT PRIMARY KEY,
    BookingID INT NOT NULL,
    Rating FLOAT NOT NULL CHECK (Rating BETWEEN 1 AND 5),
    Comment TEXT,
    FOREIGN KEY (BookingID) REFERENCES Bookings(BookingID)
);
