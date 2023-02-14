# BAFL app

This is the Bay Area Football League (BAFL) official app. This allows members and parents to have quick access to important information & tools to simplify the experience and operations.

This is a mobile application targeted at iOS and Android, built on .Net MAUI for cross-platform application development. This is a single code base that targets all platforms. This is available as open source for anyone to download and propose fixes & enhancements.

This was initially developed and currently moderated by Adam Jordan (@xylamic).

# Architecture

The architecture of the application combines the mobile frontend with Azure hosting on the backend. These Azure hosting serves two types of content:

- Semi-static content, such as the schedule and board members
- Dynamic content, specifically real-time updates for events like cheer competition

![Architecture](docs/Architecture.jpg)

The Azure hosting is fully implemented as Azure Functions directly in the Azure portal (no outside code required).

# Contributions and Moderation
