# Server Parser

This project is designed to parse JSON data and identify **rented** servers that are available for purchase. It includes an SMTP email notification feature to alert users when a suitable server is found.

---

<p align="center">
  <img src="https://github.com/user-attachments/assets/8420f4c5-8997-409b-93b7-314e95d73644" alt="Result screenshot" />
</p>

---


## Features
- **JSON Parsing**: Fetches server data from a specified URL.
- **Filters**: Detects servers based on price and specific conditions.
- **Email Notifications**: Sends an email when a matching server is found.

## How It Works
1. The application retrieves server data from a remote JSON file.
2. It filters servers that meet the criteria:
   - Price below a defined threshold.
   - Includes specific attributes (e.g., hardware specials).
3. Sends an email with server details using an SMTP client.




