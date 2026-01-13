# Reminder task
## Steps to Start the API

I implemented email sending using SMTP(gmail) 
and to enable it user needs to configure appsettings.json (I didn't push it to repo):

"EmailSettings": {
  "SmtpHost": "smtp.gmail.com",
  "SmtpPort": 587,
  "FromEmail": "example@gmail.com",
  "FromName": "Reminder Service",
  "Username": "example@gmail.com",
  "Password": "App password", (not real gmail account password)
  "UseSsl": true
}

In this project I use Entity Framework Core with SQLite as a local database.
The connection string should be set:

"ConnectionStrings": {
  "DefaultConnection": "Data Source=reminders.db"
}

## Overview:

A layered architecture is used to separate API, application logic and infrastructure.
I did not implement a repository layer because EF Core already serves that purpose for this mostly CRUD application.
Initially, I didn't add the service layer, but I added it later to separate business logic from controllers. 
Reminders are processed with background service to avoid blocking HTTP requests.
Implemented strategy pattern to allow sending notification by SMS or other channel.
Global exception handling middleware and logging middleware are implemented and in the future I will extend logging middleware to save logs to database.
I convert all scheduled times to UTC to avoid timezone issues.
