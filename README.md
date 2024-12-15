# Mini AirBnb Management API

[YouTube Link - Midterm](https://www.youtube.com/watch?v=4k15DfdxaeM)  
[YouTube Link - Assignment 2](https://www.youtube.com/watch?v=1F8bziPUl7U) 

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

### Assumptions:
- Users are categorized as **Host**, **Guest**, or **Admin** with appropriate roles and permissions.
- Booking conflicts are handled at the database query level, ensuring no double bookings.

---

## Issues Encountered

- **Authentication Challenges:** Integrating OAuth 2.0 required configuring Azure Active Directory and fine-tuning tokens, but it remains incomplete.

---

## Entity-Relationship (ER) Diagram

Below is the Entity-Relationship model for the database:

### **Listings**
- **Attributes:**
  - `ListingID` (Primary Key)
  - `NoOfPeople` (Number of people the listing can accommodate)
  - `Country` (Location country)
  - `City` (Location city)
  - `Price` (Price per stay)
  - `Rating` (Optional, average rating for the listing)

- **Relationships:**
  - `Listings` have a **one-to-many relationship** with **Bookings** (Each listing can have multiple bookings).

### **Bookings**
- **Attributes:**
  - `BookingID` (Primary Key)
  - `ListingID` (Foreign Key referencing `Listings.ListingID`)
  - `StartDate` (Start date of the booking)
  - `EndDate` (End date of the booking)
  - `NoOfPeople` (Number of people booking the listing)

- **Relationships:**
  - `Bookings` have a **many-to-one relationship** with **Listings** (Each booking belongs to a single listing).
  - `Bookings` have a **one-to-one relationship** with **Reviews** (A booking can have only one review).

### **Reviews**
- **Attributes:**
  - `ReviewID` (Primary Key)
  - `BookingID` (Foreign Key referencing `Bookings.BookingID`)
  - `Rating` (Rating given by the guest, between 1 and 5)
  - `Comment` (Optional comment by the guest)

- **Relationships:**
  - `Reviews` have a **many-to-one relationship** with **Bookings** (Each review is linked to a single booking).

### Summary of Relationships
- `Listings` ↔ `Bookings` (One-to-Many)
- `Bookings` ↔ `Reviews` (One-to-One)

The relationships ensure data integrity and enforce constraints for seamless data handling.

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

---

## Updates for Assignment 2

### Implemented API Gateway Pattern
- **Gateway Design:** A gateway was implemented to centralize and streamline communication between the client and the backend services. This API Gateway aggregates three critical endpoints:
  - Listing retrieval.
  - Booking creation.
  - Review submission.
- **Purpose:** The gateway improves scalability, reduces client-side complexity, and provides a single entry point for API consumers.
- **Example:** The gateway routes requests based on the endpoint path and method type while ensuring security checks.

### Message-Driven Architecture with RabbitMQ
- **Payment Processing Workflow:** A RabbitMQ-based message queue system was implemented to handle a three-step payment process:
  1. **Payment Queue:** Payments are sent to the queue with details like user email, payment type, and card number.
  2. **Processing Service:** The payment is consumed from the queue, validated, and marked as completed.
  3. **Notification Queue:** A success notification is pushed to another queue for delivery to the user via email.
- **Design Assumptions:**
  - Payments are processed asynchronously to avoid blocking other operations.
  - Notifications are decoupled from the payment process for better scalability.
- **Example Payload:**
  ```json
  {
      "user": "ali@gmail.com",
      "paymentType": "credit",
      "cardNo": "1234123412341234"
  }
